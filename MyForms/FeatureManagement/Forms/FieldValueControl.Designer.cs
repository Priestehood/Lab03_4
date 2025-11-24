
namespace Lab04_4.MyForms.FeatureManagement.Forms
{
    partial class FieldValueControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvFields = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FieldValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvFields
            // 
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FieldName,
            this.FieldValue});
            this.dgvFields.Location = new System.Drawing.Point(17, 30);
            this.dgvFields.Name = "dgvFields";
            this.dgvFields.RowHeadersWidth = 72;
            this.dgvFields.RowTemplate.Height = 33;
            this.dgvFields.Size = new System.Drawing.Size(749, 542);
            this.dgvFields.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgvFields);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(789, 600);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "字段列表";
            // 
            // FieldName
            // 
            this.FieldName.HeaderText = "字段名称";
            this.FieldName.MinimumWidth = 9;
            this.FieldName.Name = "FieldName";
            this.FieldName.ReadOnly = true;
            this.FieldName.Width = 175;
            // 
            // FieldValue
            // 
            this.FieldValue.HeaderText = "数据值";
            this.FieldValue.MinimumWidth = 9;
            this.FieldValue.Name = "FieldValue";
            this.FieldValue.Width = 175;
            // 
            // FieldValueControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FieldValueControl";
            this.Size = new System.Drawing.Size(795, 603);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFields;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FieldValue;
    }
}
