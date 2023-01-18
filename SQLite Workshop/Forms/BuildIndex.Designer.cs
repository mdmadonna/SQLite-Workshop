namespace SQLiteWorkshop
{
    partial class BuildIndex
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildIndex));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelResult = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnShowSQL = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBoxTableName = new System.Windows.Forms.ComboBox();
            this.txtWhereClause = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ChkUnique = new System.Windows.Forms.CheckBox();
            this.dgvIndexColumns = new System.Windows.Forms.DataGridView();
            this.IdxColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColOrder = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvColumns = new System.Windows.Forms.DataGridView();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTableName = new System.Windows.Forms.Label();
            this.txtIndexName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIndexColumns)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumns)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelResult});
            this.statusStrip1.Location = new System.Drawing.Point(1, 522);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelResult
            // 
            this.toolStripStatusLabelResult.Name = "toolStripStatusLabelResult";
            this.toolStripStatusLabelResult.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabelResult.Text = "toolStripStatusLabel1";
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(1, 1);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(792, 28);
            this.panelTop.TabIndex = 1;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.panelBottom.Controls.Add(this.btnShowSQL);
            this.panelBottom.Controls.Add(this.btnCreate);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 483);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(792, 39);
            this.panelBottom.TabIndex = 2;
            // 
            // btnShowSQL
            // 
            this.btnShowSQL.Location = new System.Drawing.Point(578, 6);
            this.btnShowSQL.Name = "btnShowSQL";
            this.btnShowSQL.Size = new System.Drawing.Size(96, 23);
            this.btnShowSQL.TabIndex = 4;
            this.btnShowSQL.Text = "Show SQL";
            this.btnShowSQL.UseVisualStyleBackColor = true;
            this.btnShowSQL.Click += new System.EventHandler(this.btnShowSQL_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(485, 6);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(692, 6);
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
            this.panel1.Controls.Add(this.comboBoxTableName);
            this.panel1.Controls.Add(this.txtWhereClause);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.ChkUnique);
            this.panel1.Controls.Add(this.dgvIndexColumns);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lblTableName);
            this.panel1.Controls.Add(this.txtIndexName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(1, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 454);
            this.panel1.TabIndex = 3;
            // 
            // comboBoxTableName
            // 
            this.comboBoxTableName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTableName.FormattingEnabled = true;
            this.comboBoxTableName.Location = new System.Drawing.Point(109, 14);
            this.comboBoxTableName.Name = "comboBoxTableName";
            this.comboBoxTableName.Size = new System.Drawing.Size(166, 21);
            this.comboBoxTableName.TabIndex = 13;
            this.comboBoxTableName.SelectedIndexChanged += new System.EventHandler(this.comboBoxTableName_SelectedIndexChanged);
            // 
            // txtWhereClause
            // 
            this.txtWhereClause.Location = new System.Drawing.Point(82, 409);
            this.txtWhereClause.Name = "txtWhereClause";
            this.txtWhereClause.Size = new System.Drawing.Size(685, 20);
            this.txtWhereClause.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 412);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Where:";
            // 
            // ChkUnique
            // 
            this.ChkUnique.AutoSize = true;
            this.ChkUnique.Location = new System.Drawing.Point(41, 86);
            this.ChkUnique.Name = "ChkUnique";
            this.ChkUnique.Size = new System.Drawing.Size(60, 17);
            this.ChkUnique.TabIndex = 10;
            this.ChkUnique.Text = "Unique";
            this.ChkUnique.UseVisualStyleBackColor = true;
            // 
            // dgvIndexColumns
            // 
            this.dgvIndexColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIndexColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IdxColName,
            this.ColOrder});
            this.dgvIndexColumns.Location = new System.Drawing.Point(41, 121);
            this.dgvIndexColumns.MultiSelect = false;
            this.dgvIndexColumns.Name = "dgvIndexColumns";
            this.dgvIndexColumns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIndexColumns.Size = new System.Drawing.Size(266, 266);
            this.dgvIndexColumns.TabIndex = 9;
            this.dgvIndexColumns.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvIndexColumns_MouseDown);
            // 
            // IdxColName
            // 
            this.IdxColName.HeaderText = "Column Name";
            this.IdxColName.Name = "IdxColName";
            this.IdxColName.ReadOnly = true;
            this.IdxColName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColOrder
            // 
            this.ColOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ColOrder.HeaderText = "Sort Order";
            this.ColOrder.Items.AddRange(new object[] {
            "Ascending",
            "Descending"});
            this.ColOrder.Name = "ColOrder";
            this.ColOrder.ReadOnly = true;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.dgvColumns);
            this.panel2.Location = new System.Drawing.Point(335, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(432, 381);
            this.panel2.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(217, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Double Click Columns to include in the Index";
            // 
            // dgvColumns
            // 
            this.dgvColumns.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvColumns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnName,
            this.ColumnType});
            this.dgvColumns.Location = new System.Drawing.Point(19, 44);
            this.dgvColumns.MultiSelect = false;
            this.dgvColumns.Name = "dgvColumns";
            this.dgvColumns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvColumns.Size = new System.Drawing.Size(394, 305);
            this.dgvColumns.TabIndex = 6;
            this.dgvColumns.DoubleClick += new System.EventHandler(this.dgvColumns_DoubleClick);
            // 
            // ColumnName
            // 
            this.ColumnName.HeaderText = "Column";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            this.ColumnName.Width = 180;
            // 
            // ColumnType
            // 
            this.ColumnType.HeaderText = "Data Type";
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.ReadOnly = true;
            this.ColumnType.Width = 150;
            // 
            // lblTableName
            // 
            this.lblTableName.AutoSize = true;
            this.lblTableName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTableName.ForeColor = System.Drawing.Color.DimGray;
            this.lblTableName.Location = new System.Drawing.Point(15, 15);
            this.lblTableName.Name = "lblTableName";
            this.lblTableName.Size = new System.Drawing.Size(87, 16);
            this.lblTableName.TabIndex = 5;
            this.lblTableName.Text = "Table Name:";
            // 
            // txtIndexName
            // 
            this.txtIndexName.Location = new System.Drawing.Point(109, 46);
            this.txtIndexName.Name = "txtIndexName";
            this.txtIndexName.Size = new System.Drawing.Size(198, 20);
            this.txtIndexName.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Index Name:";
            // 
            // BuildIndex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(794, 545);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BuildIndex";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "BuildIndex";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BuildIndex_FormClosed);
            this.Load += new System.EventHandler(this.BuildIndex_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIndexColumns)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColumns)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTableName;
        private System.Windows.Forms.DataGridView dgvColumns;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelResult;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.TextBox txtWhereClause;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ChkUnique;
        private System.Windows.Forms.DataGridView dgvIndexColumns;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn IdxColName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColOrder;
        private System.Windows.Forms.Button btnShowSQL;
        private System.Windows.Forms.ComboBox comboBoxTableName;
        private System.Windows.Forms.TextBox txtIndexName;
    }
}