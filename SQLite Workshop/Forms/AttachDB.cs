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

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;
using static SQLiteWorkshop.DataAccess;

namespace SQLiteWorkshop
{
    public partial class AttachDB : Form
    {
        ToolTip toolTip;
        private readonly string DatabaseLocation;

        private readonly string[] usedSchemas = new string[] { "main", "temp" };
    
        public AttachDB(string dbLocation)
        {
            InitializeComponent();
            DatabaseLocation = dbLocation;

        }

        private void AttachDB_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            HouseKeeping(this, "Attach Database");
            lblPanelHeading.Text = string.Format(TITLE_ATTACHDB, DatabaseLocation);
            txtDbName.Focus();

            if (SchemaDefinitions.TryGetValue(DatabaseLocation, out SchemaDefinition sd))
                if (!string.IsNullOrEmpty(sd.password)) lblError.Text = ERR_NOATTACHENCRYPT;
        }

        private void AttachDB_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGetFile_Click(object sender, EventArgs e)
        {
            txtDbName.Text = FindFileLocation("DB to Attach", true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            if (!ValidateInput()) return;
            AttachDbName();
        }

        private bool ValidateInput()
        {
            if (SchemaDefinitions.TryGetValue(DatabaseLocation, out SchemaDefinition sd))
                if (!string.IsNullOrEmpty(sd.password))
                {
                    lblError.Text = ERR_NOATTACHENCRYPT;
                    return false;
                }

            if (string.IsNullOrEmpty(txtDbName.Text))
            {
                txtDbName.Focus();
                lblError.Text = ERR_VALIDDB;
                return false;
            }

            // Make sure the db to attach exists
            FileInfo fi = new FileInfo(txtDbName.Text);
            if (!fi.Exists)
            {
                txtDbName.Focus();
                lblError.Text = ERR_VALIDDB;
            }

            if (string.IsNullOrEmpty(txtSchemaName.Text))
            {
                txtSchemaName.Focus();
                lblError.Text = string.Format(ERR_VALIDSCHEMA, txtSchemaName.Text);
                return false;
            }

            if (usedSchemas.Contains(txtSchemaName.Text))
            {
                txtSchemaName.Focus();
                lblError.Text = string.Format(ERR_INVALIDSCHEMA, txtSchemaName.Text);
                return false;
            }

            // Determine if the db to attach is valid.
            if (!DataAccess.IsValidDB(txtDbName.Text, null, out _)) 
            {
                if (ShowMsg(string.Format(WARN_NOTADB, txtDbName.Text), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) 
                    return false;
            }
            return true;
        }

        private void AttachDbName()
        {
            string SqlStatement = string.Format("ATTACH DATABASE '{0}' AS '{1}';", txtDbName.Text, txtSchemaName.Text);
            string SuccessMessage = string.Format(OK_ATTACH, txtDbName.Text);
            string ErrorMessage = ERR_SQL;
            if (!TryAttach(SqlStatement, SuccessMessage, ErrorMessage))
            {
                toolStripStatus.Text = ERR_ATTACH;
                return;
            }

            toolStripStatus.Text = string.Format(OK_ATTACH, txtDbName.Text);
            DataAccess.AddAttachedDb(DatabaseLocation, txtDbName.Text, txtSchemaName.Text);
            MainForm.mInstance.RefreshAttachedDBs();
        }

        private bool TryAttach(string sql, string SuccessMessage, string ErrorMessage)
        {
            SQLiteErrorCode returnCode;
            SQLiteConnection conn = null;
            SQLiteCommand cmd = null;
            if (!DataAccess.OpenDB(DatabaseLocation, ref conn, ref cmd))
                return false;

            cmd.CommandText = sql;
            long rc = DataAccess.ExecuteNonQuery(cmd, out returnCode);
            if (returnCode != SQLiteErrorCode.Ok)
            {
                string eMsg = string.Format(ErrorMessage, DataAccess.LastError, returnCode.ToString());
                lblError.Text = eMsg.IndexOf(":") > 0 ? eMsg.Substring(0, eMsg.IndexOf(":")) : eMsg;
                conn.Close();
                return false;
            }
            try
            {
                cmd.CommandText = "pragma database_list";
                DataTable dt = DataAccess.ExecuteDataTable(cmd, out returnCode);
                if (returnCode != SQLiteErrorCode.Ok) return false;

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["name"].ToString() == txtSchemaName.Text && dr["file"].ToString() == txtDbName.Text) return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                conn.Close();
                return false;
            }
            finally
            {
                DataAccess.CloseDB(conn);
            }
        }


        #region Helpers

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
