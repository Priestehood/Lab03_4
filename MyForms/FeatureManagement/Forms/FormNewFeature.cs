using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Windows.Forms;

namespace Lab04_4.MyForms.FeatureManagement.Forms
{
    public partial class FormNewFeature : FormFeatureBase
    {
        #region 私有成员
        private IFeatureLayer layer;
        #endregion

        #region 重写基类虚属性 - 使用不同的命名避免冲突
        protected override FieldValueControl BaseTblFieldValue => this.tblFieldValue;

        protected override Button BaseBtnCancel => this.btnCancel;

        protected override Button BaseBtnConfirm => this.btnConfirm;
        #endregion

        #region 构造函数
        public FormNewFeature(IFeatureLayer featureLayer)
        {
            InitializeComponent();
            this.layer = featureLayer;
            DisplayFeatureClassInfo();
        }
        #endregion

        #region 重写基类虚方法
        /// <summary>
        /// 显示要素类路径及字段列表
        /// </summary>
        protected virtual void DisplayFeatureClassInfo()
        {
            IFeatureClass featureClass = layer.FeatureClass;
            try
            {
                txtSource.Text = Helpers.LayerHelper.GetDataSource(layer);
                tblFieldValue.Fields = featureClass.Fields;
            }
            catch (Exception ex)
            {
                MessageBox.Show("[异常] " + ex.Message, "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        #endregion

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
