using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    public partial class RecordEditTabControl : UserControl
    {
        string RowIDColName;
        string PrimaryKeyName;
        bool tableHasRowID;

        string[] Rowids = new string[] { "rowid", "_rowid_", "OID" };

        public string DatabaseName { get; set; }
        public string TableName { get; set; }

        internal RecordEditTabControl(string dbName, string tblname)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            //toolStripStatusLabelMsg.Text = string.Empty;
            //toolStripStatusLabelTableName.Text = tblname;
            dgRecord.AutoGenerateColumns = false;
            DatabaseName = dbName;
            TableName = tblname;
            InitializeFirstRecord();
        }

        protected void InitializeFirstRecord()
        {
            SQLiteErrorCode returnCode;

            string sql = string.Format("Select Count(*) From \"{0}\"", TableName);
            int recordcount = Convert.ToInt32(DataAccess.ExecuteScalar(DatabaseName, sql, out returnCode));
            BindingSource bs = new BindingSource();
            BindingList bl = new BindingList(recordcount);
            bs.DataSource = bl.GetList();
            bindingNavigator1.BindingSource = bs;


            Dictionary<string, ColumnLayout> columns = DataAccess.SchemaDefinitions[DatabaseName].Tables[TableName].Columns;
            foreach (var col in columns)
            {
                
                dgRecord.Rows.Add(new object[] { col.Key, col.Key });
                dgRecord.Rows[dgRecord.RowCount - 1].Cells[1].ReadOnly = true;
            }
            dgRecord.Refresh();
        }
        internal void Execute()
        { }

    }

    class BindingList : System.ComponentModel.IListSource
    {
        private int totalrecords;

        public BindingList(int recordcount)
        {
            totalrecords = recordcount;
        }

        public bool ContainsListCollection { get; protected set; }

        public System.Collections.IList GetList()
        {
            List<int> records = new List<int>();
            records.AddRange(Enumerable.Range(0, totalrecords).Select(pageidx => new int()));
            return records;
        }

        public string TableName { get; protected set; }
    }

}
