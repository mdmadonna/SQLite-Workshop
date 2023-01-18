using System;
using System.Drawing;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;

namespace SQLiteWorkshop
{
    internal partial class SqlTabControl : MainTabControl
    {
        Int64 startclock;

        internal SqlTabControl(string DBName)
        {
            InitializeComponent();
            txtSqlStatement.ScrollBars = RichTextBoxScrollBars.Both;
            toolStripRowCount.Alignment = ToolStripItemAlignment.Right;
            toolStripRowCount.Text = string.Empty;
            toolStripClock.Alignment = ToolStripItemAlignment.Right;
            toolStripClock.Text = string.Empty;
            toolStripRowCount.Text = string.Empty; toolStripResult.Text = string.Empty;
            this.Dock = DockStyle.Fill;
            tabResults.TabPages.Remove(tabErrors);
            if (Int32.TryParse(appSetting(Config.CFG_HSPLITP), out int parm)) hSplitter.SplitPosition = parm;

            InitializeClass(DBName);
        }

        internal bool CancelExecution { get; set; }
        internal string SqlFileName { get; set; }
        internal override string SqlStatement 
        {
            get { return txtSqlStatement.Text; }
            set { txtSqlStatement.Text = value; }
        }

        private void SqlTabControl_Leave(object sender, EventArgs e)
        {
            saveSetting(Config.CFG_HSPLITP, hSplitter.SplitPosition.ToString());
        }
        
