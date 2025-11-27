using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.SpatialQuery.Helpers
{
    /// <summary>
    /// 面积极值查询结果
    /// </summary>
    public struct AreaExtremeResult
    {
        public IFeature MaxAreaFeature { get; set; }
        public IFeature MinAreaFeature { get; set; }
        public double MaxArea { get; set; }
        public double MinArea { get; set; }
        public int NameFieldIndex { get; set; }
        public int IDFieldIndex { get; set; }
        public int ProcessedCount { get; set; }
        public int ValidGeometryCount { get; set; }
        public bool IsGeographicCoordinateSystem { get; set; }
    }

    /// <summary>
    /// 字段索引查询结果
    /// </summary>
    public class FieldIndicesResult
    {
        public int NameIndex { get; set; }
        public int IDIndex { get; set; }
        public bool IsValid { get; set; }

        public FieldIndicesResult()
        {
            NameIndex = -1;
            IDIndex = -1;
            IsValid = false;
        }

        public FieldIndicesResult(int nameIndex, int idIndex, bool isValid)
        {
            NameIndex = nameIndex;
            IDIndex = idIndex;
            IsValid = isValid;
        }
    }
}
