using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Lab04_4.MyForms.FeatureManagement.Forms
{
    public partial class FormFeatureBase : Form
    {
        #region 虚属性 - 子类可以重写
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual FieldValueControl BaseTblFieldValue => null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Button BaseBtnCancel => null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected virtual Button BaseBtnConfirm => null;
        #endregion

        #region 虚方法 - 子类可以重写

        /// <summary>
        /// 设置要素类的字段属性值
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="featureClass"></param>
        public virtual void SetFeatureFields(IFeature feature)
        {
            BaseTblFieldValue.SetFeatureFields(feature);
        }
        #endregion

        #region 共享的事件处理方法
        protected virtual void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        protected virtual void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
        #endregion
    }
}
