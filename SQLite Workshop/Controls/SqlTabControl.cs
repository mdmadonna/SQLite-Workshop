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
using System.Runtime.InteropServices;

namespace SQLiteWorkshop
{
    internal partial class SqlTabControl : UserControl
    {
        private string _databasename;

        internal SqlTabControl(string DBName)
        {
            _databasename = DBName;
            InitializeComponent();
            txtSqlStatement.ScrollBars = RichTextBoxScrollBars.Both;
            toolStripRowCount.Alignment = ToolStripItemAlignment.Right;
            toolStripRowCount.Text = string.Empty;
            toolStripClock.Alignment = ToolStripItemAlignment.Right;
            toolStripClock.Text = string.Empty;
            toolStripRowCount.Text = string.Empty; toolStripResult.Text = string.Empty;
            this.Dock = DockStyle.Fill;
            if (Int32.TryParse(MainForm.cfg.appsetting(Config.CFG_HSPLITP), out int parm)) hSplitter.SplitPosition = parm;
        }

        internal string DatabaseName {
            get { return _databasename; }
            set { _databasename = value; }
        }

        internal string SqlStatement
        {
            get { return txtSqlStatement.Text; }
            set { txtSqlStatement.Text = value; }
        }

        private void SqlTabControl_Leave(object sender, EventArgs e)
        {
            MainForm.cfg.SetSetting(Config.CFG_HSPLITP, hSplitter.SplitPosition.ToString());
        }
        
        internal bool Execute()
        {
            DataTable dt;
            string sql = GetSQL();
            toolStripResult.Text = string.Empty;
            toolStripRowCount.Text = string.Empty;
            toolStripClock.Text = string.Empty;

            Int64 startclock = Timers.QueryPerformanceCounter();
            try
            {
                MainForm.mInstance.Cursor = Cursors.WaitCursor;
                if (sql.ToLower().StartsWith("select") || sql.ToLower().StartsWith("pragma"))
                {
                    dt = DataAccess.ExecuteDataTable(_databasename, sql, out SQLiteErrorCode returnCode);
                    if (returnCode == SQLiteErrorCode.Ok)
                    {
                        //Populate DataGridView
                        gvResults.DataError += GvResults_DataError;
                        gvResults.DataSource = dt;
                        gvResults.Refresh();
                        gvResults.Visible = true;
                        txtSqlResults.Visible = false;
                        toolStripRowCount.Text = string.Format("{0} rows", dt.Rows.Count.ToString());
                        toolStripResult.Text = Common.OK_QUERY;
                    }
                    else
                    {
                        gvResults.Visible = false;
                        txtSqlResults.Visible = true;
                        txtSqlResults.Text = string.Format(Common.ERR_SQL, DataAccess.LastError, returnCode.ToString());
                        toolStripResult.Text = Common.ERR_QUERY;
                    }
                }
                else
                {
                    gvResults.Visible = false;
                    txtSqlResults.Visible = true;

                    int count = DataAccess.ExecuteNonQuery(_databasename, sql, out SQLiteErrorCode returnCode);
                    if (returnCode != SQLiteErrorCode.Ok)
                    {
                        txtSqlResults.Text = string.Format(Common.ERR_SQL, DataAccess.LastError, returnCode.ToString());
                        toolStripResult.Text = Common.ERR_SQLERR;
                    }
                    else
                    {
                        txtSqlResults.Text = string.Format(Common.OK_RECORDSAFFECTED, count.ToString());
                        toolStripResult.Text = Common.OK_SQL;
                    }
                }
            }
            catch (Exception ex)
            {
                gvResults.Visible = false;
                txtSqlResults.Visible = true;
                txtSqlResults.Text = ex.Message;
                toolStripResult.Text = Common.ERR_SQLERR;
            }
            finally { MainForm.mInstance.Cursor = Cursors.Default; }
            toolStripClock.Text = Timers.DisplayTime(Timers.QueryLapsedTime(startclock));

            return true;
        }

        /// <summary>
        /// Execute an Explain or Explain Query Plan on the SQL Statement and show the results.
        /// </summary>
        /// <param name="ExplainPlan">true- Execute 'Explain Query Plan', false - Execute 'Explain'</param>
        internal void Explain(bool ExplainPlan)
        {
            string sql = GetSQL();
            if (string.IsNullOrEmpty(sql)) return;

            toolStripResult.Text = string.Empty;
            toolStripRowCount.Text = string.Empty;
            toolStripClock.Text = string.Empty;

            Int64 startclock = Timers.QueryPerformanceCounter();
            sql = ExplainPlan ? string.Format("Explain Query Plan {0}", sql) : string.Format("Explain {0}", sql);
            DataTable dt = DataAccess.ExecuteDataTable(_databasename, sql, out SQLiteErrorCode returnCode);
            if (returnCode == SQLiteErrorCode.Ok)
            {
                gvResults.DataSource = dt;
                gvResults.Refresh();
                gvResults.Visible = true;
                txtSqlResults.Visible = false;
                toolStripResult.Text = Common.OK_EXPLAIN;
            }
            else
            {
                gvResults.Visible = false;
                txtSqlResults.Visible = true;
                txtSqlResults.Text = string.Format(Common.ERR_EXPLAIN, DataAccess.LastError, returnCode.ToString());
                toolStripResult.Text = Common.ERR_EXPLAINERR;
            }
            toolStripClock.Text = Timers.DisplayTime(Timers.QueryLapsedTime(startclock));
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

            int iCursor = txtSqlStatement.SelectionStart;
            RichTextBox rtb = new RichTextBox();
            rtb.Rtf = txtSqlStatement.Rtf;

            rtb.Select(0, rtb.Text.Length - 1);
            rtb.SelectionColor = rtb.ForeColor;

            MarkComments(rtb);

            foreach (string keyword in Common.keywords)
            {
                MarkKeyword(rtb, keyword, Color.Blue, 0);
            }

            ignoreChange = true;
            txtSqlStatement.Rtf = rtb.Rtf;
            ignoreChange = false;

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
                pos = pos + rtb.SelectionLength;
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
