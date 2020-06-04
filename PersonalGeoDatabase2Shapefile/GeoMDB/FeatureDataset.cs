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
    /// 一个要素数据集的空间参考是一致的
    /// </summary>
    public class FeatureDataset
    {
        string _prjectionString;
        public string PrjectionString
        {
            get
            {
                return _prjectionString;
            }
        }

        /// <summary>
        /// 要素数据集名称
        /// </summary>
        public string Name { get; set; }

        FeatureClass[] _featureClasses;
        public FeatureClass[] FeatureClasses
        {
            get
            {
                return _featureClasses;
            }
        }

        public void InitFromTable(DataRow dr,DBMgr pDBgr)
        {
            Name = dr["Name"].ToString();
            string datasetId = dr["ID"].ToString();
            string srid = dr["SRID"].ToString();
            string msg;
            //获取坐标信息
            DataTable pDT = pDBgr.GetTable(ReadPersonalGeoDB.GDB_SpatialRefs, out msg, "SRID");
            DataRow srDr = pDT.AsEnumerable().Where<DataRow>(c => c["SRID"].ToString() == srid).First();
            _prjectionString = srDr["SRTEXT"].ToString();

            //初始化该要素数据集所属的所有要素类
            DataTable pGeomColumnsTable= pDBgr.GetTable(ReadPersonalGeoDB.GDB_GeomColumns, out msg, "");
            pDT = pDBgr.GetTable(ReadPersonalGeoDB.GDB_ObjectClasses, out msg, "ID");
            DataRow[] objClasses = pDT.AsEnumerable().Where<DataRow>(c => c["DatasetID"].ToString() == datasetId).ToArray();

            _featureClasses = new FeatureClass[objClasses.Length];
            int i = 0;
            foreach (DataRow objClass in objClasses)
            {
                _featureClasses[i] = new FeatureClass(pDBgr);
                _featureClasses[i].Name = objClass["Name"].ToString();
                _featureClasses[i].DatasetName = Name;
                _featureClasses[i].ProjectionString = _prjectionString;
                DataRow geomColumnRow = pGeomColumnsTable.AsEnumerable().Where<DataRow>(c => c["TableName"].ToString() == _featureClasses[i].Name).First();
                _featureClasses[i].GeometryType = (EsriGeometryType)Convert.ToInt32(geomColumnRow["ShapeType"]);
                _featureClasses[i].GeomShapeFieldName = geomColumnRow["FieldName"].ToString();
                i++;
            }
        }
    }
}
