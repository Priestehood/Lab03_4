using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Lab04_4.MyForms.FeatureClassManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.SpatialQuery.Services
{
    /// <summary>
    /// 面积近似计算服务
    /// </summary>
    public class AreaCalculation
    {
        public AreaCalculation()
        {
        }

        /// <summary>
        /// 计算面积
        /// </summary>
        public double? CalculateAreaSafely(IGeometry geometry, ISpatialReference spatialReference, bool isGeographic)
        {
            try
            {
                double? calculatedArea = CalculateAreaUsingMultipleMethods(geometry);

                // 如果计算出了面积且是地理坐标系，则转换为平方米
                if (calculatedArea.HasValue && isGeographic)
                {
                    calculatedArea = ConvertGeographicAreaToSquareMeters(calculatedArea.Value, geometry, spatialReference);
                }

                return calculatedArea;
            }
            catch (Exception ex)
            {
                Logger.Error("面积计算过程中发生异常", ex);
                return null;
            }
        }

        /// <summary>
        /// 使用多种方法计算面积
        /// </summary>
        private double? CalculateAreaUsingMultipleMethods(IGeometry geometry)
        {
            // 方法1: 直接使用 IArea 接口
            var area = CalculateAreaDirectly(geometry);
            if (area.HasValue) return area.Value;

            // 方法2: 使用简化后的几何图形计算面积
            var simplifiedArea = CalculateAreaWithSimplifiedGeometry(geometry);
            if (simplifiedArea.HasValue) return simplifiedArea.Value;

            Logger.Warn("所有面积计算方法都失败了");
            return null;
        }

        /// <summary>
        /// 直接使用IArea接口计算面积
        /// </summary>
        private double? CalculateAreaDirectly(IGeometry geometry)
        {
            IArea area = geometry as IArea;
            if (area == null) return null;

            double rawArea = area.Area;
            if (IsValidAreaValue(rawArea))
            {
                return rawArea;
            }

            return null;
        }

        /// <summary>
        /// 使用简化后的几何图形计算面积
        /// </summary>
        private double? CalculateAreaWithSimplifiedGeometry(IGeometry geometry)
        {
            try
            {
                ITopologicalOperator topoOperator = geometry as ITopologicalOperator;
                if (topoOperator == null) return null;

                IClone cloneable = geometry as IClone;
                if (cloneable == null) return null;

                IGeometry clonedGeometry = cloneable.Clone() as IGeometry;
                if (clonedGeometry == null) return null;

                // 对克隆后的几何对象进行简化
                ITopologicalOperator clonedTopoOperator = clonedGeometry as ITopologicalOperator;
                if (clonedTopoOperator == null) return null;

                clonedTopoOperator.Simplify();

                return CalculateAreaDirectly(clonedGeometry);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 将地理坐标系的面积（平方度）转换为平方米
        /// </summary>
        private double ConvertGeographicAreaToSquareMeters(double areaInSquareDegrees, IGeometry geometry, ISpatialReference spatialReference)
        {
            try
            {
                IEnvelope envelope = geometry.Envelope;
                if (envelope == null)
                {
                    Logger.Warn("无法获取几何图形的范围，使用原始平方度值");
                    return areaInSquareDegrees;
                }

                double centerY = (envelope.YMax + envelope.YMin) / 2.0; // 中心纬度
                double latLength = 111320.0; // 1度纬度约111.32km
                double lonLength = 111320.0 * Math.Cos(centerY * Math.PI / 180.0); // 1度经度长度

                // 近似转换因子
                double conversionFactor = latLength * lonLength;
                double approximateArea = areaInSquareDegrees * conversionFactor;

                Logger.Info($"使用近似方法计算地理坐标系面积: {areaInSquareDegrees:F6} 平方度 ≈ {approximateArea:F4} 平方米");

                return approximateArea;
            }
            catch (Exception ex)
            {
                Logger.Error("地理坐标面积转换失败", ex);
                return areaInSquareDegrees;
            }
        }

        /// <summary>
        /// 检查面积值是否有效
        /// </summary>
        private bool IsValidAreaValue(double area)
        {
            return !double.IsNaN(area) && !double.IsInfinity(area) && area >= 0;
        }
    }
}
