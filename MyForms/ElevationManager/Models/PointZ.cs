using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.ElevationManager.Models
{
    /// <summary>
    /// 保存文本文件中解析出来的 XYZ 点
    /// </summary>
    public class PointZ
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public PointZ() { }
        public PointZ(double x, double y, double z) { X = x; Y = y; Z = z; }
    }
}
