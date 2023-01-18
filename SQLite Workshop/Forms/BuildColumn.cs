using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

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

        internal string DatabaseName { get; set; }
        internal TreeNode TargetNode { get; set; }
        internal SQLType ExecType { get; set; }


        private void BuildColumn_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            HouseKeeping(this, "Column Editor");
            InitializeForm();
        }

        private void BuildColumn_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
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
                    btnSave.Text = "Add";
                    break;
                case SQLType.SQLModifyColumn:
                    lblPanelHeading.Text = "Modify Column";
                    InitProps();
                    lblNewColumnName.Visible = false;
                    txtNewColumn.Visible = false;
                    btnSave.Text = "Modify";
                    break;
                case SQLType.SQLDeleteColumn:
                    lblPanelHeading.Text = "Delete Column";
                    InitProps();
                    props.ReadOnly(true);
                    lblNewColumnName.Visible = false;
                    txtNewColumn.Visible = false;
                    btnSave.Text = "Delete";
                    break;
                case SQLType.SQLRenameColumn:
                    lblPanelHeading.Text = "Rename Column";
                    InitProps();
                    props.ReadOnly(true);
                    btnSave.Text = "Rename";
                    break;
                default:
                    break;
            }

            propertyGridColumn.SelectedObject = props;
        }

        private void InitProps()
        {
            txtColumn.Text = TargetNode.Tag.ToString();
            txtColumn.ReadOnly = true;
            ColumnLayout cl = FindColumnLayout(DatabaseName, tablename, txtColumn.Text);
            props.Type = GetColumnType(cl.ColumnType);
            props.Size = GetColumnSize(cl.ColumnType);
            props.DefaultValue = cl.DefaultValue;
            props.AllowNull = cl.NullType == 0 ;
            props.CollatingSequence = cl.Collation;
            props.ForeignKeyColumn = cl.ForeignKey.From;
            props.ForeignKeyParent = cl.ForeignKey.Table;
            props.ForeignKeyOnDelete = cl.ForeignKey.OnDelete;
            props.ForeignKeyOnUpdate = cl.ForeignKey.OnUpdate;
        }

        private string GetColumnType(string coltype)
        {
            int i = coltype.IndexOf("(");
            return i < 0 ? coltype : coltype.Substring(0, i);
        }

        private int GetColumnSize(string coltype)
        {
            int size = 0;
            int i = coltype.IndexOf("(");
            string type = i < 0 ? coltype : coltype.Substring(0, i);
            if (IsText(type))
            {
                int j = coltype.IndexOf(")");
                if (j > i) size = Convert.ToInt32(coltype.Substring(i + 1, j - (i+1)).Trim());
            }
            return size;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = string.Empty;
            switch (ExecType)
            {
                case SQLType.SQLAddColumn:
                    if (!ValidateAddInput()) return;
                    AddColumn();
                    break;
                case SQLType.SQLModifyColumn:
                    if (!ValidateModifyInput()) return;
                    if (RebuildTable())
                    {
                        toolStripStatusLabel1.Text = "Column Modified.";
                        btnSave.Enabled = false;
                    }
                    break;
                case SQLType.SQLRenameColumn:
                    if (!ValidateRenameInput()) return;
                    if (RebuildTable())
                    {
                        toolStripStatusLabel1.Text = "Column Renamed.";
                        btnSave.Enabled = false;
                    }
                    break;
                case SQLType.SQLDeleteColumn:
                    if (!ValidateDeleteInput()) return;
                    if (RebuildTable())
                    {
                        toolStripStatusLabel1.Text = "Column Deleted.";
                        btnSave.Enabled = false;
                    }
                    break;
                default:
                    break;
            }
        }


        protected bool ValidateAddInput()
        {
            if (string.IsNullOrEmpty(txtColumn.Text))
            {
                ShowMsg("Please enter a name for this column.");
                txtColumn.Focus();
                return false;
            }
            return ValidateModifyInput();
        }

        protected bool ValidateModifyInput()
        {
            return true;
        }

        protected bool ValidateRenameInput()
        {
            if (string.IsNullOrEmpty(txtNewColumn.Text))
            {
                ShowMsg("Please enter a new name for this column.");
                txtNewColumn.Focus();
                return false;
            }
            if (HasInvalidChars(txtNewColumn.Text, out string chars))
            {
                ShowMsg(string.Format("Please remove characters \"{0}\" from the new column name.", chars));
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
                ShowMsg(string.Format("Add Column Failed.\r\n{0}", DataAccess.LastError));
                return;
            }
            toolStripStatusLabel1.Text = "Column Added.";
            MainForm.mInstance.AddTable(tablename, DatabaseName);
        }

        protected bool RebuildTable()
        {
            bool.TryParse(appSetting(Config.CFG_COLUMNEDITWARN), out bool bNoWarning);
            if (!bNoWarning)
            {
                if (!ShowWarning()) return false;
            }

            string sql;
            bool foreign_key_enabled;
            string CurrentDB = MainForm.mInstance.CurrentDB;

            sql = "Pragma foreign_keys";
            var data = DataAccess.ExecuteScalar(CurrentDB, sql, out SQLiteErrorCode returnCode);
            foreign_key_enabled = (long)data != 0;

            sql = string.Format("Select * From sqlite_master Where type = \"index\" AND tbl_name = \"{0}\"", tablename);
            DataTable idxDT = DataAccess.ExecuteDataTable(CurrentDB, sql, out returnCode);

            sql = string.Format("Select * From sqlite_master Where type = \"trigger\" AND tbl_name = \"{0}\"", tablename);
            DataTable trigDT = DataAccess.ExecuteDataTable(CurrentDB, sql, out returnCode);

            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;

            Dictionary<string, ColumnLayout> columns = DataAccess.SchemaDefinitions[CurrentDB].Tables[tablename].Columns;
            Dictionary<string, ColumnLayout> newcolumns = new Dictionary<string, ColumnLayout>();

            ArrayList DateCols = new ArrayList();
            switch (ExecType)
            {
                case SQLType.SQLRenameColumn:
                    foreach (var col in columns) 
                    { 
                        newcolumns.Add(col.Key == txtColumn.Text ? txtNewColumn.Text : col.Key, col.Value); 
                    }
                    break;
                case SQLType.SQLModifyColumn:
                    foreach (var col in columns) 
                    { 
                        newcolumns.Add(col.Key, col.Key == txtColumn.Text ? BuildColumnLayout() : col.Value);
                        if (IsDate(newcolumns[col.Key].ColumnType) && !IsDate(col.Value.ColumnType))
                            DateCols.Add(col.Key);
                    }
                    break;
                case SQLType.SQLDeleteColumn:
                    columns.Remove(txtColumn.Text);
                    newcolumns = columns;
                    break;
                default:
                    return false;
            }

            string tmptablename = TempTableName();
            if (string.IsNullOrEmpty(tmptablename))
            {
                ShowMsg("Cannot build temporary table - terminating.");
                return false;
            }
            string CreateSQL = SqlFactory.CreateSQL(tmptablename, newcolumns);
            string SelectSQL = SqlFactory.SelectSql(tablename, columns);

            if (foreign_key_enabled) DataAccess.ExecuteNonQuery(CurrentDB, "Pragma foreign_keys=false", out returnCode);

            bool rCode = DataAccess.OpenDB(CurrentDB, ref conn, ref cmd, false);
            if (!rCode || returnCode != SQLiteErrorCode.Ok)
            {
                ShowMsg(String.Format(ERR_SQL, DataAccess.LastError, returnCode));
                return false;
            }

            // Dates are a problematic to handle.  If any columns are being modified to a datetime format
            // we will need to modify the current date values to the correct value;
            Functions.BindFunction(conn, new Functions.ToDate());

            SQLiteTransaction sqlT;
            sqlT = conn.BeginTransaction();

            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Changing a column to a date or datetime type is problematic.  The source value MUST
                // be in the format YYYY-MM-DDTHH:MM:SS AM" so let's change them before starting
                if (DateCols.Count > 0)
                {
                    cmd.CommandText = string.Format("UPDATE {0} SET {1} = ToDate({1})", tablename, DateCols[0]);
                    long l = cmd.ExecuteNonQuery();
                }

                //Create the temp table
                cmd.CommandText = CreateSQL;
                DataAccess.ExecuteNonQuery(cmd, out returnCode);
                if (returnCode != SQLiteErrorCode.Ok) throw new Exception(String.Format("Cannot create Temp Table.\r\n{0}", DataAccess.LastError));

                toolStripStatusLabel1.Text = "Copying Data to a temporary table.";
                Application.DoEvents();
                //Copy data from the current table to the temp table
                string insertSQL = string.Format("Insert Into {0} {1}", tmptablename, SelectSQL);
                cmd.CommandText = insertSQL;
                var insertRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);
                if (insertRtnCode < 0) throw new Exception(String.Format("Cannot Copy Rows into Temp Table.\r\n{0}", DataAccess.LastError));

                toolStripStatusLabel1.Text = "Dropping table.";
                Application.DoEvents();
                //delete the current table
                cmd.CommandText = SqlFactory.DropSql(tablename);
                var deleteRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);
                if (deleteRtnCode < 0) throw new Exception(String.Format("Cannot Delete Original Table.\r\n{0}", DataAccess.LastError));

                toolStripStatusLabel1.Text = "Renaming table.";
                Application.DoEvents();
                //rename the temp table to the old table name
                cmd.CommandText = string.Format("Alter Table {0} Rename To {1}", tmptablename, tablename);
                var renameRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);
                if (renameRtnCode < 0) throw new Exception(String.Format("Cannot Rename Temp Table.\r\n{0}", DataAccess.LastError));


                toolStripStatusLabel1.Text = "Rebuilding Indexes.";
                Application.DoEvents();
                //Rebuild Indexes
                foreach (DataRow dr in idxDT.Rows)
                {
                    // AutoIndex will not have SQL associated with it
                    if (!string.IsNullOrEmpty(dr["sql"].ToString()))
                    {
                        cmd.CommandText = dr["sql"].ToString();
                        var indexRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);
                        if (indexRtnCode < 0) throw new Exception(String.Format("Cannot Rebuild Indexes.\r\n{0}", DataAccess.LastError));
                    }
                }


                toolStripStatusLabel1.Text = "Rebuilding Triggers.";
                Application.DoEvents();
                //Rebuild Triggers
                foreach (DataRow dr in trigDT.Rows)
                {
                    cmd.CommandText = dr["sql"].ToString();
                    var triggerRtnCode = DataAccess.ExecuteNonQuery(cmd, out returnCode);
                    if (triggerRtnCode < 0) throw new Exception(String.Format("Cannot Create Triggers.\r\n{0}", DataAccess.LastError));
                }
                toolStripStatusLabel1.Text = "Committing Changes.";
                Application.DoEvents();
                sqlT.Commit();
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Rolling Back Changes.";
                Application.DoEvents();
                sqlT.Rollback();
                ShowMsg(String.Format(ERR_SQL, ex.Message, returnCode));
                return false;
            }
            finally
            {
                DataAccess.CloseDB(conn);
                if (foreign_key_enabled) DataAccess.ExecuteNonQuery(CurrentDB, "Pragma foreign_keys=true", out returnCode);
                this.Cursor = Cursors.Default;
            }
            MainForm.mInstance.AddTable(tablename, DatabaseName);
            return true;
        }

        #region Helpers

        private bool ShowWarning()
        {
            ShowMsg sm = new ShowMsg
            {
                Message = "WARNING!!!.  This feature is not natively supported by SQLite.  It is implemented by copying the table with column changes, deleting the old table and renaming the copied table.  Once complete, YOU MUST MANUALLY CHANGE ANY INDEX, VIEW OR TRIGGER CONTAINING THIS COLUMN!!!. IF YOU DO NOT DO THIS YOU MAY LEAVE YOUR INDEX, VIEW OR TRIGGER IN AN INCONSISTENT STATE!!!\r\n\r\nPress 'Ok' to continue or 'Cancel' to exit."
            };
            sm.ShowDialog();
            if (sm.DoNotShow) saveSetting(Config.CFG_COLUMNEDITWARN, "true");
            return sm.Result != DialogResult.Cancel;
        }


        internal ColumnLayout BuildColumnLayout()
        {
            ColumnLayout col = new ColumnLayout
            {
                Check = string.Empty,
                Collation = props.CollatingSequence,
                ColumnType = BuildColumnType(),
                DefaultValue = props.DefaultValue,
                ForeignKey = new ForeignKeyLayout()
                {
                    Table = props.ForeignKeyParent,
                    To = props.ForeignKeyColumn,
                    OnUpdate = props.ForeignKeyOnUpdate,
                    OnDelete = props.ForeignKeyOnDelete
                },
                NullType = props.AllowNull ? 0 : 1,
                PrimaryKey = 0,
                Unique = false
            };

            return col;
        }

        protected string BuildColumnType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(props.Type);
            if (IsText(props.Type)) sb.AppendFormat("({0})", props.Size.ToString());
            return sb.ToString();
        }

        #endregion
       
    }
}
