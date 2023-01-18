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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panelBody = new System.Windows.Forms.Panel();
            this.richTextBoxData = new System.Windows.Forms.RichTextBox();
            this.spLeft = new System.Windows.Forms.Splitter();
            this.spTop = new System.Windows.Forms.Splitter();
            this.spRight = new System.Windows.Forms.Splitter();
            this.spBottom = new System.Windows.Forms.Splitter();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelBody.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 2);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(350, 28);
            this.panelTop.TabIndex = 1;
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
            this.Name = "ShowImg";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Show Image";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ShowImg_FormClosed);
            this.Load += new System.EventHandler(this.ShowImg_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelBody.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.RichTextBox richTextBoxData;
        private System.Windows.Forms.Splitter spLeft;
        private System.Windows.Forms.Splitter spTop;
        private System.Windows.Forms.Splitter spRight;
        private System.Windows.Forms.Splitter spBottom;
    }
}