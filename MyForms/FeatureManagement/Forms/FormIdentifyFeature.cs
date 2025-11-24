using ESRI.ArcGIS.Geodatabase;
using System;
using System.Windows.Forms;


namespace Lab04_4.MyForms.FeatureManagement.Forms
{
    public partial class FormIdentifyFeature : FormFeatureBase
    {
        private IFeature feature;

        #region 重写基类虚属性 - 使用不同的命名避免冲突
        protected override FieldValueControl BaseTblFieldValue => this.tblFieldValue;
        #endregion

        #region 构造函数
        public FormIdentifyFeature(IFeature feature)
        {
            InitializeComponent();
            this.tblFieldValue.ReadOnly = true;
            this.feature = feature;
            DisplayFeatureInfo();
        }
        #endregion

        private void DisplayFeatureInfo()
        {
            try
            {
                BaseTblFieldValue.Feature = feature;
            }
            catch (Exception ex)
            {
                MessageBox.Show("[异常] " + ex.Message, "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
