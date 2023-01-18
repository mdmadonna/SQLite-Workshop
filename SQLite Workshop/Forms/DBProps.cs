using System;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class DBProps : Form
    {

        protected enum ReportType
        {
            consolidated,
            table,
            index
        }

        internal string DatabaseLocation { get; set; }
        
        public DBProps(string DBLocation)
        {
            DatabaseLocation = DBLocation;
            InitializeComponent();
        }

        private void DBProps_Load(object sender, EventArgs e)
        {
            HouseKeeping(this, "Database Properties");
            RefreshPropertyGrid();
        }
        private void DBProps_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }


        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshPropertyGrid();
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
                         
        internal void RefreshPropertyGrid()
        {
            DBProperties p = new DBProperties();
            propertyGridDBProperties.SelectedObject = p.dbprops;
            propertyGridDBProperties.Refresh();
            propertyGridDBRuntime.SelectedObject = p.dbRT;
            propertyGridDBRuntime.Refresh();

        }

    }
}
