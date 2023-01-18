/*********************************************************************************************
 * 
 * Template used to create new Windows Forms
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
    public partial class Template : Form
    {
        ToolTip toolTip;
        public Template()
        {
            InitializeComponent();
        }

        private void Template_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            HouseKeeping(this, "Template");
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Template_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }
    }
}
