using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.Config;

namespace SQLiteWorkshop
{
    class DBMSAccessManager : DBManager
    {
        // For future use. For the time being just execute the base constructor
        internal DBMSAccessManager(string SourceDB, string TargetDB, string SourceServer = null, string SourceUserName = null, string SourcePassword = null) : base(SourceDB, TargetDB, SourceServer, SourceUserName, SourcePassword)
        {
            ImportKey = CFG_IMPMSACCESS;
        }

        ~DBMSAccessManager()
        {
            CloseImportDB();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal override DBDatabaseList GetDatabaseList()
        {
            throw new NotImplementedException();
        }

        internal override DBSchema GetSchema()
        {

            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            try
            {
                if (conn == null || conn.State != ConnectionState.Open) OpenImportDB();
            }
            catch { return new DBSchema(); }

            // Exclude System Tables
            string[] restrictionValues = new string[4];
            restrictionValues[3] = "Table";

            DataTable TableList = ((OleDbConnection)conn).GetSchema("Tables", restrictionValues);

            foreach (DataRow dr in TableList.Rows)
            {
                DBTable dbt = new DBTable() { Name = dr["TABLE_NAME"].ToString() };
                Tables.Add(dbt.Name, dbt);
            }

            DBSchema schema = new DBSchema
            {
                Tables = Tables
            };
            CloseImportDB();
            return schema;
        }

        internal override Dictionary<string, DBColumn> GetColumns(string TableName)
        {

            Dictionary<string, DBColumn> DBColumns = base.GetColumns(TableName);

            // MSAccess maintains additional info in a somewhat proprietary manner
            // We need to retrieve it.
            try
            {
                if (conn == null || conn.State != ConnectionState.Open) OpenImportDB();
            }
            catch { return DBColumns; }

            // Limit result to the current table
            string[] restrictionValues = new string[4];
            restrictionValues[2] = TableName;

            DataTable ColumnList = ((OleDbConnection)conn).GetSchema("Columns", restrictionValues);

            foreach (DataRow dr in ColumnList.Rows)
            {

                DBColumn dbc = DBColumns[dr["COLUMN_NAME"].ToString()];
                dbc.HasDefault = (bool)dr["COLUMN_HASDEFAULT"];
                dbc.DefaultValue = dr["COLUMN_DEFAULT"] is DBNull ? string.Empty : dr["COLUMN_DEFAULT"].ToString();
                dbc.DatetimePrecision = dr["DATETIME_PRECISION"] is DBNull ? 0 : Convert.ToInt32(dr["DATETIME_PRECISION"]);
                DBColumns[dr["COLUMN_NAME"].ToString()] = dbc;
            }

            CloseImportDB();
            return DBColumns;
        }

        internal override bool Import(string SourceTable, string DestTable, Dictionary<string, DBColumn> columns = null)
        {
            return base.Import(SourceTable, DestTable, columns);
        }

        internal override DataTable PreviewData(string TableName)
        {
            return base.PreviewData(TableName);
        }
    }
}
