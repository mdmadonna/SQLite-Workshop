using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLiteWorkshop
{
    public partial class ShowImg : Form
    {
        ToolTip toolTip;
        public ShowImg()
        {
            InitializeComponent();
        }

        private void ShowImg_Load(object sender, EventArgs e)
        {
            // Establish ToolTips for various controls.
            toolTip = new ToolTip();
            toolTip.SetToolTip(pbMin, "Minimize");
            toolTip.SetToolTip(pbMax, "Maximize");
            toolTip.SetToolTip(pbClose, "Close");
            this.MaximumSize = Screen.FromRectangle(this.Bounds).WorkingArea.Size;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        internal bool setPicture(byte[] img)
        {
            try
            {
                var ms = new MemoryStream(img);
                Image pic = Image.FromStream(ms);
                int height = pic.Height;
                int width = pic.Width;
                this.Width = width;
                this.Height = height + panelTop.Height;
                pictureBox1.Image = pic;
                pictureBox1.Visible = true;
                richTextBoxData.Visible = false;
            }
            catch { return false; }
            lblFormHeading.Text = "Picture Viewer";
            return true;
        }

        internal bool setText(string text)
        {
            try
            {
                richTextBoxData.Text = text;
                pictureBox1.Visible = false;
                richTextBoxData.Visible = true;
                richTextBoxData.WordWrap = true;
            }
            catch { return false; }
            lblFormHeading.Text = "Text Viewer";
            return true;
        }

        internal bool setBinary(string text)
        {
            try
            {
                richTextBoxData.Text = text;
                pictureBox1.Visible = false;
                richTextBoxData.Visible = true;
                richTextBoxData.WordWrap = false;
                this.Width = 1000;
            }
            catch { return false; }
            lblFormHeading.Text = "Binary Viewer";
            return true;
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

        private void pbMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                pbMax.Image = Properties.Resources.Restore;
                toolTip.SetToolTip(pbMax, "Maximize");
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                pbMax.Image = Properties.Resources.Maximize;
                toolTip.SetToolTip(pbMax, "Restore");
            }
            pbMax.Refresh();
        }

        private void pbMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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

        #region Form Sizing and Control

        bool grabbed = false;
        int minWidth = 500;
        int minHeight = 200;

        private void sp_MouseDown(object sender, MouseEventArgs e)
        {
            grabbed = true;
        }

        private void sp_MouseUp(object sender, MouseEventArgs e)
        {
            grabbed = false;
        }

        private void spMouseMove(object sender, MouseEventArgs e)
        {
            if (!grabbed) return;
            int x = MousePosition.X;
            int y = MousePosition.Y;

            int newsize;

            switch (((Splitter)sender).Name)
            {

                case "spLeft":
                    newsize = this.Width + (this.Left - x);
                    if (newsize < minWidth) break;
                    this.Left = x;
                    this.Width = newsize;
                    break;
                case "spRight":
                    newsize = x - this.Left;
                    if (newsize < minWidth) break;
                    this.Width = newsize;
                    break;
                case "spTop":
                    newsize = this.Height + (this.Top - y);
                    if (newsize < minHeight) break;
                    this.Top = y;
                    this.Height = newsize;
                    break;
                case "spBottom":
                    newsize = y - this.Top;
                    if (newsize < minHeight) break;
                    this.Height = newsize;
                    break;
                default:
                    break;
            }
        }

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

        #endregion


        private void toolStripButtonIncrease_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonDecrease_Click(object sender, EventArgs e)
        {

        }
    }
}
