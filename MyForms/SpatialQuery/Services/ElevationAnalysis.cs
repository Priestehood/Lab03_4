using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Linq;
using System.Collections.Generic;
using Lab04_4.MyForms.SpatialQuery.Helpers;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;

namespace Lab04_4.MyForms.SpatialQuery.Services
{
    class ElevationAnalysis
    {
        public IFeatureLayer elevPointLayer { get; private set; }
        private string ZFieldName = "Z";

        // 构造函数不绑定图层
        public ElevationAnalysis()
        {
            elevPointLayer = null;
        }

        /// <summary>
        /// 延迟绑定高程点图层，地图里第一个点要素图层会被识别
        /// </summary>
        public bool AssignLayersAutomatically(AxMapControl map)
        {
            elevPointLayer = null; // 先清空

            for (int i = 0; i < map.LayerCount; i++)
            {
                ILayer layer = map.get_Layer(i);
                IFeatureLayer fl = layer as IFeatureLayer;
                if (fl == null) continue;

                // 找到点图层
                if (fl.FeatureClass != null && fl.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
                {
                    int zIndex = fl.FeatureClass.Fields.FindField(ZFieldName);
                    if (zIndex >= 0)
                    {
                        elevPointLayer = fl;
                        ZFieldName = fl.FeatureClass.Fields.get_Field(zIndex).Name;
                        return true; // 找到高程点图层
                    }
                }
            }

            // 没找到图层，返回 false，不弹窗
            return false;
        }

        /// <summary>
        /// 高程点异常值检测（滑动窗口法）
        /// </summary>
        public int DetectAbnormalElevations(int n)
        {
            if (elevPointLayer == null) return 0;

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

        private List<IFeature> GetKNN(IFeature target, int n)
        {
            if (!(target.Shape is IPoint)) throw new Exception("指定要素非点要素");

            List<KNearestNeighbor.FeatureDistancePair> results =
                KNearestNeighbor.FindKNearest(target.Shape as IPoint, n, elevPointLayer.FeatureClass,
                target.OID);
            if (results is null) return new List<IFeature>();

            return results.ConvertAll(pair => pair.feature);
        }

        private bool IsOutlier(IFeature target, List<IFeature> NN)
        {
            NN.Add(target);
            List<double> elevations = GetElevations(NN);

            double avg = elevations.Average();
            double sumOfSquares = elevations.Select(e => Math.Pow(e - avg, 2)).Sum();
            double variance = sumOfSquares / (NN.Count - 1);
            double std = Math.Sqrt(variance);

            double avgPlus3Std = avg + 3 * std;
            double avgMinus3Std = avg - 3 * std;
            double targetElevation = elevations.Last();

            return targetElevation < avgMinus3Std || targetElevation > avgPlus3Std;
        }

        private List<double> GetElevations(List<IFeature> features)
        {
            int ZFieldIndex = features[0].Fields.FindField(ZFieldName);
            List<double> elevations = new List<double>();
            foreach (IFeature feature in features)
            {
                elevations.Add(Convert.ToDouble(feature.Value[ZFieldIndex]));
            }
            return elevations;
        }

        /// <summary>
        /// 反距离权重插值法
        /// </summary>
        public double IntepolateElevation(IPoint click, int n)
        {
            if (elevPointLayer == null)
                throw new Exception("⚠ 高程图层未绑定，无法计算插值！");

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

                if (dist == 0) return elevation;

                double weight = 1 / Math.Pow(dist, p);
                weightedSum += weight * elevation;
                weightTotal += weight;
            }

            return weightedSum / weightTotal;
        }
    }
}
