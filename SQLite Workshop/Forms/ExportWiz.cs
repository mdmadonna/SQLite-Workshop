using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;
using CsvHelper;

namespace SQLiteWorkshop
{
    public partial class ExportWiz : Form
    {
        ToolTip toolTip;

        public string DatabaseLocation { get; set; }
        public string TableName { get; set; }
        private bool cancelExport = false;

        public ExportWiz()
        {
            InitializeComponent();
        }

        private void ExportWiz_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            HouseKeeping(this, string.Format("Export Wizard: {0}", DatabaseLocation));
            toolStripStatusLabel1.Text = string.Empty;
            InitializeForm();
        }
        private void ExportWiz_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }
        private void InitializeForm()
        {
            listBoxTables.Items.Clear();
            foreach (var table in DataAccess.SchemaDefinitions[DatabaseLocation].Tables)
            {
                if (!IsSystemTable(table.Key)) listBoxTables.Items.Add(table.Key);
            }
            foreach (var view in DataAccess.SchemaDefinitions[DatabaseLocation].Views)
            {
                listBoxTables.Items.Add(view.Key);
            }
            if (!string.IsNullOrEmpty(TableName))
            {
                listBoxTables.SelectedItem = TableName;
            }
        }

        private void btnFindFile_Click(object sender, EventArgs e)
        {
            GetFile();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            cancelExport = false;
            btnExport.Enabled = false;
            btnClose.Text = "Cancel";
            btnClose.Cursor = Cursors.Default;
            DoExport();
            btnExport.Enabled = true;
            btnClose.Text = "Close";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (btnClose.Text == "Cancel")
            {
                DialogResult rc = ShowMsg(WARN_EXPCANCEL, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (rc == DialogResult.Yes) cancelExport = true;
                return;
            }
            this.Close();
        }

        private void radioSQL_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxHeadings.Text = "Include Column Names in INSERT Statement";
            listBoxTables.SelectionMode = SelectionMode.MultiExtended;
            if (!string.IsNullOrWhiteSpace(txtFileDestination.Text)) txtFileDestination.Text = Path.ChangeExtension(txtFileDestination.Text, "sql");
        }

        private void radioComma_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxHeadings.Text = "Include Headings in First Line";
            listBoxTables.SelectionMode = SelectionMode.One;
            if (listBoxTables.SelectedIndex >= 0)
            { txtFileDestination.Text = string.Format("{0}.csv", listBoxTables.SelectedItem); }
            else
            { txtFileDestination.Text = string.Empty; }
        }

        private void listBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedIndex < 0) return;
            string path = string.IsNullOrEmpty(txtFileDestination.Text) ? string.Empty : Path.GetDirectoryName(txtFileDestination.Text);
            if (radioComma.Checked)
            {
                txtFileDestination.Text = string.IsNullOrEmpty(path) ? string.Format("{0}.csv", listBoxTables.Items[listBoxTables.SelectedIndex].ToString()) : string.Format("{0}\\{1}.csv", path, listBoxTables.Items[listBoxTables.SelectedIndex].ToString());
                return;
            }
            txtFileDestination.Text = string.IsNullOrEmpty(path) ? "SQLWorkshop.sql" : string.Format("{0}\\SQLWorkshop.sql", path);
        }

        protected void GetFile()
        {
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "Select Destination File",
                Filter = radioComma.Checked ? "All files (*.*)|*.*|Comma-delimited FIles (*.csv)|*.csv" : "All files (*.*)|*.*|Sql FIles (*.sql)|*.sql",
                FilterIndex = 2,
                AddExtension = true,
                AutoUpgradeEnabled = true,
                DefaultExt = radioComma.Checked ? "csv" : "sql",
                RestoreDirectory = true,
                ValidateNames = true,
                OverwritePrompt = false
            };
            if (radioComma.Checked)
            {
                if (listBoxTables.SelectedIndex >= 0)
                {
                    txtFileDestination.Text = string.Format("{0}.csv", listBoxTables.SelectedItem.ToString());
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtFileDestination.Text)) txtFileDestination.Text = "SQLWorkshop.sql";
            }
            saveFile.FileName = txtFileDestination.Text;

            if (saveFile.ShowDialog() != DialogResult.OK) return;
            txtFileDestination.Text = saveFile.FileName;
            return;
        }

        private bool ValidateInput()
        {
            if (listBoxTables.SelectedIndex < 0)
            {
                ShowMsg("Please select a table to export.");
                listBoxTables.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtFileDestination.Text))
            {
                ShowMsg("Please enter a destination file name.");
                txtFileDestination.Focus();
                return false;
            }

            FileInfo fi = new FileInfo(txtFileDestination.Text);
            if (fi.Exists)
            {
                DialogResult dr = ShowMsg(string.Format("{0} already exists.\r\nDo you want to replace it?", txtFileDestination.Text), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dr != DialogResult.Yes) { txtFileDestination.Focus(); return false; }
            }

            return true;
        }

        private void DoExport()
        {
            if (radioSQL.Checked)
            {
                ExportSQL();
                return;
            }

            StreamWriter sw;
            CsvWriter csv;

            try
            {
                sw = new StreamWriter(txtFileDestination.Text);
                csv = new CsvWriter(sw);
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Unable to open {0}\r\n{1}", txtFileDestination.Text, ex.Message));
                toolStripStatusLabel1.Text = EXP_FAILED;
                return;
            }

            SQLiteConnection conn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteDataReader dr = null;
            long RecordCount;

            var rtn = DataAccess.OpenDB(DatabaseLocation, ref conn, ref cmd, false);
            if (!rtn)
            {
                ShowMsg("Unable to open database.");
                toolStripStatusLabel1.Text = EXP_FAILED;
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            toolStripStatusLabel1.Text = "Working...";
            try
            {
                cmd.CommandText = string.Format("Select Count(*) From {0}", listBoxTables.SelectedItem.ToString());
                RecordCount = Convert.ToInt64(cmd.ExecuteScalar());
                cmd.CommandText = string.Format("Select * From {0}", listBoxTables.SelectedItem.ToString());
                dr = cmd.ExecuteReader();
                if (checkBoxHeadings.Checked)
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        csv.WriteField(dr.GetName(i));
                    }
                    csv.NextRecord();
                }
                long recs = 0;
                while (dr.Read())
                {
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        csv.WriteField(DBNull.Value.Equals(dr[i]) ? string.Empty : dr[i].GetType().Equals(typeof(byte[])) ? FormatBytes((byte[])dr[i]) : dr[i].ToString());
                    }
                    csv.NextRecord();
                    recs++;
                    if (recs % 100 == 0) { toolStripStatusLabel1.Text = string.Format("{0} of {1} records written", recs, RecordCount); Application.DoEvents(); }
                    if (cancelExport) break;
                }
                toolStripStatusLabel1.Text = cancelExport ? EXP_CANCEL : EXP_COMPLETE;
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Export Failed.\r\n{0}", ex.Message));
                toolStripStatusLabel1.Text = EXP_FAILED;
                return;
            }
            finally
            {
                if (sw != null) sw.Close();
                cmd.Cancel();
                dr.Close();
                DataAccess.CloseDB(conn);
                this.Cursor = Cursors.Default;
            }
            return;
        }

        private void ExportSQL()
        {
            StreamWriter sw;
            try
            {
                sw = new StreamWriter(txtFileDestination.Text);
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Unable to open {0}\r\n{1}", txtFileDestination.Text, ex.Message));
                toolStripStatusLabel1.Text = EXP_FAILED; 
                return;
            }

            SQLiteConnection conn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteDataReader dr = null;
            long RecordCount;

            var rtn = DataAccess.OpenDB(DatabaseLocation, ref conn, ref cmd, false);
            if (!rtn)
            {
                ShowMsg("Unable to open database.");
                toolStripStatusLabel1.Text = EXP_FAILED;
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            StringBuilder sb = new StringBuilder();
            StringBuilder sbVal = new StringBuilder();
            try
            {
                RecordCount = 0;
                sw.WriteLine("pragma foreign_keys=off;");
                sw.WriteLine("begin;");
                foreach (string table in listBoxTables.SelectedItems)
                {
                    cmd.CommandText = string.Format("Select Count(*) From {0}", table);
                    RecordCount += Convert.ToInt64(cmd.ExecuteScalar());
                }
                long recs = 0;
                foreach (string table in listBoxTables.SelectedItems)
                {
                    if (cancelExport) break;
                    toolStripStatusLabel1.Text = string.Format("Exporting {0}", table);
                    string CreateSQL = DataAccess.SchemaDefinitions[DatabaseLocation].Tables[table].CreateSQL.Trim();
                    if (!CreateSQL.EndsWith(";")) CreateSQL += ";";
                    sw.WriteLine(CreateSQL);

                    cmd.CommandText = string.Format("Select * From \"{0}\"", table);
                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        sb.Clear();
                        sbVal.Clear();
                        sb.AppendFormat("Insert Into \"{0}\" ", table);
                        if (checkBoxHeadings.Checked) sb.Append("(");
                        sbVal.Append("Values (");
                        bool bComma = false;
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            if (checkBoxHeadings.Checked) sb.Append(bComma ? "," : string.Empty).AppendFormat("\"{0}\"", dr.GetName(i));
                            sbVal.Append(bComma ? "," : string.Empty);
                            sbVal.Append(DBNull.Value.Equals(dr[i]) ? "null" : FormatOutput(dr[i].GetType(), dr[i]));
                            bComma = true;
                        }
                        if (checkBoxHeadings.Checked) sb.Append(")");
                        sbVal.Append(");");
                        sw.WriteLine(string.Format("{0} {1}", sb.ToString(), sbVal.ToString()));
                        recs++;
                        if (recs % 100 == 0) { toolStripStatusLabel1.Text = string.Format("Exporting {0}: {1} of {2} records written", table, recs, RecordCount); Application.DoEvents(); }
                        if (cancelExport) break;
                    }
                    dr.Close();
                    if (!cancelExport)
                    {
                        string IndexSql;
                        foreach (var index in DataAccess.SchemaDefinitions[DatabaseLocation].Tables[table].Indexes)
                        {
                            IndexSql = index.Value.CreateSQL;
                            if (!IndexSql.EndsWith(";")) IndexSql += ";";
                            sw.WriteLine(IndexSql);
                        }
                    }
                }
                sw.WriteLine("commit;");
                toolStripStatusLabel1.Text = cancelExport ? EXP_CANCEL : EXP_COMPLETE;
            }
            catch (Exception ex)
            {
                ShowMsg(string.Format("Export Failed.\r\n{0}", ex.Message));
                toolStripStatusLabel1.Text = EXP_FAILED;
                return;
            }
            finally
            {
                cmd.Cancel();
                try { dr.Close(); } catch { }
                DataAccess.CloseDB(conn);
                this.Cursor = Cursors.Default;
                try { sw.Close(); } catch { }
            }
            return;
        }

        private string FormatOutput(Type type, object item)
        {
            switch (type.ToString().ToLower())
            {
                case "system.byte[]":
                    return FormatBytes((byte[])item);
                case "system.string":
                    return string.Format("\"{0}\"", item.ToString().Replace("\"", "\"\""));
                case "system.boolean":
                    bool.TryParse(item.ToString(), out bool boolValue);
                    return boolValue ? "1" : "0";
                case "system.datetime":
                    DateTime.TryParse(item.ToString(), out DateTime dateValue);
                    return string.Format("\"{0}\"", dateValue.ToString("o"));
                default:
                    return item.ToString();

            }
        }

        private string FormatBytes(byte[] data)
        {
            if (data == null) return string.Empty;
            if (data.Length == 0) return string.Empty;

            string binstr = string.Join("", data.Select(b => Convert.ToString(b, 16).PadLeft(2,'0')));
            StringBuilder sb = new StringBuilder();
            return sb.AppendFormat("x'{0}'",binstr).ToString();
        }
    }
}
