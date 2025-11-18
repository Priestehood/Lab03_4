using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab03_4.MyForms
{
    public partial class FormNewFeatureClass : Form
    {
        public FormNewFeatureClass(IFeatureClass featureClass)
        {
            InitializeComponent();
        }

        public string Folder { get; private set; }
        public string ShpFileName { get; private set; }

        #region 菜单-要素类管理

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnFormNewSelectPath_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnAddField_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnDeleteField_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnClearField_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 创建要素类
        /// </summary>
        private void btnConfirmNew_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 辅助函数

        #endregion
    }
}
