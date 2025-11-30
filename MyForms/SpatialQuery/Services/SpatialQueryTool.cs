using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace Lab04_4.MyForms.SpatialQuery.Services
{
    /// <summary>
    /// SpatialQueryTool 类实现实验任务 3 与 4：
    /// 1）鼠标点击查询建筑或道路信息；
    /// 2）绘制多义线，并计算缓冲区内与之相交建筑。
    /// </summary>
    public class SpatialQueryTool
    {
        private AxMapControl _axMap;
        private IFeatureLayer _buildingLayer;
        private IFeatureLayer _roadLayer;
        private IFeatureLayer _elevationLayer;

        // 用于多义线绘制
        private List<IPoint> _points = new List<IPoint>();
        private IPolyline _drawnPolyline;

        public SpatialQueryTool(AxMapControl mapControl)
        {
            _axMap = mapControl ?? throw new ArgumentNullException(nameof(mapControl));
        }

        /// <summary>
        /// 尝试分配图层。silent=true：静默（不弹窗）；silent=false：交互（发现没图层时弹窗提示并返回false）。
        /// </summary>
        public bool EnsureLayersAssigned(bool silent = true)
        {
            IFeatureLayer foundBuilding = null;
            IFeatureLayer foundRoad = null;
            IFeatureLayer foundElev = null;

            for (int i = 0; i < _axMap.LayerCount; i++)
            {
                ILayer layer = _axMap.get_Layer(i);
                if (layer is IFeatureLayer fl && fl.FeatureClass != null)
                {
                    switch (fl.FeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPolygon:
                            if (foundBuilding == null) foundBuilding = fl;
                            break;
                        case esriGeometryType.esriGeometryPolyline:
                            if (foundRoad == null) foundRoad = fl;
                            break;
                        case esriGeometryType.esriGeometryPoint:
                            if (foundElev == null) foundElev = fl;
                            break;
                    }
                }
            }

            _buildingLayer = foundBuilding ?? _buildingLayer;
            _roadLayer = foundRoad ?? _roadLayer;
            _elevationLayer = foundElev ?? _elevationLayer;

            bool ok = (_buildingLayer != null && _roadLayer != null)
                      || (_buildingLayer != null && _roadLayer != null); // 保证字段名正确

            // 修：不要在 silent 模式下弹窗；只有在用户触发工具时（silent=false）才提示
            if (!ok && !silent)
            {
                MessageBox.Show(
                    "⚠ 未检测到建筑（面）或道路（线）图层。\n请先加载至少一个面状建筑图层和一个线状道路图层，然后重试。",
                    "图层未就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return ok;
        }

        public SpatialQueryTool(AxMapControl axMap, IFeatureLayer buildingLayer, IFeatureLayer roadLayer)
        {
            _axMap = axMap;
            _buildingLayer = buildingLayer;
            _roadLayer = roadLayer;
        }

        #region 图上点击查询建筑/道路

        public void QueryElement(IPoint clickPoint)
        {
            // 1）点状判断建筑物
            IFeature building = QueryFeatureByPoint(_buildingLayer, clickPoint);
            if (building != null)
            {
                ShowFeatureInfo(building, "建筑");
                return;
            }

            // 2）道路使用缓冲区查询
            double bufferRadius = 5.0;
            IGeometry buffer = ((ITopologicalOperator)clickPoint).Buffer(bufferRadius);

            IFeature road = QueryFeatureByGeometry(_roadLayer, buffer);
            if (road != null)
            {
                ShowFeatureInfo(road, "道路");
                return;
            }

            MessageBox.Show("⚠ 未点击到任何建筑或道路，请重试。");
        }

        private IFeature QueryFeatureByPoint(IFeatureLayer layer, IPoint pt)
        {
            if (layer is null) return null;

            ISpatialFilter filter = new SpatialFilterClass();
            filter.Geometry = pt;
            filter.GeometryField = layer.FeatureClass.ShapeFieldName;
            filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            return layer.FeatureClass.Search(filter, false).NextFeature();
        }

        private IFeature QueryFeatureByGeometry(IFeatureLayer layer, IGeometry geometry)
        {
            if (layer is null) return null;

            ISpatialFilter filter = new SpatialFilterClass();
            filter.Geometry = geometry;
            filter.GeometryField = layer.FeatureClass.ShapeFieldName;
            filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            return layer.FeatureClass.Search(filter, false).NextFeature();
        }

        private void ShowFeatureInfo(IFeature feature, string featureType)
        {
            string name = feature.Fields.FindField("Name") >= 0
                ? feature.get_Value(feature.Fields.FindField("Name")).ToString()
                : "（无名称字段）";

            MessageBox.Show($"{featureType} 信息：\n\n📌 ID: {feature.OID}\n📌 名称: {name}", "查询结果");
        }

        #endregion

        #region 绘制多义线 + 缓冲区相交建筑计算

        public void ClearPoints()
        {
            _points.Clear();
        }

        public void AddPoint(IPoint pt)
        {
            _points.Add(pt);
        }

        public void FinishAddingPoints()
        {
            if (_points.Count < 2)
            {
                MessageBox.Show("⚠ 多义线至少需要两个点！");
                return;
            }

            IPolyline line = new PolylineClass();
            IPointCollection pc = line as IPointCollection;

            foreach (var p in _points) pc.AddPoint(p);

            _drawnPolyline = line;
            _points.Clear();

            // 进入缓冲分析步骤
            PerformBufferAnalysis();
        }

        public void FinishDrawingLine(IPolyline polyline)
        {
            _drawnPolyline = polyline;
            _points.Clear();

            // 进入缓冲分析步骤
            PerformBufferAnalysis();
        }

        public void PerformBufferAnalysis()
        {
            if (_drawnPolyline == null)
            {
                MessageBox.Show("⚠ 请先绘制多义线再执行缓冲分析。");
                return;
            }

            double bufferDistance = 5.0; // 单位：地图坐标系单位
            IGeometry buffer = ((ITopologicalOperator)_drawnPolyline).Buffer(bufferDistance);

            List<string> result = new List<string>();

            IFeatureCursor cursor =
                _buildingLayer.FeatureClass.Search(new SpatialFilter
                {
                    Geometry = buffer,
                    SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
                }, false);
            //IFeatureCursor cursor = _buildingLayer.FeatureClass.Search(null, false);
            IFeature feature;

            while ((feature = cursor.NextFeature()) != null)
            {
                //IRelationalOperator rel = feature.Shape as IRelationalOperator;

                ////ArcEngine 判断相交方式：!Disjoint()
                //if (rel != null && !rel.Disjoint(buffer))
                //{
                    string name = feature.Fields.FindField("Name") >= 0
                        ? feature.get_Value(feature.Fields.FindField("Name")).ToString()
                        : "无名称";

                    double area = ((IArea)feature.Shape).Area;
                    result.Add($"ID:{feature.OID} | 名称:{name} | 面积:{area:F2}");
                //}
            }

            if (result.Count == 0)
                MessageBox.Show("🔎 缓冲区范围内没有建筑。");
            else
                MessageBox.Show(string.Join("\n", result), "📌 缓冲区相交建筑列表");
        }

        #endregion
    }
}






