using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CsvHelper;

namespace SQLiteWorkshop
{
    public partial class ExportWiz : Form
    {
        ToolTip toolTip;

        public string DatabaseLocation { get; set; }
        public string TableName { get; set; }

        public ExportWiz()
        {
            InitializeComponent();
        }

        private void ExportWiz_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = "Export Wizard";

            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbClose, "Close");
            toolStripStatusLabel1.Text = string.Empty;
            InitializeForm();
        }

        private void InitializeForm()
        {
            listBoxTables.Items.Clear();
            foreach (var table in DataAccess.SchemaDefinitions[DatabaseLocation].Tables)
            {
                if (!Common.IsSystemTable(table.Key)) listBoxTables.Items.Add(table.Key);
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
            if (ValidateInput()) DoExport();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTables.SelectedIndex < 0) return;
            string path = string.IsNullOrEmpty(txtFileDestination.Text) ? string.Empty : Path.GetDirectoryName(txtFileDestination.Text);
            txtFileDestination.Text = string.IsNullOrEmpty(path) ? string.Format("{0}.csv", listBoxTables.Items[listBoxTables.SelectedIndex].ToString()) : string.Format("{0}\\{1}.csv", path, listBoxTables.Items[listBoxTables.SelectedIndex].ToString());
        }

        protected void GetFile()
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Select Destination File";
            saveFile.Filter = "All files (*.*)|*.*|Comma-delimited FIles (*.csv)|*.csv";
            saveFile.FilterIndex = 2;
            saveFile.AddExtension = true;
            saveFile.AutoUpgradeEnabled = true;
            saveFile.DefaultExt = "csv";
            saveFile.RestoreDirectory = true;
            saveFile.ValidateNames = true;
            saveFile.OverwritePrompt = false;
            if (listBoxTables.SelectedIndex >= 0)
            {
                txtFileDestination.Text = string.Format("{0}.csv", listBoxTables.SelectedItem.ToString());
                saveFile.FileName = txtFileDestination.Text;
            }

            if (saveFile.ShowDialog() != DialogResult.OK) return;
            txtFileDestination.Text = saveFile.FileName;
            return;
        }

        private bool ValidateInput()
        {
            if (listBoxTables.SelectedIndex < 0)
            {
                Common.ShowMsg("Please select a table to export.");
                listBoxTables.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(txtFileDestination.Text))
            {
                Common.ShowMsg("Please enter a destination file name.");
                txtFileDestination.Focus();
                return false;
            }

            FileInfo fi = new FileInfo(txtFileDestination.Text);
            if (fi.Exists)
            {
                DialogResult dr = Common.ShowMsg(string.Format("{0} already exists.\r\nDo you want to replace it?", txtFileDestination.Text), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (dr != DialogResult.Yes) { txtFileDestination.Focus(); return false; }
            }

            return true;
        }

        private void DoExport()
        {

            StreamWriter sw;
            CsvWriter csv;

            try
            {
                sw = new StreamWriter(txtFileDestination.Text);
                csv = new CsvWriter(sw);
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Unable to open {0}\r\n{1}", txtFileDestination.Text, ex.Message));
                return;
            }

            SQLiteConnection conn = new SQLiteConnection();
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteErrorCode returnCode;
            long RecordCount;

            var rtn = DataAccess.OpenDB(DatabaseLocation, ref conn, ref cmd, out returnCode, false);
            if (!rtn || returnCode != SQLiteErrorCode.Ok)
            {
                Common.ShowMsg("Unable to open database.");
                return;
            }

            try
            {
                cmd.CommandText = string.Format("Select Count(*) From {0}", listBoxTables.SelectedItem.ToString());
                RecordCount = Convert.ToInt64(cmd.ExecuteScalar());
                cmd.CommandText = string.Format("Select * From {0}", listBoxTables.SelectedItem.ToString());
                SQLiteDataReader dr = cmd.ExecuteReader();
                if (checkBoxHeadings.Checked) //sw.WriteLine(GetHeadingLine(dr));
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
                    //w.WriteLine(GetDetailLine(dr));
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        csv.WriteField(DBNull.Value.Equals(dr[i]) ? string.Empty : dr[i].ToString()); ;
                    }
                    csv.NextRecord();
                    recs++;
                    if (recs % 100 == 0) { toolStripStatusLabel1.Text = string.Format("{0} of {1} records written", recs, RecordCount); Application.DoEvents(); }
                }
                toolStripStatusLabel1.Text = "Export Complete";
                sw.Close();
                dr.Close();
            }
            catch (Exception ex)
            {
                Common.ShowMsg(string.Format("Export Failed.\r\n{0}", ex.Message));
                return;
            }
            finally
            {
                DataAccess.CloseDB(conn);
            }
            return;
        }

        private string GetHeadingLine(SQLiteDataReader dr)
        {
            StringBuilder sb = new StringBuilder();
            string comma = string.Empty;

            for (int i = 0; i < dr.FieldCount; i++)
            {
                sb.AppendFormat("{0}{1}", comma, enQuote(dr.GetName(i)));
                comma = ",";
            }
            return sb.ToString();
        }

        private string GetDetailLine(SQLiteDataReader dr)
        {
            StringBuilder sb = new StringBuilder();
            string comma = string.Empty;

            for (int i = 0; i < dr.FieldCount; i++)
            {
                sb.AppendFormat("{0}{1}", comma, enQuote(DBNull.Value.Equals(dr[i]) ? string.Empty : dr[i].ToString()));
                comma = ",";
            }
            return sb.ToString();
        }

        private string enQuote(string token)
        {
            return token.Contains(",") ? string.Format("\"{0}\"", token) : token;
        }

        #region ControlBox Handlers
        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
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


    }
}
