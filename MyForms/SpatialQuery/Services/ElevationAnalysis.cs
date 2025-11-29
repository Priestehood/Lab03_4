using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Lab04_4.MyForms.SpatialQuery.Helpers;
using System.Windows.Forms;

namespace Lab04_4.MyForms.SpatialQuery.Services
{
    public delegate void UpdateStatusDelegate(string message);

    class ElevationAnalysis
    {
        private IFeatureLayer elevPointLayer;
        private string ZFieldName = "Z";

        private IFeatureCursor updateCursor;
        private int n; // 搜索最近高程点的数量n
        private int deleteCount;

        private UpdateStatusDelegate UpdateStatus;

        public ElevationAnalysis(IFeatureLayer layer,
            UpdateStatusDelegate updateStatus)
        {
            this.elevPointLayer = layer;
            this.UpdateStatus = updateStatus;
        }

        public int DetectAbnormalElevations(int kOfKNN)
        {
            // 初始化
            this.n = kOfKNN;
            deleteCount = 0;
            IFeatureClass featureClass = elevPointLayer.FeatureClass;
            updateCursor = featureClass.Search(null, false);

            LongOperation operation =
                new LongOperation(featureClass.FeatureCount(null),
                doStep, onCompleted);
            operation.Start("正在进行高程点滤波");

            return deleteCount;
        }

        private bool doStep()
        {
            IFeature elevationPoint = updateCursor.NextFeature();
            if (elevationPoint is null) return true;

            //if (elevationPoint.OID < 1720) return false;

            List<IFeature> NN = GetKNN(elevationPoint, n);
            if (IsOutlier(elevationPoint, NN))
            {
                elevationPoint.Delete();
                deleteCount++;
            }

            return false;
        }

        private void onCompleted(RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                UpdateStatus("高程点滤波操作已取消。");
            else if (e.Error != null)
                MessageBox.Show($"错误: {e.Error.Message}");
            else
                UpdateStatus($"已完成高程点滤波，删除了{deleteCount}个高程点。");
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
            List<KNearestNeighbor.FeatureDistancePair> results =
                KNearestNeighbor.FindKNearest(click, n, elevPointLayer.FeatureClass);

            // TODO: 反距离内插法
            double elevation = 0;
            return elevation;
        }
    }
}
