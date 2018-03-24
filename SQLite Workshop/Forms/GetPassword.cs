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

namespace SQLiteWorkshop
{
    public partial class GetPassword : Form
    {

        internal string Password;
        internal bool Cancelled;

        internal string DBLocation { get; set; }
        public GetPassword()
        {
            InitializeComponent();
            txtPassword.Focus();
            lblMsg.Text = string.Empty;
        }

        private void GetPassword_Load(object sender, EventArgs e)
        {
            
            //In case Alt-F4 is pressed or Close Control box is clicked
            Cancelled = true;
            Password = string.Empty;

        }

        private void btnEnter_Click(object sender, EventArgs e)
        {

            if (!ValidatePassword()) return;

            Password = txtPassword.Text;
            Cancelled = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancelled = true;
            Password = string.Empty;
            this.Close();
        }

        private bool ValidatePassword()
        {
            if (DataAccess.VerifyPassword(DBLocation, txtPassword.Text)) return true;

            lblMsg.Text = "Invalid Password";
            txtPassword.Focus();
            return false;
        }
    }
}
