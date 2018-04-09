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
    class DBTextManager : DBManager
    {
        
        internal string FileName { get; set; }
        internal bool FirstRowHasHeadings { get; set; }
        internal char Delimiter { get; set; }
        internal string TextQualifier { get; set; }

        internal DBTextManager(string fileName) : base(fileName)
        {
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
                Common.ShowMsg(string.Format("Cannot Open File {0}\r\nError: {1}", FileName, ex.Message));
                return columns;
            }

            string line = sr.ReadLine();
            sr.Close();

            string[] col = SplitLine(line, -1, Delimiter, TextQualifier);
            int i;

            for (i = 0; i < col.Length; i++)
            {
                DBColumn dbc = new DBColumn();
                columns.Add(FirstRowHasHeadings ? col[i] : string.Format("Column {0}", i.ToString()), dbc);
            }

            return columns;
        }

        internal override bool Import(string SourceFile, string DestTable, Dictionary<string, DBColumn> columns)
        {
            bool rCode;
            int rtnCode;
            string InsertSQL;
            int recCount = 0;
            int i;
            SQLiteTransaction sqlT = null;
            StreamReader sr = null; ;

            SQLiteErrorCode returnCode;

            //Only if table does not exist
            string CreateSql = BuildCreateSql(DestTable, columns);

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
                SQCmd.CommandText = CreateSql;
                rtnCode = DataAccess.ExecuteNonQuery(SQCmd, out returnCode);
                if (rtnCode < 0 || returnCode != SQLiteErrorCode.Ok)
                {
                    Common.ShowMsg(String.Format(Common.ERR_SQL, DataAccess.LastError, returnCode));
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
                string line;

                if (FirstRowHasHeadings) sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    string[] fields = SplitLine(line, columns.Count, Delimiter, TextQualifier);
                    SQCmd.Parameters.Clear();
                    for (i = 0; i < fields.Length; i++)
                    {
                        if (bInclude[i])
                        {
                            if (columns[fldName[i]].Type != "blob")
                            { SQCmd.Parameters.AddWithValue(String.Empty, fields[i]); }
                            else
                            { SQCmd.Parameters.Add(String.Empty, DbType.Binary).Value = FormatData(fields[i]); }
                        }
                    }
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
            MainForm.mInstance.AddTable(DestTable);
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

        internal string[] SplitLine(string line, int count, char delimiter, string textQualifier)
        {
            string[] fields = count > 0 ? new string[count] : new string[999];
            string[] tmpFields;

            // if no delimiter is present, just split and return a max of 'count' fields
            if (string.IsNullOrWhiteSpace(textQualifier))
            {
                tmpFields = line.Split(delimiter);
                if (count < 0) count = tmpFields.Length;
                if (tmpFields.Length == count) return tmpFields;
                Array.Copy(tmpFields, fields, tmpFields.Length > count ? count : tmpFields.Length);
                return fields;
            }

            // Otherwise pull fields out 1 at a time
            int fieldCount = 0;
            int curpos = 0;
            bool tqFnd = false;
            string szDelimiter = delimiter.ToString();
            StringBuilder sb = new StringBuilder();

            while (curpos < line.Length)
            {
                string curchar = line.Substring(curpos, 1);
                if (tqFnd)                                                      // Are we processing Qualified Text?
                {
                    if (curchar != textQualifier)                               // if we haven't encountered a textqualifier, copy char to string and move on
                    {
                        sb.Append(curchar);
                    }
                    else
                    if (curpos == line.Length) break;                           // If we found ending qualifier and are at end off line, we're done
                    else
                    if (line.Substring(curpos + 1, 1) == textQualifier)         // if we found double text qualifier, copy char and move on but skip double
                    {
                        sb.Append(curchar);
                        curpos++;
                    }
                    else                                                        // otherwise we're at end of field
                    {
                        fields[fieldCount] = sb.ToString();                     // move field to results array
                        fieldCount++;                                           // increment fieldcounter
                        sb.Clear();                                             // clear stringbuilder
                        tqFnd = false;                                          // mark end of qualified field
                        //let's look for the delimiter so we can move to the next field
                        while (curpos < line.Length && line.Substring(curpos + 1, 1) != szDelimiter)
                        {
                            curpos++;                                           // move past this character
                            if (line.Substring(curpos, 1) != " ")               // if it's non blank, we have an improperly quoted field
                            {
                                return null;                                    // Just exit and return nothing
                            }
                        }
                    }
                }
                else
                if (curchar == textQualifier) { tqFnd = true; }                 // If we found a qualifier. indicate qualified mode and continue
                else
                if (curchar == szDelimiter || curpos == line.Length - 1)         // if we found a delimiter, we're at end of field
                {
                    fields[fieldCount] = sb.ToString();
                    fieldCount++;
                    sb.Clear();
                }
                else
                {
                    sb.Append(curchar);
                }

                curpos++;
                if (!(fieldCount < fields.Length)) break;
            }

            if (count < 0)
            {
                tmpFields = new string[fieldCount];
                Array.Copy(fields, tmpFields, fieldCount);
                return tmpFields;
            }
            return fields;
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
