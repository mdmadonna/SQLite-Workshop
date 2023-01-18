﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;

using ExcelDataReader;
using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.Config;

namespace SQLiteWorkshop
{
    class DBExcelManager : DBManager
    {
        
        internal string FileName { get; set; }
        internal bool FirstRowHasHeadings { get; set; }
        internal string WorkSheet { get; set; }
        internal Dictionary<string, ImportWizTextPropertySettings> ColumnSettings { get; set; }

        DataSet Contents;

        internal DBExcelManager(string fileName, string DBLoc) : base(fileName, DBLoc)
        {
            ImportKey = CFG_IMPEXCEL;
            FileName = fileName;
        }

        ~DBExcelManager()
        { }

        internal override DBDatabaseList GetDatabaseList()
        {

            DBDatabaseList DbDl = new DBDatabaseList
            {
                Databases = new Dictionary<string, DBInfo>()
            };

            try
            {
                using (Stream str = File.Open(FileName, FileMode.Open, FileAccess.Read))
                {
                    using (var excelDataReader = ExcelDataReader.ExcelReaderFactory.CreateOpenXmlReader(str))
                    {
                        for (int i = 0; i < excelDataReader.ResultsCount; i++)
                        {
                            DBInfo di = new DBInfo
                            {
                                Name = excelDataReader.Name
                            };
                            DbDl.Databases.Add(di.Name, di);
                            excelDataReader.NextResult();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Cannot Open File {0}\r\nError: {1}", FileName, ex.Message));
            }
            return DbDl;
        }

        /// <summary>
        /// Not valid for Excel Files.
        /// </summary>
        /// <returns></returns>
        internal override DBSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return column names for Excel files
        /// </summary>
        /// <param name="FileName">Name of the text file being imported</param>
        /// <returns></returns>
        internal override Dictionary<string, DBColumn> GetColumns(string FileName)
        {
            Dictionary<string, DBColumn> columns = new Dictionary<string, DBColumn>();

            if (Contents == null)
            {
                Stream sr;

                try
                {
                    sr = File.Open(FileName, FileMode.Open, FileAccess.Read);
                    var reader = ExcelReaderFactory.CreateReader(sr);
                    Contents = reader.AsDataSet();
                    reader.Close();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    ShowMsg(string.Format("Cannot Open File {0}\r\nError: {1}", FileName, ex.Message));
                    return columns;
                }
            }

            DataTable dt = Contents.Tables[WorkSheet];

            int i;

            for (i = 0; i < dt.Columns.Count; i++)
            {
                DBColumn dbc = new DBColumn();
                string columnName = FirstRowHasHeadings ? dt.Rows[0][i].ToString() : string.Format("Column {0}", i.ToString());
                //Make sure column name is unique
                int j = 0;
                while (columns.ContainsKey(columnName))
                {
                    j++;
                    columnName = string.Format("{0}_{1}", columnName, j);
                }
                columns.Add(columnName, dbc);
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
            DataTable dtSource = Contents.Tables[WorkSheet];

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

                int count = 0;
                if (FirstRowHasHeadings) count++;

                while (count < dtSource.Rows.Count)
                {
                    SQCmd.Parameters.Clear();
                    for (i = 0; i < columns.Count; i++)
                    {
                        if (i < dtSource.Columns.Count && bInclude[i])
                        {
                            var FieldValue = dtSource.Rows[count][i]?.ToString();
                            switch (columns[fldName[i]].Type.ToLower())
                            {
                                case "blob":
                                    SQCmd.Parameters.Add(String.Empty, DbType.Binary).Value = FormatData(FieldValue);
                                    break;
                                case "date":
                                case "datetime":
                                    if (DateTime.TryParse(FieldValue, out DateTime WrkDate))
                                    { SQCmd.Parameters.AddWithValue(String.Empty, WrkDate.ToString("o")); }
                                    else
                                    { SQCmd.Parameters.AddWithValue(String.Empty, FieldValue); }
                                    break;
                                default:
                                    SQCmd.Parameters.AddWithValue(String.Empty, FieldValue);
                                    break;
                            }
                        }
                    }
                    SQCmd.ExecuteNonQuery();
                    count++;
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

        
        /// <summary>
        /// Should never be called
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        internal override DataTable PreviewData(string TableName)
        {
            DataTable dt = new DataTable(); 
            DataTable dtTarget = Contents.Tables[WorkSheet];

            foreach (var cs in ColumnSettings)
            {
                DataColumn dc = new DataColumn(cs.Key)
                {
                    ColumnName = cs.Value.Name
                };
                dt.Columns.Add(dc);
            }

            int count = 0;
            int colcount = dt.Columns.Count;
            if (FirstRowHasHeadings) count++;
            int maxcount = 100 + count;
            try
            {
                while (count < dtTarget.Rows.Count)
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < colcount; i++)
                    {
                        dr[i] = dtTarget.Rows[count][i];
                    }
                    dt.Rows.Add(dr);
                    count++;
                    if (count >= maxcount) break;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                ShowMsg(msg);
            }
            return dt;
        }
    }
}