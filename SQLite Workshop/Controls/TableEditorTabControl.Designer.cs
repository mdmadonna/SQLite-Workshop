namespace SQLiteWorkshop
{
    partial class TableEditorTabControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TableEditorTabControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvTableDef = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnShowSQL = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.txtTableName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.propertyGridTable = new System.Windows.Forms.PropertyGrid();
            this.hSplitter = new System.Windows.Forms.Splitter();
            this.panelMid = new System.Windows.Forms.Panel();
            this.ColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColAllowNulls = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColDefault = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrimaryKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColUnique = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColCollation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCheck = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FKey_Table = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FKey_Column = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FKey_OnUpdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FKey_OnDelete = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableDef)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelMid.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvTableDef
            // 
            this.dgvTableDef.AllowUserToResizeRows = false;
            this.dgvTableDef.BackgroundColor = System.Drawing.Color.White;
            this.dgvTableDef.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableDef.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColName,
            this.ColType,
            this.ColAllowNulls,
            this.ColDefault,
            this.colPrimaryKey,
            this.ColUnique,
            this.ColCollation,
            this.colCheck,
            this.FKey_Table,
            this.FKey_Column,
            this.FKey_OnUpdate,
            this.FKey_OnDelete});
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvTableDef.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvTableDef.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTableDef.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dgvTableDef.Location = new System.Drawing.Point(0, 0);
            this.dgvTableDef.MultiSelect = false;
            this.dgvTableDef.Name = "dgvTableDef";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTableDef.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.dgvTableDef.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvTableDef.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTableDef.Size = new System.Drawing.Size(789, 283);
            this.dgvTableDef.TabIndex = 0;
            this.dgvTableDef.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTableDef_CellClick);
            this.dgvTableDef.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTableDef_CellEndEdit);
            this.dgvTableDef.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dgvTableDef_CellValidating);
            this.dgvTableDef.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvTableDef_EditingControlShowing);
            this.dgvTableDef.SelectionChanged += new System.EventHandler(this.dgvTableDef_SelectionChanged);
            this.dgvTableDef.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvTableDef_KeyDown);
            this.dgvTableDef.Leave += new System.EventHandler(this.dgvTableDef_Leave);
            this.dgvTableDef.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvTableDef_MouseDown);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 434);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(789, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.panel1);
            this.panelTop.Controls.Add(this.txtTableName);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(789, 48);
            this.panelTop.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnShowSQL);
            this.panel1.Controls.Add(this.btnCreate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(537, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(252, 48);
            this.panel1.TabIndex = 3;
            // 
            // btnShowSQL
            // 
            this.btnShowSQL.Location = new System.Drawing.Point(144, 11);
            this.btnShowSQL.Name = "btnShowSQL";
            this.btnShowSQL.Size = new System.Drawing.Size(96, 23);
            this.btnShowSQL.TabIndex = 3;
            this.btnShowSQL.Text = "Show SQL";
            this.btnShowSQL.UseVisualStyleBackColor = true;
            this.btnShowSQL.Click += new System.EventHandler(this.btnShowSQL_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(33, 11);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(96, 23);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "Create Table";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // txtTableName
            // 
            this.txtTableName.Location = new System.Drawing.Point(86, 13);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.Size = new System.Drawing.Size(167, 20);
            this.txtTableName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Table Name:";
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.propertyGridTable);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 334);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(789, 100);
            this.panelBottom.TabIndex = 3;
            // 
            // propertyGridTable
            // 
            this.propertyGridTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridTable.Location = new System.Drawing.Point(0, 0);
            this.propertyGridTable.Name = "propertyGridTable";
            this.propertyGridTable.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGridTable.Size = new System.Drawing.Size(789, 100);
            this.propertyGridTable.TabIndex = 1;
            this.propertyGridTable.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridTable_PropertyValueChanged);
            this.propertyGridTable.Leave += new System.EventHandler(this.propertyGridTable_Leave);
            // 
            // hSplitter
            // 
            this.hSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hSplitter.Location = new System.Drawing.Point(0, 331);
            this.hSplitter.Name = "hSplitter";
            this.hSplitter.Size = new System.Drawing.Size(789, 3);
            this.hSplitter.TabIndex = 4;
            this.hSplitter.TabStop = false;
            // 
            // panelMid
            // 
            this.panelMid.Controls.Add(this.dgvTableDef);
            this.panelMid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMid.Location = new System.Drawing.Point(0, 48);
            this.panelMid.Name = "panelMid";
            this.panelMid.Size = new System.Drawing.Size(789, 283);
            this.panelMid.TabIndex = 5;
            // 
            // ColName
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.ColName.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColName.HeaderText = "Column Name";
            this.ColName.Name = "ColName";
            this.ColName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColName.ToolTipText = "Enter the name of the column.";
            // 
            // ColType
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.ColType.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColType.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.ColType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ColType.HeaderText = "Type";
            this.ColType.Items.AddRange(new object[] {
            "bigint",
            "blob",
            "boolean",
            "char(20)",
            "date",
            "datetime",
            "decimal(10,5)",
            "double",
            "int",
            "integer primary key autoincrement",
            "mediumint",
            "nchar(20)",
            "nvarchar(20)",
            "single",
            "smaillint",
            "text",
            "tinyint",
            "unsigned bigint",
            "unsigned int",
            "unsigned smallint",
            "varchar(20)"});
            this.ColType.Name = "ColType";
            this.ColType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColType.Sorted = true;
            this.ColType.ToolTipText = "Select the data type for this column.";
            this.ColType.Width = 140;
            // 
            // ColAllowNulls
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.NullValue = false;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.ColAllowNulls.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColAllowNulls.HeaderText = "Allow Nulls";
            this.ColAllowNulls.Name = "ColAllowNulls";
            this.ColAllowNulls.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColAllowNulls.ToolTipText = "Indicate whether null values are valid for this column.";
            // 
            // ColDefault
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.ColDefault.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColDefault.HeaderText = "Default Value";
            this.ColDefault.Name = "ColDefault";
            this.ColDefault.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColDefault.ToolTipText = "Enter the Default Value for this column.  Leave this blank if there is no default" +
    " value.  Functions should be entered within parentheses.";
            this.ColDefault.Visible = false;
            this.ColDefault.Width = 200;
            // 
            // colPrimaryKey
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.colPrimaryKey.DefaultCellStyle = dataGridViewCellStyle5;
            this.colPrimaryKey.HeaderText = "Primary Key";
            this.colPrimaryKey.Name = "colPrimaryKey";
            this.colPrimaryKey.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPrimaryKey.ToolTipText = resources.GetString("colPrimaryKey.ToolTipText");
            this.colPrimaryKey.Width = 50;
            // 
            // ColUnique
            // 
            this.ColUnique.HeaderText = "Unique";
            this.ColUnique.Name = "ColUnique";
            this.ColUnique.ToolTipText = "Indicate whether this column requires a unique value.";
            this.ColUnique.Width = 50;
            // 
            // ColCollation
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            this.ColCollation.DefaultCellStyle = dataGridViewCellStyle6;
            this.ColCollation.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.ColCollation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ColCollation.HeaderText = "Collation Sequence";
            this.ColCollation.Items.AddRange(new object[] {
            "BINARY",
            "NOCASE",
            "RTRIM"});
            this.ColCollation.Name = "ColCollation";
            this.ColCollation.ToolTipText = "Select the Collating Sequence for this column.";
            this.ColCollation.Visible = false;
            // 
            // colCheck
            // 
            this.colCheck.HeaderText = "Check Constraint";
            this.colCheck.Name = "colCheck";
            this.colCheck.Visible = false;
            // 
            // FKey_Table
            // 
            this.FKey_Table.HeaderText = "Foreign Key Table";
            this.FKey_Table.Name = "FKey_Table";
            this.FKey_Table.Visible = false;
            // 
            // FKey_Column
            // 
            this.FKey_Column.HeaderText = "Foreign Key Column";
            this.FKey_Column.Name = "FKey_Column";
            this.FKey_Column.Visible = false;
            // 
            // FKey_OnUpdate
            // 
            this.FKey_OnUpdate.HeaderText = "Foreign Key OnUpdate";
            this.FKey_OnUpdate.Name = "FKey_OnUpdate";
            this.FKey_OnUpdate.Visible = false;
            // 
            // FKey_OnDelete
            // 
            this.FKey_OnDelete.HeaderText = "Foreign Key OnDelete";
            this.FKey_OnDelete.Name = "FKey_OnDelete";
            this.FKey_OnDelete.Visible = false;
            // 
            // TableEditorTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMid);
            this.Controls.Add(this.hSplitter);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.Name = "TableEditorTabControl";
            this.Size = new System.Drawing.Size(789, 456);
            this.Leave += new System.EventHandler(this.TableEditorTabControl_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableDef)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelMid.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTableDef;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.TextBox txtTableName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Splitter hSplitter;
        private System.Windows.Forms.PropertyGrid propertyGridTable;
        private System.Windows.Forms.Button btnShowSQL;
        private System.Windows.Forms.Panel panelMid;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColAllowNulls;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDefault;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrimaryKey;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColUnique;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColCollation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCheck;
        private System.Windows.Forms.DataGridViewTextBoxColumn FKey_Table;
        private System.Windows.Forms.DataGridViewTextBoxColumn FKey_Column;
        private System.Windows.Forms.DataGridViewTextBoxColumn FKey_OnUpdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn FKey_OnDelete;
    }
}
