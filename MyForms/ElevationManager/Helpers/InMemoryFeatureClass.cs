using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab04_4.MyForms.ElevationManager.Models;

namespace Lab04_4.MyForms.ElevationManager.Helpers
{
    /// <summary>
    /// 辅助方法：创建 InMemory 的点要素类，用于将 DAT 导入并显示在地图上（带 Z 字段）。
    /// </summary>
    public static class InMemoryFeatureClass
    {
        /// <summary>
        /// 创建一个内存点要素类，包含 Shape (Point) 与 ZValue 字段。
        /// 返回 IFeatureClass。
        /// </summary>
        public static IFeatureClass CreatePointFeatureClass(string fcName, ISpatialReference sref = null)
        {
            IFeatureWorkspace fws = CreateInMemoryWorkspace("MemWS_" + Guid.NewGuid().ToString("N"));

            IFields fields = new FieldsClass();
            IFieldsEdit fe = (IFieldsEdit)fields;

            // Shape 字段
            IField shapeField = new FieldClass();
            IFieldEdit shapeEdit = (IFieldEdit)shapeField;
            shapeEdit.Name_2 = "Shape";
            shapeEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;

            IGeometryDef geomDef = new GeometryDefClass();
            IGeometryDefEdit geomEdit = (IGeometryDefEdit)geomDef;
            geomEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;

            if (sref == null)
            {
                ISpatialReferenceFactory srFact = new SpatialReferenceEnvironmentClass();
                sref = srFact.CreateProjectedCoordinateSystem(3857); // WGS 84 / Web Mercator
            }
            geomEdit.SpatialReference_2 = sref;
            shapeEdit.GeometryDef_2 = geomDef;
            fe.AddField(shapeField);

            // Z 字段
            IField zField = new FieldClass();
            IFieldEdit zEdit = (IFieldEdit)zField;
            zEdit.Name_2 = "ZValue";
            zEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
            fe.AddField(zField);

            // 创建 FC
            return fws.CreateFeatureClass(
                fcName,
                fields,
                null,
                null,
                esriFeatureType.esriFTSimple,
                "Shape",
                ""
            );
        }

        /// <summary>
        /// 创建 InMemory 工作空间
        /// </summary>
        private static IFeatureWorkspace CreateInMemoryWorkspace(string name = "MemWS")
        {
            IWorkspaceFactory wf = new InMemoryWorkspaceFactoryClass();
            IWorkspaceName wsName = wf.Create("", name, null, 0) as IWorkspaceName;

            IName wsName2 = (IName)wsName;  // 你缺失的就是这一步
            IWorkspace ws = (IWorkspace)wsName2.Open();

            return (IFeatureWorkspace)ws;
        }

        /// <summary>
        /// 将一个 PointZ 转为 IPoint（可以设置 spatial reference）
        /// </summary>
        public static IPoint CreatePoint(double x, double y, ISpatialReference sref = null)
        {
            IPoint p = new PointClass();
            p.PutCoords(x, y);
            if (sref != null)
            {
                p.SpatialReference = sref;
            }
            return p;
        }

        /// <summary>
        /// 将点集合插入到目标要素类，假设要素类包含 ZValue 字段与 Shape 字段。
        /// </summary>
        public static void InsertPointsToFeatureClass(IFeatureClass fc, IEnumerable<PointZ> points, ISpatialReference sref = null)
        {
            if (fc == null) throw new ArgumentNullException(nameof(fc));
            int zFieldIndex = fc.FindField("ZValue");
            if (zFieldIndex < 0) throw new Exception("FeatureClass 缺少 ZValue 字段");

            IFeatureBuffer buffer = fc.CreateFeatureBuffer();
            IFeatureCursor insertCursor = fc.Insert(true);
            foreach (var pt in points)
            {
                IPoint ip = CreatePoint(pt.X, pt.Y, sref);
                buffer.Shape = (IGeometry)ip;
                buffer.set_Value(zFieldIndex, pt.Z);
                insertCursor.InsertFeature(buffer);
            }
            insertCursor.Flush();
        }

    }
}
