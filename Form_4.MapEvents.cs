using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Lab03_4
{
    public partial class Form_4 : Form
    {
        #region 地图事件处理

        /// <summary>
        /// 主地图鼠标移动显示坐标
        /// </summary>
        private void axMap_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            try
            {
                IPoint point = new PointClass();
                point.X = e.mapX;
                point.Y = e.mapY;

                // 获取地图的空间参考
                ISpatialReference spatialRef = axMap.Map.SpatialReference;
                string coordFormat;

                if (spatialRef != null)
                {
                    // 根据空间参考类型确定坐标格式
                    if (IsProjectedCoordinateSystem(spatialRef))
                    {
                        coordFormat = "F2";
                    }
                    else
                    {
                        coordFormat = "F6";
                    }
                }
                else
                {
                    // 如果没有空间参考信息，根据坐标值范围判断
                    if (Math.Abs(e.mapX) > 180 || Math.Abs(e.mapY) > 90)
                    {
                        coordFormat = "F2";
                    }
                    else
                    {
                        coordFormat = "F6";
                    }
                }

                string xCoord = e.mapX.ToString(coordFormat);
                string yCoord = e.mapY.ToString(coordFormat);

                tslCoor.Text = $"X: {xCoord}, Y: {yCoord}";
                // tslCoor.ToolTipText = $"地图坐标 - X: {xCoord}, Y: {yCoord}";
            }
            catch (Exception ex)
            {
                tslCoor.Text = "坐标获取失败";
                System.Diagnostics.Debug.WriteLine($"坐标显示错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 主地图显示
        /// </summary>
        private void axMap_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            try
            {
                if(e is null)
                {
                    UpdateThumbnailExtentBox(axMap.Extent);
                    return;
                }
              
                IEnvelope newEnvelope = e.newEnvelope as IEnvelope;
                UpdateThumbnailExtentBox(newEnvelope);
            }
            catch
            {
                UpdateThumbnailExtentBox(axMap.Extent);
            }
        }

        private void axThum_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
        }

        #endregion

        #region 鹰眼图范围框

        /// <summary>
        /// 更新鹰眼图范围框
        /// </summary>
        private void UpdateThumbnailExtentBox(IEnvelope envelope)
        {
            try
            {
                if (axThum.LayerCount == 0)
                {
                    ClearThumbnailExtentBox();
                    return;
                }

                //IEnvelope currentMapExtent = axMap.Extent;
                IEnvelope thumbExtent = TransformEnvelopeToThumbnail(envelope);
                IElement extentBoxElement = CreateExtentBoxElement(thumbExtent);
                UpdateThumbnailGraphics(extentBoxElement);
                //this.ext = currentMapExtent;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("鹰眼地图范围框更新失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 辅助：清除鹰眼图范围框
        /// </summary>
        private void ClearThumbnailExtentBox()
        {
            try
            {
                axThum.ActiveView.GraphicsContainer.DeleteAllElements();
                axThum.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("清除鹰眼图范围框失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 创建图元
        /// </summary>
        private IElement CreateExtentBoxElement(IEnvelope envelope)
        {
            // 创建矩形元素
            IElement element = new RectangleElementClass();
            element.Geometry = envelope;

            // 创建红色边框符号
            ILineSymbol lineSymbol = new SimpleLineSymbolClass();
            IRgbColor lineColor = new RgbColorClass();
            lineColor.Red = 255;
            lineColor.Green = 0;
            lineColor.Blue = 0;
            lineSymbol.Color = lineColor;
            lineSymbol.Width = 2.0;

            // 创建透明填充符号
            IFillSymbol fillSymbol = new SimpleFillSymbolClass();
            IRgbColor fillColor = new RgbColorClass();
            fillColor.NullColor = true; // 透明填充
            fillSymbol.Color = fillColor;
            fillSymbol.Outline = lineSymbol;

            // 设置元素符号
            IFillShapeElement fillElement = (IFillShapeElement)element;
            fillElement.Symbol = fillSymbol;

            return element;
        }

        /// <summary>
        /// 刷新显示
        /// </summary>
        private void UpdateThumbnailGraphics(IElement element)
        {
            try
            {
                // 获取图形容器
                IGraphicsContainer graphicsContainer = axThum.ActiveView.GraphicsContainer;

                // 清除所有现有图形元素
                graphicsContainer.DeleteAllElements();

                // 添加新的范围框元素
                graphicsContainer.AddElement(element, 0);

                // 刷新显示（使用多种刷新方式确保生效）
                axThum.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

                // 强制刷新UI
                axThum.Refresh();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("更新鹰眼图图形失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 坐标转换方法
        /// </summary>
        private IEnvelope TransformEnvelopeToThumbnail(IEnvelope sourceEnvelope)
        {
            try
            {
                if (axThum.LayerCount == 0) return sourceEnvelope;

                // 获取鹰眼图的空间参考
                ISpatialReference thumbSpatialRef = axThum.Map.SpatialReference;
                ISpatialReference mapSpatialRef = axMap.Map.SpatialReference;

                // 如果空间参考相同，直接返回
                if (thumbSpatialRef == null || mapSpatialRef == null ||
                    thumbSpatialRef.FactoryCode == mapSpatialRef.FactoryCode)
                {
                    return sourceEnvelope;
                }

                // 进行坐标转换
                IEnvelope transformedEnvelope = new EnvelopeClass();
                transformedEnvelope.XMin = sourceEnvelope.XMin;
                transformedEnvelope.YMin = sourceEnvelope.YMin;
                transformedEnvelope.XMax = sourceEnvelope.XMax;
                transformedEnvelope.YMax = sourceEnvelope.YMax;

                transformedEnvelope.Project(thumbSpatialRef);
                return transformedEnvelope;
            }
            catch
            {
                // 转换失败时返回原范围
                return sourceEnvelope;
            }
        }

        #endregion
    }
}