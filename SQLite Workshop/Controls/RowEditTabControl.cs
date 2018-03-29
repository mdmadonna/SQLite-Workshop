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
using System.Collections;

namespace SQLiteWorkshop
{
    public partial class RecordEditTabControl : UserControl
    {
        string RowIDColName;
        int RowIdIndex;
        string PrimaryKeyName;
        int PKIndex;
        bool tableHasRowID;

        string BaseSQL = string.Empty;
        int CurrentRow = 0;
        int RecordCount = 0;

        BindingSource bs;
        DataTable dt;

        string[] Rowids = new string[] { "rowid", "_rowid_", "OID" };

        public string DatabaseName { get; set; }
        public string TableName { get; set; }

        internal RecordEditTabControl(string dbName, string tblname)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;
            toolStripLabel1.Text = string.Empty;
            //toolStripStatusLabelTableName.Text = tblname;
            DatabaseName = dbName;
            TableName = tblname;
            InitializePage();
            LoadRecord(0);
        }

        protected void InitializePage()
        {
            lblTable.Text = TableName;
            SQLiteErrorCode returnCode;

            tableHasRowID = DataAccess.CheckForRowID(DatabaseName, TableName, out RowIDColName, out PrimaryKeyName);

            BaseSQL = string.Format("Select Count(*) From \"{0}\"", TableName);
            RecordCount = Convert.ToInt32(DataAccess.ExecuteScalar(DatabaseName, BaseSQL, out returnCode));
            BaseSQL = tableHasRowID ? string.Format("Select {0}, * From \"{1}\"", RowIDColName, TableName) : BaseSQL = string.Format("Select * From \"{0}\"", TableName);
            RowIdIndex = -1;
            
            bs = new BindingSource();
            BindingList bl = new BindingList(RecordCount);
            bs.DataSource = bl.GetList();
            bindingNavigator1.BindingSource = bs;

            SQLiteConnection conn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            DataAccess.OpenDB(DatabaseName, ref conn, ref cmd, out returnCode, false);
            cmd.CommandText = BaseSQL;
            SQLiteDataReader dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly);

            int start = 50;
            int lablen = 100;
            int height = 40;

