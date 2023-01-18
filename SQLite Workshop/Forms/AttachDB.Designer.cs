namespace SQLiteWorkshop
{
    partial class AttachDB
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
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelFill = new System.Windows.Forms.Panel();
            this.lblError = new System.Windows.Forms.Label();
            this.txtSchemaName = new System.Windows.Forms.TextBox();
            this.btnGetFile = new System.Windows.Forms.Button();
            this.txtDbName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblPanelHeading = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelFill.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStrip1.Location = new System.Drawing.Point(1, 262);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(550, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(550, 28);
            this.panelTop.TabIndex = 1;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelBottom.Controls.Add(this.btnSave);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 217);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(550, 45);
            this.panelBottom.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(324, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Attach";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(436, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelFill
            // 
            this.panelFill.BackColor = System.Drawing.SystemColors.Window;
            this.panelFill.Controls.Add(this.lblError);
            this.panelFill.Controls.Add(this.txtSchemaName);
            this.panelFill.Controls.Add(this.btnGetFile);
            this.panelFill.Controls.Add(this.txtDbName);
            this.panelFill.Controls.Add(this.label2);
            this.panelFill.Controls.Add(this.label1);
            this.panelFill.Controls.Add(this.lblPanelHeading);
            this.panelFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFill.Location = new System.Drawing.Point(1, 29);
            this.panelFill.Name = "panelFill";
            this.panelFill.Size = new System.Drawing.Size(550, 188);
            this.panelFill.TabIndex = 3;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(56, 150);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 13);
            this.lblError.TabIndex = 4;
            // 
            // txtSchemaName
            // 
            this.txtSchemaName.Location = new System.Drawing.Point(144, 116);
            this.txtSchemaName.Name = "txtSchemaName";
            this.txtSchemaName.Size = new System.Drawing.Size(164, 20);
            this.txtSchemaName.TabIndex = 3;
            // 
            // btnGetFile
            // 
            this.btnGetFile.Location = new System.Drawing.Point(461, 81);
            this.btnGetFile.Name = "btnGetFile";
            this.btnGetFile.Size = new System.Drawing.Size(27, 23);
            this.btnGetFile.TabIndex = 2;
            this.btnGetFile.Text = "...";
            this.btnGetFile.UseVisualStyleBackColor = true;
            this.btnGetFile.Click += new System.EventHandler(this.btnGetFile_Click);
            // 
            // txtDbName
            // 
            this.txtDbName.Location = new System.Drawing.Point(144, 83);
            this.txtDbName.Name = "txtDbName";
            this.txtDbName.Size = new System.Drawing.Size(309, 20);
            this.txtDbName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Schema Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Database:";
            // 
            // lblPanelHeading
            // 
            this.lblPanelHeading.AutoSize = true;
            this.lblPanelHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPanelHeading.ForeColor = System.Drawing.Color.DimGray;
            this.lblPanelHeading.Location = new System.Drawing.Point(53, 37);
            this.lblPanelHeading.Name = "lblPanelHeading";
            this.lblPanelHeading.Size = new System.Drawing.Size(114, 15);
            this.lblPanelHeading.TabIndex = 0;
            this.lblPanelHeading.Text = "lblPanelHeading";
            // 
            // AttachDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(552, 285);
            this.Controls.Add(this.panelFill);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AttachDB";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Template";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AttachDB_FormClosed);
            this.Load += new System.EventHandler(this.AttachDB_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panelFill.ResumeLayout(false);
            this.panelFill.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panelFill;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblPanelHeading;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSchemaName;
        private System.Windows.Forms.Button btnGetFile;
        private System.Windows.Forms.TextBox txtDbName;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
    }
}