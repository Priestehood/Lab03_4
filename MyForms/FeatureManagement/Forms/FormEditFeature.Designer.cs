
namespace Lab04_4.MyForms.FeatureManagement.Forms
{
    partial class FormEditFeature
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
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tblFieldValue = new Lab04_4.MyForms.FeatureManagement.Forms.FieldValueControl();
            this.SuspendLayout();
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(222, 657);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(125, 41);
            this.btnConfirm.TabIndex = 6;
            this.btnConfirm.Text = "确认";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(517, 657);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(125, 41);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tblFieldValue
            // 
            this.tblFieldValue.Fields = null;
            this.tblFieldValue.Location = new System.Drawing.Point(41, 31);
            this.tblFieldValue.Name = "tblFieldValue";
            this.tblFieldValue.Size = new System.Drawing.Size(795, 603);
            this.tblFieldValue.TabIndex = 4;
            // 
            // FormEditFeature
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 736);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tblFieldValue);
            this.Name = "FormEditFeature";
            this.Text = "编辑要素";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private FieldValueControl tblFieldValue;
    }
}