            for (int i = 0; i < dr.FieldCount; i++)
            {
                string colname = dr.GetName(i);
                if (RowIdIndex < 0)  if (tableHasRowID) if (RowIDColName == colname) RowIdIndex = i;
                if (i == RowIdIndex) continue;
                if (colname == PrimaryKeyName) PKIndex = i;

                Label lbl = new Label();
                lbl.Text = string.Format("{0}:", colname);
                lbl.Top = start;
                lbl.Left = 50;
                panelBody.Controls.Add(lbl);

                TextBox txt = new TextBox();
                txt.Name = string.Format("txt{0}", i.ToString().PadLeft(4, '0'));
                txt.Tag = colname;
                txt.Top = start;
                txt.Left = lbl.Left + lablen;
                txt.Width = 300;
                panelBody.Controls.Add(txt);                
                start += height;
            }
            dr.Close();
            DataAccess.CloseDB(conn);
        }

        protected void LoadRecord(int RecordNum)
        {
            if (RecordNum >= RecordCount) RecordNum = RecordCount - 1;
            if (RecordNum < 0) RecordNum = 0;
            //toolStripLabel1.Text = string.Empty;

            string sql = string.Format("{0} Limit 1 Offset {1}", BaseSQL, RecordNum);
            dt = DataAccess.ExecuteDataTable(DatabaseName, sql, out SQLiteErrorCode returnCode);

            if (dt.Rows.Count != 1) return;
            DataRow dr = dt.Rows[0];
            for (int i = 0; i < dr.ItemArray.Count(); i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                t.Text = dr.ItemArray[i].ToString();
            }
            CurrentRow = RecordNum;
            bs.Position = CurrentRow;
        }

        protected bool UpdateRecord()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Update \"{0}\" Set", TableName);
            int count = 0;
            int i = 0;
            ArrayList parms = new ArrayList();
            toolStripLabel1.Text = string.Empty;
            SQLiteErrorCode returnCode;

            DataRow dr = dt.Rows[0];

            for (i = 0; i < dr.ItemArray.Count(); i++)
            {
                if (i == RowIdIndex) continue;
                TextBox t = FindTextBox(string.Format("txt{0}", i.ToString().PadLeft(4, '0')));
                if (dr[i].ToString() != t.Text)
                {
                    count++;
                    sb.Append(count > 1 ? "," : string.Empty).AppendFormat(" \"{0}\" = ?", t.Tag);
                    parms.Add(t.Text);
                }
            }
            if (count == 0) return true;
            if (tableHasRowID)
            {
                sb.AppendFormat(" Where {0} = {1}", RowIDColName, dr.ItemArray[RowIdIndex].ToString());
            }
            else
            if (!string.IsNullOrEmpty(PrimaryKeyName))
            {
                sb.AppendFormat(" Where {0} = {1}", PrimaryKeyName, dr.ItemArray[PKIndex].ToString());
            }
            else
            {
                StringBuilder sbWhere = new StringBuilder();
                sbWhere.Append("Where ");
                for (i = 0; i < dr.Table.Columns.Count - 1; i++)
                {
                    sbWhere.Append("\"").Append(dr.Table.Columns[i].ColumnName).AppendFormat("\" = \"{0}\"", dr[i].ToString());
                    sbWhere.Append(" And ");
                }
                sbWhere.Append("\"").Append(dr.Table.Columns[dr.Table.Columns.Count - 1].ColumnName).AppendFormat("\" = \"{0}\"", dr[i].ToString());
                var ucount = DataAccess.ExecuteScalar(DatabaseName, string.Format("Select Count(*) From \"{0}\" {1}", TableName, sbWhere.ToString()), out returnCode);
                if (Convert.ToInt32(ucount) != 1)
                {
                    Common.ShowMsg(Common.ERR_MULTIUPDATE);
                    return false;
                }
                sb.AppendFormat(" {0}", sbWhere.ToString());
            }


            if (RecordUpdated())
            {
                DialogResult dgResult = Common.ShowMsg("The Row has changed since it was retrieved.  Click 'Yes' to save you changes anyway or click 'No' to discard your change and retrieve current data for this row.", MessageBoxButtons.YesNo);
                if (dgResult == DialogResult.No)
                {
                    LoadRecord(CurrentRow);
                    return false;
                }
            }
            int recsupdated = DataAccess.ExecuteNonQuery(DatabaseName, sb.ToString(), parms, out returnCode);
            toolStripLabel1.Text = string.Format("{0} Record(s) updated.", recsupdated.ToString());
            return true;
        }

        protected bool DeleteRecord()
        {
            toolStripLabel1.Text = string.Empty;
            DialogResult dgResult;
            if (RecordUpdated())
            {
                dgResult = Common.ShowMsg("The Row has changed since it was retrieved.  Click 'Yes' to delete this row or click 'No' to abort.", MessageBoxButtons.YesNo);
                if (dgResult == DialogResult.No)
                {
                    LoadRecord(CurrentRow);
                    return false;
                }
            }
            else
            {
                dgResult = Common.ShowMsg("Confirm Delete. Press 'Ok' to Delete or 'Cancel' to abort.", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dgResult == DialogResult.Cancel) return false;
            }
            DataRow dr = dt.Rows[0];
            string sql = string.Format("Delete from \"{0}\" where {1} = {2}", TableName, RowIDColName, dr.ItemArray[RowIdIndex].ToString());
            int recsupdated = DataAccess.ExecuteNonQuery(DatabaseName, sql, out SQLiteErrorCode returnCode);
            toolStripLabel1.Text = "1 Record deleted.";
            return true;
        }
        protected bool RecordUpdated()
        {
            string sql = string.Format("{0} Limit 1 Offset {1}", BaseSQL, CurrentRow);
            DataTable currdt = DataAccess.ExecuteDataTable(DatabaseName, sql, out SQLiteErrorCode returnCode);

            if (dt.Rows.Count != 1) return true;
            DataRow currdr = currdt.Rows[0];
            DataRow dr = dt.Rows[0];

            for (int i = 0; i < dr.ItemArray.Count(); i++)
            {
                if (!dr.ItemArray[i].Equals(currdr.ItemArray[i])) return true;
            }
            return false;
        }

        protected TextBox FindTextBox(string tbName)
        {
            return (TextBox)panelBody.Controls[tbName];            
        }
        internal void Execute()
        { }

        private void bindingNavigatorMoveNextItem_Click(object sender, EventArgs e)
        {
            if (!UpdateRecord()) return;
            if (CurrentRow < RecordCount) CurrentRow++;
            LoadRecord(CurrentRow);
        }

        private void bindingNavigatorMovePreviousItem_Click(object sender, EventArgs e)
        {
            if (!UpdateRecord()) return;
            if (CurrentRow > 0) CurrentRow--;
            LoadRecord(CurrentRow);
        }

        private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
        {
            if (!UpdateRecord()) return;
            LoadRecord(RecordCount-1);
        }

        private void bindingNavigatorMoveFirstItem_Click(object sender, EventArgs e)
        {
            if (!UpdateRecord()) return;
            LoadRecord(0);
        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {
            if (DeleteRecord()) LoadRecord(CurrentRow);
        }
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

    }

}
