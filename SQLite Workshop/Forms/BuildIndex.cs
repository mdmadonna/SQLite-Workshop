using System;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

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
            toolTip = new ToolTip();
            HouseKeeping(this, "Build Index");
            toolStripStatusLabelResult.Text = string.Empty;

            DatabaseLocation = MainForm.mInstance.CurrentDB;
            sd = DataAccess.SchemaDefinitions[DatabaseLocation];

            comboBoxTableName.Items.Clear();
            foreach (var table in sd.Tables)
            {
                if (!IsSystemTable(table.Key)) comboBoxTableName.Items.Add(table.Key);
            }
            if (!string.IsNullOrEmpty(TableName)) comboBoxTableName.SelectedItem = TableName;

            dgiContextMenu = new ContextMenu();
            dgiContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Move Up", dgiContextMenu_Clicked, dgiContextMenu_Popup, dgiContextMenu_Selected, null) { Name = "MoveUp" });
            dgiContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Move Down", dgiContextMenu_Clicked, dgiContextMenu_Popup, dgiContextMenu_Selected, null) { Name = "MoveDown" });
            dgiContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Remove", dgiContextMenu_Clicked, dgiContextMenu_Popup, dgiContextMenu_Selected, null) { Name = "Remove" });
            dgvIndexColumns.ContextMenu = dgiContextMenu;

        }

        private void BuildIndex_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
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
                ShowMsg(ErrorMessage);
                return;
            }

            string sql = BuildSQL();
            Cursor = Cursors.WaitCursor;
            DataAccess.ExecuteNonQuery(DatabaseLocation, sql, out SQLiteErrorCode returnCode);
            Cursor = Cursors.Default;
            if (returnCode != SQLiteErrorCode.Ok)
            {
                ShowMsg(string.Format(ERR_CREATEIDXFAILED, DataAccess.LastError, returnCode.ToString()));
            }
            else
            {
                toolStripStatusLabelResult.Text = string.Format(OK_IDXCREATED, txtIndexName.Text);
                MainForm.mInstance.AddTable(TableName, DatabaseLocation);
            }
        }

        private void btnShowSQL_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                ShowMsg(ErrorMessage);
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

        #region GridView handlers
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

    }
}
