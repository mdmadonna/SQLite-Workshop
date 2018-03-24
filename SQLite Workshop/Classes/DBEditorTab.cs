using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    class DBEditorTab
    {

        MainForm m;
        TabPage sTab;
        DBEditorTabControl dbeTabControl;
        SchemaDefinition sd;

        static string SPACER = Environment.NewLine + "\t";

        internal string DatabaseLocation { get; set; }
        internal DBEditorTab()
        {
            m = MainForm.mInstance;
            m.tabMain.Visible = true;
        }

        internal void BuildTab(TreeNode TargetNode)
        {
            m.sqlTabTrack++;
            int curtab = m.sqlTabTrack;

            DatabaseLocation = TargetNode.Parent.Tag.ToString();
            sd = DataAccess.SchemaDefinitions[DatabaseLocation];
            BuildTab(sd.DBLocation, TargetNode.Text);

            sTab.Text = string.Format("   Edit - {1}          ", curtab, sd.DBName);

            dbeTabControl.Execute();

        }


        protected void BuildTab(string dbName, string tblname)
        {

            sTab = new TabPage();

            dbeTabControl = new DBEditorTabControl(dbName, tblname);
            sTab.Controls.Add(dbeTabControl);
            m.tabMain.TabPages.Add(sTab);
            m.SetTabHeader();
            m.tabMain.SelectedTab = sTab;
        }
    }
}
