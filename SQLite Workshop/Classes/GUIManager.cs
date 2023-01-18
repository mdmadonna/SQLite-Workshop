using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;

namespace SQLiteWorkshop
{
    static class GUIManager
    {
        #region Form Handling
        #region UI_Helpers

        /// <summary>
        /// All Forms must call here in thier constructor or during their Load Event.
        /// </summary>
        /// <param name="f">Form instance that is opened.</param>
        static internal void HouseKeeping(Form f, string caption, bool FixedSize = true)
        {
            f.Icon = Properties.Resources.MainIcon;
            LoadParms(f);
            StyleForm(f);
            LoadTips(f);
            BuildTopPanel(f, caption, FixedSize);
        }

        /// <summary>
        /// All forms that close must call here in their Form_Closing event
        /// </summary>
        /// <param name="f">Form instance that is closing.</param>
        static internal void FormClose(Form f)
        {
            SaveParms(f);
            SaveGrids(f);

            // Unhook Dragging Event Handlers
            Panel p = f.Controls.Find("panelTop", true).FirstOrDefault() as Panel;
            if (p != null) p.MouseDown -= SQ_MouseDown;
            if (p?.Controls.Find("lblFormHeading", true).FirstOrDefault() is Label l) l.MouseDown -= SQ_MouseDown;
        }

        /// <summary>
        /// Load Form tooltips
        /// </summary>
        /// <param name="f"></param>
        static private void LoadTips(Form f)
        {
            //t.SetToolTip(pbMin, "Minimize");
            //t.SetToolTip(pbMax, "Maximize");
            //t.SetToolTip(pbClose, "Close");
        }

        /// <summary>
        /// Apply Styles to Form
        /// </summary>
        /// <param name="container"></param>
        static private void StyleForm(Control container)
        {

            foreach (Control c in container.Controls)
            {
                if (c.GetType().Equals(typeof(Button)))
                {
                    Button b = (Button)c;
                    b.BackColor = Color.SteelBlue;
                    b.ForeColor = Color.White;
                    b.FlatAppearance.MouseOverBackColor = Color.Olive;
                    b.FlatAppearance.BorderSize = 0;
                    b.FlatStyle = FlatStyle.Flat;
                }
                else if (c.GetType().Equals(typeof(DataGridView)))
                {
                    LoadParms((DataGridView)c);
                }
                else if (c.GetType().Equals(typeof(ListView)))
                {
                    LoadParms((ListView)c);
                }
                if (c.HasChildren) StyleForm(c);
            }
        }

