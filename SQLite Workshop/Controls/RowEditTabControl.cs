using System;
using System.Drawing;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using static SQLiteWorkshop.Common;

namespace SQLiteWorkshop
{
    internal partial class RowEditTabControl : MainTabControl
    {
        internal struct ColData
        {
            internal string colname;
            internal string coltype;
            internal string collength;
        }

        string RowIDColName;
        int RowIdIndex;
        string PrimaryKeyName;
        int PKIndex;
        bool tableHasRowID;

        string BaseSQL = string.Empty;
        string searchSQL = string.Empty;
        string searchWhereClause = string.Empty;
        long startclock;
        long CurrentRow = 0;
        long RowCount = 0;
        bool AddingRow = false;

        DataTable dt;
        readonly string[] Rowids = new string[] { "rowid", "_rowid_", "OID" };

        protected enum RowStatus
        {
            Ok,
            Updated,
            NotFound,
            Inconsistent
        }

        internal override string SqlStatement { 
            get 
            {
                string wc = string.IsNullOrEmpty(richTextWhere.Text) ? string.Empty : string.Format("Where {0}", richTextWhere.Text.Trim());
                return string.Format("{0} {1}", BaseSQL, wc); 
            }
            set { } 
        }

        
        public string TableName { get; set; }

        internal RowEditTabControl(string dbName, string tblname)
        {
            InitializeComponent();
            InitializeClass(dbName);
            this.Dock = DockStyle.Fill;
            toolStripLabelTotalRecords.Text = string.Empty;
            toolStripLabelStatus.Text = string.Empty;
            TableName = tblname;
            InitializePage();
            LoadRecord(0);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            InitializePage();
            LoadRecord(0);
        }


        /// <summary>
        /// Determine how many resords are in the table and point the editor to the first row.  If the table
        /// is empty, go into insert mode and dispaly a blank page.
        /// </summary>
        protected void InitializePage()
        {
            lblTable.Text = TableName;

            tableHasRowID = DataAccess.CheckForRowID(DatabaseName, TableName, out RowIDColName, out PrimaryKeyName);

            BaseSQL = string.Format("Select Count(*) From \"{0}\"", TableName);
            searchWhereClause = string.IsNullOrEmpty(richTextWhere.Text) ? string.Empty : string.Format("Where {0}", richTextWhere.Text.Trim());

            RowIdIndex = -1;
            searchSQL = string.Format("{0} {1}", BaseSQL, searchWhereClause);

            int iRowCount = Convert.ToInt32(DataAccess.ExecuteScalar(DatabaseName, searchSQL, out SQLiteErrorCode returnCode));
            if (iRowCount == -1 || returnCode != SQLiteErrorCode.Ok)
            {
                ShowMsg(string.Format(ERR_SQLWHERE, DataAccess.LastError));
                searchSQL = BaseSQL;
                searchWhereClause = string.Empty;
                RowCount = 0;
                LoadRecord(0);
                return;
            }

            RowCount = iRowCount;

            BaseSQL = tableHasRowID ? string.Format("Select {0}, * From \"{1}\"", RowIDColName, TableName) : BaseSQL = string.Format("Select * From \"{0}\"", TableName);
            searchSQL = string.Format("{0} {1}", BaseSQL, searchWhereClause);

            SQLiteConnection conn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            DataAccess.OpenDB(DatabaseName, ref conn, ref cmd, false);
            DataTable ColumnList;
            SQLiteDataReader dr;
            cmd.CommandText = searchSQL;
            try
            {
                dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
                ColumnList = dr.GetSchemaTable();
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format(ERR_SQLWHERE, ex.Message));
                searchSQL = BaseSQL;
                searchWhereClause = string.Empty;
                DataAccess.CloseDB(conn);
                RowCount = 0;
                LoadRecord(0);
                return;
            }

            int start = 50;
            int lablen = 180;
            int height = 30;

