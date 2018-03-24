using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{

    class SqlTab
    {

        MainForm m;
        TabPage sTab;
        SqlTabControl sqlTabControl;
        SchemaDefinition sd;

        //static string SPACER = Environment.NewLine + "\t";

        internal string DatabaseLocation { get; set; }
        internal SqlTab()
        {
            m = MainForm.mInstance;
            m.tabMain.Visible = true;
        }

        internal void BuildTab(TreeNode TargetNode, SQLType sqlType)
        {
            m.sqlTabTrack++;
            int curtab = m.sqlTabTrack;

            DatabaseLocation = MainForm.mInstance.CurrentDB;
            sd = DataAccess.SchemaDefinitions[DatabaseLocation];
            BuildTab(sd.DBLocation);

            sTab.Text = string.Format("  Query{0}.sql - {1}          ", curtab, sd.DBName);

            switch (sqlType)
            {
                case SQLType.SQLGenCreate:
                    sqlTabControl.SqlStatement = BuildCreateSql(TargetNode) + ";";
                    break;

                case SQLType.SQLGenDropAndCreate:
                    sqlTabControl.SqlStatement = string.Format("{0}{1}{1}{2}", BuildDropSql(TargetNode) + ";", Environment.NewLine, BuildCreateSql(TargetNode) + ";");
                    break;

                case SQLType.SQLSelect1000:
                    sqlTabControl.SqlStatement = BuildSelectSql(TargetNode, 1000) + ";";
                    sqlTabControl.Execute();
                    break;

                case SQLType.SQLSelect1000View:
                    sqlTabControl.SqlStatement = BuildSelectViewSql(TargetNode, 1000) + ";";
                    sqlTabControl.Execute();
                    break;

                case SQLType.SQLGenSelect:
                    sqlTabControl.SqlStatement = BuildSelectSqlTemplate(TargetNode);
                    break;

                case SQLType.SQLGenInsert:
                    sqlTabControl.SqlStatement = BuildInsertSqlTemplate(TargetNode);
                    break;

                case SQLType.SQLGenUpdate:
                    sqlTabControl.SqlStatement = BuildUpdateSqlTemplate(TargetNode);
                    break;

                case SQLType.SQLGenDelete:
                    sqlTabControl.SqlStatement = BuildDeleteSqlTemplate(TargetNode);
                    break;

                case SQLType.SQLGenDrop:
                    sqlTabControl.SqlStatement = BuildDropSql(TargetNode);
                    break;

                case SQLType.SQLGenIndex:
                    sqlTabControl.SqlStatement = BuildIndexSql(TargetNode);
                    break;

                case SQLType.SQLGenAllIndexes:
                    sqlTabControl.SqlStatement = BuildAllIndexSql(TargetNode);
                    break;

                case SQLType.SQLGenViewCreate:
                    sqlTabControl.SqlStatement = BuildCreateViewSql(TargetNode);
                    break;

                case SQLType.SQLCreateView:
                    sqlTabControl.SqlStatement = BuildCreateViewSqlTemplate();
                    break;

                case SQLType.SQLEditView:
                    sqlTabControl.SqlStatement = BuildEditViewSql(TargetNode);
                    break;

                case SQLType.SQLGenTriggerCreate:
                    sqlTabControl.SqlStatement = BuildCreateTriggerSql(TargetNode);
                    break;

                case SQLType.SQLCreateTrigger:
                    sqlTabControl.SqlStatement = BuildCreateTriggerSqlTemplate();
                    break;

                case SQLType.SQLEditTrigger:
                    sqlTabControl.SqlStatement = BuildEditTriggerSql(TargetNode);
                    break;

                default:
                    break;
            }


        }

        internal void BuildTab(string dbLocation, SQLType sqlType)
        {
            m.sqlTabTrack++;
            int curtab = m.sqlTabTrack;

            sd = DataAccess.SchemaDefinitions[dbLocation];
            BuildTab(sd.DBLocation);

            sTab.Text = string.Format("  Query{0}.sql - {1}          ", curtab, sd.DBName);

            switch (sqlType)
            {
                case SQLType.SQLIntegrityCheck:
                    sqlTabControl.SqlStatement = BuildIntegrityCheckSql() + ";";
                    sqlTabControl.Execute();
                    break;

                default:
                    break;
            }


        }

        protected void BuildTab(string dbName)
        {

            sTab = new TabPage();

            sqlTabControl = new SqlTabControl(dbName);
            sTab.Controls.Add(sqlTabControl);
            m.tabMain.TabPages.Add(sTab);
            m.SetTabHeader();
            m.tabMain.SelectedTab = sTab;
        }

        protected string BuildCreateSql(TreeNode tblNode)
        {
            return sd.Tables[tblNode.Text].CreateSQL;
        }

        protected string BuildDropSql(TreeNode tblNode)
        {
            return SqlFactory.DropSql(tblNode.Text);
        }

        protected string BuildIntegrityCheckSql()
        {
            return SqlFactory.IntegrityCheckSql();
        }
        protected string BuildSelectSql(TreeNode tblNode, int count = 0)
        {
            return SqlFactory.SelectSql(sd, tblNode.Text, count);
        }

        protected string BuildSelectViewSql(TreeNode tblNode, int count = 0)
        {
            return SqlFactory.SelectViewSql(sd, tblNode.Text, count);
        }

        protected string BuildSelectSqlTemplate(TreeNode tblNode)
        {
            return SqlFactory.SelectTemplate(sd, tblNode.Text);
        }

        protected string BuildInsertSqlTemplate(TreeNode tblNode)
        {
            return SqlFactory.InsertTemplate(sd, tblNode.Text);
        }

        protected string BuildUpdateSqlTemplate(TreeNode tblNode)
        {
            return SqlFactory.UpdateTemplate(sd, tblNode.Text);
        }

        protected string BuildDeleteSqlTemplate(TreeNode tblNode)
        {
            return SqlFactory.DeleteTemplate(tblNode.Text);
        }

        protected string BuildIndexSql(TreeNode idxNode)
        {
            TreeNode tblNode = idxNode.Parent.Parent;
            string CreateSQL = sd.Tables[tblNode.Text].Indexes[idxNode.Tag.ToString()].CreateSQL;
            if (string.IsNullOrEmpty(CreateSQL))
                CreateSQL = string.Format("-- Index ({0}) is System generated.  Create SQL is not available.", idxNode.Tag.ToString());
            return CreateSQL;
        }

        protected string BuildAllIndexSql(TreeNode indexNode)
        {
            string sql = string.Empty;

            foreach (TreeNode idxNode in indexNode.Nodes)
            {
                sql += string.Format("{0};\r\n\r\n", BuildIndexSql(idxNode));
            }
            return sql;
        }

        protected string BuildCreateViewSql(TreeNode tblNode)
        {
            return sd.Views[tblNode.Text].CreateSQL;
        }

        protected string BuildEditViewSql(TreeNode tblNode)
        {
            return SqlFactory.EditViewSql(sd, tblNode.Text);
        }

        protected string BuildCreateViewSqlTemplate()
        {
            return SqlFactory.ViewTemplate();
        }

        protected string BuildCreateTriggerSql(TreeNode tblNode)
        {

            return sd.Triggers[tblNode.Text].CreateSQL;

        }

        protected string BuildEditTriggerSql(TreeNode tblNode)
        {
            return SqlFactory.EditTriggerSql(sd, tblNode.Text);
        }

        protected string BuildCreateTriggerSqlTemplate()
        {
            return SqlFactory.TriggerTemplate();
        }
    }
}
