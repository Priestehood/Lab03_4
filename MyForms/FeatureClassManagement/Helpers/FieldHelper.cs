using ESRI.ArcGIS.Geodatabase;
using System;

namespace Lab03_4.MyForms.FeatureClassManagement.Helpers
{
    /// <summary>
    /// 字段相关方法 | 工具类
    /// </summary>
    public static class FieldHelper
    {
        /// <summary>
        /// 检查是否为系统字段
        /// </summary>
        public static bool IsSystemField(IField field)
        {
            return field.Type == esriFieldType.esriFieldTypeOID ||
                   field.Type == esriFieldType.esriFieldTypeGeometry;
        }

        /// <summary>
        /// 验证字段名称
        /// </summary>
        public static bool ValidateFieldName(string fieldName, IFeatureClass featureClass = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException("字段名称不能为空");
            }

            // 检查字段名是否已存在
            if (featureClass != null && featureClass.FindField(fieldName.Trim()) >= 0)
            {
                throw new ArgumentException($"字段名称 '{fieldName}' 已存在");
            }

            return true;
        }

        /// <summary>
        /// 验证字段类型和长度
        /// </summary>
        public static void ValidateFieldTypeAndLength(string fieldType, string lengthStr)
        {
            if (string.IsNullOrWhiteSpace(fieldType))
            {
                throw new ArgumentException("请选择字段类型");
            }

            if (fieldType == "文本")
            {
                if (string.IsNullOrWhiteSpace(lengthStr))
                {
                    throw new ArgumentException("文本字段必须指定长度");
                }

                if (!int.TryParse(lengthStr, out int length) || length <= 0)
                {
                    throw new ArgumentException("字段长度必须是正整数");
                }
            }
        }
    }
}