        internal void SaveSql(bool saveAs = false)
        {

            Control c = this.Parent;
            string filename;
            if (string.IsNullOrEmpty(SqlFileName) || saveAs)
            {
                filename = ((TabPage)c).Text.Trim();
                filename = GetFile(filename);
                if (string.IsNullOrEmpty(filename)) return;
            }
            else
            { filename = SqlFileName;  }

            if (string.IsNullOrEmpty(SqlFileName) || saveAs)
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists)
                {
                    DialogResult dr = ShowMsg(string.Format("{0} already exists.\r\nDo you want to replace it?", filename), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (dr != DialogResult.Yes) return;
                }
            }

            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.Write(txtSqlStatement.Text);
                sw.Close();
                MainForm.mInstance.WriteStatusStripMessage(string.Format("{0} Saved.", Path.GetFileName(filename)));
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("An error occurred while writing {0}.\r\n{1}", filename, ex.Message));
            }
            SqlFileName = filename;
            ((TabPage)c).Text = string.Format("  {0}          ", Path.GetFileName(filename)); ;
            return;
        }

        protected string GetFile(string filename)
        {
            string path = appSetting(Config.CFG_DFLTSQLDIR);
            if (string.IsNullOrEmpty(path))
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "SQLite Workshop");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                saveSetting(Config.CFG_DFLTSQLDIR, path);
            }

            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "Select Destination File",
                Filter = "All files (*.*)|*.*|Sql Files (*.sql)|*.sql",
                FilterIndex = 2,
                AddExtension = true,
                AutoUpgradeEnabled = true,
                DefaultExt = "sql",
                InitialDirectory = path,
                RestoreDirectory = true,
                ValidateNames = true,
                OverwritePrompt = false,
                FileName = filename
            };

            if (saveFile.ShowDialog() != DialogResult.OK) return string.Empty;
            return saveFile.FileName;
        }

        internal void ProgressEventHandler(object sender, ProgressEventArgs e)
        {
            e.ReturnCode = CancelExecution ? SQLiteProgressReturnCode.Interrupt : SQLiteProgressReturnCode.Continue;
            toolStripClock.Text = Timers.DisplayTime(Timers.QueryLapsedTime(startclock));
            Application.DoEvents();
        }

        internal bool Execute()
        {
            string sql = GetSQL();
            toolStripResult.Text = string.Empty;
            toolStripRowCount.Text = string.Empty;
            toolStripClock.Text = string.Empty;
            CancelExecution = false;

            MainForm.mInstance.Cursor = Cursors.WaitCursor;
            if (tabResults.Controls.Contains(tabErrors)) tabResults.TabPages.Remove(tabErrors);

            ConnProps.connSettings.ExecStart = DateTime.Now.ToString();
            startclock = Timers.QueryPerformanceCounter();

            // This is a static event so be sure to unsubscribe
            DataAccess.ProgressReport += ProgressEventHandler;
            if (IsSelect(sql))
            { ExecuteSelect(sql); }
            else
            { ExecuteNonQuery(sql); }
            DataAccess.ProgressReport -= ProgressEventHandler;

            MainForm.mInstance.Cursor = Cursors.Default;
            toolStripClock.Text = Timers.DisplayTime(Timers.QueryLapsedTime(startclock));
            ConnProps.connSettings.ExecEnd = DateTime.Now.ToString();
            ConnProps.connSettings.ElapsedTime = toolStripClock.Text;
            m.LoadConnectionProperties();

            return true;
        }

        private void ExecuteSelect(string sql)
        {
            DataTable dt;
            try
            {
                dt = DataAccess.ExecuteDataTable(DatabaseName, sql, out SQLiteErrorCode returnCode);
                ConnProps.connSettings.RowsAffected = dt == null ? "0" : dt.Rows.Count.ToString();
                ConnProps.connSettings.LastSqlStatus = returnCode.ToString();
                if (returnCode == SQLiteErrorCode.Ok || (returnCode == SQLiteErrorCode.Error && DataAccess.FormatErrorCount > 0))
                {
                    //Populate DataGridView
                    gvResults.DataError += GvResults_DataError;
                    gvResults.DataSource = dt;
                    gvResults.Refresh();
                    gvResults.Visible = true;
                    txtSqlResults.Visible = false;
                    toolStripRowCount.Text = string.Format("{0} rows", dt.Rows.Count.ToString());
                    if (returnCode == SQLiteErrorCode.Ok)
                    {
                        toolStripResult.Text = OK_QUERY;
                    }
                    else
                    {
                        richTextErrors.Text = string.Format(ERR_SQLWIDE, DataAccess.FormatErrorCount.ToString(), returnCode.ToString(), Math.Min(DataAccess.MAX_ERRORS, DataAccess.FormatErrorCount).ToString(), String.Join("\r\n", DataAccess.FormatErrors.ToArray()));
                        if (!tabResults.Controls.Contains(tabErrors)) tabResults.TabPages.Add(tabErrors);
                        toolStripResult.Text = ERR_QUERY;
                    }
                }
                else
                {
                    gvResults.Visible = false;
                    txtSqlResults.Visible = true;
                    txtSqlResults.Text = string.Format(ERR_SQL, DataAccess.LastError, returnCode);
                }
            }
            catch (Exception ex)
            {
                // Most likely an Sql syntax error
                gvResults.Visible = false;
                txtSqlResults.Visible = true;
                txtSqlResults.Text = ex.Message;
                toolStripResult.Text = ERR_SQLERR;
            }
        }

        private void ExecuteNonQuery(string sql)
        {
            try
            {
                gvResults.Visible = false;
                txtSqlResults.Visible = true;

                long count = DataAccess.ExecuteNonQuery(DatabaseName, sql, out SQLiteErrorCode returnCode);
                ConnProps.connSettings.RowsAffected = count.ToString();
                ConnProps.connSettings.LastSqlStatus = returnCode.ToString();

                if (returnCode != SQLiteErrorCode.Ok)
                {
                    txtSqlResults.Text = string.Format(ERR_SQL, DataAccess.LastError, returnCode.ToString());
                    toolStripResult.Text = ERR_SQLERR;
                }
                else
                {
                    txtSqlResults.Text = string.Format(OK_RECORDSAFFECTED, count.ToString());
                    toolStripResult.Text = OK_SQL;
                }
            }
            catch (Exception ex)
            {
                // Most likely an Sql syntax error
                gvResults.Visible = false;
                txtSqlResults.Visible = true;
                txtSqlResults.Text = ex.Message;
                toolStripResult.Text = ERR_SQLERR;
            }
        }

        /// <summary>
        /// Execute an Explain or Explain Query Plan on the SQL Statement and show the results.
        /// </summary>
        /// <param name="ExplainPlan">true- Execute 'Explain', false - Execute 'Explain Query Plan'</param>
        internal void Explain(bool ExplainPlan)
        {
            string sql = GetSQL();
            if (string.IsNullOrEmpty(sql)) return;

            toolStripResult.Text = string.Empty;
            toolStripRowCount.Text = string.Empty;
            toolStripClock.Text = string.Empty;

            ConnProps.connSettings.ExecStart = DateTime.Now.ToString();
            Int64 startclock = Timers.QueryPerformanceCounter();
            sql = ExplainPlan ? string.Format("Explain {0}", sql) : string.Format("Explain Query Plan {0}", sql);
            DataTable dt = DataAccess.ExecuteDataTable(DatabaseName, sql, out SQLiteErrorCode returnCode);
            ConnProps.connSettings.RowsAffected = dt == null ? "0" : dt.Rows.Count.ToString();
            ConnProps.connSettings.LastSqlStatus = returnCode.ToString();
            if (returnCode == SQLiteErrorCode.Ok)
            {
                gvResults.DataSource = dt;
                gvResults.Refresh();
                gvResults.Visible = true;
                txtSqlResults.Visible = false;
                toolStripResult.Text = OK_EXPLAIN;
            }
            else
            {
                gvResults.Visible = false;
                txtSqlResults.Visible = true;
                txtSqlResults.Text = string.Format(ERR_EXPLAIN, DataAccess.LastError, returnCode.ToString());
                toolStripResult.Text = ERR_EXPLAINERR;
            }
            toolStripClock.Text = Timers.DisplayTime(Timers.QueryLapsedTime(startclock));
            ConnProps.connSettings.ExecEnd = DateTime.Now.ToString();
            ConnProps.connSettings.ElapsedTime = toolStripClock.Text;
            m.LoadConnectionProperties();
            return;
        }

        /// <summary>
        /// Process errors related to the DataGridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GvResults_DataError(object sender, DataGridViewDataErrorEventArgs e)
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
            view.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = string.Format("Unable to view due to error: {0}", ErrorMessage);

        }

        internal bool Execute(string szSQL)
        {
            SqlStatement = szSQL;
            return Execute();
        }

        private string GetSQL()
        {
            if (txtSqlStatement.SelectedText == string.Empty) return txtSqlStatement.Text;
            return txtSqlStatement.SelectedText;
        }

        private void gvResults_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
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


        bool ignoreChange = false;
        private void txtSqlStatement_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChange) return; 
                       
            int firstVisibleChar = txtSqlStatement.GetCharIndexFromPosition(new Point(0, 1));
            int lineIndex = txtSqlStatement.GetLineFromCharIndex(firstVisibleChar);
            //string firstVisibleLine = txtSqlStatement.Lines[lineIndex];
            int iCursor = txtSqlStatement.SelectionStart;

            RichTextBox rtb = new RichTextBox
            {
                Rtf = txtSqlStatement.Rtf
            };

            rtb.Select(0, rtb.Text.Length - 1);
            rtb.SelectionColor = rtb.ForeColor;

            MarkComments(rtb);

            foreach (string keyword in keywords)
            {
                MarkKeyword(rtb, keyword, Color.Blue, 0);
            }

            ignoreChange = true;
            txtSqlStatement.Rtf = rtb.Rtf;
            ignoreChange = false;
            int sStart = txtSqlStatement.Lines.Length > 0 ? txtSqlStatement.Find(txtSqlStatement.Lines[lineIndex], RichTextBoxFinds.NoHighlight) : 0;
            txtSqlStatement.SelectionStart = sStart < 0 ? 0 : sStart;
            txtSqlStatement.ScrollToCaret();

            txtSqlStatement.SelectionStart = iCursor < 0 ? 0 : iCursor;
            txtSqlStatement.SelectionLength = 0;
        }

        private void MarkKeyword(RichTextBox rtb, string word, Color color, int startIndex)
        {
            int pos = rtb.Find(word, 0, rtb.Text.Length, RichTextBoxFinds.WholeWord);
            while (pos >= 0)
            {
                rtb.Select(pos, word.Length);
                if (rtb.SelectionColor.ToArgb().Equals(txtSqlStatement.ForeColor.ToArgb()))
                {
                    rtb.SelectionColor = color;
                    rtb.Select(pos + word.Length, 0);
                    rtb.SelectionColor = txtSqlStatement.ForeColor;
                }
                pos = rtb.Find(word, pos + 1, rtb.Text.Length, RichTextBoxFinds.WholeWord);
            }
        }

        private void MarkComments(RichTextBox rtb)
        {
            int length = rtb.Text.Length;
            int pos = 0;
            const string comment = "--";

            while (pos < length)
            {
                // This isn't exactly correct as it will find -- within quotes.  I'll worry about that later
                pos = rtb.Find(comment, pos, length, RichTextBoxFinds.MatchCase);
                if (pos < 0) break;
                int eol = rtb.Find(Environment.NewLine, pos, length, RichTextBoxFinds.MatchCase);
                if (eol < 0) eol = rtb.Find("\n", pos, length, RichTextBoxFinds.MatchCase);
                if (eol < 0) eol = rtb.Find("\r", pos, length, RichTextBoxFinds.MatchCase);
                rtb.Select(pos, eol < 0 ? length - pos : eol - pos);
                rtb.SelectionColor = Color.Green;
                pos += rtb.SelectionLength;
            }
        }

        private void txtSqlStatement_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtSqlStatement_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
