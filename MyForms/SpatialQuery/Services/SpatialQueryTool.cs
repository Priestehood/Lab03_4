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
        private bool _isBufferQueryActive = false;

     
        public bool IsBufferQueryActive => _isBufferQueryActive;

        public bool IsElementQueryActive => _isElementQueryActive; // 点击查询模式状态
        public bool IsDrawPolylineActive => _isDrawPolylineActive; // 绘制多义线模式状态

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
            // 如果已经找到则直接返回 true
            if (_buildingLayer != null && _roadLayer != null) return true;

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

        // ================================================================
        // 实验步骤 3：图上点击查询建筑/道路
        // ================================================================
        public void MenuClick_ElementQuery()
        {

            _isElementQueryActive = true; // 激活标记
            _isDrawPolylineActive = false; // 互斥

            MessageBox.Show("🔍 现在请点击地图上的建筑或道路进行查询。\n右键取消。");
            _axMap.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            _axMap.OnMouseDown -= Map_OnClick_Query;
            _axMap.OnMouseDown += Map_OnClick_Query;

           
        }

        private void Map_OnClick_Query(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button != 1) return; // 左键生效

            IPoint clickPoint = new PointClass();
            clickPoint.PutCoords(e.mapX, e.mapY);

            // 1）点状判断建筑物
            IFeature building = QueryFeatureByPoint(_buildingLayer, clickPoint);
            if (building != null)
            {
                ShowFeatureInfo(building, "建筑");
                _axMap.OnMouseDown -= Map_OnClick_Query;
                return;
            }

            // 2）道路使用缓冲区查询
            double bufferRadius = 5.0;
            IGeometry buffer = ((ITopologicalOperator)clickPoint).Buffer(bufferRadius);

            IFeature road = QueryFeatureByGeometry(_roadLayer, buffer);
            if (road != null)
            {
                ShowFeatureInfo(road, "道路");
                _axMap.OnMouseDown -= Map_OnClick_Query;
                return;
            }

            MessageBox.Show("⚠ 未点击到任何建筑或道路，请重试。");

            _axMap.OnMouseDown -= Map_OnClick_Query;
            _isElementQueryActive = false; // 重置标记
        }

        private IFeature QueryFeatureByPoint(IFeatureLayer layer, IPoint pt)
        {
            ISpatialFilter filter = new SpatialFilterClass();
            filter.Geometry = pt;
            filter.GeometryField = layer.FeatureClass.ShapeFieldName;
            filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            return layer.FeatureClass.Search(filter, false).NextFeature();
        }

        private IFeature QueryFeatureByGeometry(IFeatureLayer layer, IGeometry geometry)
        {
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

        // ================================================================
        // 实验步骤 4：绘制多义线 + 缓冲区相交建筑计算
        // ================================================================
        public void MenuClick_DrawPolyline()
        {
            _isDrawPolylineActive = true;
            _isElementQueryActive = false;

            MessageBox.Show("📌 请左键依次点击绘制多义线，右键结束绘制。");
            _points.Clear();

            _axMap.MousePointer = esriControlsMousePointer.esriPointerCrosshair;

            // ❗关键：先解绑避免重复绑定
            _axMap.OnMouseDown -= Map_OnDrawPolyline;
            // ❗再绑定事件
            _axMap.OnMouseDown += Map_OnDrawPolyline;


        }

        private void Map_OnDrawPolyline(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            IPoint pt = new PointClass();
            pt.PutCoords(e.mapX, e.mapY);

            if (e.button == 1)
            {
                _points.Add(pt);
            }
            else if (e.button == 2) // 右键结束绘制
            {
                // 1. 重置绘制模式标记（关键！）
                _isDrawPolylineActive = false;

                // 2. 原有绘制逻辑
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

                // 3. 恢复鼠标样式
                _axMap.OnMouseDown -= Map_OnDrawPolyline;
                _axMap.MousePointer = esriControlsMousePointer.esriPointerDefault;
                // 4. 执行缓冲分析
                MenuClick_BufferAnalysis();

                
              
            }
          
        }

        public void MenuClick_BufferAnalysis()
        {
            _isBufferQueryActive = true; // 标记缓冲查询激活

            if (_drawnPolyline == null)
            {
                _isBufferQueryActive = false; // 未就绪则标记为未激活
                MessageBox.Show("⚠ 请先绘制多义线再执行缓冲分析。");
                return;
            }


            double bufferDistance = 5.0; // 单位：地图坐标系单位
            IGeometry buffer = ((ITopologicalOperator)_drawnPolyline).Buffer(bufferDistance);

            List<string> result = new List<string>();

            IFeatureCursor cursor = _buildingLayer.FeatureClass.Search(null, false);
            IFeature feature;

            while ((feature = cursor.NextFeature()) != null)
            {
                IRelationalOperator rel = feature.Shape as IRelationalOperator;

                // ArcEngine 判断相交方式：!Disjoint()
                if (rel != null && !rel.Disjoint(buffer))
                {
                    string name = feature.Fields.FindField("Name") >= 0
                        ? feature.get_Value(feature.Fields.FindField("Name")).ToString()
                        : "无名称";

                    double area = ((IArea)feature.Shape).Area;
                    result.Add($"ID:{feature.OID} | 名称:{name} | 面积:{area:F2}");
                }
            }

            if (result.Count == 0)
                MessageBox.Show("🔎 缓冲区范围内没有建筑。");
            else
                MessageBox.Show(string.Join("\n", result), "📌 缓冲区相交建筑列表");
        }

        public void ToggleBufferQueryMode(bool isEnable)
        {
            // 逻辑：启用时检查是否已绘制多义线，未绘制则提示；禁用时清空多义线
            if (isEnable)
            {
                if (_drawnPolyline == null)
                    MessageBox.Show("请先绘制多义线再启用缓冲查询！");
            }
            else
            {
                _drawnPolyline = null; // 清空已绘制的多义线，避免误触发
            }
        }

        // ================================================================
        // 新增：模式切换方法（供Form4调用，实现互斥）
        // ================================================================
        /// <summary>
        /// 启用/禁用「点击查询建筑/道路」模式（对应实验步骤3）
        /// </summary>
        public void ToggleElementQueryMode(bool isEnable)
        {
            if (isEnable)
            {
                // 启用：执行你原有的点击查询初始化逻辑
                if (EnsureLayersAssigned(false)) // 检测图层，未就绪则不启用
                {
                    MenuClick_ElementQuery();
                }
            }
            else
            {
                // 禁用：取消鼠标事件绑定，恢复鼠标样式
                _axMap.OnMouseDown -= Map_OnClick_Query;
                _axMap.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
            _isBufferQueryActive = false; // 分析完成后重置状态
        }

        /// <summary>
        /// 启用/禁用「绘制多义线」模式（对应实验步骤4）
        /// </summary>
        public void ToggleDrawPolylineMode(bool isEnable)
        {
            if (isEnable)
            {
                // 启用：执行你原有的绘制多义线初始化逻辑
                if (EnsureLayersAssigned(false)) // 检测图层，未就绪则不启用
                {
                    MenuClick_DrawPolyline();
                }
            }
            else
            {
                // 禁用：取消鼠标事件绑定，恢复鼠标样式，清空绘制数据
                _axMap.OnMouseDown -= Map_OnDrawPolyline;
                _axMap.MousePointer = esriControlsMousePointer.esriPointerDefault;
                _points.Clear();
                _drawnPolyline = null;
            }
        }

        /// <summary>
        /// 统一鼠标事件入口（供Form4调用，分发到对应功能）
        /// </summary>
        public void OnMapMouseDown(IMapControlEvents2_OnMouseDownEvent e)
        {
            // 如果是点击查询模式，执行查询逻辑
            if (_isElementQueryActive) // 需新增一个标记变量
            {
                Map_OnClick_Query(null, e);
            }
            // 如果是绘制多义线模式，执行绘制逻辑
            else if (_isDrawPolylineActive) // 需新增一个标记变量
            {
                Map_OnDrawPolyline(null, e);
            }
        }

        // 同时在 SpatialQueryTool 类中新增两个模式标记变量（放在类顶部）：
        private bool _isElementQueryActive = false; // 点击查询激活标记
        private bool _isDrawPolylineActive = false; // 绘制多义线激活标记
    }
}






