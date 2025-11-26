using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using Lab04_4.MyForms.ElevationManager.Models;
using Lab04_4.MyForms.ElevationManager.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab04_4.MyForms.ElevationManager.Services
{
    /// <summary>
    /// 管理所有高程数据源：添加、查询已启用源、删除等。
    /// </summary>
    public class ElevationManagerD
    {
        public List<ElevationSource> Sources { get; private set; } = new List<ElevationSource>();

        /// <summary>
        /// 将一个 FeatureLayer 添加为高程源（默认启用）。
        /// </summary>
        public void AddSource(IFeatureLayer layer, string zField, bool isFromDat, bool isEnabled = true)
        {
            // 如果已经存在同名图层，更新字段和状态
            var existing = Sources.FirstOrDefault(s => s.Layer == layer);
            if (existing != null)
            {
                existing.ZField = zField;
                existing.IsFromDat = isFromDat;
                existing.Enabled = isEnabled;
                return;
            }

            Sources.Add(new ElevationSource
            {
                Layer = layer,
                ZField = zField,
                IsFromDat = isFromDat,
                Enabled = isEnabled
            });
        }

        /// <summary>
        /// 获取当前启用的高程源
        /// </summary>
        public List<ElevationSource> GetEnabledSources()
        {
            return Sources.Where(s => s.Enabled).ToList();
        }

        /// <summary>
        /// 根据图层查找对应的源对象（可能为 null）
        /// </summary>
        public ElevationSource GetSourceByLayer(IFeatureLayer layer)
        {
            return Sources.FirstOrDefault(s => s.Layer == layer);
        }

        /// <summary>
        /// 移除某个 Source（仅从管理器移除，不删除图层）
        /// </summary>
        public void RemoveSource(IFeatureLayer layer)
        {
            var found = Sources.FirstOrDefault(s => s.Layer == layer);
            if (found != null) Sources.Remove(found);
        }
    }
}
