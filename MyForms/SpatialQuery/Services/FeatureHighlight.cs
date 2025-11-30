using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Lab04_4.MyForms.SpatialQuery.Helpers;
using Lab04_4.MyForms.FeatureClassManagement.Helpers;

namespace Lab04_4.MyForms.SpatialQuery.Services
{
    /// <summary>
    /// 要素高亮服务，负责要素的选择、高亮和导航
    /// </summary>
    public class FeatureHighlight
    {
        private readonly AxMapControl _mapControl;

        public FeatureHighlight(AxMapControl mapControl)
        {
            _mapControl = mapControl;
        }

        /// <summary>
        /// 高亮显示指定的要素
        /// </summary>
        public bool HighlightFeatures(IFeatureLayer featureLayer, int[] featureOIDs)
        {
            try
            {
                if (featureLayer == null || featureOIDs == null || featureOIDs.Length == 0)
                {
                    Logger.Warn("高亮显示要素失败：参数无效");
                    return false;
                }

                IFeatureSelection featureSelection = featureLayer as IFeatureSelection;
                if (featureSelection == null)
                {
                    Logger.Warn("高亮显示要素失败：图层不支持选择操作");
                    return false;
                }

                // 清除之前的选择
                featureSelection.Clear();

                // 创建选择集
                if (!SelectFeatures(featureLayer, featureOIDs))
                {
                    return false;
                }

                // 设置选择符号
                SetSelectionSymbol(featureLayer);

                // 刷新地图显示
                RefreshMap();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("高亮显示要素失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 选择指定的要素
        /// </summary>
        private bool SelectFeatures(IFeatureLayer featureLayer, int[] featureOIDs)
        {
            try
            {
                IFeatureClass featureClass = featureLayer.FeatureClass;
                IFeatureSelection featureSelection = featureLayer as IFeatureSelection;

                string oidFieldName = featureClass.OIDFieldName;
                string whereClause = BuildWhereClause(oidFieldName, featureOIDs);

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = whereClause;

                featureSelection.SelectFeatures(queryFilter,
                    esriSelectionResultEnum.esriSelectionResultNew, false);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("选择要素失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 构建查询条件
        /// </summary>
        private string BuildWhereClause(string oidFieldName, int[] featureOIDs)
        {
            if (featureOIDs.Length == 1)
            {
                return $"{oidFieldName} = {featureOIDs[0]}";
            }

            string oidList = string.Join(",", featureOIDs);
            return $"{oidFieldName} IN ({oidList})";
        }

        /// <summary>
        /// 设置选择符号
        /// </summary>
        private void SetSelectionSymbol(IFeatureLayer featureLayer)
        {
            try
            {
                IGeoFeatureLayer geoFeatureLayer = featureLayer as IGeoFeatureLayer;
                if (geoFeatureLayer == null) return;

                IFeatureSelection featureSelection = geoFeatureLayer as IFeatureSelection;
                if (featureSelection == null) return;

                ISymbol selectionSymbol = CreateHighlightSymbol();
                featureSelection.SelectionSymbol = selectionSymbol;
            }
            catch (Exception ex)
            {
                Logger.Warn($"设置选择符号失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建高亮符号
        /// </summary>
        private ISymbol CreateHighlightSymbol()
        {
            ISimpleFillSymbol fillSymbol = new SimpleFillSymbolClass();
            fillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;

            // 设置填充颜色（浅红色）
            IRgbColor fillColor = CreateColor(255, 200, 200, 150);
            fillSymbol.Color = fillColor;

            // 设置边框颜色（深红色）
            ISimpleLineSymbol outlineSymbol = new SimpleLineSymbolClass();
            outlineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            outlineSymbol.Width = 3;

            IRgbColor outlineColor = CreateColor(255, 0, 0);
            outlineSymbol.Color = outlineColor;

            fillSymbol.Outline = outlineSymbol;

            return fillSymbol as ISymbol;
        }

        /// <summary>
        /// 创建颜色对象
        /// </summary>
        private IRgbColor CreateColor(byte red, byte green, byte blue, byte transparency = 255)
        {
            IRgbColor color = new RgbColorClass();
            color.Red = red;
            color.Green = green;
            color.Blue = blue;
            color.Transparency = transparency;
            return color;
        }

        /// <summary>
        /// 缩放到指定的要素
        /// </summary>
        public bool ZoomToFeatures(IFeatureLayer featureLayer, int[] featureOIDs)
        {
            try
            {
                if (_mapControl == null || featureLayer == null || featureOIDs == null || featureOIDs.Length == 0)
                {
                    return false;
                }

                IEnvelope fullExtent = CalculateFeaturesExtent(featureLayer, featureOIDs);
                if (fullExtent == null || fullExtent.IsEmpty)
                {
                    return false;
                }

                // 稍微扩大范围以便更好地查看
                fullExtent.Expand(1.1, 1.1, true);
                _mapControl.Extent = fullExtent;

                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn($"缩放到要素失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 计算指定要素的合并范围
        /// </summary>
        private IEnvelope CalculateFeaturesExtent(IFeatureLayer featureLayer, int[] featureOIDs)
        {
            try
            {
                IFeatureClass featureClass = featureLayer.FeatureClass;
                IEnvelope fullExtent = new EnvelopeClass();
                fullExtent.SetEmpty();

                string whereClause = BuildWhereClause(featureClass.OIDFieldName, featureOIDs);
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = whereClause;

                using (var featureCursor = new ComObjectWrapper<IFeatureCursor>(featureClass.Search(queryFilter, false)))
                {
                    IFeature feature;
                    while ((feature = featureCursor.Object.NextFeature()) != null)
                    {
                        if (feature.Shape != null)
                        {
                            IEnvelope featureExtent = feature.Shape.Envelope;
                            fullExtent.Union(featureExtent);
                        }
                    }
                }

                return fullExtent;
            }
            catch (Exception ex)
            {
                Logger.Error("计算要素范围失败", ex);
                return null;
            }
        }

        /// <summary>
        /// 刷新地图显示
        /// </summary>
        private void RefreshMap()
        {
            try
            {
                if (_mapControl != null)
                {
                    _mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"刷新地图失败: {ex.Message}");
            }
        }
    }
}
