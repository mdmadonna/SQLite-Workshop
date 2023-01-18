/*********************************************************************************************
 * 
 * Template used to create new Windows Forms
 * 
 ********************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class Mapping : Form
    {
        ToolTip toolTip;

        private readonly string tableName;
        private readonly string sqliteTableName;
        bool bPopulatingDest;
        Dictionary<string, DBColumn> cols;

        internal DBManager db { get; set; }
        

        public Mapping(string TableName, string SQLiteTableName)
        {
            tableName = TableName;
            sqliteTableName = SQLiteTableName;
            InitializeComponent();
        }

        private void Mapping_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            HouseKeeping(this, "Column Mapping");
            InitPage();            
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Mapping_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void InitPage()
        {
            lblSource.Text = tableName;
            lblTarget.Text = sqliteTableName;
            
            SetupDataGrid();

            SchemaDefinition sd = DataAccess.SchemaDefinitions[db.TargetDB];
            if (sd.Tables.ContainsKey(sqliteTableName))
            {
                radioAppendRows.Enabled = true;
                radioDeleteRows.Enabled = true;
                radioCreateDest.Enabled = false;
                radioAppendRows.Checked = true;
            }
            else
            {
                radioAppendRows.Enabled = false;
                radioDeleteRows.Enabled = false;
                radioCreateDest.Enabled = true;
                radioCreateDest.Checked = true;
            }

        }

        private void SetupDataGrid()
        {
            dataGridViewCols.Rows.Clear();

            cols = db.GetColumns(tableName);

            foreach (KeyValuePair<string, DBColumn> col in cols)
            {
                DBColumn dbc = col.Value;
                DataGridViewRow dr = new DataGridViewRow();
                DataGridViewTextBoxCell dgSource = new DataGridViewTextBoxCell() { Value = col.Key };
                DataGridViewComboBoxCell dgDest = new DataGridViewComboBoxCell
                {
                    FlatStyle = FlatStyle.Standard
                };
                DataGridViewTextBoxCell dgType = new DataGridViewTextBoxCell() { Value = col.Value.SqlType };
                DataGridViewTextBoxCell dgSize = new DataGridViewTextBoxCell() { Value = col.Value.Size };
                DataGridViewCheckBoxCell dgNull = new DataGridViewCheckBoxCell() { Value = col.Value.IsNullable };
                string pValue = col.Value.SqlType.ToLower() == "decimal" ? dbc.NumericPrecision.ToString() : string.Empty;
                DataGridViewTextBoxCell dgPrecision = new DataGridViewTextBoxCell() { Value = pValue };
                string sValue = col.Value.SqlType.ToLower() == "decimal" ? dbc.NumericScale.ToString() : string.Empty;
                DataGridViewTextBoxCell dgScale = new DataGridViewTextBoxCell() { Value = sValue };
                dr.Cells.AddRange(new DataGridViewCell[] { dgSource, dgDest, dgType, dgSize, dgNull, dgPrecision, dgScale });
                dataGridViewCols.Rows.Add(dr);
            }
            PopulateDestColumn();
            dataGridViewCols.Refresh();
        }
        void PopulateDestColumn()
        {
            bPopulatingDest = true;
            foreach (DataGridViewRow dr in dataGridViewCols.Rows)
            {
                DataGridViewComboBoxCell cmb = (DataGridViewComboBoxCell)dr.Cells[1];
                string key = dr.Cells[0].Value.ToString();
                cmb.Items.Clear();
                cmb.Items.Add("<ignore>");
                cmb.Items.Add(key);
                cmb.Value = key;
            }
            bPopulatingDest = false;
        }

        void UpdateDestColumn(int ChangedRow)
        {
            if (bPopulatingDest) return;
            StringBuilder sb = new StringBuilder();
            foreach (DataGridViewRow dr in dataGridViewCols.Rows)
            {
                if (dr.Cells[1].Value.ToString() != "<ignore>") sb.Append(dr.Cells[1].Value).Append("|");
            }
            string FieldList = sb.ToString();
            sb.Clear();

            for (int i = 0; i < dataGridViewCols.Rows.Count; i++)
            {
                DataGridViewRow dr = dataGridViewCols.Rows[i];
                if (i != ChangedRow)
                {
                    DataGridViewComboBoxCell cmb = (DataGridViewComboBoxCell)dr.Cells[1];
                    string key = dr.Cells[0].Value.ToString();
                    cmb.Items.Clear();
                    if (!cmb.Items.Contains("<ignore>")) cmb.Items.Add("<ignore>");
                    if (!cmb.Items.Contains(key) && cmb.Value.ToString() != "<ignore>") cmb.Items.Add(key);
                    foreach (KeyValuePair<string, DBColumn> dbc in cols)
                    {
                        if (!FieldList.Contains(dbc.Key + "|")) if (!cmb.Items.Contains(dbc.Key)) cmb.Items.Add(dbc.Key);
                    }
                }
            }
        }

        private void dataGridViewCols_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Looking only for Change in Dest field
            if (e.ColumnIndex == 1 && e.RowIndex >= 0) UpdateDestColumn(e.RowIndex);
        }
        
    }
}
