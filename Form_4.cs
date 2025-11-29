using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.ComponentModel;
using System.Drawing;
using Lab04_4.MyForms.SpatialQuery.Services;
using System.Windows.Forms;


namespace Lab04_4
{
    public partial class Form_4 : Form
    {
        private ILayer m_selectedLayer; // 当前选中的图层
        IEnvelope ext = null; // 主地图显示范围

        public Form_4()
        {
            InitializeComponent();
            this.Load += Form_4_Load;
            InitializeFeatureClassManagement();
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

        #region 工具栏-图层
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

        #region 菜单-要素类管理

        private void menuFeatureClassNew_Click(object sender, EventArgs e)
        {
            CreateNewFeatureClass();
        }

        private void menuFeatureClassEdit_Click(object sender, EventArgs e)
        {
            EditFeatureClassFields();
        }

        private void menuFeatureClassDelete_Click(object sender, EventArgs e)
        {
            DeleteFeatureClass();
        }

        #endregion

        #region 工具栏-要素类管理

        private void tlbFeatureClassNew_Click(object sender, EventArgs e)
        {
            CreateNewFeatureClass();
        }

        private void tlbFeatureClassEdit_Click(object sender, EventArgs e)
        {
            EditFeatureClassFields();
        }

        private void tlbFeatureClassDelete_Click(object sender, EventArgs e)
        {
            DeleteFeatureClass();
        }

        #endregion

        #region 菜单-要素管理

        private void menuFeatureNew_Click(object sender, EventArgs e)
        {
            BeginCreateNewFeature();
        }

        private void menuFeatureEditByLocation_Click(object sender, EventArgs e)
        {
            BeginEditFeature("Location");
        }

        private void menuFeatureEditByRectangle_Click(object sender, EventArgs e)
        {
            BeginEditFeature("Rectangle");
        }

        private void menuFeatureEditByPolygon_Click(object sender, EventArgs e)
        {
            BeginEditFeature("Polygon");
        }

        private void menuFeatureDeleteByLocation_Click(object sender, EventArgs e)
        {
            BeginDeleteFeature("Location");
        }

        private void menuFeatureDeleteByRectangle_Click(object sender, EventArgs e)
        {
            BeginDeleteFeature("Rectangle");
        }

        private void menuFeatureDeleteByPolygon_Click(object sender, EventArgs e)
        {
            BeginDeleteFeature("Polygon");
        }

        private void menuFeatureBrowse_Click(object sender, EventArgs e)
        {
            BrowseFeatures();
        }

        private void menuFeatureIdentify_Click(object sender, EventArgs e)
        {
            BeginIdentifyFeature();
        }


        #endregion

        #region 工具栏-要素管理

        private void tlbFeatureNew_Click(object sender, EventArgs e)
        {
            BeginCreateNewFeature();
        }

        private void tlbFeatureEditByLocation_Click(object sender, EventArgs e)
        {
            BeginEditFeature("Location");
        }

        private void tlbFeatureEditByRectangle_Click(object sender, EventArgs e)
        {
            BeginEditFeature("Rectangle");
        }

        private void tlbFeatureEditByPolygon_Click(object sender, EventArgs e)
        {
            BeginEditFeature("Polygon");
        }

        private void tlbFeatureDeleteByLocation_Click(object sender, EventArgs e)
        {
            BeginDeleteFeature("Location");
        }

        private void tlbFeatureDeleteByRectangle_Click(object sender, EventArgs e)
        {
            BeginDeleteFeature("Rectangle");
        }

        private void tlbFeatureDeleteByPolygon_Click(object sender, EventArgs e)
        {
            BeginDeleteFeature("Polygon");
        }

        private void tlbFeatureBrowse_Click(object sender, EventArgs e)
        {
            BrowseFeatures();
        }

        private void tlbFeatureIdentify_Click(object sender, EventArgs e)
        {
            BeginIdentifyFeature();
        }

        #endregion

        // Lab04_4

        #region 菜单-空间查询

        private void menuSQQueryAreaExtremeValue_Click(object sender, EventArgs e)
        {
            QueryAreaExtremeValue();
        }

        #endregion

        #region 工具栏-空间查询

        private void tlbSQAreaExtremeValue_Click(object sender, EventArgs e)
        {
            QueryAreaExtremeValue();
        }

        #endregion

        #region 菜单-高程分析

        private void menuEAElevationPointFiltering_Click(object sender, EventArgs e)
        {
            FilterAbnormalElevations();
        }

        #endregion

        #region 工具栏-高程分析

        private void tlbEAElevationPointFiltering_Click(object sender, EventArgs e)
        {
            FilterAbnormalElevations();
        }

        #endregion

        #region 鼠标点击位置的高程
        private void axMap_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            IPoint clickPoint = new PointClass();
            clickPoint.PutCoords(e.mapX, e.mapY);

            IFeatureLayer elevationLayer = axMap.get_Layer(0) as IFeatureLayer;

            if (elevationLayer == null)
            {
                MessageBox.Show("未找到高程图层!");
                return;
            }

            ElevationAnalysis analysis = new ElevationAnalysis(elevationLayer);
            double result = analysis.IntepolateElevation(clickPoint, 8);

            MessageBox.Show($"插值高程：{result:F2} 米");
        }
        # endregion
    }

}