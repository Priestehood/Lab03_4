using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Windows.Forms;

namespace Lab03_4.MyForms.FeatureManagement.Forms
{
    public partial class FormEditFeature : FormFeatureBase
    {
        private IFeature feature;

        #region 重写基类虚属性 - 使用不同的命名避免冲突

        protected override FieldValueControl BaseTblFieldValue => this.tblFieldValue;

        protected override Button BaseBtnCancel => this.btnCancel;

        protected override Button BaseBtnConfirm => this.btnConfirm;
        #endregion

        #region 构造函数
        public FormEditFeature(IFeature feature)
        {
            InitializeComponent();
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

        #region 事件处理方法 - 修复Designer错误
        // 保留设计器生成的事件绑定，但重定向到基类方法
        private new void btnCancel_Click(object sender, EventArgs e)
        {
            base.btnCancel_Click(sender, e);
        }

        private new void btnConfirm_Click(object sender, EventArgs e)
        {
            base.btnConfirm_Click(sender, e);
        }
        #endregion
    }
}
