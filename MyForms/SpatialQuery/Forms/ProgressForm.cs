using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Lab04_4.MyForms.SpatialQuery.Forms
{
    public partial class ProgressForm : Form
    {
        public event EventHandler CancelRequested;
        public ProgressForm(string title = "进度")
        {
            InitializeComponent();
            this.Text = title;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
        }

        public void UpdateProgress(int percent, string message)
        {
            progressBar1.Value = percent;
            lblProgress.Text = $"{percent}%";
            lblStatus.Text = message;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
