using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    public partial class DBAnalyze : Form
    {

        internal string DatabaseLocation { get; set; }
        internal string TableName { get; set; }

        protected DataTable dtTableList;
        protected DataTable dtObjectList;
        SQLiteErrorCode returnCode;
        WaitDialog wd;
        Analyzer analyzer;

        public DBAnalyze(string DBLocation, string Table = "All")
        {
            TableName = Table;
            DatabaseLocation = DBLocation;
            InitializeComponent();
        }

        private void DBAnalyze_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = "Database Analysis";
            AnalyzeDB();
        }

        private bool ShowWarning()
        {
            ShowMsg sm = new ShowMsg();
            sm.Message = "WARNING!!!.  Depending on the size of your database, the Analyzer may take several minutes to several hours to execute.  Additionally, it will create a table in your database containing key statistics related to the data in your database.\r\n\r\nPress 'Ok' to continue or 'Cancel' to exit.";
            sm.ShowDialog();
            if (sm.DoNotShow) MainForm.cfg.SetSetting(Config.CFG_ANALYZEWARN, "true");
            return sm.Result == DialogResult.Cancel ? false : true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!LoadStats()) return;
            AnalyzeDB();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //Create an instance of our printer class
            SQPrinter p = new SQPrinter();

            //Set the TextToPrint property
            p.TextToPrint = richTextBoxReport.Text;

            PrintDialog pd = new PrintDialog();
            pd.AllowPrintToFile = true;
            pd.AllowSomePages = true;
            pd.Document = p;
            pd.AllowCurrentPage = true;
            pd.AllowSelection = true;
            DialogResult dgr = pd.ShowDialog();
            if (dgr == DialogResult.OK)
            {
                p.PrinterSettings = pd.PrinterSettings;
                p.PrinterFont = new Font("Courier", 10);
                p.PrinterFont = richTextBoxReport.Font;
                p.Print();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected bool LoadStats()
        {
            bool bNoWarning = false;
            bool.TryParse(MainForm.cfg.appsetting(Config.CFG_ANALYZEWARN), out bNoWarning);
            if (!bNoWarning)
            {
                if (!ShowWarning()) { this.Close(); return false; }
            }
            wd = new WaitDialog();
            analyzer = new Analyzer(DatabaseLocation);
            analyzer.LoadStatsReport += StatsReport;
            wd.minObjects = 0;
            wd.maxObjects = analyzer.ObjectCount;
            wd.CancelReport += CancelReport;
            wd.Show();
            wd.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            analyzer.Start();
            analyzer.LoadStatsReport -= StatsReport;
            wd.CancelReport -= CancelReport;
            wd.Close();
            if (analyzer.Cancel) return false;
            return true;
        }

        internal void StatsReport(object sender , LoadStatsEventArgs e)
        {
            wd.ReportProgress(e.CurrentObject, e.LoadComplete);
            Application.DoEvents();
        }

        internal void CancelReport(object sender, CancelEventArgs e)
        {
            if (e.cancel) analyzer.Cancel = true;
            Application.DoEvents();
        }


        protected void AnalyzeDB()
        {

            if (!StatsFileExists()) if (!LoadStats()) return;
            richTextBoxReport.Text = string.Empty;

            string sql = string.Format("Select Distinct tblname From {0}", Common.StatsTable);
            dtTableList = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);
            sql = string.Format("Select Distinct tblname, name From {0}", Common.StatsTable);
            dtObjectList = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);

            PrintUtilization();
            PrintTablePageCounts();
            PrintObjectPageCounts();
            PrintAllObjectsSummary();
            PrintAllTablesSummary();
            PrintAllIndexesSummary();
            PrintTableDetails();
            PrintDefinitions();

        }

        long pageSize;
        long totalPages;
        Single sinTotalPages;
        long tableCount;
        long freelistCount;

        protected void PrintUtilization()
        {
            string sql = "SELECT count(*) FROM sqlite_master WHERE type = \"table\"";
            tableCount = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            tableCount++;
            sql = "SELECT count(*) FROM sqlite_master WHERE type = \"index\"";
            long indexCount = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            sql = "SELECT count(*) FROM sqlite_master WHERE name LIKE \"sqlite_autoindex%\"";
            long autoIndexCount = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            sql = "Pragma page_size";
            pageSize = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            sql = "Pragma page_count";
            totalPages = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            sinTotalPages = Convert.ToSingle(totalPages);
            sql = "Pragma freelist_count";
            freelistCount = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);

            sql = string.Format("Select min(sdate) from \"{0}\"", Common.StatsTable);
            var obj = DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            string reportDate = obj.ToString();

            FileInfo fi = new FileInfo(DatabaseLocation);
            long fileSize = fi.Exists ? fi.Length : 0;
            DataTable dt = FetchSummaryData();
            long inusePages = Convert.ToInt64(dt.Rows[0]["leaf_pages"]) + Convert.ToInt64(dt.Rows[0]["int_pages"]) + Convert.ToInt64(dt.Rows[0]["ovfl_pages"]);
            long payload = Convert.ToInt64(dt.Rows[0]["payload"]);

            richTextBoxReport.AppendText(string.Format("*** Disk-Space Utilization Report For {0}\r\n", DatabaseLocation));
            richTextBoxReport.AppendText(string.Format("*** Statistics as of {0}\r\n\r\n", reportDate));
            richTextBoxReport.AppendText(BL("Page size in bytes", pageSize));
            richTextBoxReport.AppendText(BL("Pages in the whole file(measured)", totalPages));
            richTextBoxReport.AppendText(BL("Pages in the whole file(calculated)", 0));
            richTextBoxReport.AppendText(BL("Pages that store data", inusePages, inusePages / sinTotalPages));
            richTextBoxReport.AppendText(BL("Pages on the freelist(per header)", freelistCount, freelistCount / sinTotalPages));
            richTextBoxReport.AppendText(BL("Pages on the freelist(calculated)", 0, 0));
            richTextBoxReport.AppendText(BL("Pages of auto-vacuum overhead", 0, 0));
            richTextBoxReport.AppendText(BL("Number of tables in the database", tableCount));
            richTextBoxReport.AppendText(BL("Number of indices", indexCount));
            richTextBoxReport.AppendText(BL("Number of defined indices", indexCount - autoIndexCount));
            richTextBoxReport.AppendText(BL("Number of implied indices", autoIndexCount));
            richTextBoxReport.AppendText(BL("Size of the file in bytes", fileSize));
            richTextBoxReport.AppendText(BL("Bytes of user payload stored", payload, payload / Convert.ToSingle(fileSize)));
            richTextBoxReport.AppendText("\r\n\r\n");

        }
        protected void PrintTablePageCounts()
        {
            richTextBoxReport.AppendText(MakeCaption("*** Page counts for all tables with their indices "));
            richTextBoxReport.AppendText("\r\n\r\n");

            string sql = string.Format("Select * From (Select tblname, sum(leaf_pages) + sum(int_pages) + sum(ovfl_pages) AS totpages From {0} Group By tblname) Order By totpages Desc", Common.StatsTable);
            DataTable dt = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                long pageCount = Convert.ToInt64(dr["totpages"]);
                richTextBoxReport.AppendText(BL(dr["tblname"].ToString(), pageCount, pageCount / sinTotalPages));
            }
            richTextBoxReport.AppendText("\r\n\r\n");
        }

        protected void PrintObjectPageCounts()
        {
            
            richTextBoxReport.AppendText(MakeCaption("*** Page counts for all tables and indices separately "));
            richTextBoxReport.AppendText("\r\n\r\n");
            string sql = string.Format("Select * From (Select name, sum(leaf_pages) + sum(int_pages) + sum(ovfl_pages) AS totpages From {0} Group By name) Order By totpages Desc", Common.StatsTable);
            DataTable dt = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);
            foreach (DataRow dr in dt.Rows)
            {
                long pageCount = Convert.ToInt64(dr["totpages"]);
                richTextBoxReport.AppendText(BL(dr["name"].ToString(), pageCount, pageCount / sinTotalPages));
            }
            richTextBoxReport.AppendText("\r\n\r\n");
        }
        protected void PrintAllObjectsSummary()
        {
            DataTable dt = FetchSummaryData();
            PrintDetails("*** All tables and indices ", dt);
        }
           
        protected void PrintAllTablesSummary()
        {
            string where = "Where isindex = 0";
            DataTable dt = FetchSummaryData(where);
            PrintDetails("*** All tables ", dt);
        }
        protected void PrintAllIndexesSummary()
        {
            string where = "Where isindex = 1";
            DataTable dt = FetchSummaryData(where);
            PrintDetails("*** All indices ", dt);
        }
        protected void PrintTableDetails()
        {
            string where;
            DataTable dt;

            foreach (DataRow dr in dtTableList.Rows)
            {
                where = string.Format("Where tblname = \"{0}\"", dr["tblname"].ToString());
                dt = FetchSummaryData(where);
                PrintDetails(string.Format("*** Table {0} and all its indices ", dr["tblname"].ToString()), dt);
                where = string.Format("Where name = \"{0}\"", dr["tblname"].ToString());
                dt = FetchSummaryData(where);
                PrintDetails(string.Format("*** Table {0} w/o any indices ", dr["tblname"].ToString()), dt);
                foreach (DataRow drIndex in dtObjectList.Rows)
                {
                    if (drIndex["tblname"].Equals(dr["tblname"]) && !drIndex["name"].Equals(drIndex["tblname"]))
                    {
                        where = string.Format("Where name = \"{0}\"", drIndex["name"].ToString());
                        dt = FetchSummaryData(where);
                        PrintDetails(string.Format("*** Index {0} of table {1} ", drIndex["name"].ToString(), dr["tblname"].ToString()), dt);
                    }
                }
            }
            
        }
        protected void PrintDefinitions()
        {
            try
            {
                StreamReader sr = new StreamReader("Definitions.txt");
                richTextBoxReport.AppendText(sr.ReadToEnd());
                sr.Close();
            }
            catch { }
        }

        protected void PrintDetails(string Caption, DataTable dt)
        {
            DataRow dr = dt.Rows[0];
            long numEntries = Convert.ToInt64(dr["nentry"]);
            long numLeafEntries = Convert.ToInt64(dr["nleaf"]);
            long payload = Convert.ToInt64(dr["payload"]);
            long ovfl_payload = Convert.ToInt64(dr["ovfl_payload"]);
            long mx_payload = Convert.ToInt64(dr["mx_payload"]);
            long ovfl_cnt = Convert.ToInt64(dr["ovfl_cnt"]);
            long leaf_pages = Convert.ToInt64(dr["leaf_pages"]);
            long int_pages = Convert.ToInt64(dr["int_pages"]);
            long ovfl_pages = Convert.ToInt64(dr["ovfl_pages"]);
            long leaf_unused = Convert.ToInt64(dr["leaf_unused"]);
            long int_unused = Convert.ToInt64(dr["int_unused"]);
            long ovfl_unused = Convert.ToInt64(dr["ovfl_unused"]);
            long gap_cnt = Convert.ToInt64(dr["gap_cnt"]);
            long compressed_size = Convert.ToInt64(dr["compressed_size"]);

            long totPages = leaf_pages + int_pages + ovfl_pages;
            long storage = totPages * pageSize;
            long totUnused = leaf_unused + int_unused + ovfl_unused;
            Single fanout = (leaf_pages + int_pages - tableCount) / Convert.ToSingle(int_pages);



            richTextBoxReport.AppendText(MakeCaption(Caption));
            richTextBoxReport.AppendText("\r\n\r\n");
            richTextBoxReport.AppendText(BLPercent("Percentage of total database", totPages / sinTotalPages));
            richTextBoxReport.AppendText(BL("Number of entries", numEntries));
            richTextBoxReport.AppendText(BL("Bytes of storage consumed", storage));
            richTextBoxReport.AppendText(BL("Bytes of payload", payload, payload / Convert.ToSingle(storage)));
            richTextBoxReport.AppendText(BLSingle("Average payload per entry", payload / Convert.ToSingle(numEntries)));
            richTextBoxReport.AppendText(BLSingle("Average unused bytes per entry", totUnused / Convert.ToSingle(numLeafEntries)));
            richTextBoxReport.AppendText(BLSingle("Average fanout", fanout));
            richTextBoxReport.AppendText(BL("Maximum payload per entry", mx_payload));
            richTextBoxReport.AppendText(BL("Entries that use overflow", ovfl_cnt, ovfl_cnt / Convert.ToSingle(numEntries)));
            richTextBoxReport.AppendText(BL("Index pages used", int_pages));
            richTextBoxReport.AppendText(BL("Primary pages used", leaf_pages));
            richTextBoxReport.AppendText(BL("Overflow pages used", ovfl_pages));
            richTextBoxReport.AppendText(BL("Total pages used", totPages));
            richTextBoxReport.AppendText(BL("Unused bytes on index pages", int_unused, int_pages == 0 ? 0 : int_unused / Convert.ToSingle(int_pages * pageSize)));
            richTextBoxReport.AppendText(BL("Unused bytes on primary pages", leaf_unused, leaf_pages == 0 ? 0 : leaf_unused / Convert.ToSingle(leaf_pages * pageSize)));
            richTextBoxReport.AppendText(BL("Unused bytes on overflow pages", ovfl_unused, ovfl_pages == 0 ? 0 : ovfl_unused / Convert.ToSingle(ovfl_pages * pageSize)));
            richTextBoxReport.AppendText(BL("Unused bytes on all pages", totUnused, totUnused / Convert.ToSingle(totPages * pageSize)));

            richTextBoxReport.AppendText("\r\n\r\n");
        }

        protected string MakeCaption(string caption)
        {
            return caption.PadRight(80, '*');
        }

        int linewidth = 50;
        /// <summary>
        /// Build a report line
        /// </summary>
        /// <param name="caption">Text of the report line</param>
        /// <param name="value">Value to print</param>
        /// <param name="pct">Optional percentage to print</param>
        protected string BL(string caption, long value, Single pct = -1)
        {
            return string.Format("{0} {1}{2}\r\n", caption.PadRight(linewidth, '.'), value.ToString().PadRight(15, ' '), pct == -1 ? string.Empty : string.Format("{0:F}", pct.ToString("#0.##%")));
        }

        protected string BLPercent(string caption, Single pct = -1)
        {
            return string.Format("{0} {1}\r\n", caption.PadRight(linewidth, '.'), pct == -1 ? string.Empty : string.Format("{0:F}", pct.ToString("#0.##%")));
        }

        protected string BLSingle(string caption, Single value)
        {
            return string.Format("{0} {1}\r\n", caption.PadRight(linewidth, '.'), string.Format("{0:F}", value.ToString("#0.##")));
        }

        protected DataTable FetchSummaryData(string objName = null)
        {
            string sql = FetchSql(objName);
            DataTable dt = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);
            return dt;
        }

        protected string FetchSql(string whereClause)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            sb.Append("(sum(nentry)) AS nentry, ");
            sb.Append("(sum(leaf_entries)) AS nleaf, ");
            sb.Append("(sum(payload)) AS payload, ");
            sb.Append("(sum(ovfl_payload)) AS ovfl_payload, ");
            sb.Append("(mx_payload) AS mx_payload, ");
            sb.Append("(sum(ovfl_cnt)) as ovfl_cnt, ");
            sb.Append("(sum(leaf_pages)) AS leaf_pages, ");
            sb.Append("(sum(int_pages)) AS int_pages, ");
            sb.Append("(sum(ovfl_pages)) AS ovfl_pages, ");
            sb.Append("(sum(leaf_unused)) AS leaf_unused, ");
            sb.Append("(sum(int_unused)) AS int_unused, ");
            sb.Append("(sum(ovfl_unused)) AS ovfl_unused, ");
            sb.Append("(sum(gap_cnt)) AS gap_cnt, ");
            sb.Append("(sum(compressed_size)) AS compressed_size ");
            sb.AppendFormat("FROM \"{0}\" {1}", Common.StatsTable, whereClause);
            return sb.ToString();
        }
        protected bool StatsFileExists()
        {
            SQLiteErrorCode returnCode;

            string sql = string.Format("Select count(*) From sqlite_master Where tbl_name = \"{0}\"", Common.StatsTable);
            long count = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            if (count == 0) return false;

            sql = string.Format("Select count(*) From \"{0}\"", Common.StatsTable);
            count = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            return count == 0 ? false : true; ;
        }
 

        #region ControlBox Handlers
        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ControlBox_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.White;
        }

        private void ControlBox_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackColor = SystemColors.InactiveCaption;
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }
        private void ControlBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.Wheat;
            ((PictureBox)sender).BorderStyle = BorderStyle.Fixed3D;
        }

        private void ControlBox_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).BackColor = SystemColors.InactiveCaption;
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }
        #endregion

        #region Form Management

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion


    }
}
