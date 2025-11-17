using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using System;
using System.Windows.Forms;

namespace Lab03_4
{
    public partial class Form_4 : Form
    {
        #region 图层操作功能

        /// <summary>
        /// 移除选中图层
        /// </summary>
        private void RemoveSelectedLayer()
        {
            m_selectedLayer = GetSelectedLayer();
            if (m_selectedLayer != null)
            {
                try
                {
                    string layerName = m_selectedLayer.Name;

                    // 从主地图移除
                    axMap.Map.DeleteLayer(m_selectedLayer);

                    // 从鹰眼地图移除同名图层
                    RemoveLayerFromThumbnail(layerName);

                    UpdateStatus("图层移除成功: " + layerName);
                    m_selectedLayer = null;

                    // 刷新地图
                    axMap.Refresh();
                    axThum.Refresh();
                    axTOC.Update();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("移除图层失败: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请先选中要移除的图层!", "提示");
            }
        }

        /// <summary>
        /// 设置图层可选状态
        /// </summary>
        private void SetLayerSelectable()
        {
            m_selectedLayer = GetSelectedLayer();
            if (m_selectedLayer != null)
            {
                try
                {
                    // 验证是否为要素图层
                    IFeatureLayer selectedFeatureLayer = m_selectedLayer as IFeatureLayer;
                    if (selectedFeatureLayer == null)
                    {
                        MessageBox.Show("选中的图层不是要素图层，无法设置选择状态", "提示",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 设置所有图层为不可选
                    SetAllLayersUnselectable();

                    // 设置选中图层为可选
                    selectedFeatureLayer.Selectable = true;

                    // 更新状态和界面
                    UpdateSelectionStatus();

                    // 更新状态显示
                    UpdateSelectionStatusDisplay();

                    // 记录操作日志
                    System.Diagnostics.Debug.WriteLine($"图层 '{m_selectedLayer.Name}' 已设置为唯一可选图层");
                }
                catch (Exception ex)
                {
                    HandleSelectionError(ex);
                }
            }
            else
            {
                MessageBox.Show("请先选择一个图层!", "提示");
                return;
            }
        }

        /// <summary>
        /// 辅助：设置所有图层为不可选状态
        /// </summary>
        private void SetAllLayersUnselectable()
        {
            for (int i = 0; i < axMap.LayerCount; i++)
            {
                ILayer layer = axMap.get_Layer(i);
                IFeatureLayer featureLayer = layer as IFeatureLayer;
                if (featureLayer != null)
                {
                    featureLayer.Selectable = false;
                }
            }
        }

        /// <summary>
        /// 辅助：更新选择状态显示
        /// </summary>
        private void UpdateSelectionStatus()
        {
            if (m_selectedLayer != null)
            {
                string statusMessage = $"图层 '{m_selectedLayer.Name}' 已设置为唯一可选图层";

                // 更新状态栏
                UpdateStatus(statusMessage);

                // 更新TOC显示（通过改变图层图标或颜色提示）
                UpdateTOCSelectionIndicator();

                // 更新工具栏按钮状态
                UpdateToolbarSelectionState();
            }
        }

        /// <summary>
        /// 辅助：更新TOC中的选择状态指示器
        /// </summary>
        private void UpdateTOCSelectionIndicator()
        {
            try
            {
                // 刷新TOC控件显示
                axTOC.Update();

                // 可选：在图层名称旁添加选择状态标识
                // 这里可以扩展为改变图层图标的显示
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("更新TOC选择指示器失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 辅助：更新工具栏选择状态
        /// </summary>
        private void UpdateToolbarSelectionState()
        {
            // 更新工具栏按钮的启用状态
            tlbLayerSelectable.Enabled = (m_selectedLayer != null);
            menuLayerSelectable.Enabled = (m_selectedLayer != null);
            tsmSelectable.Enabled = (m_selectedLayer != null);
        }

        /// <summary>
        /// 辅助：处理选择操作错误
        /// </summary>
        private void HandleSelectionError(Exception ex)
        {
            string errorMessage = $"设置图层选择状态失败: {ex.Message}";

            MessageBox.Show(errorMessage, "错误",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            UpdateStatus("设置选择状态时发生错误");

            // 记录详细错误信息
            System.Diagnostics.Debug.WriteLine($"选择状态设置错误: {ex.ToString()}");
        }

        /// <summary>
        /// 辅助：获取当前可选图层的名称
        /// </summary>
        public string GetCurrentSelectableLayerName()
        {
            for (int i = 0; i < axMap.LayerCount; i++)
            {
                IFeatureLayer featureLayer = axMap.get_Layer(i) as IFeatureLayer;
                if (featureLayer != null && featureLayer.Selectable)
                {
                    return featureLayer.Name;
                }
            }
            return "无";
        }

        /// <summary>
        /// 辅助：重置所有图层为可选状态
        /// </summary>
        public void ResetAllLayersSelectable()
        {
            try
            {
                for (int i = 0; i < axMap.LayerCount; i++)
                {
                    IFeatureLayer featureLayer = axMap.get_Layer(i) as IFeatureLayer;
                    if (featureLayer != null)
                    {
                        featureLayer.Selectable = true;
                    }
                }

                UpdateStatus("所有图层已重置为可选状态");
                axTOC.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"重置选择状态失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 辅助：检查当前是否有图层处于可选状态
        /// </summary>
        public bool HasSelectableLayers()
        {
            for (int i = 0; i < axMap.LayerCount; i++)
            {
                IFeatureLayer featureLayer = axMap.get_Layer(i) as IFeatureLayer;
                if (featureLayer != null && featureLayer.Selectable)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 切换图层显示/隐藏
        /// </summary>
        private void ToggleLayerVisibility()
        {
            m_selectedLayer = GetSelectedLayer();
            if (m_selectedLayer != null)
            {
                try
                {
                    m_selectedLayer.Visible = !m_selectedLayer.Visible;
                    axMap.Refresh();

                    string status = m_selectedLayer.Visible ? "显示" : "隐藏";
                    UpdateStatus($"图层 '{m_selectedLayer.Name}' 已{status}");
                    UpdateMenuStatus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("切换图层显示状态失败: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请先选中要切换的图层!", "提示");
                return;
            }
        }

        /// <summary>
        /// 上移图层
        /// </summary>
        private void MoveLayerUp()
        {
            m_selectedLayer = GetSelectedLayer();
            if (m_selectedLayer != null)
            {
                try
                {
                    int currentIndex = GetLayerIndex(m_selectedLayer);
                    if (currentIndex >= 0)
                    {
                        int newIndex = currentIndex - 1;
                        if (newIndex >= 0)
                        {
                            axMap.MoveLayerTo(currentIndex, newIndex);
                            // 同步到鹰眼地图
                            SyncLayerToThumbnail(m_selectedLayer, currentIndex, newIndex);
                            axMap.Refresh();
                            axTOC.Update();
                            UpdateStatus($"图层 '{m_selectedLayer.Name}' 已上移");
                        }
                        else
                        {
                            MessageBox.Show("图层已在最顶层，无法上移");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("上移图层失败: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请先选择一个图层", "提示");
                return;
            }
        }

        /// <summary>
        /// 下移图层
        /// </summary>
        private void MoveLayerDown()
        {
            m_selectedLayer = GetSelectedLayer();
            if (m_selectedLayer != null)
            {
                try
                {
                    int currentIndex = GetLayerIndex(m_selectedLayer);
                    if (currentIndex >= 0)
                    {
                        int newIndex = currentIndex + 1;
                        if (newIndex < axMap.LayerCount)
                        {
                            axMap.MoveLayerTo(currentIndex, newIndex);
                            // 同步到鹰眼地图
                            SyncLayerToThumbnail(m_selectedLayer, currentIndex, newIndex);
                            axMap.Refresh();
                            axTOC.Update();
                            UpdateStatus($"图层 '{m_selectedLayer.Name}' 已下移");
                        }
                        else
                        {
                            MessageBox.Show("图层已在最底层，无法下移");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("下移图层失败: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("请先选择一个图层", "提示");
                return;
            }
        }

        #endregion
    }
}