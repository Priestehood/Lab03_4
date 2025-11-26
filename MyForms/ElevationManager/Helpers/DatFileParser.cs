using Lab04_4.MyForms.ElevationManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.ElevationManager.Helpers
{
    /// <summary>
    /// 负责将 DAT / TXT 文件解析为 List<PointZ>
    /// 支持以空格、制表符或逗号分隔的常见格式。
    /// </summary>
    public static class DatFileParser
    {
        public static List<PointZ> Parse(string filePath)
        {
            var list = new List<PointZ>();
            if (!File.Exists(filePath)) return list;

            var lines = File.ReadAllLines(filePath);
            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                var parts = line.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 3) continue;

                if (double.TryParse(parts[0], out double x) &&
                    double.TryParse(parts[1], out double y) &&
                    double.TryParse(parts[2], out double z))
                {
                    list.Add(new PointZ(x, y, z));
                }
                else
                {
                    // 忽略解析失败的行，或按需抛出/记录
                }
            }
            return list;
        }
    }
}
