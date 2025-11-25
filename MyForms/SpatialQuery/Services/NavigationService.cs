using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Lab04_4.MyForms.SpatialQuery.Helpers;
using Lab04_4.MyForms.FeatureClassManagement.Helpers;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.SpatialQuery.Services
{
    /// <summary>
    /// 导航服务，提供要素导航功能
    /// </summary>
    public class NavigationService
    {
        private readonly AxMapControl _mapControl;

        public NavigationService(AxMapControl mapControl)
        {
            _mapControl = mapControl;
        }

        /// <summary>
        /// 显示导航选项对话框
        /// </summary>
        public void ShowNavigationOptions(string maxFeatureName, string minFeatureName,
            Action zoomToMaxAction, Action zoomToMinAction)
        {
            try
            {
                var dialog = CreateNavigationDialog(maxFeatureName, minFeatureName, zoomToMaxAction, zoomToMinAction);
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error("显示导航选项失败", ex);
                MessageBox.Show("显示导航选项时发生错误", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 创建导航对话框
        /// </summary>
        private Form CreateNavigationDialog(string maxFeatureName, string minFeatureName,
            Action zoomToMaxAction, Action zoomToMinAction)
        {
            var form = new Form
            {
                Text = "导航选项",
                Size = new System.Drawing.Size(450, 200),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var label = new Label
            {
                Text = $"已高亮显示最大和最小面积要素。\n\n最大面积要素: {maxFeatureName}\n最小面积要素: {minFeatureName}",
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(420, 80),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 9f)
            };

            var btnOK = new Button
            {
                Text = "确定",
                Location = new System.Drawing.Point(50, 100),
                Size = new System.Drawing.Size(80, 30),
                DialogResult = DialogResult.OK
            };

            var btnMax = new Button
            {
                Text = "查看最大面积要素",
                Location = new System.Drawing.Point(150, 100),
                Size = new System.Drawing.Size(120, 30)
            };
            btnMax.Click += (s, e) => { zoomToMaxAction?.Invoke(); form.Close(); };

            var btnMin = new Button
            {
                Text = "查看最小面积要素",
                Location = new System.Drawing.Point(280, 100),
                Size = new System.Drawing.Size(120, 30)
            };
            btnMin.Click += (s, e) => { zoomToMinAction?.Invoke(); form.Close(); };

            form.Controls.AddRange(new Control[] { label, btnOK, btnMax, btnMin });

            return form;
        }

        /// <summary>
        /// 缩放到单个要素
        /// </summary>
        public bool ZoomToFeature(IFeatureLayer featureLayer, int featureOID)
        {
            try
            {
                if (_mapControl == null || featureLayer == null || featureOID == -1)
                {
                    return false;
                }

                IFeatureClass featureClass = featureLayer.FeatureClass;
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = $"{featureClass.OIDFieldName} = {featureOID}";

                using (var featureCursor = new ComObjectWrapper<IFeatureCursor>(featureClass.Search(queryFilter, false)))
                {
                    IFeature feature = featureCursor.Object.NextFeature();
                    if (feature?.Shape == null) return false;

                    IEnvelope featureExtent = feature.Shape.Envelope;
                    if (featureExtent.IsEmpty) return false;

                    // 稍微扩大范围以便更好地查看
                    featureExtent.Expand(1.2, 1.2, true);
                    _mapControl.Extent = featureExtent;

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"缩放到要素失败: {ex.Message}");
                return false;
            }
        }
    }
}
