namespace SQLiteWorkshop
{
    partial class Mapping
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
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnEditSql = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.topPanel = new System.Windows.Forms.Panel();
            this.radioAppendRows = new System.Windows.Forms.RadioButton();
            this.radioDeleteRows = new System.Windows.Forms.RadioButton();
            this.radioCreateDest = new System.Windows.Forms.RadioButton();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.dataGridViewCols = new System.Windows.Forms.DataGridView();
            this.ColumnSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDestination = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnNull = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnPrecision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnScale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelBottom.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCols)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip1.Location = new System.Drawing.Point(1, 413);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(649, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
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
            this.panelBottom.Controls.Add(this.btnEditSql);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 368);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(649, 45);
            this.panelBottom.TabIndex = 2;
            // 
            // btnEditSql
            // 
            this.btnEditSql.Location = new System.Drawing.Point(464, 13);
            this.btnEditSql.Name = "btnEditSql";
            this.btnEditSql.Size = new System.Drawing.Size(75, 23);
            this.btnEditSql.TabIndex = 1;
            this.btnEditSql.Text = "Edit SQL";
            this.btnEditSql.UseVisualStyleBackColor = true;
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
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.SystemColors.Window;
            this.topPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.topPanel.Controls.Add(this.radioAppendRows);
            this.topPanel.Controls.Add(this.radioDeleteRows);
            this.topPanel.Controls.Add(this.radioCreateDest);
            this.topPanel.Controls.Add(this.lblTarget);
            this.topPanel.Controls.Add(this.lblSource);
            this.topPanel.Controls.Add(this.label2);
            this.topPanel.Controls.Add(this.label1);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(1, 29);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(649, 100);
            this.topPanel.TabIndex = 3;
            // 
            // radioAppendRows
            // 
            this.radioAppendRows.AutoSize = true;
            this.radioAppendRows.Location = new System.Drawing.Point(388, 76);
            this.radioAppendRows.Name = "radioAppendRows";
            this.radioAppendRows.Size = new System.Drawing.Size(87, 17);
            this.radioAppendRows.TabIndex = 6;
            this.radioAppendRows.TabStop = true;
            this.radioAppendRows.Text = "Append rows";
            this.radioAppendRows.UseVisualStyleBackColor = true;
            // 
            // radioDeleteRows
            // 
            this.radioDeleteRows.AutoSize = true;
            this.radioDeleteRows.Location = new System.Drawing.Point(388, 52);
            this.radioDeleteRows.Name = "radioDeleteRows";
            this.radioDeleteRows.Size = new System.Drawing.Size(145, 17);
            this.radioDeleteRows.TabIndex = 5;
            this.radioDeleteRows.TabStop = true;
            this.radioDeleteRows.Text = "Delete rows before import";
            this.radioDeleteRows.UseVisualStyleBackColor = true;
            // 
            // radioCreateDest
            // 
            this.radioCreateDest.AutoSize = true;
            this.radioCreateDest.Location = new System.Drawing.Point(388, 28);
            this.radioCreateDest.Name = "radioCreateDest";
            this.radioCreateDest.Size = new System.Drawing.Size(142, 17);
            this.radioCreateDest.TabIndex = 4;
            this.radioCreateDest.TabStop = true;
            this.radioCreateDest.Text = "Create Destination Table";
            this.radioCreateDest.UseVisualStyleBackColor = true;
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(137, 54);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(48, 13);
            this.lblTarget.TabIndex = 3;
            this.lblTarget.Text = "lblTarget";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(137, 28);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(51, 13);
            this.lblSource.TabIndex = 2;
            this.lblSource.Text = "lblSource";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Destination Table:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Table:";
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.SystemColors.Window;
            this.mainPanel.Controls.Add(this.dataGridViewCols);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(1, 129);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(649, 239);
            this.mainPanel.TabIndex = 4;
            // 
            // dataGridViewCols
            // 
            this.dataGridViewCols.AllowUserToAddRows = false;
            this.dataGridViewCols.AllowUserToDeleteRows = false;
            this.dataGridViewCols.AllowUserToResizeRows = false;
            this.dataGridViewCols.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewCols.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnSource,
            this.ColumnDestination,
            this.ColumnType,
            this.ColumnSize,
            this.ColumnNull,
            this.ColumnPrecision,
            this.ColumnScale});
            this.dataGridViewCols.Location = new System.Drawing.Point(3, 6);
            this.dataGridViewCols.MultiSelect = false;
            this.dataGridViewCols.Name = "dataGridViewCols";
            this.dataGridViewCols.RowHeadersVisible = false;
            this.dataGridViewCols.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewCols.Size = new System.Drawing.Size(643, 227);
            this.dataGridViewCols.TabIndex = 0;
            this.dataGridViewCols.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewCols_CellValueChanged);
            // 
            // ColumnSource
            // 
            this.ColumnSource.HeaderText = "Source";
            this.ColumnSource.Name = "ColumnSource";
            this.ColumnSource.ReadOnly = true;
            this.ColumnSource.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnSource.Width = 150;
            // 
            // ColumnDestination
            // 
            this.ColumnDestination.HeaderText = "Destination";
            this.ColumnDestination.Name = "ColumnDestination";
            this.ColumnDestination.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnDestination.Width = 180;
            // 
            // ColumnType
            // 
            this.ColumnType.HeaderText = "Type";
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.ReadOnly = true;
            this.ColumnType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnType.Width = 80;
            // 
            // ColumnSize
            // 
            this.ColumnSize.HeaderText = "Size";
            this.ColumnSize.Name = "ColumnSize";
            this.ColumnSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnSize.Width = 60;
            // 
            // ColumnNull
            // 
            this.ColumnNull.HeaderText = "Null";
            this.ColumnNull.Name = "ColumnNull";
            this.ColumnNull.Width = 35;
            // 
            // ColumnPrecision
            // 
            this.ColumnPrecision.HeaderText = "Precision";
            this.ColumnPrecision.Name = "ColumnPrecision";
            this.ColumnPrecision.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnPrecision.Width = 60;
            // 
            // ColumnScale
            // 
            this.ColumnScale.HeaderText = "Scale";
            this.ColumnScale.Name = "ColumnScale";
            this.ColumnScale.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnScale.Width = 60;
            // 
            // Mapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(651, 436);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Mapping";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Template";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Mapping_FormClosed);
            this.Load += new System.EventHandler(this.Mapping_Load);
            this.panelBottom.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewCols)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnEditSql;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.RadioButton radioAppendRows;
        private System.Windows.Forms.RadioButton radioDeleteRows;
        private System.Windows.Forms.RadioButton radioCreateDest;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.DataGridView dataGridViewCols;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSource;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnDestination;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSize;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnNull;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnPrecision;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnScale;
    }
}