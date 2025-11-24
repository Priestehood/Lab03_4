using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using Lab04_4.MyForms.FeatureClassManagement.Forms;
using Lab04_4.MyForms.FeatureClassManagement.Helpers;
using Lab04_4.MyForms.FeatureClassManagement.Services;
using System;
using System.IO;
using System.Windows.Forms;

namespace Lab04_4
{
    public partial class Form_4 : Form
    {
        #region 私有字段
        private ShapefileService _shapefileService;
        //private readonly ShapefileService _shapefileService;
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化要素类管理服务
        /// </summary>
        private void InitializeFeatureClassManagement()
        {
            _shapefileService = new ShapefileService();
        }
        #endregion

        #region 要素类管理-创建新的要素类
        /// <summary>
        /// 创建新的要素类
        /// </summary>
        private void CreateNewFeatureClass()
        {
            try
            {
                using (FormNewFeatureClass form = new FormNewFeatureClass())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        HandleSuccessfulCreation(form);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleFeatureClassError("创建要素类失败", ex);
            }
        }

        /// <summary>
        /// 处理成功创建的情况
        /// </summary>
        private void HandleSuccessfulCreation(FormNewFeatureClass form)
        {
            string message = $"SHP文件 [{form.Folder}\\{form.ShpFileName}] 创建成功！\n是否加载到地图中？";

            if (ShowCreationSuccessDialog(message))
            {
                LoadCreatedShapefile(form);
            }
            else
            {
                UpdateStatus($"SHP文件创建成功: {form.ShpFileName}");
            }
        }

        /// <summary>
        /// 显示创建成功对话框
        /// </summary>
        private bool ShowCreationSuccessDialog(string message)
        {
            return MessageBox.Show(message, "创建成功",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// 加载创建的Shapefile到地图
        /// </summary>
        private void LoadCreatedShapefile(FormNewFeatureClass form)
        {
            try
            {
                IFeatureLayer featureLayer = CreateFeatureLayer(form);
                AddLayerToMap(featureLayer);
                RefreshMapDisplay();

                UpdateStatus($"已加载新创建的SHP文件: {form.ShpFileName}");
            }
            catch (Exception ex)
            {
                HandleFeatureClassError("加载创建的Shapefile失败", ex);
            }
        }

        /// <summary>
        /// 创建要素图层
        /// </summary>
        private IFeatureLayer CreateFeatureLayer(FormNewFeatureClass form)
        {
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = form.FeatureClass;
            featureLayer.Name = Path.GetFileNameWithoutExtension(form.ShpFileName);
            return featureLayer;
        }

        /// <summary>
        /// 添加图层到地图
        /// </summary>
        private void AddLayerToMap(ILayer layer)
        {
            axMap.AddLayer(layer);
        }

        /// <summary>
        /// 刷新地图显示
        /// </summary>
        private void RefreshMapDisplay()
        {
            axMap.Refresh();
            axTOC.Update();
        }

        /// <summary>
        /// 处理要素类操作错误
        /// </summary>
        private void HandleFeatureClassError(string operation, Exception ex)
        {
            string errorMessage = $"{operation}: {ex.Message}";
            MessageBox.Show(errorMessage, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateStatus($"操作失败: {operation}");
        }
        #endregion

        #region 要素类管理-管理字段

        /// <summary>
        /// 编辑要素类字段
        /// </summary>
        private void EditFeatureClassFields()
        {
            try
            {
                var selectedLayer = GetSelectedLayer();
                if (!ValidateFeatureLayer(selectedLayer, "管理字段")) return;

                IFeatureLayer featureLayer = selectedLayer as IFeatureLayer;

                using (var form = new FormNewFeatureClass(featureLayer))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        RefreshMapDisplay();
                        UpdateStatus($"字段管理完成: {selectedLayer.Name}");
                        Logger.Info($"成功管理图层字段: {selectedLayer.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                HandleFeatureClassError("管理字段失败", ex);
                Logger.Error("管理字段失败", ex);
            }
        }

        /// <summary>
        /// 验证要素图层
        /// </summary>
        private bool ValidateFeatureLayer(ILayer layer, string operation)
        {
            if (layer == null)
            {
                ShowOperationWarning("请先在TOC中选择一个要素图层", operation);
                return false;
            }

            IFeatureLayer featureLayer = layer as IFeatureLayer;
            if (featureLayer?.FeatureClass == null)
            {
                ShowOperationWarning("选中的图层不是要素图层", operation);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 显示操作警告
        /// </summary>
        private void ShowOperationWarning(string message, string operation)
        {
            MessageBox.Show(message, $"{operation} - 提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion

        #region 要素类管理-删除要素类

        /// <summary>
        /// 删除要素类
        /// </summary>
        private void DeleteFeatureClass()
        {
            // 获取当前选中图层
            m_selectedLayer = GetSelectedLayer();
            ILayer layer = m_selectedLayer;
            if (layer == null)
            {
                MessageBox.Show("请先选中一个图层。", "提示",  
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 弹窗确认
            var result = MessageBox.Show(
                $"确定要删除要素类：{layer.Name} ?\n\n此操作不可恢复，是否继续？",
                "确认删除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            // 从地图移除图层（视觉上先移除）
            axMap.Map.DeleteLayer(layer);
            RemoveLayerFromThumbnail(m_selectedLayer.Name);
            axMap.Refresh();
            axTOC.Refresh();
            axThum.Refresh();
            UpdateStatus("要素类删除成功: " + m_selectedLayer.Name);
            m_selectedLayer = null;

            // 获取完整 shapefile 路径
            IFeatureLayer fl = layer as IFeatureLayer;
            IDataset ds = fl.FeatureClass as IDataset; 
            //IDataset ds = (layer as IFeatureLayer)?.FeatureClass as IDataset;
            if (ds == null)
            {
                MessageBox.Show("错误：无法获取要素类", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string shpFullPath = System.IO.Path.Combine(ds.Workspace.PathName, ds.Name + ".shp");

            System.Runtime.InteropServices.Marshal.ReleaseComObject(fl);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(ds);
            fl = null;
            ds = null;

            // 删除shapefile
            DeleteFeatureClassByGP(shpFullPath);
        }

        /// <summary>
        /// 删除shapefile
        /// </summary>
        private void DeleteFeatureClassByGP(string fullPath)
        {
            try
            {
                // 强制释放 ArcObjects 对文件的引用
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                foreach (var ext in new[] { ".shp", ".shx", ".dbf", ".prj", ".sbn", ".sbx", ".fbn", ".fbx", ".atx", ".ixs", ".xml", ".cpg" })
                {
                    string p = System.IO.Path.ChangeExtension(fullPath, ext);
                    if (System.IO.File.Exists(p))
                    {
                        try { System.IO.File.Delete(p); }
                        catch { /* 如果锁被释放，这里一定成功 */ }
                    }
                }

                MessageBox.Show("要素类已成功删除。", "完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("删除失败：" + ex.Message);
            }
        }

        #endregion
    }
}