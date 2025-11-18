
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
            this.formNewGroupBox2 = new System.Windows.Forms.GroupBox();
            this.labelFormNewPath = new System.Windows.Forms.Label();
            this.txtFormNewPath = new System.Windows.Forms.TextBox();
            this.btnFormNewSelectPath = new System.Windows.Forms.Button();
            this.labelShpType = new System.Windows.Forms.Label();
            this.comboBoxShpType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.formNewGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // formNewGroupBox1
            // 
            this.formNewGroupBox1.Controls.Add(this.label1);
            this.formNewGroupBox1.Controls.Add(this.comboBoxShpType);
            this.formNewGroupBox1.Controls.Add(this.labelShpType);
            this.formNewGroupBox1.Controls.Add(this.btnFormNewSelectPath);
            this.formNewGroupBox1.Controls.Add(this.txtFormNewPath);
            this.formNewGroupBox1.Controls.Add(this.labelFormNewPath);
            this.formNewGroupBox1.Location = new System.Drawing.Point(13, 13);
            this.formNewGroupBox1.Name = "formNewGroupBox1";
            this.formNewGroupBox1.Size = new System.Drawing.Size(684, 133);
            this.formNewGroupBox1.TabIndex = 0;
            this.formNewGroupBox1.TabStop = false;
            this.formNewGroupBox1.Text = "  文件属性  ";
            // 
            // formNewGroupBox2
            // 
            this.formNewGroupBox2.Location = new System.Drawing.Point(13, 173);
            this.formNewGroupBox2.Name = "formNewGroupBox2";
            this.formNewGroupBox2.Size = new System.Drawing.Size(684, 494);
            this.formNewGroupBox2.TabIndex = 1;
            this.formNewGroupBox2.TabStop = false;
            this.formNewGroupBox2.Text = "  字段列表  ";
            // 
            // labelFormNewPath
            // 
            this.labelFormNewPath.AutoSize = true;
            this.labelFormNewPath.Location = new System.Drawing.Point(22, 38);
            this.labelFormNewPath.Name = "labelFormNewPath";
            this.labelFormNewPath.Size = new System.Drawing.Size(67, 15);
            this.labelFormNewPath.TabIndex = 0;
            this.labelFormNewPath.Text = "保存路径";
            // 
            // txtFormNewPath
            // 
            this.txtFormNewPath.Location = new System.Drawing.Point(97, 33);
            this.txtFormNewPath.Name = "txtFormNewPath";
            this.txtFormNewPath.ReadOnly = true;
            this.txtFormNewPath.Size = new System.Drawing.Size(471, 25);
            this.txtFormNewPath.TabIndex = 1;
            // 
            // btnFormNewSelectPath
            // 
            this.btnFormNewSelectPath.Location = new System.Drawing.Point(579, 30);
            this.btnFormNewSelectPath.Name = "btnFormNewSelectPath";
            this.btnFormNewSelectPath.Size = new System.Drawing.Size(79, 30);
            this.btnFormNewSelectPath.TabIndex = 2;
            this.btnFormNewSelectPath.Text = "选择";
            this.btnFormNewSelectPath.UseVisualStyleBackColor = true;
            // 
            // labelShpType
            // 
            this.labelShpType.AutoSize = true;
            this.labelShpType.Location = new System.Drawing.Point(23, 86);
            this.labelShpType.Name = "labelShpType";
            this.labelShpType.Size = new System.Drawing.Size(67, 15);
            this.labelShpType.TabIndex = 3;
            this.labelShpType.Text = "几何类型";
            // 
            // comboBoxShpType
            // 
            this.comboBoxShpType.FormattingEnabled = true;
            this.comboBoxShpType.Items.AddRange(new object[] {
            "点",
            "线",
            "面"});
            this.comboBoxShpType.Location = new System.Drawing.Point(97, 82);
            this.comboBoxShpType.Name = "comboBoxShpType";
            this.comboBoxShpType.Size = new System.Drawing.Size(121, 23);
            this.comboBoxShpType.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(294, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "坐标系统";
            // 
            // FormNewFeatureClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 679);
            this.Controls.Add(this.formNewGroupBox2);
            this.Controls.Add(this.formNewGroupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormNewFeatureClass";
            this.Text = "创建SHP文件";
            this.formNewGroupBox1.ResumeLayout(false);
            this.formNewGroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox formNewGroupBox1;
        private System.Windows.Forms.GroupBox formNewGroupBox2;
        private System.Windows.Forms.Button btnFormNewSelectPath;
        private System.Windows.Forms.TextBox txtFormNewPath;
        private System.Windows.Forms.Label labelFormNewPath;
        private System.Windows.Forms.Label labelShpType;
        private System.Windows.Forms.ComboBox comboBoxShpType;
        private System.Windows.Forms.Label label1;
    }
}