using System;
using System.Drawing;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class ShowSQL : Form
    {
        ToolTip toolTip;
        internal string SQL;
        public string objectType { get; set; }
        public ShowSQL()
        {
            InitializeComponent();
        }

        private void ShowSQL_Load(object sender, EventArgs e)
        {
            toolStripStatusLabelResults.Text = string.Empty;
            toolTip = new ToolTip();
            HouseKeeping(this, "Parse SQL");
            txtSQL.Text = SQL; 
        }

        private void ShowSQL_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            bool rc = DataAccess.Parse(MainForm.mInstance.CurrentDB, txtSQL.Text, out _);
            if (rc)
            {
                toolStripStatusLabelResults.Text = OK_PARSE;
                return;
            }
            toolStripStatusLabelResults.Text = string.Format(ERR_PARSE, DataAccess.LastError);
            ShowMsg(string.Format(ERR_PARSE, DataAccess.LastError));

        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
