using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.Config;

namespace SQLiteWorkshop
{
    internal class LoadSqlEventArgs : EventArgs
    {
        internal string Message;
        internal long StmtCount;
    }

    internal class ImportError
    {
        internal int recordnum;
        internal int sqlnum;
        internal string sqlstmt;
        internal string errormsg;
    }

    class SqlLoader
    {
        internal event EventHandler<LoadSqlEventArgs> LoadSqlStatusReport;

        internal Dictionary<int, ImportError> ErrList = new Dictionary<int, ImportError>();
        bool ignoreErrors;
        int maxErrors;

        internal long RecordCount { get; set; }
        internal long SqlCount { get; set; }
        internal long ErrCount { get; set; }

        /// <summary>
        /// Not really used as it is always false. We expect BEGIN and COMMIT statements to
        /// be present in the input file if sql statements are to be executed within a 
        /// transaction.
        /// 
        /// Left here for future consideration
        /// 
        /// </summary>
        internal bool useTransaction { get; set; }

        /// <summary>
        /// Generic routine to read a text file of SQL statements and execute them.
        /// The SQL Statements will be wrapped in an SQLite Transaction,
        /// </summary>
        internal SqlLoader()
        {
            StartUp(true);
        }
        
        /// <summary>
        /// Generic routine to read a text file of SQL statements and execute them.
        /// </summary>
        /// <param name="Transaction">false if SQL should not be wrapped in a transaction,</param>
        internal SqlLoader(bool Transaction)
        {
            StartUp(Transaction);
        }

        internal void StartUp(bool Transaction)
        {
            useTransaction = Transaction;
            if (!bool.TryParse(appSetting(CFG_IGNOREIMPERRORS), out ignoreErrors)) ignoreErrors = false;
            if (!int.TryParse(appSetting(CFG_MAXIMPERRORS), out maxErrors)) maxErrors = 0;

        }
        /// <summary>
        /// Read an file of SQL statements and execute them.
        /// </summary>
        /// <param name="Source">File of SQL statements</param>
        /// <param name="TargetDB">SQLite Database that is the target of the SQL statements</param>
        /// <returns>true if all statements read and executed, otherwise false</returns>
        internal bool LoadSql(string Source, string TargetDB)
        {
            bool rCode;
            int recCount = 0;
            int sqlCount = 0;
            int errCount = 0;

            SQLiteTransaction sqlT = null;
            StreamReader sr = null;

            SQLiteConnection SQConn = new SQLiteConnection();
            SQLiteCommand SQCmd = new SQLiteCommand();

            rCode = DataAccess.OpenDB(TargetDB, ref SQConn, ref SQCmd);
            if (!rCode)
            {
                ShowMsg(String.Format(ERR_IMPDBOPEN, DataAccess.LastError));
                return false;
            }

            try
            {
                sr = new StreamReader(Source);
            }
            catch (Exception ex)
            {
                try { if (sr != null) sr.Close(); } catch { }
                throw new Exception(string.Format(ERR_CANTOPENINPUT, Source, ex.Message));
            }
                
            string line;
            string nextline;
            try
            {
                if (useTransaction) sqlT = SQConn.BeginTransaction();

                while ((line = sr.ReadLine()) != null)
                {
                    // Read input file until semicolon is found
                    recCount++;
                    while (!FoundEndMarker(line))
                    {
                        if ((nextline = sr.ReadLine()) != null)
                        {
                            recCount++;
                            // ReadLine strips newline chars so we need a space at the end for
                            // incomplete SQL statements
                            if (!nextline.EndsWith(";")) nextline += " ";
                            line += nextline;
                        }
                        else { break; }
                    }
                    SQCmd.CommandText = line;
                    try
                    {
                        SQCmd.ExecuteNonQuery();
                        sqlCount++;
                    }
                    // SQL Statement failed
                    // Execute failure logic based on import error settings
                    catch (Exception ex)
                    {
                        errCount++;
                        // Save the error related info
                        ImportError ie = new ImportError()
                        {
                            recordnum = recCount,
                            sqlnum = sqlCount,
                            sqlstmt = SQCmd.CommandText,
                            errormsg = ex.Message
                        };
                        ErrList.Add(errCount, ie);

                        //if max error count is reached and/or Ignore Errors is not set
                        //terminate the import, otherwise keep executing
                        if (!ignoreErrors && errCount > maxErrors)
                        {
                            if (useTransaction) sqlT.Rollback();
                            try { if (sr != null) sr.Close(); } catch { }
                            throw new Exception(ex.Message);
                        }
                    }
                    if (sqlCount % 100 == 0) FireLoadStatusEvent(string.Empty, sqlCount);
                }
                sr.Close();
                if (useTransaction) sqlT.Commit();
            }
            // File Read failed 
            // Fail the entire import
            catch (Exception ex)
            {
                if (useTransaction) sqlT.Rollback();
                try { if (sr != null) sr.Close(); } catch { }
                throw new Exception(ex.Message);
            }
            finally
            {
                // Autocommit is off only when this class did not wrap the executed SQL in a
                // Transaction and the input file had a BEGIN stmt without a corresponding
                // COMMIT.  If this happens, roll back everything.
                if (!SQConn.AutoCommit)
                {
                    SQCmd.CommandText = "ROLLBACK;";
                    SQCmd.ExecuteNonQuery();
                }
                DataAccess.CloseDB(SQConn);
                RecordCount = recCount;
                SqlCount = sqlCount;
                ErrCount = errCount;
            }
            try { FireLoadStatusEvent(string.Empty, sqlCount); } catch { }
            return true;
        }

        protected void FireLoadStatusEvent(string message, long RecordCount)
        {
            LoadSqlEventArgs e = new LoadSqlEventArgs
            {
                Message = message,
                StmtCount = RecordCount
            };
            EventHandler<LoadSqlEventArgs> eventHandler = LoadSqlStatusReport;
            eventHandler(this, e);
        }
    }
}
