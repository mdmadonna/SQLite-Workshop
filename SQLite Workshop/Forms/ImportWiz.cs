using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SQLiteWorkshop
{
    public partial class ImportWiz : Form
    {

        ToolTip toolTip;
        ImportSource SourceType;
        DBSchema schema;
        DBManager db = null;

        Dictionary <string, ImportWizTextPropertySettings> ColumnSettings;

        public string DatabaseLocation { get; set; }

        private enum ImportSource
        {
            SQLite,
            MSAccess,
            SQLServer,
            MySql,
            Text,
            Excel,
            ODBC
        }

        public ImportWiz()
        {
            InitializeComponent();
        }

        private void ImportWiz_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = "Import Wizard";

            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbClose, "Close");

            toolStripStatusMsg.Text = string.Empty;
            btnPrevious.Enabled = false;

            panelWizMain.Dock = DockStyle.Fill;
            panelWizMainDB.Dock = DockStyle.Fill;
            panelWizMainDB.Visible = false;
            panelWizMainText.Dock = DockStyle.Fill;
            panelWizMainODBC.Dock = DockStyle.Fill;
            panelWizMainMySql.Dock = DockStyle.Fill;
            panelWizText.Dock = DockStyle.Fill;
            panelWizDB.Dock = DockStyle.Fill;
            

            panelWizMain.Visible = true;
            panelWizText.Visible = false;
            panelWizDB.Visible = false;
            cmbSourceDB.SelectedIndex = 0;
            LoadTableNames(cmbDestinationTable);
        }

        /// <summary>
        /// Navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            toolStripStatusMsg.Text = string.Empty;
            if (panelWizMain.Visible) { ProcessPanelMain(false); return; }
            if (panelWizText.Visible) { ProcessPanelText(false); return; }
            if (panelWizDB.Visible) { ProcessPanelDB(false); return; }
        }

        /// <summary>
        /// Navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            toolStripStatusMsg.Text = string.Empty;
            if (panelWizMain.Visible) { ProcessPanelMain(true); return; }
            if (panelWizText.Visible) { ProcessPanelText(true); return; }
            if (panelWizDB.Visible) { ProcessPanelDB(true); return; }
        }

        /// <summary>
        /// Close Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// File Lookup button click event.  Used for text, Excel, SQLite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFileSearch_Click(object sender, EventArgs e)
        {
            txtFileName.Text = FindFile();
            if (string.IsNullOrEmpty(txtFileName.Text)) return;

            //LoadSourceTableNames();
            return;
        }

        /// <summary>
        /// Find a file on local or network attached disk.
        /// </summary>
        /// <returns></returns>
        private string FindFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.CheckFileExists = true;
            openFile.AddExtension = true;
            openFile.AutoUpgradeEnabled = true;
            GetDefaultFileData(out string Title, out string DefaultExt, out string Filter);
            openFile.Title = Title;
            openFile.DefaultExt = DefaultExt;
            openFile.Filter = Filter;
            openFile.FilterIndex = 2;
            openFile.RestoreDirectory = true;
            //openFile.InitialDirectory = cfg.appsetting(Config.CFG_LASTOPEN);
            openFile.Multiselect = false;
            openFile.ShowReadOnly = false;
            openFile.ValidateNames = true;
            if (openFile.ShowDialog() != DialogResult.OK) return string.Empty;
            return openFile.FileName;
        }

        /// <summary>
        /// Load ComboBox with the names of tables in the currently opened SQLite database.
        /// </summary>
        /// <param name="cmb">ComboBox to load</param>
        private void LoadTableNames(ComboBox cmb)
        {
            SchemaDefinition sd = DataAccess.SchemaDefinitions[DatabaseLocation];
            cmb.Items.Clear();
            cmb.Items.Add("");
            foreach (var table in sd.Tables)
            {
                if (table.Value.TblType != SQLiteTableType.system) cmb.Items.Add(table.Key);
            }
        }

        /// <summary>
        /// Traffic cop routine to direct logic to appropriate routine based on the type of data
        /// being imported.
        /// </summary>
        /// <param name="bNext">true if user pressed 'Next' button.</param>
        private void ProcessPanelMain(bool bNext)
        {
            switch (SourceType)
            {
                case ImportSource.Text:
                    if (!ValidateTextPanel()) return;
                    panelWizMain.Visible = false;
                    panelWizText.Visible = true;
                    panelWizText.Dock = DockStyle.Fill;
                    panelWizDB.Visible = false;
                    btnPrevious.Enabled = true;
                    lblFileName.Text = txtFileName.Text;
                    tabText.SelectedTab = tabTextGeneral;
                    db = new DBTextManager(txtFileName.Text);
                    break;
                case ImportSource.SQLite:
                    if (!ValidateTextPanel()) return;
                    ImportFromDB();
                    break;
                case ImportSource.MSAccess:
                    if (!ValidateTextPanel()) return;
                    ImportFromDB();
                    break;
                case ImportSource.SQLServer:
                    if (!ValidateSQLServerPanel()) return;
                    ImportFromDB();
                    break;
                case ImportSource.ODBC:
                    if (!ValidateODBCPanel()) return;
                    ImportFromDB();
                    break;                
                case ImportSource.MySql:
                    if (!ValidateMySqlPanel()) return;
                    ImportFromDB();
                    break;
                default:
                    Common.ShowMsg("This Feature is not yet implemented.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }
        }

        /// <summary>
        /// Routines to manage import from a database.
        /// </summary>
        #region Import from Database

        /// <summary>
        /// Initialize first panel displayed when importing from a database.
        /// </summary>
        private void ImportFromDB()
        {
            switch (SourceType)
            {
                case ImportSource.SQLite:
                    lblDBName.Text = string.Format("(SQLite) {0}", txtFileName.Text);
                    db = new DBSQLiteManager(txtFileName.Text);
                    InitializeTablesPanel();
                    break;
                case ImportSource.MSAccess:
                    lblDBName.Text = string.Format("(MS Access) {0}", txtFileName.Text);
                    db = new DBMSAccessManager(txtFileName.Text);
                    InitializeTablesPanel();
                    break;
                case ImportSource.SQLServer:
                    lblDBName.Text = string.Format("(SQL Server) {0}:{1}", txtServer.Text,comboBoxDatabaseList.Text);
                    if (db == null) db = new DBMSAccessManager(txtServer.Text);
                    ((DBSqlServerManager)db).DatabaseUserName = txtDbUserName.Text;
                    ((DBSqlServerManager)db).DatabasePassword = txtDbPassword.Text;
                    ((DBSqlServerManager)db).DatabaseName = comboBoxDatabaseList.Text;
                    ((DBSqlServerManager)db).UseWindowsAuthentication = radioWinAuth.Checked;
                    InitializeTablesPanel();
                    break;
                case ImportSource.ODBC:
                    lblDBName.Text = string.Format("(ODBC) {0}", comboBoxOdbcDataSource.Text);
                    if (db == null) db = new DBMSAccessManager(comboBoxOdbcDataSource.Text);
                    InitializeTablesPanel();
                    break;
                case ImportSource.MySql:
                    lblDBName.Text = string.Format("(MySql) {0}:{1}", txtMySqlServer.Text, comboBoxMySqlDatabaseList.Text);
                    if (db == null) db = new DBMSAccessManager(txtMySqlServer.Text);
                    ((DBMySqlManager)db).DatabaseUserName = txtMySqlUserName.Text;
                    ((DBMySqlManager)db).DatabasePassword = txtMySqlPassword.Text;
                    ((DBMySqlManager)db).DatabaseName = comboBoxMySqlDatabaseList.Text;
                    InitializeTablesPanel();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Text Import

        Dictionary<string, DBColumn> textColumns;

        /// <summary>
        /// Control logic to handle navigation and initialization of flat file imports
        /// like Text and Excel.
        /// </summary>
        /// <param name="bNext"></param>
        private void ProcessPanelText(bool bNext)
        {
            switch (tabText.SelectedTab.Name.ToLower())
            {
                case "tabtextgeneral":
                    if (bNext) { tabText.SelectedTab = tabTextColumns; }
                    else
                    {
                        btnPrevious.Enabled = false;
                        panelWizMain.Visible = true;
                        panelWizText.Visible = false;
                    }
                    break;

                case "tabtextcolumns":
                    if (bNext) { tabText.SelectedTab = tabTextPreview; } else { tabText.SelectedTab = tabTextGeneral; }
                    break;
                case "tabtextpreview":
                    if (bNext) { ImportText(); } else { tabText.SelectedTab = tabTextColumns;  }
                    break;
            }
        }

        /// <summary>
        /// Initialize appropriate tabs as user navigates through the text import tabs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabText_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabText.SelectedTab.Name.ToLower())
            {
                case "tabtextgeneral":
                    lblFileName.Text = txtFileName.Text;
                    break;
                case "tabtextcolumns":
                    InitializeTextColumns();
                    break;
                case "tabtextpreview":
                    InitializeTextPreview();
                    break;
            }
        }

        /// <summary>
        /// Set up the ListBox showing columns found in the Text or Excel file
        /// being imported.
        /// </summary>
        protected void InitializeTextColumns()
        {
            UpdateTextManager();
            if (listBoxColumns.Items.Count == 0)
            {
                textColumns = db.GetColumns(txtFileName.Text);
                if (textColumns.Count == 0) return;

                foreach (var textColumn in textColumns)
                {
                    listBoxColumns.Items.Add(textColumn.Key);
                }
                PopulatePropertyGrid();
            }
        }

        /// <summary>
        /// Translate selected field separator into a useable character.
        /// </summary>
        /// <returns>Separator</returns>
        private char GetSeparator()
        {
            if (radioButtonComma.Checked) return ',';
            if (radioButtonPipe.Checked) return '|';
            if (radioButtonSemiColon.Checked) return ';';
            if (radioButtonSpace.Checked) return ' ';
            if (radioButtonTab.Checked) return '\t';
            return (Convert.ToChar(txtOtherDelimiter.Text));
        }

        /// <summary>
        /// Translate selected delimiter into a useable string.
        /// </summary>
        /// <returns>Delimiter</returns>
        private string GetDelimiter()
        {
            return comboBoxTextQualifier.Text == "(none)" ? string.Empty : comboBoxTextQualifier.Text;
        }

        /// <summary>
        /// Insure Import routines have current user selections.
        /// </summary>
        private void UpdateTextManager()
        {
            ((DBTextManager)db).FirstRowHasHeadings = checkBoxFirstRowContainsHeadings.Checked;
            ((DBTextManager)db).Delimiter = GetSeparator();
            ((DBTextManager)db).TextQualifier = GetDelimiter();
        }

        /// <summary>
        /// Create and initialize Property Grid settings for all columns.
        /// </summary>
        /// <param name="column"></param>
        private void PopulatePropertyGrid(string column = null)
        {
            if (ColumnSettings == null)
            {
                ColumnSettings = new Dictionary<string, ImportWizTextPropertySettings>();
                for (int i = 0; i < listBoxColumns.Items.Count; i++ )
                {
                    ImportWizTextPropertySettings ips = new ImportWizTextPropertySettings();
                    ips.Name = listBoxColumns.Items[i].ToString();
                    ips.ColumnDelimeter = GetSeparator().ToString();
                    ips.ColumnWidth = 255;
                    ips.Type = "TEXT";
                    ips.Exclude = false;
                    ColumnSettings.Add(ips.Name, ips);
                }
            }

            if (column == null) column = listBoxColumns.Items[0].ToString();
            listBoxColumns.SelectedIndex = 0;
            propertyGridColumns.SelectedObject = ColumnSettings[column];

        }

        /// <summary>
        /// Initialize the text preview panel with data from the first 100 rows in the file..
        /// </summary>
        protected void InitializeTextPreview()
        {
            StreamReader sr;
            DataTable dt;

            try
            {
                sr = new StreamReader(txtFileName.Text);
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Cannot Open File {0}\r\nError: {1}", txtFileName.Text, ex.Message));
                return;
            }

            dt = new DataTable();
            foreach (var cs in ColumnSettings)
            {
                DataColumn dc = new DataColumn(cs.Key);
                dc.ColumnName = cs.Value.Name;
                dt.Columns.Add(dc);
            }

            int count = 0;
            char delimiter = GetSeparator();
            string line;

            if (checkBoxFirstRowContainsHeadings.Checked) sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                string[] cols = ((DBTextManager)db).SplitLine(line, dt.Columns.Count, delimiter, comboBoxTextQualifier.Text);
                if (cols.Length > listBoxColumns.Items.Count) Array.Copy(cols, cols, listBoxColumns.Items.Count);
                dt.Rows.Add(cols);
                count++;
                if (count >= 100) break;
            }
            dataGridViewText.DataSource = dt;
            lblRowCount.Text = string.Format("Preview Rows 1-{0}", dataGridViewText.Rows.Count);
        }

        /// <summary>
        /// Detect Property Grid changes and update related UI elements as needed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void propertyGridColumns_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {

            GridItem p = (GridItem)e.ChangedItem;

            switch (p.PropertyDescriptor.Name)
            {
                case "Name":
                    string newName = ((ImportWizTextPropertySettings)propertyGridColumns.SelectedObject).Name;
                    if (!ValidateColumnName(newName))
                    {
                        ((ImportWizTextPropertySettings)propertyGridColumns.SelectedObject).Name = listBoxColumns.Items[listBoxColumns.SelectedIndex].ToString();
                        return;
                    }
                    listBoxColumns.Items[listBoxColumns.SelectedIndex] = ((ImportWizTextPropertySettings)propertyGridColumns.SelectedObject).Name;
                    break;

                case "Type":
                    string newType = ((ImportWizTextPropertySettings)propertyGridColumns.SelectedObject).Type;
                    if (Common.IsText(newType))
                    {
                        ((ImportWizTextPropertySettings)propertyGridColumns.SelectedObject).AutoIncrement = false;
                        //p.Parent.GridItems["Auto Increment"].PropertyDescriptor.IsReadOnly = true;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Detect new columns being selected and display the corresponding property grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iKey = listBoxColumns.SelectedIndex == -1 ? 0 : listBoxColumns.SelectedIndex;
            propertyGridColumns.SelectedObject = ColumnSettings[string.Format("Column {0}", iKey.ToString())];
            propertyGridColumns.Refresh();
        }

        #endregion

        #region Import From DB
        protected void ProcessPanelDB(bool bNext)
        {
            if (bNext)
            {
                ImportDB();
            }
            else
            {
                btnPrevious.Enabled = false;
                panelWizMain.Visible = true;
                panelWizDB.Visible = false;
            }
        }

        protected void InitializeTablesPanel()
        {
            panelWizMain.Visible = false;
            panelWizText.Visible = false;
            panelWizDB.Visible = true;
            btnPrevious.Enabled = true;

            dgvTables.Rows.Clear();

            //Create Custom Heading
            DataGridViewCellStyle dgItemStyle = dgvTables.Columns[1].DefaultCellStyle;
            DataGridViewCellStyle dgHeaderStyle = new DataGridViewCellStyle() { BackColor = SystemColors.ControlLight, ForeColor = SystemColors.ControlText, SelectionBackColor = SystemColors.ControlLight, SelectionForeColor = SystemColors.ControlText, Font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold, GraphicsUnit.Point, 0, false) };
            DataGridViewRow dgvrow = new DataGridViewRow();
            DataGridViewCheckBoxCell dgvCB = new DataGridViewCheckBoxCell();
            DataGridViewTextBoxCell dgvST = new DataGridViewTextBoxCell() { Value = "Source Table" };
            DataGridViewTextBoxCell dgvDT = new DataGridViewTextBoxCell() { Value = "Destination Table" };
            DataGridViewTextBoxCell dgvMT = new DataGridViewTextBoxCell() { Value = "Map" };
            dgvrow.Cells.AddRange(new DataGridViewCell[] { dgvCB, dgvST, dgvDT, dgvMT });
            dgvrow.Cells[1].ReadOnly = true;
            dgvrow.Cells[2].ReadOnly = true;
            dgvrow.Cells[3].ReadOnly = true;
            dgvTables.Rows.Add(dgvrow);
            dgvTables[0, 0].Style = dgHeaderStyle;
            dgvTables[1, 0].Style = dgHeaderStyle;
            dgvTables[2, 0].Style = dgHeaderStyle;
            dgvTables[3, 0].Style = dgHeaderStyle;
            

            schema = db.GetSchema();
            foreach (var table in schema.Tables)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                DataGridViewCheckBoxCell dgvCheckBox = new DataGridViewCheckBoxCell();
                DataGridViewTextBoxCell dgvSourceText = new DataGridViewTextBoxCell() { Value = table.Value.Name };
                DataGridViewComboBoxCell dgvComboDest = new DataGridViewComboBoxCell();
                PopulateTables(ref dgvComboDest, table.Value.Name);
                dgvComboDest.FlatStyle = FlatStyle.Standard;
                DataGridViewButtonCell dgvButton = new DataGridViewButtonCell() { Value = "Map" };
                dgvButton.FlatStyle = FlatStyle.Standard;
                dgvr.Cells.AddRange(new DataGridViewCell[] { dgvCheckBox, dgvSourceText, dgvComboDest, dgvButton });
                dgvr.Cells[1].ReadOnly = true;
                dgvr.Cells[2].ReadOnly = false;
                PopulateTables(ref dgvComboDest, table.Value.Name);
                dgvr.Cells[3].ReadOnly = true;
                dgvTables.Rows.Add(dgvr);
                //dgvButton.PositionEditingControl(true, true, new Rectangle(8, 2, 34, 12), new Rectangle(8, 2, 34, 12), dgvTables.Columns[3].DefaultCellStyle, false, false, false, false);

            }

        }

        protected void PopulateTables(ref DataGridViewComboBoxCell cmb, string tblName)
        {
            SchemaDefinition sd = DataAccess.SchemaDefinitions[DatabaseLocation];
            string newTable = tblName;
            cmb.Items.Clear();

            int i = 1;
            bool dup = true;
            while (dup)
            {
                dup = false;
                foreach (var table in sd.Tables)
                {
                    if (newTable.ToLower() == table.Key.ToLower())
                    {
                        dup = true;
                        newTable = string.Format("{0}_{1}", tblName, i.ToString());
                        i++;
                    }
                }
            }

            cmb.Items.Add(newTable);
            cmb.Value = newTable;
            foreach (var table in sd.Tables)
            {
                if (!Common.IsSystemTable(table.Key)) cmb.Items.Add(table.Key);
            }
        }

        private void dgvTables_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Looking only for CheckBox in top row
            if (e.ColumnIndex == 0 && e.RowIndex == 0)
            {
                for (int i = 1; i < dgvTables.RowCount; i++)
                {
                    ((DataGridViewCheckBoxCell)dgvTables.Rows[i].Cells[0]).Value = ((DataGridViewCheckBoxCell)dgvTables.Rows[e.RowIndex].Cells[e.ColumnIndex]).Value;
                }
            }
        }

        private void dgvTables_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex == 0)
            {
                dgvTables.EndEdit();
            }
        }

        #region SqlServer
        private void radioSqlServerAuth_CheckedChanged(object sender, EventArgs e)
        {
            txtDbPassword.Enabled = radioSqlServerAuth.Checked;
            txtDbUserName.Enabled = radioSqlServerAuth.Checked;
        }

        private void LoadCatalogs()
        {
            db = new DBSqlServerManager(txtServer.Text);
            ((DBSqlServerManager)db).DatabaseUserName = txtDbUserName.Text;
            ((DBSqlServerManager)db).DatabasePassword = txtDbPassword.Text;
            ((DBSqlServerManager)db).UseWindowsAuthentication = radioWinAuth.Checked;
            DBDatabaseList DbDl;

            try
            {
                DbDl = db.GetDatabaseList();
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Cannot retrieve Database list: \r\n{0}", ex.Message));
                return;
            }

            comboBoxDatabaseList.Items.Clear();
            foreach (var db in DbDl.Databases)
            {
                comboBoxDatabaseList.Items.Add(db.Key);
            }

        }

        private void comboBoxDatabaseList_DropDown(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtServer.Text)) LoadCatalogs();
        }
        #endregion

        #region ODBC
        private void LoadOdbcDataSources()
        {
            db = new DBOdbcManager(string.Empty);
            DBDatabaseList DbDl = db.GetDatabaseList();

            comboBoxOdbcDataSource.Items.Clear();
            foreach (var db in DbDl.Databases)
            {
                comboBoxOdbcDataSource.Items.Add(db.Key);
            }
            if (comboBoxOdbcDataSource.Items.Count > 0) comboBoxOdbcDataSource.Text = comboBoxOdbcDataSource.Items[0].ToString();
        }
        #endregion

        #region MySql

        private void LoadMySqlCatalogs()
        {
            db = new DBMySqlManager(txtMySqlServer.Text);
            ((DBMySqlManager)db).DatabaseUserName = txtMySqlUserName.Text;
            ((DBMySqlManager)db).DatabasePassword = txtMySqlPassword.Text;
            ((DBMySqlManager)db).DatabaseServer = txtMySqlServer.Text;
            DBDatabaseList DbDl;

            try
            {
                DbDl = db.GetDatabaseList();
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Cannot retrieve Database list: \r\n{0}", ex.Message));
                return;
            }

            comboBoxMySqlDatabaseList.Items.Clear();
            foreach (var db in DbDl.Databases)
            {
                comboBoxMySqlDatabaseList.Items.Add(db.Key);
            }

        }

        private void comboBoxMySqlDatabaseList_DropDown(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMySqlServer.Text)) LoadMySqlCatalogs();
        }
        #endregion


        #endregion

        #region Validation

        protected bool ValidateTextPanel()
        {
            if (string.IsNullOrEmpty(txtFileName.Text))
            {
                Common.ShowMsg("Please enter a file to import.");
                txtFileName.Focus();
                return false;
            }

            FileInfo f = new FileInfo(txtFileName.Text);
            if (!f.Exists)
            {
                Common.ShowMsg(string.Format("File {0} does not exist.", txtFileName.Text));
                txtFileName.Focus();
                return false;
            }

            return true;
        }

        protected bool ValidateSQLServerPanel()
        {
            if (string.IsNullOrEmpty(txtServer.Text))
            {
                Common.ShowMsg("Please enter a server name.");
                txtServer.Focus();
                return false;
            }

            if (radioSqlServerAuth.Checked)
            {
                if (string.IsNullOrEmpty(txtDbUserName.Text))
                {
                    Common.ShowMsg("Please enter your User Name.");
                    txtDbUserName.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(txtDbPassword.Text))
                {
                    Common.ShowMsg("Please enter your password.");
                    txtDbPassword.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(comboBoxDatabaseList.Text))
                {
                    Common.ShowMsg("Please select a database.");
                    comboBoxDatabaseList.Focus();
                    return false;
                }
            }

            return true;
        }

        protected bool ValidateODBCPanel()
        {
            if (string.IsNullOrEmpty(comboBoxOdbcDataSource.Text))
            {
                Common.ShowMsg("Please select an ODBC Data Source.");
                comboBoxOdbcDataSource.Focus();
                return false;
            }
            db = new DBOdbcManager(comboBoxOdbcDataSource.Text);
            ((DBOdbcManager)db).DatabaseUserName = txtOdbcUserName.Text;
            ((DBOdbcManager)db).DatabasePassword = txtOdbcPassword.Text;
            try
            {
                ((DBOdbcManager)db).TestOdbcConnection();
            }
            catch (Exception ex)
            { 
                Common.ShowMsg(string.Format("Cannot connect to this Data Source.\r\n{0}", ex.Message));
                comboBoxOdbcDataSource.Focus();
                return false;
            }
            return true;
        }

        protected bool ValidateMySqlPanel()
        {
            if (string.IsNullOrEmpty(txtMySqlServer.Text))
            {
                Common.ShowMsg("Please enter a server name.");
                txtServer.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtMySqlUserName.Text))
            {
                Common.ShowMsg("Please enter your User Name.");
                txtMySqlUserName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtMySqlPassword.Text))
            {
                Common.ShowMsg("Please enter your password.");
                txtMySqlPassword.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(comboBoxMySqlDatabaseList.Text))
            {
                Common.ShowMsg("Please select a database.");
                comboBoxMySqlDatabaseList.Focus();
                return false;
            }

            db = new DBMySqlManager(txtMySqlServer.Text);
            ((DBMySqlManager)db).DatabaseUserName = txtMySqlUserName.Text;
            ((DBMySqlManager)db).DatabasePassword = txtMySqlPassword.Text;
            try
            {
                ((DBMySqlManager)db).TestMySqlConnection();
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Cannot connect to this Database.\r\n{0}", ex.Message));
                comboBoxOdbcDataSource.Focus();
                return false;
            }
            return true;
        }

        protected bool ValidateColumnName(string ColName)
        {
            if (string.IsNullOrWhiteSpace(ColName))
            {
                Common.ShowMsg("Please enter a valid column name.");
                return false;
            }
            if (Common.IsKeyword(ColName))
            {
                Common.ShowMsg(string.Format("{0} is an SQLite reserved word.  Please enter a different column name.", ColName));
                return false;
            }
            if (Common.HasInvalidChars(ColName, out string chars))
            {
                Common.ShowMsg(string.Format("Please do not use any of these characters in your column name - \"{0}\".", chars));
                return false;
            }
            int useCount = 0;
            foreach (var col in ColumnSettings)
            {
                if (col.Value.Name.ToLower() == ColName.ToLower()) useCount++;
            }
            if (useCount> 1)
            {
                Common.ShowMsg("This column name is already in use. Please enter another column name.");
                return false;
            }
            return true;
        }

        #endregion

        #region Import Routines
        #region Import Text
        private void ImportText()
        {
            Dictionary<string, DBColumn> Columns = new Dictionary<string, DBColumn>();
            foreach (var columnSetting in ColumnSettings)
            {

                if (string.IsNullOrEmpty(columnSetting.Value.Name)) break;

                DBColumn column = new DBColumn();
                column.Name = columnSetting.Value.Name;
                column.DefaultValue = string.Empty;
                column.HasDefault = false;
                column.IsAutoIncrement = columnSetting.Value.AutoIncrement;
                column.IsKey = false;
                column.IsNullable = columnSetting.Value.AllowNulls;
                column.IsUnique = columnSetting.Value.Unique;
                column.IncludeInImport = !columnSetting.Value.Exclude;
                column.Type = Common.IsText(columnSetting.Value.Type) ? string.Format("{0}({1})", columnSetting.Value.Type, columnSetting.Value.ColumnWidth.ToString()) : columnSetting.Value.Type;
                column.DefaultValue = string.Empty;
                column.PrimaryKey = columnSetting.Value.PrimaryKey;
                Columns.Add(columnSetting.Value.Name, column);
            }
            db.Import(txtFileName.Text, cmbDestinationTable.Text, Columns);
        }

        #endregion

        #region ImportDB

        protected void ImportDB()
        {
            switch (SourceType)
            {
                case ImportSource.SQLite:
                    Import_SQLite();
                    break;
                case ImportSource.MSAccess:
                    Import_MSAccess();
                    break;
                case ImportSource.SQLServer:
                    Import_SqlServer();
                    break;
                case ImportSource.ODBC:
                    Import_SqlServer();
                    break;
                case ImportSource.MySql:
                    Import_SqlServer();
                    break;
                default:
                    Common.ShowMsg("Not yet implemented");
                    break;

            }
        }
        private void Import_SQLite()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                for (int i = 1; i < dgvTables.RowCount; i++)
                {
                    DataGridViewRow dgvr = dgvTables.Rows[i];
                    if (dgvr.Cells[0].Value != null && (bool)dgvr.Cells[0].Value == true)
                    {
                        toolStripStatusMsg.Text = string.Format("Importing {0}.", dgvr.Cells[1].Value.ToString());
                        db.Import(dgvr.Cells[1].Value.ToString(), dgvr.Cells[2].Value.ToString());
                    }
                }
                this.Cursor = Cursors.Default;
                toolStripStatusMsg.Text = "Import Complete.";
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Common.ShowMsg(string.Format("Import Failed: {0}", ex.Message));
                toolStripStatusMsg.Text = "Import Failed.";
            }
        }

        private void Import_MSAccess()
        {
            Import_SQLite();
        }

        private void Import_SqlServer()
        {
            Import_SQLite();
        }

        #endregion
        #endregion

        private void cmbSourceDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = (ComboBox)sender;

            switch (c.Text)
            {
                case "MS Access Database":
                    SourceType = ImportSource.MSAccess;
                    panelWizMainText.Visible = true;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    panelWizMainText.Dock = DockStyle.Fill;
                    lblPassword.Visible = true;
                    txtPassword.Visible = true;
                    break;

                case "SQLite Database":
                    SourceType = ImportSource.SQLite;
                    panelWizMainText.Visible = true;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    lblPassword.Visible = true;
                    txtPassword.Visible = true;
                    break;

                case "Comma Delimited Text File":
                    SourceType = ImportSource.Text;
                    panelWizMainText.Visible = true;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    lblPassword.Visible = false;
                    txtPassword.Visible = false;
                    break;

                case "Excel":
                    SourceType = ImportSource.Excel;
                    panelWizMainText.Visible = true;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    lblPassword.Visible = false;
                    txtPassword.Visible = false;
                    break;

                case "SQL Server":
                    SourceType = ImportSource.SQLServer;
                    panelWizMainDB.Visible = true;
                    panelWizMainText.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    break;

                case "ODBC":
                    SourceType = ImportSource.ODBC;
                    panelWizMainText.Visible = false;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = true;
                    panelWizMainMySql.Visible = false;
                    LoadOdbcDataSources();
                    break;

                case "MySQL":
                    SourceType = ImportSource.MySql;
                    panelWizMainMySql.Visible = true;
                    panelWizMainText.Visible = false;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    break;

                default:
                    break;
            }
            //cmbSourceTables.Items.Clear();
        }

        #region helpers

        /// <summary>
        /// Retrieve all Table Names from the source and place them into the Source ComboBox
        /// </summary>
        protected void LoadSourceTableNames()
        {
            switch (SourceType)
            {
                case ImportSource.SQLite:
                    db = new DBSQLiteManager(txtFileName.Text);
                    break;
                case ImportSource.MSAccess:
                    db = new DBMSAccessManager(txtFileName.Text);
                    break;
                default:
                    break;
            }
            try
            {
                schema = db.GetSchema();
            }
            catch (Exception ex)
            {
                Common.ShowMsg(ex.Message);
                return;
            }

            //cmbSourceTables.Items.Add("All");
            //cmbSourceTables.SelectedIndex = 0;
            foreach (var dbt in schema.Tables)
            {
                //cmbSourceTables.Items.Add(dbt.Value.Name);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Extension"></param>
        /// <param name="Filter"></param>
        /// <returns></returns>
        private void GetDefaultFileData(out string Title, out String Extension, out string Filter)
        {
            Title = "Open File";
            Filter = "All files (*.*)|*.*";
            Extension = string.Empty;

            switch (SourceType)
            {
                case ImportSource.MSAccess:
                    Title = "Open Access Database";
                    Extension = "accdb";
                    Filter = "All files (*.*)|*.*|Database Files (*.mdb;*.accdb)|*.mdb;*.accdb";
                    break;

                case ImportSource.SQLite:
                    Title = "Open SQLite Database";
                    Extension = "db";
                    Filter = "All files (*.*)|*.*|Database Files (*.db;*.db3)|*.db;*.db3";
                    break;

                case ImportSource.Text:
                    Title = "Open Text File";
                    Extension = "csv";
                    Filter = "All files (*.*)|*.*|Comma Delimited Files (*.csv;*.txt)|*.csv;*.txt";
                    break;
                    
                case ImportSource.Excel:
                    Title = "Open Excel Spreadsheet";
                    Extension = "xslx";
                    Filter = "All files (*.*)|*.*|Database Files (*.xls;*.xlsx)|*.xls;*.xlsx";
                    break;

                default:
                    break;
            }
            return;
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
