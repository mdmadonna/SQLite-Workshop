using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Data.SQLite;
using System.Text;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;

namespace SQLiteWorkshop
{
    internal partial class DBEditorTabControl : MainTabControl
    {

        string RowIDColName;
        string PrimaryKeyName;
        bool tableHasRowID;
        long startclock;
        private long sqlLimit = DEF_ROWEDIT;


        internal string TableName { get; set; }

        internal bool CancelExecution { get; set; }

        internal override string SqlStatement { 
            get { return sqlRichTextBox.Text; }
            set { } 
        }

        internal DBEditorTabControl(string dbName, string tblname)
        {
            InitializeComponent();
            InitializeClass(dbName);
            this.Dock = DockStyle.Fill;
            toolStripStatusLabelMsg.Text = string.Empty;
            toolStripStatusLabelTableName.Text = tblname;
            
            // Reserved for future use
            sqlRichTextBox.Visible = false;

            TableName = tblname;
            cmbLimit.Text = DEF_ROWEDIT.ToString();
        }

        // Reserved for future use
        internal void ToggleSqlPanel()
        {
            panel1.Visible = !panel1.Visible;
        }

        // Reserved for future use
        internal void ExecuteSql()
        {
            
        }

        internal void ProgressEventHandler(object sender, ProgressEventArgs e)
        {
            DataAccess.CancelAction = CancelExecution;
            Application.DoEvents();
        }

        private void dgMain_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();
            var centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void dgMain_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dgMain.BeginEdit(true);
        }

        DataTable dt;

        internal bool Execute()
        {
            bool rtn = InitializeWindow();
            bool.TryParse(appSetting(Config.CFG_ROWEDITWARN), out bool bNoWarning);
            if (!bNoWarning) ShowWarning();
            return rtn;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (cmbLimit.Text.Trim().ToLower() == "all")
            {
                sqlLimit = 0;
            }
            else 
            if (!long.TryParse(cmbLimit.Text.Trim(), out sqlLimit))
            {
                ShowMsg(ERR_INVALIDLIMIT);
                cmbLimit.Focus();
                return;
            }
            InitializeWindow();
        }

        private void ShowWarning()
        {
            ShowMsg sm = new ShowMsg(ShowMsg.ButtonStyle.OK)
            {
                Message = WARN_DBEDITOR
            };
            sm.ShowDialog();
            if (sm.DoNotShow) saveSetting(Config.CFG_ROWEDITWARN, "true");
        }

        /// <summary>
        /// If a Vacuum is executed while this page is displayed, rowids may possibly get 
        /// changed. This routine is called after a Vacuum to reload data.  This should occur
        /// only if the table does not contain a Primary Key, but we will always reload the 
        /// page if rowid exists.
        /// </summary>
        internal void VacuumCompleted()
        {
            if (tableHasRowID) InitializeWindow();
        }

        protected bool InitializeWindow()
        { 
            MainForm.mInstance.Cursor = Cursors.WaitCursor;

            try
            {
                tableHasRowID = DataAccess.CheckForRowID(DatabaseName, TableName, out RowIDColName, out PrimaryKeyName);
                sqlRichTextBox.Text = BuildSelectSql();
                
                InitSettings();
                CancelExecution = false;
                // This is a static event so be sure to unsubscribe
                DataAccess.ProgressReport += ProgressEventHandler;
                dt = DataAccess.ExecuteDataTable(DatabaseName, sqlRichTextBox.Text, out SQLiteErrorCode returnCode);
                DataAccess.ProgressReport -= ProgressEventHandler;
                ShowSettings(dt == null ? 0 : dt.Rows.Count, returnCode);
                
                if (returnCode == SQLiteErrorCode.Interrupt)
                {
                    toolStripStatusLabelMsg.Text = ERR_CANCELLED;
                    ShowMsg(ERR_CANCELLED);
                }

                if (returnCode == SQLiteErrorCode.Error)
                {
                    toolStripStatusLabelMsg.Text = string.Format(ERR_FORMATERRORCOUNT, DataAccess.FormatErrorCount.ToString());
                    ShowMsg(ERR_FORMATERROR);
                }
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Error reading database.\r\n{0}", ex.Message));
                return false;
            }
            finally { MainForm.mInstance.Cursor = Cursors.Default; }

            bindingSource.DataSource = dt;
            dt.RowChanging += DataTableChanging;
            dt.RowChanged += DataTableChanged;
            dt.RowDeleting += DataTableDeleting;

            //Populate DataGridView
            dgMain.DataError += dgMain_DataError;
            dgMain.DataSource = bindingSource;
            dgMain.Refresh();
            //Do not allow edits to the 'rowid' column
            if (tableHasRowID)
            {
                dgMain.Columns[0].ReadOnly = true;
                dgMain.Columns[0].Visible = false;
            }
            dt.AcceptChanges();
            return true;
        }

