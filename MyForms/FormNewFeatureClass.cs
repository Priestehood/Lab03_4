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
            InitializeSpatialReferenceComboBox();
            InitializeDataGridView();
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
            // 仅文本类型设置字段长度
            dataGridViewField.Columns["colFieldLength"].Visible = false; 
        }
        #endregion

        #region 主窗口菜单-要素类管理-弹窗-按钮事件处理
        /// <summary>
        /// 选择存储目录
        /// </summary>
        private void btnFormNewSelectPath_Click(object sender, EventArgs e)
        {
            BrowseFolder();
        }

        /// <summary>
        /// 添加字段属性
        /// </summary>
        private void btnAddField_Click(object sender, EventArgs e)
        {
            AddField();
        }

        /// <summary>
        /// 删除选中字段
        /// </summary>
        private void btnDeleteField_Click(object sender, EventArgs e)
        {
            DeleteSelectedFields();
        }

        /// <summary>
        /// 清空所有字段
        /// </summary>
        private void btnClearField_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        /// <summary>
        /// 取消创建
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelCreation();
        }

        /// <summary>
        /// 确定创建
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
        /// 添加自定义字段属性
        /// </summary>
        private void AddField()
        {
            int index = dataGridViewField.Columns.Count;

            // 默认列名，如：Attr1, Attr2...
            string internalName = "colAttr" + index;
            string headerText = "自定义属性" + index;

            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.Name = internalName;
            col.HeaderText = headerText;
            col.Width = 120;

            // 允许用户编辑列标题
            col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridViewField.Columns.Add(col);
        }

        /// <summary>
        /// 鼠标双击单元格事件-编辑列标题
        /// </summary>
        private void dataGridViewField_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 如果是双击列标题（行号为 -1）
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                DataGridViewColumn col = dataGridViewField.Columns[e.ColumnIndex];

                // 创建一个文本框覆盖在标题上，让用户编辑
                TextBox tb = new TextBox();
                tb.Text = col.HeaderText;
                tb.BorderStyle = BorderStyle.FixedSingle;

                Rectangle rect = dataGridViewField.GetCellDisplayRectangle(e.ColumnIndex, -1, true);
                tb.SetBounds(rect.X, rect.Y, rect.Width, rect.Height);

                tb.Leave += (s, ev) =>
                {
                    col.HeaderText = tb.Text;  // 更新标题
                    tb.Dispose();
                };

                tb.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter)
                    {
                        col.HeaderText = tb.Text;
                        tb.Dispose();
                    }
                };

                dataGridViewField.Controls.Add(tb);
                tb.Focus();
                tb.SelectAll();
            }
        }

        /// <summary>
        /// 删除选中字段
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
        /// 验证用户输入非空
        /// </summary>
        private bool ValidateInputs()
        {
            return ValidateFolder() &&
                   ValidateFileName() &&
                   ValidateGeometryType() &&
                   ValidateSpatialReference() &&
                   ValidateFields();
        }
        
        /// <summary>
        /// 存储目录输入非空
        /// </summary>
        private bool ValidateFolder()
        {
            if (string.IsNullOrWhiteSpace(txtFormNewPath.Text))
            {
                ShowWarning("请选择存储目录");
                return false;
            }
            return true;
        }
       
        /// <summary>
        /// 文件名称输入非空
        /// </summary>
        private bool ValidateFileName()
        {
            if (string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                ShowWarning("请输入文件名称");
                return false;
            }
            return true;
        }
       
        /// <summary>
        /// 几何类型输入非空
        /// </summary>
        private bool ValidateGeometryType()
        {
            if (cmbGeometryType.SelectedIndex == -1)
            {
                ShowWarning("请选择几何类型");
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// 空间参考输入非空
        /// </summary>
        private bool ValidateSpatialReference()
        {
            if (cmbSR.SelectedIndex == -1)
            {
                ShowWarning("请选择坐标系");
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// 字段集合输入非空
        /// </summary>
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

        /// <summary>
        /// 辅助：验证单个字段输入非空
        /// </summary>
        private bool ValidateFieldRow(DataGridViewRow row)
        {
            // 验证字段名称
            if (row.Cells["colFieldName"].Value == null ||
                string.IsNullOrWhiteSpace(row.Cells["colFieldName"].Value.ToString()))
            {
                ShowWarning("字段名称不能为空");
                return false;
            }

            // 验证字段类型
            if (row.Cells["colFieldType"].Value == null)
            {
                ShowWarning("请选择字段类型");
                return false;
            }

            // 验证文本字段长度
            string fieldType = row.Cells["colFieldType"].Value.ToString();
            if (fieldType == "文本")
            {
                if (row.Cells["colFieldLength"].Value == null ||
                    string.IsNullOrWhiteSpace(row.Cells["colFieldLength"].Value.ToString()))
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
        private void dataGridViewField_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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
            if (row.Cells["colFieldType"].Value == null) return;

            string fieldType = row.Cells["colFieldType"].Value.ToString();
            bool isTextType = (fieldType == "文本");

            dataGridViewField.Columns["colFieldLength"].Visible = isTextType;

            if (isTextType && (row.Cells["colFieldLength"].Value == null ||
                string.IsNullOrWhiteSpace(row.Cells["colFieldLength"].Value.ToString())))
            {
                row.Cells["colFieldLength"].Value = "50";
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
