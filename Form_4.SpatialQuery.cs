using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Windows.Forms;
using Lab04_4.MyForms.FeatureClassManagement.Helpers;
using Lab04_4.MyForms.SpatialQuery.Forms;
using Lab04_4.MyForms.SpatialQuery.Services;
using Lab04_4.MyForms.SpatialQuery.Helpers;

namespace Lab04_4
{
    public partial class Form_4 : Form
    {
        #region 初始化

        // 服务实例
        private AreaCalculation _areaCalculationService;
        private FeatureHighlight _featureHighlightService;
        private NavigationService _navigationService;
        private ElevationAnalysis _elevationAnalysisService;
        private SpatialQueryTool _spatialQueryService;

        // 存储当前查询状态
        private int _maxAreaFeatureOID = -1;
        private int _minAreaFeatureOID = -1;
        private IFeatureLayer _currentFeatureLayer = null;

        // 服务属性 - 延迟初始化
        private AreaCalculation AreaCalculationService
        {
            get
            {
                if (_areaCalculationService == null)
                    _areaCalculationService = new AreaCalculation();
                return _areaCalculationService;
            }
        }

        private FeatureHighlight FeatureHighlightService
        {
            get
            {
                if (_featureHighlightService == null)
                    _featureHighlightService = new FeatureHighlight(axMap);
                return _featureHighlightService;
            }
        }

