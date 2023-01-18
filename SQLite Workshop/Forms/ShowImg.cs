using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class ShowImg : Form
    {
        ToolTip toolTip;
        Label lblFormHeading = null;
        public ShowImg()
        {
            InitializeComponent();
            toolTip = new ToolTip();
            HouseKeeping(this, string.Empty, true);
            lblFormHeading = this.Controls.Find("lblFormHeading", true).FirstOrDefault() as Label;
        }

        private void ShowImg_Load(object sender, EventArgs e)
        {
            this.MaximumSize = Screen.FromRectangle(this.Bounds).WorkingArea.Size;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void ShowImg_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
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
        #endregion
    }
}
