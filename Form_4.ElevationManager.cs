using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using Lab04_4.MyForms.ElevationManager.Forms;
using Lab04_4.MyForms.ElevationManager.Helpers;
using Lab04_4.MyForms.ElevationManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04_4
{
    public partial class Form_4 : Form
    {
        #region 私有字段
        private ElevationManagerD _elevationManager;
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化高程数据管理服务
        /// </summary>
        private void InitializeElevationManager()
        {
            _elevationManager = new ElevationManagerD();
        }
        #endregion

        #region 功能实现
        /// <summary>
        /// 加载 DAT 并创建内存图层
        /// </summary>
        private void LoadElevationDat()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "DAT/TXT Files (*.dat;*.txt)|*.dat;*.txt";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                var points = DatFileParser.Parse(ofd.FileName);
                if (points == null || points.Count == 0)
                {
                    MessageBox.Show("未解析到有效点。", "加载失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 使用 map 的 SpatialReference（若存在）
                var map = axMap.Map;
                ISpatialReference sref = null;
                if (map.SpatialReference != null) sref = map.SpatialReference;

                // 创建内存要素类
                var fc = InMemoryFeatureClass.CreatePointFeatureClass("DatElevation_" + Guid.NewGuid().ToString("N"), sref);
                InMemoryFeatureClass.InsertPointsToFeatureClass(fc, points, sref);

                // 创建图层并加入地图
                IFeatureLayer fl = new FeatureLayerClass();
                fl.FeatureClass = fc;
                fl.Name = "高程点(DAT) - " + System.IO.Path.GetFileName(ofd.FileName);
                axMap.Map.AddLayer((ILayer)fl);
                axMap.Refresh();

                // 添加到 manager
                _elevationManager.AddSource(fl, "ZValue", isFromDat: true);
                MessageBox.Show($"加载完成，已导入 {points.Count} 个点并加入高程数据源。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 更新状态栏
                UpdateStatus("已加载高程点数据："+ fl.Name);
            }
        }

        /// <summary>
        /// 设置高程源图层
        /// </summary>
        private void SetElevationLayers()
        {
            var dlg = new FormSelectElevationLayers(_elevationManager);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("高程图层设置已更新。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion
    }
}
