using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace SQLiteWorkshop
{
    public partial class RecordEditTabControl : UserControl
    {
        string RowIDColName;
        int RowIdIndex;
        string PrimaryKeyName;
        int PKIndex;
        bool tableHasRowID;

        string BaseSQL = string.Empty;
        string searchSQL = string.Empty;
        string searchWhereClause = string.Empty;
        int CurrentRow = 0;
        int RowCount = 0;
        bool AddingRow = false;

        DataTable dt;

        string[] Rowids = new string[] { "rowid", "_rowid_", "OID" };

        protected enum RowStatus
        {
            Ok,
            Updated,
            NotFound,
            Inconsistent
        }

        public string DatabaseName { get; set; }
        public string TableName { get; set; }

        internal RecordEditTabControl(string dbName, string tblname)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            toolStripLabelTotalRecords.Text = string.Empty;
            toolStripLabelStatus.Text = string.Empty;
            DatabaseName = dbName;
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
            SQLiteErrorCode returnCode;

            tableHasRowID = DataAccess.CheckForRowID(DatabaseName, TableName, out RowIDColName, out PrimaryKeyName);

            BaseSQL = string.Format("Select Count(*) From \"{0}\"", TableName);
            searchWhereClause = string.IsNullOrEmpty(richTextWhere.Text) ? string.Empty : string.Format("Where {0}", richTextWhere.Text.Trim());
           
            RowIdIndex = -1;
            searchSQL = string.Format("{0} {1}", BaseSQL, searchWhereClause);
            
            int iRowCount = Convert.ToInt32(DataAccess.ExecuteScalar(DatabaseName, searchSQL, out returnCode));
            if (iRowCount == -1 || returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(string.Format("Error processing SQL.  Please review your WHERE clause.\r\n{0}", DataAccess.LastError));
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
            DataAccess.OpenDB(DatabaseName, ref conn, ref cmd, out returnCode, false);
            SQLiteDataReader dr;
            cmd.CommandText = searchSQL;
            try
            {
                dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Error processing SQL.  Please review your WHERE clause.\r\n{0}", ex.Message));
                searchSQL = BaseSQL;
                searchWhereClause = string.Empty;
                DataAccess.CloseDB(conn);
                RowCount = 0;
                LoadRecord(0);
                return;
            }

            int start = 50;
            int lablen = 100;
            int height = 40;

            // draw a textbox for each column in the row except rowid.
            for (int i = 0; i < dr.FieldCount; i++)
            {
                string colname = dr.GetName(i);
                if (RowIdIndex < 0)  if (tableHasRowID) if (RowIDColName == colname) RowIdIndex = i;
                if (i == RowIdIndex) continue;
                if (colname == PrimaryKeyName) PKIndex = i;

                Label lbl = new Label();
                lbl.Text = string.Format("{0}:", colname);
                lbl.Top = start;
                lbl.Left = 50;
                panelBody.Controls.Add(lbl);

                TextBox txt = new TextBox();
                txt.Name = string.Format("txt{0}", i.ToString().PadLeft(4, '0'));
                txt.Tag = colname;
                txt.Top = start;
                txt.Left = lbl.Left + lablen;
                txt.Width = 300;
                panelBody.Controls.Add(txt);                
                start += height;
            }
            dr.Close();
            DataAccess.CloseDB(conn);
        }

        /// <summary>
        /// Load a record onto the window and enable/disable controls appropriately
        /// </summary>
        /// <param name="RowNum"></param>
        protected void LoadRecord(int RowNum)
        {
            if (RowCount == 0) { InitEmptyNavigator(); return; }

            if (RowNum > RowCount) RowNum = RowCount;
            if (RowNum < 1) RowNum = 1;

            string sql = string.Format("{0} Limit 1 Offset {1}", searchSQL, RowNum - 1);
            dt = DataAccess.ExecuteDataTable(DatabaseName, sql, out SQLiteErrorCode returnCode);

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
            int i = 0;
            ArrayList parms = new ArrayList();
            SQLiteErrorCode returnCode;

            DataRow dr = dt.Rows[0];

            for (i = 0; i < dr.ItemArray.Count(); i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                if (dr[i].ToString() != t.Text)
                {
                    count++;
                    sb.Append(count > 1 ? "," : string.Empty).AppendFormat(" \"{0}\" = ?", t.Tag);
                    parms.Add(t.Text);
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
                    dgResult = Common.ShowMsg("The Row has changed since it was retrieved.  Click 'Yes' to save your changes anyway or click 'No' to discard your change and retrieve current data for this row.", MessageBoxButtons.YesNo);
                    if (dgResult == DialogResult.No)
                    {
                        LoadRecord(CurrentRow);
                        return false;
                    }
                    break;
                case RowStatus.Inconsistent:
                    dgResult = Common.ShowMsg("The Row has changed since it was retrieved.  Please read the Row again before updating it.");
                    return false;
                case RowStatus.NotFound:
                    dgResult = Common.ShowMsg("The Row cannot be found.");
                    return false;
                default:
                    break;
            }

            if (!BuildWhereClause(currDataRow, out string whereClause)) return false;
            sb.AppendFormat(" {0}", whereClause);

            int recsupdated = DataAccess.ExecuteNonQuery(DatabaseName, sb.ToString(), parms, out returnCode);
            toolStripLabelStatus.Text = string.Format("{0} Record(s) updated.", recsupdated.ToString());
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
            int i = 0;
            SQLiteErrorCode returnCode;

            for (i = 0; i <= panelBody.Controls.Count; i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                if (t == null) break;
                if (!string.IsNullOrEmpty(t.Text))
                {
                    count++;
                    sb.Append(count > 1 ? "," : string.Empty).AppendFormat(" \"{0}\"", t.Tag);
                    sbValues.Append(count > 1 ? "," : string.Empty).Append("?");
                    parms.Add(t.Text);
                }
            }
            if (count == 0)
            {
                DialogResult dgResult = Common.ShowMsg("No data to insert. Please enter data to insert or press the 'Undo' button to cancel insert.");
                return false;
            }
            sb.Append(")");
            sbValues.Append(")");

            int recsupdated = DataAccess.ExecuteNonQuery(DatabaseName, string.Format("{0} {1}", sb.ToString(), sbValues.ToString()), parms, out returnCode);
            if (recsupdated == -1 || returnCode != SQLiteErrorCode.Ok)
            {
                DialogResult dgResult = Common.ShowMsg(string.Format(Common.ERR_SQL, DataAccess.LastError, returnCode));
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
                    dgResult = Common.ShowMsg("The Row has changed since it was retrieved.  Click 'Yes' to delete this row or click 'No' to abort.", MessageBoxButtons.YesNo);
                    if (dgResult == DialogResult.No)
                    {
                        LoadRecord(CurrentRow);
                        return false;
                    }
                    break;
                case RowStatus.Inconsistent:
                    dgResult = Common.ShowMsg("The Row has changed since it was retrieved.  Please read theRow again before deleting it.");
                    return false;
                case RowStatus.NotFound:
                    dgResult = Common.ShowMsg("The Row cannot be found.");
                    return false;
                default:
                    break;
            }
           
            dgResult = Common.ShowMsg("Confirm Delete. Press 'Ok' to Delete or 'Cancel' to abort.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dgResult == DialogResult.Cancel) return false;
           
            if (!BuildWhereClause(currDataRow, out string whereClause)) return false;
            string sql = string.Format("Delete from \"{0}\" {1}", TableName, whereClause);
            int recsupdated = DataAccess.ExecuteNonQuery(DatabaseName, sql, out SQLiteErrorCode returnCode);
            toolStripLabelStatus.Text = "1 Record deleted.";
            RowCount--;
            return true;
        }
        #endregion

        #region helpers
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
            DataTable currdt = DataAccess.ExecuteDataTable(DatabaseName, sql, out SQLiteErrorCode returnCode);

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
                    Common.ShowMsg(Common.ERR_MULTIUPDATE);
                    WhereClause = string.Empty;
                    return false;
                }
                sb.AppendFormat(" {0}", sbWhere.ToString());
            }
            WhereClause = sb.ToString();
            return true;
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
            if (!UpdateRow()) return;
            LoadRecord(0);
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
            if (!UpdateRow()) return;
            if (CurrentRow > 1) CurrentRow--;
            LoadRecord(CurrentRow);
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
            if (!UpdateRow()) return;
            if (CurrentRow < RowCount) CurrentRow++;
            LoadRecord(CurrentRow);
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
            if (!UpdateRow()) return;
            LoadRecord(RowCount);
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
            UpdateRow();
            LoadRecord(CurrentRow);
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
            Common.ShowMsg(string.Format("Please enter a number between 1 and {0}", RowCount.ToString()));
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
