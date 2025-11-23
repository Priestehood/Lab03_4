using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Lab03_4.MyForms.FeatureClassManagement.Helpers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Lab03_4.MyForms.FeatureClassManagement.Forms
{
    /// <summary>
    /// 要素类窗体基类，包含共享的功能和UI元素
    /// </summary>
    public partial class FormFeatureClassBase : Form
    {
        #region 保护字段
        protected IFeatureLayer _featureLayer;
        protected IFeatureClass _featureClass;
        protected bool _isEditMode = false;
        #endregion

        #region 构造函数
        public FormFeatureClassBase()
        {
            // 设计器友好的构造函数
            InitializeBaseComponents();
        }

        public FormFeatureClassBase(IFeatureLayer featureLayer)
        {
            _featureLayer = featureLayer ?? throw new ArgumentNullException(nameof(featureLayer));
            _featureClass = featureLayer.FeatureClass;
            _isEditMode = true;
            InitializeBaseComponents();
        }

        /// <summary>
        /// 初始化基类组件
        /// </summary>
        private void InitializeBaseComponents()
        {
            // 设计器需要的基本初始化
            // 避免在设计时执行复杂逻辑
            if (!DesignMode)
            {
                // 运行时初始化逻辑
            }
        }
        #endregion

        #region 虚属性 - 子类可以重写
        [Browsable(false)] // 设计器中不显示这些属性
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual TextBox BaseTxtFileName => null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual ComboBox BaseCmbGeometryType => null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual ComboBox BaseCmbSR => null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual TextBox BaseTxtFormNewPath => null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Button BaseBtnFormNewSelectPath => null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual DataGridView BaseDataGridViewField => null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Button BaseBtnConfirmNew => null;
        #endregion

        #region 虚方法 - 子类可以重写
        protected virtual void InitializeSpecificControls() { }

        protected virtual void OnConfirmButtonClick()
        {
            throw new NotImplementedException("子类必须重写OnConfirmButtonClick方法");
        }

        protected virtual void OnCancelButtonClick()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion

        #region 共享的初始化方法
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        protected virtual void FormFeatureClassBase_Load(object sender, EventArgs e)
        {
            // 设计时不执行业务逻辑
            if (DesignMode) return;

            try
            {
                if (_isEditMode)
                {
                    InitializeEditMode();
                }
                else
                {
                    InitializeCreateMode();
                }
                InitializeSpecificControls();
            }
            catch (Exception ex)
            {
                HandleError($"初始化失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 初始化创建模式
        /// </summary>
        protected virtual void InitializeCreateMode()
        {
            InitializeSpatialReferenceComboBox();
            InitializeDataGridView();
        }

        /// <summary>
        /// 初始化编辑模式
        /// </summary>
        protected virtual void InitializeEditMode()
        {
            LoadLayerInfo();
            InitializeSpatialReferenceComboBox();
            InitializeDataGridView();
            DisplayExistingFields();
            SetControlsStateForEditMode();
        }

        /// <summary>
        /// 设置控件 - 编辑模式状态
        /// </summary>
        protected virtual void SetControlsStateForEditMode()
        {
            if (BaseTxtFileName != null) SetControlReadOnly(BaseTxtFileName, true);
            if (BaseCmbGeometryType != null) SetControlEnabled(BaseCmbGeometryType, false);
            if (BaseCmbSR != null) SetControlEnabled(BaseCmbSR, false);
            if (BaseTxtFormNewPath != null) SetControlReadOnly(BaseTxtFormNewPath, true);
            if (BaseBtnFormNewSelectPath != null) SetControlEnabled(BaseBtnFormNewSelectPath, false);
            SetConfirmButtonText("保存字段");
        }
        #endregion

        #region 共享的数据加载方法
        /// <summary>
        /// 加载图层信息
        /// </summary>
        protected void LoadLayerInfo()
        {
            if (_featureLayer == null || _featureClass == null) return;

            try
            {
                SetFileName(_featureLayer.Name);
                SetGeometryType(_featureClass.ShapeType);
                SetSpatialReference(GetSpatialReference());
                SetDataSourcePath(GetDataSourcePath());
                SetFormTitle($"管理字段 - {_featureLayer.Name}");
            }
            catch (Exception ex)
            {
                Logger.Warn($"加载图层信息失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取空间参考
        /// </summary>
        protected ISpatialReference GetSpatialReference()
        {
            try
            {
                IGeoDataset geoDataset = _featureClass as IGeoDataset;
                return geoDataset?.SpatialReference;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取数据源路径
        /// </summary>
        protected string GetDataSourcePath()
        {
            try
            {
                IDataset dataset = _featureClass as IDataset;
                if (dataset?.Workspace != null)
                {
                    return dataset.Workspace.PathName;
                }
                return "【当前选中图层】";
            }
            catch
            {
                return "【当前选中图层】";
            }
        }
        #endregion

        #region 共享的UI控制方法
        /// <summary>
        /// 设置控件只读状态
        /// </summary>
        protected void SetControlReadOnly(Control control, bool readOnly)
        {
            if (control == null) return;

            if (control is TextBox textBox)
            {
                textBox.ReadOnly = readOnly;
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.Enabled = !readOnly;
            }
            else
            {
                control.Enabled = !readOnly;
            }
        }

        /// <summary>
        /// 设置控件启用状态
        /// </summary>
        protected void SetControlEnabled(Control control, bool enabled)
        {
            control?.SetEnabled(enabled);
        }

        /// <summary>
        /// 初始化空间参考下拉框
        /// </summary>
        protected void InitializeSpatialReferenceComboBox()
        {
            if (BaseCmbSR == null) return;

            BaseCmbSR.Items.Clear();
            BaseCmbSR.Items.AddRange(SpatialReferenceHelper.GetPredefinedSpatialReferenceNames());
            if (!_isEditMode)
            {
                BaseCmbSR.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 初始化数据网格
        /// </summary>
        protected void InitializeDataGridView()
        {
            if (BaseDataGridViewField == null) return;
            BaseDataGridViewField.Columns["colFieldLength"].Visible = false;
        }

        /// <summary>
        /// 显示现有字段
        /// </summary>
        protected void DisplayExistingFields()
        {
            if (BaseDataGridViewField == null) return;

            try
            {
                BaseDataGridViewField.Rows.Clear();

                IFields fields = _featureClass.Fields;
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    IField field = fields.Field[i];

                    if (FieldHelper.IsSystemField(field)) continue;

                    AddFieldToGrid(field, true);
                }
            }
            catch (Exception ex)
            {
                HandleError($"加载字段列表失败: {ex.Message}");
            }
        }
        #endregion

        #region 共享的字段管理方法 *
        /// <summary>
        /// 添加字段到网格
        /// </summary>
        protected void AddFieldToGrid(IField field, bool isReadOnly)
        {
            if (BaseDataGridViewField == null) return;

            string fieldType = FieldTypeHelper.ConvertToChineseName(field.Type);
            string fieldLength = FieldTypeHelper.RequiresLength(field.Type) ?
                field.Length.ToString() : "";

            int rowIndex = BaseDataGridViewField.Rows.Add(
                field.Name,
                field.AliasName,
                fieldType,
                fieldLength
            );

            SetRowReadOnly(BaseDataGridViewField.Rows[rowIndex], isReadOnly);
        }

        /// <summary>
        /// 添加新字段行
        /// </summary>
        protected void AddNewFieldRow()
        {
            if (BaseDataGridViewField == null) return;

            int rowIndex = BaseDataGridViewField.Rows.Add();
            DataGridViewRow row = BaseDataGridViewField.Rows[rowIndex];

            row.Cells["colFieldType"].Value = "文本";
            UpdateFieldLengthVisibility(rowIndex);
            SetRowReadOnly(row, false);
        }

        /// <summary>
        /// 设置行只读状态
        /// </summary>
        protected void SetRowReadOnly(DataGridViewRow row, bool readOnly)
        {
            row.ReadOnly = readOnly;
            row.DefaultCellStyle.BackColor = readOnly ? Color.LightGray : Color.White;
            row.DefaultCellStyle.ForeColor = readOnly ? Color.Gray : Color.Black;
        }

        /// <summary>
        /// 更新字段长度可见性
        /// </summary>
        protected void UpdateFieldLengthVisibility(int rowIndex)
        {
            if (BaseDataGridViewField == null) return;

            DataGridViewRow row = BaseDataGridViewField.Rows[rowIndex];
            if (row.Cells["colFieldType"].Value == null) return;

            string fieldType = row.Cells["colFieldType"].Value.ToString();
            bool isTextType = (fieldType == "文本");

            BaseDataGridViewField.Columns["colFieldLength"].Visible = isTextType;

            if (isTextType && string.IsNullOrWhiteSpace(row.Cells["colFieldLength"].Value?.ToString()))
            {
                row.Cells["colFieldLength"].Value = "50";
            }
        }

        #endregion

        #region 共享的UI设置方法
        protected void SetFileName(string fileName)
        {
            if (BaseTxtFileName != null) BaseTxtFileName.Text = fileName;
        }

        protected void SetGeometryType(esriGeometryType geometryType)
        {
            if (BaseCmbGeometryType != null)
                BaseCmbGeometryType.Text = GeometryTypeHelper.ConvertToChineseName(geometryType);
        }

        protected void SetSpatialReference(ISpatialReference spatialRef)
        {
            if (BaseCmbSR != null)
                BaseCmbSR.Text = SpatialReferenceHelper.GetDisplayName(spatialRef);
        }

        protected void SetDataSourcePath(string path)
        {
            if (BaseTxtFormNewPath != null) BaseTxtFormNewPath.Text = path;
        }

        protected void SetFormTitle(string title) => this.Text = title;

        protected void SetConfirmButtonText(string text)
        {
            if (BaseBtnConfirmNew != null) BaseBtnConfirmNew.Text = text;
        }
        #endregion

        #region 共享的事件处理方法
        protected virtual void btnFormNewSelectPath_Click(object sender, EventArgs e)
        {
            if (_isEditMode) return;
            BrowseFolder();
        }

        protected virtual void btnAddField_Click(object sender, EventArgs e)
        {
            AddNewFieldRow();
        }

        protected virtual void btnDeleteField_Click(object sender, EventArgs e)
        {
            DeleteSelectedFields();
        }

        protected virtual void btnClearField_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        protected virtual void btnCancel_Click(object sender, EventArgs e)
        {
            OnCancelButtonClick();
        }

        protected virtual void btnConfirmNew_Click(object sender, EventArgs e)
        {
            OnConfirmButtonClick();
        }

        protected virtual void dataGridViewField_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 2) return;
            UpdateFieldLengthVisibility(e.RowIndex);
        }

        #endregion

        #region 共享的业务逻辑方法
        /// <summary>
        /// 浏览文件夹，选择存储目录
        /// </summary>
        protected void BrowseFolder()
        {
            if (BaseTxtFormNewPath == null) return;

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "选择SHP文件存储目录";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    BaseTxtFormNewPath.Text = folderDialog.SelectedPath;
                }
            }
        }

        /// <summary>
        /// 删除选中字段
        /// </summary>
        protected void DeleteSelectedFields()
        {
            if (BaseDataGridViewField == null) return;

            if (BaseDataGridViewField.SelectedRows.Count == 0)
            {
                ShowWarning("请先选择要删除的字段行");
                return;
            }

            foreach (DataGridViewRow row in BaseDataGridViewField.SelectedRows)
            {
                if (_isEditMode && row.ReadOnly)
                {
                    ShowWarning("系统字段不能删除");
                    continue;
                }

                if (!row.IsNewRow)
                {
                    BaseDataGridViewField.Rows.Remove(row);
                }
            }
        }

        /// <summary>
        /// 清空所有字段
        /// </summary>
        protected void ClearAllFields()
        {
            if (BaseDataGridViewField == null) return;
            if (BaseDataGridViewField.Rows.Count == 0) return;

            if (ShowConfirmation("确定要清空所有字段吗？"))
            {
                if (_isEditMode)
                {
                    // 在编辑模式下，只清空新添加的字段
                    for (int i = BaseDataGridViewField.Rows.Count - 1; i >= 0; i--)
                    {
                        DataGridViewRow row = BaseDataGridViewField.Rows[i];
                        if (!row.IsNewRow && !row.ReadOnly)
                        {
                            BaseDataGridViewField.Rows.Remove(row);
                        }
                    }
                }
                else
                {
                    BaseDataGridViewField.Rows.Clear();
                }
            }
        }
        #endregion

        #region 共享的消息显示方法
        protected void ShowInformation(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected void ShowWarning(string message)
        {
            MessageBox.Show(message, "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected void ShowSuccess(string message)
        {
            MessageBox.Show(message, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected bool ShowConfirmation(string message)
        {
            return MessageBox.Show(message, "确认操作",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        protected void HandleError(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
    }

    #region 其他
    /// <summary>
    /// Control扩展方法<para></para>
    /// 安全地设置控件是否可用（Enabled），避免 null 或已释放控件导致程序崩溃
    /// </summary>
    public static class ControlExtensions
    {
        public static void SetEnabled(this Control control, bool enabled)
        {
            if (control != null && !control.IsDisposed)
            {
                control.Enabled = enabled;
            }
        }
    }
    #endregion
}