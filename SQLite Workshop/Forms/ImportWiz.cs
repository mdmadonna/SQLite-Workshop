using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.Config;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class ImportWiz : Form
    {

        ToolTip toolTip;
        ImportSource SourceType;
        DBSchema schema;
        DBManager db = null;
        string textmsg;
        string CatalogDB = string.Empty;
        bool bSaveImp;
        int maxTokens = 0;
        Dictionary <string, ImportWizTextPropertySettings> ColumnSettings;
        Dictionary<string, List<ImportData>> ImportHistory = new Dictionary<string, List<ImportData>>();

        public string DatabaseLocation { get; set; }

        private enum ImportSource
        {
            SQLite,
            MSAccess,
            SQLServer,
            MySql,
            Text,
            Excel,
            ODBC,
            SQL
        }

        public ImportWiz()
        {
            InitializeComponent();
        }

        private void ImportWiz_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            HouseKeeping(this, string.Format("Import Wizard {0}", DatabaseLocation));
            bool.TryParse(appSetting(CFG_SAVEIMPORT), out bSaveImp);

            toolStripStatusMsg.Text = string.Empty;
            btnPrevious.Enabled = false;

            //Load Import History
            string impHist = Decrypt(appSetting(CFG_IMPHISTORY));
            if (!string.IsNullOrEmpty(impHist)) ImportHistory = JsonSerializer.Deserialize<Dictionary<string, List<ImportData>>>(impHist);
            maxTokens = 6;

            this.Width = 650;
            this.Height = 390;
            lblWrkSheet.Top = 40;
            cmbWrkSheet.Top = 37;

            panelWizDBTop.Dock = DockStyle.Top;
            panelWizMain.Dock = DockStyle.Fill;
            panelWizMainDB.Dock = DockStyle.Fill;
            panelWizMainDB.Visible = false;
            panelWizMainText.Dock = DockStyle.Fill;
            panelWizMainODBC.Dock = DockStyle.Fill;
            panelWizMainMySql.Dock = DockStyle.Fill;
            panelWizText.Dock = DockStyle.Fill;
            panelWizDB.Dock = DockStyle.Fill;
            panelWizStatus.Dock = DockStyle.Fill;
            txtStatusMsg.Dock = DockStyle.Fill;
            dgvTables.Dock = DockStyle.Fill;

            panelWizMain.Visible = true;
            panelWizText.Visible = false;
            panelWizDB.Visible = false;
            panelWizStatus.Visible = false;
            panelWizSummary.Visible = false;
            cmbSourceDB.SelectedIndex = 0;

            LoadTableNames(cmbDestinationTable);
        }

        private void ImportWiz_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void txtStatusMsg_TextChanged(object sender, EventArgs e)
        {
            txtStatusMsg.SelectionLength = 0;
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

            if (panelWizStatus.Visible)
            {
                panelWizStatus.Visible = false;
                txtStatusMsg.Text = string.Empty;
                switch (SourceType)
                {
                    case ImportSource.Text:
                        panelWizText.Visible = true;
                        break;
                    default:
                        panelWizDB.Visible = true;
                        break;
                }
            }
        }
        private void comboBoxOdbcDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxOdbcDataSource_Leave(sender, e);
        }

        private void comboBoxOdbcDataSource_Leave(object sender, EventArgs e)
        {
            if (comboBoxOdbcDataSource.Text.ToLower() != CatalogDB)
            {
                db = null;
                
                if (string.IsNullOrEmpty(comboBoxOdbcDataSource.Text)) return;
                // See if pw is available
                if (!ImportHistory.ContainsKey(CFG_IMPODBC)) return;
                List<ImportData> li = ImportHistory[CFG_IMPODBC];
                string dbname = comboBoxOdbcDataSource.Text.ToLower();
                for (int i = 0; i < li.Count; i++)
                {
                    if (li[i].name == dbname)
                    {
                        txtMySqlUserName.Text = li[i].userid;
                        txtMySqlPassword.Text = li[i].password;
                        return;
                    }
                }
            }
        }

        private void cmbMySqlServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbMySqlServer_Leave(sender, e);
        }

        private void cmbMySqlServer_Leave(object sender, EventArgs e)
        {
            if (cmbMySqlServer.Text.ToLower() != CatalogDB)
            {
                db = null;
                comboBoxMySqlDatabaseList.Items.Clear();

                if (string.IsNullOrEmpty(cmbMySqlServer.Text)) return;
                // See if pw is available
                if (!ImportHistory.ContainsKey(CFG_IMPMYSQL)) return;
                List<ImportData> li = ImportHistory[CFG_IMPMYSQL];
                string dbname = cmbMySqlServer.Text.ToLower();
                for (int i = 0; i < li.Count; i++)
                {
                    if (li[i].name == dbname)
                    {
                        txtMySqlUserName.Text = li[i].userid;
                        txtMySqlPassword.Text = li[i].password;
                        return;
                    }
                }
            }
        }

        private void cmbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbServer_Leave(sender, e);
        }

        private void cmbServer_Leave(object sender, EventArgs e)
        {
            if (cmbServer.Text.ToLower() != CatalogDB)
            {
                db = null;
                comboBoxDatabaseList.Items.Clear();

                if (string.IsNullOrEmpty(cmbServer.Text)) return;
                // See if pw is available
                if (!ImportHistory.ContainsKey(CFG_IMPSQLSERVER)) return;
                List<ImportData> li = ImportHistory[CFG_IMPSQLSERVER];
                string dbname = cmbServer.Text.ToLower();
                for (int i = 0; i < li.Count; i++)
                {
                    if (li[i].name == dbname)
                    {
                        txtDbUserName.Text = li[i].userid;
                        txtDbPassword.Text = li[i].password;
                        radioWinAuth.Checked = Convert.ToBoolean(li[i].authtype);
                        radioSqlServerAuth.Checked = !radioWinAuth.Checked;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            toolStripStatusMsg.Text = string.Empty;

            if (btnNext.Text == "Finish")
            {
                this.Close();
                return;
            }

            if (panelWizMain.Visible) { ProcessPanelMain(true); btnNext.Enabled = true; return; }

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
            cmbFileName.Text = FindFile();
            if (string.IsNullOrEmpty(cmbFileName.Text)) return;

            //LoadSourceTableNames();
            return;
        }

        /// <summary>
        /// Find a file on local or network attached disk.
        /// </summary>
        /// <returns></returns>
        private string FindFile()
        {
            GetDefaultFileData(out string Title, out string DefaultExt, out string Filter);
            FileDialogInfo fi = new FileDialogInfo()
            {
                CheckFileExists = true,
                AddExtension = true,
                AutoUpgradeEnabled = true,
                Title = Title,
                DefaultExt = DefaultExt,
                Filter = Filter,
                FilterIndex = 2,
                RestoreDirectory = true,
                Multiselect = false,
                ShowReadOnly = false,
                ValidateNames = true
            };
            //openFile.InitialDirectory = cfg.appsetting(Config.CFG_LASTOPEN);
            return GetFileName(fi);
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
                    ImportFromText();
                    break;
                case ImportSource.Excel:
                    if (!ValidateTextPanel()) return;
                    ImportFromText();
                    break;
                case ImportSource.SQL:
                    if (!ValidateTextPanel()) return;
                    ImportFromSQL();
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
                    ShowMsg(NOTIMPLEMENTED, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            }
        }

        #region Import From Text or Excel
        /// <summary>
        /// Setup UI to Import from a Text file or Excel Spreadsheet
        /// </summary>
        protected void ImportFromText()
        {
            MainForm.mInstance.Cursor = Cursors.WaitCursor;
            lblFileName.Text = cmbFileName.Text;
            tabText.SelectedTab = tabTextGeneral;
            cmbDestinationTable.Text = Path.GetFileNameWithoutExtension(cmbFileName.Text);

            // Set up type specific inputs
            bool bTextType = SourceType == ImportSource.Text;
            if (bTextType)
            { db = new DBTextManager(cmbFileName.Text, DatabaseLocation); }
            else
            { db = new DBExcelManager(cmbFileName.Text, DatabaseLocation); LoadSheets(); }
            
            label7.Text = bTextType ? "Text Import" : "Excel Import"; 
            lblWrkSheet.Visible = !bTextType;
            cmbWrkSheet.Visible = !bTextType;
            lblTextQualifier.Visible = bTextType;
            comboBoxTextQualifier.Visible = bTextType;
            groupBoxDelimiter.Visible = bTextType;

            ColumnSettings = null;
            InitializeTextColumns();
            panelWizMain.Visible = false;
            panelWizText.Visible = true;
            panelWizText.Dock = DockStyle.Fill;
            panelWizDB.Visible = false;
            btnPrevious.Enabled = true;
            MainForm.mInstance.Cursor = Cursors.Default;
        }

        protected void LoadSheets()
        {
            DBDatabaseList SheetList = db.GetDatabaseList();
            if (SheetList.Databases.Count == 0) return;
            cmbWrkSheet.Items.Clear();
            foreach (KeyValuePair<string, DBInfo> ky in SheetList.Databases)
            {
                cmbWrkSheet.Items.Add(ky.Key);
            }
            cmbWrkSheet.SelectedIndex = 0;
        }

        #endregion

        #region Import from SQL
        protected void ImportFromSQL()
        {
            btnNext.Enabled = false;
            btnClose.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            db = new DBSqlManager(cmbFileName.Text, DatabaseLocation);
            db.StatusReport += StatusReport;
            bool rc = db.Import(cmbFileName.Text, null, null);
            db.StatusReport -= StatusReport;
            btnNext.Text = "Finish";
            btnNext.Enabled = true;
            btnClose.Enabled = true;
            MainForm.mInstance.LoadDB(DatabaseLocation, true);

            // Finallly update the historical list of impoprted files.
            if (rc) SaveHistory(db.ImportKey, cmbFileName.Text);
            this.Cursor = Cursors.Default;
        }
        #endregion

        /// <summary>
        /// Routines to manage import from a database.
        /// </summary>
        #region Import from Database

        /// <summary>
        /// Instantiate the appropriate DNManager class and initialize
        /// the first panel displayed when importing from a database.
        /// </summary>
        private void ImportFromDB()
        {
            switch (SourceType)
            {
                case ImportSource.SQLite:
                    lblDBName.Text = string.Format("(SQLite) {0}", cmbFileName.Text);
                    db = new DBSQLiteManager(cmbFileName.Text, DatabaseLocation, cmbFileName.Text, null, txtPassword.Text);
                    InitializeTablesPanel();
                    break;
                case ImportSource.MSAccess:
                    lblDBName.Text = string.Format("(MS Access) {0}", cmbFileName.Text);
                    db = new DBMSAccessManager(cmbFileName.Text, DatabaseLocation, cmbFileName.Text, null, txtPassword.Text);
                    InitializeTablesPanel();
                    break;
                case ImportSource.SQLServer:
                    lblDBName.Text = string.Format("(SQL Server) {0}:{1}", cmbServer.Text,comboBoxDatabaseList.Text);
                    if (db == null) db = new DBSqlServerManager(comboBoxDatabaseList.Text, DatabaseLocation, cmbServer.Text, txtDbUserName.Text, txtDbPassword.Text);
                    ((DBSqlServerManager)db).UseWindowsAuthentication = radioWinAuth.Checked;
                    InitializeTablesPanel();
                    break;
                case ImportSource.ODBC:
                    lblDBName.Text = string.Format("(ODBC) {0}", comboBoxOdbcDataSource.Text);
                    if (db == null) db = new DBOdbcManager(comboBoxOdbcDataSource.Text, DatabaseLocation);
                    InitializeTablesPanel();
                    break;
                case ImportSource.MySql:
                    lblDBName.Text = string.Format("(MySql) {0}:{1}", cmbMySqlServer.Text, comboBoxMySqlDatabaseList.Text);
                    if (db == null) db = new DBMySqlManager(cmbMySqlServer.Text, DatabaseLocation, comboBoxMySqlDatabaseList.Text, txtMySqlUserName.Text, txtMySqlPassword.Text);
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
                    if (bNext) 
                    { 
                        tabText.SelectedTab = tabTextColumns;
                    }
                    else
                    {
                        btnPrevious.Enabled = false;
                        panelWizMain.Visible = true;
                        panelWizText.Visible = false;
                    }
                    break;

                case "tabtextcolumns":
                    if (bNext) 
                    { 
                        tabText.SelectedTab = tabTextPreview; 
                    } 
                    else 
                    {
                        panelWizText.Visible = true;
                        panelWizStatus.Visible = false;
                        tabText.SelectedTab = tabTextGeneral; 
                    }
                    break;
                case "tabtextpreview":
                    if (bNext) 
                    {
                        panelWizText.Visible = false;
                        panelWizStatus.Visible = true;
                        btnNext.Enabled = false;
                        btnPrevious.Enabled = false;
                        txtStatusMsg.Text = IMP_START;
                        if (ImportText())
                        {
                            btnNext.Text = "Finish";
                            txtStatusMsg.Text += IMP_COMPLETE;
                            txtStatusMsg.SelectionLength = 0;
                        }
                        else
                        {
                            btnPrevious.Enabled = true;

                        }
                        btnNext.Enabled = true;
                    } 
                    else 
                    { 
                        tabText.SelectedTab = tabTextColumns;  
                    }
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
                    lblFileName.Text = cmbFileName.Text;
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
            UpdateTextManager(out bool DelimiterChanged);
            if (ColumnSettings != null && !DelimiterChanged) return;

            if (!InitTextListBox()) return; 
            PopulatePropertyGrid();
        }

        protected bool InitTextListBox()
        {
            listBoxColumns.Items.Clear();
            if (SourceType == ImportSource.Excel) ((DBExcelManager)db).WorkSheet = cmbWrkSheet.Text;
            textColumns = db.GetColumns(cmbFileName.Text);
            if (textColumns.Count == 0) return false;

            foreach (var textColumn in textColumns)
            {
                listBoxColumns.Items.Add(textColumn.Key);
            }
            return true;
        }

        /// <summary>
        /// If the checkbox changes after initialization, we need to reload the listbox
        /// and change the names in the propertygrid settings objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxFirstRowContainsHeadings_CheckedChanged(object sender, EventArgs e)
        {
            if (ColumnSettings == null) return;
            int idx = listBoxColumns.SelectedIndex;
            UpdateTextManager(out _);
            if (!InitTextListBox()) return;
            if (idx < listBoxColumns.Items.Count) listBoxColumns.SelectedIndex = idx;
            for (int i = 0; i < listBoxColumns.Items.Count; i++)
            {
                ColumnSettings[string.Format("col{0}", i.ToString().PadLeft(3, '0'))].Name = listBoxColumns.Items[i].ToString();
            }
        }

        /// <summary>
        /// Translate selected field separator into a useable character.
        /// </summary>
        /// <returns>Separator</returns>
        private char GetSeparator()
        {
            if (radioButtonComma.Enabled && radioButtonComma.Checked) return ',';
            if (radioButtonPipe.Enabled && radioButtonPipe.Checked) return '|';
            if (radioButtonSemiColon.Enabled && radioButtonSemiColon.Checked) return ';';
            if (radioButtonSpace.Enabled && radioButtonSpace.Checked) return ' ';
            if (radioButtonTab.Enabled && radioButtonTab.Checked) return '\t';
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
        private void UpdateTextManager(out bool newDelimiter)
        {
            newDelimiter = false;
            switch (SourceType)
            {
                case ImportSource.Text:
                    ((DBTextManager)db).FirstRowHasHeadings = checkBoxFirstRowContainsHeadings.Checked;
                    char delimiter = GetSeparator();
                    newDelimiter = delimiter != ((DBTextManager)db).Delimiter;
                    ((DBTextManager)db).Delimiter = delimiter;
                    ((DBTextManager)db).TextQualifier = GetDelimiter();
                    break;
                case ImportSource.Excel:
                    ((DBExcelManager)db).FirstRowHasHeadings = checkBoxFirstRowContainsHeadings.Checked;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Create and initialize Property Grid settings for all columns.
        /// </summary>
        /// <param name="column"></param>
        private void PopulatePropertyGrid(string column = null)
        {
            ColumnSettings = new Dictionary<string, ImportWizTextPropertySettings>();
            for (int i = 0; i < listBoxColumns.Items.Count; i++)
            {
                ImportWizTextPropertySettings ips = new ImportWizTextPropertySettings();
                string key = string.Format("col{0}", i.ToString().PadLeft(3, '0'));
                ips.Name = listBoxColumns.Items[i].ToString();
                ips.ColumnWidth = 255;
                ips.Type = "varchar";
                ips.Exclude = false;
                ips.AllowNulls = true;
                ColumnSettings.Add(key, ips);
            }

            listBoxColumns.SelectedIndex = 0;
            propertyGridColumns.SelectedObject = ColumnSettings[string.Format("col{0}", listBoxColumns.SelectedIndex.ToString().PadLeft(3, '0'))];
        }

        /// <summary>
        /// Initialize the text preview panel with data from the first 100 rows in the file..
        /// </summary>
        protected void InitializeTextPreview()
        {
            UpdateTextManager(out _);
            if (SourceType == ImportSource.Text)
            { ((DBTextManager)db).ColumnSettings = ColumnSettings; }
            else
            { ((DBExcelManager)db).ColumnSettings = ColumnSettings; }
            DataTable dt = db.PreviewData(string.Empty);
            if (dt == null) return;
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

            ImportWizTextPropertySettings iw = (ImportWizTextPropertySettings)propertyGridColumns.SelectedObject;
            iw.SetReadOnly("Name", false);
            iw.SetReadOnly("Type", false);
            iw.SetReadOnly("ColumnWidth", false);
            iw.SetReadOnly("Exclude", false);
            iw.SetReadOnly("PrimaryKey", false);
            iw.SetReadOnly("Unique", false);
            iw.SetReadOnly("AllowNulls", true);
            switch (p.PropertyDescriptor.Name)
            {
                case "Name":
                    string newName = iw.Name;
                    if (!ValidateColumnName(newName))
                    {
                        iw.Name = listBoxColumns.Items[listBoxColumns.SelectedIndex].ToString();
                        return;
                    }
                    listBoxColumns.Items[listBoxColumns.SelectedIndex] = iw.Name;
                    break;

                case "Type":
                    string newType = iw.Type;
                    if (IsNumber(newType))
                    {
                        iw.SetReadOnly("PrimaryKey", false);
                        iw.ColumnWidth = 0;
                        iw.SetReadOnly("ColumnWidth", true);
                        break;
                    }
                    if (IsText(newType))
                    {
                        iw.SetReadOnly("ColumnWidth", false);
                    }
                    break;
                case "Exclude":
                    iw.SetReadOnly("Name", iw.Exclude);
                    iw.SetReadOnly("Type", iw.Exclude);
                    iw.SetReadOnly("ColumnWidth", iw.Exclude);
                    iw.SetReadOnly("PrimaryKey", iw.Exclude);
                    iw.SetReadOnly("Unique", iw.Exclude);
                    iw.SetReadOnly("AllowNulls", iw.Exclude);
                    break;
                default:
                    break;
            }
            propertyGridColumns.Refresh();
        }

        /// <summary>
        /// Detect new columns being selected and display the corresponding property grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iKey = listBoxColumns.SelectedIndex == -1 ? 0 : listBoxColumns.SelectedIndex;
            propertyGridColumns.SelectedObject = ColumnSettings[string.Format("col{0}", iKey.ToString().PadLeft(3,'0'))];
            propertyGridColumns.Refresh();
        }


        #endregion

        #region Import From DB
        protected void ProcessPanelDB(bool bNext)
        {
            if (bNext)
            {
                panelWizDB.Visible = false;
                panelWizStatus.Visible = true;
                btnNext.Enabled = false;
                btnPrevious.Enabled = false;
                txtStatusMsg.Text = IMP_START;
                if (ImportDB())
                {
                    btnNext.Text = "Finish";
                }
                else
                {
                    btnPrevious.Enabled = true;
                }
                btnNext.Enabled = true;
                txtStatusMsg.Text += IMP_COMPLETE;
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
            DataGridViewTextBoxCell dgvPT = new DataGridViewTextBoxCell() { Value = "Preview" };
            dgvrow.Cells.AddRange(new DataGridViewCell[] { dgvCB, dgvST, dgvDT, dgvMT, dgvPT });

            dgvrow.Cells[1].ReadOnly = true;
            dgvrow.Cells[2].ReadOnly = true;
            dgvrow.Cells[3].ReadOnly = true;
            dgvrow.Cells[4].ReadOnly = true;
            dgvTables.Rows.Add(dgvrow);
            dgvTables[0, 0].Style = dgHeaderStyle;
            dgvTables[1, 0].Style = dgHeaderStyle;
            dgvTables[2, 0].Style = dgHeaderStyle;
            dgvTables[3, 0].Style = dgHeaderStyle;
            dgvTables[4, 0].Style = dgHeaderStyle;


            schema = db.GetSchema();
            if (schema.Tables == null) return;
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
                DataGridViewButtonCell dgvButtonPreview = new DataGridViewButtonCell() { Value = "Preview" };
                dgvButtonPreview.FlatStyle = FlatStyle.Standard;
                dgvr.Cells.AddRange(new DataGridViewCell[] { dgvCheckBox, dgvSourceText, dgvComboDest, dgvButton, dgvButtonPreview });
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
                if (!IsSystemTable(table.Key)) cmb.Items.Add(table.Key);
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

        private void dgvTables_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView)sender;

            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell && e.RowIndex >= 0)
            {
                string buttonText = ((DataGridViewButtonCell)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex]).Value.ToString().ToLower();
                switch (buttonText)
                {
                    case "map":
                        dgvTables_Map(dgv.Rows[e.RowIndex].Cells[1].Value.ToString(), dgv.Rows[e.RowIndex].Cells[2].Value.ToString());
                        break;
                    case "preview":
                        dgvTables_Preview(dgv.Rows[e.RowIndex].Cells[1].Value.ToString());
                        break;
                    default:
                    break;
                }
            }
        }

        private void dgvTables_Map(string tableName, string newTableName)
        {
            Mapping mp = new Mapping(tableName, newTableName)
            {
                db = db
            };
            mp.ShowDialog();
        }


        private void dgvTables_Preview(string tableName)
        {
            Preview p;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                DataTable dt = db.PreviewData(tableName);
                if (dt == null)
                {
                    ShowMsg(string.Format("Cannot read Table {0}\r\nError: {1}", tableName, db.LastError));
                    return;
                }
                p = new Preview();
                p.setPreview(tableName, dt);
                this.Cursor = Cursors.Default;
                p.Show();
            }
            finally
            {
                this.Cursor = Cursors.Default;
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
            db = new DBSqlServerManager(null, DatabaseLocation, cmbServer.Text, txtDbUserName.Text, txtDbPassword.Text);
            ((DBSqlServerManager)db).UseWindowsAuthentication = radioWinAuth.Checked;
            DBDatabaseList DbDl;

            try
            {
                DbDl = db.GetDatabaseList();
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Cannot retrieve Database list: \r\n{0}", ex.Message));
                return;
            }

            comboBoxDatabaseList.Items.Clear();
            foreach (var db in DbDl.Databases)
            {
                comboBoxDatabaseList.Items.Add(db.Key);
            }
            CatalogDB = cmbServer.Text.ToLower();
        }

        private void comboBoxDatabaseList_DropDown(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cmbServer.Text)) 
                if (comboBoxDatabaseList.Items.Count == 0 || cmbServer.Text.ToLower() != CatalogDB) LoadCatalogs();
        }
        #endregion

        #region ODBC
        private void LoadOdbcDataSources()
        {
            db = new DBOdbcManager(string.Empty, DatabaseLocation);
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
            db = new DBMySqlManager(null, DatabaseLocation, cmbMySqlServer.Text, txtMySqlUserName.Text, txtMySqlPassword.Text);
            DBDatabaseList DbDl;

            try
            {
                DbDl = db.GetDatabaseList();
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Cannot retrieve Database list: \r\n{0}", ex.Message));
                return;
            }

            comboBoxMySqlDatabaseList.Items.Clear();
            foreach (var db in DbDl.Databases)
            {
                comboBoxMySqlDatabaseList.Items.Add(db.Key);
            }
            CatalogDB = cmbMySqlServer.Text.ToLower();
        }

        private void comboBoxMySqlDatabaseList_DropDown(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(cmbMySqlServer.Text)) 
                if (comboBoxMySqlDatabaseList.Items.Count == 0 || cmbMySqlServer.Text.ToLower() != CatalogDB) LoadMySqlCatalogs();
        }
        #endregion


        #endregion

        #region Validation

        protected bool ValidateTextPanel()
        {
            if (string.IsNullOrEmpty(cmbFileName.Text))
            {
                ShowMsg("Please enter a file to import.");
                cmbFileName.Focus();
                return false;
            }

            FileInfo f = new FileInfo(cmbFileName.Text);
            if (!f.Exists)
            {
                ShowMsg(string.Format("File {0} does not exist.", cmbFileName.Text));
                cmbFileName.Focus();
                return false;
            }

            return true;
        }

        protected bool ValidateSQLServerPanel()
        {
            if (string.IsNullOrEmpty(cmbServer.Text))
            {
                ShowMsg("Please enter a server name.");
                cmbServer.Focus();
                return false;
            }

            if (radioSqlServerAuth.Checked)
            {
                if (string.IsNullOrEmpty(txtDbUserName.Text))
                {
                    ShowMsg("Please enter your User Name.");
                    txtDbUserName.Focus();
                    return false;
                }
                if (string.IsNullOrEmpty(txtDbPassword.Text))
                {
                    ShowMsg("Please enter your password.");
                    txtDbPassword.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(comboBoxDatabaseList.Text))
            {
                ShowMsg("Please select a database.");
                comboBoxDatabaseList.Focus();
                return false;
            }

            if (db == null)
            {
                db = new DBSqlServerManager(comboBoxDatabaseList.Text, DatabaseLocation, cmbServer.Text, txtDbUserName.Text, txtDbPassword.Text);
            }
            else
            {
                db.SourceDB = comboBoxDatabaseList.Text;
            }

            try
            {
                db.TestConnection();
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Cannot connect to this Database.\r\n{0}", ex.Message));
                comboBoxDatabaseList.Focus();
                return false;
            }


            return true;
        }

        protected bool ValidateODBCPanel()
        {
            if (string.IsNullOrEmpty(comboBoxOdbcDataSource.Text))
            {
                ShowMsg("Please select an ODBC Data Source.");
                comboBoxOdbcDataSource.Focus();
                return false;
            }
            db = new DBOdbcManager(comboBoxOdbcDataSource.Text, DatabaseLocation, null, txtOdbcUserName.Text, txtOdbcPassword.Text);
            try
            {
                db.TestConnection();
            }
            catch (Exception ex)
            { 
                ShowMsg(string.Format("Cannot connect to this Data Source.\r\n{0}", ex.Message));
                comboBoxOdbcDataSource.Focus();
                return false;
            }
            return true;
        }

        protected bool ValidateMySqlPanel()
        {
            if (string.IsNullOrEmpty(cmbMySqlServer.Text))
            {
                ShowMsg("Please enter a server name.");
                cmbMySqlServer.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtMySqlUserName.Text))
            {
                ShowMsg("Please enter your User Name.");
                txtMySqlUserName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtMySqlPassword.Text))
            {
                ShowMsg("Please enter your password.");
                txtMySqlPassword.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(comboBoxMySqlDatabaseList.Text))
            {
                ShowMsg("Please select a database.");
                comboBoxMySqlDatabaseList.Focus();
                return false;
            }

            if (db == null)
            {
                db = new DBMySqlManager(comboBoxMySqlDatabaseList.Text, DatabaseLocation, cmbMySqlServer.Text, txtMySqlUserName.Text, txtMySqlPassword.Text);
            }
            else
            {
                db.SourceDB = comboBoxMySqlDatabaseList.Text;
            }

            try
            {
                db.TestConnection();
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Cannot connect to this Database.\r\n{0}", ex.Message));
                comboBoxMySqlDatabaseList.Focus();
                return false;
            }
            return true;
        }

        protected bool ValidateColumnName(string ColName)
        {
            if (string.IsNullOrWhiteSpace(ColName))
            {
                ShowMsg("Please enter a valid column name.");
                return false;
            }
            if (IsKeyword(ColName))
            {
                ShowMsg(string.Format("{0} is an SQLite reserved word.  Please enter a different column name.", ColName));
                return false;
            }
            if (HasInvalidChars(ColName, out string chars))
            {
                ShowMsg(string.Format("Please do not use any of these characters in your column name - \"{0}\".", chars));
                return false;
            }
            int useCount = 0;
            foreach (var col in ColumnSettings)
            {
                if (col.Value.Name.ToLower() == ColName.ToLower()) useCount++;
            }
            if (useCount> 1)
            {
                ShowMsg("This column name is already in use. Please enter another column name.");
                return false;
            }
            return true;
        }

        #endregion

        #region Import Routines

        /// <summary>
        /// The DB Managers will periodically fire this event to relay current import status to the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void StatusReport(object sender, StatusEventArgs e)
        {
            string msg = SourceType == ImportSource.SQL ? "stmts" : "rows";
            switch (e.Status)
            {
                case ImportStatus.Starting:
                    textmsg = txtStatusMsg.Text;
                    break;
                case ImportStatus.InProgress:
                    // Causes too much flicker - I may come back to this later
                    //txtStatusMsg.Text = string.Format("{0}{1}", textmsg, e.RowCount.ToString());
                    toolStripStatusMsg.Text = string.Format("{0}{1} {2}", string.Empty, e.RowCount.ToString(), msg);
                    break;
                case ImportStatus.Failed:
                    txtStatusMsg.Text = string.Format("{0}{1}", textmsg, IMP_FAILED);
                    toolStripStatusMsg.Text = IMP_FAILED;
                    break;
                case ImportStatus.Complete:
                    txtStatusMsg.Text = string.Format("{0}{1} {2}{3}", textmsg, e.RowCount.ToString(), msg, Environment.NewLine);
                    toolStripStatusMsg.Text = string.Format("Import Complete. {0} {1}", e.RowCount.ToString(), msg);
                    break;
            }
            txtStatusMsg.SelectionLength = 0;
            Application.DoEvents();
        }

        #region Import Text/Excel
        /// <summary>
        /// Import a Text/Excel File
        /// </summary>
        /// <returns></returns>
        private bool ImportText()
        {
            // Create List Of Columns
            Dictionary<string, DBColumn> Columns = new Dictionary<string, DBColumn>();
            foreach (var columnSetting in ColumnSettings)
            {

                if (string.IsNullOrEmpty(columnSetting.Value.Name)) break;

                DBColumn column = new DBColumn
                {
                    Name = columnSetting.Value.Name,
                    DefaultValue = string.Empty,
                    HasDefault = false,
                    IsKey = false,
                    IsNullable = columnSetting.Value.AllowNulls,
                    IsUnique = columnSetting.Value.Unique,
                    IncludeInImport = !columnSetting.Value.Exclude,
                    Type = IsText(columnSetting.Value.Type) ? string.Format("{0}({1})", columnSetting.Value.Type, columnSetting.Value.ColumnWidth.ToString()) : columnSetting.Value.Type
                };
                column.DefaultValue = string.Empty;
                column.PrimaryKey = columnSetting.Value.PrimaryKey;
                Columns.Add(columnSetting.Value.Name, column);
            }

            // Import the Text File
            toolStripStatusMsg.Text = string.Format("Importing {0}.", cmbFileName.Text);
            txtStatusMsg.Text += string.Format("Importing {0}", cmbDestinationTable.Text.PadRight(50, ' '));
            db.StatusReport += StatusReport;
            bool rc = db.Import(cmbFileName.Text, cmbDestinationTable.Text, Columns);
            db.StatusReport -= StatusReport;

            // Finallly update the historical list of imported files.
            if (rc) SaveHistory(db.ImportKey, cmbFileName.Text);
            return rc;
        }
        #endregion

        #region ImportDB

        protected bool ImportDB()
        {
            try
            {
                switch (SourceType)
                {
                    case ImportSource.SQLite:
                        return Import_SQLite();
                    case ImportSource.MSAccess:
                        return Import_MSAccess();
                    case ImportSource.SQLServer:
                        return Import_SqlServer();
                    case ImportSource.ODBC:
                        return Import_ODBC();
                    case ImportSource.MySql:
                        return Import_MySql();
                    default:
                        ShowMsg("Not yet implemented");
                        return false;
                }
            }
            finally { txtStatusMsg.SelectionLength = 0; }
        }
        private bool Import_SQLite()
        {
            this.Cursor = Cursors.WaitCursor;
            db.StatusReport += StatusReport;
            try
            {
                for (int i = 1; i < dgvTables.RowCount; i++)
                {
                    DataGridViewRow dgvr = dgvTables.Rows[i];
                    if (dgvr.Cells[0].Value != null && (bool)dgvr.Cells[0].Value == true)
                    {
                        toolStripStatusMsg.Text = string.Format("Importing {0}.", dgvr.Cells[1].Value.ToString());
                        txtStatusMsg.Text += string.Format("Importing {0}", dgvr.Cells[1].Value.ToString().PadRight(50, ' '));
                        db.Import(dgvr.Cells[1].Value.ToString(), dgvr.Cells[2].Value.ToString());
                    }
                }
                toolStripStatusMsg.Text = "Import Complete.";
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Import Failed: {0}", ex.Message));
                toolStripStatusMsg.Text = "Import Failed.";
                txtStatusMsg.Text += string.Format("{0}{1}{2}", textmsg, "Import Failed", Environment.NewLine);
                return false;
            }
            finally
            {
                db.StatusReport -= StatusReport;
                this.Cursor = Cursors.Default;
            }

            SaveHistory(db.ImportKey,  db.SourceServer, db.SourceUserName, db.SourcePassword, db.UseWindowsAuthentication.ToString());
            return true;
        }

        private bool Import_MSAccess()
        {
            return Import_SQLite();
        }

        private bool Import_SqlServer()
        {
            return Import_SQLite();
        }

        private bool Import_ODBC()
        {
            return Import_SQLite();
        }

        private bool Import_MySql()
        {
            return Import_SQLite();
        }

        #endregion
        #endregion

        #region helpers

        /// <summary>
        /// Load most recently used list of files or dbs.
        /// </summary>
        /// <param name="iType">Name of Key holding the import history.</param>
        /// <param name="cmb">ComboBox to load</param>
        /// <param name="impdata">Selected ComboBox Item</param>
        private void LoadHistory(string iType, ComboBox cmb)
        {
            cmb.Items.Clear();
            if (!bSaveImp) return;
            if (!ImportHistory.ContainsKey(iType)) return;
            List<ImportData> li = ImportHistory[iType];
            foreach (ImportData id in li)
            {
                cmb.Items.Add(id.name);
            }
        }
        private void SaveHistory(string iType, string Name, string UserID = null, string Password = null, string Auth = null)
        {
            ImportData id = new ImportData()
            {
                lastuse = DateTime.Now,
                name = Name.ToLower(),
                userid = UserID?.ToLower(),
                password = Password,
                authtype = Auth
            };

            int i;
            if (ImportHistory.ContainsKey(iType))
            {
                List<ImportData> li = ImportHistory[iType];
                for (i = 0; i < li.Count; i++)
                {
                    if (li[i].name == id.name)
                    {
                        li[i] = id;
                        break;
                    }
                }
                if (i == li.Count) li.Add(id);
                li = li.OrderByDescending(s => s.lastuse).ToList(); 
                while (li.Count > maxTokens) { li.RemoveAt(maxTokens); }
                ImportHistory[iType] = li;
            }
            else
            {
                List<ImportData> l = new List<ImportData>
                {
                    id
                };
                ImportHistory.Add(iType, l);
            }
            string iHist = JsonSerializer.Serialize<Dictionary<string, List<ImportData>>>(ImportHistory);
            saveSetting(CFG_IMPHISTORY, Encrypt(iHist));
        }

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
                    LoadHistory(CFG_IMPMSACCESS, cmbFileName);
                    break;

                case "SQLite Database":
                    SourceType = ImportSource.SQLite;
                    panelWizMainText.Visible = true;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    lblPassword.Visible = true;
                    txtPassword.Visible = true;
                    LoadHistory(CFG_IMPSQLITE, cmbFileName);
                    break;

                case "Delimited Text File":
                    SourceType = ImportSource.Text;
                    panelWizMainText.Visible = true;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    lblPassword.Visible = false;
                    txtPassword.Visible = false;
                    LoadHistory(CFG_IMPTEXT, cmbFileName);
                    break;

                case "Excel":
                    SourceType = ImportSource.Excel;
                    panelWizMainText.Visible = true;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    lblPassword.Visible = false;
                    txtPassword.Visible = false;
                    LoadHistory(CFG_IMPEXCEL, cmbFileName);
                    break;

                case "SQL Server":
                    SourceType = ImportSource.SQLServer;
                    panelWizMainDB.Visible = true;
                    panelWizMainText.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    LoadHistory(CFG_IMPSQLSERVER, cmbServer);
                    break;

                case "ODBC":
                    SourceType = ImportSource.ODBC;
                    panelWizMainText.Visible = false;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = true;
                    panelWizMainMySql.Visible = false;
                    LoadOdbcDataSources();
                    LoadHistory(CFG_IMPODBC, cmbFileName);
                    break;

                case "MySQL":
                    SourceType = ImportSource.MySql;
                    panelWizMainMySql.Visible = true;
                    panelWizMainText.Visible = false;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    LoadHistory(CFG_IMPMYSQL, cmbMySqlServer);
                    break;

                case "SQL":
                    SourceType = ImportSource.SQL;
                    panelWizMainText.Visible = true;
                    panelWizMainDB.Visible = false;
                    panelWizMainODBC.Visible = false;
                    panelWizMainMySql.Visible = false;
                    lblPassword.Visible = false;
                    txtPassword.Visible = false;
                    LoadHistory(CFG_IMPSQL, cmbFileName);
                    break;

                default:
                    break;
            }
        }


        private void cmbFileName_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbFileName_Leave(sender, e);
        }
        /// <summary>
        /// The input has been changed. If importing from SQLite or MSAccess, look for 
        /// password in History
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbFileName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cmbFileName.Text)) return;
            if (SourceType == ImportSource.SQLite || SourceType == ImportSource.MSAccess)
            {
                // See if pw is available
                string impkey = SourceType == ImportSource.SQLite ? CFG_IMPSQLITE : CFG_IMPMSACCESS;
                if (!ImportHistory.ContainsKey(impkey)) return;
                List<ImportData> li = ImportHistory[impkey];
                string db = cmbFileName.Text.ToLower();
                for (int i = 0; i < li.Count; i++)
                {
                    if (li[i].name == db)
                    {
                        txtPassword.Text = li[i].password;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Common routine used to re-initialize text file columns when the delimiter
        /// is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_Click(object sender, EventArgs e)
        {
            InitializeTextColumns();
        }

        /// <summary>
        /// Delimiter separating values in text/csv file has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtOtherDelimiter_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOtherDelimiter.Text))
            {
                setRadioButtons(true);
            }
            else
            {
                setRadioButtons(false);
            }
            InitializeTextColumns();
        }

        /// <summary>
        /// Enable/Disable Text File Delimiter buttons
        /// </summary>
        /// <param name="enabled"></param>
        private void setRadioButtons(bool enabled)
        {
            radioButtonComma.Enabled = enabled;
            radioButtonPipe.Enabled = enabled;
            radioButtonSemiColon.Enabled = enabled;
            radioButtonSpace.Enabled = enabled;
            radioButtonTab.Enabled = enabled;
        }

        /// <summary>
        /// Retrieve all Table Names from the source and place them into the Source ComboBox
        /// </summary>
        protected void LoadSourceTableNames()
        {
            switch (SourceType)
            {
                case ImportSource.SQLite:
                    db = new DBSQLiteManager(cmbFileName.Text, DatabaseLocation, cmbFileName.Text, null, txtPassword.Text);
                    break;
                case ImportSource.MSAccess:
                    db = new DBMSAccessManager(cmbFileName.Text, DatabaseLocation, cmbFileName.Text, null, txtPassword.Text);
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
                ShowMsg(ex.Message);
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
                    Filter = "All files (*.*)|*.*|Text Files (*.csv;*.txt)|*.csv;*.txt";
                    break;
                    
                case ImportSource.Excel:
                    Title = "Open Excel Spreadsheet";
                    Extension = "xslx";
                    Filter = "All files (*.*)|*.*|Database Files (*.xls;*.xlsx)|*.xls;*.xlsx";
                    break;

                case ImportSource.SQL:
                    Title = "Open SQL File";
                    Extension = "sql";
                    Filter = "All files (*.*)|*.*|SQL Files (*.sql)|*.sql";
                    break;

                default:
                    break;
            }
            return;
        }


        #endregion

    }
}
