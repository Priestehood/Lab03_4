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
    public delegate void SelectFeaturesDelegate(object features, IFeatureLayer featureLayer);
    public delegate void DeleteFeaturesDelegate(object features, IFeatureLayer featureLayer);

    class ElevationAnalysis
    {
        private IFeatureLayer elevPointLayer;
        private IFeatureClass featureClass
        {
            get => elevPointLayer.FeatureClass;
        }
        private string ZFieldName = "Z";

        private IFeatureCursor updateCursor;
        private int n; // 搜索最近高程点的数量n
        private List<int> deletedOIDs;

        private UpdateStatusDelegate UpdateStatus;
        private SelectFeaturesDelegate SelectFeatures;
        private DeleteFeaturesDelegate DeleteFeatures;

        public ElevationAnalysis(UpdateStatusDelegate updateStatus)
        {
            this.UpdateStatus = updateStatus;
            this.deletedOIDs = new List<int>();
        }

        public void SetLayer(IFeatureLayer elevPointLayer)
        {
            this.elevPointLayer = elevPointLayer;
        }

        public void DetectAbnormalElevations(int kOfKNN,
            SelectFeaturesDelegate selectFeatures,
            DeleteFeaturesDelegate deleteFeatures)
        {
            if (elevPointLayer == null)
                throw new Exception("⚠ 高程图层未绑定，无法计算插值！");

            // 初始化
            this.n = kOfKNN;
            this.SelectFeatures = selectFeatures;
            this.DeleteFeatures = deleteFeatures;
            updateCursor = featureClass.Update(null, false);

            LongOperation operation =
                new LongOperation(featureClass.FeatureCount(null),
                doStep, onCompleted);
            operation.Start("正在进行高程点滤波");

            return;
        }

        private bool doStep()
        {
            IFeature elevationPoint = updateCursor.NextFeature();
            if (elevationPoint is null) return true;

            //if (elevationPoint.OID < 1720) return false;

            List<IFeature> NN = GetKNN(elevationPoint, n);
            if (IsOutlier(elevationPoint, NN))
            {
                //elevationPoint.Delete();
                deletedOIDs.Add(elevationPoint.OID);
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
            {
                if (deletedOIDs.Count <= 0)
                {
                    UpdateStatus($"已完成高程点滤波，未发现异常高程点。");
                    return;
                }

                // 选中所有要素
                SelectFeatures(deletedOIDs, elevPointLayer);

                // 等待用户确认删除                
                DialogResult result = MessageBox.Show("是否删除所选的异常高程点？",
                    "高程点滤波", MessageBoxButtons.YesNoCancel);
                if (result != DialogResult.Yes) return;

                // 删除要素
                DeleteFeatures(deletedOIDs, elevPointLayer);
                UpdateStatus($"已完成高程点滤波，删除了{deletedOIDs.Count()}个高程点。");
            }
        }

        /// <summary>
        /// 搜索目标高程点最近的n个高程点，组成N近邻
        /// </summary>
        /// <param name="target">目标高程点</param>
        /// <param name="n">搜索最近高程点的数量n</param>
        /// <returns>最近的n个高程点的要素列表</returns>
        private List<IFeature> GetKNN(IFeature target, int n)
        {
            if (!(target.Shape is IPoint)) throw new Exception("指定要素非点要素");

            List<KNearestNeighbor.FeatureDistancePair> results =
                KNearestNeighbor.FindKNearest(target.Shape as IPoint, n, elevPointLayer.FeatureClass,
                target.OID);
            if (results is null) return new List<IFeature>();
            return results.ConvertAll(pair => pair.feature);
        }

        /// <summary>
        /// 判断目标高程点是否为离群点
        /// </summary>
        /// <param name="target">目标高程点</param>
        /// <param name="NN">目标高程点的N近邻</param>
        /// <returns>目标高程点是离群点时为true，否则为false</returns>
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
            double variance = sumOfSquares / (NN.Count);
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
                elevations.Add(Convert.ToDouble(feature.Value[ZFieldIndex]));
            }
            return elevations;
        }

        /// <summary>
        /// 反距离权重插值得到高程
        /// </summary>
        /// <param name="click">点击位置的点</param>
        /// <param name="n">搜索最近高程点的数量n</param>
        /// <returns></returns>
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
