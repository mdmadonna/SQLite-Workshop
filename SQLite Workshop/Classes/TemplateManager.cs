using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    class TemplateManager
    {
        delegate void BuildTreeCallback();

        TreeView treeTemplates;
        string templatesDirectory = string.Empty;
        FileSystemWatcher watcher;

        public TemplateManager(TreeView t)
        {
            treeTemplates = t;
#if DEBUG
            templatesDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates");
#else
            templatesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SQLite Workshop\\Templates");
#endif
            BuildTree();
            StartTemplateWatch();
        }

        internal void BuildTree()
        {
            if (treeTemplates.InvokeRequired)
            {
                BuildTreeCallback d = new BuildTreeCallback(BuildTree);
                treeTemplates.Invoke(d);
                return;
            }


            ContextMenu tmpContextMenu = new ContextMenu();
            tmpContextMenu.MenuItems.Add(new MenuItem(MenuMerge.Add, 0, Shortcut.None, "Delete", tmpContextMenu_Clicked, tmpContextMenu_Popup, tmpContextMenu_Selected, null));
            treeTemplates.Nodes.Clear();

            TreeNode topNode = new TreeNode("Sql Templates", 0, 0);
            //topNode.ContextMenu = tmpContextMenu;

            DirectoryInfo di = new DirectoryInfo(templatesDirectory);
            if (!di.Exists) return;

            string[] dirs = Directory.GetDirectories(templatesDirectory);
            foreach (string dir in dirs)
            {
                TreeNode dirNode = new TreeNode(new DirectoryInfo(dir).Name, 2, 2);
                string[] files = Directory.GetFiles(dir, "*.sql");
                foreach (string file in files)
                {
                    TreeNode sqlNode = new TreeNode(Path.GetFileNameWithoutExtension(file), 3, 3);
                    sqlNode.Tag = file;
                    sqlNode.ContextMenu = tmpContextMenu;
                    dirNode.Nodes.Add(sqlNode);
                }
                topNode.Nodes.Add(dirNode);
            }
            treeTemplates.Nodes.Add(topNode);
            topNode.Expand();
        }

        private void tmpContextMenu_Popup(object sender, EventArgs e)
        {

        }

        private void tmpContextMenu_Selected(object sender, EventArgs e)
        {
        }

        private void tmpContextMenu_Clicked(object sender, EventArgs e)
        {
            switch (((MenuItem)sender).Text.ToLower())
            {
                case "delete":
                    DeleteTemplate();
                    break;
                default:
                    break;
            }
        }

        private void DeleteTemplate()
        {
            string fileName = treeTemplates.SelectedNode.Tag.ToString();
            FileInfo fi = new FileInfo(Path.Combine(templatesDirectory, fileName));
            try
            {
                if (fi.Exists)
                {
                    fi.Delete();
                    MainForm.mInstance.WriteStatusStripMessage(string.Format("{0} Deleted.", Path.GetFileName(fileName)));
                }
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Cannot delete {0}.{1}{2}.", fileName, Environment.NewLine, ex.Message));
            }
        }

        internal void StartTemplateWatch()
        {
            watcher = new FileSystemWatcher();
            watcher.Path = templatesDirectory;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            watcher.IncludeSubdirectories = true;
            watcher.Created += new FileSystemEventHandler(OnTemplateDirChanged);
            watcher.Changed += new FileSystemEventHandler(OnTemplateDirChanged);
            watcher.Deleted += new FileSystemEventHandler(OnTemplateDirChanged);
            watcher.Renamed += new RenamedEventHandler(OnTemplateRenamed);
            watcher.EnableRaisingEvents = true;
        }

        internal void OnTemplateDirChanged(object source, FileSystemEventArgs e)
        {
            BuildTree();
        }

        internal void OnTemplateRenamed(object source, RenamedEventArgs e)
        {
            BuildTree();
        }

        internal void SaveTemplate(SqlTabControl tc)
        {

            Control c = tc.Parent;
            string filename = null;

            if (string.IsNullOrEmpty(tc.SqlFileName))
            { filename = ((TabPage)c).Text.Trim(); }
            else
            { filename = tc.SqlFileName; }

            do
            {
                filename = GetFile(filename);
                if (string.IsNullOrEmpty(filename)) return;
                if (ValidateTemplateDirectory(filename)) break;
                Common.ShowMsg(string.Format("Templates must be placed in a sub-folder{0}of the Templates Folder located at{0}{1}", Environment.NewLine, templatesDirectory));
            } while (true);

            FileInfo fi = new FileInfo(filename);
            if (fi.Exists)
            {
                DialogResult dr = Common.ShowMsg(string.Format("{0} already exists.\r\nDo you want to replace it?", filename), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dr != DialogResult.Yes) return;
            }

            try
            {
                StreamWriter sw = new StreamWriter(filename);
                sw.Write(tc.SqlStatement);
                sw.Close();
                MainForm.mInstance.WriteStatusStripMessage(string.Format("{0} Saved.", Path.GetFileName(filename)));
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("An error occurred while writing {0}.\r\n{1}", filename, ex.Message));
            }
            tc.SqlFileName = filename;
            ((TabPage)c).Text = string.Format("  {0}          ", Path.GetFileName(filename)); ;
            return;
        }

        protected string GetFile(string filename)
        {
            string path = templatesDirectory;

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Select Destination File";
            saveFile.Filter = "All files (*.*)|*.*|Sql Files (*.sql)|*.sql";
            saveFile.FilterIndex = 2;
            saveFile.AddExtension = true;
            saveFile.AutoUpgradeEnabled = true;
            saveFile.DefaultExt = "sql";
            saveFile.InitialDirectory = path;
            saveFile.RestoreDirectory = true;
            saveFile.ValidateNames = true;
            saveFile.OverwritePrompt = false;
            saveFile.FileName = filename;

            if (saveFile.ShowDialog() != DialogResult.OK) return string.Empty;
            return saveFile.FileName;
        }

        protected bool ValidateTemplateDirectory(string filename)
        {
            if (!filename.StartsWith(templatesDirectory)) return false;
            string path = Path.GetDirectoryName(filename).Replace(templatesDirectory, string.Empty);
            if (string.IsNullOrEmpty(path) || !path.StartsWith(@"\")) return false;
            return true;
        }
    }
}
