using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    class TableEditorTab
    {
        MainForm m;
        TabPage tbTab;
        TableEditorTabControl tblTabControl;
        SchemaDefinition sd;

        internal string DatabaseLocation { get; set; }
        internal TableEditorTab()
        {
            m = MainForm.mInstance;
            m.tabMain.Visible = true;
        }
        internal void BuildTab()
        {
            m.sqlTabTrack++;
            int curtab = m.sqlTabTrack;

            DatabaseLocation = m.CurrentDB;
            sd = DataAccess.SchemaDefinitions[DatabaseLocation];
            
            BuildTab(sd.DBLocation);
            tbTab.Text = string.Format("  Create Table {0}.sql - {1}          ", curtab, sd.DBName);
        }

        internal void BuildTab(TreeNode TargetNode)
        { }

        protected void BuildTab(string dbName)
        {

            tbTab = new TabPage();

            tblTabControl = new TableEditorTabControl(dbName);
            tbTab.Controls.Add(tblTabControl);
            tblTabControl.Dock = DockStyle.Fill;
            m.tabMain.TabPages.Add(tbTab);
            m.SetTabHeader();
            m.tabMain.SelectedTab = tbTab;
        }

    }
}
