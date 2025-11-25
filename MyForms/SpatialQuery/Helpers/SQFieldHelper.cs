using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.SpatialQuery.Helpers
{
    /// <summary>
    /// 空间查询-字段辅助类
    /// </summary>
    public static class SQFieldHelper
    {
        /// <summary>
        /// 查找字段索引
        /// </summary>
        public static int FindFieldIndex(IFeatureClass featureClass, string[] fieldNames)
        {
            for (int i = 0; i < featureClass.Fields.FieldCount; i++)
            {
                string fieldName = featureClass.Fields.Field[i].Name.ToUpper();
                foreach (string name in fieldNames)
                {
                    if (fieldName == name.ToUpper())
                        return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 安全获取字段值
        /// </summary>
        public static string GetFieldValueSafely(IFeature feature, int fieldIndex)
        {
            try
            {
                object value = feature.get_Value(fieldIndex);
                return value?.ToString() ?? "未知";
            }
            catch
            {
                return "未知";
            }
        }
    }
}
