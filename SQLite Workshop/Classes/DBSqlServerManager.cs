using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using static SQLiteWorkshop.Config;

namespace SQLiteWorkshop
{
    class DBSqlServerManager : DBManager
    {

        // For future use. For the time being just execute the base constructor
        internal DBSqlServerManager(string SourceDB, string TargetDB, string SourceServer, string SourceUserName, string SourcePassword) : base(SourceDB, TargetDB, SourceServer, SourceUserName, SourcePassword)
        {
            ImportKey = CFG_IMPSQLSERVER;
        }

        ~DBSqlServerManager()
        {
            CloseImportDB();
        }

        internal override DBDatabaseList GetDatabaseList()
        {
            OpenImportDB();
            cmd.CommandText = "SELECT [name] FROM sys.databases WHERE name NOT IN('master', 'tempdb', 'model', 'msdb', 'ReportServer', 'ReportServerTempDB') ORDER BY [name]";

            DBDatabaseList DbDl = new DBDatabaseList
            {
                Databases = new Dictionary<string, DBInfo>()
            };
            SqlDataReader dr;

            try
            {
                dr = ((SqlCommand)cmd).ExecuteReader();
                while (dr.Read())
                {
                    DBInfo di = new DBInfo
                    {
                        Name = dr["name"].ToString()
                    };
                    DbDl.Databases.Add(di.Name, di);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally { CloseImportDB(); }
            return DbDl;
        }

        internal override DBSchema GetSchema()
        {

            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            try
            {
                if (conn == null || conn.State != ConnectionState.Open) OpenImportDB();
            }
            catch { return new DBSchema(); }

            cmd.CommandText = "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";

            SqlDataReader dr = ((SqlCommand)cmd).ExecuteReader();

            while (dr.Read())
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
            return base.GetColumns(TableName);
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
