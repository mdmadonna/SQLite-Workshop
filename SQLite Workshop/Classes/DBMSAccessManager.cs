using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class DBMSAccessManager : DBManager
    {

        const string accdbConnectionstring = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=True";
        const string mdbConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data source=(0)";

        OleDbConnection conn;

        internal string DatabaseName { get; set; }
        internal string LastError { get; set; }

        internal DBMSAccessManager(string DBName) : base(DBName)
        {
            DatabaseName = DBName;
        }

        ~DBMSAccessManager()
        {
            CloseDB();
        }

        protected bool OpenDB()
        {
            conn = new OleDbConnection();
            conn.ConnectionString = DatabaseName.EndsWith(".mdb") ? string.Format(mdbConnectionString, DatabaseName) : string.Format(accdbConnectionstring, DatabaseName);
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
            throw new NotImplementedException();
        }

        internal override DBSchema GetSchema()
        {

            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            if (conn == null || conn.State != ConnectionState.Open) OpenDB();

            // Exclude System Tables
            string[] restrictionValues = new string[4];
            restrictionValues[3] = "Table";

            DataTable TableList = conn.GetSchema("Tables", restrictionValues);

            foreach (DataRow dr in TableList.Rows)
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

            if (conn == null || conn.State != ConnectionState.Open) OpenDB();

            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            cmd.CommandText = string.Format("Select * From {0}", TableName);
            OleDbDataReader OleDbdr = cmd.ExecuteReader(CommandBehavior.SingleRow);
            OleDbdr.Read();
            DataTable ColumnList = OleDbdr.GetSchemaTable();
            int i = 0;
            foreach (DataRow dr in ColumnList.Rows)
            {
                DBColumn dbc = new DBColumn() { Name = dr["ColumnName"].ToString() };
                dbc.Ordinal = (int)dr["ColumnOrdinal"];
                dbc.Type = dr["DataType"].ToString();
                dbc.SqlType = OleDbdr.GetDataTypeName(dbc.Ordinal);
                dbc.Size = (int)dr["ColumnSize"];
                dbc.NumericPrecision = Convert.ToInt32(dr["NumericPrecision"]);
                dbc.NumericScale = Convert.ToInt32(dr["NumericScale"]);
                dbc.IsLong = (bool)dr["IsLong"];
                dbc.IsNullable = (bool)dr["AllowDBNull"];
                dbc.IsUnique = (bool)dr["IsUnique"];
                dbc.IsKey = (bool)dr["IsKey"];
                dbc.IsAutoIncrement = (bool)dr["IsAutoIncrement"];
                DBColumns.Add(dbc.Name, dbc);
                i++;
            }

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




            CloseDB();
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
                OleDbCommand OleCmd = new OleDbCommand();
                OleCmd.Connection = conn;
                OleCmd.CommandText = string.Format("Select * FROM {0}", SourceTable);
                OleDbDataReader dr = OleCmd.ExecuteReader();
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
