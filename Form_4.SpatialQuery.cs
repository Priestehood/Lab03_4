using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geometry;
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using Lab04_4.MyForms.FeatureClassManagement.Helpers;
using ESRI.ArcGIS.esriSystem;

namespace Lab04_4
{
    public partial class Form_4 : Form
    {
        #region 空间查询-查询面积极值

        /// <summary>
        /// 查询面积最大和最小的建筑
        /// </summary>
        private void QueryAreaExtremeValue()
        {
            try
            {
                var selectedLayer = GetSelectedLayer();
                if (!ValidateFeatureLayer(selectedLayer, "查询面积极值")) return;

                IFeatureLayer featureLayer = selectedLayer as IFeatureLayer;
                IFeatureClass featureClass = featureLayer.FeatureClass;

                // 检查是否为面状要素
                if (featureClass.ShapeType != esriGeometryType.esriGeometryPolygon)
                {
                    MessageBox.Show("请选择一个面状要素图层（建筑图层）", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 检查坐标系统 - 面积计算需要投影坐标系
                bool isGeographic = IsGeographicCoordinateSystem(featureClass);
                if (isGeographic)
                {
                    MessageBox.Show("警告：当前图层使用地理坐标系，面积计算将自动转换为平方米。",
                        "坐标系提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // 查找名称和ID字段
                int nameFieldIndex = FindFieldIndex(featureClass, new[] { "NAME", "名称", "建筑名称", "BUILDING", "MC", "建筑名" });
                int idFieldIndex = FindFieldIndex(featureClass, new[] { "ID", "FID", "OBJECTID", "编号", "BH" });

                if (nameFieldIndex == -1 || idFieldIndex == -1)
                {
                    MessageBox.Show("未找到必要的字段（名称字段和ID字段）", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 查询面积极值
                var result = FindAreaExtremeValues(featureClass, nameFieldIndex, idFieldIndex, isGeographic);

                // 显示结果
                ShowAreaExtremeResult(result, selectedLayer.Name, isGeographic);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"查询面积极值失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Error("查询面积极值失败", ex);
            }
        }

        /// <summary>
        /// 检查是否为地理坐标系
        /// </summary>
        private bool IsGeographicCoordinateSystem(IFeatureClass featureClass)
        {
            try
            {
                IGeoDataset geoDataset = featureClass as IGeoDataset;
                if (geoDataset?.SpatialReference == null) return false;

                return IsGeographicCoordinateSystem(geoDataset.SpatialReference);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查是否为地理坐标系
        /// </summary>
        private bool IsGeographicCoordinateSystem(ISpatialReference spatialReference)
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
        /// 查找字段索引
        /// </summary>
        private int FindFieldIndex(IFeatureClass featureClass, string[] fieldNames)
        {
            for (int i = 0; i < featureClass.Fields.FieldCount; i++)
            {
                string fieldName = featureClass.Fields.Field[i].Name.ToUpper();
                foreach (string name in fieldNames)
                {
                    if (fieldName == name.ToUpper())
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查找面积最大和最小的要素
        /// </summary>
        private AreaExtremeResult FindAreaExtremeValues(IFeatureClass featureClass, int nameFieldIndex, int idFieldIndex, bool isGeographic)
        {
            IFeatureCursor featureCursor = featureClass.Search(null, false);
            IFeature feature;

            IFeature maxAreaFeature = null;
            IFeature minAreaFeature = null;
            double maxArea = double.MinValue;
            double minArea = double.MaxValue;

            int processedCount = 0;
            int validGeometryCount = 0;

            // 获取空间参考用于单位转换
            ISpatialReference spatialReference = null;
            IGeoDataset geoDataset = featureClass as IGeoDataset;
            if (geoDataset != null)
            {
                spatialReference = geoDataset.SpatialReference;
            }

            try
            {
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    processedCount++;

                    if (feature.Shape == null)
                    {
                        Logger.Warn($"要素 {processedCount} 的几何图形为空");
                        continue;
                    }

                    // 更安全的面积计算方法，传入空间参考信息
                    double? currentArea = CalculateAreaSafely(feature.Shape, spatialReference, isGeographic);
                    if (!currentArea.HasValue)
                    {
                        Logger.Warn($"要素 {processedCount} 的面积计算失败");
                        continue;
                    }

                    validGeometryCount++;

                    // 更新最大面积要素
                    if (currentArea.Value > maxArea)
                    {
                        maxArea = currentArea.Value;
                        maxAreaFeature = feature;
                    }

                    // 更新最小面积要素
                    if (currentArea.Value < minArea)
                    {
                        minArea = currentArea.Value;
                        minAreaFeature = feature;
                    }
                }

                Logger.Info($"处理了 {processedCount} 个要素，其中 {validGeometryCount} 个有有效几何图形");

                return new AreaExtremeResult
                {
                    MaxAreaFeature = maxAreaFeature,
                    MinAreaFeature = minAreaFeature,
                    MaxArea = maxArea,
                    MinArea = minArea,
                    NameFieldIndex = nameFieldIndex,
                    IDFieldIndex = idFieldIndex,
                    ProcessedCount = processedCount,
                    ValidGeometryCount = validGeometryCount,
                    IsGeographicCoordinateSystem = isGeographic
                };
            }
            finally
            {
                // 清理资源
                if (featureCursor != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            }
        }

        /// <summary>
        /// 安全计算面积 - 使用多种方法确保面积计算正确
        /// </summary>
        private double? CalculateAreaSafely(IGeometry geometry, ISpatialReference spatialReference, bool isGeographic)
        {
            try
            {
                double? calculatedArea = null;

                // 方法1: 直接使用 IArea 接口
                IArea area = geometry as IArea;
                if (area != null)
                {
                    double rawArea = area.Area;
                    if (!double.IsNaN(rawArea) && !double.IsInfinity(rawArea) && rawArea >= 0)
                    {
                        calculatedArea = rawArea;
                    }
                }

                // 方法2: 如果方法1失败，使用 ITopologicalOperator 确保几何图形有效
                if (!calculatedArea.HasValue)
                {
                    ITopologicalOperator topoOperator = geometry as ITopologicalOperator;
                    if (topoOperator != null)
                    {
                        try
                        {
                            // 使用 IClone 接口克隆几何对象
                            IClone cloneable = geometry as IClone;
                            if (cloneable != null)
                            {
                                IGeometry clonedGeometry = cloneable.Clone() as IGeometry;
                                if (clonedGeometry != null)
                                {
                                    // 对克隆后的几何对象进行简化
                                    ITopologicalOperator clonedTopoOperator = clonedGeometry as ITopologicalOperator;
                                    if (clonedTopoOperator != null)
                                    {
                                        clonedTopoOperator.Simplify();

                                        IArea simplifiedArea = clonedGeometry as IArea;
                                        if (simplifiedArea != null)
                                        {
                                            double rawArea = simplifiedArea.Area;
                                            if (!double.IsNaN(rawArea) && !double.IsInfinity(rawArea) && rawArea >= 0)
                                            {
                                                calculatedArea = rawArea;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // 简化失败，继续尝试其他方法
                        }
                    }
                }

                // 如果计算出了面积且是地理坐标系，则转换为平方米
                if (calculatedArea.HasValue && isGeographic)
                {
                    calculatedArea = ConvertGeographicAreaToSquareMeters(calculatedArea.Value, geometry, spatialReference);
                }

                if (!calculatedArea.HasValue)
                {
                    Logger.Warn("所有面积计算方法都失败了");
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
        /// 将地理坐标系的面积（平方度）转换为平方米
        /// </summary>
        private double ConvertGeographicAreaToSquareMeters(double areaInSquareDegrees, IGeometry geometry, ISpatialReference spatialReference)
        {
            try
            {
                // 使用近似转换（基于WGS84椭球体）
                IEnvelope envelope = geometry.Envelope;
                if (envelope != null)
                {
                    double centerY = (envelope.YMax + envelope.YMin) / 2.0; // 中心纬度
                    double latLength = 111320.0; // 1度纬度约111.32km
                    double lonLength = 111320.0 * Math.Cos(centerY * Math.PI / 180.0); // 1度经度长度

                    // 近似转换因子
                    double conversionFactor = latLength * lonLength;
                    double approximateArea = areaInSquareDegrees * conversionFactor;

                    // 记录使用近似方法
                    Logger.Info($"使用近似方法计算地理坐标系面积: {areaInSquareDegrees:F6} 平方度 ≈ {approximateArea:F6} 平方米");

                    return approximateArea;
                }

                // 如果无法计算，返回原始值并记录警告
                Logger.Warn("无法将地理坐标面积转换为平方米，使用原始平方度值");
                return areaInSquareDegrees;
            }
            catch (Exception ex)
            {
                Logger.Error("地理坐标面积转换失败", ex);
                return areaInSquareDegrees;
            }
        }

        /// <summary>
        /// 显示面积极值查询结果
        /// </summary>
        private void ShowAreaExtremeResult(AreaExtremeResult result, string layerName, bool isGeographic)
        {
            if (result.MaxAreaFeature == null || result.MinAreaFeature == null)
            {
                string messageA = "未找到有效的面状要素";
                if (result.ProcessedCount > 0)
                {
                    messageA += $"\n\n处理了 {result.ProcessedCount} 个要素，但只有 {result.ValidGeometryCount} 个有有效的几何图形";
                }

                MessageBox.Show(messageA, "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string maxAreaName = GetFieldValueSafely(result.MaxAreaFeature, result.NameFieldIndex);
            string maxAreaID = GetFieldValueSafely(result.MaxAreaFeature, result.IDFieldIndex);

            string minAreaName = GetFieldValueSafely(result.MinAreaFeature, result.NameFieldIndex);
            string minAreaID = GetFieldValueSafely(result.MinAreaFeature, result.IDFieldIndex);

            // 确定面积单位和显示格式
            string areaUnit = isGeographic ? "平方米" : GetAreaUnit(result.MaxAreaFeature);
            string formatString = isGeographic ? "F8" : "F6"; // 平方米用8位小数，其他用6位

            string message = $@"图层: {layerName}
坐标系: {(isGeographic ? "地理坐标系（已转换为平方米）" : "投影坐标系")}

面积最大的建筑:
名称: {maxAreaName}
ID: {maxAreaID}
近似面积: {result.MaxArea.ToString(formatString)} {areaUnit}

面积最小的建筑:
名称: {minAreaName}
ID: {minAreaID}
近似面积: {result.MinArea.ToString(formatString)} {areaUnit}

统计信息:
- 处理要素总数: {result.ProcessedCount}
- 有效几何图形: {result.ValidGeometryCount}";

            MessageBox.Show(message, "建筑面积极值查询结果",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 更新状态栏
            UpdateStatus($"已查询要素面积极值 - 最大: {maxAreaName}, 最小: {minAreaName}");
        }

        /// <summary>
        /// 安全获取字段值
        /// </summary>
        private string GetFieldValueSafely(IFeature feature, int fieldIndex)
        {
            try
            {
                object value = feature.get_Value(fieldIndex);
                return value?.ToString() ?? "未知";
            }
            catch
            {
                return "未知";
            }
        }

        /// <summary>
        /// 获取面积单位
        /// </summary>
        private string GetAreaUnit(IFeature feature)
        {
            try
            {
                IGeoDataset geoDataset = feature.Class as IGeoDataset;
                if (geoDataset?.SpatialReference != null)
                {
                    ISpatialReference spatialRef = geoDataset.SpatialReference;

                    // 检查是否为投影坐标系
                    if (!IsGeographicCoordinateSystem(spatialRef))
                    {
                        IProjectedCoordinateSystem projCS = spatialRef as IProjectedCoordinateSystem;
                        if (projCS != null)
                        {
                            ILinearUnit linearUnit = projCS.CoordinateUnit;
                            if (linearUnit != null)
                            {
                                string unitName = linearUnit.Name.ToLower();
                                if (unitName.Contains("meter")) return "平方米";
                                if (unitName.Contains("foot")) return "平方英尺";
                                if (unitName.Contains("degree")) return "平方度";
                            }
                        }
                        return "投影单位平方";
                    }
                    else
                    {
                        return "平方度（地理坐标）";
                    }
                }
            }
            catch
            {
                // 单位获取失败
            }

            return "平方单位";
        }

        #endregion

        #region 辅助结构

        /// <summary>
        /// 面积极值查询结果
        /// </summary>
        private struct AreaExtremeResult
        {
            public IFeature MaxAreaFeature { get; set; }
            public IFeature MinAreaFeature { get; set; }
            public double MaxArea { get; set; }
            public double MinArea { get; set; }
            public int NameFieldIndex { get; set; }
            public int IDFieldIndex { get; set; }
            public int ProcessedCount { get; set; }
            public int ValidGeometryCount { get; set; }
            public bool IsGeographicCoordinateSystem { get; set; }
        }

        #endregion
    }
}