        private NavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                    _navigationService = new NavigationService(axMap);
                return _navigationService;
            }
        }

        private ElevationAnalysis ElevationAnalysisService
        {
            get
            {
                if (_elevationAnalysisService == null)
                    _elevationAnalysisService = new ElevationAnalysis(UpdateStatus);
                return _elevationAnalysisService;
            }
        }

        private SpatialQueryTool SpatialQueryService
        {
            get
            {
                if (_spatialQueryService == null)
                    _spatialQueryService = new SpatialQueryTool(axMap);
                return _spatialQueryService;
            }
        }

        #endregion

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
                _currentFeatureLayer = featureLayer;

                if (!ValidatePolygonLayer(featureLayer)) return;

                // 检查坐标系
                bool isGeographic = CoordinateSystem.IsGeographicCoordinateSystem(featureLayer.FeatureClass);
                CoordinateSystem.ShowCoordinateSystemWarning(isGeographic);

                // 查找必要字段
                var fieldIndices = FindRequiredFieldIndices(featureLayer.FeatureClass);
                if (!fieldIndices.IsValid) return;

                // 查询面积极值
                var result = FindAreaExtremeValues(featureLayer.FeatureClass, fieldIndices, isGeographic);

                // 显示结果
                ShowAreaExtremeResult(result, selectedLayer.Name, isGeographic);
            }
            catch (Exception ex)
            {
                ShowError($"查询面积极值失败: {ex.Message}");
                Logger.Error("查询面积极值失败", ex);
            }
        }

        #region 空间查询-查询面积极值-辅助方法
        /// <summary>
        /// 验证面状要素图层
        /// </summary>
        private bool ValidatePolygonLayer(IFeatureLayer featureLayer)
        {
            if (featureLayer?.FeatureClass?.ShapeType != esriGeometryType.esriGeometryPolygon)
            {
                MessageBox.Show("请选择一个面状要素图层（建筑图层）", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 查找必要字段索引
        /// </summary>
        private FieldIndicesResult FindRequiredFieldIndices(IFeatureClass featureClass)
        {
            string[] nameFields = { "NAME", "名称", "建筑名称", "BUILDING", "MC", "建筑名" };
            string[] idFields = { "ID", "FID", "OBJECTID", "编号", "BH" };

            int nameIndex = SQFieldHelper.FindFieldIndex(featureClass, nameFields);
            int idIndex = SQFieldHelper.FindFieldIndex(featureClass, idFields);

            if (nameIndex == -1 || idIndex == -1)
            {
                MessageBox.Show("未找到必要的字段（名称字段和ID字段）", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new FieldIndicesResult(-1, -1, false);
            }

            return new FieldIndicesResult(nameIndex, idIndex, true);
        }

        /// <summary>
        /// 查找面积最大和最小的要素
        /// </summary>
        private AreaExtremeResult FindAreaExtremeValues(IFeatureClass featureClass,
            FieldIndicesResult fieldIndices, bool isGeographic)
        {
            var result = new AreaExtremeResult
            {
                NameFieldIndex = fieldIndices.NameIndex,
                IDFieldIndex = fieldIndices.IDIndex,
                IsGeographicCoordinateSystem = isGeographic
            };

            IFeatureCursor featureCursor = null;
            try
            {
                featureCursor = featureClass.Search(null, false);
                IFeature feature;
                IFeature maxAreaFeature = null;
                IFeature minAreaFeature = null;
                double maxArea = double.MinValue;
                double minArea = double.MaxValue;

                while ((feature = featureCursor.NextFeature()) != null)
                {
                    result.ProcessedCount++;

                    if (feature.Shape == null)
                    {
                        Logger.Warn($"要素 {result.ProcessedCount} 的几何图形为空");
                        continue;
                    }

                    // 使用属性访问服务
                    double? currentArea = AreaCalculationService.CalculateAreaSafely(
                        feature.Shape, GetSpatialReference(featureClass), isGeographic);

                    if (!currentArea.HasValue)
                    {
                        Logger.Warn($"要素 {result.ProcessedCount} 的面积计算失败");
                        continue;
                    }

                    result.ValidGeometryCount++;
                    UpdateExtremeFeatures(feature, currentArea.Value, ref maxAreaFeature, ref minAreaFeature, ref maxArea, ref minArea);
                }

                result.MaxAreaFeature = maxAreaFeature;
                result.MinAreaFeature = minAreaFeature;
                result.MaxArea = maxArea;
                result.MinArea = minArea;

                // 存储要素OID用于后续操作
                if (maxAreaFeature != null) _maxAreaFeatureOID = GetFeatureOID(maxAreaFeature);
                if (minAreaFeature != null) _minAreaFeatureOID = GetFeatureOID(minAreaFeature);
            }
            finally
            {
                // 清理资源
                if (featureCursor != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
            }

            Logger.Info($"处理了 {result.ProcessedCount} 个要素，其中 {result.ValidGeometryCount} 个有有效几何图形");
            return result;
        }

        /// <summary>
        /// 获取空间参考
        /// </summary>
        private ISpatialReference GetSpatialReference(IFeatureClass featureClass)
        {
            IGeoDataset geoDataset = featureClass as IGeoDataset;
            return geoDataset?.SpatialReference;
        }

        /// <summary>
        /// 更新极值要素
        /// </summary>
        private void UpdateExtremeFeatures(IFeature feature, double currentArea,
            ref IFeature maxAreaFeature, ref IFeature minAreaFeature,
            ref double maxArea, ref double minArea)
        {
            if (currentArea > maxArea)
            {
                maxArea = currentArea;
                maxAreaFeature = feature;
            }

            if (currentArea < minArea)
            {
                minArea = currentArea;
                minAreaFeature = feature;
            }
        }

        /// <summary>
        /// 获取要素OID
        /// </summary>
        private int GetFeatureOID(IFeature feature)
        {
            try
            {
                return feature.OID;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 高亮和导航
        /// </summary>
        private void HighlightAndNavigate(AreaExtremeResult result)
        {
            string maxAreaName = SQFieldHelper.GetFieldValueSafely(result.MaxAreaFeature, result.NameFieldIndex);
            string minAreaName = SQFieldHelper.GetFieldValueSafely(result.MinAreaFeature, result.NameFieldIndex);

            // 高亮显示要素 - 使用属性访问服务
            if (FeatureHighlightService.HighlightFeatures(_currentFeatureLayer,
                new[] { _maxAreaFeatureOID, _minAreaFeatureOID }))
            {
                // 显示导航选项 - 使用属性访问服务
                NavigationService.ShowNavigationOptions(
                    maxAreaName,
                    minAreaName,
                    () => NavigationService.ZoomToFeature(_currentFeatureLayer, _maxAreaFeatureOID),
                    () => NavigationService.ZoomToFeature(_currentFeatureLayer, _minAreaFeatureOID)
                );
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
                    if (!CoordinateSystem.IsGeographicCoordinateSystem(spatialRef))
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

        #region 空间查询-查询面积极值-消息与提示
        /// <summary>
        /// 显示面积极值查询结果
        /// </summary>
        private void ShowAreaExtremeResult(AreaExtremeResult result, string layerName, bool isGeographic)
        {
            if (result.MaxAreaFeature == null || result.MinAreaFeature == null)
            {
                ShowNoValidFeaturesMessage(result);
                return;
            }

            string message = BuildResultMessage(result, layerName, isGeographic);
            DialogResult dialogResult = MessageBox.Show(message, "建筑面积极值查询结果",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                HighlightAndNavigate(result);
            }

            UpdateStatusBar(result);
        }

        /// <summary>
        /// 显示无有效要素消息
        /// </summary>
        private void ShowNoValidFeaturesMessage(AreaExtremeResult result)
        {
            string message = "未找到有效的面状要素";
            if (result.ProcessedCount > 0)
            {
                message += $"\n\n处理了 {result.ProcessedCount} 个要素，但只有 {result.ValidGeometryCount} 个有有效的几何图形";
            }

            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 构建结果消息
        /// </summary>
        private string BuildResultMessage(AreaExtremeResult result, string layerName, bool isGeographic)
        {
            string maxAreaName = SQFieldHelper.GetFieldValueSafely(result.MaxAreaFeature, result.NameFieldIndex);
            string maxAreaID = SQFieldHelper.GetFieldValueSafely(result.MaxAreaFeature, result.IDFieldIndex);
            string minAreaName = SQFieldHelper.GetFieldValueSafely(result.MinAreaFeature, result.NameFieldIndex);
            string minAreaID = SQFieldHelper.GetFieldValueSafely(result.MinAreaFeature, result.IDFieldIndex);

            string areaUnit = isGeographic ? "平方米" : GetAreaUnit(result.MaxAreaFeature);
            string formatString = isGeographic ? "F6" : "F8";

            return $@"图层: {layerName}
坐标系: {(isGeographic ? "地理坐标系（已转换为平方米）" : "投影坐标系")}

面积最大的要素:
名称: {maxAreaName}
ID: {maxAreaID}
近似面积: {result.MaxArea.ToString(formatString)} {areaUnit}

面积最小的要素:
名称: {minAreaName}
ID: {minAreaID}
近似面积: {result.MinArea.ToString(formatString)} {areaUnit}

统计信息:
- 处理要素总数: {result.ProcessedCount}
- 有效几何图形: {result.ValidGeometryCount}

是否要在地图上高亮显示这两个要素？";
        }

        /// <summary>
        /// 更新状态栏
        /// </summary>
        private void UpdateStatusBar(AreaExtremeResult result)
        {
            string maxAreaName = SQFieldHelper.GetFieldValueSafely(result.MaxAreaFeature, result.NameFieldIndex);
            string maxAreaID = SQFieldHelper.GetFieldValueSafely(result.MaxAreaFeature, result.IDFieldIndex);
            string minAreaName = SQFieldHelper.GetFieldValueSafely(result.MinAreaFeature, result.NameFieldIndex);
            string minAreaID = SQFieldHelper.GetFieldValueSafely(result.MinAreaFeature, result.IDFieldIndex);

            // 处理名称为空的情况
            string maxDisplayName = string.IsNullOrWhiteSpace(maxAreaName) || maxAreaName == "未知"
                ? "未知"
                : maxAreaName;

            string minDisplayName = string.IsNullOrWhiteSpace(minAreaName) || minAreaName == "未知"
                ? "未知"
                : minAreaName;

            UpdateStatus($"已查询要素面积极值 - 最大: {maxDisplayName}(ID: {maxAreaID}), 最小: {minDisplayName}(ID: {minAreaID})");
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        private void ShowError(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #endregion

        #region 空间分析-分析
        private void AxMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            SpatialQueryService.EnsureLayersAssigned(true);
            _lastLayerCount = axMap.LayerCount;
        }


        private void AxMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            // 如果图层数量变化 → 自动重新识别
            if (_lastLayerCount != axMap.LayerCount)
            {
                _lastLayerCount = axMap.LayerCount;
                SpatialQueryService.EnsureLayersAssigned(true);
            }
        }

        private void BeginElementQuery()
        {
            MessageBox.Show("🔍 现在请点击地图上的建筑或道路进行查询。\n右键取消。");
            axMap.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            sketcher.shape = MyForms.FeatureManagement.Services.Shape.Point;
            mapOperation = MapOperationType.ElementQuery;
        }

        private void BeginDrawPolyline()
        {
            MessageBox.Show("📌 请左键依次点击绘制多义线，右键结束绘制。");
            axMap.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
            sketcher.shape = MyForms.FeatureManagement.Services.Shape.Polyline;
            SpatialQueryService.ClearPoints();
            mapOperation = MapOperationType.DrawPolyline;
        }

        private void BeginBufferAnalysis()
        {
            SpatialQueryService.PerformBufferAnalysis();
        }

        #endregion

        #region 高程分析

        private void FilterAbnormalElevations()
        {
            try
            {
                var selectedLayer = GetSelectedLayer();
                if (!ValidateFeatureLayer(selectedLayer, "高程点滤噪")) return;

                IFeatureLayer featureLayer = selectedLayer as IFeatureLayer;
                _currentFeatureLayer = featureLayer;

                int kOfKNN = 10;
                InputIntegerForm form = new InputIntegerForm(
                    "请输入N近邻的点数n：", kOfKNN.ToString(), "高程点滤波");
                DialogResult result = form.ShowDialog();
                if (result != DialogResult.OK) return;
                kOfKNN = form.Value;

                ElevationAnalysisService.SetLayer(featureLayer);
                ElevationAnalysisService.DetectAbnormalElevations(kOfKNN,
                    SelectFeatures, DeleteFeatures);
            }
            catch (Exception ex)
            {
                ShowError($"高程点滤噪失败: {ex.Message}");
                Logger.Error("高程点滤噪失败", ex);
            }
        }

        private void BeginIntepolateElevation()
        {
            mapOperation = MapOperationType.IntepolateElevation;
            sketcher.shape = MyForms.FeatureManagement.Services.Shape.Point;
        }

        private void IntepolateElevation(IPoint clickPoint)
        {
            try
            {
                var selectedLayer = GetSelectedLayer();
                if (!ValidateFeatureLayer(selectedLayer, "高程插值")) return;

                IFeatureLayer featureLayer = selectedLayer as IFeatureLayer;

                ElevationAnalysisService.SetLayer(featureLayer);
                double result = ElevationAnalysisService.IntepolateElevation(clickPoint, 8);

                MessageBox.Show($"点击位置的插值高程：{result:F2} 米", "高程插值");
            }
            catch (Exception ex)
            {
                ShowError($"高程插值失败: {ex.Message}");
                Logger.Error("高程插值失败", ex);
            }
        }

        #endregion
    }
}