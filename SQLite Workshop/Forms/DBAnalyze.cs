﻿using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class DBAnalyze : Form
    {

        protected enum ReportType
        {
            consolidated,
            table,
            index
        }
        
        ToolTip toolTip;
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
            toolTip = new ToolTip();
            HouseKeeping(this, "Database Analysis");
            AnalyzeDB();
        }

        private void DBAnalyze_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private bool ShowWarning()
        {
            ShowMsg sm = new ShowMsg
            {
                Message = "WARNING!!!.  Depending on the size of your database, the Analyzer may take several minutes to several hours to execute.  Additionally, it will create a table in your database containing key statistics related to the data in your database.\r\n\r\nPress 'Ok' to continue or 'Cancel' to exit."
            };
            sm.ShowDialog();
            if (sm.DoNotShow) saveSetting(Config.CFG_ANALYZEWARN, "true");
            return sm.Result != DialogResult.Cancel;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (!LoadStats()) return;
            AnalyzeDB();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //Create an instance of our printer class
            SQPrinter p = new SQPrinter
            {
                //Set the TextToPrint property
                TextToPrint = richTextBoxReport.Text
            };

            PrintDialog pd = new PrintDialog
            {
                AllowPrintToFile = true,
                AllowSomePages = true,
                Document = p,
                AllowCurrentPage = true,
                AllowSelection = true
            };
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
            bool.TryParse(appSetting(Config.CFG_ANALYZEWARN), out bool bNoWarning);
            if (!bNoWarning)
            {
                if (!ShowWarning()) { this.Close(); return false; }
            }

            string sql = "Select Count(*) From sqlite_master Where type = \"table\"";
            long tablecount = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);

            if (tablecount == 0)
            {
                ShowMsg("This database does not contain any tables.\r\nAnalysis terminated.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return false;
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
            try
            {
                if (!StatsFileExists()) if (!LoadStats()) return;
                richTextBoxReport.Text = string.Empty;

                string sql = string.Format("Select Distinct tblname From {0}", StatsTable);
                dtTableList = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);
                sql = string.Format("Select Distinct tblname, name From {0}", StatsTable);
                dtObjectList = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);

                if (dtTableList.Rows.Count == 0)
                {
                    ShowMsg("This database does not contain any tables.\r\nAnalysis terminated.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }

                PrintUtilization();
                PrintTablePageCounts();
                PrintObjectPageCounts();
                PrintAllObjectsSummary();
                PrintAllTablesSummary();
                PrintAllIndexesSummary();
                PrintTableDetails();
                PrintDefinitions();
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format(ERR_GENERAL, ex.Message));
                return;
            }

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
            sql = "Pragma auto_vacuum";
            long AutoVacuum = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            long AutoVacOverhead;
            AutoVacOverhead = (totalPages == 1 || AutoVacuum == 0) ? 0 : (long)Math.Ceiling((totalPages - 1) / ((Single)(pageSize / 5) + 1));



            sql = string.Format("Select min(sdate) from \"{0}\"", StatsTable);
            var obj = DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            string reportDate = obj.ToString();

            FileInfo fi = new FileInfo(DatabaseLocation);
            long fileSize = fi.Exists ? fi.Length : 0;
            DataTable dt = FetchSummaryData();
            long leaf_pages = string.IsNullOrEmpty(dt.Rows[0]["leaf_pages"].ToString()) ? 0 : Convert.ToInt64(dt.Rows[0]["leaf_pages"]);
            long int_pages = string.IsNullOrEmpty(dt.Rows[0]["int_pages"].ToString()) ? 0 : Convert.ToInt64(dt.Rows[0]["int_pages"]);
            long ovfl_pages = string.IsNullOrEmpty(dt.Rows[0]["ovfl_pages"].ToString()) ? 0 : Convert.ToInt64(dt.Rows[0]["ovfl_pages"]);
            long payload = string.IsNullOrEmpty(dt.Rows[0]["payload"].ToString()) ? 0 : Convert.ToInt64(dt.Rows[0]["payload"]);

            long inusePages = leaf_pages + int_pages + ovfl_pages;

            richTextBoxReport.AppendText(string.Format("*** Disk-Space Utilization Report For {0}\r\n", DatabaseLocation));
            richTextBoxReport.AppendText(string.Format("*** Statistics as of {0}\r\n\r\n", reportDate));
            richTextBoxReport.AppendText(BL("Page size in bytes", pageSize));
            richTextBoxReport.AppendText(BL("Pages in the whole file(measured)", totalPages));
            richTextBoxReport.AppendText(BL("Pages in the whole file(calculated)", totalPages));
            richTextBoxReport.AppendText(BL("Pages that store data", inusePages, inusePages / sinTotalPages));
            richTextBoxReport.AppendText(BL("Pages on the freelist(per header)", freelistCount, freelistCount / sinTotalPages));
            richTextBoxReport.AppendText(BL("Pages on the freelist(calculated)", freelistCount, freelistCount / sinTotalPages));
            richTextBoxReport.AppendText(BL("Pages of auto-vacuum overhead", AutoVacOverhead, AutoVacOverhead / sinTotalPages ));
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

            string sql = string.Format("Select * From (Select tblname, sum(leaf_pages) + sum(int_pages) + sum(ovfl_pages) AS totpages From {0} Group By tblname) Order By totpages Desc", StatsTable);
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
            string sql = string.Format("Select * From (Select name, sum(leaf_pages) + sum(int_pages) + sum(ovfl_pages) AS totpages From {0} Group By name) Order By totpages Desc", StatsTable);
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

        protected void PrintDetails(string Caption, DataTable dt, ReportType type = ReportType.consolidated)
        {
            DataRow dr = dt.Rows[0];
            long numEntries = string.IsNullOrEmpty(dr["nentry"].ToString()) ? 0 : Convert.ToInt64(dr["nentry"]);
            long numLeafEntries = string.IsNullOrEmpty(dr["nleaf"].ToString()) ? 0 : Convert.ToInt64(dr["nleaf"]);
            long payload = string.IsNullOrEmpty(dr["payload"].ToString()) ? 0 : Convert.ToInt64(dr["payload"]);
            long ovfl_payload = string.IsNullOrEmpty(dr["ovfl_payload"].ToString()) ? 0 : Convert.ToInt64(dr["ovfl_payload"]);
            long mx_payload = string.IsNullOrEmpty(dr["mx_payload"].ToString()) ? 0 : Convert.ToInt64(dr["mx_payload"]);
            long ovfl_cnt = string.IsNullOrEmpty(dr["ovfl_cnt"].ToString()) ? 0 : Convert.ToInt64(dr["ovfl_cnt"]);
            long leaf_pages = string.IsNullOrEmpty(dr["leaf_pages"].ToString()) ? 0 : Convert.ToInt64(dr["leaf_pages"]);
            long int_pages = string.IsNullOrEmpty(dr["int_pages"].ToString()) ? 0 : Convert.ToInt64(dr["int_pages"]);
            long ovfl_pages = string.IsNullOrEmpty(dr["ovfl_pages"].ToString()) ? 0 : Convert.ToInt64(dr["ovfl_pages"]);
            long leaf_unused = string.IsNullOrEmpty(dr["leaf_unused"].ToString()) ? 0 : Convert.ToInt64(dr["leaf_unused"]);
            long int_unused = string.IsNullOrEmpty(dr["int_unused"].ToString()) ? 0 : Convert.ToInt64(dr["int_unused"]);
            long ovfl_unused = string.IsNullOrEmpty(dr["ovfl_unused"].ToString()) ? 0 : Convert.ToInt64(dr["ovfl_unused"]);
            long gap_cnt = string.IsNullOrEmpty(dr["gap_cnt"].ToString()) ? 0 : Convert.ToInt64(dr["gap_cnt"]);
            long compressed_size = string.IsNullOrEmpty(dr["compressed_size"].ToString()) ? 0 : Convert.ToInt64(dr["compressed_size"]);

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
            //need to fix fanout formula
            //richTextBoxReport.AppendText(BLSingle("Average fanout", fanout));
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

        readonly int linewidth = 50;
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
            sb.AppendFormat("FROM \"{0}\" {1}", StatsTable, whereClause);
            return sb.ToString();
        }
        protected bool StatsFileExists()
        {

            string sql = string.Format("Select count(*) From sqlite_master Where tbl_name = \"{0}\"", StatsTable);
            long count = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out SQLiteErrorCode returnCode);
            if (count == 0) return false;

            sql = string.Format("Select count(*) From \"{0}\"", StatsTable);
            count = (long)DataAccess.ExecuteScalar(DatabaseLocation, sql, out returnCode);
            return count != 0;
        }

    }
}
