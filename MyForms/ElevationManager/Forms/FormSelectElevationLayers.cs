using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using Lab04_4.MyForms.ElevationManager.Services;

namespace Lab04_4.MyForms.ElevationManager.Forms
{
    /// <summary>
    /// 用于选择/取消高程图层（多选 + 字段选择），只负责 UI，不直接处理逻辑
    /// </summary>
    public partial class FormSelectElevationLayers : Form
    {
        #region 初始化
        private ElevationManagerD _manager;

        public FormSelectElevationLayers(ElevationManagerD manager)
        {
            InitializeComponent();
            _manager = manager;
            LoadData();
        }
        #endregion

        #region 功能实现
        /// <summary>
        /// 加载数据
        /// </summary>
        private void LoadData()
        {
            dgvEA.Rows.Clear();
            foreach (var src in _manager.Sources)
            {
                int idx = dgvEA.Rows.Add();
                dgvEA.Rows[idx].Cells[colEnable.Index].Value = src.Enabled;
                dgvEA.Rows[idx].Cells[colLayerName.Index].Value = src.Layer?.Name ?? "(无名图层)";
                dgvEA.Rows[idx].Cells[colSource.Index].Value = src.IsFromDat ? "DAT" : "SHP";

                var combo = (DataGridViewComboBoxCell)dgvEA.Rows[idx].Cells[colZField.Index];
                combo.Items.Clear();

                if (src.IsFromDat)
                {
                    combo.Items.Add(src.ZField ?? "ZValue");
                    combo.Value = src.ZField ?? "ZValue";
                    combo.ReadOnly = true;
                }
                else
                {
                    // 列出数值字段
                    var fc = src.Layer?.FeatureClass;
                    FillZFieldOptions(combo, fc);
                    // 预选当前值
                    if (!string.IsNullOrEmpty(src.ZField) && combo.Items.Contains(src.ZField))
                        combo.Value = src.ZField;
                    else if (combo.Items.Count > 0)
                        combo.Value = combo.Items[0];
                }
            }
        }

        /// <summary>
        /// ZValue填充
        /// </summary>
        private void FillZFieldOptions(DataGridViewComboBoxCell combo, IFeatureClass fc)
        {
            for (int i = 0; i < fc.Fields.FieldCount; i++)
            {
                var fld = fc.Fields.get_Field(i);
                if (fld.Type == esriFieldType.esriFieldTypeDouble ||
                    fld.Type == esriFieldType.esriFieldTypeSingle ||
                    fld.Type == esriFieldType.esriFieldTypeInteger)
                {
                    combo.Items.Add(fld.Name);
                }
            }
        }

        /// <summary>
        /// ”确认“按钮
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // 将 UI 上的更改写回 manager.Sources（逐项对应）
            for (int i = 0; i < _manager.Sources.Count && i < dgvEA.Rows.Count; i++)
            {
                var src = _manager.Sources[i];
                var row = dgvEA.Rows[i];

                // Enable
                src.Enabled = Convert.ToBoolean(row.Cells[colEnable.Index].Value ?? false);

                // ZField
                var val = row.Cells[colZField.Index].Value;
                if (val != null) src.ZField = val.ToString();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// ”取消“按钮
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        #endregion
    }
}
