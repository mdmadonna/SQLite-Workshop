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
using System.Collections;

namespace SQLiteWorkshop
{
    public partial class TableEditorTabControl : UserControl
    {
        private string _dbLocation;
        private string[] PrimaryKeys;
        private string errorMsg;
        private StringBuilder sbFKeyClause;

        TableEditorPropertySettings tableSettings;

        private ContextMenu tbContextMenu;

        private enum PropertyRowDefs
        {
            DefaultValue = 0,
            FKeyHeader = 1,
            FKeyTable = 2,
            FKeyColumn = 3,
            FKeyOnUpdate = 4,
            FKeyOnDelete = 5,
            CollatingSequence = 6,
            CheckConstraint = 7
        }

        /// <summary>
        /// Descriptor for an entered row.  Transfer DataGrid values here for legibility
        /// </summary>
        protected struct ColumnRow
        {
            internal string ColName ;
            internal string ColType ;
            internal bool ColAllowNulls;
            internal bool ColUnique;
            internal string ColDefault;
            internal int ColPrimaryKey;
            internal string ColCheck;
            internal string ColCollation;
            internal string FK_Table;
            internal string FK_Column;
            internal string FK_OnUpdate;
            internal string FK_OnDelete;
        }
        
        public TableEditorTabControl(string dbLocation)
        {
            _dbLocation = dbLocation;
            InitializeComponent();
            if (Int32.TryParse(MainForm.cfg.appsetting(Config.CFG_TABLEEDITHSPLITP), out int parm)) hSplitter.SplitPosition = parm;

            tbContextMenu = new ContextMenu();
            tbContextMenu.MenuItems.AddRange(new MenuItem[]
            {
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Insert Column", tbContextMenu_Clicked, tbContextMenu_Popup, tbContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete Column", tbContextMenu_Clicked, tbContextMenu_Popup, tbContextMenu_Selected, null),
            });
            dgvTableDef.ContextMenu = tbContextMenu;

