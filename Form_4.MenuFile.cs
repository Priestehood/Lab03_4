using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Lab04_4
{
    public partial class Form_4 : Form
    {
        #region 菜单-文件

        /// <summary>
        /// 创建新地图
        /// </summary>
        private void CreateNewMap()
        {
            if (axMap.LayerCount > 0)
            {
                DialogResult result = MessageBox.Show("是否保存当前修改？",
                    "文档修改",
                    MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.OK)
                {
                    executeCommandByProgId("esriControls.ControlsSaveAsDocCommand");
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            // 释放现有资源
            if (axMap.Map != null)
            {
                RemoveAllLayers();
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(axMap.Map);
            }

            // 创建新地图对象
            IMap newMap = new MapClass();
            newMap.Name = "Untitled";
            axMap.Map = newMap;

            axMap.Refresh();
            UpdateStatus("已新建空文档");
        }

        /// <summary>
        /// 根据progID执行内置命令
        /// </summary>
        private void executeCommandByProgId(string progId)
        {
            UID uid = new UIDClass();
            uid.Value = progId;
            ICommand command = axToolbar.CommandPool.FindByUID(uid);
            command.OnClick();
        }


        /// <summary>
        /// 移除所有图层
        /// </summary>
        private void RemoveAll()
        {
            try
            {
                if (axMap.Map.LayerCount == 0)
                {
                    MessageBox.Show("地图中没有图层。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 弹出确认对话框
                DialogResult result = MessageBox.Show(
                    $"确定要移除所有图层吗？当前共有 {axMap.Map.LayerCount} 个图层。",
                    "确认移除所有图层",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2); // 默认选择"No"按钮

                if (result == DialogResult.Yes)
                {
                    RemoveAllLayers();
                }
                else
                {
                    UpdateStatus("用户取消了移除所有图层的操作");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("移除图层失败：" + ex.Message, "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 辅助：清空
        /// </summary>
        private void RemoveAllLayers()
        {
            try
            {
                // 清空主地图图层
                axMap.Map.ClearLayers();
                axMap.Refresh();
                axTOC.Update();

                // 清空鹰眼地图图层
                axThum.Map.ClearLayers();

                // 清除鹰眼地图中的红色范围框
                axThum.ActiveView.GraphicsContainer.DeleteAllElements();
                axThum.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                axThum.Refresh();

                UpdateStatus("已移除所有图层!");
            }
            catch (Exception ex)
            {
                throw new Exception("移除所有图层时发生错误: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// 退出应用程序
        /// </summary>
        private void ExitApplication()
        {
            DialogResult result = MessageBox.Show("确定要退出系统吗？", "确认退出",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
