namespace SQLiteWorkshop
{
    partial class DBRecover
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblPanelHeading = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDbIn = new System.Windows.Forms.TextBox();
            this.lblNewColumnName = new System.Windows.Forms.Label();
            this.txtDbOut = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblError = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioDump = new System.Windows.Forms.RadioButton();
            this.radioRecover = new System.Windows.Forms.RadioButton();
            this.btnDbIn = new System.Windows.Forms.Button();
            this.btnDbOut = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusMsg});
            this.statusStrip.Location = new System.Drawing.Point(1, 325);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(500, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusMsg
            // 
            this.toolStripStatusMsg.Name = "toolStripStatusMsg";
            this.toolStripStatusMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(500, 28);
            this.panelTop.TabIndex = 1;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelBottom.Controls.Add(this.btnSave);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 280);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(500, 45);
            this.panelBottom.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(283, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Start";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(386, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblPanelHeading
            // 
            this.lblPanelHeading.AutoSize = true;
            this.lblPanelHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblPanelHeading.ForeColor = System.Drawing.Color.DimGray;
            this.lblPanelHeading.Location = new System.Drawing.Point(13, 22);
            this.lblPanelHeading.Name = "lblPanelHeading";
            this.lblPanelHeading.Size = new System.Drawing.Size(114, 15);
            this.lblPanelHeading.TabIndex = 4;
            this.lblPanelHeading.Text = "lblPanelHeading";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Database to Recover::";
            // 
            // txtDbIn
            // 
            this.txtDbIn.Location = new System.Drawing.Point(16, 69);
            this.txtDbIn.Name = "txtDbIn";
            this.txtDbIn.Size = new System.Drawing.Size(400, 20);
            this.txtDbIn.TabIndex = 1;
            // 
            // lblNewColumnName
            // 
            this.lblNewColumnName.AutoSize = true;
            this.lblNewColumnName.Location = new System.Drawing.Point(13, 102);
            this.lblNewColumnName.Name = "lblNewColumnName";
            this.lblNewColumnName.Size = new System.Drawing.Size(143, 13);
            this.lblNewColumnName.TabIndex = 9;
            this.lblNewColumnName.Text = "Recovered Database Name:";
            // 
            // txtDbOut
            // 
            this.txtDbOut.Location = new System.Drawing.Point(16, 118);
            this.txtDbOut.Name = "txtDbOut";
            this.txtDbOut.Size = new System.Drawing.Size(400, 20);
            this.txtDbOut.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.Controls.Add(this.lblError);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.btnDbIn);
            this.panel2.Controls.Add(this.btnDbOut);
            this.panel2.Controls.Add(this.txtDbOut);
            this.panel2.Controls.Add(this.lblPanelHeading);
            this.panel2.Controls.Add(this.lblNewColumnName);
            this.panel2.Controls.Add(this.txtDbIn);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(500, 251);
            this.panel2.TabIndex = 11;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(16, 214);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioDump);
            this.panel1.Controls.Add(this.radioRecover);
            this.panel1.Location = new System.Drawing.Point(16, 160);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 38);
            this.panel1.TabIndex = 5;
            // 
            // radioDump
            // 
            this.radioDump.AutoSize = true;
            this.radioDump.Location = new System.Drawing.Point(167, 11);
            this.radioDump.Name = "radioDump";
            this.radioDump.Size = new System.Drawing.Size(53, 17);
            this.radioDump.TabIndex = 1;
            this.radioDump.Text = "Dump";
            this.radioDump.UseVisualStyleBackColor = true;
            this.radioDump.CheckedChanged += new System.EventHandler(this.radioDump_CheckedChanged);
            // 
            // radioRecover
            // 
            this.radioRecover.AutoSize = true;
            this.radioRecover.Checked = true;
            this.radioRecover.Location = new System.Drawing.Point(28, 11);
            this.radioRecover.Name = "radioRecover";
            this.radioRecover.Size = new System.Drawing.Size(66, 17);
            this.radioRecover.TabIndex = 0;
            this.radioRecover.TabStop = true;
            this.radioRecover.Text = "Recover";
            this.radioRecover.UseVisualStyleBackColor = true;
            this.radioRecover.CheckedChanged += new System.EventHandler(this.radioRecover_CheckedChanged);
            // 
            // btnDbIn
            // 
            this.btnDbIn.Location = new System.Drawing.Point(422, 67);
            this.btnDbIn.Name = "btnDbIn";
            this.btnDbIn.Size = new System.Drawing.Size(27, 23);
            this.btnDbIn.TabIndex = 2;
            this.btnDbIn.Text = "...";
            this.btnDbIn.UseVisualStyleBackColor = true;
            this.btnDbIn.Click += new System.EventHandler(this.btnDbIn_Click);
            // 
            // btnDbOut
            // 
            this.btnDbOut.Location = new System.Drawing.Point(422, 116);
            this.btnDbOut.Name = "btnDbOut";
            this.btnDbOut.Size = new System.Drawing.Size(27, 23);
            this.btnDbOut.TabIndex = 4;
            this.btnDbOut.Text = "...";
            this.btnDbOut.UseVisualStyleBackColor = true;
            this.btnDbOut.Click += new System.EventHandler(this.btnDbOut_Click);
            // 
            // DBRecover
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(502, 348);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DBRecover";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Database Recovery";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DBRecover_FormClosed);
            this.Load += new System.EventHandler(this.DBRecover_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblPanelHeading;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDbIn;
        private System.Windows.Forms.Label lblNewColumnName;
        private System.Windows.Forms.TextBox txtDbOut;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnDbIn;
        private System.Windows.Forms.Button btnDbOut;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioDump;
        private System.Windows.Forms.RadioButton radioRecover;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusMsg;
    }
}