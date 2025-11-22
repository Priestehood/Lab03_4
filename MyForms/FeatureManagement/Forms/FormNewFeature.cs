using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab03_4.MyForms.FeatureManagement.Forms
{
    public partial class FormNewFeature : Form
    {
        #region 私有成员和公开属性
        IFeatureLayer layer;
        public IFeatureLayer Layer
        {
            set
            {
                this.layer = value;
                DisplayFeatureClassInfo();
            }
        }
        #endregion

        #region 构造函数
        public FormNewFeature(IFeatureLayer featureLayer, IGeometry geometry)
        {
            InitializeComponent();
            this.Layer = featureLayer;
        }
        #endregion

        /// <summary>
        /// 显示要素类路径及字段列表
        /// </summary>
        private void DisplayFeatureClassInfo()
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

        /// <summary>
        /// 设置要素类的字段属性值
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="featureClass"></param>
        public void SetFeatureFields(IFeature feature, IFeatureClass featureClass)
        {
            tblFieldValue.SetFeatureFields(feature, featureClass);
        }

        #region 按钮点击事件响应函数
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
        #endregion
    }
}
