using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Linq;
using System.Collections.Generic;
using Lab04_4.MyForms.SpatialQuery.Helpers;
using Lab04_4.MyForms.SpatialQuery.Services;
using System.Windows.Forms;   // MessageBox


namespace Lab04_4.MyForms.SpatialQuery.Services
{
    class ElevationAnalysis
    {
        private IFeatureLayer elevPointLayer;
        private string ZFieldName = "Z";

        public ElevationAnalysis(IFeatureLayer layer)
        {
            this.elevPointLayer = layer;
        }

        public int DetectAbnormalElevations(int n)
        {
            IFeatureCursor cursor = elevPointLayer.FeatureClass.Update(null, true);

            int deleteCount = 0;
            IFeature elevationPoint;
            while ((elevationPoint = cursor.NextFeature()) != null)
            {
                List<IFeature> NN = GetKNN(elevationPoint, n);
                if (IsOutlier(elevationPoint, NN))
                {
                    elevationPoint.Delete();
                    deleteCount++;
                }
            }

            return deleteCount;
        }

        /// <summary>
        /// 搜索目标高程点最近的n个高程点，组成N近邻
        /// </summary>
        /// <param name="target">目标高程点</param>
        /// <param name="n">搜索最近高程点的数量n</param>
        /// <returns></returns>
        private List<IFeature> GetKNN(IFeature target, int n)
        {
            if (!(target.Shape is IPoint)) throw new Exception("指定要素非点要素");

            List<KNearestNeighbor.FeatureDistancePair> results =
                KNearestNeighbor.FindKNearest(target.Shape as IPoint, n, elevPointLayer.FeatureClass,
                target.OID);
            if (results is null) return null;

            List<IFeature> features = new List<IFeature>();
            return results.ConvertAll((pair) => pair.feature);
        }

        /// <summary>
        /// 判断目标高程点是否为离群点
        /// </summary>
        /// <param name="target">目标高程点</param>
        /// <param name="NN">目标高程点的N近邻</param>
        /// <returns></returns>
        private bool IsOutlier(IFeature target, List<IFeature> NN)
        {
            NN.Add(target);
            // 获取高程值
            List<double> elevations = GetElevations(NN);

            // 计算平均值、标准差
            double avg = elevations.Average();
            double sumOfSquares = elevations
                .Select(e => Math.Pow(e - avg, 2))
                .Sum();
            double variance = sumOfSquares / (NN.Count - 1);
            double std = Math.Sqrt(variance);

            // 计算容差范围
            double avgPlus3Std = avg + 3 * std;
            double avgMinus3Std = avg - 3 * std;
            double targetElevation = elevations.Last();

            return targetElevation < avgMinus3Std ||
                targetElevation > avgPlus3Std;
        }

        /// <summary>
        /// 将高程点的要素列表转换为高程值列表
        /// </summary>
        /// <param name="features">高程点的要素列表</param>
        /// <returns>高程值列表</returns>
        private List<double> GetElevations(List<IFeature> features)
        {
            int ZFieldIndex = features[0].Fields.FindField(ZFieldName);
            List<double> elevations = new List<double>();
            foreach (IFeature feature in features)
            {
                double elevation = (double)feature.Value[ZFieldIndex];
                elevations.Add(elevation);
            }
            return elevations;
        }

        public double IntepolateElevation(IPoint click, int n)
        {
            //List<KNearestNeighbor.FeatureDistancePair> results =
            //KNearestNeighbor.FindKNearest(click, n, elevPointLayer.FeatureClass);

            // TODO: 反距离内插法
            if (elevPointLayer == null)
                throw new Exception("未找到高程图层!");

            List<KNearestNeighbor.FeatureDistancePair> results =
                KNearestNeighbor.FindKNearest(click, n, elevPointLayer.FeatureClass);

            if (results == null || results.Count == 0)
                throw new Exception("未找到任何邻近的高程点，无法计算插值。");

            int zIndex = elevPointLayer.FeatureClass.Fields.FindField(ZFieldName);
            if (zIndex < 0)
                throw new Exception("未找到高程字段：" + ZFieldName);

            double weightedSum = 0;
            double weightTotal = 0;
            double p = 2; // IDW指数

            foreach (var item in results)
            {
                double dist = item.distance;
                double elevation = Convert.ToDouble(item.feature.Value[zIndex]);

                if (dist == 0) return elevation; // 避免除0

                double weight = 1 / Math.Pow(dist, p);
                weightedSum += weight * elevation;
                weightTotal += weight;
            }

            return weightedSum / weightTotal;
        }

    }

    
}
