using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.Config;
using static SQLiteWorkshop.DataAccess;

namespace SQLiteWorkshop
{
    class DBSQLiteManager : DBManager
    {

        // For future use. For the time being just execute the base constructor
        internal DBSQLiteManager(string SourceDB, string TargetDB, string SourceServer=null, string SourceUserName=null, string SourcePassword=null) : base(SourceDB, TargetDB, SourceServer, SourceUserName, SourcePassword)
        {
            ImportKey = CFG_IMPSQLITE;
        }

        ~DBSQLiteManager()
        {
        }

        /// <summary>
        /// Not applicable to SQLite
        /// </summary>
        /// <returns></returns>
        internal override DBDatabaseList GetDatabaseList()
        {
            throw new NotImplementedException();
        }

        internal override DBSchema GetSchema()
        {
            DBSchema schema = new DBSchema();
            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            Dictionary<string, TableLayout> SQLiteTables = GetTables(SourceDB, SourcePassword);
            if (SQLiteTables == null) return schema;

            foreach (var table in SQLiteTables)
            {
                if (!IsSystemTable(table.Key))
                {
                    DBTable dbt = new DBTable() { Name = table.Key };
                    Tables.Add(dbt.Name, dbt);
                }
            }

            schema.Tables = Tables;
            return schema;
        }

        internal override Dictionary<string, DBColumn> GetColumns(string TableName)
        {
            return base.GetColumns(TableName);
            /*
            SQLiteErrorCode returnCode;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!OpenDB(SourceDB, ref conn, ref cmd, false, SourcePassword)) return null;

            cmd.CommandText =  string.Format("Pragma table_info(\"{0}\")", TableName);
            DataTable dt = ExecuteDataTable(cmd, out returnCode);
            CloseDB(conn);

            Dictionary<string, DBColumn> DBColumns = new Dictionary<string, DBColumn>();

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                DBColumn dbc = new DBColumn() { Name = dr["name"].ToString() };
                dbc.Type = dr["type"].ToString();
                dbc.IsNullable = dr["notnull"].ToString() == "0";
                dbc.SqlType = dbc.Type;
                DBColumns.Add(dbc.Name, dbc);
                i++;
            }

            return DBColumns;
            */
        }

        internal override bool Import(string SourceTable, string DestTable, Dictionary<string, DBColumn> columns = null)
        {
            return base.Import(SourceTable, DestTable, columns);
        }

        internal override DataTable PreviewData(string TableName)
        {
            SQLiteErrorCode returnCode;
            SQLiteCommand cmd = DataAccess.AttachDatabase(TargetDB, SourceDB, "Import", out returnCode, SourcePassword);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                ShowMsg(String.Format("Could not attach {0}\r\n{1}", SourceDB, DataAccess.LastError));
                return null;
            }
            cmd.CommandText = string.Format("Select * FROM Import.\"{0}\" Limit 100", TableName);
            DataTable dt = LoadPreviewData(cmd);
            DataAccess.DetachDatabase(cmd, "Import", out returnCode);
            return dt;
        }
    }
}
