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

namespace SQLiteWorkshop
{
    public partial class WaitDialog : Form
    {

        public event EventHandler<CancelEventArgs> CancelReport = delegate { };
        public void CancelEventHandler(object sender, CancelEventArgs e)
        {
            CancelReport(sender, e);
        }

        ToolTip toolTip;

        public int maxObjects {
            get { return progressLoad.Maximum / progressLoad.Step; }
            set { progressLoad.Maximum = value * 10;}
        }

        public int minObjects
        {
            get { return progressLoad.Minimum / progressLoad.Step; }
            set { progressLoad.Minimum = value * progressLoad.Step; }
        }


        public WaitDialog()
        {
            InitializeComponent();
        }

        public void ReportProgress(string curObjectName, bool bComplete)
        {
            toolStripStatusLabel.Text = string.Format("Analyzing {0}", curObjectName);
            if (bComplete) progressLoad.PerformStep();
        }
        private void WaitDialog_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = "Wait...";

            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbClose, "Close");
            this.Cursor = Cursors.WaitCursor;
            btnCancel.Cursor = Cursors.Default;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelEventArgs ce = new CancelEventArgs();
            ce.cancel = true;
            CancelReport(sender, ce);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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

    public class CancelEventArgs : EventArgs
    {
        public bool cancel = false;
    }
}
