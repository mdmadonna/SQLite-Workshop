/*********************************************************************************************
 * 
 * Ask User for the database password
 * 
 ********************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class GetPassword : Form
    {
        ToolTip toolTip;
        internal string Password;
        internal bool Cancelled;

        internal string DBLocation { get; set; }
        public GetPassword()
        {
            InitializeComponent();
        }

        private void GetPassword_Load(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;
            toolTip = new ToolTip();
            HouseKeeping(this, "Database Password");
            //In case Alt-F4 is pressed or Close Control box is clicked
            Cancelled = true;
            Password = string.Empty;
            txtPassword.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidatePassword()) return;

            Password = txtPassword.Text;
            Cancelled = false;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            this.Close();
        }

        private void GetPassword_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void btnViewPW_Click(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = txtPassword.PasswordChar == '*' ? '\0' : '*';
        }

        private bool ValidatePassword()
        {
            if (DataAccess.IsValidDB(DBLocation, txtPassword.Text, out _)) return true;

            lblError.Text = "Invalid Password";
            txtPassword.Focus();
            return false;
        }
    }
}
