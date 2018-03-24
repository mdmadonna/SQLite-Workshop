namespace SQLiteWorkshop
{
    partial class BuildColumn
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildColumn));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblFormHeading = new System.Windows.Forms.Label();
            this.panelControls = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelPropertyGrid = new System.Windows.Forms.Panel();
            this.lblPanelHeading = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTableName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtColumn = new System.Windows.Forms.TextBox();
            this.lblNewColumnName = new System.Windows.Forms.Label();
            this.txtNewColumn = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.propertyGridColumn = new System.Windows.Forms.PropertyGrid();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelPropertyGrid.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(1, 299);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(639, 22);
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
            this.panelTop.Size = new System.Drawing.Size(639, 28);
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
            // panelControls
            // 
            this.panelControls.Controls.Add(this.pbClose);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelControls.Location = new System.Drawing.Point(516, 0);
            this.panelControls.Name = "panelControls";
            this.panelControls.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.panelControls.Size = new System.Drawing.Size(123, 28);
            this.panelControls.TabIndex = 3;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelBottom.Controls.Add(this.btnSave);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 254);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(639, 45);
            this.panelBottom.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(543, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelPropertyGrid
            // 
            this.panelPropertyGrid.BackColor = System.Drawing.SystemColors.Window;
            this.panelPropertyGrid.Controls.Add(this.propertyGridColumn);
            this.panelPropertyGrid.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelPropertyGrid.Location = new System.Drawing.Point(216, 29);
            this.panelPropertyGrid.Name = "panelPropertyGrid";
            this.panelPropertyGrid.Size = new System.Drawing.Size(424, 225);
            this.panelPropertyGrid.TabIndex = 3;
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(19, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Table:";
            // 
            // lblTableName
            // 
            this.lblTableName.AutoSize = true;
            this.lblTableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTableName.ForeColor = System.Drawing.Color.DimGray;
            this.lblTableName.Location = new System.Drawing.Point(68, 50);
            this.lblTableName.Name = "lblTableName";
            this.lblTableName.Size = new System.Drawing.Size(84, 13);
            this.lblTableName.TabIndex = 6;
            this.lblTableName.Text = "lblTableName";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Column Name:";
            // 
            // txtColumn
            // 
            this.txtColumn.Location = new System.Drawing.Point(22, 111);
            this.txtColumn.Name = "txtColumn";
            this.txtColumn.Size = new System.Drawing.Size(156, 20);
            this.txtColumn.TabIndex = 8;
            // 
            // lblNewColumnName
            // 
            this.lblNewColumnName.AutoSize = true;
            this.lblNewColumnName.Location = new System.Drawing.Point(19, 144);
            this.lblNewColumnName.Name = "lblNewColumnName";
            this.lblNewColumnName.Size = new System.Drawing.Size(101, 13);
            this.lblNewColumnName.TabIndex = 9;
            this.lblNewColumnName.Text = "New Column Name:";
            // 
            // txtNewColumn
            // 
            this.txtNewColumn.Location = new System.Drawing.Point(22, 160);
            this.txtNewColumn.Name = "txtNewColumn";
            this.txtNewColumn.Size = new System.Drawing.Size(156, 20);
            this.txtNewColumn.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.Controls.Add(this.txtNewColumn);
            this.panel2.Controls.Add(this.lblPanelHeading);
            this.panel2.Controls.Add(this.lblNewColumnName);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtColumn);
            this.panel2.Controls.Add(this.lblTableName);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(215, 225);
            this.panel2.TabIndex = 11;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(440, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pbIcon
            // 
            this.pbIcon.Image = ((System.Drawing.Image)(resources.GetObject("pbIcon.Image")));
            this.pbIcon.Location = new System.Drawing.Point(3, 3);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(24, 24);
            this.pbIcon.TabIndex = 4;
            this.pbIcon.TabStop = false;
            // 
            // pbClose
            // 
            this.pbClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.pbClose.Image = ((System.Drawing.Image)(resources.GetObject("pbClose.Image")));
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
            // propertyGridColumn
            // 
            this.propertyGridColumn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridColumn.Location = new System.Drawing.Point(0, 0);
            this.propertyGridColumn.Name = "propertyGridColumn";
            this.propertyGridColumn.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGridColumn.Size = new System.Drawing.Size(424, 225);
            this.propertyGridColumn.TabIndex = 0;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // BuildColumn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(641, 322);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelPropertyGrid);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BuildColumn";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Template";
            this.Load += new System.EventHandler(this.BuildColumn_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelControls.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelPropertyGrid.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
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
        private System.Windows.Forms.Panel panelPropertyGrid;
        private System.Windows.Forms.Label lblPanelHeading;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTableName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtColumn;
        private System.Windows.Forms.Label lblNewColumnName;
        private System.Windows.Forms.TextBox txtNewColumn;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PropertyGrid propertyGridColumn;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}