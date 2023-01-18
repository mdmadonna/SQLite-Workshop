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

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class Preview : Form
    {
        ToolTip toolTip;
        public Preview()
        {
            InitializeComponent();
        }

        private void Preview_Load(object sender, EventArgs e)
        {
            toolTip = new ToolTip();
            HouseKeeping(this, "Import Preview", true);
            this.MaximumSize = Screen.FromRectangle(this.Bounds).WorkingArea.Size;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void Preview_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        internal void setPreview(string table, DataTable dt)
        {
            this.Text = string.Format("Preview {0}", table);
            lblTable.Text = string.Format("Table: {0}", table);
            dgPreview.DataSource = dt;
            dgPreview.Refresh();
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
