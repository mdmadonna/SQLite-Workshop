using System.Collections.Generic;
using System.Data;

using static SQLiteWorkshop.Config;

namespace SQLiteWorkshop
{
    class DBMySqlManager : DBManager
    {

        // For future use. For the time being just execute the base constructor
        internal DBMySqlManager(string SourceDB, string TargetDB, string SourceServer, string SourceUserName, string SourcePassword) : base(SourceDB, TargetDB, SourceServer, SourceUserName, SourcePassword)
        {
            ImportKey = CFG_IMPMYSQL;
        }

        ~DBMySqlManager()
        {
            CloseImportDB();
        }

        internal override DBDatabaseList GetDatabaseList()
        {
            OpenImportDB();
            cmd.CommandText = "Show Schemas";
            IDataReader dr = cmd.ExecuteReader();

            DBDatabaseList DbDl = new DBDatabaseList
            {
                Databases = new Dictionary<string, DBInfo>()
            };

            while (dr.Read())
            {
                DBInfo di = new DBInfo
                {
                    Name = dr["Database"].ToString()
                };
                DbDl.Databases.Add(di.Name, di);
            }
            dr.Close();
            CloseImportDB();
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

            cmd.CommandText = string.Format("SELECT table_name FROM information_schema.tables where table_schema='{0}'", SourceDB);
            IDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                DBTable dbt = new DBTable() { Name = dr["table_name"].ToString() };
                Tables.Add(dbt.Name, dbt);
            }
            dr.Close();


            DBSchema schema = new DBSchema();
            schema.Tables = Tables;
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
