using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class DBMySqlManager : DBManager
    {
        System.Data.IDbConnection conn;

        internal string DatabaseServer { get; set; }
        internal string DatabaseName { get; set; }
        internal string DatabaseUserName { get; set; }
        internal string DatabasePassword { get; set; }
        internal bool UseWindowsAuthentication { get; set; }
        internal string LastError { get; set; }

        string MySqlLibrary = "MySql.Data.dll";
        string MySqlConnectionClass = "MySql.Data.MySqlClient.MySqlConnection";
        string MySqlCommandClass = "MySql.Data.MySqlClient.MySqlCommand";
        Assembly assMySql;
        object iMySql;

        internal DBMySqlManager(string DBName) : base(DBName)
        {
            DatabaseServer = DBName;
            FileInfo fi = new FileInfo(MySqlLibrary);
            if (!fi.Exists)
            {
                Common.ShowMsg("The MySql driver is not found.  Please place it the the execution directory.");
                return;
            }
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            assMySql = Assembly.LoadFile(string.Format("{0}\\{1}", dir, MySqlLibrary));
            iMySql = assMySql.CreateInstance(MySqlConnectionClass, true);
            conn = (IDbConnection)assMySql.CreateInstance(MySqlConnectionClass, true);
        }

        ~DBMySqlManager()
        {
            CloseDB();
        }

        protected bool OpenDB()
        {

            conn.ConnectionString = String.Format("Server={0};Database={1};Uid={2};Pwd={3};", DatabaseServer, DatabaseName, DatabaseUserName, DatabasePassword);
            //if (!string.IsNullOrEmpty(DatabaseUserName)) conn.ConnectionString = string.Format("{0}UID={1};Pwd={2};", conn.ConnectionString, DatabaseUserName, DatabasePassword);
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

        internal bool TestMySqlConnection()
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
            OpenDB();
            IDbCommand cmd = (IDbCommand)assMySql.CreateInstance(MySqlCommandClass, true);
            cmd.Connection = conn;
            cmd.CommandText = "Show Schemas";
            IDataReader dr = cmd.ExecuteReader();

            DBDatabaseList DbDl = new DBDatabaseList();
            DbDl.Databases = new Dictionary<string, DBInfo>();
            
            while (dr.Read())
            {
                DBInfo di = new DBInfo();
                di.Name = dr["Database"].ToString();
                DbDl.Databases.Add(di.Name, di);
            }
            dr.Close();
            CloseDB();
            return DbDl;
        }

        internal override DBSchema GetSchema()
        {

            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            if (conn == null || conn.State != ConnectionState.Open) OpenDB();

            IDbCommand cmd = (IDbCommand)assMySql.CreateInstance(MySqlCommandClass, true);
            cmd.Connection = conn;
            cmd.CommandText = string.Format("SELECT table_name FROM information_schema.tables where table_schema='{0}'", DatabaseName);
            IDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                DBTable dbt = new DBTable() { Name = dr["table_name"].ToString() };
                Tables.Add(dbt.Name, dbt);
            }
            dr.Close();


            DBSchema schema = new DBSchema();
            schema.Tables = Tables;
            CloseDB();
            return schema;
        }

        internal override Dictionary<string, DBColumn> GetColumns(string TableName)
        {

            Dictionary<string, DBColumn> DBColumns = new Dictionary<string, DBColumn>();
            IDataReader sqldr;

            if (conn == null || conn.State != ConnectionState.Open) OpenDB();

            IDbCommand cmd = (IDbCommand)assMySql.CreateInstance(MySqlCommandClass, true);
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
                    dbc.SqlType = sqldr.GetDataTypeName(dbc.Ordinal - 1);
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
                sqldr.Close();
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
                IDbCommand SqlCmd = (IDbCommand)assMySql.CreateInstance(MySqlCommandClass, true);
                SqlCmd.Connection = conn;
                SqlCmd.CommandText = string.Format("Select * FROM {0}", SourceTable);
                IDataReader dr = SqlCmd.ExecuteReader();
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
                FireStatusEvent(ImportStatus.Complete, recCount);
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
