using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;
using System.Windows.Forms;

namespace Lab04_4
{
    public partial class Form_4 : Form
    {
        #region 鹰眼图功能

        /// <summary>
        /// 添加图层到鹰眼地图
        /// </summary>
        private void AddLayerToThumbnail()
        {
            m_selectedLayer = GetSelectedLayer();
            if (m_selectedLayer != null)
            {
                try
                {
                    // 检查是否已存在同名图层
                    ILayer existingLayer = FindLayerInThumbnail(m_selectedLayer.Name);
                    if (existingLayer != null)
                    {
                        DialogResult result = MessageBox.Show(
                            $"鹰眼地图中已存在名为 '{m_selectedLayer.Name}' 的图层。是否覆盖？",
                            "确认覆盖图层",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                        {
                            UpdateStatus("用户取消了添加图层到鹰眼地图");
                            return;
                        }

                        // 用户选择覆盖，先移除现有图层
                        axThum.Map.DeleteLayer(existingLayer);
                    }

                    // 复制图层到鹰眼地图
                    ILayer copiedLayer = CopyLayer(m_selectedLayer);
                    axThum.AddLayer(copiedLayer);

                    // 设置鹰眼地图的显示范围为鹰眼地图的全图范围
                    if (axThum.LayerCount > 0)
                    {
                        axThum.Extent = axThum.FullExtent;
                    }

                    axThum.Refresh();

                    // 手动触发一次范围更新，确保红色范围框显示
                    if (axMap.Extent != null)
                    {
                        axMap_OnExtentUpdated(null, null);
                    }

                    UpdateStatus($"图层 '{m_selectedLayer.Name}' 已添加到鹰眼地图");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("添加到鹰眼失败: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请先选择一个图层", "提示");
            }
        }

        /// <summary>
        /// 从鹰眼地图移除图层
        /// </summary>
        private void RemoveLayerFromThumbnail(string layerName)
        {
            try
            {
                ILayer thumbLayer = FindLayerInThumbnail(layerName);
                if (thumbLayer != null)
                {
                    axThum.Map.DeleteLayer(thumbLayer);
                    axThum.Refresh();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("从鹰眼地图移除图层失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 在鹰眼地图中查找图层
        /// </summary>
        private ILayer FindLayerInThumbnail(string layerName)
        {
            for (int i = 0; i < axThum.LayerCount; i++)
            {
                ILayer layer = axThum.get_Layer(i);
                if (layer.Name == layerName)
                {
                    return layer;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取图层在鹰眼地图中的索引
        /// </summary>
        private int GetLayerIndexInThumbnail(ILayer layer)
        {
            for (int i = 0; i < axThum.LayerCount; i++)
            {
                if (axThum.get_Layer(i) == layer)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 同步图层操作到鹰眼地图
        /// </summary>
        private void SyncLayerToThumbnail(ILayer layer, int oldIndex, int newIndex)
        {
            try
            {
                ILayer thumbLayer = FindLayerInThumbnail(layer.Name);
                if (thumbLayer != null)
                {
                    int thumbIndex = GetLayerIndexInThumbnail(thumbLayer);
                    if (thumbIndex >= 0)
                    {
                        axThum.MoveLayerTo(thumbIndex, newIndex);
                        axThum.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("同步图层到鹰眼地图失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 复制图层
        /// </summary>
        private ILayer CopyLayer(ILayer sourceLayer)
        {
            // 这里需要根据图层类型进行复制
            // 简化实现，实际应用中可能需要更复杂的复制逻辑
            if (sourceLayer is IFeatureLayer)
            {
                IFeatureLayer sourceFeatureLayer = sourceLayer as IFeatureLayer;
                IFeatureLayer newFeatureLayer = new FeatureLayerClass();
                newFeatureLayer.FeatureClass = sourceFeatureLayer.FeatureClass;
                newFeatureLayer.Name = sourceFeatureLayer.Name;
                return newFeatureLayer;
            }
            return sourceLayer;
        }

        #endregion
    }
}