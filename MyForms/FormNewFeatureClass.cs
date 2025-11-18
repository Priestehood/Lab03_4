using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using Lab03_4.MyForms.FeatureClassManagement.Helpers;
using Lab03_4.MyForms.FeatureClassManagement.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab03_4.MyForms
{
    public partial class FormNewFeatureClass : Form
    {
        #region 公有属性
        public string Folder { get; private set; }
        public string ShpFileName { get; private set; }
        public IFeatureClass FeatureClass { get; private set; }
        #endregion

        #region 私有字段
        private readonly ShapefileService _shapefileService;
        private readonly FieldBuilderService _fieldBuilderService;
        #endregion

        #region 构造函数
        public FormNewFeatureClass()
        {
            InitializeComponent();
            _shapefileService = new ShapefileService();
            _fieldBuilderService = new FieldBuilderService();
        }
        #endregion

        #region 窗体Load事件
        private void FormNewFeatureClass_Load_1(object sender, EventArgs e)
        {
            InitializeControls();
        }
        #endregion

        #region 初始化控件
        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitializeControls()
        {
            InitializeGeometryTypeComboBox();
            InitializeSpatialReferenceComboBox();
            InitializeDataGridView();
        }

        /// <summary>
        /// 初始化几何类型下拉框
        /// </summary>
        private void InitializeGeometryTypeComboBox()
        {
            cmbGeometryType.Items.Clear();
            cmbGeometryType.Items.AddRange(GeometryTypeHelper.GetSupportedGeometryTypes());
            cmbGeometryType.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化空间参考下拉框
        /// </summary>
        private void InitializeSpatialReferenceComboBox()
        {
            cmbSR.Items.Clear();
            cmbSR.Items.AddRange(SpatialReferenceHelper.GetPredefinedSpatialReferenceNames());
            cmbSR.SelectedIndex = 0;
        }

        /// <summary>
        /// 初始化字段数据网格
        /// </summary>
        private void InitializeDataGridView()
        {
            dataGridViewField.Columns.Clear();

            // 添加列
            dataGridViewField.Columns.Add("FieldName", "字段名称");
            dataGridViewField.Columns.Add("FieldAlias", "字段别名");

            DataGridViewComboBoxColumn typeColumn = new DataGridViewComboBoxColumn();
            typeColumn.HeaderText = "字段类型";
            typeColumn.Items.AddRange("整数", "数字", "日期", "文本");
            dataGridViewField.Columns.Add(typeColumn);

            dataGridViewField.Columns.Add("FieldLength", "字段长度");
            // dataGridViewField.Columns["FieldLength"].Visible = false;
        }
        #endregion

        #region 主窗口菜单-要素类管理-弹窗-按钮事件处理
        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnFormNewSelectPath_Click(object sender, EventArgs e)
        {
            BrowseFolder();
        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnAddField_Click(object sender, EventArgs e)
        {
            AddField();
        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnDeleteField_Click(object sender, EventArgs e)
        {
            DeleteSelectedFields();
        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnClearField_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelCreation();
        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnConfirmNew_Click(object sender, EventArgs e)
        {
            CreateShapefile();
        }
        #endregion

        #region 按钮功能实现函数

        /// <summary>
        /// 浏览文件夹
        /// </summary>
        private void BrowseFolder()
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "选择SHP文件存储目录";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFormNewPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        private void AddField()
        {
            int newRowIndex = dataGridViewField.Rows.Add();
            dataGridViewField.Rows[newRowIndex].Cells["FieldLength"].Value = "50";
        }

        /// <summary>
        /// 删除选中的字段
        /// </summary>
        private void DeleteSelectedFields()
        {
            if (dataGridViewField.SelectedRows.Count == 0)
            {
                ShowWarning("请先选择要删除的字段行");
                return;
            }

            foreach (DataGridViewRow row in dataGridViewField.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    dataGridViewField.Rows.Remove(row);
                }
            }
        }

        /// <summary>
        /// 清空所有字段
        /// </summary>
        private void ClearAllFields()
        {
            if (dataGridViewField.Rows.Count == 0) return;

            if (ShowConfirmation("确定要清空所有字段吗？"))
            {
                dataGridViewField.Rows.Clear();
            }
        }

        /// <summary>
        /// 取消创建
        /// </summary>
        private void CancelCreation()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 创建Shapefile文件
        /// </summary>
        private void CreateShapefile()
        {
            try
            {
                if (!ValidateInputs()) return;

                // 获取用户输入
                var userInput = GetUserInput();
                if (userInput == null) return;

                // 创建空间参考
                var spatialReference = CreateSpatialReference();
                if (spatialReference == null) return;

                // 创建字段集合
                var fields = CreateFields(spatialReference);
                if (fields == null) return;

                // 创建Shapefile
                FeatureClass = _shapefileService.CreateShapefile(
                    userInput.FolderPath,
                    userInput.FileName,
                    userInput.GeometryType,
                    spatialReference,
                    fields);

                // 保存属性
                Folder = userInput.FolderPath;
                ShpFileName = userInput.FileName + ".shp";

                ShowSuccess("SHP文件创建成功！");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                HandleError($"创建SHP文件失败: {ex.Message}");
            }
        }
        #endregion

        #region 验证和数据处理方法
        /// <summary>
        /// 验证用户输入
        /// </summary>
        private bool ValidateInputs()
        {
            return ValidateFolder() &&
                   ValidateFileName() &&
                   ValidateGeometryType() &&
                   ValidateSpatialReference() &&
                   ValidateFields();
        }

        private bool ValidateFolder()
        {
            if (string.IsNullOrWhiteSpace(txtFormNewPath.Text))
            {
                ShowWarning("请选择存储目录");
                return false;
            }
            return true;
        }

        private bool ValidateFileName()
        {
            if (string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                ShowWarning("请输入文件名称");
                return false;
            }
            return true;
        }

        private bool ValidateGeometryType()
        {
            if (cmbGeometryType.SelectedIndex == -1)
            {
                ShowWarning("请选择几何类型");
                return false;
            }
            return true;
        }

        private bool ValidateSpatialReference()
        {
            if (cmbSR.SelectedIndex == -1)
            {
                ShowWarning("请选择坐标系");
                return false;
            }
            return true;
        }

        private bool ValidateFields()
        {
            if (dataGridViewField.Rows.Count == 0 ||
                (dataGridViewField.Rows.Count == 1 &&
                dataGridViewField.Rows[0].IsNewRow))
            {
                ShowWarning("请至少添加一个字段");
                return false;
            }

            // 验证每个字段
            foreach (DataGridViewRow row in dataGridViewField.Rows)
            {
                if (row.IsNewRow) continue;

                if (!ValidateFieldRow(row))
                    return false;
            }

            return true;
        }

        private bool ValidateFieldRow(DataGridViewRow row)
        {
            // 验证字段名称
            if (row.Cells["FieldName"].Value == null ||
                string.IsNullOrWhiteSpace(row.Cells["FieldName"].Value.ToString()))
            {
                ShowWarning("字段名称不能为空");
                return false;
            }

            // 验证字段类型
            if (row.Cells["FieldType"].Value == null)
            {
                ShowWarning("请选择字段类型");
                return false;
            }

            // 验证文本字段长度
            string fieldType = row.Cells["FieldType"].Value.ToString();
            if (fieldType == "文本")
            {
                if (row.Cells["FieldLength"].Value == null ||
                    string.IsNullOrWhiteSpace(row.Cells["FieldLength"].Value.ToString()))
                {
                    ShowWarning("文本字段必须指定长度");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取用户输入
        /// </summary>
        private UserInput GetUserInput()
        {
            try
            {
                string folderPath = txtFormNewPath.Text.Trim();
                string fileName = txtFileName.Text.Trim();
                string geometryTypeStr = cmbGeometryType.SelectedItem.ToString();

                esriGeometryType geometryType = GeometryTypeHelper.ConvertToGeometryType(geometryTypeStr);

                return new UserInput
                {
                    FolderPath = folderPath,
                    FileName = fileName,
                    GeometryType = geometryType
                };
            }
            catch (Exception ex)
            {
                HandleError($"获取用户输入失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 创建空间参考
        /// </summary>
        private ISpatialReference CreateSpatialReference()
        {
            try
            {
                string selectedSR = cmbSR.SelectedItem.ToString();

                if (selectedSR == "更多...")
                {
                    return ShowAdvancedSpatialReferenceDialog();
                }

                int? factoryCode = SpatialReferenceHelper.GetFactoryCodeFromDisplayName(selectedSR);
                if (factoryCode.HasValue)
                {
                    return SpatialReferenceHelper.CreateSpatialReferenceByFactoryCode(factoryCode.Value);
                }

                throw new ArgumentException($"不支持的坐标系: {selectedSR}");
            }
            catch (Exception ex)
            {
                HandleError($"创建空间参考失败: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 创建字段集合
        /// </summary>
        private IFields CreateFields(ISpatialReference spatialReference)
        {
            try
            {
                string geometryTypeStr = cmbGeometryType.SelectedItem.ToString();
                esriGeometryType geometryType = GeometryTypeHelper.ConvertToGeometryType(geometryTypeStr);

                return _fieldBuilderService.CreateFeatureClassFields(geometryType, spatialReference, dataGridViewField.Rows);
            }
            catch (Exception ex)
            {
                HandleError($"创建字段集合失败: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region UI辅助方法
        /// <summary>
        /// 显示高级坐标系选择对话框
        /// </summary>
        private ISpatialReference ShowAdvancedSpatialReferenceDialog()
        {
            // 这里可以调用ArcEngine的坐标系选择对话框
            // 简化实现，返回WGS84
            ShowInformation("坐标系选择功能待实现，暂时使用WGS84坐标系");
            return SpatialReferenceHelper.CreateSpatialReferenceByFactoryCode(4326);
        }

        private void ShowInformation(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ShowConfirmation(string message)
        {
            return MessageBox.Show(message, "确认操作",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        private void HandleError(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        #region 数据网格事件
        private void dgvFields_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 2) return;

            UpdateFieldLengthVisibility(e.RowIndex);
        }

        /// <summary>
        /// 更新字段长度列的可见性
        /// </summary>
        private void UpdateFieldLengthVisibility(int rowIndex)
        {
            DataGridViewRow row = dataGridViewField.Rows[rowIndex];
            if (row.Cells["FieldType"].Value == null) return;

            string fieldType = row.Cells["FieldType"].Value.ToString();
            bool isTextType = (fieldType == "文本");

            dataGridViewField.Columns["FieldLength"].Visible = isTextType;

            if (isTextType && (row.Cells["FieldLength"].Value == null ||
                string.IsNullOrWhiteSpace(row.Cells["FieldLength"].Value.ToString())))
            {
                row.Cells["FieldLength"].Value = "50";
            }
        }
        #endregion

        #region 内部类
        /// <summary>
        /// 用户输入数据类
        /// </summary>
        private class UserInput
        {
            public string FolderPath { get; set; }
            public string FileName { get; set; }
            public esriGeometryType GeometryType { get; set; }
        }
        #endregion

    }
}
