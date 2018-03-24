using System;
using System.Collections;
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
    public partial class ExecuteForm : Form
    {
        internal SQLType execType;
        internal TreeNode TargetNode;
        MainForm m;

        internal string DatabaseLocation { get; set; }
        SchemaDefinition sd;

        bool bActionApproved;
        internal bool bActionComplete;

        #region Messages
        #region Form Header
        const string TITLE_TRUNCATE             = "Truncate Table";
        const string TITLE_DROP                 = "Drop Table";
        const string TITLE_RENAME               = "Rename Table";
        const string TITLE_COMPRESS             = "Compress Database";
        const string TITLE_ENCRYPT              = "Encrypt Database";
        const string TITLE_BACKUP               = "Backup Database";
        const string TITLE_CLONE                = "Clone Database";
        const string TITLE_DELETEINDEX          = "Delete Index";
        const string TITLE_DELETEINDEXES        = "Delete All Indexes";
        const string TITLE_DELETEVIEW           = "Delete View";
        const string TITLE_REBUILDINDEX         = "Rebuild Index";
        const string TITLE_REBUILDALLINDEXES    = "Rebuild All Indexes";
        const string TITLE_DELETETRIGGER        = "Delete Trigger";
        #endregion

        #region Info Messages
        const string TRUNCATEWARNING        = "WARNING!!  This action will delete all rows from {0} in Database {1}.  Once deleted, these rows cannot be recovered.";
        const string DROPWARNING            = "WARNING!!  This action will delete Table {0} from Database {1}.  Once deleted, this table cannot be recovered.";
        const string RENAMEWARNING          = "WARNING!!  This action will rename Table {0} in Database {1}. Any Views that refer to this table will need to be recreated after the table is renamed.  Additionally, any Triggers that execute statements that refer to this table may need to be recreated.";
        const string COMPRESSWARNING        = "WARNING!!  This action will compress (vacuum) Database {0} to reorganize and recover unused space. Depending on the size of the database, this may take a long time.";
        const string ENCRYPTWARNING         = "WARNING!!  This action will Encrypt Database {0}. Encryption may not be supported on all platforms or by all interfaces - use it at your own risk. Enter blanks to remove encryption.";
        const string BACKUPWARNING          = "WARNING!!  This action will Backup {0}. Depending on the size of your database, this may take some time.";
        const string CLONEWARNING           = "WARNING!!  This action will Clone {0}. The cloned database will contain all data structures found in this database but no data will be copied.";
        const string DELALLINDEXWARNING     = "WARNING!!  This action will Delete all indexes on table {0}. ";
        const string DELINDEXWARNING        = "WARNING!!  This action will Delete index {0}.";
        const string DELVIEWWARNING         = "WARNING!!  This action will Delete View {0}. Once deleted, this view cannot be recovered.";
        const string REINDEXWARNING         = "WARNING!!  This action will Rebuild index {0}.";
        const string REINDEXALLWARNING      = "WARNING!!  This action will Rebuild all indexes for table {0}.";
        const string DELTRIGGERWARNING      = "WARNING!!  This action will Delete Trigger {0}. Once deleted, this trigger cannot be recovered.";

        const string EXECCONTINUE           = "Press Yes to continue or Cancel to terminate.";
        #endregion
        #endregion

        public ExecuteForm()
        {
            InitializeComponent();
            txtMessage.Text = string.Empty;
            toolStripExecutionStatus.Text = string.Empty;
            lblTxtInfo.Text = string.Empty;
            txtInfo.Visible = false;
            m = MainForm.mInstance;
        }

        private void ExecuteForm_Load(object sender, EventArgs e)
        {
            DatabaseLocation = MainForm.mInstance.CurrentDB;
            sd = DataAccess.SchemaDefinitions[DatabaseLocation];

            lblFormHeading.Text = string.Empty;
            lblError.Text = string.Empty;
            bActionApproved = false;
            bActionComplete = false;
            statusStrip.ShowItemToolTips = true;
            ExecuteCommand();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            bActionApproved = true;
            ExecuteCommand();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected void ExecuteCommand()
        {
            switch (execType)
            {
                case SQLType.SQLTruncate:
                    lblFormHeading.Text = TITLE_TRUNCATE;
                    TruncateTable();
                    break;
                case SQLType.SQLDrop:
                    lblFormHeading.Text = TITLE_DROP;
                    DropTable();
                    break;
                case SQLType.SQLRename:
                    lblFormHeading.Text = TITLE_RENAME;
                    RenameTable();
                    break;
                case SQLType.SQLCompress:
                    lblFormHeading.Text = TITLE_COMPRESS;
                    VacuumDB();
                    break;
                case SQLType.SQLEncrypt:
                    lblFormHeading.Text = TITLE_ENCRYPT;
                    EncryptDB();
                    break;
                case SQLType.SQLBackup:
                    lblFormHeading.Text = TITLE_BACKUP;
                    BackupDB();
                    break;
                case SQLType.SQLClone:
                    lblFormHeading.Text = TITLE_CLONE;
                    CloneDB();
                    break;
                case SQLType.SQLDeleteIndex:
                    lblFormHeading.Text = TITLE_DELETEINDEX;
                    DeleteIndex();
                    break;
                case SQLType.SQLDeleteAllIndexes:
                    lblFormHeading.Text = TITLE_DELETEINDEXES;
                    DeleteAllIndexes();
                    break;
                case SQLType.SQLRebuildIndex:
                    lblFormHeading.Text = TITLE_REBUILDINDEX;
                    RebuildIndex();
                    break;
                case SQLType.SQLRebuildAllIndexes:
                    lblFormHeading.Text = TITLE_REBUILDALLINDEXES;
                    RebuildAllIndexes();
                    break;
                case SQLType.SQLDeleteView:
                    lblFormHeading.Text = TITLE_DELETEVIEW;
                    DeleteView();
                    break;
                case SQLType.SQLDeleteTrigger:
                    lblFormHeading.Text = TITLE_DELETETRIGGER;
                    DeleteTrigger();
                    break;
                default:
                    break;
            }
        }

        #region Truncate Table
        private void TruncateTable()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(TRUNCATEWARNING, TargetNode.Text, DatabaseLocation));
                return;
            }

            string SqlStatement = string.Format("DELETE FROM \"{0}\";", TargetNode.Text);
            string SuccessMessage = string.Format(Common.OK_RECORDSAFFECTED, "%count%");
            string ErrorMessage = Common.ERR_SQL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }
        #endregion

        #region Drop Table
        private void DropTable()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(DROPWARNING, TargetNode.Text, DatabaseLocation));
                return;
            }

            string SqlStatement = string.Format("DROP TABLE \"{0}\";", TargetNode.Text);
            string SuccessMessage = string.Format(Common.OK_TBLDELETE, TargetNode.Text);
            string ErrorMessage = Common.ERR_SQL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }
        #endregion

        #region Rename Table

        private void RenameTable()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(RENAMEWARNING, TargetNode.Text, DatabaseLocation));
                txtInfo.Visible = true;
                lblTxtInfo.Text = "New Table Name:";
                txtInfo.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtInfo.Text))
            {
                txtInfo.Focus();
                toolStripExecutionStatus.Text = "Enter a valid Table Name.";
                return;
            }

            string SqlStatement = string.Format("ALTER TABLE \"{0}\" RENAME TO \"{1}\";", TargetNode.Text, txtInfo.Text);
            string SuccessMessage = string.Format(Common.OK_RENAME, TargetNode.Text);
            string ErrorMessage = Common.ERR_RENAMEFAIL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }
        #endregion

        #region Compress Database

        /// <summary>
        /// Compress (Vacuum) the current database.
        /// </summary>
        private void VacuumDB()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(COMPRESSWARNING, DatabaseLocation));
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            string SqlStatement = "Vacuum";
            string SuccessMessage = string.Format(Common.OK_VACUUM, DatabaseLocation);
            string ErrorMessage = Common.ERR_VACUUMFAIL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);

            // A database Vacuum may cause rowids to change under some circumstances.  If any
            // tables are currently being edited, reload the data to insure rowids are
            // consistent.
            if (MainForm.mInstance.tabMain.TabPages.Count > 0)
            {
                foreach (TabPage tb in MainForm.mInstance.tabMain.TabPages)
                {
                    foreach (Control c in tb.Controls)
                    {
                        if (c.GetType().Equals(typeof(DBEditorTabControl)))
                        {
                            ((DBEditorTabControl)c).VacuumCompleted();
                        }
                    }
                }
            }
            this.Cursor = Cursors.Default;
            return;
        }
        #endregion

        #region Encrypt Database


        private void EncryptDB()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(ENCRYPTWARNING, DatabaseLocation));
                txtInfo.Visible = true;
                lblTxtInfo.Text = "Password:";
                txtInfo.Focus();
                return;
            }

            string SqlStatement = string.Empty;
            string SuccessMessage = string.Format(Common.OK_ENCRYPT, DatabaseLocation);
            string ErrorMessage = Common.ERR_ENCRYPTFAILED;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;           
        }
        #endregion

        #region Backup Database
        private void BackupDB()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(BACKUPWARNING, DatabaseLocation));
                txtInfo.Visible = true;
                lblTxtInfo.Text = "Backup File:";
                txtInfo.Width -= 40;
                Button btnFileDialog = new Button();
                btnFileDialog.Text = "...";
                btnFileDialog.Height = txtInfo.Height;
                btnFileDialog.Width = 30;
                panelFill.Controls.Add(btnFileDialog);
                btnFileDialog.Top = txtInfo.Top;
                btnFileDialog.Left = txtInfo.Left + txtInfo.Width + 10;
                btnFileDialog.Click += btnFileFind_Click;
                txtInfo.Focus();
                return;
            }

            if (!ValidateBackup()) return;

            string SqlStatement = txtInfo.Text;
            string SuccessMessage = string.Format(Common.OK_ENCRYPT, DatabaseLocation);
            string ErrorMessage = Common.ERR_BACKUPFAILED;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }

        protected bool ValidateBackup()
        {
            string BackupFile = txtInfo.Text;

            if (string.IsNullOrEmpty(BackupFile))
            {
                lblError.Text = Common.ERR_FILEENTRY;
                txtInfo.Focus();
                return false;
            }

            FileInfo f = new FileInfo(BackupFile);
            if (!f.Exists) return true;

            // Let's make sure it's OK to overwrite and let's insure we're writing on top of the same file
            string UNCSource;
            string UNCTarget;

            Uri uriDB = new Uri(DatabaseLocation);
            UNCSource = uriDB.IsUnc ? DatabaseLocation : Common.GetUniversalName(DatabaseLocation);
            if (string.IsNullOrEmpty(UNCSource)) UNCSource = DatabaseLocation;
            uriDB = new Uri(txtInfo.Text);
            UNCTarget = uriDB.IsUnc ? txtInfo.Text : Common.GetUniversalName(txtInfo.Text);
            if (string.IsNullOrEmpty(UNCTarget)) UNCTarget = txtInfo.Text;

            if (UNCSource.ToLower() == UNCTarget.ToLower())
            {
                Common.ShowMsg(Common.ERR_BACKUPSAMEFILE);
                return false;
            }

            DialogResult result = Common.ShowMsg(Common.MSG_FILEEXISTS, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) { f.Delete(); }
            else { return false; }
            
            return true;
        }
        #endregion

        #region Clone Database
        /// <summary>
        /// Create a structural copy of the current database without population any rows.
        /// </summary>
        private void CloneDB()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(CLONEWARNING, DatabaseLocation));
                txtInfo.Visible = true;
                lblTxtInfo.Text = "Cloned File:";
                txtInfo.Width -= 40;
                Button btnFileDialog = new Button();
                btnFileDialog.Text = "...";
                btnFileDialog.Height = txtInfo.Height;
                btnFileDialog.Width = 30;
                panelFill.Controls.Add(btnFileDialog);
                btnFileDialog.Top = txtInfo.Top;
                btnFileDialog.Left = txtInfo.Left + txtInfo.Width + 10;
                btnFileDialog.Click += btnFileFind_Click;
                txtInfo.Focus();
                return;
            }

           
            if (!ValidateClone()) return;

            btnExecute.Enabled = false;
            SQLiteErrorCode returnCode;

            bool bresult = DataAccess.CreateDB(txtInfo.Text);
            if (!bresult)
            {
                GenerateErrorMessage(Common.ERR_CLONEFAILED,  0);
                return;
            }

            string SqlStatement = "Select * FROM sqlite_master Where Type = \"table\"";
            DataTable dtMaster = DataAccess.ExecuteDataTable(DatabaseLocation, SqlStatement, out returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                KillFile(txtInfo.Text);
                GenerateErrorMessage(Common.ERR_CLONEFAILED, returnCode);
                return;
            }

            ArrayList sqlSave = new ArrayList();
            foreach (DataRow dr in dtMaster.Rows)
            {
                if (!Common.IsSystemTable(dr["tbl_name"].ToString()))
                {
                    int iresult = DataAccess.ExecuteNonQuery(txtInfo.Text, dr["sql"].ToString(), out returnCode);
                    if (iresult < 0 || returnCode != SQLiteErrorCode.Ok) sqlSave.Add(dr["sql"].ToString());
                }
            }

            foreach (string szSql in sqlSave)
            {
                int iresult = DataAccess.ExecuteNonQuery(txtInfo.Text, szSql, out returnCode);
                {
                    GenerateErrorMessage(Common.ERR_CLONEFAILED, returnCode);
                    KillFile(txtInfo.Text);
                    return;
                }
            }

            SqlStatement = "Select * FROM sqlite_master Where Type != \"table\"";
            dtMaster = DataAccess.ExecuteDataTable(DatabaseLocation, SqlStatement, out returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                GenerateErrorMessage(Common.ERR_CLONEFAILED, returnCode);
                KillFile(txtInfo.Text);
                return;
            }

            foreach (DataRow dr in dtMaster.Rows)
            {
                int iresult = DataAccess.ExecuteNonQuery(txtInfo.Text, dr["sql"].ToString(), out returnCode);
                if (iresult < 0 || returnCode != SQLiteErrorCode.Ok)
                {
                    GenerateErrorMessage(Common.ERR_CLONEFAILED, returnCode);
                    KillFile(txtInfo.Text);
                    return;
                }
            }

            toolStripExecutionStatus.Text = "Clone Ok";
            toolStripExecutionStatus.ToolTipText = string.Format(Common.OK_CLONE, DatabaseLocation, txtInfo.Text);
            btnCancel.Text = "Close";
            bActionComplete = true;
        }

        protected bool ValidateClone()
        {
            string ClonedFile = txtInfo.Text;

            if (string.IsNullOrEmpty(ClonedFile))
            {
                lblError.Text = Common.ERR_FILEENTRY;
                txtInfo.Focus();
                return false;
            }

            FileInfo f = new FileInfo(ClonedFile);
            if (!f.Exists) return true;

            // Let's make sure it's OK to overwrite and let's insure we're writing on top of the same file
            string UNCSource;
            string UNCTarget;

            Uri uriDB = new Uri(DatabaseLocation);
            UNCSource = uriDB.IsUnc ? DatabaseLocation : Common.GetUniversalName(DatabaseLocation);
            if (string.IsNullOrEmpty(UNCSource)) UNCSource = DatabaseLocation;
            uriDB = new Uri(txtInfo.Text);
            UNCTarget = uriDB.IsUnc ? txtInfo.Text : Common.GetUniversalName(txtInfo.Text);
            if (string.IsNullOrEmpty(UNCTarget)) UNCTarget = txtInfo.Text;

            if (UNCSource.ToLower() == UNCTarget.ToLower())
            {
                Common.ShowMsg(Common.ERR_BACKUPSAMEFILE);
                return false;
            }

            DialogResult result = Common.ShowMsg(Common.MSG_FILEEXISTS, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes) { f.Delete(); }
            else { return false; }

            return true;
        }

        private void KillFile(string szFile)
        {
            FileInfo f = new FileInfo(szFile);
            try { f.Delete(); } catch { }
        }

        #endregion

        #region Rebuild Index
        private void RebuildIndex()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(REINDEXWARNING, TargetNode.Tag));
                return;
            }

            string SqlStatement = string.Format("REINDEX \"{0}\";", TargetNode.Tag);
            string SuccessMessage = string.Format(Common.OK_REINDEX, TargetNode.Tag);
            string ErrorMessage = Common.ERR_SQL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }
        #endregion

        #region Rebuild All Indexes
        private void RebuildAllIndexes()
        {
            string tablename = TargetNode.Parent.Text;
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(REINDEXALLWARNING, tablename));
                return;
            }

            string SqlStatement = string.Empty;
            string SuccessMessage = string.Format(Common.OK_REINDEXALL, tablename);
            string ErrorMessage = Common.ERR_SQL;

            foreach (TreeNode idxNode in TargetNode.Nodes)
            {
                SqlStatement = string.Format("REINDEX \"{0}\";", idxNode.Tag);
                if (!ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage)) break;
            }
            return;
        }
        #endregion

        #region Delete Index
        private void DeleteIndex()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(DELINDEXWARNING, TargetNode.Text));
                return;
            }

            string SqlStatement = string.Format("DROP INDEX \"{0}\";", TargetNode.Tag);
            string SuccessMessage = string.Format(Common.OK_DELINDEX, TargetNode.Tag);
            string ErrorMessage = Common.ERR_SQL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }
        #endregion

        #region Delete All Indexes
        private void DeleteAllIndexes()
        {
            string tablename = TargetNode.Parent.Text;
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(DELALLINDEXWARNING, tablename));
                return;
            }

            string SqlStatement = string.Empty;
            string SuccessMessage = string.Format(Common.OK_DELALLINDEXES, tablename);
            string ErrorMessage = Common.ERR_SQL;

            foreach (TreeNode idxNode in TargetNode.Nodes)
            {
                SqlStatement = string.Format("DROP INDEX \"{0}\";", idxNode.Tag);
                if (!ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage)) break;
            }
            return;
        }
        #endregion

        #region Delete View
        private void DeleteView()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(DELVIEWWARNING, TargetNode.Text));
                return;
            }

            string SqlStatement = string.Format("DROP VIEW \"{0}\";", TargetNode.Text);
            string SuccessMessage = string.Format(Common.OK_DELVIEW, TargetNode.Text);
            string ErrorMessage = Common.ERR_SQL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }
        #endregion

        #region Delete Trigger
        private void DeleteTrigger()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(DELTRIGGERWARNING, TargetNode.Text));
                return;
            }

            string SqlStatement = string.Format("DROP TRIGGER \"{0}\";", TargetNode.Text);
            string SuccessMessage = string.Format(Common.OK_DELTRIGGER, TargetNode.Text);
            string ErrorMessage = Common.ERR_SQL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }
        #endregion

        #region helpers

        /// <summary>
        /// Display an informative message indicate the effect of executing the requested action.
        /// </summary>
        /// <param name="message">Message to display.</param>
        private void DisplayWarning(string message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(message).Append(Environment.NewLine).Append(Environment.NewLine).Append(EXECCONTINUE);
            txtMessage.Text = sb.ToString();
            sb.Clear();
            return;
        }

        private bool ExecuteAction(string sql, string SuccessMessage, string ErrorMessage)
        {
            int count = 0;
            bool result;
            SQLiteErrorCode returnCode;
            btnExecute.Enabled = false;
            // Disabling the button causes txtMessage text to become selected - turn it off.
            txtMessage.SelectionLength = 0;

            Cursor = Cursors.WaitCursor;
            switch (execType)
            {
                case SQLType.SQLBackup:
                    result = DataAccess.BackupDatabase(DatabaseLocation, txtInfo.Text, out returnCode);
                    break;
                case SQLType.SQLEncrypt:
                    result = DataAccess.EncryptDB(DatabaseLocation, txtInfo.Text, out returnCode);
                    break;
                default:
                    count = DataAccess.ExecuteNonQuery(DatabaseLocation, sql, out returnCode);
                    result = count < 0 ? false : true;
                    break;
            }
            Cursor = Cursors.Default;
            if (!result || returnCode != SQLiteErrorCode.Ok)
            {
                //Common.ShowMsg(string.Format(ErrorMessage, DataAccess.LastError, returnCode.ToString()));
                string eMsg = string.Format(ErrorMessage, DataAccess.LastError, returnCode.ToString());
                toolStripExecutionStatus.Text = eMsg.IndexOf(":") > 0 ? eMsg.Substring(0, eMsg.IndexOf(":")) : eMsg.Substring(0, 18);
                toolStripExecutionStatus.ToolTipText = eMsg;
                return false;
            }
            else
            {
                SuccessMessage = SuccessMessage.Replace("%count%", count.ToString());
                toolStripExecutionStatus.Text = SuccessMessage.IndexOf(":") > 0 ? SuccessMessage.Substring(0, SuccessMessage.IndexOf(":")) : SuccessMessage.Substring(0, 18);
                toolStripExecutionStatus.ToolTipText = SuccessMessage;
                btnCancel.Text = "Close";
                bActionComplete = true;
                return true;
            }
        }

        /// <summary>
        /// Display an abbreviated error message in the statusbar and place the entire message in the tooltip for the message.
        /// </summary>
        /// <param name="errMessage">Message to display.</param>
        /// <param name="returnCode">Error code returned by SQLite.</param>
        private void GenerateErrorMessage(string errMessage, SQLiteErrorCode returnCode)
        {
            string eMsg = string.Format(errMessage, DataAccess.LastError, returnCode.ToString());
            toolStripExecutionStatus.Text = eMsg.IndexOf(":") > 0 ? eMsg.Substring(0, eMsg.IndexOf(":")) : eMsg.Substring(0, 18);
            toolStripExecutionStatus.ToolTipText = eMsg;
            btnExecute.Enabled = true;
        }

        private void btnFileFind_Click(object sender, EventArgs e)
        {
            txtInfo.Text = FindBackupLocation(false);
        }

        private string FindBackupLocation(bool bFileExists = true)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Select Database Backup File";
            openFile.Filter = "All files (*.*)|*.*|Database Files (*.db)|*.db";
            openFile.FilterIndex = 2;
            openFile.CheckFileExists = bFileExists;
            openFile.AddExtension = true;
            openFile.AutoUpgradeEnabled = true;
            openFile.DefaultExt = "db";
            openFile.InitialDirectory = MainForm.cfg.appsetting(Config.CFG_LASTBKPOPEN);
            openFile.Multiselect = false;
            openFile.ShowReadOnly = false;
            openFile.ValidateNames = true;
            if (openFile.ShowDialog() != DialogResult.OK) return string.Empty;
            MainForm.cfg.SetSetting(Config.CFG_LASTBKPOPEN, Path.GetDirectoryName(openFile.FileName));
            return openFile.FileName;

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
    }
}
