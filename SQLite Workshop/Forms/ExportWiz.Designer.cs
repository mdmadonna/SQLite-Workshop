namespace SQLiteWorkshop
{
    partial class ExportWiz
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportWiz));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.radioSQL = new System.Windows.Forms.RadioButton();
            this.radioComma = new System.Windows.Forms.RadioButton();
            this.btnFindFile = new System.Windows.Forms.Button();
            this.txtFileDestination = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxHeadings = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxTables = new System.Windows.Forms.ListBox();
            this.statusStrip1.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(1, 369);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(649, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(649, 28);
            this.panelTop.TabIndex = 1;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelBottom.Controls.Add(this.btnExport);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 324);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(649, 45);
            this.panelBottom.TabIndex = 2;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(471, 13);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 1;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
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
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.btnFindFile);
            this.panel1.Controls.Add(this.txtFileDestination);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.checkBoxHeadings);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.listBoxTables);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(649, 295);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.radioSQL);
            this.panel2.Controls.Add(this.radioComma);
            this.panel2.Location = new System.Drawing.Point(230, 62);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(297, 30);
            this.panel2.TabIndex = 6;
            // 
            // radioSQL
            // 
            this.radioSQL.AutoSize = true;
            this.radioSQL.Location = new System.Drawing.Point(174, 4);
            this.radioSQL.Name = "radioSQL";
            this.radioSQL.Size = new System.Drawing.Size(46, 17);
            this.radioSQL.TabIndex = 1;
            this.radioSQL.Text = "SQL";
            this.radioSQL.UseVisualStyleBackColor = true;
            this.radioSQL.CheckedChanged += new System.EventHandler(this.radioSQL_CheckedChanged);
            // 
            // radioComma
            // 
            this.radioComma.AutoSize = true;
            this.radioComma.Checked = true;
            this.radioComma.Location = new System.Drawing.Point(16, 4);
            this.radioComma.Name = "radioComma";
            this.radioComma.Size = new System.Drawing.Size(68, 17);
            this.radioComma.TabIndex = 0;
            this.radioComma.TabStop = true;
            this.radioComma.Text = "Delimited";
            this.radioComma.UseVisualStyleBackColor = true;
            this.radioComma.CheckedChanged += new System.EventHandler(this.radioComma_CheckedChanged);
            // 
            // btnFindFile
            // 
            this.btnFindFile.Location = new System.Drawing.Point(539, 200);
            this.btnFindFile.Name = "btnFindFile";
            this.btnFindFile.Size = new System.Drawing.Size(29, 23);
            this.btnFindFile.TabIndex = 5;
            this.btnFindFile.Text = "...";
            this.btnFindFile.UseVisualStyleBackColor = true;
            this.btnFindFile.Click += new System.EventHandler(this.btnFindFile_Click);
            // 
            // txtFileDestination
            // 
            this.txtFileDestination.Location = new System.Drawing.Point(230, 202);
            this.txtFileDestination.Name = "txtFileDestination";
            this.txtFileDestination.Size = new System.Drawing.Size(297, 20);
            this.txtFileDestination.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Destination Filename:";
            // 
            // checkBoxHeadings
            // 
            this.checkBoxHeadings.AutoSize = true;
            this.checkBoxHeadings.Checked = true;
            this.checkBoxHeadings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxHeadings.Location = new System.Drawing.Point(227, 119);
            this.checkBoxHeadings.Name = "checkBoxHeadings";
            this.checkBoxHeadings.Size = new System.Drawing.Size(167, 17);
            this.checkBoxHeadings.TabIndex = 2;
            this.checkBoxHeadings.Text = "Include Headings in First Row";
            this.checkBoxHeadings.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(149, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select a Table/View to Export";
            // 
            // listBoxTables
            // 
            this.listBoxTables.FormattingEnabled = true;
            this.listBoxTables.Location = new System.Drawing.Point(30, 62);
            this.listBoxTables.Name = "listBoxTables";
            this.listBoxTables.Size = new System.Drawing.Size(141, 199);
            this.listBoxTables.Sorted = true;
            this.listBoxTables.TabIndex = 0;
            this.listBoxTables.SelectedIndexChanged += new System.EventHandler(this.listBoxTables_SelectedIndexChanged);
            // 
            // ExportWiz
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
            this.Name = "ExportWiz";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Export Wizard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExportWiz_FormClosed);
            this.Load += new System.EventHandler(this.ExportWiz_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnFindFile;
        private System.Windows.Forms.TextBox txtFileDestination;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxHeadings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxTables;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton radioSQL;
        private System.Windows.Forms.RadioButton radioComma;
    }
}