            // draw a textbox for each column in the row except rowid.
            for (int i = 0; i < dr.FieldCount; i++)
            {
                string colname = dr.GetName(i);
                if (RowIdIndex < 0) if (tableHasRowID) if (RowIDColName == colname) RowIdIndex = i;
                if (i == RowIdIndex) continue;
                if (colname == PrimaryKeyName) PKIndex = i;

                Label lbl = new Label
                {
                    Text = string.Format("{0}:", colname),
                    Top = start,
                    Left = 50,
                    Width = lablen - 10
                };
                panelBody.Controls.Add(lbl);

                TextBox txt = new TextBox
                {
                    Name = string.Format("txt{0}", i.ToString().PadLeft(4, '0'))
                };
                ColData cd = new ColData()
                {
                    colname = colname,
                    //coltype = dr.GetDataTypeName(i),
                    coltype = ColumnList.Rows[i].ItemArray[24].ToString(),
                    collength = ColumnList.Rows[i]["ColumnSize"].ToString()
                };

                txt.Tag = cd;
                txt.Top = start;
                txt.Left = lbl.Left + lablen;
                txt.Width = 300;
                panelBody.Controls.Add(txt);

                if (dr.GetFieldType(i).Equals(typeof(byte[])))
                {
                    txt.ReadOnly = true;
                    Button btn = new Button
                    {
                        Name = string.Format("btn{0}", i.ToString().PadLeft(4, '0')),
                        Text = "View",
                        Left = txt.Left + 320,
                        Top = txt.Top - 2,
                        Height = 24,
                        Width = 60,
                        BackColor = SystemColors.Control,
                        Tag = colname
                    };
                    btn.Click += button_clicked;
                    panelBody.Controls.Add(btn);

                }
                start += height;
            }
            dr.Close();
            DataAccess.CloseDB(conn);
        }

        protected void button_clicked(object sender, EventArgs e)
        {
            if (!ShowData(((Button)sender).Tag.ToString()))
            {
                ShowMsg(ERR_NODISPLAY);
                return;
            }
        }
        /// <summary>
        /// Load a record onto the window and enable/disable controls appropriately
        /// </summary>
        /// <param name="RowNum"></param>
        protected void LoadRecord(long RowNum)
        {
            if (RowCount == 0) { InitEmptyNavigator(); return; }

            if (RowNum > RowCount) RowNum = RowCount;
            if (RowNum < 1) RowNum = 1;

            string sql = string.Format("{0} Limit 1 Offset {1}", searchSQL, RowNum - 1);

            InitSettings();

            dt = DataAccess.ExecuteDataTable(DatabaseName, sql, out SQLiteErrorCode returnCode);

            ShowSettings(dt == null ? 0 : dt.Rows.Count, returnCode);
            
            if (dt.Rows.Count != 1) return;
            DataRow dr = dt.Rows[0];
            for (int i = 0; i < dr.ItemArray.Count(); i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                t.Text = dr.ItemArray[i].ToString();
            }
            CurrentRow = RowNum;
            toolStripButtonMoveFirst.Enabled = true;
            toolStripButtonMoveLast.Enabled = true;
            toolStripButtonMovePrevious.Enabled = CurrentRow != 1;
            toolStripButtonMoveNext.Enabled = CurrentRow != RowCount;
            InitNavigatorStatus();
        }

        /// <summary>
        /// Separate routine to initialize the navigator for empty tables
        /// </summary>
        protected void InitEmptyNavigator()
        {
            AddingRow = true;
            toolStripButtonMoveFirst.Enabled = false;
            toolStripButtonMoveLast.Enabled = false;
            toolStripButtonMovePrevious.Enabled = false;
            toolStripButtonMoveNext.Enabled = false;
            CurrentRow = 0;
            for (int i = 0; i <= panelBody.Controls.Count; i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                if (t == null) break;
                t.Text = string.Empty;
            }
            InitNavigatorStatus();
            return;
        }

        /// <summary>
        /// Display the current row and total rows in the navigator
        /// </summary>
        private void InitNavigatorStatus()
        {
            toolStripTextBoxCurrentItem.Text = CurrentRow.ToString();
            toolStripLabelTotalRecords.Text = string.Format("of {0}", RowCount.ToString());
        }

