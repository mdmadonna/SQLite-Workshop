namespace SQLiteWorkshop
{
    partial class Preview

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preview));
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBody = new System.Windows.Forms.Panel();
            this.dgPreview = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTable = new System.Windows.Forms.Label();
            this.spLeft = new System.Windows.Forms.Splitter();
            this.spTop = new System.Windows.Forms.Splitter();
            this.spRight = new System.Windows.Forms.Splitter();
            this.spBottom = new System.Windows.Forms.Splitter();
            this.panelBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPreview)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 2);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(839, 28);
            this.panelTop.TabIndex = 1;
            // 
            // panelBody
            // 
            this.panelBody.AutoScroll = true;
            this.panelBody.AutoSize = true;
            this.panelBody.Controls.Add(this.dgPreview);
            this.panelBody.Controls.Add(this.panel1);
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.Location = new System.Drawing.Point(1, 30);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(839, 313);
            this.panelBody.TabIndex = 4;
            // 
            // dgPreview
            // 
            this.dgPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgPreview.Location = new System.Drawing.Point(0, 31);
            this.dgPreview.Name = "dgPreview";
            this.dgPreview.ReadOnly = true;
            this.dgPreview.Size = new System.Drawing.Size(839, 282);
            this.dgPreview.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.lblTable);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(839, 31);
            this.panel1.TabIndex = 1;
            // 
            // lblTable
            // 
            this.lblTable.AutoSize = true;
            this.lblTable.Location = new System.Drawing.Point(21, 7);
            this.lblTable.Name = "lblTable";
            this.lblTable.Size = new System.Drawing.Size(35, 13);
            this.lblTable.TabIndex = 0;
            this.lblTable.Text = "label1";
            // 
            // spLeft
            // 
            this.spLeft.Location = new System.Drawing.Point(1, 30);
            this.spLeft.Name = "spLeft";
            this.spLeft.Size = new System.Drawing.Size(1, 313);
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
            this.spTop.Size = new System.Drawing.Size(839, 1);
            this.spTop.TabIndex = 10;
            this.spTop.TabStop = false;
            this.spTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sp_MouseDown);
            this.spTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spMouseMove);
            this.spTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sp_MouseUp);
            // 
            // spRight
            // 
            this.spRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.spRight.Location = new System.Drawing.Point(839, 30);
            this.spRight.Name = "spRight";
            this.spRight.Size = new System.Drawing.Size(1, 313);
            this.spRight.TabIndex = 11;
            this.spRight.TabStop = false;
            this.spRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sp_MouseDown);
            this.spRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spMouseMove);
            this.spRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sp_MouseUp);
            // 
            // spBottom
            // 
            this.spBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.spBottom.Location = new System.Drawing.Point(2, 342);
            this.spBottom.Name = "spBottom";
            this.spBottom.Size = new System.Drawing.Size(837, 1);
            this.spBottom.TabIndex = 12;
            this.spBottom.TabStop = false;
            this.spBottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.sp_MouseDown);
            this.spBottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spMouseMove);
            this.spBottom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sp_MouseUp);
            // 
            // Preview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(841, 344);
            this.Controls.Add(this.spBottom);
            this.Controls.Add(this.spRight);
            this.Controls.Add(this.spLeft);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.spTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Preview";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Preview";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Preview_FormClosed);
            this.Load += new System.EventHandler(this.Preview_Load);
            this.panelBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgPreview)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.Splitter spLeft;
        private System.Windows.Forms.Splitter spTop;
        private System.Windows.Forms.Splitter spRight;
        private System.Windows.Forms.Splitter spBottom;
        private System.Windows.Forms.DataGridView dgPreview;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblTable;
    }
}