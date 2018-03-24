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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExecuteForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblTxtInfo = new System.Windows.Forms.Label();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripExecutionStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripTimerStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblFormHeading = new System.Windows.Forms.Label();
            this.panelControl = new System.Windows.Forms.Panel();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.panelFill = new System.Windows.Forms.Panel();
            this.lblError = new System.Windows.Forms.Label();
            this.statusStrip.SuspendLayout();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.panelFill.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(319, 204);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(218, 204);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 1;
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
            this.txtInfo.TabIndex = 6;
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
            this.panelTop.Controls.Add(this.pictureBox1);
            this.panelTop.Controls.Add(this.lblFormHeading);
            this.panelTop.Controls.Add(this.panelControl);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(409, 28);
            this.panelTop.TabIndex = 8;
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SQLiteWorkshop.Properties.Resources.MainSmall;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(24, 24);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // lblFormHeading
            // 
            this.lblFormHeading.AutoSize = true;
            this.lblFormHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFormHeading.ForeColor = System.Drawing.Color.DimGray;
            this.lblFormHeading.Location = new System.Drawing.Point(29, 7);
            this.lblFormHeading.Name = "lblFormHeading";
            this.lblFormHeading.Size = new System.Drawing.Size(119, 16);
            this.lblFormHeading.TabIndex = 1;
            this.lblFormHeading.Text = "lblFormHeading";
            // 
            // panelControl
            // 
            this.panelControl.Controls.Add(this.pbClose);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControl.Location = new System.Drawing.Point(352, 0);
            this.panelControl.Name = "panelControl";
            this.panelControl.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panelControl.Size = new System.Drawing.Size(57, 28);
            this.panelControl.TabIndex = 0;
            // 
            // pbClose
            // 
            this.pbClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.pbClose.Image = global::SQLiteWorkshop.Properties.Resources.Close;
            this.pbClose.Location = new System.Drawing.Point(23, 0);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(24, 28);
            this.pbClose.TabIndex = 0;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            this.pbClose.MouseEnter += new System.EventHandler(this.ControlBox_MouseEnter);
            this.pbClose.MouseLeave += new System.EventHandler(this.ControlBox_MouseLeave);
            // 
            // panelFill
            // 
            this.panelFill.BackColor = System.Drawing.SystemColors.Window;
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExecuteForm";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "ExecuteForm";
            this.Load += new System.EventHandler(this.ExecuteForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
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
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.Panel panelFill;
        private System.Windows.Forms.Label lblFormHeading;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}