        private bool ShowData(string columnName)
        {

            byte[] data;

            string sql = string.Format("Select {0} From {1}", columnName, TableName);
            bool bld = BuildWhereClause(dt.Rows[0], out string WhereClause);
            if (!bld) return false;

            try
            {
                data = (byte[])DataAccess.ExecuteScalar(DatabaseName, string.Format("{0} {1}", sql, WhereClause), out SQLiteErrorCode returnCode);
            }
            catch { return false; }

            ShowImg si = new ShowImg();

            // Not terribly exact but will do for now.
            Boolean isAscii = data.All(b => b >= 8 && b <= 127);
            if (isAscii)
            {
                string str = System.Text.Encoding.Default.GetString(data);
                si.setText(str);
                si.Show();
                return true;
            }

            //Let's try to display it as a picture
            try
            {
                if (si.setPicture(data))
                {
                    si.Show();
                    return true;
                }
            }
            catch { }

            //Binary is last resort
            StringBuilder t = FormatBinary(data);
            if (t == null) return false;

            si.setBinary(t.ToString());
            si.Show();
            return true;
        }

        protected StringBuilder FormatBinary(byte[] data)
        {
            try
            {
                string binstr = string.Join("", data.Select(b => Convert.ToString(b, 16).PadLeft(2, '0').ToUpper()));
                int m = binstr.Length % 64;
                StringBuilder sb = new StringBuilder();
                sb.Append(binstr);
                if (m > 0) sb.Append(new string(' ', 64 - m));

                int i = 0;
                StringBuilder t = new StringBuilder();
                StringBuilder ch = new StringBuilder();
                while (i < sb.Length)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        t.AppendFormat("{0}  ", ((i / 2) + (j * 16)).ToString("X8"));
                        for (int k = 0; k < 4; k++)
                        {
                            string chunk = sb.ToString(i + (j * 32) + (k * 8), 8);
                            t.AppendFormat("{0} ", chunk);
                            for (int l = 0; l < 4; l++)
                            {
                                String hs = chunk.Substring(l, 2).Trim();
                                if (!string.IsNullOrWhiteSpace(hs))
                                {
                                    uint ui = Convert.ToUInt16(hs, 16);
                                    ch.Append(ui >= 32 && ui <= 127 ? Convert.ToChar(ui).ToString() : ".");
                                }
                            }
                            ch.Append(" ");
                        }
                        t.Append("   ");
                    }
                    t.AppendFormat("   {0}\r\n", ch.ToString());
                    ch.Clear();
                    i += 64;
                }
                return t;
            }
            catch { return null; }

        }
        #region Row Update Routines

        #region Update a row
        /// <summary>
        /// Determine if any changes have been made and update any changed columns.  Make
        /// sure the underlying row has not been updated in the meantime.
        /// </summary>
        /// <returns></returns>
        protected bool UpdateRow()
        {
            toolStripLabelStatus.Text = string.Empty;
            if (AddingRow) return InsertRow();
            if (RowCount == 0) return false;

            // Iterate through all the rows to determine if any data chas changed.
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Update \"{0}\" Set", TableName);
            int count = 0;
            ArrayList parms = new ArrayList();

            DataRow dr = dt.Rows[0];

            int i;
            for (i = 0; i < dr.ItemArray.Count(); i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                if (dr[i].ToString() != t.Text)
                {
                    ColData cd = (ColData)t.Tag;
                    count++;
                    sb.Append(count > 1 ? "," : string.Empty).AppendFormat(" \"{0}\" = ?", cd.colname);
                    if (!ValidateData(t, out string szData)) return false;
                    parms.Add(szData);
                }
            }
            // If no data has changed, just return
            if (count == 0) return true;

            //Let's see if the row was updated through another process
            RowStatus rowStat = RecordUpdated(out DataRow currDataRow);
            DialogResult dgResult;
            switch (rowStat)
            {
                case RowStatus.Updated:
                    dgResult = ShowMsg("The Row has changed since it was retrieved.  Click 'Yes' to save your changes anyway or click 'No' to discard your change and retrieve current data for this row.", MessageBoxButtons.YesNo);
                    if (dgResult == DialogResult.No)
                    {
                        LoadRecord(CurrentRow);
                        return false;
                    }
                    break;
                case RowStatus.Inconsistent:
                    ShowMsg("The Row has changed since it was retrieved.  Please read the Row again before updating it.");
                    return false;
                case RowStatus.NotFound:
                    ShowMsg("The Row cannot be found.");
                    return false;
                default:
                    break;
            }

            if (!BuildWhereClause(currDataRow, out string whereClause)) return false;
            sb.AppendFormat(" {0}", whereClause);

            InitSettings();

            long recsupdated = DataAccess.ExecuteNonQuery(DatabaseName, sb.ToString(), parms, out SQLiteErrorCode returnCode);
            toolStripLabelStatus.Text = string.Format("{0} Record(s) updated.", recsupdated.ToString());

            ShowSettings(recsupdated, returnCode);
            return true;
        }
        #endregion

        #region Insert Row
        /// <summary>
        /// Insert the current into the table.
        /// </summary>
        /// <returns></returns>
        protected bool InsertRow()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Insert Into \"{0}\" (", TableName);
            StringBuilder sbValues = new StringBuilder();
            sbValues.Append("Values(");
            ArrayList parms = new ArrayList();

            int count = 0;
            for (int i = 0; i <= panelBody.Controls.Count; i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                if (t == null) break;
                if (!string.IsNullOrEmpty(t.Text))
                {
                    count++;
                    ColData cd = (ColData)t.Tag;
                    sb.Append(count > 1 ? "," : string.Empty).AppendFormat(" \"{0}\"", cd.colname);
                    sbValues.Append(count > 1 ? "," : string.Empty).Append("?");
                    if (!ValidateData(t, out string szData)) return false;
                    parms.Add(szData);
                }
            }
            if (count == 0)
            {
                ShowMsg("No data to insert. Please enter data to insert or press the 'Undo' button to cancel insert.");
                return false;
            }
            sb.Append(")");
            sbValues.Append(")");

            InitSettings();
            long recsupdated = DataAccess.ExecuteNonQuery(DatabaseName, string.Format("{0} {1}", sb.ToString(), sbValues.ToString()), parms, out SQLiteErrorCode returnCode);
            ShowSettings(recsupdated, returnCode);

            if (recsupdated == -1 || returnCode != SQLiteErrorCode.Ok)
            {
                ShowMsg(string.Format(ERR_SQL, DataAccess.LastError, returnCode));
                return false;
            }

            toolStripLabelStatus.Text = string.Format("{0} Record(s) added.", recsupdated.ToString());
            AddingRow = false;
            RowCount++;
            CurrentRow = RowCount;
            return true;
        }
        #endregion

        #region Delete Row
        /// <summary>
        /// Delete the current row after determining that the underlying row has not been changed since it was displayed.
        /// </summary>
        /// <returns></returns>
        protected bool DeleteRow()
        {
            if (RowCount == 0) return false;
            if (AddingRow) return false;

            toolStripLabelStatus.Text = string.Empty;
            DialogResult dgResult;
            RowStatus rowStat = RecordUpdated(out DataRow currDataRow);

            switch (rowStat)
            {
                case RowStatus.Updated:
                    dgResult = ShowMsg("The Row has changed since it was retrieved.  Click 'Yes' to delete this row or click 'No' to abort.", MessageBoxButtons.YesNo);
                    if (dgResult == DialogResult.No)
                    {
                        LoadRecord(CurrentRow);
                        return false;
                    }
                    break;
                case RowStatus.Inconsistent:
                    ShowMsg("The Row has changed since it was retrieved.  Please read theRow again before deleting it.");
                    return false;
                case RowStatus.NotFound:
                    ShowMsg("The Row cannot be found.");
                    return false;
                default:
                    break;
            }

            dgResult = ShowMsg("Confirm Delete. Press 'Ok' to Delete or 'Cancel' to abort.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dgResult == DialogResult.Cancel) return false;

            if (!BuildWhereClause(currDataRow, out string whereClause)) return false;
            string sql = string.Format("Delete from \"{0}\" {1}", TableName, whereClause);

            InitSettings();
            long recsupdated = DataAccess.ExecuteNonQuery(DatabaseName, sql, out SQLiteErrorCode returnCode);
            ShowSettings(recsupdated, returnCode);

            if (returnCode != SQLiteErrorCode.Ok)
            {
                MessageBox.Show(string.Format(MB_DELETE_ERROR, returnCode.ToString(), DataAccess.LastError));
            }
            toolStripLabelStatus.Text = string.Format("{0} Record(s) deleted.", recsupdated);
            RowCount -= recsupdated;
            return true;
        }
        #endregion

        #region helpers

        readonly string[] boolvalues = new string[] { "0", "1", "true", "false" };
        /// <summary>
        /// Validate entered data and convert it to SQLite acceptablr value
        /// </summary>
        /// <param name="t"></param>
        /// <param name="szValue"></param>
        /// <returns></returns>
        protected bool ValidateData(TextBox t, out string szValue)
        {
            szValue = "";
            ColData cd = (ColData)t.Tag;
            switch (cd.coltype.ToLower())
            {
                case "boolean":
                    if (boolvalues.Contains(t.Text.ToLower().Trim()))
                    {
                        szValue = t.Text;
                        return true;
                    }
                    ShowMsg("Please enter 'true' or 'false'");
                    t.Focus();
                    break;

                case "datetime":
                case "date":
                case "timestamp":
                    if (DateTime.TryParse(t.Text, out DateTime newdate))
                    {
                        szValue = newdate.ToString("yyyy-MM-dd HH:mm:ss");
                        return true;
                    }
                    ShowMsg("Please enter a valid Date");
                    t.Focus();
                    break;

                case "time":
                    if (DateTime.TryParse(t.Text, out DateTime newtime))
                    {
                        szValue = newtime.ToString("HH:mm:ss");
                        return true;
                    }
                    ShowMsg("Please enter a valid Time");
                    t.Focus();
                    break;

                case "bigint":
                case "int8":
                    if (Int64.TryParse(t.Text, out _))
                    {
                        szValue = t.Text;
                        return true;
                    }
                    ShowMsg("Please enter a valid Long Integer");
                    t.Focus();
                    break;

                case "integer":
                case "int":
                case "mediumint":
                case "int4":
                    if (int.TryParse(t.Text, out _))
                    {
                        szValue = t.Text;
                        return true;
                    }
                    ShowMsg("Please enter a valid Integer");
                    t.Focus();
                    break;

                case "smallint":
                case "int2":
                    if (Int16.TryParse(t.Text, out _))
                    {
                        szValue = t.Text;
                        return true;
                    }
                    ShowMsg("Please enter a valid Small Integer");
                    t.Focus();
                    break;

                case "tinyint":
                    if (Int16.TryParse(t.Text, out Int16 i))
                    {
                        if (i >= 0 && i <= 255)
                        { szValue = t.Text; return true; }
                    }
                    ShowMsg("Please enter a valid Tiny Integer");
                    t.Focus();
                    break;

                case "unsigned bigint":
                case "unsigned int8":
                    if (UInt64.TryParse(t.Text, out _))
                    {
                        szValue = t.Text;
                        return true;
                    }
                    ShowMsg("Please enter a valid unsigned Long Integer");
                    t.Focus();
                    break;

                case "unsigned integer":
                case "unsigned int":
                case "unsigned mediumint":
                case "unsigned int4":
                    if (UInt32.TryParse(t.Text, out _))
                    {
                        szValue = t.Text;
                        return true;
                    }
                    ShowMsg("Please enter a valid unsigned Integer");
                    t.Focus();
                    break;

                case "unsigned smallint":
                case "unsigned int2":
                    if (UInt16.TryParse(t.Text, out _))
                    {
                        szValue = t.Text;
                        return true;
                    }
                    ShowMsg("Please enter a valid unsigned Small Integer");
                    t.Focus();
                    break;

                case "float":
                case "numeric":
                case "real":
                case "double":
                case "double precision":
                case "single":
                case "single precision":
                    if (double.TryParse(t.Text, out double newdbl))
                    {
                        szValue = newdbl.ToString();
                        return true;
                    }
                    ShowMsg("Please enter a valid value");
                    t.Focus();
                    break;

                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                    if (t.Text.Length <= Convert.ToInt32(cd.collength))
                    {
                        szValue = t.Text;
                        return true;
                    }
                    ShowMsg(string.Format("Length cannot exceed {0} characters.", cd.collength));
                    t.Focus();
                    break;

                case "decimal":
                    if (decimal.TryParse(t.Text, out decimal newdec))
                    {
                        szValue = newdec.ToString();
                        return true;
                    }
                    ShowMsg("Please enter a valid Decimal value");
                    t.Focus();
                    break;

                default:
                    szValue = t.Text;
                    return true;
            }

            return false;

        }
        /// <summary>
        /// determine if the underlying row has been modified or deleted
        /// </summary>
        /// <param name="currDataRow"></param>
        /// <returns></returns>
        protected RowStatus RecordUpdated(out DataRow currDataRow)
        {
            string sql;
            bool noKey = false;
            if (tableHasRowID)
            {
                sql = string.Format("{0} Where \"{1}\" = {2}", BaseSQL, RowIDColName, dt.Rows[0][RowIDColName].ToString());
            }
            else
            if (!string.IsNullOrEmpty(PrimaryKeyName))
            {
                sql = string.Format("{0} Where \"{1}\" = {2}", BaseSQL, PrimaryKeyName, dt.Rows[0][PrimaryKeyName].ToString());
            }
            else
            {
                // Worst case - could lead to problems so do not allow update if the record has changed- require a re-read.
                sql = string.Format("{0} Limit 1 Offset {1}", searchSQL, CurrentRow - 1);
                noKey = true;
            }

            currDataRow = null;

            InitSettings();
            DataTable currdt = DataAccess.ExecuteDataTable(DatabaseName, sql, out SQLiteErrorCode returnCode);
            ShowSettings(currdt == null ? 0 : currdt.Rows.Count, returnCode);

            if (dt.Rows.Count != 1) return RowStatus.NotFound;

            currDataRow = currdt.Rows[0];
            DataRow dr = dt.Rows[0];

            for (int i = 0; i < dr.ItemArray.Count(); i++)
            {
                if (!dr.ItemArray[i].Equals(currDataRow.ItemArray[i])) return noKey ? RowStatus.Inconsistent : RowStatus.Updated;
            }
            return RowStatus.Ok;
        }

        /// <summary>
        /// Build the appropriate Where Clause for the current record.  Most of the time rowid will work fine.  If the
        /// record does not have a rowis, we can use the primary Key.  Worst case if neither exists, use all fields 
        /// in the current record and insure that only 1 row will be deleted.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="WhereClause"></param>
        /// <returns></returns>
        protected bool BuildWhereClause(DataRow dr, out string WhereClause)
        {
            StringBuilder sb = new StringBuilder();
            if (tableHasRowID)
            {
                sb.AppendFormat(" Where {0} = {1}", RowIDColName, dr.ItemArray[RowIdIndex].ToString());
            }
            else
            if (!string.IsNullOrEmpty(PrimaryKeyName))
            {
                sb.AppendFormat(" Where {0} = {1}", PrimaryKeyName, dr.ItemArray[PKIndex].ToString());
            }
            else
            {
                StringBuilder sbWhere = new StringBuilder();
                sbWhere.Append("Where ");
                int i;
                for (i = 0; i < dr.Table.Columns.Count - 1; i++)
                {
                    sbWhere.Append("\"").Append(dr.Table.Columns[i].ColumnName).AppendFormat("\" = \"{0}\"", dr[i].ToString());
                    sbWhere.Append(" And ");
                }
                sbWhere.Append("\"").Append(dr.Table.Columns[dr.Table.Columns.Count - 1].ColumnName).AppendFormat("\" = \"{0}\"", dr[i].ToString());
                var ucount = DataAccess.ExecuteScalar(DatabaseName, string.Format("Select Count(*) From \"{0}\" {1}", TableName, sbWhere.ToString()), out SQLiteErrorCode returnCode);
                if (Convert.ToInt32(ucount) != 1)
                {
                    ShowMsg(ERR_MULTIUPDATE);
                    WhereClause = string.Empty;
                    return false;
                }
                sb.AppendFormat(" {0}", sbWhere.ToString());
            }
            WhereClause = sb.ToString();
            return true;
        }

        /// <summary>
        /// Initialize Connection Property Settings before DB access
        /// </summary>
        protected void InitSettings()
        {
            ConnProps.connSettings.ExecStart = DateTime.Now.ToString();
            startclock = Timers.QueryPerformanceCounter();
        }

        /// <summary>
        /// Finalize and display Connection Property Settings after DB access is complete.
        /// </summary>
        protected void ShowSettings(long rowcount, SQLiteErrorCode rc)
        {
            ConnProps.connSettings.ExecEnd = DateTime.Now.ToString();
            ConnProps.connSettings.ElapsedTime = Timers.DisplayTime(Timers.QueryLapsedTime(startclock)); ;
            ConnProps.connSettings.LastSqlStatus = rc.ToString();
            ConnProps.connSettings.RowsAffected = rowcount.ToString();
            m.LoadConnectionProperties();

        }

        /// <summary>
        /// Find the textbox controlrepresenting a specific column.
        /// </summary>
        /// <param name="tbName">Name of the TextBox to locate.</param>
        /// <returns>Null or the targetted TextBox</returns>
        protected TextBox FindTextBox(string tbName)
        {
            return (TextBox)panelBody.Controls[tbName];            
        }
        #endregion
        #endregion

        #region Navigator Handlers   
        /// <summary>
        /// Move to the first row in the table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonMoveFirst_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (UpdateRow()) LoadRecord(0);
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Move to the previous row in the table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonMovePrevious_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (UpdateRow())
            {
                if (CurrentRow > 1) CurrentRow--;
                LoadRecord(CurrentRow);
            }
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Move to the next row in the table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonMoveNext_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (UpdateRow())
            {
                if (CurrentRow < RowCount) CurrentRow++;
                LoadRecord(CurrentRow);
            }
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Move to the last row in the table,
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonMoveLast_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (UpdateRow()) LoadRecord(RowCount);
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Insert a new row into the table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonInsert_Click(object sender, EventArgs e)
        {
            if (!UpdateRow()) return;
            for (int i = 0; i <= panelBody.Controls.Count; i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                if (t == null) break;
                t.Text = string.Empty;
            }
            AddingRow = true;
        }

        private void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            if (AddingRow) AddingRow = false;
            LoadRecord(CurrentRow);
        }

        /// <summary>
        /// Save the vurrent changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonCommit_Click(object sender, EventArgs e)
        {
            if (UpdateRow()) LoadRecord(CurrentRow);
        }

        /// <summary>
        /// Delete the current row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            if (DeleteRow()) LoadRecord(CurrentRow - 1);
        }

        /// <summary>
        /// When the focus leaves the CurrentItem box move to the record entered in the box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBoxCurrentItem_Leave(object sender, EventArgs e)
        {
            // TryParse error check is unnecessary as the control requires numeric input
            // but leave it here for future consideration
            if (int.TryParse(toolStripTextBoxCurrentItem.Text, out int position))
            {
                if (position > 0 && position <= RowCount)
                {
                    if (!UpdateRow()) return;
                    LoadRecord(position );
                    return;
                }
            }
            ShowMsg(string.Format("Please enter a number between 1 and {0}", RowCount.ToString()));
            toolStripTextBoxCurrentItem.Text = (CurrentRow).ToString();
        }

        /// <summary>
        /// Detect an 'Enter' keypress and move to the record in the CurrentItem box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripTextBoxCurrentItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                toolStripTextBoxCurrentItem_Leave(sender, e);
            }
        }
        #endregion

    }
}
