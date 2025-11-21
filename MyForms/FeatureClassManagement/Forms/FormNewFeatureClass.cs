using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using Lab03_4.MyForms.FeatureClassManagement.Helpers;
using Lab03_4.MyForms.FeatureClassManagement.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab03_4.MyForms.FeatureClassManagement.Forms
{
    public partial class FormNewFeatureClass : FormFeatureClassBase
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
        public FormNewFeatureClass() : base()
        {
            InitializeComponent();
            _shapefileService = new ShapefileService();
            _fieldBuilderService = new FieldBuilderService();
        }

        public FormNewFeatureClass(IFeatureLayer featureLayer) : base(featureLayer)
        {
            InitializeComponent();
            _shapefileService = new ShapefileService();
            _fieldBuilderService = new FieldBuilderService();
            FeatureClass = featureLayer?.FeatureClass;
        }
        #endregion

        #region 重写基类虚属性 - 使用不同的命名避免冲突
        protected override TextBox BaseTxtFileName => this.txtFileName;
        protected override ComboBox BaseCmbGeometryType => this.cmbGeometryType;
        protected override ComboBox BaseCmbSR => this.cmbSR;
        protected override TextBox BaseTxtFormNewPath => this.txtFormNewPath;
        protected override Button BaseBtnFormNewSelectPath => this.btnFormNewSelectPath;
        protected override DataGridView BaseDataGridViewField => this.dataGridViewField;
        protected override Button BaseBtnConfirmNew => this.btnConfirmNew;
        #endregion

        #region 事件处理方法 - 修复Designer错误
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void FormNewFeatureClass_Load_1(object sender, EventArgs e)
        {
            // 调用基类的加载逻辑
            base.FormFeatureClassBase_Load(sender, e);
        }

        /// <summary>
        /// 数据网格单元格双击事件 - 编辑列标题
        /// </summary>
        private void dataGridViewField_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 在编辑模式下，不允许编辑现有字段
            if (_isEditMode)
            {
                DataGridViewRow row = dataGridViewField.Rows[e.RowIndex];
                if (row.ReadOnly) return;
            }

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
        /// 动态添加自定义属性列，可直接编辑列标题<para></para>
        /// 服务于创建模式下的 “添加字段属性”，为便于编辑模式下的“管理字段”，未嵌入该功能
        /// </summary>
        private void AddCustomColumn()
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
        /// 数据网格单元格值改变事件
        /// </summary>
        private new void dataGridViewField_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 调用基类的实现
            base.dataGridViewField_CellValueChanged(sender, e);
        }
        #endregion

        #region 重写基类虚方法
        protected override void InitializeSpecificControls()
        {
            // 创建模式特定的初始化
            if (!_isEditMode)
            {
                SetFormTitle("创建SHP文件");
            }
        }

        protected override void OnConfirmButtonClick()
        {
            if (_isEditMode)
            {
                SaveFieldsToExistingFeatureClass();
            }
            else
            {
                CreateShapefile();
            }
        }

        protected override void OnCancelButtonClick()
        {
            base.OnCancelButtonClick(); // 调用基类实现
        }

        // 保留设计器生成的事件绑定，但重定向到基类方法
        private new void btnFormNewSelectPath_Click(object sender, EventArgs e)
        {
            base.btnFormNewSelectPath_Click(sender, e);
        }

        private new void btnAddField_Click(object sender, EventArgs e)
        {
            base.btnAddField_Click(sender, e);
        }

        private new void btnDeleteField_Click(object sender, EventArgs e)
        {
            base.btnDeleteField_Click(sender, e);
        }

        private new void btnClearField_Click(object sender, EventArgs e)
        {
            base.btnClearField_Click(sender, e);
        }

        private new void btnConfirmNew_Click(object sender, EventArgs e)
        {
            base.btnConfirmNew_Click(sender, e);
        }

        private new void btnCancel_Click(object sender, EventArgs e)
        {
            base.btnCancel_Click(sender, e);
        }

        #endregion

        #region 业务逻辑方法
        /// <summary>
        /// 创建Shapefile文件（创建模式）
        /// </summary>
        private void CreateShapefile()
        {
            try
            {
                if (!ValidateInputs()) return;

                var userInput = GetUserInput();
                if (userInput == null) return;

                var spatialReference = CreateSpatialReference();
                if (spatialReference == null) return;

                var fields = CreateFields(spatialReference);
                if (fields == null) return;

                FeatureClass = _shapefileService.CreateShapefile(
                    userInput.FolderPath,
                    userInput.FileName,
                    userInput.GeometryType,
                    spatialReference,
                    fields);

                Folder = userInput.FolderPath;
                ShpFileName = userInput.FileName + ".shp";

                ShowSuccess("SHP文件创建成功！");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                HandleError($"创建SHP文件失败: {ex.Message}");
                Logger.Error("创建SHP文件失败", ex);
            }
        }

        /// <summary>
        /// 保存字段到现有要素类（编辑模式）
        /// </summary>
        private void SaveFieldsToExistingFeatureClass()
        {
            try
            {
                if (!ValidateNewFields()) return;

                int addedCount = 0;
                foreach (DataGridViewRow row in dataGridViewField.Rows)
                {
                    if (row.IsNewRow || row.ReadOnly) continue;

                    if (AddFieldToExistingFeatureClass(row))
                    {
                        addedCount++;
                    }
                }

                if (addedCount > 0)
                {
                    ShowSuccess($"成功添加 {addedCount} 个新字段！");
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    ShowInformation("没有需要添加的新字段。");
                    this.DialogResult = DialogResult.Cancel;
                }
                this.Close();
            }
            catch (Exception ex)
            {
                HandleError($"保存字段失败: {ex.Message}");
                Logger.Error("保存字段失败", ex);
            }
        }

        /// <summary>
        /// 添加字段到现有要素类
        /// </summary>
        private bool AddFieldToExistingFeatureClass(DataGridViewRow row)
        {
            try
            {
                var newField = CreateFieldFromRow(row);
                FeatureClass.AddField(newField);
                return true;
            }
            catch (Exception ex)
            {
                string fieldName = row.Cells["colFieldName"].Value?.ToString() ?? "未知字段";
                Logger.Error($"添加字段 '{fieldName}' 失败", ex);
                return false;
            }
        }
        #endregion

        #region 验证和数据处理方法
        /// <summary>
        /// 验证用户输入（创建模式）
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
        /// 验证新字段（编辑模式）
        /// </summary>
        private bool ValidateNewFields()
        {
            bool hasNewFields = false;

            foreach (DataGridViewRow row in dataGridViewField.Rows)
            {
                if (row.IsNewRow || row.ReadOnly) continue;

                hasNewFields = true;
                if (!ValidateNewFieldRow(row))
                    return false;
            }

            if (!hasNewFields)
            {
                ShowInformation("没有需要添加的新字段。");
                return false;
            }

            return true;
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
                (dataGridViewField.Rows.Count == 1 && dataGridViewField.Rows[0].IsNewRow))
            {
                ShowWarning("请至少添加一个字段");
                return false;
            }

            foreach (DataGridViewRow row in dataGridViewField.Rows)
            {
                if (row.IsNewRow) continue;
                if (!ValidateFieldRow(row)) return false;
            }

            return true;
        }

        private bool ValidateFieldRow(DataGridViewRow row)
        {
            try
            {
                string fieldName = row.Cells["colFieldName"].Value?.ToString();
                string fieldType = row.Cells["colFieldType"].Value?.ToString();
                string fieldLength = row.Cells["colFieldLength"].Value?.ToString();

                FieldHelper.ValidateFieldName(fieldName);
                FieldHelper.ValidateFieldTypeAndLength(fieldType, fieldLength);

                return true;
            }
            catch (ArgumentException ex)
            {
                ShowWarning(ex.Message);
                return false;
            }
        }

        private bool ValidateNewFieldRow(DataGridViewRow row)
        {
            if (!ValidateFieldRow(row)) return false;

            try
            {
                string fieldName = row.Cells["colFieldName"].Value.ToString().Trim();
                FieldHelper.ValidateFieldName(fieldName, FeatureClass);
                return true;
            }
            catch (ArgumentException ex)
            {
                ShowWarning(ex.Message);
                return false;
            }
        }

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

        private IField CreateFieldFromRow(DataGridViewRow row)
        {
            IFieldEdit newField = new FieldClass() as IFieldEdit;

            string fieldName = row.Cells["colFieldName"].Value.ToString().Trim();
            newField.Name_2 = fieldName;

            string fieldAlias = row.Cells["colFieldAlias"].Value?.ToString();
            newField.AliasName_2 = string.IsNullOrWhiteSpace(fieldAlias) ? fieldName : fieldAlias.Trim();

            string fieldTypeStr = row.Cells["colFieldType"].Value.ToString();
            esriFieldType fieldType = FieldTypeHelper.ConvertToFieldType(fieldTypeStr);
            newField.Type_2 = fieldType;

            if (FieldTypeHelper.RequiresLength(fieldType))
            {
                string lengthStr = row.Cells["colFieldLength"].Value.ToString();
                newField.Length_2 = int.TryParse(lengthStr, out int length) ?
                    length : FieldTypeHelper.GetDefaultLength(fieldType);
            }

            return newField;
        }
        #endregion

        #region UI辅助方法
        private ISpatialReference ShowAdvancedSpatialReferenceDialog()
        {
            ShowInformation("坐标系选择功能待实现，暂时使用WGS84坐标系");
            return SpatialReferenceHelper.CreateSpatialReferenceByFactoryCode(4326);
        }
        #endregion

        #region 内部类
        private class UserInput
        {
            public string FolderPath { get; set; }
            public string FileName { get; set; }
            public esriGeometryType GeometryType { get; set; }
        }
        #endregion
    }
}