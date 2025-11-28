using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;

namespace Lab04_4.MyForms.SpatialQuery.Helpers
{
    class KNearestNeighbor
    {
        public class FeatureDistancePair
        {
            public IFeature feature;
            public double distance;

            public FeatureDistancePair(IFeature feature, double distance)
            {
                this.feature = feature;
                this.distance = distance;
            }
        }

        public static List<FeatureDistancePair> FindKNearest(
            IPoint point, int k, IFeatureClass featureClass,
            int excludeOID = -1)
        {
            if (k <= 0) throw new Exception("k应为正整数");

            // 初始化 要素-距离 列表
            List<FeatureDistancePair> candidates = new List<FeatureDistancePair>();
            // 初始的搜索范围半径
            double range = 2.5;
            int loop = 0;
            // 在搜索结果不足k个时
            while (candidates.Count < k)
            {
                if (loop++ > 20)
                    return null;

                // 扩大搜索范围
                range *= 2;
                // 如果需要排除目标点，根据传入的OID构建where表达式
                string whereClause = "";
                if (excludeOID != -1)
                {
                    whereClause =
                        string.Format("{0} <> {1}",
                        featureClass.OIDFieldName, excludeOID.ToString());
                }
                // 建立缓冲区，进行搜索
                IFeatureCursor cursor =
                    SearchFeatureInRange(point, range, featureClass, whereClause);
                // 计算距离，更新列表
                UpdateCandidates(point, cursor, candidates);
            }

            // 抛弃多余的结果
            candidates.RemoveRange(k, candidates.Count - k);

            return candidates;
        }

        static IFeatureCursor SearchFeatureInRange(
            IPoint point, double distance,
            IFeatureClass fc, string whereClause = "")
        {
            // 创建缓冲区
            ITopologicalOperator topoOperator = point as ITopologicalOperator;
            IGeometry searchArea = topoOperator.Buffer(distance);

            // 设置空间过滤器
            ISpatialFilter filter = new SpatialFilter
            {
                Geometry = searchArea,
                SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects,
                WhereClause = whereClause
            };

            // 对要素类执行查询
            IFeatureCursor cursor = fc.Search(filter, false);
            return cursor;
        }

        static void UpdateCandidates(IPoint source, IFeatureCursor cursor,
            List<FeatureDistancePair> candidates)
        {
            IFeature feature;
            while ((feature = cursor.NextFeature()) != null)
            {
                // 如果点要素已经存在则跳过
                //if (candidates.ConvertAll(c=>c.feature).Contains(feature))
                if (candidates.ConvertAll(c => c.feature.OID).Contains(feature.OID))
                    continue;

                // 计算每个点要素到目标点的距离
                IPoint point = feature.Shape as IPoint;


                double distance = (source as IProximityOperator).ReturnDistance(point);
                candidates.Add(new FeatureDistancePair(feature, distance));
                candidates.Sort((a, b) => a.distance.CompareTo(b.distance));
            }
        }
    }
}
