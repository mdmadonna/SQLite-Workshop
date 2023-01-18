using System;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    class TableEditorTab : TabManager
    {

        TabPage tbTab;
        TableEditorTabControl tblTabControl;
        SchemaDefinition sd;

        internal TableEditorTab(string DbLocation)
        {
            m = MainForm.mInstance;
            m.tabMain.Visible = true;
            DatabaseLocation = DbLocation;
        }
        internal void BuildTab()
        {
            m.sqlTabTrack++;
            int curtab = m.sqlTabTrack;

            sd = DataAccess.SchemaDefinitions[DatabaseLocation];
            
            BuildTab(sd.DBLocation);
            tbTab.Text = string.Format("  Create Table {0}.sql - {1}          ", curtab, sd.DBName);
        }

        protected void BuildTab(string dbName)
        {

            tbTab = new TabPage();
            tbTab.Enter += new EventHandler(OnEnter);
            tblTabControl = new TableEditorTabControl(dbName);
            tbTab.Controls.Add(tblTabControl);
            tblTabControl.Dock = DockStyle.Fill;
            m.tabMain.TabPages.Add(tbTab);
            m.SetTabHeader();
            m.tabMain.SelectedTab = tbTab;
            if (m.tabMain.TabPages.Count == 1) m.LoadConnectionProperties();
        }

    }
}
