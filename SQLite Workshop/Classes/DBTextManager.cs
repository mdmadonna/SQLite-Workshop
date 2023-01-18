using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

using CsvHelper;
using static SQLiteWorkshop.Config;
using static SQLiteWorkshop.Common;

namespace SQLiteWorkshop
{
    class DBTextManager : DBManager
    {
        
        internal string FileName { get; set; }
        internal bool FirstRowHasHeadings { get; set; }
        internal char Delimiter { get; set; }
        internal string TextQualifier { get; set; }
        internal Dictionary<string, ImportWizTextPropertySettings> ColumnSettings { get; set; }

        internal DBTextManager(string fileName, string DBLoc) : base(fileName, DBLoc)
        {
            ImportKey = CFG_IMPTEXT;
            FileName = fileName;
        }

        ~DBTextManager()
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
            Dictionary<string, DBColumn> columns = new Dictionary<string, DBColumn>();
            StreamReader sr;

            try
            {
                sr = new StreamReader(FileName);
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Cannot Open File {0}\r\nError: {1}", FileName, ex.Message));
                return columns;
            }

            CsvHelper.Configuration.Configuration csvConfig = BuildConfig();
            CsvReader csv = new CsvReader(sr, csvConfig);
            csv.Read();

            try
            {
                if (isBadRecord)
                {
                    ShowMsg(string.Format(ERR_BADRECORD, badRecords[0]));
                }
                if (FirstRowHasHeadings) { csv.ReadHeader(); }

                int i;

                for (i = 0; i < csv.Context.Record.Length; i++)
                {
                    DBColumn dbc = new DBColumn();
                    string columnName = FirstRowHasHeadings ? csv.Context.HeaderRecord[i] : string.Format("Column {0}", i.ToString());
                    //Make sure column name is unique
                    int j = 0;
                    while (columns.ContainsKey(columnName))
                    {
                        j++;
                        columnName = string.Format("{0}_{1}", columnName, j);
                    }
                    columns.Add(columnName, dbc);
                }
            }
            finally
            {
                sr.Close();
            }
            return columns;

        }

        internal override bool Import(string SourceFile, string DestTable, Dictionary<string, DBColumn> columns)
        {
            bool rCode;
            long rtnCode;
            string InsertSQL;
            long recCount = 0;
            int i;
            SQLiteTransaction sqlT = null;
            StreamReader sr = null;
            CsvReader csv = null;

            SQLiteErrorCode returnCode;

            //Only if table does not exist
            string CreateSql = BuildCreateSql(DestTable, columns);

            SQLiteConnection SQConn = new SQLiteConnection();
            SQLiteCommand SQCmd = new SQLiteCommand();

            rCode = DataAccess.OpenDB(TargetDB, ref SQConn, ref SQCmd);
            if (!rCode)
            {
                FireStatusEvent(ImportStatus.Failed, 0);
                ShowMsg(String.Format(ERR_OPEN, DataAccess.LastError));
                return false;
            }

            try
            {
                SQCmd.CommandText = CreateSql;
                rtnCode = DataAccess.ExecuteNonQuery(SQCmd, out returnCode);
                if (rtnCode < 0 || returnCode != SQLiteErrorCode.Ok)
                {
                    FireStatusEvent(ImportStatus.Failed, 0);
                    ShowMsg(String.Format(ERR_SQL, DataAccess.LastError, returnCode));
                    return false;
                }

                SQCmd.CommandText = string.Format("pragma table_info(\"{0}\")", DestTable);
                DataTable dt = DataAccess.ExecuteDataTable(SQCmd, out returnCode);
                InsertSQL = BuildInsertSql(DestTable, dt);

                sqlT = SQConn.BeginTransaction();
                SQCmd.CommandText = InsertSQL;

                bool[] bInclude = new bool[columns.Count];
                string[] fldName = new string[columns.Count];
                i = 0;
                foreach (var col in columns)
                {
                    bInclude[i] = col.Value.IncludeInImport;
                    fldName[i] = col.Key;
                    i++;
                }

                sr = new StreamReader(SourceFile);
                //string line;
                FireStatusEvent(ImportStatus.Starting, 0);

                CsvHelper.Configuration.Configuration csvConfig = BuildConfig();
                csv = new CsvReader(sr,csvConfig);

                if (FirstRowHasHeadings) { csv.Read(); csv.ReadHeader(); }
                while (csv.Read())
                {
                    if (isBadRecord) throw new Exception(string.Format(ERR_BADRECORD, badRecords[0]));

                    SQCmd.Parameters.Clear();
                    for (i = 0; i < columns.Count; i++)
                    {
                        if (i < csv.Context.Record.Length && bInclude[i])
                        {
                            switch (columns[fldName[i]].Type.ToLower())
                            {
                                case "blob":
                                    SQCmd.Parameters.Add(String.Empty, DbType.Binary).Value = FormatData(csv.GetField(i));
                                    break;
                                case "date":
                                case "datetime":
                                    if (DateTime.TryParse(csv.GetField(i), out DateTime WrkDate))
                                    { SQCmd.Parameters.AddWithValue(String.Empty, WrkDate.ToString("o")); }
                                    else
                                    { SQCmd.Parameters.AddWithValue(String.Empty, csv.GetField(i)); }
                                    break;
                                default:
                                    SQCmd.Parameters.AddWithValue(String.Empty, csv.GetField(i));
                                    break;
                            }
                        }
                    }
                    SQCmd.ExecuteNonQuery();
                    recCount++;
                    if (recCount % 100 == 0) FireStatusEvent(ImportStatus.InProgress, recCount, DestTable);
                }
               
                sr.Close();
                sqlT.Commit();
            }
            catch (Exception ex)
            {
                sqlT.Rollback();
                try { if (sr != null) sr.Close(); } catch { }
                FireStatusEvent(ImportStatus.Failed, 0);
                if (SQCmd.Connection.ExtendedResultCode() != SQLiteErrorCode.Ok)
                {
                    ShowMsg(string.Format(ERR_SQL, ex.Message, SQCmd.Connection.ExtendedResultCode().ToString()));
                }
                else
                {
                    ShowMsg(ex.Message);
                }
                return false;
            }
            finally
            {
                DataAccess.CloseDB(SQConn);
            }
            MainForm.mInstance.AddTable(DestTable, TargetDB);
            try { FireStatusEvent(ImportStatus.Complete, recCount); } catch { }
            return true;
        }

