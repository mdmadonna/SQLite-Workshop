using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    public partial class BuildColumn : Form
    {
        ToolTip toolTip;
        string tablename;
        ColumnPropertySettings props;

        public BuildColumn()
        {
            InitializeComponent();
        }

        internal string DatabaseLocation { get; set; }
        internal TreeNode TargetNode { get; set; }
        internal SQLType ExecType { get; set; }


        private void BuildColumn_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = "Column Editor";

            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbClose, "Close");
            InitializeForm();
        }

        protected void InitializeForm()
        {
            tablename = TargetNode.Text == "Columns" ? TargetNode.Parent.Text : TargetNode.Parent.Parent.Text;
            lblTableName.Text = tablename;
            props = new ColumnPropertySettings();
            switch (ExecType)
            {
                case SQLType.SQLAddColumn:
                    lblPanelHeading.Text = "Add Column";
                    lblNewColumnName.Visible = false;
                    txtNewColumn.Visible = false;
                    props.Type = "varchar";
                    props.AllowNull = true;
                    props.Size = 50;
                    break;
                case SQLType.SQLModifyColumn:
                    lblPanelHeading.Text = "Modify Column";
                    txtColumn.Text = TargetNode.Tag.ToString();
                    txtColumn.ReadOnly = true;
                    lblNewColumnName.Visible = false;
                    txtNewColumn.Visible = false;
                    break;
                case SQLType.SQLDeleteColumn:
                    lblPanelHeading.Text = "Delete Column";
                    txtColumn.Text = TargetNode.Tag.ToString();
                    txtColumn.ReadOnly = true;
                    InitializePropertyGrid(tablename, TargetNode.Tag.ToString());
                    props.ReadOnly(true);
                    lblNewColumnName.Visible = false;
                    txtNewColumn.Visible = false;
                    break;
                case SQLType.SQLRenameColumn:
                    lblPanelHeading.Text = "Rename Column";
                    txtColumn.Text = TargetNode.Tag.ToString();
                    txtColumn.ReadOnly = true;
                    
                    break;
                default:
                    break;
            }

            propertyGridColumn.SelectedObject = props;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            switch (ExecType)
            {
                case SQLType.SQLAddColumn:
                    if (!ValidateAddInput()) return;
                    AddColumn();
                    break;
                case SQLType.SQLModifyColumn:
                    if (!ValidateModifyInput()) return;
                    ModifyColumn();
                    break;
                case SQLType.SQLRenameColumn:
                    if (!ValidateRenameInput()) return;
                    RenameColumn();
                    break;
                case SQLType.SQLDeleteColumn:
                    if (!ValidateDeleteInput()) return;
                    DeleteColumn();
                    break;
                default:
                    break;
            }
        }

        protected bool ValidateAddInput()
        {
            if (string.IsNullOrEmpty(txtColumn.Text))
            {
                Common.ShowMsg("Please enter a name for this column.");
                txtColumn.Focus();
                return false;
            }
            return true;
        }

        protected bool ValidateModifyInput()
        {
            return true;
        }

        protected bool ValidateRenameInput()
        {
            if (string.IsNullOrEmpty(txtNewColumn.Text))
            {
                Common.ShowMsg("Please enter a new name for this column.");
                txtNewColumn.Focus();
                return false;
            }
            if (Common.HasInvalidChars(txtNewColumn.Text, out string chars))
            {
                Common.ShowMsg(string.Format("Please remove characters \"{0}\" from the new column name.", chars));
                txtNewColumn.Focus();
                return false;
            }
            return true;
        }

        protected bool ValidateDeleteInput()
        {
            return true;
        }

        protected void AddColumn()
        {
            string sql = SqlFactory.BuildAddColumnSQL(tablename, txtColumn.Text, BuildColumnLayout());
            var result = DataAccess.ExecuteNonQuery(MainForm.mInstance.CurrentDB, sql, out SQLiteErrorCode returnCode);
            if (result == -1 || returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(string.Format("Add Column Failed.\r\n{0}", DataAccess.LastError));
                return;
            }
            toolStripStatusLabel1.Text = "Column Added.";
            MainForm.mInstance.AddTable(tablename);
        }
        protected void ModifyColumn()
        {
            if (RebuildTable())
                toolStripStatusLabel1.Text = "Column Modified.";
        }
        protected void RenameColumn()
        {
            if (RebuildTable())
                toolStripStatusLabel1.Text = "Column Renamed.";
        }
        protected void DeleteColumn()
        {
            if (RebuildTable())
                toolStripStatusLabel1.Text = "Column Deleted.";
        }


        protected bool RebuildTable()
        {
            bool bNoWarning = false;
            bool.TryParse(MainForm.cfg.appsetting(Config.CFG_COLUMNEDITWARN), out bNoWarning);
            if (!bNoWarning)
            {
                if (!ShowWarning()) return false;
            }

            string sql;
            SQLiteErrorCode returnCode;
            bool foreign_key_enabled;
            string CurrentDB = MainForm.mInstance.CurrentDB;

            sql = "Pragma foreign_keys";
            var data = DataAccess.ExecuteScalar(CurrentDB, sql, out returnCode);
            foreign_key_enabled = (long)data == 0 ? false : true;

            sql = string.Format("Select * From sqlite_master Where type = \"index\" AND tbl_name = \"{0}\"", tablename);
            DataTable idxDT = DataAccess.ExecuteDataTable(CurrentDB, sql, out returnCode);

            sql = string.Format("Select * From sqlite_master Where type = \"trigger\" AND tbl_name = \"{0}\"", tablename);
            DataTable trigDT = DataAccess.ExecuteDataTable(CurrentDB, sql, out returnCode);

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            Dictionary<string, ColumnLayout> columns = DataAccess.SchemaDefinitions[CurrentDB].Tables[tablename].Columns;
            Dictionary<string, ColumnLayout> newcolumns = new Dictionary<string, ColumnLayout>(); ;

            switch (ExecType)
            {
                case SQLType.SQLRenameColumn:
                    foreach (var col in columns) { newcolumns.Add(col.Key == txtColumn.Text ? txtNewColumn.Text : col.Key, col.Value); }
                    break;
                case SQLType.SQLModifyColumn:
                    foreach (var col in columns) { newcolumns.Add(col.Key, col.Key == txtColumn.Text ? BuildColumnLayout() : col.Value); }
                    break;
                case SQLType.SQLDeleteColumn:
                    columns.Remove(txtColumn.Text);
                    newcolumns = columns;
                    break;
                default:
                    return false;
            }

            string tmptablename = Common.TempTableName();
            if (string.IsNullOrEmpty(tmptablename))
            {
                Common.ShowMsg("Cannot build temporary table - terminating.");
                return false;
            }
            string CreateSQL = SqlFactory.CreateSQL(tmptablename, newcolumns);
            string SelectSQL = SqlFactory.SelectSql(tablename, columns);

            if (foreign_key_enabled) DataAccess.ExecuteNonQuery(CurrentDB, "Pragma foreign_keys=false", out returnCode);

            bool rCode = DataAccess.OpenDB(CurrentDB, ref conn, ref cmd, out returnCode, false);
            if (!rCode || returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(String.Format(Common.ERR_SQL, DataAccess.LastError, returnCode));
                return false;
            }

            SQLiteTransaction sqlT;
            sqlT = conn.BeginTransaction();

            try
            {
                //Create the temp table
                cmd.CommandText = CreateSQL;
                var createRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);

                //Copy data from the current table to the temp table
                string insertSQL = string.Format("Insert Into {0} {1}", tmptablename, SelectSQL);
                cmd.CommandText = insertSQL;
                var insertRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);

                //delete the current table
                cmd.CommandText = SqlFactory.DropSql(tablename);
                var deleteRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);

                //rename the temp table to the old table name
                cmd.CommandText = string.Format("Alter Table {0} Rename To {1}", tmptablename, tablename);
                var renameRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);

                //Rebuild Indexes
                foreach (DataRow dr in idxDT.Rows)
                {
                    cmd.CommandText = dr["sql"].ToString();
                    var indexRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);
                }
               

                //Rebuild Triggers
                foreach (DataRow dr in trigDT.Rows)
                {
                    cmd.CommandText = dr["sql"].ToString();
                    var indexRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);
                }

                sqlT.Commit();
            }
            catch (Exception ex)
            {
                Common.ShowMsg(String.Format(Common.ERR_SQL, ex.Message, returnCode));
                sqlT.Rollback();
            }
            finally
            {
                DataAccess.CloseDB(conn);
                if (foreign_key_enabled) DataAccess.ExecuteNonQuery(CurrentDB, "Pragma foreign_keys=true", out returnCode);
            }
            MainForm.mInstance.AddTable(tablename);
            return true;
        }

        #region Helpers

        private bool ShowWarning()
        {
            ShowMsg sm = new ShowMsg();
            sm.Message = "WARNING!!!.  This feature is not natively supported by SQLite.  It is implemented by copying the table with column changes, deleteing the old table and renaming the copied table.  Once complete, YOU MUST MANUALLY CHANGE ANY INDEX, VIEW OR TRIGGER CONTAINING THIS COLUMN!!!. IF YOU DO NOT DO THIS YOU MAY LEAVE YOUR INDEX, VIEW OR TRIGGER IN AN INCONSISTENT STATE!!!\r\n\r\nPress 'Ok' to continue or 'Cancel' to exit.";
            sm.ShowDialog();
            if (sm.DoNotShow) MainForm.cfg.SetSetting(Config.CFG_COLUMNEDITWARN, "true");
            return sm.Result == DialogResult.Cancel ? false : true;
        }

        protected void InitializePropertyGrid(string TableName, string ColumnName)
        {
            
            //Columnse props.SetCustomAttribute(cab);
        }

        internal ColumnLayout BuildColumnLayout()
        {
            ColumnLayout col = new ColumnLayout();
            col.Check = string.Empty;
            col.Collation = props.CollatingSequence;
            col.ColumnType = BuildColumnType();
            col.DefaultValue = props.DefaultValue;
            col.ForeignKey = new ForeignKeyLayout();
            col.ForeignKey.Table = props.ForeignKeyParent;
            col.ForeignKey.To = props.ForeignKeyColumn;
            col.ForeignKey.OnUpdate = props.ForeignKeyOnUpdate;
            col.ForeignKey.OnDelete = props.ForeignKeyOnDelete;
            col.NullType = props.AllowNull ? 0 : 1;
            col.PrimaryKey = 0;
            col.Unique = false;
            return col;
        }

        protected string BuildColumnType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(props.Type);
            if (Common.IsText(props.Type)) sb.AppendFormat("({0})", props.Size.ToString());
            return sb.ToString();
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
