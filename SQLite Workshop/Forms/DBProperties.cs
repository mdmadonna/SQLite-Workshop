using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    public partial class DBProperties : Form
    {

        private string[] dbPragmaList = new string[]
        {
            "encoding",
            "foreign_key_check",
            "foreign_key_list",
            "freelist_count",
            "page_count",
            "page_size",
            "schema_version",
            "user_version",
        };

        private string[] pragmaList = new string[]
        {
            "application_id",
            "auto_vacuum",
            "automatic_index",
            "busy_timeout",
            "cache_size",
            "cache_spill",
            "case_sensitive_like",
            "cell_size_check",
            "checkpoint_fullfsync",
            "collation_list",
            "compile_options",
            "data_version",
            "database_list",
            "defer_foreign_keys",
            //"encoding",
            //"foreign_key_check",
            //"foreign_key_list",
            "foreign_keys",
            //"freelist_count",
            "fullfsync",
            "function_list",
            "ignore_check_constraints",
            //"incremental_vacuum",
            //"index_info",
            //"index_list",
            //"index_xinfo",
            //"integrity_check",
            "journal_mode",
            "journal_size_limit",
            "legacy_file_format",
            "locking_mode",
            "max_page_count",
            "mmap_size",
            "module_list",
            //"optimize",
            //"page_count",
            //"page_size",
            //"parser_trace",
            "pragma_list",
            "query_only",
            //"quick_check",
            "read_uncommitted",
            "recursive_triggers",
            "reverse_unordered_selects",
            //"schema_version",
            "secure_delete",
            //"shrink_memory",
            "soft_heap_limit",
            //"stats",
            "synchronous",
            //"table_info",
            "temp_store",
            "threads",
            //"user_version",
            //"vdbe_addoptrace",
            //"vdbe_debug",
            //"vdbe_listing",
            //"vdbe_trace",
            "wal_autocheckpoint",
            "wal_checkpoint",
            "writable_schema"
        };
        public DBProperties()
        {
            InitializeComponent();
        }

        private void DBProperties_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = "Database Properties";
            propGridView.AutoGenerateColumns = false;
            LoadOptions();
        }

        internal string DatabaseLocation { get; set; }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        protected void LoadOptions()
        {

            SchemaDefinition sd = DataAccess.GetSchema(DatabaseLocation);

            LoadSpacer ("Database Properties");
            LoadOption( "Database File Name", DatabaseLocation);
            LoadOption( "Database Name", sd.DBName);
            LoadOption( "Size", sd.DBSize.ToString());
            LoadOption( "Creation Date", sd.CreateDate.ToString());
            LoadOption( "Last Update", sd.LastUpDate.ToString());
            //LoadOption( "Last Access", sd.LastAccess.ToString());
            LoadOption( "Number of Tables", sd.Tables.Count.ToString());
            LoadOption("Number of Views", sd.Views.Count.ToString());
            int idxCount = 0;
            foreach (var table in sd.Tables)  { idxCount += table.Value.Indexes.Count; }
            LoadOption("Number of Indexes", idxCount.ToString());

            LoadPragma(dbPragmaList);

            LoadSpacer();
            LoadSpacer("Runtime Properties");

            LoadPragma(pragmaList);

        }

        protected void LoadPragma(string[] pragmas)
        {
            string sql;
            SQLiteErrorCode returnCode;

            foreach (string option in pragmas)
            {
                sql = string.Format("Pragma {0}", option);
                DataTable dt = DataAccess.ExecuteDataTable(DatabaseLocation, sql, out returnCode);

                if (returnCode != SQLiteErrorCode.Ok)
                {
                    LoadOption(option, string.Format("NA: {0} Err: {1}", returnCode.ToString(), DataAccess.LastError));
                    continue;
                }

                if (dt.Rows.Count == 0)
                { LoadOption(option, string.Empty); }
                else if (dt.Rows.Count == 1)
                { LoadOption(option, FormatValue(dt.Rows[0])); }
                else
                { LoadComboOption(option, BuildList(dt)); }
            }
        }

        private string FormatValue(DataRow dr)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(dr.ItemArray[0] == null ? string.Empty : dr.ItemArray[0].ToString());
            for (int i = 1; i < dr.ItemArray.Count(); i++)
            {
                sb.Append("|").Append(dr.ItemArray[i] == null ? string.Empty : dr.ItemArray[0].ToString());
            }
            return sb.ToString();
        }

        private DataGridViewComboBoxCell BuildList(DataTable dt)
        {
            
            DataGridViewComboBoxCell dgc = new DataGridViewComboBoxCell();
            dgc.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.DropDownButton;
            dgc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;

            foreach (DataRow dr in dt.Rows)
            {
                dgc.Items.Add(FormatValue(dr));
            }
            return dgc;
        }
        protected void LoadOption(string option, string value)
        { 
            if (option == "cache_size")
            {
                if (Int32.TryParse(value, out int val)) if (val < 0) value = Math.Abs(val * 1024).ToString();
            }
            propGridView.Rows.Add(new object[] { option, value });
            propGridView.Rows[propGridView.RowCount - 1].Cells[1].ReadOnly = true;
            return;
        }

        protected void LoadComboOption(string option, DataGridViewComboBoxCell dgc)
        {
            DataGridViewRow dgr = new DataGridViewRow();

            DataGridViewTextBoxCell dgvTbCell = new DataGridViewTextBoxCell();
            dgvTbCell.Value = option;
            dgr.Cells.Add(dgvTbCell);

            dgr.Cells.Add(dgc);
            dgc.ReadOnly = false;

            propGridView.Rows.Add(dgr);
            dgr.ReadOnly = false;
            return;
        }

        protected void LoadSpacer(string caption = "")
        {
            DataGridViewRow dgr = new DataGridViewRow();

            DataGridViewTextBoxCell dgvTbCell = new DataGridViewTextBoxCell();
            dgvTbCell.Value = null;

            DataGridViewTextBoxCell dgvTbCell1 = new DataGridViewTextBoxCell();
            dgvTbCell1.Value = null;

            dgr.Cells.Add(dgvTbCell);
            dgr.Cells.Add(dgvTbCell1);
            propGridView.Rows.Add(dgr);
            if (!string.IsNullOrEmpty(caption))
            {
                propGridView.Rows[propGridView.RowCount - 1].Cells[0].Value = caption;
                Font capFont = new Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                DataGridViewCellStyle capStyle = new DataGridViewCellStyle { BackColor = Color.DarkGray, ForeColor = Color.AntiqueWhite, SelectionBackColor = Color.DarkGray, SelectionForeColor = Color.AntiqueWhite };
                capStyle.Font = capFont;
                propGridView.Rows[propGridView.RowCount - 1].DefaultCellStyle = capStyle;
            }
            return;
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

        private void propGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 0) e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            if (e.ColumnIndex < 1 || e.RowIndex < 1) return;

            if (propGridView[e.ColumnIndex, e.RowIndex].Value == null && propGridView[e.ColumnIndex - 1, e.RowIndex].Value == null)
            {
                e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            }
            else
            {
                e.AdvancedBorderStyle.Left = propGridView.AdvancedCellBorderStyle.Left;
            }
        }

        private void propGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 1 || e.RowIndex < 1) return;
            if (propGridView[e.ColumnIndex, e.RowIndex].Value == null && propGridView[e.ColumnIndex - 1, e.RowIndex].Value == null)
            {
                e.Value = string.Empty;
                e.FormattingApplied = true;
            }
        }
    }
}
