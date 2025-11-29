using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.ComponentModel;
using System.Drawing;
using Lab04_4.MyForms.SpatialQuery.Services;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;



namespace Lab04_4
{
    public partial class Form_4 : Form
    {
        private ILayer m_selectedLayer; // 当前选中的图层
        IEnvelope ext = null; // 主地图显示范围
        private SpatialQueryTool _spatialQueryTool;// 声明步骤3+4的封装类实例
        private int _lastLayerCount = -1;// 用于检测图层是否变化
        private ElevationAnalysis _elevationAnalysis;
        private bool _isElevationQueryMode = false;//检测是否带调用点击显示高程按钮



        public Form_4()
        {
            InitializeComponent();
            this.Load += Form_4_Load;
            InitializeFeatureClassManagement();
      
        }
        // 在地图控件加载完成后执行
       

        #region 窗体加载和初始化
        private void Form_4_Load(object sender, EventArgs e)
        {
            


            // 动态设置缩放大小
            menu.ImageScalingSize = new Size(16, 16);
            tool.ImageScalingSize = new Size(16, 16);

            //关闭滚轮缩放功能
            this.axThum.AutoMouseWheel = false;

            // 1) 先创建工具（不进行强制识别）
            _spatialQueryTool = new SpatialQueryTool(axMap);

            // 订阅地图事件：当地图被替换或添加图层时静默尝试识别（不会弹窗）
            axMap.OnMapReplaced += AxMapControl1_OnMapReplaced;

            // 地图刷新后触发，用来检测是否新增图层
            axMap.OnAfterScreenDraw += AxMapControl1_OnAfterScreenDraw;
            // Form_4_Load 或者合适位置
            _elevationAnalysis = new ElevationAnalysis(); // 不传图层
            axMap.OnMouseDown += axMap_OnMouseDown;




        }

        private void AxMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            _spatialQueryTool.EnsureLayersAssigned(true);
            _lastLayerCount = axMap.LayerCount;
        }


        private void AxMapControl1_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            // 如果图层数量变化 → 自动重新识别
            if (_lastLayerCount != axMap.LayerCount)
            {
                _lastLayerCount = axMap.LayerCount;
                _spatialQueryTool.EnsureLayersAssigned(true);
            }
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

        #region 点击地图显示高程

        /// <summary>
        /// 菜单：menuElevationQuery（步骤6：查询指定点高程）
        /// 功能：启用/禁用高程查询模式，与其他模式互斥
        /// </summary>
        private void menuEAQueryElevation_Click(object sender, EventArgs e)
        {
            _isElevationQueryMode = !_isElevationQueryMode;
            menuEAQueryElevation.Checked = _isElevationQueryMode;

            // 模式互斥：禁用所有空间查询模式
            if (_isElevationQueryMode)
            {
                // 禁用空间查询的3个模式
                _spatialQueryTool.ToggleElementQueryMode(false);
                _spatialQueryTool.ToggleDrawPolylineMode(false);
                _spatialQueryTool.ToggleBufferQueryMode(false);
                // 取消空间查询控件的勾选状态
                menuSQElementClickQuery.Checked = false;
                menuSQDrawAPolyline.Checked = false;
                menuSQBufferAnalysis.Checked = false;
                status.Items[0].Text = "当前模式：高程查询（左键点击地图获取高程）";
            }
            else
            {
                status.Items[0].Text = "当前模式：默认（请选择功能模式）";
            }
        }

        /// <summary>
        /// 高程查询执行逻辑（独立封装）
        /// </summary>
        private void ExecuteElevationQuery(IMapControlEvents2_OnMouseDownEvent e)
        {
            // 这里放入你原有的高程查询代码（比如绑定图层、插值计算等）
            bool layerFound = _elevationAnalysis.elevPointLayer != null
                              || _elevationAnalysis.AssignLayersAutomatically(axMap);

            if (!layerFound)
            {
                MessageBox.Show("⚠ 没有高程点图层，请先加载高程数据！", "提示");
                return;
            }

            IPoint clickPoint = new PointClass();
            clickPoint.PutCoords(e.mapX, e.mapY);

            try
            {
                double elevation = _elevationAnalysis.IntepolateElevation(clickPoint, 8);
                MessageBox.Show($"当前位置高程：{elevation:F2} 米", "插值结果");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误");
            }
        }

        private void axMap_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button != 1) return;

            // 先检查 _spatialQueryTool 是否初始化
            if (_spatialQueryTool == null)
            {
                MessageBox.Show("空间查询工具未初始化！");
                return;
            }

            bool isClickQuery = _spatialQueryTool != null && _spatialQueryTool.IsElementQueryActive;
            bool isDrawPolyline = _spatialQueryTool != null && _spatialQueryTool.IsDrawPolylineActive;
            // bool isBufferQuery = _spatialQueryTool != null && _spatialQueryTool.IsBufferQueryActive;

            if (isClickQuery || isDrawPolyline)
            {
                _spatialQueryTool.OnMapMouseDown(e);
            }
            else if (_isElevationQueryMode)
            {
                ExecuteElevationQuery(e); // 高程查询逻辑
            }
        }

        #endregion

        #region 菜单-绘制多义线及查询缓冲要素
        private void menuSQBufferAnalysis_Click(object sender, EventArgs e)
        {
            _spatialQueryTool.MenuClick_BufferAnalysis();
        }

        private void menuSQDrawAPolyline_Click(object sender, EventArgs e)
        {
            _spatialQueryTool.MenuClick_DrawPolyline();
        }
        

        private void menuSQElementClickQuery_Click(object sender, EventArgs e)
        {
            _spatialQueryTool.MenuClick_ElementQuery();
        }

        private void menuSpatialQuery_Click(object sender, EventArgs e)
        {

        }
        #endregion

        
    }

}