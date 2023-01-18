using System;

namespace SQLiteWorkshop
{
    abstract class TabManager
    {
        protected string DatabaseLocation { get; set; }
        protected MainForm m;

        internal TabManager()
        {
            m = MainForm.mInstance;
        }
        protected virtual void OnEnter(object sender, EventArgs e)
        {
            // Reserved for future use
            //m.SQLButton(this is DBEditorTab);
            if (DatabaseLocation == m.CurrentDB) return;
            m.CurrentDB = DatabaseLocation;
            //m.RefreshPropertyGrid();
        }

    }
}
