
namespace Lab03_4.MyForms
{
    partial class FormNewFeatureClass
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormNewFeatureClass));
            this.formNewGroupBox1 = new System.Windows.Forms.GroupBox();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.labelFileName = new System.Windows.Forms.Label();
            this.cmbSR = new System.Windows.Forms.ComboBox();
            this.labelSR = new System.Windows.Forms.Label();
            this.cmbGeometryType = new System.Windows.Forms.ComboBox();
            this.labelGeometryType = new System.Windows.Forms.Label();
            this.btnFormNewSelectPath = new System.Windows.Forms.Button();
            this.txtFormNewPath = new System.Windows.Forms.TextBox();
            this.labelFormNewPath = new System.Windows.Forms.Label();
            this.formNewGroupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridViewField = new System.Windows.Forms.DataGridView();
            this.colFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFieldAlias = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFieldType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colFieldLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDeleteField = new System.Windows.Forms.Button();
            this.btnClearField = new System.Windows.Forms.Button();
            this.btnConfirmNew = new System.Windows.Forms.Button();
            this.btnAddField = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.formNewGroupBox1.SuspendLayout();
            this.formNewGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewField)).BeginInit();
            this.SuspendLayout();
            // 
            // formNewGroupBox1
            // 
            this.formNewGroupBox1.Controls.Add(this.txtFileName);
            this.formNewGroupBox1.Controls.Add(this.labelFileName);
            this.formNewGroupBox1.Controls.Add(this.cmbSR);
            this.formNewGroupBox1.Controls.Add(this.labelSR);
            this.formNewGroupBox1.Controls.Add(this.cmbGeometryType);
            this.formNewGroupBox1.Controls.Add(this.labelGeometryType);
            this.formNewGroupBox1.Controls.Add(this.btnFormNewSelectPath);
            this.formNewGroupBox1.Controls.Add(this.txtFormNewPath);
            this.formNewGroupBox1.Controls.Add(this.labelFormNewPath);
            this.formNewGroupBox1.Location = new System.Drawing.Point(13, 24);
            this.formNewGroupBox1.Name = "formNewGroupBox1";
            this.formNewGroupBox1.Size = new System.Drawing.Size(684, 133);
            this.formNewGroupBox1.TabIndex = 0;
            this.formNewGroupBox1.TabStop = false;
            this.formNewGroupBox1.Text = "  文件属性  ";
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(93, 81);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(122, 25);
            this.txtFileName.TabIndex = 8;
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.Location = new System.Drawing.Point(22, 86);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(67, 15);
            this.labelFileName.TabIndex = 7;
            this.labelFileName.Text = "文件名称";
            // 
            // cmbSR
            // 
            this.cmbSR.FormattingEnabled = true;
            this.cmbSR.Items.AddRange(new object[] {
            "EPSG:4326 (WGS84)",
            "EPSG:3857 (Web Mercator)",
            "EPSG:4490 (CGCS2000)",
            "EPSG:4547 (CGCS2000/3-degree Gauss-Kruger CM 114E)",
            "更多..."});
            this.cmbSR.Location = new System.Drawing.Point(519, 82);
            this.cmbSR.Name = "cmbSR";
            this.cmbSR.Size = new System.Drawing.Size(139, 23);
            this.cmbSR.TabIndex = 6;
            // 
            // labelSR
            // 
            this.labelSR.AutoSize = true;
            this.labelSR.Location = new System.Drawing.Point(449, 86);
            this.labelSR.Name = "labelSR";
            this.labelSR.Size = new System.Drawing.Size(67, 15);
            this.labelSR.TabIndex = 5;
            this.labelSR.Text = "坐标系统";
            // 
            // cmbGeometryType
            // 
            this.cmbGeometryType.FormattingEnabled = true;
            this.cmbGeometryType.Items.AddRange(new object[] {
            "点",
            "线",
            "面"});
            this.cmbGeometryType.Location = new System.Drawing.Point(304, 82);
            this.cmbGeometryType.Name = "cmbGeometryType";
            this.cmbGeometryType.Size = new System.Drawing.Size(121, 23);
            this.cmbGeometryType.TabIndex = 4;
            // 
            // labelGeometryType
            // 
            this.labelGeometryType.AutoSize = true;
            this.labelGeometryType.Location = new System.Drawing.Point(234, 86);
            this.labelGeometryType.Name = "labelGeometryType";
            this.labelGeometryType.Size = new System.Drawing.Size(67, 15);
            this.labelGeometryType.TabIndex = 3;
            this.labelGeometryType.Text = "几何类型";
            // 
            // btnFormNewSelectPath
            // 
            this.btnFormNewSelectPath.Location = new System.Drawing.Point(579, 36);
            this.btnFormNewSelectPath.Name = "btnFormNewSelectPath";
            this.btnFormNewSelectPath.Size = new System.Drawing.Size(79, 30);
            this.btnFormNewSelectPath.TabIndex = 2;
            this.btnFormNewSelectPath.Text = "选择";
            this.btnFormNewSelectPath.UseVisualStyleBackColor = true;
            this.btnFormNewSelectPath.Click += new System.EventHandler(this.btnFormNewSelectPath_Click);
            // 
            // txtFormNewPath
            // 
            this.txtFormNewPath.Location = new System.Drawing.Point(97, 39);
            this.txtFormNewPath.Name = "txtFormNewPath";
            this.txtFormNewPath.ReadOnly = true;
            this.txtFormNewPath.Size = new System.Drawing.Size(471, 25);
            this.txtFormNewPath.TabIndex = 1;
            // 
            // labelFormNewPath
            // 
            this.labelFormNewPath.AutoSize = true;
            this.labelFormNewPath.Location = new System.Drawing.Point(22, 44);
            this.labelFormNewPath.Name = "labelFormNewPath";
            this.labelFormNewPath.Size = new System.Drawing.Size(67, 15);
            this.labelFormNewPath.TabIndex = 0;
            this.labelFormNewPath.Text = "存储目录";
            // 
            // formNewGroupBox2
            // 
            this.formNewGroupBox2.Controls.Add(this.dataGridViewField);
            this.formNewGroupBox2.Location = new System.Drawing.Point(13, 184);
            this.formNewGroupBox2.Name = "formNewGroupBox2";
            this.formNewGroupBox2.Size = new System.Drawing.Size(684, 418);
            this.formNewGroupBox2.TabIndex = 1;
            this.formNewGroupBox2.TabStop = false;
            this.formNewGroupBox2.Text = "  字段列表  ";
            // 
            // dataGridViewField
            // 
            this.dataGridViewField.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewField.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFieldName,
            this.colFieldAlias,
            this.colFieldType,
            this.colFieldLength});
            this.dataGridViewField.Location = new System.Drawing.Point(22, 37);
            this.dataGridViewField.Name = "dataGridViewField";
            this.dataGridViewField.RowHeadersWidth = 51;
            this.dataGridViewField.RowTemplate.Height = 27;
            this.dataGridViewField.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewField.Size = new System.Drawing.Size(636, 350);
            this.dataGridViewField.TabIndex = 0;
            this.dataGridViewField.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewField_CellValueChanged);
            // 
            // colFieldName
            // 
            this.colFieldName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFieldName.HeaderText = "字段名称";
            this.colFieldName.MinimumWidth = 6;
            this.colFieldName.Name = "colFieldName";
            // 
            // colFieldAlias
            // 
            this.colFieldAlias.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFieldAlias.HeaderText = "字段别名";
            this.colFieldAlias.MinimumWidth = 6;
            this.colFieldAlias.Name = "colFieldAlias";
            // 
            // colFieldType
            // 
            this.colFieldType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFieldType.HeaderText = "字段类型";
            this.colFieldType.Items.AddRange(new object[] {
            "整数",
            "数字",
            "日期",
            "文本"});
            this.colFieldType.MinimumWidth = 6;
            this.colFieldType.Name = "colFieldType";
            // 
            // colFieldLength
            // 
            this.colFieldLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colFieldLength.HeaderText = "字段长度";
            this.colFieldLength.MinimumWidth = 6;
            this.colFieldLength.Name = "colFieldLength";
            // 
            // btnDeleteField
            // 
            this.btnDeleteField.Location = new System.Drawing.Point(157, 625);
            this.btnDeleteField.Name = "btnDeleteField";
            this.btnDeleteField.Size = new System.Drawing.Size(105, 33);
            this.btnDeleteField.TabIndex = 2;
            this.btnDeleteField.Text = "删除字段";
            this.btnDeleteField.UseVisualStyleBackColor = true;
            this.btnDeleteField.Click += new System.EventHandler(this.btnDeleteField_Click);
            // 
            // btnClearField
            // 
            this.btnClearField.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClearField.Location = new System.Drawing.Point(279, 625);
            this.btnClearField.Name = "btnClearField";
            this.btnClearField.Size = new System.Drawing.Size(105, 33);
            this.btnClearField.TabIndex = 3;
            this.btnClearField.Text = "清空字段";
            this.btnClearField.UseVisualStyleBackColor = true;
            this.btnClearField.Click += new System.EventHandler(this.btnClearField_Click);
            // 
            // btnConfirmNew
            // 
            this.btnConfirmNew.Location = new System.Drawing.Point(566, 625);
            this.btnConfirmNew.Name = "btnConfirmNew";
            this.btnConfirmNew.Size = new System.Drawing.Size(105, 33);
            this.btnConfirmNew.TabIndex = 4;
            this.btnConfirmNew.Text = "确认创建";
            this.btnConfirmNew.UseVisualStyleBackColor = true;
            this.btnConfirmNew.Click += new System.EventHandler(this.btnConfirmNew_Click);
            // 
            // btnAddField
            // 
            this.btnAddField.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAddField.Location = new System.Drawing.Point(35, 625);
            this.btnAddField.Name = "btnAddField";
            this.btnAddField.Size = new System.Drawing.Size(105, 33);
            this.btnAddField.TabIndex = 5;
            this.btnAddField.Text = "添加字段";
            this.btnAddField.UseVisualStyleBackColor = true;
            this.btnAddField.Click += new System.EventHandler(this.btnAddField_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(444, 625);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(105, 33);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "取消创建";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormNewFeatureClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(709, 679);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAddField);
            this.Controls.Add(this.btnConfirmNew);
            this.Controls.Add(this.btnClearField);
            this.Controls.Add(this.btnDeleteField);
            this.Controls.Add(this.formNewGroupBox2);
            this.Controls.Add(this.formNewGroupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormNewFeatureClass";
            this.Text = "创建SHP文件";
            this.Load += new System.EventHandler(this.FormNewFeatureClass_Load_1);
            this.formNewGroupBox1.ResumeLayout(false);
            this.formNewGroupBox1.PerformLayout();
            this.formNewGroupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewField)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox formNewGroupBox1;
        private System.Windows.Forms.GroupBox formNewGroupBox2;
        private System.Windows.Forms.Button btnFormNewSelectPath;
        private System.Windows.Forms.TextBox txtFormNewPath;
        private System.Windows.Forms.Label labelFormNewPath;
        private System.Windows.Forms.Label labelGeometryType;
        private System.Windows.Forms.ComboBox cmbGeometryType;
        private System.Windows.Forms.Label labelSR;
        private System.Windows.Forms.ComboBox cmbSR;
        private System.Windows.Forms.DataGridView dataGridViewField;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFieldAlias;
        private System.Windows.Forms.DataGridViewComboBoxColumn colFieldType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFieldLength;
        private System.Windows.Forms.Button btnDeleteField;
        private System.Windows.Forms.Button btnClearField;
        private System.Windows.Forms.Button btnConfirmNew;
        private System.Windows.Forms.Button btnAddField;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelFileName;
        private System.Windows.Forms.TextBox txtFileName;
    }
}