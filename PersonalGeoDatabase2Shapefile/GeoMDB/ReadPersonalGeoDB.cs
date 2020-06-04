using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZC.IO.DataBase;

namespace SZC.IO.GeoMDB
{
    /// <summary>
    /// 读取ArcGIS的个人文件数据库
    /// </summary>
    public class ReadPersonalGeoDB
    {
        /// <summary>
        /// 记录各个要素类存储几何信息的字段名称、几何类型、空间参考ID
        /// </summary>
        public static readonly string GDB_GeomColumns = "GDB_GeomColumns";
        /// <summary>
        /// 存储要素类名称及所属的要素数据集ID
        /// </summary>
        public static readonly string GDB_ObjectClasses = "GDB_ObjectClasses";
        /// <summary>
        /// 要素数据集名称及空间参考ID
        /// </summary>
        public static readonly string GDB_FeatureDataset = "GDB_FeatureDataset";
        /// <summary>
        /// 空间参考信息
        /// </summary>
        public static readonly string GDB_SpatialRefs = "GDB_SpatialRefs";

        DBMgr mDBMgr = null;

        public bool ReadGeoMDB(string mdbFile)
        {
            mDBMgr = new DBMgr(mdbFile);
            return mDBMgr.Open();
        }

        /// <summary>
        /// 获取所有的要素数据集名称；需要注意的是，可能没有要素数据集，直接存储的是要素集
        /// </summary>
        /// <returns></returns>
        public FeatureDataset[] GetFeatureDatasets()
        {
            string msg;
            DataTable pDT = mDBMgr.GetTable(GDB_FeatureDataset,out msg,"ID");
            FeatureDataset[] pFeaDataSet = new FeatureDataset[pDT.Rows.Count];
            int i = 0;
            foreach (DataRow dr in pDT.Rows)
            {
                pFeaDataSet[i] = new FeatureDataset();
                pFeaDataSet[i].InitFromTable(dr, mDBMgr);
                i++;   
            }
            return pFeaDataSet;
        }

        /// <summary>
        /// 获取指定要素数据集名称下的要素集名称
        /// </summary>
        /// <param name="dataSetName"></param>
        /// <returns></returns>
        public string[] GetFeatureClassNames(string dataSetName)
        {
            string msg;
            DataTable pDT = mDBMgr.GetTable(GDB_FeatureDataset, out msg, "ID");
            //要素数据集的名称不会重复
            DataRow dr = pDT.AsEnumerable().Where<DataRow>(c => c["name"].ToString() == dataSetName).First();
            string datasetID = dr["ID"].ToString();
            pDT = mDBMgr.GetTable(GDB_ObjectClasses, out msg, "ID");
            DataRow[] pFeaClassDRs= pDT.AsEnumerable().Where<DataRow>(c => c["DatasetID"].ToString() == datasetID).ToArray();
            return (from row in pFeaClassDRs select row.Field<string>("Name")).ToArray();
        }

        /// <summary>
        /// 获取所有要素集名称
        /// </summary>
        /// <returns></returns>
        public string[] GetAllFeatureClassNames()
        {
            string msg;
            DataTable pDT = mDBMgr.GetTable(GDB_ObjectClasses, out msg, "ID");
            DataRow[] pFeaClassDRs = pDT.AsEnumerable().ToArray();
            return (from row in pFeaClassDRs select row.Field<string>("Name")).ToArray();
        }

        /// <summary>
        /// 获取所有的要素集
        /// </summary>
        /// <returns></returns>
        public FeatureClass[] GetAllFeatureClasses()
        {
            string msg;
            //DataTable pDT = mDBMgr.GetTable(GDB_ObjectClasses, out msg, "ID");
            DataTable pGeomColumnsTable = mDBMgr.GetTable(ReadPersonalGeoDB.GDB_GeomColumns, out msg, "");
            DataTable srDT = mDBMgr.GetTable(ReadPersonalGeoDB.GDB_SpatialRefs, out msg, "SRID");


            if (pGeomColumnsTable == null)
            {
                return new FeatureClass[0];
            }
            FeatureClass[] _featureClasses = new FeatureClass[pGeomColumnsTable.Rows.Count];
            int i = 0;
            foreach (DataRow dr in pGeomColumnsTable.Rows)
            {

                //获取要素数据集的名称
                _featureClasses[i] = new FeatureClass(mDBMgr);

                _featureClasses[i].Name = dr["TableName"].ToString();            

                _featureClasses[i].GeometryType = (EsriGeometryType)Convert.ToInt32(dr["ShapeType"]);
                _featureClasses[i].GeomShapeFieldName = dr["FieldName"].ToString();
                string srid = dr["SRID"].ToString();
                DataRow srDr = srDT.AsEnumerable().Where<DataRow>(c => c["SRID"].ToString() == srid).First();
                _featureClasses[i].ProjectionString = srDr["SRTEXT"].ToString();
                i++;
            }
            return _featureClasses;
        }

        public void CloseMDB()
        {
            if(mDBMgr!=null)
            {
                mDBMgr.Close();            
            }
        }
    }
}
