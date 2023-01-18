using System;
using System.Collections.Generic;
using System.Data;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.Config;

namespace SQLiteWorkshop
{
    class DBSqlManager : DBManager
    {
        
        internal string FileName { get; set; }
        public bool useTransaction { get; set; }
        
        long sqlCount;

        internal DBSqlManager(string fileName, string DBLoc) : base(fileName, DBLoc)
        {
            ImportKey = CFG_IMPSQL;
            FileName = fileName;
            // default to performing inserts wrapped in a transaction
            // Note that when the sql file being read has a 'BEGIN' stmt in it 
            // and useTransaction is true, an error will be generated.
            useTransaction = false;
        }

        ~DBSqlManager()
        { }

        /// <summary>
        /// Does not apply to SQL statements
        /// </summary>
        /// <returns></returns>
        internal override DBDatabaseList GetDatabaseList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Does not apply to SQL statements
        /// </summary>
        /// <returns></returns>
        internal override DBSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Does not apply to SQL statements
        /// </summary>
        /// <param name="FileName">Name of the text file being imported</param>
        /// <returns></returns>
        internal override Dictionary<string, DBColumn> GetColumns(string FileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Run the Import by reading a file of SQL statements and executing
        /// them.
        /// </summary>
        /// <param name="SourceFile">Name of the DQL File.</param>
        /// <param name="DestTable">null</param>
        /// <param name="columns">null</param>
        /// <returns>true if successful, otherwise false</returns>
        internal override bool Import(string Source, string DestTable, Dictionary<string, DBColumn> columns)
        {
            
            SqlLoader sl = new SqlLoader(useTransaction);
            sl.LoadSqlStatusReport += LoadSqlStatusReport;
            try
            {
                sl.LoadSql(Source, TargetDB);
            }
            catch (Exception ex)
            {
                FireStatusEvent(ImportStatus.Failed, 0);
                ShowMsg(string.Format(ERR_SQLLOADERR, ex.Message));
                return false;
            }
            finally { sl.LoadSqlStatusReport -= LoadSqlStatusReport; }
            try { FireStatusEvent(ImportStatus.Complete, sqlCount); } catch { }
            return true;
        }

        internal void LoadSqlStatusReport(object sender, LoadSqlEventArgs e)
        {
            sqlCount = e.StmtCount;
            FireStatusEvent(ImportStatus.InProgress, e.StmtCount);
        }


        /// <summary>
        /// Does not apply to SQL statements
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        internal override DataTable PreviewData(string TableName)
        {
            throw new NotImplementedException();
        }
    }
}
