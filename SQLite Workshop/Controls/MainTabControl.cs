using System.Windows.Forms;

namespace SQLiteWorkshop
{
    partial class MainTabControl : UserControl
    {
        internal MainForm m;
        internal SchemaDefinition sd;
        internal string DatabaseName { get; set; }

        internal virtual string SqlStatement { get; set; }

        internal ConnectionProperties ConnProps { get; set; }

        internal MainTabControl()
        {
            m = MainForm.mInstance;
        }

        internal void InitializeClass(string DBName)
        {

            DatabaseName = DBName;
            sd = DataAccess.GetSchema(DBName);

            ConnProps = new ConnectionProperties();
            ConnProps.connSettings.DbFileName = DBName;
            ConnProps.connSettings.DbName = sd.DBName;
        }

    }
}
