using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.IO;
using System.Windows.Forms;
using Lab03_4.MyForms.FeatureManagement.Services;
using Lab03_4.MyForms.FeatureManagement.Forms;
using System.Data;
using ESRI.ArcGIS.esriSystem;

namespace Lab03_4
{
    /// <summary>
    /// 地图操作枚举类型
    /// </summary>
    public enum MapOperationType
    {
        /// <summary>
        /// 默认（无操作）
        /// </summary>
        Default,

        /// <summary>
        /// 从地图上创建要素
        /// </summary>
        CreateFeature,

        /// <summary>
        /// 编辑要素
        /// </summary>
        EditFeature,

        /// <summary>
        /// 删除要素
        /// </summary>
        DeleteFeature,

        /// <summary>
        /// 选择要素
        /// </summary>
        SelectFeature,

        /// <summary>
        /// 标识/显示要素信息
        /// </summary>
        IdentifyFeature,
    }

    public partial class Form_4 : Form
    {

        #region 私有变量
        private MapOperationType mapOperation = MapOperationType.Default;
        private ShapeSketcher sketcher = new ShapeSketcher();
        #endregion

        /// <summary>
        /// 开始创建新要素
        /// </summary>
        private void BeginCreateNewFeature()
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "添加要素")) return;

            IFeatureLayer featureLayer = selectedLayer as IFeatureLayer;
            esriGeometryType geometryType = featureLayer.FeatureClass.ShapeType;
            switch (geometryType)
            {
                case esriGeometryType.esriGeometryPoint:
                    sketcher.shape = Shape.Point;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    sketcher.shape = Shape.Polyline;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    sketcher.shape = Shape.Polygon;
                    break;
                default:
                    MessageBox.Show(string.Format("要素类几何类型为{}", geometryType.ToString()));
                    break;
            }

            mapOperation = MapOperationType.CreateFeature;
        }

        /// <summary>
        /// 根据绘制的geometry，创建新要素，弹窗确认后设置形状和属性并保存
        /// </summary>
        /// <param name="geometry"></param>
        private void CreateNewFeature(IGeometry geometry)
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "添加要素")) return;

            // 选定图层的要素类
            IFeatureLayer featureLayer = selectedLayer as IFeatureLayer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            // 创建新的空白要素
            IFeature feature = featureClass.CreateFeature();

            // 等待用户修改属性并确认
            FormNewFeature frm = new FormNewFeature(selectedLayer as IFeatureLayer);
            while (true)
            {
                DialogResult result = frm.ShowDialog();
                if (result != DialogResult.OK) return;

                try
                {
                    // 设置形状
                    SetZValue(feature, geometry);
                    feature.Shape = geometry;
                    // 设置属性
                    frm.SetFeatureFields(feature);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("[异常] " + ex.Message, "错误",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Error);
                    continue;
                }

                // 保存修改
                feature.Store();
                break;
            }

            // 刷新地图
            axMap.Refresh();
            axMap.Update();
        }

        /// <summary>
        /// 设置选择要素时所绘制的图形，可以是点选/矩形/多边形
        /// </summary>
        /// <param name="by">选择要素所绘图形类型的字符串，Location/Rectangle/Polygon</param>
        private void BeginSelectFeature(string by)
        {
            switch (by)
            {
                case "Location":
                    sketcher.shape = Shape.Point;
                    break;
                case "Rectangle":
                    sketcher.shape = Shape.Rectangle;
                    break;
                case "Polygon":
                    sketcher.shape = Shape.Polygon;
                    break;
            }
        }

        /// <summary>
        /// 开始编辑要素
        /// </summary>
        /// <param name="by">选择要素所绘图形类型的字符串</param>
        private void BeginEditFeature(string by)
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "编辑要素")) return;

            BeginSelectFeature(by);
            mapOperation = MapOperationType.EditFeature;
        }

        /// <summary>
        /// 根据绘制的geometry，过滤出选定图层中和geometry相交的要素
        /// </summary>
        /// <param name="filterGeometry">绘制的geometry</param>
        /// <param name="isUpdate">是否返回更新游标，而不是选择游标</param>
        /// <returns>和绘制geometry相交的所有要素的游标</returns>
        private IFeatureCursor GetFilteredFeatures(IGeometry filterGeometry,
            bool isUpdate = true)
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "选择要素")) return null;
            IFeatureLayer layer = selectedLayer as IFeatureLayer;

            ISpatialFilter filter = new SpatialFilterClass();
            filter.Geometry = filterGeometry;
            filter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor cursor;
            if (isUpdate)
            {
                cursor = layer.FeatureClass.Update(filter, true);
            }
            else
            {
                cursor = layer.FeatureClass.Search(filter, true);
            }
            return cursor;
        }
        
        /// <summary>
        /// 使用IIdentify获取点选要素
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private IFeature GetFeatureByPointUsingIdentify(IPoint point)
        {
            IFeatureLayer layer = GetSelectedLayer() as IFeatureLayer;
            IIdentify identifyLayer = (IIdentify)layer;

            // 检查点的空间参考系是否与选中图层一致
            IGeoDataset dataset = layer.FeatureClass as IGeoDataset;
            if (point.SpatialReference != dataset.SpatialReference)
            {
                point.Project(dataset.SpatialReference);
            }

            IArray array = identifyLayer.Identify(point);
            if (array is null) return null;

            object obj = array.get_Element(0);
            IFeatureIdentifyObj fobj = obj as IFeatureIdentifyObj;
            IRowIdentifyObject irow = fobj as IRowIdentifyObject;
            //获取选中的要素
            IFeature feature = irow.Row as IFeature;
            return feature;
        }

        private void UpdateMap()
        {
            axMap.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            axMap.Refresh();
            axMap.Update();
        }

        /// <summary>
        /// （在地图中）选中符合条件的首个要素
        /// </summary>
        /// <param name="filterGeometry">绘制的geometry</param>
        /// <returns>首个要素</returns>
        private IFeature SelectFirstFeature(IGeometry filterGeometry)
        {
            IFeature feature;
            if (filterGeometry is IPoint)
            {
                feature = GetFeatureByPointUsingIdentify(filterGeometry as IPoint);
            }
            else
            {
                IFeatureCursor cursor = GetFilteredFeatures(filterGeometry);
                // 选中首个要素
                feature = cursor.NextFeature();
            }
            // 选中要素
            axMap.Map.SelectFeature(GetSelectedLayer(), feature);

            // 刷新地图
            UpdateMap();

            // 返回
            return feature;
        }

        /// <summary>
        /// （在地图中）选中符合条件的所有要素
        /// </summary>
        /// <param name="filterGeometry">绘制的geometry</param>
        /// <returns>所有要素的更新游标</returns>
        private IFeatureCursor SelectAllFeatures(IGeometry filterGeometry)
        {
            IFeatureCursor cursor = GetFilteredFeatures(filterGeometry);
            // 选中所有要素
            IFeature feature;
            while ((feature = cursor.NextFeature()) != null)
            {
                axMap.Map.SelectFeature(GetSelectedLayer(), feature);
            }

            // 刷新地图
            UpdateMap();

            // 返回更新游标
            cursor = GetFilteredFeatures(filterGeometry, true);
            return cursor;
        }

        /// <summary>
        /// 编辑要素
        /// </summary>
        /// <param name="feature">待编辑要素</param>
        private void EditFeature(IFeature feature)
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "编辑要素")) return;

            // 等待用户修改属性并确认
            FormEditFeature frm = new FormEditFeature(feature);
            while (true)
            {
                DialogResult result = frm.ShowDialog();
                if (result != DialogResult.OK) return;

                try
                {
                    // 设置属性
                    frm.SetFeatureFields(feature);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("[异常] " + ex.Message, "错误",
                         MessageBoxButtons.OK,
                         MessageBoxIcon.Error);
                    continue;
                }

                // 保存修改
                feature.Store();
                break;
            }

            // 刷新地图
            UpdateMap();
        }

        /// <summary>
        /// 开始删除要素
        /// </summary>
        /// <param name="by">选择要素所绘图形类型的字符串</param>
        private void BeginDeleteFeature(string by)
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "删除要素")) return;

            BeginSelectFeature(by);
            mapOperation = MapOperationType.DeleteFeature;
        }

        /// <summary>
        /// 弹出提示，确认后删除要素
        /// </summary>
        /// <param name="features">待删除的要素或更新游标</param>
        private void DeleteFeatures(object features)
        {
            DialogResult dialogResult =
                MessageBox.Show("是否要删除选中要素？该操作不可撤回", "删除要素", MessageBoxButtons.OKCancel);
            if (dialogResult != DialogResult.OK) return;

            if (features is IFeatureCursor)
            {
                IFeatureCursor cursor = features as IFeatureCursor;
                while (cursor.NextFeature() != null)
                {
                    cursor.DeleteFeature();
                }
            }
            else if (features is IFeature)
            {
                IFeature feature = features as IFeature;
                feature.Delete();
            }

            // 刷新地图
            UpdateMap();
        }

        /// <summary>
        /// 浏览选中要素类的所有要素的信息
        /// </summary>
        private void BrowseFeatures()
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "要素信息")) return;

            // 调用工具类转换数据
            DataTable dt = FeatureHelper.ToDataTable((selectedLayer as IFeatureLayer).FeatureClass);
            // 打开新增的浏览窗体
            FormBrowseFeatures frm = new FormBrowseFeatures();
            frm.dgvFeatures.DataSource = dt;
            frm.ShowDialog();
        }

        /// <summary>
        /// 开始选择待查看要素
        /// </summary>
        private void BeginIdentifyFeature()
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "要素信息")) return;

            BeginSelectFeature("Location");
            mapOperation = MapOperationType.IdentifyFeature;
        }

        /// <summary>
        /// 查看要素信息
        /// </summary>
        /// <param name="feature">选中的要素</param>
        private void IdentifyFeature(IFeature feature)
        {
            FormIdentifyFeature frm = new FormIdentifyFeature(feature);
            frm.ShowDialog();
        }

        /// <summary>
        /// 为geometry设置Z值和M值，
        /// 修复The Geometry has no Z values的异常
        /// </summary>
        /// <param name="feature">新要素</param>
        /// <param name="geometry">绘制的几何图形</param>
        private void SetZValue(IFeature feature, IGeometry geometry)
        {
            int fieldIndex = feature.Fields.FindField("Shape");
            IGeometryDef geometryDef = feature.Fields.Field[fieldIndex].GeometryDef;
            IZAware zAware = geometry as IZAware;

            if (geometryDef.HasZ)
            {
                zAware.ZAware = true;
                IZ iz = geometry as IZ;
                iz.SetConstantZ(0);
            }
            else
            {
                zAware.ZAware = false;
            }
        }
    }
}