using ESRI.ArcGIS.Carto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.ElevationManager.Models
{
    /// <summary>
    /// 保存一个高程数据源的信息（图层 / 字段 / 是否启用）
    /// </summary>
    public class ElevationSource
    {
        public IFeatureLayer Layer { get; set; }     // 图层
        public string ZField { get; set; }           // 高程字段
        public bool Enabled { get; set; }            // 是否作为高程源
        public bool IsFromDat { get; set; }          // 是否从 DAT 导入
    }
}
