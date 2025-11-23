using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.IO;
using System.Windows.Forms;
using Lab03_4.MyForms.FeatureManagement.Services;
using Lab03_4.MyForms.FeatureManagement.Forms;

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
            FormNewFeature frm = new FormNewFeature(selectedLayer as IFeatureLayer, geometry);
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
                    frm.SetFeatureFields(feature, featureClass);
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
        /// （在地图中）选中符合条件的首个要素
        /// </summary>
        /// <param name="filterGeometry">绘制的geometry</param>
        /// <returns>首个要素</returns>
        private IFeature SelectFirstFeature(IGeometry filterGeometry)
        {
            IFeatureCursor cursor = GetFilteredFeatures(filterGeometry);
            // 选中首个要素
            IFeature feature = cursor.NextFeature();
            axMap.Map.SelectFeature(GetSelectedLayer(), feature);

            // 刷新地图
            axMap.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            axMap.Refresh();
            axMap.Update();

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
            axMap.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            axMap.Refresh();
            axMap.Update();

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
            //MyForms.FormEditFeature frm = new MyForms.FormEditFeature(((IFeatureLayer)this.selectedLayer).FeatureClass, feature);
            //frm.ShowDialog();
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
        /// <param name="features">待删除要素的更新游标</param>
        private void DeleteFeatures(IFeatureCursor features)
        {
            DialogResult dialogResult =
                MessageBox.Show("是否要删除选中要素？该操作不可撤回", "删除要素", MessageBoxButtons.OKCancel);
            if (dialogResult != DialogResult.OK) return;

            while (features.NextFeature() != null)
            {
                features.DeleteFeature();
            }

            // 刷新地图
            axMap.Refresh();
            axMap.Update();
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