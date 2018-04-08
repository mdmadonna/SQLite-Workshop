using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class DBSQLiteManager : DBManager
    {

        internal string DatabaseName { get; set; }

        internal DBSQLiteManager(string DBName) : base(DBName)
        {
            DatabaseName = DBName;
        }

        ~DBSQLiteManager()
        {
        }

        internal override DBDatabaseList GetDatabaseList()
        {
            throw new NotImplementedException();
        }

        internal override DBSchema GetSchema()
        {

            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            Dictionary<string, TableLayout> SQLiteTables = DataAccess.GetTables(DatabaseName);
            
            foreach (var table in SQLiteTables)
            {
                if (!Common.IsSystemTable(table.Key))
                {
                    DBTable dbt = new DBTable() { Name = table.Key };
                    Tables.Add(dbt.Name, dbt);
                }
            }

            DBSchema schema = new DBSchema();
            schema.Tables = Tables;
            return schema;
        }

        internal override Dictionary<string, DBColumn> GetColumns(string TableName)
        {
            SQLiteErrorCode returnCode;

            string sql = string.Format("Pragma table_info(\"{0}\")", TableName);
            DataTable dt = DataAccess.ExecuteDataTable(DatabaseName, sql, out returnCode);

            Dictionary<string, DBColumn> DBColumns = new Dictionary<string, DBColumn>();

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                DBColumn dbc = new DBColumn() { Name = dr["name"].ToString() };
                dbc.Type = dr["type"].ToString();
                dbc.IsNullable = dr["notnull"].ToString() == "0" ? true : false;
                DBColumns.Add(dbc.Name, dbc);
                i++;
            }

            return DBColumns;
        }

        internal override bool Import(string SourceTable, string DestTable, Dictionary<string, DBColumn> columns = null)
        {

            SQLiteErrorCode returnCode;
            
            SQLiteCommand cmd = DataAccess.AttachDatabase(MainForm.mInstance.CurrentDB, DatabaseName, "Import", out returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(String.Format("Could not attach {0}\r\n{1}", DatabaseName, DataAccess.LastError));
                return false;
            }

            cmd.CommandText = string.Format("CREATE TABLE If Not Exists \"{0}\" AS SELECT * FROM Import.\"{1}\"", DestTable, SourceTable);
            int count = DataAccess.ExecuteNonQuery(cmd, out returnCode);
            if ( returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(String.Format("Could not create table {0}\r\n{1}", DestTable, DataAccess.LastError));
                DataAccess.DetachDatabase(cmd, "Import", out returnCode);
                return false;
            }

            cmd.CommandText = string.Format("Insert into \"{0}\" Select * From Import.\"{1}\"", DestTable,  SourceTable);
            count = DataAccess.ExecuteNonQuery(cmd, out returnCode);
            if (count < 0 || returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(String.Format("Import failed.\r\n{0}", DataAccess.LastError));
                DataAccess.DetachDatabase(cmd, "Import", out returnCode);
                return false;
            }

            DataAccess.DetachDatabase(cmd, "Import", out returnCode);
            FireStatusEvent(ImportStatus.Complete, count);
            return true;
        }

        internal override DataTable PreviewData(string TableName)
        {
            SQLiteErrorCode returnCode;
            SQLiteCommand cmd = DataAccess.AttachDatabase(MainForm.mInstance.CurrentDB, DatabaseName, "Import", out returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(String.Format("Could not attach {0}\r\n{1}", DatabaseName, DataAccess.LastError));
                return null;
            }
            cmd.CommandText = string.Format("Select * FROM Import.\"{0}\" Limit 100", TableName);
            DataTable dt = LoadPreviewData(cmd);
            DataAccess.DetachDatabase(cmd, "Import", out returnCode);
            return dt;
        }


    }
}
