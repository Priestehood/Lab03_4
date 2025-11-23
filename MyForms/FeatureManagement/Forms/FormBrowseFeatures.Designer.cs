
namespace Lab03_4.MyForms.FeatureManagement.Forms
{
    partial class FormBrowseFeatures
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
            this.dgvFeatures = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeatures)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvFeatures
            // 
            this.dgvFeatures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFeatures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFeatures.Location = new System.Drawing.Point(0, 0);
            this.dgvFeatures.Name = "dgvFeatures";
            this.dgvFeatures.ReadOnly = true;
            this.dgvFeatures.RowHeadersWidth = 62;
            this.dgvFeatures.RowTemplate.Height = 30;
            this.dgvFeatures.Size = new System.Drawing.Size(800, 450);
            this.dgvFeatures.TabIndex = 0;
            // 
            // FormBrowseFeatures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvFeatures);
            this.Name = "FormBrowseFeatures";
            this.Text = "要素浏览";
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeatures)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvFeatures;
    }
}