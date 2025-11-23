using ESRI.ArcGIS.Geodatabase;
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
    public partial class FieldValueControl : UserControl
    {
        private IFields fields;
        private IFeature feature;
        public IFields Fields
        {
            get => fields;
            set
            {
                this.fields = value;
                UpdateRows();
            }
        }
        public IFeature Feature
        {
            set
            {
                this.feature = value;
                this.fields = feature.Fields;
                UpdateRows();
            }
        }

        public FieldValueControl()
        {
            fields = null;
            feature = null;
            InitializeComponent();
        }

        private void UpdateRows()
        {
            if (fields is null) return;
            try
            {
                int j = 0;
                for (int i = 0; i < fields.FieldCount; i++)
                {
                    if (fields.Field[i].Type == esriFieldType.esriFieldTypeOID
                        || fields.Field[i].Type == esriFieldType.esriFieldTypeGeometry)
                        continue;

                    //在表格中显示字段别名
                    if (feature is null)
                    {
                        dgvFields.Rows.Add(fields.Field[i].AliasName, null);
                    }
                    else
                    {
                        dgvFields.Rows.Add(fields.Field[i].AliasName, feature.Value[i].ToString());
                    }
                    //在行的Tag属性中记录字段编号
                    dgvFields.Rows[j].Tag = i;
                    j++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("[异常] " + ex.Message,
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public void SetFeatureFields(IFeature feature)
        {
            IFields fields = feature.Fields;
            foreach (DataGridViewRow row in dgvFields.Rows)
            {
                if (row.Tag is null) continue;

                int fieldIndex = (int)row.Tag;
                esriFieldType fieldType = fields.Field[fieldIndex].Type;

                object cellValue = row.Cells[1].Value;
                if (cellValue is null) continue;

                switch (fieldType)
                {
                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeSmallInteger:
                        try
                        {
                            feature.Value[fieldIndex] =
                                int.Parse(cellValue as string);
                        }
                        catch (Exception)
                        {
                            throw new Exception(string.Format("输入的属性值“{0}”不是合法整数！", cellValue));
                        }
                        continue;
                    case esriFieldType.esriFieldTypeDouble:
                    case esriFieldType.esriFieldTypeSingle:
                        try
                        {
                            feature.Value[fieldIndex] =
                                double.Parse(cellValue as string);
                        }
                        catch (Exception)
                        {
                            throw new Exception(string.Format("输入的属性值“{0}”不是合法数字！", cellValue));
                        }
                        continue;
                    case esriFieldType.esriFieldTypeDate:
                        feature.Value[fieldIndex] = cellValue;
                        continue;
                    case esriFieldType.esriFieldTypeString:
                        feature.Value[fieldIndex] = cellValue;
                        continue;
                    default:
                        continue;
                }

            }
        }
    }
}
