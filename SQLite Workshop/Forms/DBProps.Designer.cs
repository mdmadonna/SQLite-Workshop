namespace SQLiteWorkshop
{
    partial class DBProps
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DBProps));
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelFill = new System.Windows.Forms.Panel();
            this.tabProperties = new System.Windows.Forms.TabControl();
            this.tabPageDB = new System.Windows.Forms.TabPage();
            this.propertyGridDBProperties = new System.Windows.Forms.PropertyGrid();
            this.tabPageRuntime = new System.Windows.Forms.TabPage();
            this.propertyGridDBRuntime = new System.Windows.Forms.PropertyGrid();
            this.panelBottom.SuspendLayout();
            this.panelFill.SuspendLayout();
            this.tabProperties.SuspendLayout();
            this.tabPageDB.SuspendLayout();
            this.tabPageRuntime.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(811, 28);
            this.panelTop.TabIndex = 0;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelBottom.Controls.Add(this.btnRefresh);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 426);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(811, 57);
            this.panelBottom.TabIndex = 1;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(595, 18);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(692, 18);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelFill
            // 
            this.panelFill.BackColor = System.Drawing.SystemColors.Control;
            this.panelFill.Controls.Add(this.tabProperties);
            this.panelFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFill.Location = new System.Drawing.Point(1, 29);
            this.panelFill.Name = "panelFill";
            this.panelFill.Size = new System.Drawing.Size(811, 397);
            this.panelFill.TabIndex = 2;
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.tabPageDB);
            this.tabProperties.Controls.Add(this.tabPageRuntime);
            this.tabProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabProperties.Location = new System.Drawing.Point(0, 0);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.SelectedIndex = 0;
            this.tabProperties.Size = new System.Drawing.Size(811, 397);
            this.tabProperties.TabIndex = 2;
            // 
            // tabPageDB
            // 
            this.tabPageDB.Controls.Add(this.propertyGridDBProperties);
            this.tabPageDB.Location = new System.Drawing.Point(4, 22);
            this.tabPageDB.Name = "tabPageDB";
            this.tabPageDB.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDB.Size = new System.Drawing.Size(803, 371);
            this.tabPageDB.TabIndex = 0;
            this.tabPageDB.Text = "DB";
            this.tabPageDB.UseVisualStyleBackColor = true;
            // 
            // propertyGridDBProperties
            // 
            this.propertyGridDBProperties.DisabledItemForeColor = System.Drawing.SystemColors.ControlText;
            this.propertyGridDBProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridDBProperties.Location = new System.Drawing.Point(3, 3);
            this.propertyGridDBProperties.Name = "propertyGridDBProperties";
            this.propertyGridDBProperties.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridDBProperties.Size = new System.Drawing.Size(797, 365);
            this.propertyGridDBProperties.TabIndex = 0;
            // 
            // tabPageRuntime
            // 
            this.tabPageRuntime.Controls.Add(this.propertyGridDBRuntime);
            this.tabPageRuntime.Location = new System.Drawing.Point(4, 22);
            this.tabPageRuntime.Name = "tabPageRuntime";
            this.tabPageRuntime.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRuntime.Size = new System.Drawing.Size(803, 371);
            this.tabPageRuntime.TabIndex = 1;
            this.tabPageRuntime.Text = "Runtime";
            this.tabPageRuntime.UseVisualStyleBackColor = true;
            // 
            // propertyGridDBRuntime
            // 
            this.propertyGridDBRuntime.DisabledItemForeColor = System.Drawing.SystemColors.ControlText;
            this.propertyGridDBRuntime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridDBRuntime.Location = new System.Drawing.Point(3, 3);
            this.propertyGridDBRuntime.Name = "propertyGridDBRuntime";
            this.propertyGridDBRuntime.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridDBRuntime.Size = new System.Drawing.Size(797, 365);
            this.propertyGridDBRuntime.TabIndex = 0;
            // 
            // DBProps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(813, 484);
            this.Controls.Add(this.panelFill);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DBProps";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Properties";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DBProps_FormClosed);
            this.Load += new System.EventHandler(this.DBProps_Load);
            this.panelBottom.ResumeLayout(false);
            this.panelFill.ResumeLayout(false);
            this.tabProperties.ResumeLayout(false);
            this.tabPageDB.ResumeLayout(false);
            this.tabPageRuntime.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panelFill;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.TabControl tabProperties;
        private System.Windows.Forms.TabPage tabPageDB;
        private System.Windows.Forms.PropertyGrid propertyGridDBProperties;
        private System.Windows.Forms.TabPage tabPageRuntime;
        private System.Windows.Forms.PropertyGrid propertyGridDBRuntime;
    }
}