        static private void SaveGrids(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c.GetType().Equals(typeof(DataGridView)))
                {
                    SaveParms((DataGridView)c);
                }
                else if (c.GetType().Equals(typeof(ListView)))
                {
                    SaveParms((ListView)c);
                }
                if (c.HasChildren) SaveGrids(c);
            }
        }

        static private void SaveParms(Form f)
        {
            saveSetting(string.Format("{0}_TOP", f.Name), f.Top.ToString());
            saveSetting(string.Format("{0}_LEFT", f.Name), f.Left.ToString());
            saveSetting(string.Format("{0}_HEIGHT", f.Name), f.Height.ToString());
            saveSetting(string.Format("{0}_WIDTH", f.Name), f.Width.ToString());
            SaveSplitters(f);
        }
        static private void SaveSplitters(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c.GetType().Equals(typeof(Splitter)))
                    saveSetting(string.Format("{0}_POSITION", c.Name), ((Splitter)c).SplitPosition.ToString());
                if (c.HasChildren) SaveSplitters(c);
            }
        }

        static private void LoadParms(Form f)
        {
            if (int.TryParse(appSetting(string.Format("{0}_TOP", f.Name)), out int top)) f.Top = top;
            if (top < 0 || top > Screen.PrimaryScreen.Bounds.Height) f.Top = 0;
            if (int.TryParse(appSetting(string.Format("{0}_LEFT", f.Name)), out int left)) f.Left = left;
            if (left < 0 || left > Screen.PrimaryScreen.Bounds.Width) f.Left = 0;
            if (int.TryParse(appSetting(string.Format("{0}_HEIGHT", f.Name)), out int height)) f.Height = height;
            if (int.TryParse(appSetting(string.Format("{0}_Width", f.Name)), out int width)) f.Width = width;
            LoadSplitters(f);
        }

        static private void LoadSplitters(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c.GetType().Equals(typeof(Splitter)))
                    if (int.TryParse(appSetting(string.Format("{0}_POSITION", c.Name)), out int position)) ((Splitter)c).SplitPosition = position;
                if (c.HasChildren) LoadSplitters(c);
            }
        }

        static private void SaveParms(DataGridView dgv)
        {
            if (dgv.Columns.Count > 0)
            {
                foreach (DataGridViewColumn dvc in dgv.Columns)
                {
                    saveSetting(string.Format("{0}Wid_{1}", dgv.Name, dvc.Name), dvc.Width.ToString());
                }
            }
        }

        static private void LoadParms(DataGridView dgv)
        {
            if (dgv.Columns.Count > 0)
            {
                foreach (DataGridViewColumn dvc in dgv.Columns)
                {
                    if (int.TryParse(appSetting(string.Format("{0}Wid_{1}", dgv.Name, dvc.Name)), out int wid))
                        dvc.Width = wid;
                }
            }
        }

        static private void SaveParms(ListView lv)
        {
            if (lv.Columns.Count > 0)
            {
                foreach (ColumnHeader ch in lv.Columns)
                {
                    saveSetting(string.Format("{0}Wid_{1}", lv.Name, ch.Text), ch.Width.ToString());
                }
            }
        }

        static private void LoadParms(ListView lv)
        {
            if (lv.Columns.Count > 0)
            {
                foreach (ColumnHeader ch in lv.Columns)
                {
                    if (int.TryParse(appSetting(string.Format("{0}Wid_{1}", lv.Name, ch.Text)), out int wid))
                        ch.Width = wid;
                }
            }
        }

        internal static void BuildTopPanel(Form f, string caption, bool FixedSize)
        {
            Panel topPanel = f.Controls.Find("panelTop", true).FirstOrDefault() as Panel;
            // 
            // pbIcon
            // 
            PictureBox pbIcon = new PictureBox();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            pbIcon.Image = ((System.Drawing.Image)(resources.GetObject("pbIcon.Image")));
            pbIcon.Location = new Point(3, 3);
            pbIcon.Name = "pbIcon";
            pbIcon.Size = new Size(24, 24);
            pbIcon.TabStop = false;
            topPanel.Controls.Add(pbIcon);
            // 
            // lblFormHeading
            // 
            Label lblFormHeading = new Label();
            lblFormHeading.AutoSize = true;
            lblFormHeading.Font = new Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lblFormHeading.ForeColor = Color.DimGray;
            lblFormHeading.Location = new Point(29, 7);
            lblFormHeading.Name = "lblFormHeading";
            lblFormHeading.Size = new Size(121, 17);
            lblFormHeading.Text = caption;
            topPanel.Controls.Add(lblFormHeading);
            //
            // Control Box Panel
            //
            Panel panelControls = new Panel();
            panelControls.Dock = DockStyle.Right;
            panelControls.Location = new Point(526, 0);
            panelControls.Name = "panelControls";
            panelControls.Padding = new Padding(10, 0, 10, 0);
            panelControls.Size = new Size(123, 28);
            // 
            // pbClose
            // 
            PictureBox pbClose = new PictureBox();
            pbClose.Dock = DockStyle.Right;
            pbClose.Image = Properties.Resources.Close;
            pbClose.Location = new Point(89, 0);
            pbClose.Margin = new Padding(0);
            pbClose.Name = "pbClose";
            pbClose.Size = new Size(24, 28);
            pbClose.TabStop = false;
            pbClose.Click += pbClose_Click;
            pbClose.MouseDown += ControlBox_MouseDown;
            pbClose.MouseEnter += ControlBox_MouseEnter;
            pbClose.MouseLeave += ControlBox_MouseLeave;
            pbClose.MouseUp += ControlBox_MouseUp;

            panelControls.Controls.Add(pbClose);
            topPanel.Controls.Add(panelControls);

            // Enable Dragging
            // Assigning static Event Handlers to a transient control is a little 
            // sketchy. We need to insure that they are always unHooked.
            topPanel.MouseDown += SQ_MouseDown;
            lblFormHeading.MouseDown += SQ_MouseDown;

            if (FixedSize) return;

            // 
            // pbMax
            //
            PictureBox pbMax = new PictureBox();
            pbMax.Image = Properties.Resources.Maximize;
            pbMax.Location = new Point(50, 0);
            pbMax.Margin = new Padding(0);
            pbMax.Name = "pbMax";
            pbMax.Size = new Size(24, 28);
            pbMax.TabStop = false;
            pbMax.Click += pbMax_Click;
            pbMax.MouseDown += ControlBox_MouseDown;
            pbMax.MouseEnter += ControlBox_MouseEnter;
            pbMax.MouseLeave += ControlBox_MouseLeave;
            pbMax.MouseUp += ControlBox_MouseUp;
            // 
            // pbMin
            // 
            PictureBox pbMin = new PictureBox();
            pbMin.Dock = DockStyle.Left;
            pbMin.Image = Properties.Resources.Minimize;
            pbMin.Location = new Point(10, 0);
            pbMin.Margin = new Padding(0);
            pbMin.Name = "pbMin";
            pbMin.Size = new Size(24, 28);
            pbMin.TabStop = false;
            pbMin.Click += pbMin_Click;
            pbMin.MouseDown += ControlBox_MouseDown;
            pbMin.MouseEnter += ControlBox_MouseEnter;
            pbMin.MouseLeave += ControlBox_MouseLeave;
            pbMin.MouseUp += ControlBox_MouseUp;

            panelControls.Controls.Add(pbMin);
            panelControls.Controls.Add(pbMax);


        }

        #endregion

        #region Form Dragging Event Handlers
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        internal static void SQ_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                IntPtr hWnd = GetForegroundWindow();
                SendMessage(hWnd, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        #endregion

        #region ControlBox Handlers

        internal static Form ParentForm(object sender)
        {
            Control p = (Control)sender;
            while (true)
            {
                if (p.Parent.GetType().BaseType.Equals(typeof(Form))) break;
                p = p.Parent;
            }
            return p.Parent as Form;
        }
        internal static void pbClose_Click(object sender, EventArgs e)
        {
            Form.ActiveForm.Close();
        }

        internal static void pbMax_Click(object sender, EventArgs e)
        {
            string tip = string.Empty;
            if (Form.ActiveForm.WindowState == FormWindowState.Maximized)
            {
                Form.ActiveForm.WindowState = FormWindowState.Normal;
                ((PictureBox)sender).Image = Properties.Resources.Restore;
                //toolTip.SetToolTip(sender, "Maximize");
                tip = "Maximize";
            }
            else
            {
                Form.ActiveForm.WindowState = FormWindowState.Maximized;
                ((PictureBox)sender).Image = Properties.Resources.Maximize;
                //toolTip.SetToolTip(sender, "Restore");
                tip = "Minimize";
            }
            // Every Form must have a toolTip object
            //var toolTip = Form.ActiveForm.GetType().GetProperty("toolTip", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(sender, null);
            //var setTip = toolTip.GetType().GetMethod("SetToolTip");
            //setTip.Invoke(toolTip, new object[] { sender, tip });
            ((PictureBox)sender).Refresh();
        }

        internal static void pbMin_Click(object sender, EventArgs e)
        {
            ParentForm(sender).WindowState = FormWindowState.Minimized;
        }

        internal static void ControlBox_MouseEnter(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.White;
        }

        internal static void ControlBox_MouseLeave(object sender, EventArgs e)
        {
            ((PictureBox)sender).BackColor = SystemColors.InactiveCaption;
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }
        internal static void ControlBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ((PictureBox)sender).BackColor = Color.Wheat;
            ((PictureBox)sender).BorderStyle = BorderStyle.Fixed3D;
        }

        internal static void ControlBox_MouseUp(object sender, MouseEventArgs e)
        {
            ((PictureBox)sender).BackColor = SystemColors.InactiveCaption;
            ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }
        #endregion
        #endregion
    }
}
