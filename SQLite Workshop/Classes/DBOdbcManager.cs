using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class DBOdbcManager : DBManager
    {
        OdbcConnection conn;

        internal string DatabaseName { get; set; }
        internal string DatabaseUserName { get; set; }
        internal string DatabasePassword { get; set; }
        internal bool UseWindowsAuthentication { get; set; }
        internal string LastError { get; set; }

        internal DBOdbcManager(string DBName) : base(DBName)
        {
            DatabaseName = DBName;
        }

        ~DBOdbcManager()
        {
            CloseDB();
        }

        protected bool OpenDB()
        {
           
            conn = new OdbcConnection();
            conn.ConnectionString = String.Format("DSN={0};", DatabaseName);
            if (!string.IsNullOrEmpty(DatabaseUserName)) conn.ConnectionString = string.Format("{0}UID={1};Pwd={2};", conn.ConnectionString, DatabaseUserName, DatabasePassword);
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                throw new Exception(ex.Message);
            }
            
            return true;
        }

        protected bool CloseDB()
        {
            if (conn == null || conn.State != ConnectionState.Open) return false;
            conn.Close();
            return true;
        }

        internal bool TestOdbcConnection()
        {
            if (OpenDB())
            {
                CloseDB();
                return true;
            }
            return false;
        }

        internal override DBDatabaseList GetDatabaseList()
        {
            DBDatabaseList DbDl = new DBDatabaseList();
            DbDl.Databases = new Dictionary<string, DBInfo>();
            List<string> dsnList = GetDsnList();

            foreach (string dsn in dsnList)
            {
                DBInfo di = new DBInfo();
                di.Name = dsn;
                DbDl.Databases.Add(di.Name, di);
            }
            return DbDl;
        }

        private List<string> GetDsnList()
        {
            List<string> list = new List<string>();
            list.AddRange(GetDsnList(Registry.CurrentUser));
            list.AddRange(GetDsnList(Registry.LocalMachine));
            return list;
        }

        private IEnumerable<string> GetDsnList(RegistryKey rootKey)
        {
            RegistryKey regKey = rootKey.OpenSubKey(@"Software\ODBC\ODBC.INI\ODBC Data Sources");
            if (regKey != null)
            {
                foreach (string name in regKey.GetValueNames())
                {
                    string value = regKey.GetValue(name, "").ToString();
                    yield return name;
                }
            }
        }

        internal override DBSchema GetSchema()
        {

            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            if (conn == null || conn.State != ConnectionState.Open) OpenDB();

            DataTable dt = conn.GetSchema("Tables");
            
            foreach (DataRow dr in dt.Rows)
            {
                DBTable dbt = new DBTable() { Name = dr["TABLE_NAME"].ToString() };
                Tables.Add(dbt.Name, dbt);
            }

            DBSchema schema = new DBSchema();
            schema.Tables = Tables;
            CloseDB();
            return schema;
        }

        internal override Dictionary<string, DBColumn> GetColumns(string TableName)
        {

            Dictionary<string, DBColumn> DBColumns = new Dictionary<string, DBColumn>();
            OdbcDataReader sqldr;

            if (conn == null || conn.State != ConnectionState.Open) OpenDB();

            OdbcCommand cmd = new OdbcCommand();
            cmd.Connection = conn;
            cmd.CommandText = string.Format("Select * From {0}", TableName);
            try
            {
                sqldr = cmd.ExecuteReader(CommandBehavior.SingleRow);
            }
            catch (Exception ex)
            {
                CloseDB();
                throw new Exception(ex.Message);
            }

            try
            {
                sqldr.Read();
                DataTable ColumnList = sqldr.GetSchemaTable();
                int i = 0;
                foreach (DataRow dr in ColumnList.Rows)
                {
                    DBColumn dbc = new DBColumn() { Name = dr["ColumnName"].ToString() };
                    dbc.Ordinal = (int)dr["ColumnOrdinal"];
                    dbc.Type = dr["DataType"].ToString();
                    dbc.SqlType = sqldr.GetDataTypeName(dbc.Ordinal);
                    dbc.Size = (int)dr["ColumnSize"];
                    dbc.NumericPrecision = Convert.ToInt32(dr["NumericPrecision"]);
                    dbc.NumericScale = Convert.ToInt32(dr["NumericScale"]);
                    dbc.IsLong = (bool)dr["IsLong"];
                    dbc.IsNullable = (bool)dr["AllowDBNull"];
                    dbc.IsUnique = (bool)dr["IsUnique"];
                    dbc.IsKey = (DBNull.Value.Equals(dr["IsKey"])) ? false : (bool)dr["IsKey"];
                    dbc.IsAutoIncrement = (bool)dr["IsAutoIncrement"];
                    DBColumns.Add(dbc.Name, dbc);
                    i++;
                }
            }
            catch (Exception ex)
            {
                sqldr.Close();
                throw new Exception(ex.Message);
            }
            finally { CloseDB(); }

            /*
            // Limit result to the current table
            string[] restrictionValues = new string[4];
            restrictionValues[2] = TableName;

            ColumnList = conn.GetSchema("Columns", restrictionValues);

            foreach (DataRow dr in ColumnList.Rows)
            {

                DBColumn dbc = DBColumns[dr["COLUMN_NAME"].ToString()];
                dbc.HasDefault = (bool)dr["COLUMN_HASDEFAULT"];
                dbc.DefaultValue = dr["COLUMN_DEFAULT"] is DBNull ? string.Empty : dr["COLUMN_DEFAULT"].ToString();
                dbc.DatetimePrecision = dr["DATETIME_PRECISION"] is DBNull ? 0 : Convert.ToInt32(dr["DATETIME_PRECISION"]);
                DBColumns[dr["COLUMN_NAME"].ToString()] = dbc;
            }
            */
            return DBColumns;
        }

        internal override bool Import(string SourceTable, string DestTable, Dictionary<string, DBColumn> columns = null)
        {
            bool rCode;
            int rtnCode;
            string InsertSQL;
            int recCount = 0;
            SQLiteTransaction sqlT = null; ;

            SQLiteErrorCode returnCode;
            if (columns == null) columns = GetColumns(SourceTable);

            //Only if table does not exist
            string CreateSql = BuildCreateSql(DestTable, columns, out InsertSQL);

            SQLiteConnection SQConn = new SQLiteConnection();
            SQLiteCommand SQCmd = new SQLiteCommand();

            rCode = DataAccess.OpenDB(MainForm.mInstance.CurrentDB, ref SQConn, ref SQCmd, out returnCode);
            if (!rCode || returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(String.Format(Common.ERR_SQL, DataAccess.LastError, returnCode));
                return false;
            }

            try
            {
                sqlT = SQConn.BeginTransaction();
                SQCmd.CommandText = CreateSql;
                rtnCode = DataAccess.ExecuteNonQuery(SQCmd, out returnCode);
                if (rtnCode < 0 || returnCode != SQLiteErrorCode.Ok)
                {
                    Common.ShowMsg(String.Format(Common.ERR_SQL, DataAccess.LastError, returnCode));
                    sqlT.Rollback();
                    return false;
                }
                SQCmd.CommandText = InsertSQL;

                OpenDB();
                OdbcCommand SqlCmd = new OdbcCommand();
                SqlCmd.Connection = conn;
                SqlCmd.CommandText = string.Format("Select * FROM {0}", SourceTable);
                OdbcDataReader dr = SqlCmd.ExecuteReader();
                while (dr.Read())
                {
                    SQCmd.Parameters.Clear();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        //SQCmd.Parameters.AddWithValue(string.Format("p{0}", i.ToString()), dr[i]);
                        SQCmd.Parameters.AddWithValue(String.Empty, dr[i]);
                    }
                    SQCmd.ExecuteNonQuery();
                    recCount++;
                    if (recCount % 100 == 0) FireStatusEvent(ImportStatus.InProgress, recCount);
                }
                dr.Close();
                CloseDB();
                sqlT.Commit();
                DataAccess.CloseDB(SQConn);
                FireStatusEvent(ImportStatus.InProgress, recCount);
            }
            catch (Exception ex)
            {
                sqlT.Rollback();
                Common.ShowMsg(string.Format(Common.ERR_SQL, ex.Message, SQLiteErrorCode.Ok));
                return false;
            }
            finally
            {
                DataAccess.CloseDB(SQConn);
            }
            return true;
        }
    }
}
