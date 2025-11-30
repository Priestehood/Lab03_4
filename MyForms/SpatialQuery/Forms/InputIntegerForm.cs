using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04_4.MyForms.SpatialQuery.Forms
{
    public partial class InputIntegerForm : Form
    {
        private int integer;
        public int Value
        {
            get => integer;
        }

        public InputIntegerForm(
            string prompt = "请输入参数：",
            string placeholderValue = null,
            string title = "指定参数")
        {
            InitializeComponent();
            this.Text = title;
            lblPrompt.Text = prompt;
            if (placeholderValue != null)
            {
                txtInput.Text = placeholderValue;
            }
            this.integer = 0;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text;
            try
            {
                this.integer = int.Parse(input);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入的参数数值不是合法的整数，请重新输入", "请检查输入");
            }
        }
    }
}
