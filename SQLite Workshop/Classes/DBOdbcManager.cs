using Microsoft.Win32;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

using static SQLiteWorkshop.Config;

namespace SQLiteWorkshop
{
    class DBOdbcManager : DBManager
    {

        // For future use. For the time being just execute the base constructor
        internal DBOdbcManager(string SourceDB, string TargetDB, string SourceServer = null, string SourceUserName = null, string SourcePassword = null) : base(SourceDB, TargetDB, SourceServer, SourceUserName, SourcePassword)
        {
            ImportKey = CFG_IMPODBC;
        }

        ~DBOdbcManager()
        {
            CloseImportDB();
        }

        /// <summary>
        /// Retrieve a list of ODBC Data Sources
        /// </summary>
        /// <returns></returns>
        internal override DBDatabaseList GetDatabaseList()
        {
            DBDatabaseList DbDl = new DBDatabaseList();
            DbDl.Databases = new Dictionary<string, DBInfo>();
            List<string> dsnList = GetDsnList();

            foreach (string dsn in dsnList)
            {
                DBInfo di = new DBInfo
                {
                    Name = dsn
                };
                DbDl.Databases.Add(di.Name, di);
            }
            return DbDl;
        }

        /// <summary>
        /// Read the registry to obtain a complete list of Data Sources
        /// </summary>
        /// <returns>List of Data Sources defined on this machine</returns>
        private List<string> GetDsnList()
        {
            List<string> list = new List<string>();
            list.AddRange(GetDsnList(Registry.CurrentUser));
            list.AddRange(GetDsnList(Registry.LocalMachine));
            return list;
        }

        /// <summary>
        /// Actual Registry read routine
        /// </summary>
        /// <param name="rootKey">Registry key to read</param>
        /// <returns>list of Data Sources</returns>
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
            DataTable dt;
            Dictionary<string, DBTable> Tables = new Dictionary<string, DBTable>();

            try
            {
                if (conn == null || conn.State != ConnectionState.Open) OpenImportDB();
            }
            catch { return new DBSchema(); }

            dt = ((OdbcConnection)conn).GetSchema("Tables");
            
            foreach (DataRow dr in dt.Rows)
            {
                DBTable dbt = new DBTable() { Name = dr["TABLE_NAME"].ToString() };
                Tables.Add(dbt.Name, dbt);
            }

            /*  Hold off on views for the time being
            dt = conn.GetSchema("Views");

            foreach (DataRow dr in dt.Rows)
            {
                DBTable dbt = new DBTable() { Name = dr["TABLE_NAME"].ToString() };
                Tables.Add(dbt.Name, dbt);
            }
            */

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
            return Import(SourceTable, DestTable, columns);
        }
        internal override DataTable PreviewData(string TableName)
        {
            return base.PreviewData(TableName);
        }
    }
}
