using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SQLiteWorkshop
{
    class DBSqlServerManager : DBManager
    {
        SqlConnection conn;

        internal string DatabaseName { get; set; }
        internal string DatabaseServer { get; set; }
        internal string DatabaseUserName { get; set; }
        internal string DatabasePassword { get; set; }
        internal bool UseWindowsAuthentication { get; set; }
        internal string LastError { get; set; }

        internal DBSqlServerManager(string DBName) : base(DBName)
        {
            DatabaseServer = DBName;
        }

        ~DBSqlServerManager()
        {
            CloseDB();
        }

        protected bool OpenDB()
        {
            /*
             * Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;
             * Server=myServerAddress;Database=myDataBase;Trusted_Connection=True;
             */
            conn = new SqlConnection();
            conn.ConnectionString = UseWindowsAuthentication ? string.Format("Server={0};Database={1};Trusted_Connection=True;", DatabaseServer, DatabaseName) : string.Format("Server={0};Database={1};User Id={2};Password={3};", DatabaseServer, DatabaseName, DatabaseUserName, DatabasePassword);
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

        internal override DBDatabaseList GetDatabaseList()
        {
            OpenDB();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT name FROM sys.databases WHERE name NOT IN('master', 'tempdb', 'model', 'msdb', 'ReportServer', 'ReportServerTempDB')";

            DBDatabaseList DbDl = new DBDatabaseList();
            DbDl.Databases = new Dictionary<string, DBInfo>();
            SqlDataReader dr;

            try
            {
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DBInfo di = new DBInfo();
                    di.Name = dr["name"].ToString();
                    DbDl.Databases.Add(di.Name, di);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { CloseDB(); }
            return DbDl;
        }

        internal override DBSchema GetSchema()
        {

            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            if (conn == null || conn.State != ConnectionState.Open) OpenDB();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
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
            SqlDataReader sqldr;

            if (conn == null || conn.State != ConnectionState.Open) OpenDB();

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = string.Format("Select Top(1) * From {0}", TableName);
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
                SqlCommand SqlCmd = new SqlCommand();
                SqlCmd.Connection = conn;
                SqlCmd.CommandText = string.Format("Select * FROM {0}", SourceTable);
                SqlDataReader dr = SqlCmd.ExecuteReader();
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
                }
                dr.Close();
                CloseDB();
                sqlT.Commit();
                DataAccess.CloseDB(SQConn);
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
