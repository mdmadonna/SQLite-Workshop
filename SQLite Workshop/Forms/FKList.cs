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
    public partial class FKList : Form
    {
        ToolTip toolTip;
        public FKList()
        {
            InitializeComponent();
        }

        private void FKList_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = "Foreign Key List";
            lblName.Text = string.Format("Database: {0}", MainForm.mInstance.CurrentDB);

            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbClose, "Close");
            dgFKList.AutoGenerateColumns = true;
            LoadForeignKeys();
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        string[] columnList = new string[] { "source", "table", "from", "to", "on_update", "on_delete", "match" };
        protected void LoadForeignKeys()
        {
            SchemaDefinition sd = DataAccess.SchemaDefinitions[MainForm.mInstance.CurrentDB];
            SQLiteErrorCode returnCode = SQLiteErrorCode.Ok;
            DataTable dt;
            DataTable dtGrid = new DataTable();
            for (int i = 0; i < columnList.Length; i++)
            {
                DataColumn dc = new DataColumn();
                dc.DataType = Type.GetType("System.String");
                dc.ColumnName = columnList[i];
                dtGrid.Columns.Add(dc);
            }

            try
            {
                string sql;
                foreach (var table in sd.Tables)
                {
                    sql = string.Format("Pragma foreign_key_list(\"{0}\")", table.Key);
                    dt = DataAccess.ExecuteDataTable(MainForm.mInstance.CurrentDB, sql, out returnCode);
                    foreach (DataRow dr in dt.Rows)
                    {
                        dtGrid.Rows.Add(new string[] { table.Key, dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString() });
                    }
                }
                if (dtGrid.Rows.Count == 0)
                {
                    this.Close();
                    Common.ShowMsg("No Foreign Keys were found.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                dgFKList.Columns.Clear();
                dgFKList.DataSource = dtGrid;
                dgFKList.Refresh();
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Unable to load Foreign Key List.\r\n {0}", ex.Message));
                return;
            }
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

        #region Form Dragging Event Handler

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