            tableSettings = new TableEditorPropertySettings();
            propertyGridTable.SelectedObject = tableSettings;

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            errorMsg = string.Empty;
            if (!ValidateInput())
            {
                MessageBox.Show(errorMsg, Common.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string sql = BuildCreateSQL();
            DataAccess.ExecuteNonQuery(_dbLocation, sql, out SQLiteErrorCode returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg(string.Format(Common.ERR_CREATEDBFAILED, DataAccess.LastError, returnCode.ToString()));
            }
            else
            {
                Common.ShowMsg(string.Format(Common.OK_DBCREATED, txtTableName.Text),MessageBoxButtons.OK, MessageBoxIcon.Information);
                MainForm.mInstance.AddTable(txtTableName.Text);
            }
        }


        #region Input Validation

        private ColumnRow MoveGridRow(DataGridViewRow dr)
        {
            ColumnRow cr = new ColumnRow();

            // Assign to local variables for readability
            cr.ColName = dr.Cells["ColName"].Value == null ? string.Empty : dr.Cells["ColName"].Value.ToString().Trim();
            cr.ColType = dr.Cells["ColType"].Value == null ? string.Empty : dr.Cells["ColType"].Value.ToString();
            cr.ColAllowNulls = dr.Cells["ColAllowNulls"].Value == null ? false : (bool)dr.Cells["ColAllowNulls"].Value;
            cr.ColUnique = dr.Cells["ColUnique"].Value == null ? false : (bool)dr.Cells["ColUnique"].Value;
            cr.ColDefault = dr.Cells["ColDefault"].Value == null ? string.Empty : dr.Cells["ColDefault"].Value.ToString();
            cr.ColPrimaryKey = 0;
            if (dr.Cells["ColPrimaryKey"].Value != null) Int32.TryParse(dr.Cells["ColPrimaryKey"].Value.ToString(), out cr.ColPrimaryKey);
            cr.ColCollation = dr.Cells["ColCollation"].Value == null ? null : dr.Cells["ColCollation"].Value.ToString();
            cr.ColCheck = dr.Cells["ColCheck"].Value == null ? null : dr.Cells["ColCheck"].Value.ToString();
            cr.FK_Table = dr.Cells["FKey_Table"].Value == null ? null : dr.Cells["FKey_Table"].Value.ToString();
            cr.FK_Column = dr.Cells["FKey_Column"].Value == null ? null : dr.Cells["FKey_Column"].Value.ToString();
            cr.FK_OnUpdate = dr.Cells["FKey_OnUpdate"].Value == null ? null : dr.Cells["FKey_OnUpdate"].Value.ToString();
            cr.FK_OnDelete = dr.Cells["FKey_OnDelete"].Value == null ? null : dr.Cells["FKey_OnDelete"].Value.ToString();

            return cr;
        }


        /// <summary>
        /// Validate all input and flag errors one at a time.
        /// </summary>
        /// <returns>False if input errors are found, otherwise true</returns>
        private bool ValidateInput()
        {
            // Table Name is required
            if (string.IsNullOrEmpty(txtTableName.Text))
            {
                errorMsg = "Please enter a Table Name.";
                txtTableName.Focus();
                return false;
            }

            // Loop through all rows to insure fields are populated correctly
            for (int i = 0; i < dgvTableDef.RowCount - 1; i++)
            {
                DataGridViewRow dr = dgvTableDef.Rows[i];

                ColumnRow cr = MoveGridRow(dr);

                //Column name is required
                if (string.IsNullOrEmpty(cr.ColName))
                {
                    errorMsg = "Column Name is required.";
                    dgvTableDef.CurrentCell = dr.Cells["ColName"];
                    dgvTableDef.BeginEdit(true);
                    return false;
                }
                // Let's make sure column names are not duplicated
                for (int j = 0; j < dgvTableDef.RowCount - 1; j++)
                {
                    if (j == i) continue;
                    string jColName = dgvTableDef.Rows[j].Cells["ColName"].Value == null ? string.Empty : dgvTableDef.Rows[j].Cells["ColName"].Value.ToString().Trim();
                    if (jColName == cr.ColName)
                    {
                        errorMsg = "Column Names must be unique.";
                        dgvTableDef.CurrentCell = dgvTableDef.Rows[j].Cells["ColName"];
                        dgvTableDef.BeginEdit(true);
                        return false;
                    }
                }

                //Column type is required
                    if (string.IsNullOrEmpty(cr.ColType))
                {
                    errorMsg = "Column Type is required.";
                    dgvTableDef.CurrentCell = dr.Cells["ColType"];
                    dgvTableDef.BeginEdit(true);
                    return false;
                }

                //Default Value must be consistent with datatype
                if (!string.IsNullOrEmpty(cr.ColDefault))
                {
                    if (!ValidateDefault(cr.ColType, cr.ColDefault))
                    {
                        dgvTableDef.CurrentCell = dr.Cells["ColDefault"];
                        dgvTableDef.BeginEdit(true);
                        return false;
                    }
                }

                //Foreign Key Validation
                // If all FK Columns are empty just return with no error
                if (String.IsNullOrEmpty(cr.FK_Table) && string.IsNullOrEmpty(cr.FK_Column) && string.IsNullOrEmpty(cr.FK_OnUpdate) && string.IsNullOrEmpty(cr.FK_OnDelete)) return true;

                // If th parent column is empty, find the column that's not empty and return an error.
                if (String.IsNullOrEmpty(cr.FK_Table))
                {
                    if (!string.IsNullOrEmpty(cr.FK_Column))
                    {
                        errorMsg = "You cannot enter a Foreign Key Column without a Foreign Key Parent Table.";
                        propertyGridTable.Focus();                  // Best I can do for now
                        return false;
                    }
                    if (!string.IsNullOrEmpty(cr.FK_OnUpdate))
                    {
                        errorMsg = "You cannot enter an Update Action without a Foreign Key Parent Table and Foreign Key Column.";
                        propertyGridTable.Focus();                  // Best I can do for now
                        return false;
                    }
                    if (!string.IsNullOrEmpty(cr.FK_OnDelete))
                    {
                        errorMsg = "You cannot enter an Update Action without a Foreign Key Parent Table and Foreign Key Column.";
                        propertyGridTable.Focus();                  // Best I can do for now
                        return false;
                    }
                }


                //Lastly, make sure a Foriegn Key Column is selected
                if (string.IsNullOrEmpty(cr.FK_Column))
                {
                    errorMsg = "You must select a Foreigh Key Column.";
                    propertyGridTable.Focus();                  // Best I can do for now
                    return false;
                }

            }

            // Validate the Primary Key sequence, if any exists
            // Primary Key values must be sequential - starting with '1' - and unique.  Additionally, there
            // can be no gaps in the sequence.
            bool[] Keys = new bool[dgvTableDef.RowCount];
            int MaxKey = 0;
            for (int i = 0; i < dgvTableDef.RowCount - 1; i++)
            {
                int ColPrimaryKey = 0;
                if (dgvTableDef.Rows[i].Cells["ColPrimaryKey"].Value != null) Int32.TryParse(dgvTableDef.Rows[i].Cells["ColPrimaryKey"].Value.ToString(), out ColPrimaryKey);
                if (ColPrimaryKey != 0) { Keys[ColPrimaryKey] = true; MaxKey = ColPrimaryKey; }
            }
            if (MaxKey > 0)
            {
                for (int i = 1; i <= MaxKey; i++)
                {
                    if (!Keys[i])
                    {
                        errorMsg = "Primary Key continuity error." + Environment.NewLine;
                        errorMsg += "Please insure Primary Key values start a 1 and increment sequentially by 1 for each column included in the Primary Key.";
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Attempt to insuure that Default values are consistent with the Column data type. This
        /// routine is not 100% accurate but will catch many Default Value related issues.
        /// </summary>
        /// <param name="type">Column Data Type</param>
        /// <param name="value">Default Value</param>
        /// <returns></returns>
        private bool ValidateDefault(string type, string value)
        {
            // if the default value is a function just return true
            if (value.Trim().StartsWith("(")) return true;

            if (Common.IsInteger(type))
            {
                if (!Int64.TryParse(value, out Int64 i))
                {
                    errorMsg = "Default Value must be a valid integer or function.";
                    return false;
                }
                return true;
            }

            if (Common.IsText(type)) return true;

            if (Common.IsReal(type))
            {
                if (!double.TryParse(value, out double i))
                {
                    errorMsg = "Default Value must be a valid real number or function.";
                    return false;
                }
                return true;
            }
            if (Common.IsDate(type))
            {
                if (!DateTime.TryParse(value, out DateTime i))
                {
                    errorMsg = "Default Value must be a valid date or function.";
                    return false;
                }
                return true;
            }
            if (Common.IsNumeric(type))
            {
                if (!decimal.TryParse(value, out decimal i))
                {
                    errorMsg = "Default Value must be a valid decimal value or function.";
                    return false;
                }
                return true;
            }
            if (Common.IsBoolean(type))
            {
                if (!bool.TryParse(value, out bool i))
                {
                    errorMsg = "Default Value must be a valid boolean value (0 or 1) or function.";
                    return false;
                }
                return true;
            }
            return true;
        }

        private bool ValidateForeignKeys()
        {
            return true;
        }
        #endregion

        private string BuildCreateSQL()
        {
            Dictionary<string, ColumnLayout> Columns = new Dictionary<string, ColumnLayout>();
            foreach (DataGridViewRow dr in dgvTableDef.Rows)
            {
                // Assign to local variables for readability
                // ColName and ColType have already been tested to insure values are not null
                ColumnRow cr = MoveGridRow(dr);

                if (string.IsNullOrEmpty(cr.ColName)) break;

                ColumnLayout column = new ColumnLayout();
                column.Check = cr.ColCheck;
                column.Collation = cr.ColCollation;
                column.ColumnType = cr.ColType;
                column.DefaultValue = cr.ColDefault;
                column.ForeignKey = new ForeignKeyLayout();
                column.ForeignKey.Table = cr.FK_Table;
                column.ForeignKey.To = cr.FK_Column;
                column.ForeignKey.OnUpdate = cr.FK_OnUpdate;
                column.ForeignKey.OnDelete = cr.FK_OnDelete;
                column.NullType = cr.ColAllowNulls ? 0 : 1;
                column.PrimaryKey = cr.ColPrimaryKey;
                column.Unique = cr.ColUnique;
                Columns.Add(cr.ColName, column);
            }
            return SqlFactory.CreateSQL(txtTableName.Text, Columns);
        }

        private string BuildCreateSQLx()
        {
            StringBuilder sb = new StringBuilder();
            int i;

            sb.Append("Create Table \"").Append(txtTableName.Text).Append("\" (\r\n");
            PrimaryKeys = new string[dgvTableDef.RowCount];

            sbFKeyClause = new StringBuilder();
            sbFKeyClause.Clear();

            for (i = 0; i < dgvTableDef.RowCount - 2; i++)
            {
                sb.Append("\t").Append(CreateColumnEntry(dgvTableDef.Rows[i], i)).Append(",\r\n");
            }
            sb.Append("\t").Append(CreateColumnEntry(dgvTableDef.Rows[i], i));

            // Let's construct the Primary Key clause
            string JoinedKeys = string.Join("|", PrimaryKeys);
            string[] Keys = JoinedKeys.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (Keys.Length > 0 && !string.IsNullOrEmpty(Keys[0]))
            {
                sb.Append(",\r\n\t Primary Key(");
                for (i = 0; i < Keys.Length - 1; i++)
                {
                    if (!string.IsNullOrEmpty(Keys[i])) sb.Append("\"").Append(Keys[i]).Append("\",");
                }
                sb.Append("\"").Append(Keys[i]).Append("\")");
            }

            // Let's construct the Foriegn Key clause
            if (sbFKeyClause.Length > 0)
            {
                sb.Append(",\r\n\t").Append(sbFKeyClause.ToString());
            }
            sb.Append("\r\n);");
            return sb.ToString();
        }

        /// <summary>
        /// Create a column description used in a Create Table command
        /// </summary>
        /// <param name="dr">A DataGridViewRow with column information</param>
        /// <param name="i">Relative location of the DataGridViewRow</param>
        /// <returns></returns>
        protected string CreateColumnEntry(DataGridViewRow dr, int i)
        {
            StringBuilder sb = new StringBuilder();

            // Assign to local variables for readability
            // ColName and ColType have already been tested to insure values are not null
            ColumnRow cr = MoveGridRow(dr);

            sb.Append("\"").Append(cr.ColName).Append("\" ");
            sb.Append(cr.ColType);
            sb.Append(cr.ColAllowNulls ? " Null" : " Not Null");
            if (cr.ColUnique) sb.Append(" Unique");
            if (!string.IsNullOrEmpty(cr.ColDefault))
            {
                sb.Append(" Default ");
                // If Column Type is a Text Type, wrap the default value in Quotes.
                // Note that Default values that are functions must be preceeded by a "("
                if (!cr.ColDefault.Trim().StartsWith("(") || Common.IsText(cr.ColType))
                { sb.Append("\"").Append(cr.ColDefault).Append("\""); }
                else
                { sb.Append(cr.ColDefault); }
            }
            if (!string.IsNullOrEmpty(cr.ColCheck)) sb.Append(" Check").Append(cr.ColCheck);
            if (!string.IsNullOrEmpty(cr.ColCollation)) sb.Append(" Collate ").Append(cr.ColCollation);
            if (cr.ColPrimaryKey > 0) PrimaryKeys[cr.ColPrimaryKey - 1] = cr.ColName;

            //Start constructing the Foriegn Key clause
            if (!string.IsNullOrEmpty(cr.FK_Table))
            {
                if (sbFKeyClause.Length != 0) sbFKeyClause.Append(",\r\n\t");
                sbFKeyClause.Append("Foreign Key (\"").Append(cr.ColName).Append("\") References \"").Append(cr.FK_Table).Append("\"(\"").Append(cr.FK_Column).Append("\")");
                if (!string.IsNullOrEmpty(cr.FK_OnDelete)) sbFKeyClause.Append(" On Delete ").Append(cr.FK_OnDelete);
                if (!string.IsNullOrEmpty(cr.FK_OnUpdate)) sbFKeyClause.Append(" On Update ").Append(cr.FK_OnUpdate);
            }
            return sb.ToString();
        }

        #region Context Menu Handlers

        private void tbContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void tbContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void tbContextMenu_Clicked(object sender, EventArgs e)
        {
            switch (((MenuItem)sender).Text.ToLower())
            {
                case "insert column":
                    dgvTableDef.Rows.Insert(dgvTableDef.SelectedRows[0].Index);
                    break;
                case "delete column":
                    dgvTableDef.Rows.RemoveAt(dgvTableDef.SelectedRows[0].Index);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region DataGridView Editing
        private void dgvTableDef_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!dgvTableDef.CurrentCell.IsInEditMode) return;

            if (dgvTableDef.CurrentCell.ColumnIndex == dgvTableDef.Columns["ColType"].Index)
            {
                string CellValue = e.FormattedValue.ToString().ToLower();
                dgvTableDef.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = CellValue;
                if (!(((DataGridViewComboBoxCell)dgvTableDef.Rows[e.RowIndex].Cells[e.ColumnIndex]).Items.Contains(CellValue)))
                {
                    if (!ReplaceItem((DataGridViewComboBoxCell)dgvTableDef.Rows[e.RowIndex].Cells[e.ColumnIndex], CellValue))
                    {
                        dgvTableDef.Rows[e.RowIndex].ErrorText = "Invalid Column Type";
                        e.Cancel = true;
                    }
                }
            }
        }

        private void dgvTableDef_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // Make sure KeyPress event handler is not registered
            e.Control.KeyPress -= new KeyPressEventHandler(dgvTableDef_KeyPress);

            // Require numeric input for the Primary Key column
            if (dgvTableDef.CurrentCell.ColumnIndex == dgvTableDef.Columns["colPrimaryKey"].Index) 
            {
                if (e.Control != null)
                {
                    ((TextBox)e.Control).KeyPress += new KeyPressEventHandler(dgvTableDef_KeyPress);
                }
            }
            

            if (e.Control.GetType() == typeof(DataGridViewComboBoxEditingControl))
            {
                ((ComboBox)e.Control).DropDownStyle = ComboBoxStyle.DropDown;
                if (dgvTableDef.CurrentCell.ColumnIndex == dgvTableDef.Columns["colType"].Index)
                {
                    if (e.Control != null)
                    {
                        //((ComboBox)e.Control).KeyPress -= new KeyPressEventHandler(dgvComboBox_KeyPress);
                        //((ComboBox)e.Control).KeyPress += new KeyPressEventHandler(dgvComboBox_KeyPress);
                    }
                }
            }
        }

        /// <summary>
        /// This routine facilitates entry of custom lengths for column types that require a length
        /// component (i.e. char(2)).  The default value in the dropdown table (i.e. char(20)) is
        /// replaced with the user entered value.  This is needed to prevent edit errors.
        /// </summary>
        /// <param name="gridCell">The cell containing a combobox of valid SQLite datatypes.</param>
        /// <param name="ItemText">The User entered datatype.</param>
        /// <returns>True unles the datatype entered by the user is invalid.</returns>
        private bool ReplaceItem(DataGridViewComboBoxCell gridCell, string ItemText)
        {
            int i;
            int idx = ItemText.IndexOf("(");

            string coltext = idx > 0 ? ItemText.Substring(0, idx) : ItemText;
            for (i = 0; i < gridCell.Items.Count; i++)
            {
                if (gridCell.Items[i].ToString().StartsWith(coltext))
                {
                    gridCell.Items[i] = ItemText.ToLower();
                    break;
                }
            }
            if (i >= gridCell.Items.Count)
            {
                MessageBox.Show("Invalid Column Type", Common.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void dgvTableDef_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.   
            dgvTableDef.Rows[e.RowIndex].ErrorText = String.Empty;
        }


        /// <summary>
        /// DataGrid Keypress handler to require numeric input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvTableDef_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion
        
        /*
        private void dgvComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Left:
                    e.Handled = true;
                    //if (dgvTableDef.SelectedRows[0].Index < dgvTableDef.RowCount - 1) dgvTableDef.Rows.Insert(dgvTableDef.SelectedRows[0].Index);
                    break;
                default:
                    break;
            }
        }
        */

        private void dgvTableDef_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData & Keys.KeyCode)
            {
                case Keys.Insert:
                    if (dgvTableDef.SelectedRows[0].Index < dgvTableDef.RowCount - 1) dgvTableDef.Rows.Insert(dgvTableDef.SelectedRows[0].Index);
                    break;
                default:
                    break;
            }
        }

        private void dgvTableDef_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo;
            if (e.Button != MouseButtons.Right) return;

            hitTestInfo = dgvTableDef.HitTest(e.X, e.Y);
           if (hitTestInfo.Type == DataGridViewHitTestType.RowHeader && hitTestInfo.RowIndex >= 0 && hitTestInfo.RowIndex < dgvTableDef.RowCount -1)
           {
                int rowindex = hitTestInfo.RowIndex;
                dgvTableDef.Rows[rowindex].Selected = true;
                tbContextMenu.Show(dgvTableDef, new Point(e.X, e.Y));
           }
        }

        private void TableEditorTabControl_Leave(object sender, EventArgs e)
        {
            MainForm.cfg.SetSetting(Config.CFG_TABLEEDITHSPLITP, hSplitter.SplitPosition.ToString());
        }

        private void propertyGridTable_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var item = e.ChangedItem;

            switch (item.Label.ToString())
            {
                case "Foreign Key Parent Table":
                    tableSettings.LoadFKColumns(item.Value.ToString());
                    propertyGridTable.Refresh();
                    break;
                default:
                    break;
            }
            
        }

        int LastRow = -1;
        private void dgvTableDef_SelectionChanged(object sender, EventArgs e)
        {
            int currentRow = ((DataGridView)sender).SelectedRows[0].Index;
            if (currentRow == LastRow || LastRow == -1) { LastRow = currentRow; return; }
        

            DataGridViewRow dgr = ((DataGridView)sender).Rows[LastRow];
            dgr.Cells["colDefault"].Value = tableSettings.DefaultValue;
            dgr.Cells["colCollation"].Value = tableSettings.CollatingSequence;
            dgr.Cells["FKey_Table"].Value = tableSettings.ForeignKeyParent;
            dgr.Cells["FKey_Column"].Value = tableSettings.ForeignKeyColumn;
            dgr.Cells["FKey_OnUpdate"].Value = tableSettings.ForeignKeyOnUpdate;
            dgr.Cells["FKey_OnDelete"].Value = tableSettings.ForeignKeyOnDelete;
            dgr.Cells["ColCheck"].Value = tableSettings.CheckConstraint;

            dgr = ((DataGridView)sender).Rows[currentRow];
            tableSettings.DefaultValue = dgr.Cells["colDefault"].Value == null ? string.Empty :dgr.Cells["colDefault"].Value.ToString();
            tableSettings.CollatingSequence = dgr.Cells["colCollation"].Value == null ? string.Empty : dgr.Cells["colCollation"].Value.ToString();
            tableSettings.ForeignKeyParent = dgr.Cells["FKey_Table"].Value == null ? string.Empty : dgr.Cells["FKey_Table"].Value.ToString();
            tableSettings.ForeignKeyColumn = dgr.Cells["FKey_Column"].Value == null ? string.Empty : dgr.Cells["FKey_Column"].Value.ToString();
            tableSettings.ForeignKeyOnUpdate = dgr.Cells["FKey_OnUpdate"].Value == null ? string.Empty : dgr.Cells["FKey_OnUpdate"].Value.ToString();
            tableSettings.ForeignKeyOnDelete = dgr.Cells["FKey_OnDelete"].Value == null ? string.Empty : dgr.Cells["FKey_OnDelete"].Value.ToString();
            tableSettings.CheckConstraint = dgr.Cells["ColCheck"].Value == null ? string.Empty : dgr.Cells["ColCheck"].Value.ToString();
            propertyGridTable.Refresh();

            LastRow = currentRow;
        }

        private void dgvTableDef_Leave(object sender, EventArgs e)
        {
            int currentRow = ((DataGridView)sender).SelectedRows[0].Index;
            LastRow = currentRow;
            DataGridViewRow dgr = ((DataGridView)sender).Rows[currentRow];
            dgr.Cells["colDefault"].Value = tableSettings.DefaultValue;
            dgr.Cells["colCollation"].Value = tableSettings.CollatingSequence;
            dgr.Cells["FKey_Table"].Value = tableSettings.ForeignKeyParent;
            dgr.Cells["FKey_Column"].Value = tableSettings.ForeignKeyColumn;
            dgr.Cells["FKey_OnUpdate"].Value = tableSettings.ForeignKeyOnUpdate;
            dgr.Cells["FKey_OnDelete"].Value = tableSettings.ForeignKeyOnDelete;
            dgr.Cells["ColCheck"].Value = tableSettings.CheckConstraint;
        }

        private void dgvTableDef_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            dgvTableDef.BeginEdit(true);
        }

        private void propertyGridTable_Leave(object sender, EventArgs e)
        {
            int currentRow = dgvTableDef.SelectedRows[0].Index;
            LastRow = currentRow;
            DataGridViewRow dgr = dgvTableDef.Rows[currentRow];
            dgr.Cells["colDefault"].Value = tableSettings.DefaultValue;
            dgr.Cells["colCollation"].Value = tableSettings.CollatingSequence;
            dgr.Cells["FKey_Table"].Value = tableSettings.ForeignKeyParent;
            dgr.Cells["FKey_Column"].Value = tableSettings.ForeignKeyColumn;
            dgr.Cells["FKey_OnUpdate"].Value = tableSettings.ForeignKeyOnUpdate;
            dgr.Cells["FKey_OnDelete"].Value = tableSettings.ForeignKeyOnDelete;
            dgr.Cells["ColCheck"].Value = tableSettings.CheckConstraint;
        }

        private void btnShowSQL_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
            {
                MessageBox.Show(errorMsg, Common.APPNAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ShowSQL sSQL = new ShowSQL();
            sSQL.SQL = BuildCreateSQL();
            sSQL.ShowDialog();
        }
    }
}
