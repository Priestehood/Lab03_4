using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab03_4.MyForms.FeatureClassManagement.Helpers
{
    /// <summary>
    /// 字段类型转换 | 工具类
    /// </summary>
    public static class FieldTypeHelper
    {
        /// <summary>
        /// 将中文字段类型转换为esriFieldType
        /// </summary>
        public static esriFieldType ConvertToFieldType(string fieldType)
        {
            switch (fieldType?.ToLower())
            {
                case "整数":
                    return esriFieldType.esriFieldTypeInteger;
                case "数字":
                    return esriFieldType.esriFieldTypeDouble;
                case "日期":
                    return esriFieldType.esriFieldTypeDate;
                case "文本":
                    return esriFieldType.esriFieldTypeString;
                default:
                    throw new ArgumentException($"不支持的字段类型: {fieldType}");
            }
        }

        /// <summary>
        /// 将esriFieldType转换为中文字段类型
        /// </summary>
        public static string ConvertToChineseName(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeInteger:
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "整数";
                case esriFieldType.esriFieldTypeDouble:
                case esriFieldType.esriFieldTypeSingle:
                    return "数字";
                case esriFieldType.esriFieldTypeDate:
                    return "日期";
                case esriFieldType.esriFieldTypeString:
                    return "文本";
                default:
                    return "未知";
            }
        }

        /// <summary>
        /// 检查字段类型是否需要长度参数
        /// </summary>
        public static bool RequiresLength(esriFieldType fieldType)
        {
            return fieldType == esriFieldType.esriFieldTypeString;
        }

        /// <summary>
        /// 获取字段类型的默认长度
        /// </summary>
        public static int GetDefaultLength(esriFieldType fieldType)
        {
            return fieldType == esriFieldType.esriFieldTypeString ? 50 : 0;
        }
    }
}
