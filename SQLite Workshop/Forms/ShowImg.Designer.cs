namespace SQLiteWorkshop
{
    partial class ShowImg

    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowImg));
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblFormHeading = new System.Windows.Forms.Label();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pbMax = new System.Windows.Forms.PictureBox();
            this.pbMin = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelBody = new System.Windows.Forms.Panel();
            this.richTextBoxData = new System.Windows.Forms.RichTextBox();
            this.spLeft = new System.Windows.Forms.Splitter();
            this.spTop = new System.Windows.Forms.Splitter();
            this.spRight = new System.Windows.Forms.Splitter();
            this.spBottom = new System.Windows.Forms.Splitter();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.panelControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Controls.Add(this.lblFormHeading);
            this.panelTop.Controls.Add(this.pbIcon);
            this.panelTop.Controls.Add(this.panelControls);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 2);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(350, 28);
            this.panelTop.TabIndex = 1;
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            // 
            // lblFormHeading
            // 
            this.lblFormHeading.AutoSize = true;
            this.lblFormHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFormHeading.ForeColor = System.Drawing.Color.DimGray;
            this.lblFormHeading.Location = new System.Drawing.Point(29, 7);
            this.lblFormHeading.Name = "lblFormHeading";
            this.lblFormHeading.Size = new System.Drawing.Size(121, 17);
            this.lblFormHeading.TabIndex = 5;
            this.lblFormHeading.Text = "lblFormHeading";
            // 
            // pbIcon
            // 
            this.pbIcon.Image = ((System.Drawing.Image)(resources.GetObject("pbIcon.Image")));
            this.pbIcon.Location = new System.Drawing.Point(3, 3);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(24, 24);
            this.pbIcon.TabIndex = 4;
            this.pbIcon.TabStop = false;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.pbClose);
            this.panelControls.Controls.Add(this.pbMax);
            this.panelControls.Controls.Add(this.pbMin);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControls.Location = new System.Drawing.Point(227, 0);
            this.panelControls.Name = "panelControls";
            this.panelControls.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panelControls.Size = new System.Drawing.Size(123, 28);
            this.panelControls.TabIndex = 3;
            // 
            // pbClose
            // 
            this.pbClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.pbClose.Image = ((System.Drawing.Image)(resources.GetObject("pbClose.Image")));
            this.pbClose.Location = new System.Drawing.Point(89, 0);
            this.pbClose.Margin = new System.Windows.Forms.Padding(0);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(24, 28);
            this.pbClose.TabIndex = 11;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            this.pbClose.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ControlBox_MouseDown);
            this.pbClose.MouseEnter += new System.EventHandler(this.ControlBox_MouseEnter);
            this.pbClose.MouseLeave += new System.EventHandler(this.ControlBox_MouseLeave);
            this.pbClose.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ControlBox_MouseUp);
            // 
            // pbMax
            // 
            this.pbMax.Image = ((System.Drawing.Image)(resources.GetObject("pbMax.Image")));
            this.pbMax.Location = new System.Drawing.Point(50, 0);
            this.pbMax.Margin = new System.Windows.Forms.Padding(0);
            this.pbMax.Name = "pbMax";
            this.pbMax.Size = new System.Drawing.Size(24, 28);
            this.pbMax.TabIndex = 10;
            this.pbMax.TabStop = false;
            this.pbMax.Click += new System.EventHandler(this.pbMax_Click);
            this.pbMax.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ControlBox_MouseDown);
            this.pbMax.MouseEnter += new System.EventHandler(this.ControlBox_MouseEnter);
            this.pbMax.MouseLeave += new System.EventHandler(this.ControlBox_MouseLeave);
            this.pbMax.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ControlBox_MouseUp);
            // 
            // pbMin
            // 
            this.pbMin.Dock = System.Windows.Forms.DockStyle.Left;
            this.pbMin.Image = ((System.Drawing.Image)(resources.GetObject("pbMin.Image")));
            this.pbMin.Location = new System.Drawing.Point(10, 0);
            this.pbMin.Margin = new System.Windows.Forms.Padding(0);
            this.pbMin.Name = "pbMin";
            this.pbMin.Size = new System.Drawing.Size(24, 28);
            this.pbMin.TabIndex = 9;
            this.pbMin.TabStop = false;
            this.pbMin.Click += new System.EventHandler(this.pbMin_Click);
            this.pbMin.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ControlBox_MouseDown);
            this.pbMin.MouseEnter += new System.EventHandler(this.ControlBox_MouseEnter);
            this.pbMin.MouseLeave += new System.EventHandler(this.ControlBox_MouseLeave);
            this.pbMin.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ControlBox_MouseUp);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(350, 289);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // panelBody
            // 
            this.panelBody.AutoScroll = true;
            this.panelBody.AutoSize = true;
            this.panelBody.Controls.Add(this.richTextBoxData);
            this.panelBody.Controls.Add(this.pictureBox1);
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.Location = new System.Drawing.Point(1, 30);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(350, 289);
            this.panelBody.TabIndex = 4;
            // 
            // richTextBoxData
            // 
            this.richTextBoxData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxData.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxData.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxData.Name = "richTextBoxData";
            this.richTextBoxData.ReadOnly = true;
            this.richTextBoxData.Size = new System.Drawing.Size(350, 289);
            this.richTextBoxData.TabIndex = 5;
            this.richTextBoxData.Text = "";
            this.richTextBoxData.Visible = false;
            this.richTextBoxData.WordWrap = false;
            // 
            // spLeft
            // 
            this.spLeft.Location = new System.Drawing.Point(1, 30);
            this.spLeft.Name = "spLeft";
            this.spLeft.Size = new System.Drawing.Size(1, 289);
            this.spLeft.TabIndex = 7;
            this.spLeft.TabStop = false;
            this.spLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sp_MouseDown);
            this.spLeft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spMouseMove);
            this.spLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sp_MouseUp);
            // 
            // spTop
            // 
            this.spTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.spTop.Location = new System.Drawing.Point(1, 1);
            this.spTop.Name = "spTop";
            this.spTop.Size = new System.Drawing.Size(350, 1);
            this.spTop.TabIndex = 10;
            this.spTop.TabStop = false;
            this.spTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sp_MouseDown);
            this.spTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spMouseMove);
            this.spTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sp_MouseUp);
            // 
            // spRight
            // 
            this.spRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.spRight.Location = new System.Drawing.Point(350, 30);
            this.spRight.Name = "spRight";
            this.spRight.Size = new System.Drawing.Size(1, 289);
            this.spRight.TabIndex = 11;
            this.spRight.TabStop = false;
            this.spRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sp_MouseDown);
            this.spRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spMouseMove);
            this.spRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sp_MouseUp);
            // 
            // spBottom
            // 
            this.spBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.spBottom.Location = new System.Drawing.Point(2, 318);
            this.spBottom.Name = "spBottom";
            this.spBottom.Size = new System.Drawing.Size(348, 1);
            this.spBottom.TabIndex = 12;
            this.spBottom.TabStop = false;
            this.spBottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sp_MouseDown);
            this.spBottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spMouseMove);
            this.spBottom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sp_MouseUp);
            // 
            // ShowImg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(352, 320);
            this.Controls.Add(this.spBottom);
            this.Controls.Add(this.spRight);
            this.Controls.Add(this.spLeft);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.spTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShowImg";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Template";
            this.Load += new System.EventHandler(this.ShowImg_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.panelControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelBody.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.PictureBox pbMax;
        private System.Windows.Forms.PictureBox pbMin;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblFormHeading;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.RichTextBox richTextBoxData;
        private System.Windows.Forms.Splitter spLeft;
        private System.Windows.Forms.Splitter spTop;
        private System.Windows.Forms.Splitter spRight;
        private System.Windows.Forms.Splitter spBottom;
    }
}