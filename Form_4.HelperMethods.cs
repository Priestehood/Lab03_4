using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Lab03_4
{
    public partial class Form_4 : Form
    {
        #region 辅助方法

        /// <summary>
        /// 坐标系统判断方法
        /// </summary>
        private bool IsProjectedCoordinateSystem(ISpatialReference spatialRef)
        {
            try
            {
                // 方法1：直接类型转换
                if (spatialRef is IProjectedCoordinateSystem)
                    return true;

                // 方法2：通过接口查询
                IProjectedCoordinateSystem4 projCS = spatialRef as IProjectedCoordinateSystem4;
                if (projCS != null)
                    return true;

                // 方法3：检查坐标系类型枚举
                if (spatialRef is IProjectedCoordinateSystem)
                    return true;

                // 方法4：通过工厂代码判断
                int factoryCode = spatialRef.FactoryCode;

                // 常见的地理坐标系代码范围（WGS84, Beijing54, Xian80, CGCS2000 等）
                int[] geographicCodes = { 4326, 4214, 4610, 4490, 4019, 4269, 4283, 4284, 4301, 4304,
                    4322, 4324, 4202, 4203, 4204, 4205, 4206, 4207, 4208, 4209, 4210, 4211, 4212, 4213 };

                if (geographicCodes.Contains(factoryCode))
                    return false;

                // 方法5：检查坐标单位
                IUnit unit = spatialRef.ZCoordinateUnit;
                if (unit != null)
                {
                    string unitName = unit.Name.ToLower();
                    // 投影坐标系常用单位
                    string[] projectedUnits = { "meter", "metre", "foot", "feet", "us foot", "international foot", "yard" };
                    // 地理坐标系常用单位
                    string[] geographicUnits = { "degree", "grad", "radian" };

                    if (projectedUnits.Any(u => unitName.Contains(u)))
                        return true;
                    if (geographicUnits.Any(u => unitName.Contains(u)))
                        return false;
                }

                // 方法6：默认根据坐标值范围判断
                IEnvelope fullExtent = axMap.FullExtent;
                if (fullExtent != null)
                {
                    double width = Math.Abs(fullExtent.XMax - fullExtent.XMin);
                    double height = Math.Abs(fullExtent.YMax - fullExtent.YMin);

                    // 如果范围很大（超过经纬度范围），很可能是投影坐标
                    if (width > 360 || height > 180)
                        return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取在 TOC 中选中的图层
        /// </summary>
        private ILayer GetSelectedLayer()
        {
            IBasicMap basicMap = null;
            ILayer layer = null;
            object other = null;
            object index = null;
            esriTOCControlItem itemType = esriTOCControlItem.esriTOCControlItemLayer;

            axTOC.GetSelectedItem(ref itemType, ref basicMap, ref layer, ref other, ref index);
            return layer;
        }

        /// <summary>
        /// 获取图层在地图中的索引
        /// </summary>
        private int GetLayerIndex(ILayer layer)
        {
            for (int i = 0; i < axMap.LayerCount; i++)
            {
                if (axMap.get_Layer(i) == layer)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 更新状态栏信息
        /// </summary>
        private void UpdateStatus(string message)
        {
            tslMain.Text = message;
            Refresh();
        }

        /// <summary>
        /// 更新菜单状态（如隐藏/显示文字）
        /// </summary>
        private void UpdateMenuStatus()
        {
            if (m_selectedLayer != null)
            {
                string visibleText = m_selectedLayer.Visible ? "隐藏" : "显示";
                menuLayerVisible.Text = visibleText;
                tlbLayerVisible.Text = visibleText;
                tsmVisible.Text = visibleText;
            }
        }

        /// <summary>
        /// 更新选择状态显示的方法
        /// </summary>
        private void UpdateSelectionStatusDisplay()
        {
            if (tslSelectionStatus != null)
            {
                string selectableLayer = GetCurrentSelectableLayerName();
                tslSelectionStatus.Text = $"可选图层: {selectableLayer}";

                // 根据状态设置不同颜色
                if (selectableLayer == "无")
                {
                    tslSelectionStatus.ForeColor = Color.Red;
                }
                else
                {
                    tslSelectionStatus.ForeColor = Color.Green;
                }
            }
        }

        #endregion
    }
}