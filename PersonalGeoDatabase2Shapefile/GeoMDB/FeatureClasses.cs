using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SZC.IO.DataBase;

namespace SZC.IO.GeoMDB
{
    public enum EsriGeometryType
    {
        Point=1,
        MultiPoint=2,
        Line=3,
        Polygon=4
    }

    /// <summary>
    /// 一个FeatureClass对应一张表
    /// </summary>
    public class FeatureClass
    {
        /// <summary>
        /// 几何类型：1(点)、2(多点)、3(线)、4(面)
        /// 而实际SHAPE字段中5代表面
        /// </summary>
        public EsriGeometryType GeometryType;

        /// <summary>
        /// 存储几何的字段名称
        /// </summary>
        public string GeomShapeFieldName;

        public string Name;

        /// <summary>
        /// 所属要素数据集的名称
        /// </summary>
        public string DatasetName;

        public string ProjectionString;

        private DBMgr mDBMgr;
        public FeatureClass(DBMgr pDBMgr)
        {
            mDBMgr = pDBMgr;
        }

        public DataTable GetDataTable()
        {
            string msg;
            return mDBMgr.GetTable(Name, out msg, "OBJECTID");
        }

        #region 预留属性
        public bool HasZ;

        public bool HasM;

        #endregion
    }
}
