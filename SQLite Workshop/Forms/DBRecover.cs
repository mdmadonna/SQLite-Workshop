using System;
using System.IO;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class DBRecover : Form
    {

        ProcessData p;
        long totalStmts = 0;

        private ToolTip toolTip;
        public DBRecover()
        {
            InitializeComponent();
        }

        private void DBRecover_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            HouseKeeping(this, "Recover Database");
            lblPanelHeading.Text = DBRECOVERY;
            txtDbIn.Focus();
            if (appSetting(Config.CFG_RECOVERDBWARN) != "true") if (!ShowWarning()) this.Close();
        }

        private void DBRecover_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void btnDbIn_Click(object sender, EventArgs e)
        {
            txtDbIn.Text = FindDBLocation("DB to Recover", true);
        }

        private void btnDbOut_Click(object sender, EventArgs e)
        {
            txtDbOut.Text = FindFileLocation(radioDump.Checked ? "Recovered SQL" :"Recovered DB", false);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            if (!ValidateInput()) return;
            if (!ComponentsFound()) if (!LoadComponents()) return;
            try
            {
                Cursor = Cursors.WaitCursor;
                MainForm.mInstance.Cursor = Cursors.WaitCursor;
                RecoverDB();
            }
            finally
            {
                Cursor = Cursors.Default;
                MainForm.mInstance.Cursor = Cursors.Default;
            }
        }

        protected bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtDbIn.Text))
            {
                lblError.Text = ERR_REQUIREDENTRY;
                txtDbIn.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtDbOut.Text))
            {
                lblError.Text = ERR_REQUIREDENTRY;
                txtDbIn.Focus();
                return false;
            }

            if (!ValNoOverwrite(txtDbIn.Text, txtDbOut.Text, out string errmsg))
            {
                if (string.IsNullOrEmpty(errmsg)) return false;
                lblError.Text = errmsg;
                txtDbIn.Focus();
                return false;
            }

            return true;
        }

        #region Helpers

        private string FindFileLocation(string Title, bool bFileExists = true)
        {
            FileDialogInfo fi = new FileDialogInfo()
            {
                Title = Title,
                Filter = string.Format("All files (*.*)|*.*|{0}", radioDump.Checked ? "Text Files (*.sql)|*.sql" :"Database Files (*.db)|*.db"),
                FilterIndex = 2,
                CheckFileExists = bFileExists,
                DefaultExt = radioDump.Checked ? "sql" : "db",
                InitialDirectory = appSetting(Config.CFG_LASTBKPOPEN)
            };
            string fileName = GetFileName(fi);
            if (!string.IsNullOrEmpty(fileName))
                saveSetting(Config.CFG_LASTBKPOPEN, Path.GetDirectoryName(fileName));
            return fileName;
        }

        private string FindDBLocation(string Title, bool bFileExists = true)
        {
            FileDialogInfo fi = new FileDialogInfo()
            {
                Title = Title,
                Filter = string.Format("All files (*.*)|*.*|(.db)|*.db"),
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

        private void RecoverDB()
        {
            if (!ComponentsFound()) 
                if (!LoadComponents())
                {
                    ShowMsg(string.Format(ERR_CANTLOADTOOLS));
                    return; 
                }

            FileInfo fi = new FileInfo(txtDbOut.Text);
            if (fi.Exists)
            {
                if (!KillFile(txtDbOut.Text))
                {
                    ShowMsg(string.Format(ERR_CANTDELETE, txtDbOut.Text) + ERR_NEEDRECOVERYDB);
                    return;
                }
            }

            string ToolsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SQLite_Workshop", TOOLS_DIR);
            string tmpFile;
            string parm;
            if (radioRecover.Checked)
            {
                tmpFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
                parm = ".recover";
            }
            else
            {
                if (!ValidateOutputFile(txtDbOut.Text, out string msg))
                {
                    ShowMsg(string.Format(ERR_CANTCREATEOUTPUT, txtDbOut.Text, msg));
                    return;
                }
                tmpFile = txtDbOut.Text;
                parm = ".dump";
            }

            p = new ProcessData
            {
                FileName = Path.Combine(ToolsDir, TOOLS_UTILITY),
                Parms = string.Format("\"{0}\" {1}", txtDbIn.Text.Trim(), parm),
                RedirectOutput = true,
                OutputFile = tmpFile,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                Input = null,
                errorMsg = null,
                WaitForCompletion = true
            };
            toolStripStatusMsg.Text = WORKING;
            Application.DoEvents();
            Common.JobReport += JobReport;
            // Insure we unsuscribe, otherwise this DBRecover instance will hang around
            // for the life of the application
            try { SubmitJob(ref p); } finally { Common.JobReport -= JobReport; }

            if (p.ExitCode != 0)
            {
                ShowMsg(string.Format("{0}\r\n{1}", p.errorMsg, p.errorOutput));
                return;
            }

            if (radioDump.Checked)
            {
                toolStripStatusMsg.Text = OK_DUMP;
                return;
            }

            try
            {
                if (!LoadSql(tmpFile, txtDbOut.Text)) return;
                MainForm.mInstance.LoadDB(txtDbOut.Text);
                toolStripStatusMsg.Text = OK_RECOVERY;
            }
            finally
            {
                KillFile(tmpFile);
            }
        }

        internal bool LoadSql(string Source, string TargetDB)
        {
            SqlLoader sl = new SqlLoader(false);
            sl.LoadSqlStatusReport += LoadSqlStatusReport;
            try
            {
                sl.LoadSql(Source, TargetDB);
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format(ERR_SQLLOADERR, ex.Message));
                return false;
            }
            finally { sl.LoadSqlStatusReport -= LoadSqlStatusReport; }
            return true;
        }

        internal void JobReport(object sender, JobEventArgs e)
        {
            totalStmts = e.LineCount;
            toolStripStatusMsg.Text = string.Format("Recovered {0} stmts", e.LineCount.ToString());
            Application.DoEvents();
        }

        internal void LoadSqlStatusReport(object sender, LoadSqlEventArgs e)
        {
            toolStripStatusMsg.Text = string.Format("Executed {0} of {1} stmts", e.StmtCount.ToString(), totalStmts);
            Application.DoEvents();
        }

        private bool ShowWarning()
        {
            ShowMsg sm = new ShowMsg
            {
                Message = WARN_RECOVER
            };
            if (!ComponentsFound()) sm.Message += WARN_COMPONENT;
            sm.Message += OKCANCEL;
            sm.ShowDialog();
            if (sm.DoNotShow) saveSetting(Config.CFG_RECOVERDBWARN, "true");
            return sm.Result != DialogResult.Cancel;
        }

        private bool ValidateOutputFile(string fName, out string msg)
        {
            try
            {
                using (File.Create(fName)) { }
                File.Delete(fName);
                msg = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
        #endregion

        private void radioRecover_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDbOut.Text)) return;
            if (!txtDbOut.Text.EndsWith(".db")) Path.ChangeExtension(txtDbOut.Text, ".db");
        }

        private void radioDump_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDbOut.Text)) return;
            if (!txtDbOut.Text.EndsWith(".sql")) Path.ChangeExtension(txtDbOut.Text, ".sql");
        }
    }
}
