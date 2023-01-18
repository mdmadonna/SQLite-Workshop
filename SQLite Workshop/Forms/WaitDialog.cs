using System;
using System.Drawing;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

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
            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            HouseKeeping(this, "Wait...");
            this.Cursor = Cursors.WaitCursor;
            btnCancel.Cursor = Cursors.Default;
        }

        private void WaitDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
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
    }

    public class CancelEventArgs : EventArgs
    {
        public bool cancel = false;
    }

}
