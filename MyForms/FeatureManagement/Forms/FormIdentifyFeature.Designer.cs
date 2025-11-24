
namespace Lab04_4.MyForms.FeatureManagement.Forms
{
    partial class FormIdentifyFeature
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
            this.tblFieldValue = new Lab04_4.MyForms.FeatureManagement.Forms.FieldValueControl();
            this.SuspendLayout();
            // 
            // tblFieldValue
            // 
            this.tblFieldValue.Fields = null;
            this.tblFieldValue.Location = new System.Drawing.Point(36, 12);
            this.tblFieldValue.Name = "tblFieldValue";
            this.tblFieldValue.Size = new System.Drawing.Size(795, 603);
            this.tblFieldValue.TabIndex = 5;
            // 
            // FormIdentifyFeature
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 636);
            this.Controls.Add(this.tblFieldValue);
            this.Name = "FormIdentifyFeature";
            this.Text = "要素信息";
            this.ResumeLayout(false);

        }

        #endregion

        private FieldValueControl tblFieldValue;
    }
}