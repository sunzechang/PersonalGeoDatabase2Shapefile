using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace SZC.IO.DataBase
{
    public class DBMgr
    {
        private OleDbConnection mConn = null;
        private string _mdbFile;
        public string DBFile
        {
            get { return _mdbFile; }
        }

        public DBMgr(string mdbFile)
        {
            _mdbFile = mdbFile;
        }

        public bool Open()
        {
            if (!File.Exists(_mdbFile)) return false;

            try
            {
                //可以访问Access数据库也可以访问Excel；使用该方式需要安装Office2007以上版本
                mConn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + _mdbFile);
                mConn.Open();
                if (mConn.State != ConnectionState.Open) return false;
            }
            catch (Exception)
            {
                try
                {
                    mConn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + _mdbFile);
                    mConn.Open();
                    if (mConn.State != ConnectionState.Open) return false;
                }
                catch(Exception)
                {
                    //MessageCenterMgr.MessageCenter.AddErrorMsg("打开数据库失败，您需要安装该插件，关闭该窗体后会弹出安装文件位置。");
                    MessageBox.Show("打开数据库失败，您需要安装该插件，关闭该窗体后会弹出安装文件位置。");
                    System.Diagnostics.Process.Start(Path.Combine(Application.StartupPath, "需要安装的插件"));
                    return false;
                }
            }
            return true;    
        }

        /// <summary>
        /// 获取数据库文件中所有表名称
        /// </summary>
        /// <returns></returns>
        public string[] GetTables()
        {
            if (null == mConn) return null; 

            DataTable dt = mConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            string[] tableNames = new string[dt.Rows.Count];
            for (int i=0;i< dt.Rows.Count;i++)
            {
                tableNames[i] = dt.Rows[i]["TABLE_NAME"].ToString();
            }
            return tableNames;
        }

        public DataTable GetTable(string tableName,out string errorMsg,string orderByColName="ID")
        {
            errorMsg = string.Empty;
            if(mConn==null)return null;
            try
            {
                OleDbDataAdapter _Adapter = null;
                if (!string.IsNullOrWhiteSpace(orderByColName))
                {
                    _Adapter = new OleDbDataAdapter("select * from " + tableName + " order by " + orderByColName, mConn);
                }
                else
                {
                    _Adapter = new OleDbDataAdapter("select * from " + tableName, mConn);
                }
                DataSet _ds = new DataSet();
                _Adapter.Fill(_ds, tableName);
                if (_ds.Tables.Contains(tableName))
                    return _ds.Tables[tableName];
            }
            catch(Exception ex) 
            {
                errorMsg = ex.Message;
                return null;
            }
            return null;
        }

        public bool Update(DataTable pDT,string tblName)
        {
            //清空表中已有记录,但不会删除表
            string sql = "delete from " + tblName;
            ExecuteNonQuery(sql);
            //if (!delSeccess) return delSeccess;

            OleDbDataAdapter _Adapter = new OleDbDataAdapter("select * from " + tblName + " order by ID", mConn);
            OleDbCommandBuilder _cb = new OleDbCommandBuilder(_Adapter);
            _cb.QuotePrefix = "[";
            _cb.QuoteSuffix = "]";
            DataSet _ds = new DataSet();
            _Adapter.Fill(_ds, tblName);

            foreach(DataRow dr in pDT.Rows)
            {
                DataRow newDr = _ds.Tables[tblName].NewRow();
                newDr.ItemArray = dr.ItemArray;//复制行
                _ds.Tables[tblName].Rows.Add(newDr);
            }
            return _Adapter.Update(_ds, tblName) == pDT.Rows.Count ? true : false;
        }

        public void ExecuteNonQuery(string sqlStr)
        {
            if (mConn == null)return;

            OleDbCommand pCmd = mConn.CreateCommand();
            pCmd.CommandText = sqlStr;
            pCmd.ExecuteNonQuery();//如果表格本身没有数据，返回也是0. 所以不能根据返回0来判断返回true还是false。
        }

        public DataTable Select(string sqlStr,string tableName)
        {
            if (mConn == null) return null;
            try
            {
                OleDbDataAdapter _Adapter = null;
                _Adapter = new OleDbDataAdapter(sqlStr, mConn);

                DataSet _ds = new DataSet();
                _Adapter.Fill(_ds, tableName);
                if (_ds.Tables.Contains(tableName))
                    return _ds.Tables[tableName];
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        public void Close()
        {
            if(mConn==null)return;
            mConn.Close(); 
             mConn = null;
        }
    }
}
