﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    public partial class MainForm : Form
    {

        //shared variables
        internal static MainForm mInstance;
        internal int sqlTabTrack;
        internal static Config cfg;
        internal string CurrentDB;
        internal TemplateManager tm;


        ToolTip toolTip;
        ArrayList RecentDBs = new ArrayList();
        internal struct RegisteredDB
        {
            internal string Name;
            internal string Password;
        }
        Dictionary<string, RegisteredDB> RegisteredDBs = new Dictionary<string, RegisteredDB>();

        //Context Menus
        ContextMenu dbContextMenu;
        ContextMenu tblContextMenu;
        ContextMenu tablesContextMenu;
        ContextMenu columnContextMenu;
        ContextMenu colContextMenu;
        ContextMenu viewsContextMenu;
        ContextMenu vwContextMenu;
        ContextMenu indexContextMenu;
        ContextMenu idxContextMenu;
        ContextMenu triggersContextMenu;
        ContextMenu trContextMenu;

        #region Program Initialization
        public MainForm()
        {
            InitializeComponent();
            cfg = new Config();
            this.MaximumSize = Screen.FromRectangle(this.Bounds).WorkingArea.Size;
            InitializeFormGUI();
            InitializeForm();
        }

        internal void InitializeForm()
        {
            mInstance = this;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            tabMain.Visible = false;
            toolStripStatusMain.Text = string.Empty;

            lblFormHeading.Text = Common.APPNAME;
            dbContextMenu = new ContextMenu();
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New Database", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Backup Database", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Clone Database", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Optimize Database", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Compress Database", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Encrypt Database", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem("-"));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Analyze Database", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem("-"));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Register", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null) { Name = "Register" });
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Attach", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Rebuild", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Restore", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem("-"));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Foreign Key List", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.F5, "Refresh", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));
            dbContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Properties", dbContextMenu_Clicked, dbContextMenu_Popup, dbContextMenu_Selected, null));


            tablesContextMenu = new ContextMenu();
            tablesContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New Table", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null));
            tablesContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Import Table", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null));
            tablesContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Refresh", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null));

            tblContextMenu = new ContextMenu();
            MenuItem tblScript = new MenuItem("Script Table As");
            tblScript.MenuItems.AddRange(new MenuItem[]
            {
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Create", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Drop", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Drop And Create", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem("-"),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Select", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Insert", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Update", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem("-"),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
            });

            tblContextMenu.MenuItems.AddRange(new MenuItem[] 
            {
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New Table", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Design", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Select Top 1000 rows", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Edit Top 1000 rows", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Edit Rows", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                tblScript,
                new MenuItem("-"),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Export", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem("-"),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Rename Table", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Truncate Table", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Refresh Table", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null),
                new MenuItem("-"),
                new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete Table", tblContextMenu_Clicked, tblContextMenu_Popup, tblContextMenu_Selected, null)

            });

            // Context Menu for Columns Parent Entry
            columnContextMenu = new ContextMenu();
            columnContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Add Column", colContextMenu_Clicked, colContextMenu_Popup, colContextMenu_Selected, null));

            // Context Menu for Column Entry
            colContextMenu = new ContextMenu();
            colContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Add Column", colContextMenu_Clicked, colContextMenu_Popup, colContextMenu_Selected, null));
            colContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Rename Column", colContextMenu_Clicked, colContextMenu_Popup, colContextMenu_Selected, null));
            colContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Modify Column", colContextMenu_Clicked, colContextMenu_Popup, colContextMenu_Selected, null));
            colContextMenu.MenuItems.Add(new MenuItem("-"));
            colContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete Column", colContextMenu_Clicked, colContextMenu_Popup, colContextMenu_Selected, null));

            // Context Menu for Index Parent Entry
            indexContextMenu = new ContextMenu();
            indexContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New Index", idxContextMenu_Clicked, idxContextMenu_Popup, idxContextMenu_Selected, null));
            indexContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Script All Indexes", idxContextMenu_Clicked, idxContextMenu_Popup, idxContextMenu_Selected, null));
            indexContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Rebuild All Indexes", idxContextMenu_Clicked, idxContextMenu_Popup, idxContextMenu_Selected, null));
            indexContextMenu.MenuItems.Add(new MenuItem("-"));
            indexContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete All Indexes", idxContextMenu_Clicked, idxContextMenu_Popup, idxContextMenu_Selected, null));


            // Context Menu for Index Entries
            idxContextMenu = new ContextMenu();
            idxContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New Index", idxContextMenu_Clicked, idxContextMenu_Popup, idxContextMenu_Selected, null));
            idxContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Script Index", idxContextMenu_Clicked, idxContextMenu_Popup, idxContextMenu_Selected, null));
            idxContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Rebuild Index", idxContextMenu_Clicked, idxContextMenu_Popup, idxContextMenu_Selected, null));
            idxContextMenu.MenuItems.Add(new MenuItem("-"));
            idxContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete Index", idxContextMenu_Clicked, idxContextMenu_Popup, idxContextMenu_Selected, null));

            // Context Menu for View Parent Entry
            viewsContextMenu = new ContextMenu();
            viewsContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New View", vwContextMenu_Clicked, vwContextMenu_Popup, vwContextMenu_Selected, null));
            viewsContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Refresh", vwContextMenu_Clicked, vwContextMenu_Popup, vwContextMenu_Selected, null));

            // Context Menu for View Entries
            vwContextMenu = new ContextMenu();
            vwContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New View", vwContextMenu_Clicked, vwContextMenu_Popup, vwContextMenu_Selected, null));
            vwContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Edit View", vwContextMenu_Clicked, vwContextMenu_Popup, vwContextMenu_Selected, null));
            vwContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Select Top 1000 Rows", vwContextMenu_Clicked, vwContextMenu_Popup, vwContextMenu_Selected, null));
            vwContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Script View", vwContextMenu_Clicked, vwContextMenu_Popup, vwContextMenu_Selected, null));
            vwContextMenu.MenuItems.Add(new MenuItem("-"));
            vwContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Export View", vwContextMenu_Clicked, vwContextMenu_Popup, vwContextMenu_Selected, null));
            vwContextMenu.MenuItems.Add(new MenuItem("-"));
            vwContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete View", vwContextMenu_Clicked, vwContextMenu_Popup, vwContextMenu_Selected, null));

            // Context Menu for Trigger Parent Entry
            triggersContextMenu = new ContextMenu();
            triggersContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New Trigger", trContextMenu_Clicked, trContextMenu_Popup, trContextMenu_Selected, null));
            triggersContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Refresh", trContextMenu_Clicked, trContextMenu_Popup, trContextMenu_Selected, null));

            // Context Menu for Trigger Entries
            trContextMenu = new ContextMenu();
            trContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "New Trigger", trContextMenu_Clicked, trContextMenu_Popup, trContextMenu_Selected, null));
            trContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Edit Trigger", trContextMenu_Clicked, trContextMenu_Popup, trContextMenu_Selected, null));
            trContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Script Trigger", trContextMenu_Clicked, trContextMenu_Popup, trContextMenu_Selected, null));
            trContextMenu.MenuItems.Add(new MenuItem("-"));
            trContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete Trigger", trContextMenu_Clicked, trContextMenu_Popup, trContextMenu_Selected, null));


            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbMin, "Minimize");
            toolTip.SetToolTip(pbMax, "Maximize");
            toolTip.SetToolTip(pbClose, "Close");
            sqlTabTrack = 0;
        }

        #endregion

        #region Form Management

        /// <summary>
        /// Read key GUI cues to make the program appear the same as it did at last closing.
        /// </summary>
        private void InitializeFormGUI()
        {
            if (Int32.TryParse(cfg.appsetting(Config.CFG_FTOP), out int parm)) this.Top = parm;
            if (Int32.TryParse(cfg.appsetting(Config.CFG_FLEFT), out parm)) this.Left = parm;
            if (Int32.TryParse(cfg.appsetting(Config.CFG_FHEIGHT), out parm)) this.Height = parm;
            if (Int32.TryParse(cfg.appsetting(Config.CFG_FWIDTH), out parm)) this.Width = parm;
            if (Int32.TryParse(cfg.appsetting(Config.CFG_VSPLITP), out parm)) vSplitter.SplitPosition = parm;
            if (Int32.TryParse(cfg.appsetting(Config.CFG_TSPLITP), out parm)) spTemplate.SplitPosition = Math.Max(parm, 100);
            if (Int32.TryParse(cfg.appsetting(Config.CFG_PSPLITP), out parm)) spProp.SplitPosition = parm;

            // Setting the Right Panel Visibility
#pragma warning disable IDE0018 // Inline variable declaration
            bool b = true;
#pragma warning restore IDE0018 // Inline variable declaration
            templatesToolStripMenuItem.Checked = bool.TryParse(cfg.appsetting(Config.CFG_TEMPLATESVISIBLE), out b) ? true : b;
            b = true;
            propertiesToolStripMenuItem.Checked = bool.TryParse(cfg.appsetting(Config.CFG_PROPSVISIBLE), out b) ? true : b;
            SetRightPanelStatus();

            string RecentDBList = cfg.appsetting(Config.CFG_RECENTDB);
            if (!string.IsNullOrEmpty(RecentDBList))
            {
                string[] rDBs = RecentDBList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < rDBs.Length; i++)
                {
                    RecentDBs.Add(rDBs[i]);
                }
                InitToolStripOpenDropDown(RecentDBs);

            }

            Common.password = "SQLite Workshop";
            LoadTemplates();
            LoadRegisteredDBs();
        }

        /// <summary>
        /// Save key GUI cues to be used at next program execution.
        /// </summary>

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cfg.SetSetting(Config.CFG_FTOP, this.Top.ToString());
            cfg.SetSetting(Config.CFG_FLEFT, this.Left.ToString());
            cfg.SetSetting(Config.CFG_FHEIGHT, this.Height.ToString());
            cfg.SetSetting(Config.CFG_FWIDTH, this.Width.ToString());
            cfg.SetSetting(Config.CFG_VSPLITP, vSplitter.SplitPosition.ToString());
            cfg.SetSetting(Config.CFG_TEMPLATESVISIBLE, panelTemplates.Visible.ToString());
            cfg.SetSetting(Config.CFG_PROPSVISIBLE, panelProperties.Visible.ToString());

            if (spTemplate.Visible) cfg.SetSetting(Config.CFG_TSPLITP, spTemplate.SplitPosition.ToString());
            if (spProp.Visible) cfg.SetSetting(Config.CFG_PSPLITP, spProp.SplitPosition.ToString());
        }

        #endregion

        #region Menu Handlers

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string db = FindDBFileLocation(false);
            if (!string.IsNullOrEmpty(db))
            {
                if (DataAccess.CreateDB(db)) LoadDB(db);
            }
        }

        private void databaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string db = FindDBFileLocation();
            if (!string.IsNullOrEmpty(db)) LoadDB(db);
        }

        private void sQLFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;
            SqlTab st = new SqlTab();
            st.BuildTab();
        }

        private void newQuerytoolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripTbNewQryWin_Click(sender, e);
        }

        private void savetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabMain.TabPages.Count == 0) return;
            TabPage tb = tabMain.SelectedTab;
            foreach (Control c in tb.Controls)
            {
                if (c.GetType().Equals(typeof(SqlTabControl)))
                {
                    ((SqlTabControl)c).SaveSql();
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentDB)) LoadDB(CurrentDB);
        }

        private void saveAstoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabMain.TabPages.Count == 0) return;
            TabPage tb = tabMain.SelectedTab;
            foreach (Control c in tb.Controls)
            {
                if (c.GetType().Equals(typeof(SqlTabControl)))
                {
                    ((SqlTabControl)c).SaveSql(true);
                }
            }
        }

        private void saveAsTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabMain.TabPages.Count == 0) return;
            TabPage tb = tabMain.SelectedTab;
            foreach (Control c in tb.Controls)
            {
                if (c.GetType().Equals(typeof(SqlTabControl)))
                {
                    tm.SaveTemplate((SqlTabControl)c);
                }
            }
        }

        private void saveAlltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabMain.TabPages.Count == 0) return;
            foreach (TabPage tb in tabMain.TabPages)
            {
                foreach (Control c in tb.Controls)
                {
                    if (c.GetType().Equals(typeof(SqlTabControl)))
                    {
                        ((SqlTabControl)c).SaveSql();
                    }
                }
            }
            WriteStatusStripMessage("All Sql statements saved.");
        }

        private void openRegisteredDBtoolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void templatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            templatesToolStripMenuItem.Checked = !panelTemplates.Visible;
            SetRightPanelStatus();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propertiesToolStripMenuItem.Checked = !propertiesToolStripMenuItem.Checked;
            SetRightPanelStatus();
            if (string.IsNullOrEmpty(CurrentDB)) return;
            if (propertiesToolStripMenuItem.Checked)
            {
                DBProperties p = new DBProperties();
                propertyGridDBProperties.SelectedObject = p.dbprops;
                propertyGridDBProperties.Refresh();
                propertyGridDBRuntime.SelectedObject = p.dbRT;
                propertyGridDBRuntime.Refresh();
            }
        }

        #endregion

        #region Toolbar Handlers

        #region Tool Strip DB
        private void toolStripDBNew_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem_Click(sender, e);
        }

        private void toolStripOpen_Click(object sender, EventArgs e)
        {
            databaseToolStripMenuItem_Click(sender, e);
        }

        private void toolStripDBRefresh_Click(object sender, EventArgs e)
        {
            refreshToolStripMenuItem_Click(sender, e);
        }

        private void toolStripDBOpenDropDownItem_Click(object sender, EventArgs e)
        {
            LoadDB(((ToolStripMenuItem)sender).ToolTipText);
        }

        private void toolStripDBCompress_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;

            ExecuteForm ef = new ExecuteForm
            {
                execType = SQLType.SQLCompress,
                TargetNode = null,
                DatabaseLocation = CurrentDB
            };
            ef.ShowDialog();
        }

        private void toolStripDBEncrypt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;

            ExecuteForm ef = new ExecuteForm
            {
                execType = SQLType.SQLEncrypt,
                TargetNode = null,
                DatabaseLocation = CurrentDB
            };
            ef.ShowDialog();
        }

        private void toolStripDBIntegrityCheck_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;

            SqlTab st = new SqlTab();
            st.BuildTab(CurrentDB, SQLType.SQLIntegrityCheck);
        }

        private void toolStripDBAnalyze_Click(object sender, EventArgs e)
        {
            DBAnalyze analyze = new DBAnalyze(CurrentDB);
            analyze.ShowDialog();
        }
        #endregion

        #region Tool Strip Ops
        private void toolStripOpsExecute_Click(object sender, EventArgs e)
        {
            if (tabMain.TabPages.Count == 0) return;
            if (tabMain.SelectedTab == null) return;
            TabPage activeTab = tabMain.SelectedTab;
            if (activeTab == null) return;

            foreach (Control c in activeTab.Controls)
            {
                string cType = c.GetType().ToString();
                switch (cType)
                {
                    case "SQLiteWorkshop.SqlTabControl":
                        ((SqlTabControl)c).Execute();
                        break;
                    case "SQLiteWorkshop.DBEditorControl":
                        break;
                    default:
                        break;
                }
            }

        }

        private void toolStripOpsCancel_Click(object sender, EventArgs e)
        {
            if (tabMain.TabPages.Count == 0) return;
            TabPage activeTab = tabMain.SelectedTab;
            foreach (Control c in activeTab.Controls)
            {
                string cType = c.GetType().ToString();
                switch (cType)
                {
                    case "SQLiteWorkshop.SqlTabControl":
                        ((SqlTabControl)c).CancelExecution = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void toolStripOpsExplain_Click(object sender, EventArgs e)
        {
            const bool Explain = true;
            const bool ExplainQueryPlan = false;
            bool ExplainType;

            if (tabMain.TabPages.Count == 0) return;
            TabPage activeTab = tabMain.SelectedTab;

            ExplainType = ((ToolStripButton)sender).Text.ToLower() == "explain" ? Explain : ExplainQueryPlan;
            foreach (Control c in activeTab.Controls)
            {
                string cType = c.GetType().ToString();
                switch (cType)
                {
                    case "SQLiteWorkshop.SqlTabControl":
                        ((SqlTabControl)c).Explain(ExplainType);
                        break;
                    default:
                        break;
                }
            }
        }

        private void toolStripOpsParse_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Tool Strip Table
        private void toolStripTbNewQryWin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;
            SqlTab st = new SqlTab();
            st.BuildTab(SQLType.SQLNewQuery);
        }
        private void toolStripTbCreateTable_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;
            TableEditorTab tbt = new TableEditorTab();
            tbt.BuildTab();
        }

        private void toolStripTbCreateView_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;
            SqlTab st = new SqlTab();
            st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLCreateView);
        }

        private void toolStripTbCreateIndex_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;
            BuildIndex bi = new BuildIndex();

            if (treeViewMain.SelectedNode != null)
            {
                TreeNode tgtNode = treeViewMain.SelectedNode;
                while (tgtNode.Parent != null)
                {
                    if (tgtNode.Parent.Text == "Tables")
                    {
                        bi.TableName = tgtNode.Text;
                        break;
                    }
                    tgtNode = tgtNode.Parent;
                }
            }
            bi.Show();
        }

        private void toolStripTbCreateTrigger_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;
            SqlTab st = new SqlTab();
            st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLCreateTrigger);
        }
        #endregion

        #region Tool Strip Import/Export
        private void toolStripToolImport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;
            ImportWiz iWiz = new ImportWiz
            {
                DatabaseLocation = CurrentDB,
                ShowInTaskbar = true
            };
            iWiz.ShowDialog();
        }

        private void toolStripToolExport_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentDB)) return;
            ExportWiz eWiz = new ExportWiz
            {
                DatabaseLocation = CurrentDB,
                ShowInTaskbar = true
            };
            eWiz.ShowDialog();
        }
        #endregion

        #endregion

        #region Context Menu Handlers
        #region Database Context Menu Handlers
        private void dbContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void dbContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void dbContextMenu_Clicked(object sender, EventArgs e)
        {
            ExecuteForm ef;
            switch (((MenuItem)sender).Text.ToLower())
            {
                case "new database":
                    newToolStripMenuItem_Click(sender, e);
                    break;
                case "backup database":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLBackup,
                        TargetNode = null,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    break;
                case "optimize database":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLOptimize,
                        TargetNode = null,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    break;
                case "clone database":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLClone,
                        TargetNode = null,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    break;
                case "compress database":
                    toolStripDBCompress_Click(sender, e);
                    break;
                case "encrypt database":
                    toolStripDBEncrypt_Click(sender, e);
                    break;
                case "analyze database":
                    toolStripDBAnalyze_Click(sender, e);
                    break;
                case "create":
                    newToolStripMenuItem_Click(sender, e);
                    break;
                case "delete":
                    Common.ShowMsg(Common.NOTIMPLEMENTED);
                    break;
                case "properties":
                    propertiesToolStripMenuItem_Click(sender, e);
                    break;
                case "refresh":
                    LoadDB(CurrentDB);
                    break;
                case "register":
                    RegisterDB();
                    break;
                case "unregister":
                    UnRegisterDB();
                    break;
                case "rebuild":
                    Common.ShowMsg(Common.NOTIMPLEMENTED);
                    break;
                case "attach":
                    Common.ShowMsg(Common.NOTIMPLEMENTED);
                    break;
                case "restore":
                    Common.ShowMsg(Common.NOTIMPLEMENTED);
                    break;
                case "foreign key list":
                    FKList fk = new FKList();
                    fk.Show();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Table Context Menu Handlers
        private void tblContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void tblContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void tblContextMenu_Clicked(object sender, EventArgs e)
        {
            SqlTab st;
            DBEditorTab dbEditor;
            ExecuteForm ef;
            switch (((MenuItem)sender).Text.ToLower())
            {
                case "new table":
                    toolStripTbCreateTable_Click(sender, e);
                    break;
                case "import table":
                    toolStripToolImport_Click(sender, e);
                    break;
                case "create":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenCreate);
                    break;
                case "drop":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenDrop);
                    break;
                case "drop and create":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenDropAndCreate);
                    break;
                case "select":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenSelect);
                    break;
                case "insert":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenInsert);
                    break;
                case "update":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenUpdate);
                    break;
                case "delete":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenDelete);
                    break;

                case "select top 1000 rows":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLSelect1000);
                    break;
                case "edit top 1000 rows":
                    if (DBEditInProgress(treeViewMain.SelectedNode)) return;
                    dbEditor = new DBEditorTab();
                    dbEditor.BuildTab(treeViewMain.SelectedNode);
                    break;
                case "edit rows":
                    if (RowEditInProgress(treeViewMain.SelectedNode)) return;
                    RecordEditorTab RecEditor = new RecordEditorTab();
                    this.Cursor = Cursors.WaitCursor;
                    RecEditor.BuildTab(treeViewMain.SelectedNode);
                    this.Cursor = Cursors.Default;
                    break;
                case "rename table":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLRename,
                        TargetNode = treeViewMain.SelectedNode
                    };
                    ef.ShowDialog();
                    if (ef.bActionComplete)
                    {
                        DataAccess.RemoveTableFromSchema(CurrentDB, treeViewMain.SelectedNode.Text);
                        DataAccess.AddTableToSchema(CurrentDB, ef.NewTableName);
                        TreeNode newNode = BuildTableNode(ef.NewTableName);
                        TreeNode[] tblMainNodes = treeViewMain.Nodes.Find("Tables", true);
                        TreeNode tblNode = tblMainNodes[0].Nodes[treeViewMain.SelectedNode.Name];
                        int idx = tblMainNodes[0].Nodes.IndexOf(tblNode);
                        tblMainNodes[0].Nodes.RemoveAt(idx);
                        tblMainNodes[0].Nodes.Insert(idx, newNode);
                    }
                    break;
                case "truncate table":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLTruncate,
                        TargetNode = treeViewMain.SelectedNode
                    };
                    ef.ShowDialog();
                    break;

                case "delete table":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLDrop,
                        TargetNode = treeViewMain.SelectedNode
                    };
                    ef.ShowDialog();
                    if (ef.bActionComplete)
                    {
                        DataAccess.RemoveTableFromSchema(CurrentDB, treeViewMain.SelectedNode.Text);
                        treeViewMain.Nodes.Remove(treeViewMain.SelectedNode);
                    }
                    break;
                case "export":
                    ExportWiz exp = new ExportWiz
                    {
                        DatabaseLocation = CurrentDB,
                        TableName = treeViewMain.SelectedNode.Text
                    };
                    exp.ShowDialog();
                    break;
                case "refresh":
                    RefreshTables();
                    break;
                case "refresh table":
                    AddTable(treeViewMain.SelectedNode.Text);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Column Context Menu Handlers
        private void colContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void colContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void colContextMenu_Clicked(object sender, EventArgs e)
        {
            BuildColumn bc = new BuildColumn
            {
                TargetNode = treeViewMain.SelectedNode,
                DatabaseLocation = CurrentDB
            };
            switch (((MenuItem)sender).Text.ToLower())
            {
                case "add column":
                    bc.ExecType = SQLType.SQLAddColumn;
                    break;
                case "delete column":
                    bc.ExecType = SQLType.SQLDeleteColumn;
                    break;
                case "modify column":
                    bc.ExecType = SQLType.SQLModifyColumn;
                    break;
                case "rename column":
                    bc.ExecType = SQLType.SQLRenameColumn;
                    break;               
                default:
                    break;
            }
            bc.ShowDialog();
        }
        #endregion

        #region Index Context Menu Handlers
        private void idxContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void idxContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void idxContextMenu_Clicked(object sender, EventArgs e)
        {
            SqlTab st;
            //DBEditorTab dbEditor;
            //TableEditorTab tbt;
            ExecuteForm ef;
            switch (((MenuItem)sender).Text.ToLower())
            {
                case "script index":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenIndex);
                    break;
                case "script all indexes":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenAllIndexes); break;
                case "rebuild index":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLRebuildIndex,
                        TargetNode = treeViewMain.SelectedNode,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    break;
                case "rebuild all indexes":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLRebuildAllIndexes,
                        TargetNode = treeViewMain.SelectedNode,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    break;
                case "delete index":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLDeleteIndex,
                        TargetNode = treeViewMain.SelectedNode,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    if (ef.bActionComplete) ReplaceTreeEntry();
                    break;
                case "delete all indexes":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLDeleteAllIndexes,
                        TargetNode = treeViewMain.SelectedNode,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    if (ef.bActionComplete) ReplaceTreeEntry();
                    break;
                case "new index":
                    BuildIndex bi = new BuildIndex
                    {
                        TableName = treeViewMain.SelectedNode.Text == "Indexes" ? treeViewMain.SelectedNode.Parent.Text : treeViewMain.SelectedNode.Parent.Parent.Text
                    };
                    bi.Show();
                    break;
                default:
                    break;
            }
        }

        private void ReplaceTreeEntry()
        {
            string tableName = treeViewMain.SelectedNode.Text == "Indexes" ? treeViewMain.SelectedNode.Parent.Text : treeViewMain.SelectedNode.Parent.Parent.Text;
            AddTable(tableName);
        }
        #endregion

        #region Views Context Menu Handlers
        private void vwContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void vwContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void vwContextMenu_Clicked(object sender, EventArgs e)
        {
            SqlTab st;
            //DBEditorTab dbEditor;
            //TableEditorTab tbt;
            ExecuteForm ef;
            switch (((MenuItem)sender).Text.ToLower())
            {
                case "script view":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenViewCreate);
                    break;
                case "edit view":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLEditView);
                    break;
                case "select top 1000 rows":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLSelect1000View); break;
                case "new view":
                    toolStripTbCreateView_Click(sender, e);
                    break;
                case "export view":
                    ExportWiz exp = new ExportWiz
                    {
                        DatabaseLocation = CurrentDB,
                        TableName = treeViewMain.SelectedNode.Text
                    };
                    exp.ShowDialog();
                    break;
                case "delete view":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLDeleteView,
                        TargetNode = treeViewMain.SelectedNode,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    break;
                case "refresh":
                    RefreshViews();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Triggers Context Menu Handlers
        private void trContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void trContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void trContextMenu_Clicked(object sender, EventArgs e)
        {
            SqlTab st;
            //DBEditorTab dbEditor;
            //TableEditorTab tbt;
            ExecuteForm ef;
            switch (((MenuItem)sender).Text.ToLower())
            {
                case "script trigger":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLGenTriggerCreate);
                    break;
                case "edit trigger":
                    st = new SqlTab();
                    st.BuildTab(treeViewMain.SelectedNode, SQLType.SQLEditTrigger);
                    break;
                case "new trigger":
                    toolStripTbCreateTrigger_Click(sender, e);
                    break;
                case "delete trigger":
                    ef = new ExecuteForm
                    {
                        execType = SQLType.SQLDeleteTrigger,
                        TargetNode = treeViewMain.SelectedNode,
                        DatabaseLocation = CurrentDB
                    };
                    ef.ShowDialog();
                    break;
                case "refresh":
                    RefreshTriggers(); ;
                    break;
                default:
                    break;
            }
        }
        #endregion
        #endregion

        #region TreeView Handlers
        private void treeViewMain_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeViewMain.SelectedNode != null) treeViewMain.SelectedNode.BackColor = Color.White;
        }

        private void treeViewMain_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ((TreeView)sender).SelectedNode = e.Node;
        }

        #endregion

        #region Template Treeview Handlers

        internal void LoadTemplates()
        {
            tm = new TemplateManager(treeTemplates);
        }

        private void treeTemplates_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode sqlNode = treeTemplates.SelectedNode;
            if (sqlNode == null) return;
            if (sqlNode.Tag == null) return;
            if (string.IsNullOrEmpty(CurrentDB)) return;

            FileInfo fi = new FileInfo(sqlNode.Tag.ToString());
            if (!fi.Exists)
            {
                Common.ShowMsg(string.Format("{0} sql is no longer available.", sqlNode.Text));
                return;
            }

            SqlTab st = new SqlTab();
            st.BuildTab(fi);

        }

        private void treeTemplates_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode t = treeTemplates.GetNodeAt(e.Location);
            if (t != null) treeTemplates.SelectedNode = t;
        }
            

        #endregion

        #region Helpers
        private string FindDBFileLocation(bool bFileExists = true)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Open SQLite DB",
                Filter = "All files (*.*)|*.*|Database Files (*.db)|*.db",
                FilterIndex = 2,
                CheckFileExists = bFileExists,
                AddExtension = true,
                AutoUpgradeEnabled = true,
                DefaultExt = "db",
                InitialDirectory = cfg.appsetting(Config.CFG_LASTOPEN),
                Multiselect = false,
                ShowReadOnly = false,
                ValidateNames = true
            };
            if (openFile.ShowDialog() != DialogResult.OK) return string.Empty;
            cfg.SetSetting(Config.CFG_LASTOPEN, Path.GetDirectoryName(openFile.FileName));
            return openFile.FileName;
        }

        /// <summary>
        /// Determine if a table editor for a specific table is open and, if yes,
        /// display the corresponding tab.
        /// </summary>
        /// <param name="tablename">Table being edited.</param>
        /// <returns></returns>
        protected bool DBEditInProgress(TreeNode tn)
        {
            string tablename = tn.Text;
            foreach (TabPage tb in tabMain.TabPages)
            {
                foreach (Control c in tb.Controls)
                {
                    if (c.GetType().Equals(typeof(DBEditorTabControl)))
                    {
                        if (((DBEditorTabControl)c).TableName == tablename)
                        {
                            tabMain.SelectedTab = tb;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determine if a Record editor for a specific table is open and, if yes,
        /// display the corresponding tab.
        /// </summary>
        /// <param name="tablename">Table being edited.</param>
        /// <returns></returns>
        protected bool RowEditInProgress(TreeNode tn)
        {
            string tablename = tn.Text;
            foreach (TabPage tb in tabMain.TabPages)
            {
                foreach (Control c in tb.Controls)
                {
                    if (c.GetType().Equals(typeof(RecordEditTabControl)))
                    {
                        if (((RecordEditTabControl)c).TableName == tablename)
                        {
                            tabMain.SelectedTab = tb;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Rebuild/Refresh the GridView
        /// </summary>
        /// <param name="DBLocation"></param>
        private void LoadDB(string DBLocation)
        {
            if (DBLocation != CurrentDB)
            {
                tabMain.TabPages.Clear();
                tabMain.Visible = false;
            }

            CurrentDB = DBLocation;
            toolStripStatusMain.Text = CurrentDB;
            BuildTreeView(DBLocation);
            if (RecentDBs.Contains(DBLocation)) RecentDBs.Remove(DBLocation);
            RecentDBs.Add(DBLocation);
            if (RecentDBs.Count > 6) RecentDBs.RemoveAt(0);
            SaveQSetting(RecentDBs);
        }

        protected void SaveQSetting(ArrayList dbQ)
        {
            string RecentDBs = string.Join("|", dbQ.ToArray());
            cfg.SetSetting(Config.CFG_RECENTDB, RecentDBs);
            InitToolStripOpenDropDown(dbQ);
        }

        protected void InitToolStripOpenDropDown(ArrayList dbQ)
        {
            //Note that the 'New...' item uses a different Event Handler
            toolStripDBOpenDropDown.DropDownItems.Clear();
            toolStripDBOpenDropDown.DropDownItems.Add("New...", null, toolStripOpen_Click);
            databaseToolStripMenuItem.DropDown.Items.Clear();
            databaseToolStripMenuItem.DropDownItems.Add("New...", null, toolStripOpen_Click);

            for (int i = dbQ.Count; i > 0; --i)
            {
                ToolStripMenuItem tdi = new ToolStripMenuItem();
                string dbName = dbQ[i-1].ToString();
                tdi.ToolTipText = dbName;
                tdi.Text = dbName.Substring(dbName.LastIndexOf('\\') + 1);
                tdi.Click += toolStripDBOpenDropDownItem_Click;
                toolStripDBOpenDropDown.DropDownItems.Add(tdi);

                ToolStripMenuItem tdj = new ToolStripMenuItem
                {
                    ToolTipText = dbName,
                    Text = dbName.Substring(dbName.LastIndexOf('\\') + 1)
                };
                tdj.Click += toolStripDBOpenDropDownItem_Click;
                databaseToolStripMenuItem.DropDownItems.Add(tdj);
            }
        }

        internal void InitToolStripRegisteredDBDropDown(Dictionary<string, RegisteredDB> dbQ)
        {
            openRegisteredDBtoolStripMenuItem.DropDownItems.Clear();

            var sortedDictionary = from entry in dbQ orderby entry.Value.Name ascending select entry;

            foreach (var rDB in sortedDictionary)
            {
                ToolStripMenuItem tdi = new ToolStripMenuItem
                {
                    ToolTipText = rDB.Key,
                    Text = rDB.Value.Name,
                    Tag = rDB.Value.Password
                };
                tdi.Click += toolStripDBOpenDropDownItem_Click;
                openRegisteredDBtoolStripMenuItem.DropDownItems.Add(tdi);
            }
        }

        protected void BuildTreeView(string DBLocation)
        {

            treeViewMain.Nodes.Clear();

            SchemaDefinition sd = DataAccess.GetSchema(DBLocation);
            if (sd.LoadStatus != 0)
            {
                Common.ShowMsg(string.Format("The database {0} cannot be loaded.\r\n{1}", CurrentDB, DataAccess.LastError));
                return;
            }

            TreeNode topNode = new TreeNode(sd.DBName, 0, 0)
            {
                ContextMenu = dbContextMenu
            };
            MenuItem mi = topNode.ContextMenu.MenuItems["Register"];
            mi.Text = RegisteredDBs.Keys.Contains(DBLocation) ? "Unregister" : "Register";

            // Add System Tables & Tables to Treeview
            TreeNode systablesNode = new TreeNode("System Tables", 2, 2)
            {
                Name = "SysTables",
                Tag = DBLocation
            };

            TreeNode tablesNode = new TreeNode("Tables", 2, 2)
            {
                Name = "Tables",
                Tag = DBLocation,
                ContextMenu = tablesContextMenu
            };

            if (sd.Tables.Count > 0)
            {
                foreach (var table in sd.Tables)
                {
                    TreeNode tableNode = BuildTableNode(table.Key, table.Value);
                    // Add completed node to master table node
                    if (table.Value.TblType == SQLiteTableType.user ) tablesNode.Nodes.Add(tableNode);
                    if (table.Value.TblType == SQLiteTableType.system) systablesNode.Nodes.Add(tableNode);
                }
            }

            // Add views to TreeView

            TreeNode viewsNode = BuildViewNode();
            TreeNode triggersNode = BuildTriggerNode();

            topNode.Nodes.Add(systablesNode);
            topNode.Nodes.Add(tablesNode);
            topNode.Nodes.Add(viewsNode);
            topNode.Nodes.Add(triggersNode);

            tablesNode.Expand();
            viewsNode.Expand();
            triggersNode.Expand();
            topNode.Expand();
            treeViewMain.Nodes.Add(topNode);

            DBProperties p = new DBProperties();
            propertyGridDBProperties.SelectedObject = p.dbprops;
            propertyGridDBProperties.Refresh();
            propertyGridDBRuntime.SelectedObject = p.dbRT;
            propertyGridDBRuntime.Refresh();

        }

        internal void AddTable(string table)
        {
            if (!DataAccess.AddTableToSchema(CurrentDB, table)) return;

            TreeNode mainNode = treeViewMain.Nodes[0];
            TreeNode[] tblMainNodes = mainNode.Nodes.Find("Tables", false);
            TreeNode[] tblNodes = tblMainNodes[0].Nodes.Find(table, false);
            TreeNode newNode = BuildTableNode(table);
            if (tblNodes.Count() == 0)
            {
                tblMainNodes[0].Nodes.Add(newNode);
                return;
            }
            TreeNode parentNode = tblNodes[0].Parent;
            TreeNode tblNode = parentNode.Nodes[table];
            int idx = parentNode.Nodes.IndexOf(tblNode);
            parentNode.Nodes.RemoveAt(idx);
            parentNode.Nodes.Insert(idx, newNode);
            treeViewMain.SelectedNode = newNode;
        }

        
        internal TreeNode BuildTableNode(string table)
        {
            SchemaDefinition sd = DataAccess.SchemaDefinitions[CurrentDB];
            TableLayout tl = sd.Tables[table];
            if (tl.Equals(null)) return null;
            return BuildTableNode(table, tl);

        }

        internal TreeNode BuildTableNode(string table, TableLayout tl)
        {
            string[] PK = new string[tl.Columns.Count];
            TreeNode tableNode = new TreeNode(table, 3, 3)
            {
                Name = table,
                ContextMenu = tblContextMenu
            };

            // Process Columns
            TreeNode columnNode = new TreeNode("Columns", 2, 2)
            {
                ContextMenu = columnContextMenu
            };
            tableNode.Nodes.Add(columnNode);
            foreach (var column in tl.Columns)
            {
                string nul = column.Value.NullType == 0 ? "null" : "not null";
                string pk = column.Value.PrimaryKey == 0 ? string.Empty : ", PK";
                PK[column.Value.PrimaryKey] = column.Value.PrimaryKey == 0 ? string.Empty : column.Key;
                string df = string.IsNullOrEmpty(column.Value.DefaultValue) ? string.Empty : string.Format(", Default: {0}", column.Value.DefaultValue);
                TreeNode colNode = new TreeNode(string.Format("{0} ({1}, {2}{3}{4})", column.Key, column.Value.ColumnType, nul, pk, df), 4, 4)
                {
                    Tag = column.Key,
                    ContextMenu = colContextMenu
                };
                columnNode.Nodes.Add(colNode);
            }

            // Process Keys
            TreeNode KeyNode = new TreeNode("Keys", 2, 2);
            tableNode.Nodes.Add(KeyNode);
            // Add Primary key if it exists
            if (PK.Length > 1 && !string.IsNullOrEmpty(PK[1]))
            {
                string pkText = string.Format("Primary Key ({0}", PK[1]);
                for (int i = 2; i < PK.Length; i++)
                {
                    if (!string.IsNullOrEmpty(PK[i]))
                    { pkText = string.Format("{0},{1}", pkText, PK[i]); }
                    else break;
                }
                pkText += ")";
                TreeNode pkNode = new TreeNode(pkText, 7, 7);
                KeyNode.Nodes.Add(pkNode);
            }
            foreach (var foreignKey in tl.ForeignKeys)
            {
                TreeNode fkNode = new TreeNode(string.Format("Foriegn Key ({0} : {1} ({2}))", foreignKey.Value.From, foreignKey.Value.Table, foreignKey.Value.To), 7, 7);
                TreeNode fkTblNode = new TreeNode(string.Format("Parent Table: {0}", foreignKey.Value.Table), 4, 4);
                TreeNode fkFromNode = new TreeNode(string.Format("Parent Column: {0}", foreignKey.Value.To), 4, 4);
                TreeNode fkToNode = new TreeNode(string.Format("Column: {0}", foreignKey.Value.From), 4, 4);
                TreeNode fkSeqNode = new TreeNode(string.Format("Sequence: {0}", foreignKey.Value.Sequence), 4, 4);
                TreeNode fkUpdNode = new TreeNode(string.Format("On Update: {0}", foreignKey.Value.OnUpdate), 4, 4);
                TreeNode fkDelNode = new TreeNode(string.Format("On Delete: {0}", foreignKey.Value.OnDelete), 4, 4);
                TreeNode fkMatchNode = new TreeNode(string.Format("Match: {0}", foreignKey.Value.Match), 4, 4);
                fkNode.Nodes.AddRange(new TreeNode[] { fkTblNode, fkFromNode, fkToNode, fkSeqNode, fkUpdNode, fkDelNode, fkMatchNode });
                KeyNode.Nodes.Add(fkNode);
            }

            // Process Indexes
            TreeNode indexNode = new TreeNode("Indexes", 2, 2)
            {
                ContextMenu = indexContextMenu
            };
            tableNode.Nodes.Add(indexNode);
            foreach (var index in tl.Indexes)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(index.Key);
                if (index.Value.Unique) sb.Append(", Unique");
                if (index.Value.Origin == "pk") sb.Append(", PK");
                if (index.Value.Partial) sb.Append(", Partial");

                TreeNode IdxNode = new TreeNode(sb.ToString(), 5, 5)
                {
                    Tag = index.Key
                };
                indexNode.Nodes.Add(IdxNode);
                indexNode.Nodes[indexNode.Nodes.Count - 1].ContextMenu = idxContextMenu;
                foreach (var IndexCol in index.Value.Columns)
                {
                    string IdxColNodeName = string.Format("{0}, {1}, {2}", IndexCol.Key, IndexCol.Value.SortOrder, IndexCol.Value.CollatingSequence);
                    TreeNode IdxColNode = new TreeNode(IdxColNodeName, 7, 7);
                    IdxNode.Nodes.Add(IdxColNode);
                }
            }
            return tableNode;
        }
        
        protected void RefreshTables()
        {
            DataAccess.ReloadTables(CurrentDB);
            TreeNode tablesNode = BuildTablesNode();
            if (tablesNode == null) return;

            TreeNode[] tblMainNodes = treeViewMain.Nodes.Find("Tables", true);
            TreeNode parentNode = tblMainNodes[0].Parent;
            int idx = parentNode.Nodes.IndexOf(tblMainNodes[0]);
            parentNode.Nodes.RemoveAt(idx);
            parentNode.Nodes.Insert(idx, tablesNode);
            treeViewMain.SelectedNode = tablesNode;
            tablesNode.Expand();
        }

        protected TreeNode BuildTablesNode()
        {
            SchemaDefinition sd = DataAccess.GetSchema(CurrentDB);
            if (sd.LoadStatus != 0)
            {
                Common.ShowMsg(string.Format("The database {0} cannot be loaded.\r\n{1}", CurrentDB, DataAccess.LastError));
                return null;
            }

            TreeNode tablesNode = new TreeNode("Tables", 2, 2)
            {
                Name = "Tables",
                Tag = CurrentDB,
                ContextMenu = tablesContextMenu
            };

            if (sd.Tables.Count > 0)
            {
                foreach (var table in sd.Tables)
                {
                    TreeNode tableNode = BuildTableNode(table.Key, table.Value);
                    // Add completed node to master table node
                    if (table.Value.TblType == SQLiteTableType.user) tablesNode.Nodes.Add(tableNode);
                    //if (table.Value.TblType == SQLiteTableType.system) systablesNode.Nodes.Add(tableNode);
                }
            }
            return tablesNode;
        }
        protected void RefreshViews()
        {
            DataAccess.ReloadViews(CurrentDB);
            TreeNode viewsNode = BuildViewNode();
            TreeNode[] tblMainNodes = treeViewMain.Nodes.Find("Views", true);
            TreeNode parentNode = tblMainNodes[0].Parent;
            int idx = parentNode.Nodes.IndexOf(tblMainNodes[0]);
            parentNode.Nodes.RemoveAt(idx);
            parentNode.Nodes.Insert(idx, viewsNode);
            treeViewMain.SelectedNode = viewsNode;
            viewsNode.Expand();
        }

        protected TreeNode BuildViewNode()
        {
            SchemaDefinition sd = DataAccess.GetSchema(CurrentDB);
            TreeNode viewsNode = new TreeNode("Views", 2, 2)
            {
                Name = "Views",
                ContextMenu = viewsContextMenu
            };
            if (sd.Views.Count > 0)
            {
                foreach (var view in sd.Views)
                {
                    string[] PKV = new string[view.Value.Columns.Count];
                    TreeNode vwNode = new TreeNode(view.Key, 6, 6)
                    {
                        ContextMenu = vwContextMenu
                    };
                    viewsNode.Nodes.Add(vwNode);
                    // Process Columns
                    TreeNode columnNode = new TreeNode("Columns", 2, 2);
                    vwNode.Nodes.Add(columnNode);
                    foreach (var column in view.Value.Columns)
                    {
                        string nul = column.Value.NullType == 0 ? "null" : "not null";
                        string pk = column.Value.PrimaryKey == 0 ? string.Empty : ", PK";
                        PKV[column.Value.PrimaryKey] = column.Value.PrimaryKey == 0 ? string.Empty : column.Key;
                        string df = string.IsNullOrEmpty(column.Value.DefaultValue) ? string.Empty : string.Format(", Default: {0}", column.Value.DefaultValue);
                        TreeNode colNode = new TreeNode(string.Format("{0} ({1}, {2}{3}{4})", column.Key, column.Value.ColumnType, nul, pk, df), 4, 4)
                        {
                            Tag = column.Key
                        };
                        columnNode.Nodes.Add(colNode);
                    }
                }
            }
            return viewsNode;
        }

        protected void RefreshTriggers()
        {
            //DataAccess.AddTableToSchema(CurrentDB, ef.NewTableName);
            TreeNode triggersNode = BuildTriggerNode();
            TreeNode[] tblMainNodes = treeViewMain.Nodes.Find("Triggers", true);
            TreeNode parentNode = tblMainNodes[0].Parent;
            int idx = parentNode.Nodes.IndexOf(tblMainNodes[0]);
            parentNode.Nodes.RemoveAt(idx);
            parentNode.Nodes.Insert(idx, triggersNode);
            treeViewMain.SelectedNode = triggersNode;
            triggersNode.Expand();
        }

        protected TreeNode BuildTriggerNode()
        {
            SchemaDefinition sd = DataAccess.GetSchema(CurrentDB);
            TreeNode triggersNode = new TreeNode("Triggers", 2, 2)
            {
                Name = "Triggers",
                ContextMenu = triggersContextMenu
            };
            if (sd.Triggers.Count > 0)
            {
                foreach (var trigger in sd.Triggers)
                {
                    TreeNode trNode = new TreeNode(trigger.Key, 8, 8)
                    {
                        ContextMenu = trContextMenu
                    };
                    triggersNode.Nodes.Add(new TreeNode(trigger.Key, 8, 8) { ContextMenu = trContextMenu });
                }
            }
            return triggersNode;
        }


        /// <summary>
        /// Place a message on the status bar
        /// </summary>
        /// <param name="message">Message to display on the Status Bar</param>
        internal void WriteStatusStripMessage(string message)
        {
            toolStripStatusMain.Text = message;
        }

        /// <summary>
        /// Register the Current Database.
        /// </summary>
        internal void RegisterDB()
        {
            if (RegisteredDBs.Keys.Contains(CurrentDB)) return;

            RegisteredDB rDB = new RegisteredDB
            {
                Name = Path.GetFileName(CurrentDB),
                Password = string.Empty            //when implemented, always keep this encrypted
            };

            RegisteredDBs.Add(CurrentDB, rDB);
            SaveRegisteredDBSetting(RegisteredDBs);

            MenuItem mi = treeViewMain.TopNode.ContextMenu.MenuItems["Register"];
            mi.Text = "Unregister";

            WriteStatusStripMessage(string.Format("Database {0} has been added to the SQLite Workshop registry.", CurrentDB));
        }

        internal void UnRegisterDB()
        {
            if (!RegisteredDBs.Keys.Contains(CurrentDB)) return;

            RegisteredDBs.Remove(CurrentDB);
            SaveRegisteredDBSetting(RegisteredDBs);

            MenuItem mi = treeViewMain.TopNode.ContextMenu.MenuItems["Register"];
            mi.Text = "Register";

            WriteStatusStripMessage(string.Format("Database {0} has been deleted from the SQLite Workshop registry.", CurrentDB));
        }

        internal void SaveRegisteredDBSetting(Dictionary<string, RegisteredDB> dbQ)
        {
            StringBuilder sb = new StringBuilder();
            string comma = string.Empty;
            
            foreach (var rDB in dbQ)
            {
                string token = string.Format("{0}|{1}|{2}", rDB.Key, rDB.Value.Name, rDB.Value.Password);
                sb.Append(comma).Append(Common.Encrypt(token));
                comma = ";";
            }
            cfg.SetSetting(Config.CFG_REGISTEREDDBS, sb.ToString());
            InitToolStripRegisteredDBDropDown(dbQ);
        }

        internal void LoadRegisteredDBs()
        {
            RegisteredDBs.Clear();
            string rDBList = cfg.appsetting(Config.CFG_REGISTEREDDBS);
            if (string.IsNullOrEmpty(rDBList)) return;

            string[] rDBs = rDBList.Split(';');
            foreach (string r in rDBs)
            {
                string token = Common.Decrypt(r);
                string[] tokens = token.Split('|');
                RegisteredDB rDB = new RegisteredDB
                {
                    Name = tokens[1],
                    Password = tokens[2]
                };
                RegisteredDBs.Add(tokens[0], rDB);
            }
            InitToolStripRegisteredDBDropDown(RegisteredDBs);
        }

        internal void SetRightPanelStatus()
        {
            if (templatesToolStripMenuItem.Checked || propertiesToolStripMenuItem.Checked)
            {
                panelRight.Visible = true;
                spTemplate.Visible = true;
                panelProperties.Visible = propertiesToolStripMenuItem.Checked;
                panelTemplates.Visible = templatesToolStripMenuItem.Checked;
                spProp.Visible = templatesToolStripMenuItem.Checked && propertiesToolStripMenuItem.Checked;
                panelTemplates.Dock = propertiesToolStripMenuItem.Checked ? DockStyle.Top : DockStyle.Fill;
            }
            else
            {
                panelRight.Visible = false;
                spTemplate.Visible = false;
            }
        }

#endregion

#region Form Sizing and Control

        bool grabbed = false;
        int minWidth = 500;
        int minHeight = 200;
        
        private void sp_MouseDown(object sender, MouseEventArgs e)
        {
            grabbed = true;
        }

        private void sp_MouseUp(object sender, MouseEventArgs e)
        {
            grabbed = false;
        }

        private void spMouseMove(object sender, MouseEventArgs e)
        {
            if (!grabbed) return;
            int x = MousePosition.X;
            int y = MousePosition.Y;

            int newsize;

            switch (((Splitter)sender).Name)
            {

                case "spLeft":
                    newsize = this.Width + (this.Left - x);
                    if (newsize < minWidth) break;
                    this.Left = x;
                    this.Width = newsize;
                    break;
                case "spRight":
                    newsize = x - this.Left;
                    if (newsize < minWidth) break;
                    this.Width = newsize;
                    break;
                case "spTop":
                    newsize = this.Height + (this.Top - y);
                    if (newsize < minHeight) break;
                    this.Top = y;
                    this.Height = newsize;
                    break;
                case "spBottom":
                    newsize = y - this.Top;
                    if (newsize < minHeight) break;
                    this.Height = newsize;
                    break;
                default:
                    break;
           }
        }

#region Form Dragging Event Handler

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

#endregion

#endregion

#region Main Tab Control

        internal void SetTabHeader()
        {
            tabMain.Invalidate();
        }
        private void tabMain_DrawItem(object sender, DrawItemEventArgs e)
        {
            Color TabColor;
            Brush TextBrush;
            Bitmap imgsrc;
            if (e.State == DrawItemState.Selected)
            {
                TextBrush = Brushes.Black;
                TabColor = Color.PaleGoldenrod;
                imgsrc = new Bitmap(Properties.Resources.SmallClose);
            }
            else
            {
                TextBrush = Brushes.AntiqueWhite;
                TabColor = Color.FromArgb(0, 0, 50);
                imgsrc = new Bitmap(Properties.Resources.SmallWhiteClose);
            }

            //e.DrawBackground();
            using (Brush br = new SolidBrush(TabColor))
            {
                e.Graphics.FillRectangle(br, e.Bounds);
                SizeF sz = e.Graphics.MeasureString(tabMain.TabPages[e.Index].Text, e.Font);
                e.Graphics.DrawString(tabMain.TabPages[e.Index].Text, e.Font, TextBrush, e.Bounds.Left, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

                Rectangle rect = e.Bounds;
                e.Graphics.DrawImage(imgsrc, rect.Right - 18, rect.Top + 4);

                Point loc = new Point(Cursor.Position.X, Cursor.Position.Y);
                Rectangle closeButton = new Rectangle(rect.Right - 18, rect.Top + 4, 16, 16);
                if (closeButton.Contains(loc))
                {
                    //Brush hBr = new SolidBrush(Color.PaleGoldenrod);
                    e.Graphics.FillRectangle(br, closeButton);
                }
                rect.Offset(0, 1);
                rect.Inflate(0, -1);
                e.Graphics.DrawRectangle(Pens.DarkGray, rect);
                e.DrawFocusRectangle();
            }
        }

        private void tabMain_MouseDown(object sender, MouseEventArgs e)
        {
            Rectangle r = tabMain.GetTabRect(this.tabMain.SelectedIndex);
            Rectangle closeButton = new Rectangle(r.Right - 18, r.Top + 4, 16, 16);
            if (closeButton.Contains(e.Location))
            {
                tabMain.TabPages.Remove(this.tabMain.SelectedTab);
                if (tabMain.TabPages.Count == 0) tabMain.Visible = false;
            }
        }

        private void tabMain_MouseEnter(object sender, EventArgs e)
        {

        }

        private void tabMain_MouseMove(object sender, MouseEventArgs e)
        {
            Point loc = new Point(Cursor.Position.X, Cursor.Position.Y);
            for (int i = 0; i < this.tabMain.TabPages.Count; i++)
            {
                Rectangle r = tabMain.GetTabRect(i);
                //Getting the position of the "x" mark.
                Rectangle closeButton = new Rectangle(r.Right - 18, r.Top + 4, 16, 16);
                if (closeButton.Contains(loc)) Invalidate();
            }
        }

#endregion

#region ControlBox Handlers
        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pbMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                pbMax.Image = Properties.Resources.Restore;
                toolTip.SetToolTip(pbMax, "Maximize");
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                pbMax.Image = Properties.Resources.Maximize;
                toolTip.SetToolTip(pbMax, "Restore");
            }
            pbMax.Refresh();
        }

        private void pbMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ControlBox_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.White;
        }

        private void ControlBox_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackColor = SystemColors.InactiveCaption;
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }
        private void ControlBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.Wheat;
            ((PictureBox)sender).BorderStyle = BorderStyle.Fixed3D;
        }

        private void ControlBox_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).BackColor = SystemColors.InactiveCaption;
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }


        #endregion

        private void sQLiteHomePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://sqlite.org");
        }
    }
}
