using System;
using System.Collections;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class ExecuteForm : Form
    {
        internal SQLType execType;
        internal TreeNode TargetNode;

        Label lblFormHeading = null;

        internal string DatabaseLocation { get; set; }
        internal string NewTableName { get; private set; }

        bool bActionApproved;
        internal bool bActionComplete;

        public ExecuteForm()
        {
            InitializeComponent();
            txtMessage.Text = string.Empty;
            toolStripExecutionStatus.Text = string.Empty;
            lblTxtInfo.Text = string.Empty;
            txtInfo.Visible = false;
            btnGetFile.Visible = false;
        }

        private void ExecuteForm_Load(object sender, EventArgs e)
        {
            DatabaseLocation = MainForm.mInstance.CurrentDB;
            HouseKeeping(this, string.Empty);
            lblFormHeading = this.Controls.Find("lblFormHeading", true).FirstOrDefault() as Label;
            lblError.Text = string.Empty;
            bActionApproved = false;
            bActionComplete = false;
            statusStrip.ShowItemToolTips = true;
            ExecuteCommand();
        }

        private void ExecuteForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
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
                case SQLType.SQLOptimize:
                    lblFormHeading.Text = TITLE_OPTIMIZE;
                    OptimizeDB();
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
                case SQLType.SQLRestore:
                    lblFormHeading.Text = TITLE_RESTOREDB;
                    RestoreDB();
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
            string SuccessMessage = string.Format(OK_RECORDSAFFECTED, "%count%");
            string ErrorMessage = ERR_SQL;
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
            string SuccessMessage = string.Format(OK_TBLDELETE, TargetNode.Text);
            string ErrorMessage = ERR_SQL;
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
                lblTxtInfo.Text = "New Name:";
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
            string SuccessMessage = string.Format(OK_RENAME, TargetNode.Text);
            string ErrorMessage = ERR_RENAMEFAIL;
            NewTableName = txtInfo.Text;
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
            string SuccessMessage = string.Format(OK_VACUUM, DatabaseLocation);
            string ErrorMessage = ERR_VACUUMFAIL;
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
                txtInfo.PasswordChar = '*';
                txtInfo.Focus();
                return;
            }

            txtInfo.Text = txtInfo.Text.Trim();
            string SqlStatement = string.Empty;
            string SuccessMessage = string.IsNullOrEmpty(txtInfo.Text) ? string.Format(OK_DECRYPT, DatabaseLocation) : string.Format(OK_ENCRYPT, DatabaseLocation);
            string ErrorMessage = ERR_ENCRYPTFAILED;
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
                txtInfo.Width -= (btnGetFile.Width + 10);
                btnGetFile.Visible = true;
                txtInfo.Focus();
                return;
            }

            if (!ValNoOverwrite(DatabaseLocation, txtInfo.Text, out string errmsg))
            {
                if (string.IsNullOrEmpty(errmsg)) return;
                lblError.Text = errmsg;
                txtInfo.Focus();
                return;
            }

            string SqlStatement = txtInfo.Text;
            string SuccessMessage = string.Format(OK_BACKUP, DatabaseLocation, txtInfo.Text);
            string ErrorMessage = ERR_BACKUPFAILED;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }

        
        #endregion

        #region Optimize Database
        private void OptimizeDB()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(OPTIMIZEWARNING, DatabaseLocation));
                return;
            }

            string SqlStatement = "Pragma optimize";
            string SuccessMessage = string.Format(OK_OPTIMIZE, DatabaseLocation);
            string ErrorMessage = string.Format(OK_OPTIMIZE, DatabaseLocation);      //always succeeds
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
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
                txtInfo.Width -= (btnGetFile.Width + 10);
                btnGetFile.Visible = true;
                txtInfo.Focus();
                return;
            }

            if (!ValNoOverwrite(DatabaseLocation, txtInfo.Text, out string errmsg))
            {
                if (string.IsNullOrEmpty(errmsg)) return;
                lblError.Text = errmsg;
                txtInfo.Focus();
                return;
            }

            btnExecute.Enabled = false;
            bool bresult = DataAccess.CreateDB(txtInfo.Text);
            if (!bresult)
            {
                GenerateErrorMessage(ERR_CLONEFAILED,  0);
                return;
            }

            // Load a list of existing tables
            string SqlStatement = "Select * FROM sqlite_master Where Type = \"table\"";
            DataTable dtMaster = DataAccess.ExecuteDataTable(DatabaseLocation, SqlStatement, out SQLiteErrorCode returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                KillFile(txtInfo.Text);
                GenerateErrorMessage(ERR_CLONEFAILED, returnCode);
                return;
            }

            // Turn off Foreign Key check
            DataAccess.ExecuteNonQuery(txtInfo.Text, "PRAGMA foreign_keys = OFF;", out returnCode);

            // Create all non-system tables
            ArrayList sqlSave = new ArrayList();
            foreach (DataRow dr in dtMaster.Rows)
            {
                if (!IsSystemTable(dr["tbl_name"].ToString()))
                {
                    long iresult = DataAccess.ExecuteNonQuery(txtInfo.Text, dr["sql"].ToString(), out returnCode);
                    if (iresult < 0 || returnCode != SQLiteErrorCode.Ok) sqlSave.Add(dr["sql"].ToString());
                }
            }

            // This second pass is not needed at this time.  It's here for future consideration
            foreach (string szSql in sqlSave)
            {
                long iresult = DataAccess.ExecuteNonQuery(txtInfo.Text, szSql, out returnCode);
                {
                    GenerateErrorMessage(ERR_CLONEFAILED, returnCode);
                    KillFile(txtInfo.Text);
                    return;
                }
            }

            // Now that all tables are created, create indexes, views and triggers
            SqlStatement = "Select * FROM sqlite_master Where Type != \"table\"";
            dtMaster = DataAccess.ExecuteDataTable(DatabaseLocation, SqlStatement, out returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                GenerateErrorMessage(ERR_CLONEFAILED, returnCode);
                KillFile(txtInfo.Text);
                return;
            }

            foreach (DataRow dr in dtMaster.Rows)
            {
                long iresult = DataAccess.ExecuteNonQuery(txtInfo.Text, dr["sql"].ToString(), out returnCode);
                if (iresult < 0 || returnCode != SQLiteErrorCode.Ok)
                {
                    GenerateErrorMessage(ERR_CLONEFAILED, returnCode);
                    KillFile(txtInfo.Text);
                    return;
                }
            }

            toolStripExecutionStatus.Text = "Clone Ok";
            toolStripExecutionStatus.ToolTipText = string.Format(OK_CLONE, DatabaseLocation, txtInfo.Text);
            btnCancel.Text = "Close";
            bActionComplete = true;
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
            string SuccessMessage = string.Format(OK_REINDEX, TargetNode.Tag);
            string ErrorMessage = ERR_SQL;
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

            string SqlStatement;
            string SuccessMessage = string.Format(OK_REINDEXALL, tablename);
            string ErrorMessage = ERR_SQL;

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
            string SuccessMessage = string.Format(OK_DELINDEX, TargetNode.Tag);
            string ErrorMessage = ERR_SQL;
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

            string SuccessMessage = string.Format(OK_DELALLINDEXES, tablename);
            string ErrorMessage = ERR_SQL;

            foreach (TreeNode idxNode in TargetNode.Nodes)
            {
                string SqlStatement = string.Format("DROP INDEX \"{0}\";", idxNode.Tag);
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
            string SuccessMessage = string.Format(OK_DELVIEW, TargetNode.Text);
            string ErrorMessage = ERR_SQL;
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
            string SuccessMessage = string.Format(OK_DELTRIGGER, TargetNode.Text);
            string ErrorMessage = ERR_SQL;
            ExecuteAction(SqlStatement, SuccessMessage, ErrorMessage);
            return;
        }
        #endregion

        #region Restore Database
        /// <summary>
        /// Restore a database.  This simply renames the current db and copies a backup
        /// to the current location.
        /// </summary>
        private void RestoreDB()
        {
            if (!bActionApproved)
            {
                DisplayWarning(string.Format(RESTOREDBWARNING, DatabaseLocation));
                txtInfo.Visible = true;
                txtInfo.Width -= (btnGetFile.Width + 10);
                btnGetFile.Visible = true;
                lblTxtInfo.Text = "Backup DB:";
                txtInfo.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtInfo.Text))
            {
                txtInfo.Focus();
                toolStripExecutionStatus.Text = ERR_VALIDDBBKP;
                return;
            }

            // Make sure the backup exists
            FileInfo fi = new FileInfo(txtInfo.Text);
            if (!fi.Exists)
            {
                txtInfo.Focus();
                toolStripExecutionStatus.Text = ERR_VALIDDBBKP;
            }

            // Insure the source and dest are not the same file.
            if (!ValNoOverwrite(DatabaseLocation, txtInfo.Text, out string errmsg, true))
            {
                if (string.IsNullOrEmpty(errmsg)) return;
                toolStripExecutionStatus.Text = errmsg;
                txtInfo.Focus();
                return;
            }

            // Determine if the backup is a valid SQLite database.
            if (!DataAccess.IsValidDB(txtInfo.Text, null, out _))
            {
                if (ShowMsg(string.Format(WARN_NOTADB, txtInfo.Text), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) 
                    return;
            }

            if (!RenameFile(DatabaseLocation, out string newname, out string message))
            {
                ShowMsg(string.Format(ERR_CANTRENAME, DatabaseLocation, message));
                return;
            }

            toolStripExecutionStatus.Text = WORKING;
            Cursor = Cursors.WaitCursor;
            MainForm.mInstance.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            try
            {
                Cursor = Cursors.WaitCursor;
                fi.CopyTo(DatabaseLocation);
            }
            catch (Exception ex)
            {
                KillFile(DatabaseLocation);
                RenameFile(newname, DatabaseLocation, out _);
                toolStripExecutionStatus.Text = string.Format(ERR_RESTOREFAIL, string.Empty);
                ShowMsg(string.Format(ERR_RESTOREFAIL, ex.Message));
                return;
            }
            finally
            {
                MainForm.mInstance.Cursor = Cursors.Default;
                Cursor = Cursors.Default;
            }
            MainForm.mInstance.LoadDB(DatabaseLocation, true);
            toolStripExecutionStatus.Text = string.Format(OK_RESTORE, Path.GetFileName(DatabaseLocation));
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
            long count = 0;
            bool result;
            SQLiteErrorCode returnCode;
            btnExecute.Enabled = false;
            // Disabling the button causes txtMessage text to become selected - turn it off.
            txtMessage.SelectionLength = 0;

            Cursor = Cursors.WaitCursor;
            MainForm.mInstance.Cursor = Cursors.WaitCursor;
            toolStripExecutionStatus.Text = WORKING;
            Application.DoEvents();
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
                    result = count >= 0;
                    break;
            }
            Cursor = Cursors.Default;
            MainForm.mInstance.Cursor = Cursors.Default;
            if (!result || returnCode != SQLiteErrorCode.Ok)
            {
                //ShowMsg(string.Format(ErrorMessage, DataAccess.LastError, returnCode.ToString()));
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
        private void btnGetFile_Click(object sender, EventArgs e)
        {
            string caption = string.Empty;
            switch (execType)
            {
                case SQLType.SQLBackup:
                    caption = EXEC_SELECTDB;
                    break;
                case SQLType.SQLClone:
                    caption = EXEC_SELECTCLONE;
                    break;
                case SQLType.SQLRestore:
                    caption = EXEC_SELECTDB;
                    break;
                case SQLType.SQLAttach:
                    caption = EXEC_ATTACHDB;
                    break;
                default:                    
                    break;
            }
            txtInfo.Text = FindFileLocation(caption, false);
        }


        private string FindFileLocation(string Title, bool bFileExists = true)
        {
            FileDialogInfo fi = new FileDialogInfo()
            {
                Title = Title,
                Filter = "All files (*.*)|*.*|Database Files (*.db)|*.db",
                FilterIndex = 2,
                CheckFileExists = bFileExists,
                DefaultExt = "db",
                InitialDirectory = appSetting(Config.CFG_LASTBKPOPEN)
            };
            string fileName = GetFileName(fi);
            if (!string.IsNullOrEmpty(fileName))
                saveSetting(Config.CFG_LASTBKPOPEN, Path.GetDirectoryName(fileName));
            return fileName;
        }

       
        #endregion

    }
}