        internal object FormatData(string data)
        {
            string s = (data.StartsWith("x'")) ? data.Substring(3, data.Length - 3) : data;
            if (s.Length % 2 != 0) return null;

            byte[] b1 = new byte[s.Length / 2];
            for (int i = 0; i < s.Length / 2; i++)
            {
                b1[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            }
            return b1;
        }

        internal bool isBadRecord { get; set; }
        internal List<string> badRecords = new List<string>();

        internal CsvHelper.Configuration.Configuration BuildConfig()
        {
            CsvHelper.Configuration.Configuration ch = new CsvHelper.Configuration.Configuration
            {
                Quote = string.IsNullOrWhiteSpace(TextQualifier) ? '"' : TextQualifier.ToCharArray()[0],
                Delimiter = Delimiter.ToString(),
                HasHeaderRecord = FirstRowHasHeadings
            };
            isBadRecord = false;
            ch.BadDataFound = context => {
                isBadRecord = true;
                badRecords.Add(context.RawRecord);
            };
            ch.MissingFieldFound = null;

            return ch;
        }
       
        /// <summary>
        /// Setup preview data
        /// </summary>
        /// <param name="TableName">Not used</param>
        /// <returns></returns>
        internal override DataTable PreviewData(string TableName)
        {
            StreamReader sr;
            DataTable dt;

            try
            {
                sr = new StreamReader(FileName);
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Cannot Open File {0}\r\nError: {1}", FileName, ex.Message));
                return null;
            }

            dt = new DataTable();
            foreach (var cs in ColumnSettings)
            {
                DataColumn dc = new DataColumn(cs.Key)
                {
                    ColumnName = cs.Value.Name
                };
                dt.Columns.Add(dc);
            }

            int count = 0;

            CsvHelper.Configuration.Configuration csvConfig = BuildConfig();
            CsvReader csv = new CsvReader(sr, csvConfig);

            int colcount = 0;
            if (FirstRowHasHeadings) { csv.Read(); csv.ReadHeader(); }
            try
            {
                while (csv.Read())
                {
                    if (isBadRecord)
                    {
                        ShowMsg(string.Format(ERR_BADRECORD, badRecords[0]));
                        break;
                    }

                    if (colcount == 0) colcount = csv.Context.Record.Length;
                    string[] cols = new string[csv.Context.Record.Length];
                    for (int i = 0; i < cols.Length; i++)
                    {
                        cols[i] = csv.GetField(i);
                    }
                    dt.Rows.Add(cols);
                    count++;
                    if (count >= 100) break;
                }
            }
            catch (Exception ex)
            {
                try { if (sr != null) sr.Close(); } catch { }
                string msg = ex.Message;
                try { msg += csv.Context.RawRecord; } catch { }
                ShowMsg(msg);
            }
            return dt;
        }
    }
}
