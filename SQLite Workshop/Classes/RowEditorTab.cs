using System;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    class RowEditorTab : TabManager
    {
        TabPage sTab;
        RowEditTabControl dbeTabControl;
        SchemaDefinition sd;

        internal RowEditorTab(string DbLocation)
        {
            m = MainForm.mInstance;
            m.tabMain.Visible = true;
            DatabaseLocation = DbLocation;
        }

        internal void BuildTab(TreeNode TargetNode)
        {
            m.sqlTabTrack++;
            int curtab = m.sqlTabTrack;

            sd = DataAccess.SchemaDefinitions[DatabaseLocation];
            BuildTab(sd.DBLocation, TargetNode.Text);

            sTab.Text = string.Format("   Edit - {1}          ", curtab, TargetNode.Text);
        }


        protected void BuildTab(string dbName, string tblname)
        {

            sTab = new TabPage();
            sTab.Enter += new EventHandler(OnEnter);
            dbeTabControl = new RowEditTabControl(dbName, tblname);
            sTab.Controls.Add(dbeTabControl);
            m.tabMain.TabPages.Add(sTab);
            m.SetTabHeader();
            m.tabMain.SelectedTab = sTab;
            if (m.tabMain.TabPages.Count == 1) m.LoadConnectionProperties();
        }
    }
}
