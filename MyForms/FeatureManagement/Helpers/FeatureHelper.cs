using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESRI.ArcGIS.Geodatabase;
using System.Data;

namespace Lab03_4
{
    public static class FeatureHelper
    {
        public static DataTable ToDataTable(IFeatureClass featureClass)
        {
            try
            {
                DataTable dt = new DataTable();

                IFeatureCursor featureCursor = featureClass.Search(null, false);
                IFeature feature = featureCursor.NextFeature();

                if (feature != null)
                {
                    for (int i = 0; i < feature.Fields.FieldCount; i++)
                    {
                        dt.Columns.Add(feature.Fields.Field[i].Name);
                    }
                    while (feature != null)
                    {
                        DataRow dataRow = dt.NewRow();
                        for (int j = 0; j < featureCursor.Fields.FieldCount; j++)
                        {
                            if (featureCursor.Fields.Field[j].Type == esriFieldType.esriFieldTypeGeometry)
                                dataRow[j] = "Shape";
                            else
                                dataRow[j] = feature.Value[j];
                        }
                        dt.Rows.Add(dataRow);
                        feature = featureCursor.NextFeature();
                    }
                    return dt;
                }
                else
                    throw new Exception("要素类中没有要素");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
