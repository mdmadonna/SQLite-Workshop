using System;
using System.IO;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;

namespace SQLiteWorkshop
{

    class SqlTab : TabManager
    {

        TabPage sTab;
        readonly int curtab;
        SqlTabControl sqlTabControl;
        SchemaDefinition sd;

        internal SqlTab(string DbLocation)
        {
            m = MainForm.mInstance;
            m.tabMain.Visible = true;
            m.sqlTabTrack++;
            curtab = m.sqlTabTrack;

            DatabaseLocation = DbLocation;
            sd = DataAccess.SchemaDefinitions[DatabaseLocation];
            BuildTab(sd.DBLocation);
            if (m.tabMain.TabPages.Count == 1) m.LoadConnectionProperties();
        }

        internal void BuildTab(FileInfo fi)
        {
            string filename = fi.FullName;
            if (fi.Length > MAX_SQL_FILESIZE)
            {
                ShowMsg(string.Format("Cannot open {0}\r\n{1}", filename, string.Format("File Exceeds {0} bytes.", MAX_SQL_FILESIZE.ToString())));
                return;
            }

            sTab.Text = string.Format("  {0}          ", Path.GetFileName(filename));
            sqlTabControl.SqlFileName = filename;
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    sqlTabControl.SqlStatement = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Cannot open {0}\r\n{1}", filename, ex.Message));
            }

        }

        internal void BuildTab()
        {
            string filename = FindSqlFile();
            if (string.IsNullOrEmpty(filename)) return;
            BuildTab(new FileInfo(filename));
        }

        private string FindSqlFile()
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Open File",
                Filter = "All files (*.*)|*.*|Sql Files (*.sql)|*.sql",
                FilterIndex = 2,
                CheckFileExists = true,
                AddExtension = true,
                AutoUpgradeEnabled = true,
                DefaultExt = "sql",
                InitialDirectory = appSetting(Config.CFG_DFLTSQLDIR),
                RestoreDirectory = true,
                Multiselect = false,
                ShowReadOnly = false,
                ValidateNames = true
            };
            if (openFile.ShowDialog() != DialogResult.OK) return string.Empty;
            return openFile.FileName;
        }

        /// <summary>
        /// Build an empty Query tab.
        /// </summary>
        /// <param name="sqlType"></param>
        internal void BuildTab(SQLType sqlType)
        {
            sTab.Text = "  New Query.sql          ";
        }

        internal void BuildTab(TreeNode TargetNode, SQLType sqlType)
        {


            sTab.Text = string.Format("  Query{0}.sql          ", curtab);

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
            //BuildTab(sd.DBLocation);

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
            sTab.Enter += new EventHandler(OnEnter);
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
