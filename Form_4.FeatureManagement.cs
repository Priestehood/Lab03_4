using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.IO;
using System.Windows.Forms;
using Lab03_4.MyForms.FeatureManagement.Services;

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

        private void CreateNewFeature(IGeometry geometry)
        {
            var selectedLayer = GetSelectedLayer();
            if (!ValidateFeatureLayer(selectedLayer, "添加要素")) return;

            // 等待用户修改属性并确认
            //MyForms.FormNewFeature frm = new MyForms.FormNewFeature(this.selectedLayer as IFeatureLayer, geom);
            //frm.ShowDialog();

            // 添加到选定图层的要素类
            IFeatureLayer featureLayer = selectedLayer as IFeatureLayer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            IFeature feature = featureClass.CreateFeature();
            // 设置形状
            feature.Shape = geometry;
            // 设置属性
            //feature.Value[featureClass.FindField("Name")] =
            //    string.Format("Point_{0}", featureClass.FeatureCount(null));
            // 保存修改
            feature.Store();
        }
    }
}