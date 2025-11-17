using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Lab03_4
{
    public partial class Form_4 : Form
    {
        private ILayer m_selectedLayer; // 当前选中的图层
        IEnvelope ext = null; // 主地图显示范围

        public Form_4()
        {
            InitializeComponent();
            this.Load += Form_4_Load;

        }

        #region 窗体加载和初始化
        private void Form_4_Load(object sender, EventArgs e)
        {
            // 动态设置缩放大小
            menu.ImageScalingSize = new Size(16, 16);
            tool.ImageScalingSize = new Size(16, 16);

            //关闭滚轮缩放功能
            this.axThum.AutoMouseWheel = false;
        }
        #endregion

        #region 菜单-文件功能
        private void menuFileNew_Click(object sender, EventArgs e)
        {
            CreateNewMap();
        }

        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            executeCommandByProgId("esriControls.ControlsOpenDocCommand");
        }

        private void menuFileSave_Click(object sender, EventArgs e)
        {
            executeCommandByProgId("esriControls.ControlsSaveAsDocCommand");
        }

        private void menuFileCloseAll_Click(object sender, EventArgs e)
        {
            RemoveAll();
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }
        #endregion

        #region 菜单-图层功能
        private void menuLayerAllShp_Click(object sender, EventArgs e)
        {
            LoadAllShapefiles();
        }

        private void menuLayerAddShp_Click(object sender, EventArgs e)
        {
            AddShapefile();
        }

        private void menuLayerRemove_Click(object sender, EventArgs e)
        {
            RemoveSelectedLayer();
        }

        private void menuLayerSelectable_Click(object sender, EventArgs e)
        {
            SetLayerSelectable();
        }

        private void menuLayerVisible_Click(object sender, EventArgs e)
        {
            ToggleLayerVisibility();
        }

        private void menuLayerThum_Click(object sender, EventArgs e)
        {
            AddLayerToThumbnail();
        }
        #endregion

        #region 菜单-帮助功能
        private void menuHelp_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region 工具栏按钮
        private void tlbLayerAllShp_Click(object sender, EventArgs e)
        {
            LoadAllShapefiles();
        }

        private void tlbLayerAddShp_Click(object sender, EventArgs e)
        {
            AddShapefile();
        }

        private void tlbLayerRemove_Click(object sender, EventArgs e)
        {
            RemoveSelectedLayer();
        }

        private void tlbLayerSelectable_Click(object sender, EventArgs e)
        {
            SetLayerSelectable();
        }

        private void tlbLayerVisible_Click(object sender, EventArgs e)
        {
            ToggleLayerVisibility();
        }

        private void tlbLayerThum_Click(object sender, EventArgs e)
        {
            AddLayerToThumbnail();
        }
        #endregion

        #region TOC右键菜单功能
        private void tsmUp_Click(object sender, EventArgs e)
        {
            MoveLayerUp();
        }

        private void tsmDown_Click(object sender, EventArgs e)
        {
            MoveLayerDown();
        }

        private void tsmRemove_Click(object sender, EventArgs e)
        {
            RemoveSelectedLayer();
        }

        private void tsmSelectable_Click(object sender, EventArgs e)
        {
            SetLayerSelectable();
        }

        private void tsmVisible_Click(object sender, EventArgs e)
        {
            ToggleLayerVisibility();
        }

        private void tsmThum_Click(object sender, EventArgs e)
        {
            AddLayerToThumbnail();
        }

        // TOC鼠标点击事件，记录选中的图层
        private void axTOC_OnMouseDown(object sender, ESRI.ArcGIS.Controls.ITOCControlEvents_OnMouseDownEvent e)
        {
            if (e.button == 2) // 右键
            {
                // 获取选中的图层
                m_selectedLayer = GetSelectedLayer();
                // 显示右键菜单
                if (m_selectedLayer != null)
                {
                    cmTOC.Show(axTOC, new System.Drawing.Point(e.x, e.y));
                }
            }
            else // 左键
            {
                m_selectedLayer = GetSelectedLayer();
                UpdateMenuStatus();
            }
        }
        #endregion

    }
}