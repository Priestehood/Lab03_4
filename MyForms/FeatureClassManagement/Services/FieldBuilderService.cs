using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Lab03_4.MyForms.FeatureClassManagement.Helpers;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab03_4.MyForms.FeatureClassManagement.Services
{
    /// <summary>
    /// 构建字段集合 | 服务类
    /// </summary>
    public class FieldBuilderService
    {
        /// <summary>
        /// 创建要素类字段集合
        /// </summary>
        public IFields CreateFeatureClassFields(esriGeometryType geometryType,
            ISpatialReference spatialReference, DataGridViewRowCollection userFields)
        {
            try
            {
                IFieldsEdit fieldsEdit = new FieldsClass() as IFieldsEdit;

                // 添加必需的OID字段
                AddOidField(fieldsEdit);

                // 添加几何字段
                AddGeometryField(fieldsEdit, geometryType, spatialReference);

                // 添加用户自定义字段
                AddUserFields(fieldsEdit, userFields);

                return fieldsEdit as IFields;
            }
            catch (Exception ex)
            {
                throw new Exception($"构建字段集合失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 添加OID字段
        /// </summary>
        private void AddOidField(IFieldsEdit fieldsEdit)
        {
            IFieldEdit oidField = new FieldClass() as IFieldEdit;
            oidField.Name_2 = "FID";
            oidField.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(oidField);
        }

        /// <summary>
        /// 添加几何字段
        /// </summary>
        private void AddGeometryField(IFieldsEdit fieldsEdit, esriGeometryType geometryType,
            ISpatialReference spatialReference)
        {
            IFieldEdit shapeField = new FieldClass() as IFieldEdit;
            shapeField.Name_2 = "Shape";
            shapeField.Type_2 = esriFieldType.esriFieldTypeGeometry;

            IGeometryDefEdit geometryDef = new GeometryDefClass() as IGeometryDefEdit;
            geometryDef.GeometryType_2 = geometryType;
            geometryDef.SpatialReference_2 = spatialReference;

            shapeField.GeometryDef_2 = geometryDef;
            fieldsEdit.AddField(shapeField);
        }

        /// <summary>
        /// 添加用户自定义字段
        /// </summary>
        private void AddUserFields(IFieldsEdit fieldsEdit, DataGridViewRowCollection userFields)
        {
            foreach (DataGridViewRow row in userFields)
            {
                if (row.IsNewRow) continue;

                IFieldEdit field = CreateUserField(row);
                fieldsEdit.AddField(field);
            }
        }

        /// <summary>
        /// 创建用户字段
        /// </summary>
        private IFieldEdit CreateUserField(DataGridViewRow row)
        {
            IFieldEdit field = new FieldClass() as IFieldEdit;

            // 设置字段名称
            string fieldName = row.Cells["colFieldName"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException("字段名称不能为空");

            field.Name_2 = fieldName.Trim() ;

            // 设置字段别名
            string fieldAlias = row.Cells["colFieldAlias"].Value?.ToString();
            field.AliasName_2 = string.IsNullOrWhiteSpace(fieldAlias) ? fieldName.Trim() : fieldAlias.Trim();

            // 设置字段类型
            string fieldTypeStr = row.Cells["colFieldType"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(fieldTypeStr))
                throw new ArgumentException($"字段 '{fieldName.Trim()}' 的类型不能为空");

            esriFieldType fieldType = FieldTypeHelper.ConvertToFieldType(fieldTypeStr);
            field.Type_2 = fieldType;

            // 设置字段长度（仅文本类型?）
            if (FieldTypeHelper.RequiresLength(fieldType))
            {
                string lengthStr = row.Cells["colFieldLength"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(lengthStr))
                {
                    field.Length_2 = FieldTypeHelper.GetDefaultLength(fieldType);
                }
                else
                {
                    if (int.TryParse(lengthStr, out int length) && length > 0)
                    {
                        field.Length_2 = length;
                    }
                    else
                    {
                        field.Length_2 = FieldTypeHelper.GetDefaultLength(fieldType);
                    }
                }
            }

            return field;
        }
    }
}
