using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteWorkshop
{
    class DBSqlManager : DBManager
    {
        
        internal string FileName { get; set; }

        internal DBSqlManager(string fileName) : base(fileName)
        {
            FileName = fileName;
        }

        ~DBSqlManager()
        { }

        internal override DBDatabaseList GetDatabaseList()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not valid for Text Files.
        /// </summary>
        /// <returns></returns>
        internal override DBSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return column names for text files
        /// </summary>
        /// <param name="FileName">Name of the text file being imported</param>
        /// <returns></returns>
        internal override Dictionary<string, DBColumn> GetColumns(string FileName)
        {
            throw new NotImplementedException();
        }

        internal override bool Import(string SourceFile, string DestTable, Dictionary<string, DBColumn> columns)
        {
            bool rCode;
            int recCount = 0;

            SQLiteTransaction sqlT = null;
            StreamReader sr = null; ;

            SQLiteErrorCode returnCode;

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
                sr = new StreamReader(SourceFile);
                string line;
                string sql;
                int idx;

                sqlT = SQConn.BeginTransaction();

                while ((line = sr.ReadLine()) != null)
                {
                    sql = string.Empty;
                    idx = line.LastIndexOf(";");
                    while (idx < 0)
                    {
                        line += sr.ReadLine();
                        idx = line.LastIndexOf(";");
                    }
                    sql = line.Substring(0, idx);
                    line = idx < line.Length ? line.Substring(idx + 1) : string.Empty;
                    SQCmd.CommandText = sql;
                    SQCmd.ExecuteNonQuery();
                    recCount++;
                    if (recCount % 100 == 0) FireStatusEvent(ImportStatus.InProgress, recCount);
                }
                sr.Close();
                sqlT.Commit();
            }
            catch (Exception ex)
            {
                sqlT.Rollback();
                try { if (sr != null) sr.Close(); } catch { }
                Common.ShowMsg(string.Format(Common.ERR_SQL, ex.Message, SQLiteErrorCode.Ok));
                return false;
            }
            finally
            {
                DataAccess.CloseDB(SQConn);
            }
            try { FireStatusEvent(ImportStatus.Complete, recCount); } catch { }
            return true;
        }

        /// <summary>
        /// Should never be called
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        internal override DataTable PreviewData(string TableName)
        {
            throw new NotImplementedException();
        }
    }
}