        /// <summary>
        /// Process errors related to the DataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMain_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string ErrorMessage;
            switch (e.Context)
            {
                case DataGridViewDataErrorContexts.Parsing:
                    ErrorMessage = "Parse Error";
                    break;
                case DataGridViewDataErrorContexts.Formatting:
                    ErrorMessage = "Formatting Error";
                    break;
                case DataGridViewDataErrorContexts.Display:
                    ErrorMessage = "Display Error";
                    break;
                default:
                    ErrorMessage = "General Error";
                    break;

            }
            DataGridView view = (DataGridView)sender;
            //view.Rows[e.RowIndex].ErrorText = ErrorMessage;
            view.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true;
            view.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = string.Format("Edit not allowed due to error: {0}", ErrorMessage);
        }

        internal void DataTableChanged(object sender, DataRowChangeEventArgs e)
        {

            DataRow dr = e.Row;
            switch (dr.RowState)
            {
                case DataRowState.Added:
                    InsertRecord(e.Row, out long id);
                    dr.Table.AcceptChanges();
                    if (tableHasRowID) e.Row[0] = id;
                    break;
                case DataRowState.Modified:
                    if (editCancelled) { e.Row.RejectChanges(); } else { dr.Table.AcceptChanges(); }
                    break;
                case DataRowState.Deleted:
                    break;
                default:
                    break;
            }
        }

        internal void DataTableDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (e.Row.Table.Columns[0].ColumnName == "rowid")
            {
                string sql = string.Format("Delete From \"{0}\" Where rowid = {1}", TableName, e.Row[0].ToString());

                InitSettings();
                long result = DataAccess.ExecuteNonQuery(DatabaseName, sql, out SQLiteErrorCode returnCode);
                ShowSettings(result, returnCode);

                if (returnCode != SQLiteErrorCode.Ok)
                {
                    MessageBox.Show(string.Format("Delete failed: {0}{1}Error Code: {2}", DataAccess.LastError, Environment.NewLine, returnCode.ToString()), APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toolStripStatusLabelMsg.Text = "0 Records Deleted";
                    return;
                }
                e.Row.Table.AcceptChanges();
                toolStripStatusLabelMsg.Text = string.Format("{0} Record(s) Deleted", result);
            }
        }

        bool editCancelled = false;
        internal void DataTableChanging(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Change:
                    editCancelled = false;
                    if (!UpdateRecord(e.Row)) editCancelled = true;
                    break;
                case DataRowAction.Add:
                    break;
                case DataRowAction.Delete:
                    break;
                default:
                    break;
            }
        }

        protected bool UpdateRecord (DataRow dr)
        {

            StringBuilder sb = new StringBuilder();
            StringBuilder sbWhere = new StringBuilder();
            SQLiteErrorCode returnCode;



            sb.Append("Update \"").Append(TableName).Append("\" Set ");
            ArrayList parms = new ArrayList();
            ArrayList wparms = new ArrayList();

            int ColumnsToUpdate = 0;
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                if (dgMain.Columns[i].Visible)
                {
                    if (!dr[i, DataRowVersion.Proposed].Equals(dr[i, DataRowVersion.Current]))
                    {
                        if (ColumnsToUpdate > 0) sb.Append(",");
                        sb.Append("\"").Append(dr.Table.Columns[i].ColumnName).Append("\" = ?");
                        ColumnsToUpdate++;
                        ColumnLayout cl = FindColumnLayout(DatabaseName, TableName, dr.Table.Columns[i].ColumnName);
                        if (!ValidateData(cl.ColumnType, dr[i, DataRowVersion.Proposed].ToString(), out _))
                        {
                            ShowMsg(string.Format("Invalid value entered for Column [{0}]", dr.Table.Columns[i].ColumnName));
                            return false;
                        }
                        parms.Add(dr[i, DataRowVersion.Proposed]);
                    }
                }
            }

            if (ColumnsToUpdate == 0) return true;
            //sb.Append(")");

            //Create where clause
            sbWhere.Clear();
            if (tableHasRowID)
            {
                sbWhere.AppendFormat("Where {0} = ", RowIDColName).Append(dr[0].ToString());
            }
            else if (!string.IsNullOrEmpty(PrimaryKeyName))
            {
                sbWhere.AppendFormat("Where {0} = ", PrimaryKeyName).Append(dr[PrimaryKeyName].ToString());
            }
            else
            {

                sbWhere.Append("Where ");
                for (int i = 0; i < dr.Table.Columns.Count - 1; i++)
                {
                    sbWhere.Append("\"").Append(dr.Table.Columns[i].ColumnName).Append("\" = ?");
                    wparms.Add(dr[i, DataRowVersion.Current]);
                    sbWhere.Append(" And ");
                }
                sbWhere.Append("\"").Append(dr.Table.Columns[dr.Table.Columns.Count - 1].ColumnName).Append("\" = ?");
                wparms.Add(dr[dr.Table.Columns.Count - 1, DataRowVersion.Current]);
                var count = DataAccess.ExecuteScalar(DatabaseName, string.Format("Select Count(*) From \"{0}\" {1}", TableName, sbWhere.ToString()), wparms, out returnCode);
                if (Convert.ToInt32(count) != 1)
                {
                    ShowMsg(ERR_MULTIUPDATE);
                    return false;
                }
            }

            parms.AddRange(wparms);

            InitSettings();
            long result = DataAccess.ExecuteNonQuery(DatabaseName, string.Format("{0} {1}", sb.ToString(), sbWhere.ToString()), parms, out returnCode);
            ShowSettings(result, returnCode);

            if (returnCode != SQLiteErrorCode.Ok)
            {
                MessageBox.Show(string.Format("Update failed: {0}{1}Error Code: {2}", DataAccess.LastError, Environment.NewLine, returnCode.ToString()), APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabelMsg.Text = "0 Records Updated";
                return false;
            }
            toolStripStatusLabelMsg.Text = string.Format("{0} Record(s) Updated", result.ToString());
            return true;
        }
        protected bool InsertRecord(DataRow dr, out long id)
        {
            StringBuilder sbInto = new StringBuilder();
            StringBuilder sbValues = new StringBuilder();
            ArrayList parms = new ArrayList();

            sbInto.Append("Insert Into \"").Append(TableName).Append("\" (");
            sbValues.Append(" Values (");
            int idx = 0;
            if (dr.Table.Columns[0].ColumnName != RowIDColName)
            {
                sbInto.Append("\"").Append(dr.Table.Columns[0].ColumnName).Append("\",");
                sbValues.Append("?,");
                parms.Add(dr[0]);
            }
            idx++;
            for (int i = idx; i < dr.Table.Columns.Count - 1; i++)
            {
                sbInto.Append("\"").Append(dr.Table.Columns[i].ColumnName).Append("\",");
                sbValues.Append("?,");
                parms.Add(dr[i]);
            }
            sbInto.Append("\"").Append(dr.Table.Columns[dr.Table.Columns.Count - 1].ColumnName).Append("\")");
            sbValues.Append("?)");
            parms.Add(dr[dr.Table.Columns.Count - 1]);

            long result = DataAccess.ExecuteNonQuery(DatabaseName, string.Format("{0} {1}", sbInto.ToString(), sbValues.ToString()), parms, out id, out SQLiteErrorCode returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                MessageBox.Show(string.Format("Insert failed: {0}{1}Error Code: {2}", DataAccess.LastError, Environment.NewLine, returnCode.ToString()), APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabelMsg.Text = "0 Records Added";
                return false;
            }
            toolStripStatusLabelMsg.Text = string.Format("{0} Record(s) Added", result.ToString());
            return true;
        }

        protected string BuildSelectSql()
        {

            SchemaDefinition sd = DataAccess.SchemaDefinitions[DatabaseName];
            StringBuilder sb = new StringBuilder();

            var tableItem = sd.Tables[TableName];

            sb.Append(tableHasRowID ? string.Format("Select {0},", RowIDColName) : "Select ");
            bool bComma = false;
            foreach (KeyValuePair<string, ColumnLayout> item in tableItem.Columns)
            {
                if (bComma) { sb.Append(","); } else { bComma = true; }
                sb.Append("\"").Append(item.Key).Append("\"");
            }
            sb.Append(" FROM ").Append(TableName);
            if (sqlLimit > 0) sb.Append(string.Format(" LIMIT {0}", sqlLimit));

            return sb.ToString();
        }

        string _lasterror;

        internal DataTable ExecuteDataTable(string DBLocation, string SqlStatement, out SQLiteErrorCode returnCode)
        {
            returnCode = SQLiteErrorCode.Ok;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            if (!DataAccess.OpenDB(DBLocation, ref conn, ref cmd)) return null;

            cmd.CommandText = SqlStatement;
            DataTable dt = ExecuteDataTable(cmd, out returnCode);

            DataAccess.CloseDB(conn);
            return dt;
        }

        internal DataTable ExecuteDataTable(SQLiteCommand cmd, out SQLiteErrorCode returnCode)
        {

            // Cannot use a dataadapter here because of potential format errors
            DataTable dt = new DataTable();

            SQLiteDataReader dr;

            try { dr = cmd.ExecuteReader(); }
            catch (Exception ex)
            {
                _lasterror = ex.Message;
                returnCode = cmd.Connection.ExtendedResultCode();
                return null;
            }

            for (int i = 0; i < dr.FieldCount; i++)
            {
                dt.Columns.Add(dr.GetName(i), dr.GetFieldType(i));
                //dt.Columns.Add(dr.GetName(i), typeof(string));
            }
            //dt.Columns.AddRange(new DataColumn[dr.FieldCount]);
            try
            {
                while (dr.Read())
                {
                    DataRow dRow = dt.NewRow();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (dr[i].GetType() == typeof(Int64))
                        {
                            if (DBNull.Value.Equals(dr[i])) { dRow[i] = 0; continue; }
                            Int64.TryParse(dr[i].ToString(), out long i64);
                            dRow[i] = i64;
                        }
                        else
                        if (dr[i].GetType() == typeof(string))
                        {
                            dRow[i] = dr[i] == null ? string.Empty : dr[i].ToString();
                        }
                        else
                        if (dr[i].GetType() == typeof(DBNull))
                        {
                            dRow[i] = null;
                        }
                        //dRow[i] = dr[i] == null ? string.Empty : dr[i].ToString();
                    }
                    dt.Rows.Add(dRow);
                }
            }
            catch (Exception ex)
            {
                _lasterror = ex.Message;
            }
            finally { dr.Close(); }
            returnCode = cmd.Connection.ExtendedResultCode();
            return dt;
        }

        private void dgMain_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            string datacolumn = dgMain.Columns[e.ColumnIndex].Name;
            ColumnLayout cl = FindColumnLayout(DatabaseName, TableName, datacolumn);
            if (!ValidateData(cl.ColumnType, dgMain.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out _))
            {
                ShowMsg("Invalid value");
                dgMain.Focus();
                dgMain.CurrentCell = dgMain.Rows[e.RowIndex].Cells[e.ColumnIndex];
                dgMain.BeginEdit(true);
            }
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
    }
}
