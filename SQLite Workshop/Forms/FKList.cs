﻿using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class FKList : Form
    {
        ToolTip toolTip;
        public FKList()
        {
            InitializeComponent();
        }

        private void FKList_Load(object sender, EventArgs e)
        {
            lblName.Text = string.Format("Database: {0}", MainForm.mInstance.CurrentDB);
            toolTip = new ToolTip();
            HouseKeeping(this, "Foreign Key List");
            dgFKList.AutoGenerateColumns = true;
            LoadForeignKeys();
        }

        private void FKList_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        string[] columnList = new string[] { "source", "table", "from", "to", "on_update", "on_delete", "match" };
        protected void LoadForeignKeys()
        {
            SchemaDefinition sd = DataAccess.SchemaDefinitions[MainForm.mInstance.CurrentDB];
            SQLiteErrorCode returnCode = SQLiteErrorCode.Ok;
            DataTable dt;
            DataTable dtGrid = new DataTable();
            for (int i = 0; i < columnList.Length; i++)
            {
                DataColumn dc = new DataColumn();
                dc.DataType = Type.GetType("System.String");
                dc.ColumnName = columnList[i];
                dtGrid.Columns.Add(dc);
            }

            try
            {
                string sql;
                foreach (var table in sd.Tables)
                {
                    sql = string.Format("Pragma foreign_key_list(\"{0}\")", table.Key);
                    dt = DataAccess.ExecuteDataTable(MainForm.mInstance.CurrentDB, sql, out returnCode);
                    foreach (DataRow dr in dt.Rows)
                    {
                        dtGrid.Rows.Add(new string[] { table.Key, dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString(), dr[7].ToString() });
                    }
                }
                if (dtGrid.Rows.Count == 0)
                {
                    this.Close();
                    ShowMsg("No Foreign Keys were found.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                dgFKList.Columns.Clear();
                dgFKList.DataSource = dtGrid;
                dgFKList.Refresh();
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Unable to load Foreign Key List.\r\n {0}", ex.Message));
                return;
            }
        }
    }
}
