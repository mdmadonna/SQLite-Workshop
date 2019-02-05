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
    public partial class ShowMsg : Form
    {
        ToolTip toolTip;

        public enum ButtonStyle
        {
            OKCancel,
            OK
        }

        public ButtonStyle ButtonStyleValue { get; set; }
        public string Caption { get; set; }

        public string Message { get; set; }

        public DialogResult Result { get; set; }

        public bool DoNotShow { get; set; }

        public ShowMsg(ButtonStyle buttonStyle = ButtonStyle.OKCancel)
        {
            ButtonStyleValue = buttonStyle;
            InitializeComponent();
        }

        private void ShowMsg_Load(object sender, EventArgs e)
        {
            lblFormHeading.Text = string.IsNullOrEmpty(Caption) ? "Warning" : Caption;
            textBoxMessage.Text = Message;
            Result = DialogResult.Cancel;       //Set default even if Cancel Button is not displayed

            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbClose, "Close");

            switch (ButtonStyleValue)
            {
                case ButtonStyle.OKCancel:          //Default - do nothing
                    break;
                case ButtonStyle.OK:                //Hide Cancel button and move OK button
                    btnCancel.Visible = false;
                    btnOK.Left = btnCancel.Left;
                    break;
                default:
                    break;
            }
        }

        private void ShowMsg_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoNotShow = checkBoxDoNot.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
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
}
