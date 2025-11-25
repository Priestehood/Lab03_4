using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04_4.MyForms.SpatialQuery.Helpers
{
    /// <summary>
    /// 坐标系辅助类
    /// </summary>
    public static class CoordinateSystem
    {
        /// <summary>
        /// 检查是否为地理坐标系
        /// </summary>
        public static bool IsGeographicCoordinateSystem(IFeatureClass featureClass)
        {
            try
            {
                IGeoDataset geoDataset = featureClass as IGeoDataset;
                return geoDataset?.SpatialReference != null &&
                       IsGeographicCoordinateSystem(geoDataset.SpatialReference);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为地理坐标系
        /// </summary>
        public static bool IsGeographicCoordinateSystem(ISpatialReference spatialReference)
        {
            try
            {
                return spatialReference is IGeographicCoordinateSystem;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 显示坐标系警告
        /// </summary>
        public static void ShowCoordinateSystemWarning(bool isGeographic)
        {
            if (isGeographic)
            {
                MessageBox.Show("警告：当前图层使用地理坐标系，面积计算将自动转换为平方米。",
                    "坐标系提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
