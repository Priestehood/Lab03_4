using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using Lab04_4.MyForms.FeatureClassManagement.Helpers;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab04_4.MyForms.FeatureClassManagement.Services
{
    /// <summary>
    /// Shapefile文件操作 | 服务类
    /// </summary>
    public class ShapefileService
    {
        /// <summary>
        /// 创建新的Shapefile文件
        /// </summary>
        public IFeatureClass CreateShapefile(string folderPath, string fileName,
            esriGeometryType geometryType, ISpatialReference spatialReference,
            IFields fields)
        {
            try
            {
                ValidateCreateParameters(folderPath, fileName, geometryType, spatialReference, fields);

                string fullPath = System.IO.Path.Combine(folderPath, fileName + ".shp");

                // 检查文件是否已存在
                if (File.Exists(fullPath))
                {
                    throw new InvalidOperationException($"文件已存在: {fullPath}");
                }

                // 创建工作空间
                IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(folderPath, 0);

                // 创建要素类
                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(
                    fileName + ".shp",
                    fields,
                    null,
                    null,
                    esriFeatureType.esriFTSimple,
                    "Shape",
                    "");

                return featureClass;
            }
            catch (Exception ex)
            {
                throw new Exception($"创建Shapefile失败: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 验证创建参数
        /// </summary>
        private void ValidateCreateParameters(string folderPath, string fileName,
            esriGeometryType geometryType, ISpatialReference spatialReference,
            IFields fields)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("存储目录不能为空");

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("文件名称不能为空");

            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException($"目录不存在: {folderPath}");

            if (geometryType == esriGeometryType.esriGeometryNull)
                throw new ArgumentException("几何类型不能为空");

            if (spatialReference == null)
                throw new ArgumentException("空间参考不能为空");

            if (fields == null || fields.FieldCount == 0)
                throw new ArgumentException("字段集合不能为空");
        }

        /// <summary>
        /// 检查文件是否已存在
        /// </summary>
        public bool ShapefileExists(string folderPath, string fileName)
        {
            string fullPath = System.IO.Path.Combine(folderPath, fileName + ".shp");
            return File.Exists(fullPath);
        }

        /// <summary>
        /// 删除Shapefile文件
        /// </summary>
        public void DeleteShapefile(string folderPath, string fileName)
        {
            try
            {
                string basePath = System.IO.Path.Combine(folderPath, fileName);
                string[] extensions = { ".shp", ".shx", ".dbf", ".prj", ".sbn", ".sbx", ".fbn", ".fbx", ".ain", ".aih", ".ixs", ".mxs", ".atx", ".shp.xml" };

                foreach (string extension in extensions)
                {
                    string filePath = basePath + extension;
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"删除Shapefile文件失败: {ex.Message}", ex);
            }
        }
    }
}
