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
    public partial class BuildIndex : Form
    {
        ToolTip toolTip;
        SchemaDefinition sd;
        string DatabaseLocation;
        ContextMenu dgiContextMenu;
        string ErrorMessage;

        internal string TableName { get; set; }

        public BuildIndex()
        {
            InitializeComponent();
        }

        private void BuildIndex_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = "Build Index";
            toolStripStatusLabelResult.Text = string.Empty;

            DatabaseLocation = MainForm.mInstance.CurrentDB;
            sd = DataAccess.SchemaDefinitions[DatabaseLocation];

            comboBoxTableName.Items.Clear();
            foreach (var table in sd.Tables)
            {
                if (!Common.IsSystemTable(table.Key)) comboBoxTableName.Items.Add(table.Key);
            }
            if (!string.IsNullOrEmpty(TableName)) comboBoxTableName.SelectedItem = TableName;

            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbClose, "Close");

            dgiContextMenu = new ContextMenu();
            dgiContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Move Up", dgiContextMenu_Clicked, dgiContextMenu_Popup, dgiContextMenu_Selected, null) { Name = "MoveUp" });
            dgiContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Move Down", dgiContextMenu_Clicked, dgiContextMenu_Popup, dgiContextMenu_Selected, null) { Name = "MoveDown" });
            dgiContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Remove", dgiContextMenu_Clicked, dgiContextMenu_Popup, dgiContextMenu_Selected, null) { Name = "Remove" });
            dgvIndexColumns.ContextMenu = dgiContextMenu;

        }

        private void comboBoxTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTableName.SelectedIndex < 0) return;

            dgvColumns.Rows.Clear();
            foreach (var col in sd.Tables[comboBoxTableName.SelectedItem.ToString()].Columns)
            {
                dgvColumns.Rows.Add(new string[] { col.Key, col.Value.ColumnType });
            }

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                Common.ShowMsg(ErrorMessage);
                return;
            }

            string sql = BuildSQL();
            Cursor = Cursors.WaitCursor;
            DataAccess.ExecuteNonQuery(DatabaseLocation, sql, out SQLiteErrorCode returnCode);
            Cursor = Cursors.Default;
            if (returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(string.Format(Common.ERR_CREATEIDXFAILED, DataAccess.LastError, returnCode.ToString()));
            }
            else
            {
                Common.ShowMsg(string.Format(Common.OK_IDXCREATED, txtIndexName.Text), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnShowSQL_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                Common.ShowMsg(ErrorMessage);
                return;
            }

            ShowSQL sSQL = new ShowSQL();
            sSQL.SQL = BuildSQL();
            sSQL.ShowDialog();
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateInput()
        {
            if (comboBoxTableName.SelectedIndex < 0)
            {
                ErrorMessage = "Please select a table.";
                comboBoxTableName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtIndexName.Text))
            {
                ErrorMessage = "Please enter an Index Name.";
                txtIndexName.Focus();
                return false;
            }

            if (dgvIndexColumns.RowCount == 1)
            {
                ErrorMessage = "Please select one or more columns to include in the index.";
                dgvColumns.Focus();
                return false;
            }
            return true;
        }

        private string BuildSQL()
        {
            StringBuilder sb = new StringBuilder();
            string order = string.Empty;

            sb.Append("Create");
            if (ChkUnique.Checked) sb.Append(" Unique");
            sb.Append(" Index \"").Append(txtIndexName.Text).Append("\" On \"").Append(comboBoxTableName.SelectedItem.ToString()).Append("\"\r\n(");
            for (int i = 0; i < dgvIndexColumns.RowCount - 1; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append("\r\n\t\"").Append(dgvIndexColumns.Rows[i].Cells[0].Value.ToString()).Append("\"");
                order = (dgvIndexColumns.Rows[i].Cells[1].Value == null) ? string.Empty : dgvIndexColumns.Rows[i].Cells[1].Value.ToString();
                if (order.ToLower() == "descending") sb.Append(" Desc");
            }
            sb.Append("\r\n)");
            if (!string.IsNullOrEmpty(txtWhereClause.Text)) sb.Append("\r\nWhere ").Append(txtWhereClause.Text);
            return sb.ToString();
        }

        #region GridView hangdlers
        private void dgvColumns_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewRow dgr = ((DataGridView)sender).SelectedRows[0];
            string selectedColumn = dgr.Cells[0].Value.ToString();

            foreach (DataGridViewRow dg in dgvIndexColumns.Rows)
            {
                if (dg.Cells[0].Value != null)
                    if (dg.Cells[0].Value.ToString() == selectedColumn) return;
            }
            dgvIndexColumns.Rows.Add(new string[] { dgr.Cells[0].Value.ToString() });
            dgvIndexColumns.Rows[dgvIndexColumns.RowCount - 2].Cells[1].ReadOnly = false;
        }

        private void dgvIndexColumns_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo;
            if (e.Button != MouseButtons.Right) return;

            hitTestInfo = dgvIndexColumns.HitTest(e.X, e.Y);
            //if (hitTestInfo.Type == DataGridViewHitTestType.RowHeader && hitTestInfo.RowIndex >= 0 && hitTestInfo.RowIndex < dgvIndexColumns.RowCount - 1)
            if (hitTestInfo.RowIndex >= 0 && hitTestInfo.RowIndex < dgvIndexColumns.RowCount - 1)
            {
                int rowindex = hitTestInfo.RowIndex;
                dgvIndexColumns.Rows[rowindex].Selected = true;
                dgvIndexColumns.CurrentCell = dgvIndexColumns.Rows[rowindex].Cells[0];

                dgiContextMenu.Show(dgvIndexColumns, new Point(e.X, e.Y));
                //if (dgvIndexColumns.RowCount <= 2)
                //{
                //    dgiContextMenu.MenuItems["MoveUp"].Enabled = false;
                //    dgiContextMenu.MenuItems["MoveDown"].Enabled = false;
                //    return;
                //}
                //dgiContextMenu.MenuItems["MoveUp"].Enabled = rowindex == 0 ? false : true;
                //dgiContextMenu.MenuItems["MoveDown"].Enabled = rowindex > dgvIndexColumns.RowCount - 2 ? false : true;
            }
        }

        #endregion 

        #region Context Menu Handlers
        private void dgiContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void dgiContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void dgiContextMenu_Clicked(object sender, EventArgs e)
        {
            int row;
            DataGridViewRow dgr;

            switch (((MenuItem)sender).Text.ToLower())
            {
                case "move up":
                    row = dgvIndexColumns.SelectedRows[0].Index;
                    if (row == 0) return;
                    dgr = dgvIndexColumns.SelectedRows[0];
                    dgvIndexColumns.Rows.RemoveAt(row);
                    dgvIndexColumns.Rows.Insert(row - 1, dgr);
                    break;
                case "move down":
                    row = dgvIndexColumns.SelectedRows[0].Index;
                    if (row == dgvIndexColumns.RowCount - 2) return;
                    dgr = dgvIndexColumns.SelectedRows[0];
                    dgvIndexColumns.Rows.RemoveAt(row);
                    dgvIndexColumns.Rows.Insert(row + 1, dgr);
                    break;
                case "remove":
                    dgvIndexColumns.Rows.RemoveAt(dgvIndexColumns.SelectedRows[0].Index);
                    break;
                default:
                    break;
            }
        }

        #endregion

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
