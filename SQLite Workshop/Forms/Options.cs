using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.Config;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class Options : Form
    {
        ToolTip toolTip;
        string EncryptKey;
        readonly String[] EncryptedValues = new string[]
        {
            CFG_IMPHISTORY,
            CFG_REGISTEREDDBS
        };
        public Options()
        {
            InitializeComponent();
        }

        private void Options_Load(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            EncryptKey = appSetting(CFG_KEYPHRASE);
            txtEncryptKey.Text = EncryptKey;
            bool.TryParse(appSetting(CFG_OPENLASTDB), out bool OpenLastDB);
            chkOpenDB.Checked = OpenLastDB;
            bool.TryParse(appSetting(CFG_SAVEIMPORT), out bool SaveCredentials);
            chkSaveImportCredentials.Checked = SaveCredentials;
            bool.TryParse(appSetting(CFG_IGNOREIMPERRORS), out bool IgnoreErrors);
            chkIgnoreErrrors.Checked = IgnoreErrors;
            Decimal.TryParse(appSetting(CFG_MAXIMPERRORS), out decimal MaxErrors);
            upDownMaxErrors.Value = MaxErrors;
            upDownMaxErrors.Enabled = !chkIgnoreErrrors.Checked;
            toolTip = new ToolTip();
            HouseKeeping(this, "SQLite Workshop Options");
            LoadFunctions();
        }

        private void Options_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInput()) SaveOptions();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateInput()
        {
            lblError.Text = string.Empty;
            if (string.IsNullOrEmpty(txtEncryptKey.Text))
            {
                tabmain.SelectedTab = tabPageGeneral;
                txtEncryptKey.Focus();
                lblError.Text = ERR_ENCRYPTKEY;
                return false;
            }
            return true;
        }

        private void SaveOptions()
        {
            if (txtEncryptKey.Text.Trim() != EncryptKey) SaveEncryptKey(txtEncryptKey.Text.Trim());
            saveSetting(CFG_OPENLASTDB, chkOpenDB.Checked.ToString());
            saveSetting(CFG_IGNOREIMPERRORS, chkIgnoreErrrors.Checked.ToString());
            saveSetting(CFG_MAXIMPERRORS, upDownMaxErrors.Value.ToString());
            saveSetting(CFG_SAVEIMPORT, chkSaveImportCredentials.Checked.ToString());
            if (!chkSaveImportCredentials.Checked)
            {
                saveSetting(CFG_IMPTEXT, string.Empty);
                saveSetting(CFG_IMPEXCEL, string.Empty);
                saveSetting(CFG_IMPSQLITE, string.Empty);
                saveSetting(CFG_IMPMYSQL, string.Empty);
                saveSetting(CFG_IMPSQLSERVER, string.Empty);
                saveSetting(CFG_IMPMSACCESS, string.Empty);
                saveSetting(CFG_IMPODBC, string.Empty);
                saveSetting(CFG_IMPSQL, string.Empty);
            }
            SaveFunctions();
            lblError.Text = OK_OPTIONS;
        }

        /// <summary>
        /// Save the Encryption Key
        /// </summary>
        /// <param name="NewKey">New Encryption Key</param>
        private void SaveEncryptKey(string NewKey)
        {
            // Before saving the new encryption key, we need to re-encrypt all encrypted values
            // and take cae to provide some recoverability in case of failure during this routine
            saveSetting("OLDKEY", EncryptKey);
            saveSetting("NEWKEY", NewKey);
            foreach (string CfgKey in EncryptedValues)
            {
                string Val = appSetting(CfgKey);
                if (!string.IsNullOrEmpty(Val))
                {
                    GblPassword = EncryptKey;
                    string decryptedVal = Decrypt(Val);
                    GblPassword = NewKey;
                    saveSetting(CfgKey, Encrypt(decryptedVal));
                }
            }
            saveSetting(CFG_KEYPHRASE, NewKey);
            GblPassword = NewKey;
            saveSetting("OLDKEY", string.Empty);
            saveSetting("NEWKEY", string.Empty);
        }

        private void SaveFunctions()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataGridViewRow dr in dgFunctions.Rows)
            {
                if (dr.Cells[3].Value.ToString() != "CAT")
                {
                    if (!(bool)dr.Cells[0].Value)
                    {
                        sb.Append(dr.Cells[1].Value.ToString().Trim()).Append(";");
                    }
                }
            }
            saveSetting(CFG_FUNCNOLOAD, sb.ToString());
        }

        private void LoadFunctions()
        {
            List<string> Categories = DataAccess.FunctionList.Select(o => o.Category).Distinct().ToList();
            Categories.Sort();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Enabled", typeof(Boolean)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Description", typeof(string)));
            dt.Columns.Add(new DataColumn("", typeof(string)));

            foreach (string cat in Categories)
            {
                var ListByCat = DataAccess.FunctionList.Select(i => new { i.Name, i.LoadOnOpen, i.Category, i.Description }).Where(i => i.Category == cat).ToList();
                LoadGrid(ref dt, ListByCat);
            }
            dgFunctions.Rows.Clear();
            dgFunctions.DataSource = dt;

            dgFunctions.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.Red };
            dgFunctions.Columns[0].Width = 50;
            dgFunctions.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgFunctions.Columns[1].Width = 120;
            dgFunctions.Columns[1].ReadOnly = true;
            dgFunctions.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgFunctions.Columns[2].Width = 340;
            dgFunctions.Columns[2].ReadOnly = true;
            dgFunctions.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgFunctions.Columns[3].Visible = false;
            dgFunctions.RowHeadersVisible = false;
            dgFunctions.Refresh();
        }

        private void LoadGrid(ref DataTable dt, IEnumerable<dynamic> FuncList)
        {
            int i = 0;
            foreach (dynamic function in FuncList)
            {
                if (i == 0) dt.Rows.Add(new object[] { false, function.Category, string.Empty, "CAT" });
                dt.Rows.Add(new object[] { function.LoadOnOpen, string.Format("   {0}", function.Name), function.Description, function.Category });
                i++;
            }
        }

        private void dgFunctions_VisibleChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dgFunctions.Rows.Count; i++)
            {
                if (dgFunctions.Rows[i].Cells[3].Value.ToString() == "CAT")
                {
                    dgFunctions.Rows[i].Cells[1].Style.Font = new Font("Tahoma", 9, FontStyle.Bold | FontStyle.Italic);
                }
            }
            dgFunctions.Refresh();
        }

        private void dgFunctions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && dgFunctions[3, e.RowIndex].Value.ToString() == "CAT")
            {
                DataGridViewCheckBoxCell cb = (DataGridViewCheckBoxCell)dgFunctions[e.ColumnIndex, e.RowIndex];
                bool ckStatus = (bool)cb.Value;

                string cat = dgFunctions[1, e.RowIndex].Value.ToString();
                for (int i = 0; i < dgFunctions.Rows.Count; i++)
                {
                    if (dgFunctions.Rows[i].Cells[3].Value.ToString() == cat)
                    {
                        ((DataGridViewCheckBoxCell)dgFunctions.Rows[i].Cells[0]).Value = ckStatus;
                    }
                }
                dgFunctions.Refresh();
            }
        }

        private void dgFunctions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgFunctions.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void chkIgnoreErrrors_CheckedChanged(object sender, EventArgs e)
        {
            upDownMaxErrors.Enabled = !chkIgnoreErrrors.Checked;
        }
    }
}
