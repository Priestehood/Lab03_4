using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab03_4.MyForms.FeatureClassManagement.Helpers
{
    /// <summary>
    /// 几何类型 | 工具类
    /// </summary>
    public static class GeometryTypeHelper
    {
        /// <summary>
        /// 将中文字符串转换为几何类型
        /// </summary>
        public static esriGeometryType ConvertToGeometryType(string geometryType)
        {
            switch (geometryType?.ToLower())
            {
                case "点":
                    return esriGeometryType.esriGeometryPoint;
                case "线":
                    return esriGeometryType.esriGeometryPolyline;
                case "面":
                    return esriGeometryType.esriGeometryPolygon;
                default:
                    throw new System.ArgumentException($"不支持的几何类型: {geometryType}");
            }
        }

        /// <summary>
        /// 将几何类型转换为中文字符串
        /// </summary>
        public static string ConvertToChineseName(esriGeometryType geometryType)
        {
            switch (geometryType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "点";
                case esriGeometryType.esriGeometryPolyline:
                    return "线";
                case esriGeometryType.esriGeometryPolygon:
                    return "面";
                default:
                    return "未知";
            }
        }

    }
}
