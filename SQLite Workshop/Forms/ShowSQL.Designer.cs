﻿namespace SQLiteWorkshop
{
    partial class ShowSQL
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShowSQL));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblFormHeading = new System.Windows.Forms.Label();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.panelControls = new System.Windows.Forms.Panel();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.btnParse = new System.Windows.Forms.Button();
            this.toolStripStatusLabelResults = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.panelControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.panelBottom.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelResults});
            this.statusStrip1.Location = new System.Drawing.Point(1, 369);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(649, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Controls.Add(this.lblFormHeading);
            this.panelTop.Controls.Add(this.pbIcon);
            this.panelTop.Controls.Add(this.panelControls);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(649, 28);
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
            this.pbIcon.Image = global::SQLiteWorkshop.Properties.Resources.MainSmall;
            this.pbIcon.Location = new System.Drawing.Point(3, 3);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(24, 24);
            this.pbIcon.TabIndex = 4;
            this.pbIcon.TabStop = false;
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.pbClose);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControls.Location = new System.Drawing.Point(526, 0);
            this.panelControls.Name = "panelControls";
            this.panelControls.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panelControls.Size = new System.Drawing.Size(123, 28);
            this.panelControls.TabIndex = 3;
            // 
            // pbClose
            // 
            this.pbClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.pbClose.Image = global::SQLiteWorkshop.Properties.Resources.Close;
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
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelBottom.Controls.Add(this.btnParse);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 324);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(649, 45);
            this.panelBottom.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(564, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.txtSQL);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(649, 295);
            this.panel1.TabIndex = 3;
            // 
            // txtSQL
            // 
            this.txtSQL.BackColor = System.Drawing.SystemColors.Window;
            this.txtSQL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSQL.Location = new System.Drawing.Point(0, 0);
            this.txtSQL.Multiline = true;
            this.txtSQL.Name = "txtSQL";
            this.txtSQL.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSQL.Size = new System.Drawing.Size(649, 295);
            this.txtSQL.TabIndex = 0;
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(464, 13);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(75, 23);
            this.btnParse.TabIndex = 1;
            this.btnParse.Text = "Parse";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // toolStripStatusLabelResults
            // 
            this.toolStripStatusLabelResults.Name = "toolStripStatusLabelResults";
            this.toolStripStatusLabelResults.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabelResults.Text = "toolStripStatusLabel1";
            // 
            // ShowSQL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(651, 392);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShowSQL";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "ShowSQL";
            this.Load += new System.EventHandler(this.ShowSQL_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.panelControls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Label lblFormHeading;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelResults;
    }
}