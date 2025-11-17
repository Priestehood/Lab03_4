using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Lab03_4
{
    public partial class Form_4 : Form
    {
        #region 图层管理功能

        /// <summary>
        /// 加载所有SHP文件
        /// </summary>
        private void LoadAllShapefiles()
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "选择包含SHP文件的目录";

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] shpFiles = Directory.GetFiles(folderDialog.SelectedPath, "*.shp");
                    if (shpFiles.Length == 0)
                    {
                        MessageBox.Show("该目录下没有找到SHP文件");
                        return;
                    }

                    int loadedCount = 0;
                    int skippedCount = 0;

                    foreach (string shpFile in shpFiles)
                    {
                        string layerName = System.IO.Path.GetFileNameWithoutExtension(shpFile);

                        // 检查主地图中是否已存在同名图层
                        ILayer existingLayer = FindLayerInMap(layerName);
                        if (existingLayer != null)
                        {
                            // 批量加载时，自动重命名而不是提示用户
                            layerName = GetUniqueLayerNameInMap(layerName);
                            skippedCount++;
                        }

                        // 添加SHP文件到地图
                        AddShapefileToMap(shpFile, layerName);
                        loadedCount++;
                    }

                    axMap.Refresh();

                    string statusMsg = $"成功加载 {loadedCount} 个SHP文件";
                    if (skippedCount > 0)
                    {
                        statusMsg += $"，{skippedCount} 个文件因重名已自动重命名";
                    }

                    UpdateStatus(statusMsg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载SHP文件失败: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 添加单个SHP文件
        /// </summary>
        private void AddShapefile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Shapefile文件 (*.shp)|*.shp";
            openFileDialog.Title = "选择SHP文件";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    string layerName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                    // 检查主地图中是否已存在同名图层
                    ILayer existingLayer = FindLayerInMap(layerName);
                    if (existingLayer != null)
                    {
                        DialogResult result = ShowLayerExistsDialog(layerName, "主地图");

                        if (result == DialogResult.Cancel)
                        {
                            UpdateStatus("用户取消了添加图层");
                            return;
                        }
                        else if (result == DialogResult.No) // 覆盖
                        {
                            // 选择覆盖，先移除现有图层
                            axMap.Map.DeleteLayer(existingLayer);
                            UpdateStatus($"已覆盖主地图中的图层 '{layerName}'");
                        }
                        else if (result == DialogResult.Yes) // 继续添加
                        {
                            // 选择继续添加，需要重命名新图层
                            layerName = GetUniqueLayerNameInMap(layerName);
                        }
                    }

                    // 添加SHP文件到地图
                    AddShapefileToMap(filePath, layerName);
                    axMap.Refresh();
                    UpdateStatus("SHP文件加载成功: " + System.IO.Path.GetFileName(filePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载SHP文件失败: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 将SHP文件添加到地图
        /// </summary>
        private void AddShapefileToMap(string filePath, string layerName)
        {
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(
                System.IO.Path.GetDirectoryName(filePath), 0);
            IFeatureClass featureClass = featureWorkspace.OpenFeatureClass(
                System.IO.Path.GetFileNameWithoutExtension(filePath));

            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = layerName;

            axMap.AddLayer(featureLayer);
        }

        /// <summary>
        /// 在主地图中查找图层
        /// </summary>
        private ILayer FindLayerInMap(string layerName)
        {
            for (int i = 0; i < axMap.LayerCount; i++)
            {
                ILayer layer = axMap.get_Layer(i);
                if (layer.Name == layerName)
                {
                    return layer;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取主地图中唯一的图层名称
        /// </summary>
        private string GetUniqueLayerNameInMap(string baseName)
        {
            string newName = baseName;
            int counter = 1;

            while (FindLayerInMap(newName) != null)
            {
                newName = $"{baseName}_{counter}";
                counter++;
            }

            return newName;
        }

        /// <summary>
        /// 显示图层已存在对话框
        /// <summary>
        private DialogResult ShowLayerExistsDialog(string layerName, string mapType)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = "图层已存在";
                dialog.Size = new Size(400, 180);
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label label = new Label()
                {
                    Text = $"{mapType}中已存在名为 '{layerName}' 的图层。\n请选择操作：",
                    Location = new System.Drawing.Point(20, 20),
                    Size = new Size(350, 40),
                    Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0)
                };

                Button btnContinue = new Button()
                {
                    Text = "继续添加(&A)",
                    DialogResult = DialogResult.Yes,
                    Location = new System.Drawing.Point(20, 70),
                    Size = new Size(100, 30)
                };

                Button btnOverwrite = new Button()
                {
                    Text = "覆盖(&O)",
                    DialogResult = DialogResult.No,
                    Location = new System.Drawing.Point(140, 70),
                    Size = new Size(100, 30)
                };

                Button btnCancel = new Button()
                {
                    Text = "取消(&C)",
                    DialogResult = DialogResult.Cancel,
                    Location = new System.Drawing.Point(260, 70),
                    Size = new Size(100, 30)
                };

                dialog.Controls.AddRange(new Control[] { label, btnContinue, btnOverwrite, btnCancel });
                dialog.AcceptButton = btnContinue;
                dialog.CancelButton = btnCancel;

                return dialog.ShowDialog();
            }
        }

        #endregion
    }
}