namespace SQLiteWorkshop
{
    partial class ExecuteForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblTxtInfo = new System.Windows.Forms.Label();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripExecutionStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTimerStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelFill = new System.Windows.Forms.Panel();
            this.btnGetFile = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.panelFill.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(319, 204);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(218, 204);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 3;
            this.btnExecute.Text = "Yes";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.SystemColors.Window;
            this.txtMessage.Location = new System.Drawing.Point(15, 16);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(382, 123);
            this.txtMessage.TabIndex = 4;
            // 
            // lblTxtInfo
            // 
            this.lblTxtInfo.AutoSize = true;
            this.lblTxtInfo.Location = new System.Drawing.Point(12, 167);
            this.lblTxtInfo.Name = "lblTxtInfo";
            this.lblTxtInfo.Size = new System.Drawing.Size(50, 13);
            this.lblTxtInfo.TabIndex = 5;
            this.lblTxtInfo.Text = "lblTxtInfo";
            // 
            // txtInfo
            // 
            this.txtInfo.Location = new System.Drawing.Point(85, 164);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(309, 20);
            this.txtInfo.TabIndex = 1;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripExecutionStatus,
            this.toolStripTimerStatus});
            this.statusStrip.Location = new System.Drawing.Point(1, 271);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(409, 22);
            this.statusStrip.TabIndex = 7;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripExecutionStatus
            // 
            this.toolStripExecutionStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripExecutionStatus.Name = "toolStripExecutionStatus";
            this.toolStripExecutionStatus.Size = new System.Drawing.Size(0, 17);
            this.toolStripExecutionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripTimerStatus
            // 
            this.toolStripTimerStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripTimerStatus.Name = "toolStripTimerStatus";
            this.toolStripTimerStatus.Size = new System.Drawing.Size(0, 17);
            this.toolStripTimerStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(409, 28);
            this.panelTop.TabIndex = 8;
            // 
            // panelFill
            // 
            this.panelFill.BackColor = System.Drawing.SystemColors.Window;
            this.panelFill.Controls.Add(this.btnGetFile);
            this.panelFill.Controls.Add(this.lblError);
            this.panelFill.Controls.Add(this.txtMessage);
            this.panelFill.Controls.Add(this.btnCancel);
            this.panelFill.Controls.Add(this.btnExecute);
            this.panelFill.Controls.Add(this.txtInfo);
            this.panelFill.Controls.Add(this.lblTxtInfo);
            this.panelFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFill.Location = new System.Drawing.Point(1, 29);
            this.panelFill.Name = "panelFill";
            this.panelFill.Size = new System.Drawing.Size(409, 242);
            this.panelFill.TabIndex = 9;
            // 
            // btnGetFile
            // 
            this.btnGetFile.Location = new System.Drawing.Point(367, 162);
            this.btnGetFile.Name = "btnGetFile";
            this.btnGetFile.Size = new System.Drawing.Size(27, 23);
            this.btnGetFile.TabIndex = 2;
            this.btnGetFile.Text = "...";
            this.btnGetFile.UseVisualStyleBackColor = true;
            this.btnGetFile.Click += new System.EventHandler(this.btnGetFile_Click);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(16, 213);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(39, 13);
            this.lblError.TabIndex = 7;
            this.lblError.Text = "lblError";
            // 
            // ExecuteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(411, 294);
            this.Controls.Add(this.panelFill);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ExecuteForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "ExecuteForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExecuteForm_FormClosed);
            this.Load += new System.EventHandler(this.ExecuteForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelFill.ResumeLayout(false);
            this.panelFill.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lblTxtInfo;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripExecutionStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripTimerStatus;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelFill;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnGetFile;
    }
}