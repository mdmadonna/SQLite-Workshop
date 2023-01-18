namespace SQLiteWorkshop
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelResults = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnParse = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSQL = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
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
            // toolStripStatusLabelResults
            // 
            this.toolStripStatusLabelResults.Name = "toolStripStatusLabelResults";
            this.toolStripStatusLabelResults.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabelResults.Text = "toolStripStatusLabel1";
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
            this.panelBottom.Controls.Add(this.btnParse);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 324);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(649, 45);
            this.panelBottom.TabIndex = 2;
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
            this.Name = "ShowSQL";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Parse SQL";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShowSQL_FormClosed);
            this.Load += new System.EventHandler(this.ShowSQL_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtSQL;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelResults;
    }
}