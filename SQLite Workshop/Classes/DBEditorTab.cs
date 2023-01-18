using System;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    class DBEditorTab : TabManager
    {

        TabPage sTab;
        DBEditorTabControl dbeTabControl;
        SchemaDefinition sd;

        internal DBEditorTab(string DbLocation)
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

            dbeTabControl.Execute();

        }


        protected void BuildTab(string dbName, string tblname)
        {

            sTab = new TabPage();
            sTab.Enter += new EventHandler(OnEnter);
            dbeTabControl = new DBEditorTabControl(dbName, tblname);
            sTab.Controls.Add(dbeTabControl);
            m.tabMain.TabPages.Add(sTab);
            m.SetTabHeader();
            m.tabMain.SelectedTab = sTab;
            // Reserved for future use.
            //m.SQLButton(true);
            if (m.tabMain.TabPages.Count == 1) m.LoadConnectionProperties();
        }
    }
}
