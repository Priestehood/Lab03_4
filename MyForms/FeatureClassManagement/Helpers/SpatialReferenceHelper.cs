using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.FeatureClassManagement.Helpers
{
    /// <summary>
    /// 空间参考系统 | 工具类
    /// </summary>
    public static class SpatialReferenceHelper
    {
        /// <summary>
        /// 预定义的坐标系选项
        /// </summary>
        public static readonly Dictionary<string, int> PredefinedSpatialReferences = new Dictionary<string, int>
        {
            { "EPSG:4326 (WGS84)", 4326 },
            { "EPSG:3857 (Web Mercator)", 3857 },
            { "EPSG:4490 (CGCS2000)", 4490 },
            { "EPSG:4547 (CGCS2000/3-degree Gauss-Kruger CM 114E)", 4547 }
        };

        /// <summary>
        /// 根据工厂代码创建空间参考
        /// </summary>
        public static ISpatialReference CreateSpatialReferenceByFactoryCode(int factoryCode)
        {
            try
            {
                ISpatialReferenceFactory2 spatialReferenceFactory = new SpatialReferenceEnvironmentClass();

                // 判断是地理坐标系还是投影坐标系
                if (factoryCode == 4326 || factoryCode == 4490) // 地理坐标系
                {
                    return spatialReferenceFactory.CreateGeographicCoordinateSystem(factoryCode);
                }
                else // 投影坐标系
                {
                    return spatialReferenceFactory.CreateProjectedCoordinateSystem(factoryCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"创建空间参考失败 (EPSG:{factoryCode}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 根据显示名称获取工厂代码
        /// </summary>
        public static int? GetFactoryCodeFromDisplayName(string displayName)
        {
            if (PredefinedSpatialReferences.ContainsKey(displayName))
            {
                return PredefinedSpatialReferences[displayName];
            }
            return null;
        }

        /// <summary>
        /// 获取所有预定义坐标系的显示名称
        /// </summary>
        public static string[] GetPredefinedSpatialReferenceNames()
        {
            var names = new List<string>();
            foreach (var item in PredefinedSpatialReferences)
            {
                names.Add(item.Key);
            }
            names.Add("更多...");
            return names.ToArray();
        }

        /// <summary>
        /// 获取空间参考的显示名称
        /// </summary>
        public static string GetDisplayName(ISpatialReference spatialRef)
        {
            if (spatialRef == null) return "未知坐标系";

            try
            {
                int factoryCode = spatialRef.FactoryCode;

                foreach (var item in PredefinedSpatialReferences)
                {
                    if (item.Value == factoryCode)
                    {
                        return item.Key;
                    }
                }

                return $"EPSG:{factoryCode}";
            }
            catch
            {
                return "未知坐标系";
            }
        }
    }
}
