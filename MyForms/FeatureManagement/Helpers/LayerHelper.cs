using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab03_4.MyForms.FeatureManagement.Helpers
{
    class LayerHelper
    {
        public enum DataSourceType
        {
            Default,
            ShapeFile,
            // TODO: more?
        }

        /// <summary>
        /// 获取数据源的完整路径
        /// </summary>
        /// <param name="layer">矢量图层</param>
        public static string GetDataSource(IFeatureLayer layer)
        {
            try
            {
                DataSourceType dataSourceType = GetDataSourceType(layer.DataSourceType);

                string featureName, dataSource;
                GetDataSource(layer.FeatureClass, out featureName, out dataSource);
                string path = Path.Combine(dataSource, featureName);
                if (dataSourceType == DataSourceType.ShapeFile)
                    path += ".shp";

                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取要素类的数据源的目录和名称
        /// </summary>
        /// <param name="featureClass">要素类</param>
        /// <param name="featureName">要素类（shp文件）名称</param>
        /// <param name="dataSource">数据源（数据目录）</param>
        private static void GetDataSource(IFeatureClass featureClass,
            out string featureName,
            out string dataSource)
        {
            try
            {
                IDataset dataSet = (IDataset)featureClass;
                featureName = dataSet.Name;
                object names, values;

                IPropertySet propertySet = dataSet.Workspace.ConnectionProperties;
                propertySet.GetAllProperties(out names, out values);
                dataSource = ((object[])values)[0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取数据源类型
        /// </summary>
        /// <param name="dataSourceType">数据源类型</param>
        /// <returns>数据源类型</returns>
        public static DataSourceType GetDataSourceType(string dataSourceType)
        {
            switch (dataSourceType)
            {
                case "Shapefile Feature Class":
                    return DataSourceType.ShapeFile;
                case "Personal Geodatabase Feature Class":
                case "SDE Feature Class":
                case "Annotation Feature Class":
                case "Point Feature Class":
                case "Arc Feature Class":
                case "Polygon Feature Class":
                case "StreetMap Feature Class":
                case "CAD Annotation Feature Class":
                case "CAD Point Feature Class":
                case "CAD Polyline Feature Class":
                case "CAD Polygon Feature Class":
                default:
                    return default;
            }
        }
    }
}
