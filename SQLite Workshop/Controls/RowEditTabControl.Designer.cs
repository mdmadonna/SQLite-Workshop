namespace SQLiteWorkshop
{
    partial class RecordEditTabControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordEditTabControl));
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblTable = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextWhere = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelBody = new System.Windows.Forms.Panel();
            this.panelSeparator = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripTextBoxCurrentItem = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabelTotalRecords = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButtonMoveFirst = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMovePrevious = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveLast = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInsert = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCancel = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCommit = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.Window;
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Controls.Add(this.lblTable);
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.richTextWhere);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(837, 63);
            this.panelTop.TabIndex = 1;
            // 
            // lblTable
            // 
            this.lblTable.AutoSize = true;
            this.lblTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTable.ForeColor = System.Drawing.Color.DimGray;
            this.lblTable.Location = new System.Drawing.Point(28, 36);
            this.lblTable.Name = "lblTable";
            this.lblTable.Size = new System.Drawing.Size(41, 15);
            this.lblTable.TabIndex = 3;
            this.lblTable.Text = "label3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(25, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Row Editor";
            // 
            // richTextWhere
            // 
            this.richTextWhere.Location = new System.Drawing.Point(250, 17);
            this.richTextWhere.Multiline = false;
            this.richTextWhere.Name = "richTextWhere";
            this.richTextWhere.Size = new System.Drawing.Size(480, 20);
            this.richTextWhere.TabIndex = 1;
            this.richTextWhere.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Where:";
            // 
            // panelBody
            // 
            this.panelBody.AutoScroll = true;
            this.panelBody.BackColor = System.Drawing.SystemColors.Window;
            this.panelBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBody.Location = new System.Drawing.Point(0, 63);
            this.panelBody.Name = "panelBody";
            this.panelBody.Size = new System.Drawing.Size(837, 413);
            this.panelBody.TabIndex = 0;
            // 
            // panelSeparator
            // 
            this.panelSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.panelSeparator.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSeparator.Location = new System.Drawing.Point(0, 63);
            this.panelSeparator.Name = "panelSeparator";
            this.panelSeparator.Size = new System.Drawing.Size(837, 2);
            this.panelSeparator.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonMoveFirst,
            this.toolStripButtonMovePrevious,
            this.toolStripTextBoxCurrentItem,
            this.toolStripLabelTotalRecords,
            this.toolStripButtonMoveNext,
            this.toolStripButtonMoveLast,
            this.toolStripSeparator2,
            this.toolStripButtonInsert,
            this.toolStripButtonDelete,
            this.toolStripSeparator3,
            this.toolStripButtonCancel,
            this.toolStripButtonCommit,
            this.toolStripSeparator4,
            this.toolStripLabelStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 451);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.ShowItemToolTips = false;
            this.toolStrip1.Size = new System.Drawing.Size(837, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripTextBoxCurrentItem
            // 
            this.toolStripTextBoxCurrentItem.Name = "toolStripTextBoxCurrentItem";
            this.toolStripTextBoxCurrentItem.Size = new System.Drawing.Size(50, 25);
            this.toolStripTextBoxCurrentItem.ToolTipText = "Current record.";
            this.toolStripTextBoxCurrentItem.Leave += new System.EventHandler(this.toolStripTextBoxCurrentItem_Leave);
            this.toolStripTextBoxCurrentItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBoxCurrentItem_KeyDown);
            // 
            // toolStripLabelTotalRecords
            // 
            this.toolStripLabelTotalRecords.Name = "toolStripLabelTotalRecords";
            this.toolStripLabelTotalRecords.Size = new System.Drawing.Size(80, 22);
            this.toolStripLabelTotalRecords.Text = "toolStripLabel";
            this.toolStripLabelTotalRecords.ToolTipText = "Total records in the table.";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabelStatus
            // 
            this.toolStripLabelStatus.Name = "toolStripLabelStatus";
            this.toolStripLabelStatus.Size = new System.Drawing.Size(111, 22);
            this.toolStripLabelStatus.Text = "toolStripLabelstatus";
            // 
            // toolStripButtonMoveFirst
            // 
            this.toolStripButtonMoveFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveFirst.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveFirst.Image")));
            this.toolStripButtonMoveFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveFirst.Name = "toolStripButtonMoveFirst";
            this.toolStripButtonMoveFirst.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMoveFirst.Text = "toolStripButtonMoveFirst";
            this.toolStripButtonMoveFirst.ToolTipText = "Move to first row in the table.";
            this.toolStripButtonMoveFirst.Click += new System.EventHandler(this.toolStripButtonMoveFirst_Click);
            // 
            // toolStripButtonMovePrevious
            // 
            this.toolStripButtonMovePrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMovePrevious.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMovePrevious.Image")));
            this.toolStripButtonMovePrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMovePrevious.Name = "toolStripButtonMovePrevious";
            this.toolStripButtonMovePrevious.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMovePrevious.Text = "toolStripButtonMovePrevious";
            this.toolStripButtonMovePrevious.ToolTipText = "Move to the previous row in the table.";
            this.toolStripButtonMovePrevious.Click += new System.EventHandler(this.toolStripButtonMovePrevious_Click);
            // 
            // toolStripButtonMoveNext
            // 
            this.toolStripButtonMoveNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveNext.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveNext.Image")));
            this.toolStripButtonMoveNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveNext.Name = "toolStripButtonMoveNext";
            this.toolStripButtonMoveNext.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMoveNext.Text = "toolStripButtonMove Next";
            this.toolStripButtonMoveNext.ToolTipText = "Move to the next row in the table.";
            this.toolStripButtonMoveNext.Click += new System.EventHandler(this.toolStripButtonMoveNext_Click);
            // 
            // toolStripButtonMoveLast
            // 
            this.toolStripButtonMoveLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveLast.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveLast.Image")));
            this.toolStripButtonMoveLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveLast.Name = "toolStripButtonMoveLast";
            this.toolStripButtonMoveLast.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMoveLast.Text = "toolStripButtonMoveLast";
            this.toolStripButtonMoveLast.ToolTipText = "Move to the last row in the table.";
            this.toolStripButtonMoveLast.Click += new System.EventHandler(this.toolStripButtonMoveLast_Click);
            // 
            // toolStripButtonInsert
            // 
            this.toolStripButtonInsert.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonInsert.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonInsert.Image")));
            this.toolStripButtonInsert.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInsert.Name = "toolStripButtonInsert";
            this.toolStripButtonInsert.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonInsert.Text = "toolStripButtonInsert";
            this.toolStripButtonInsert.ToolTipText = "Insert a new row.";
            this.toolStripButtonInsert.Click += new System.EventHandler(this.toolStripButtonInsert_Click);
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDelete.Image")));
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelete.Text = "toolStripButtonDelete";
            this.toolStripButtonDelete.ToolTipText = "Delete the current row.";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripButtonCancel
            // 
            this.toolStripButtonCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCancel.Image = global::SQLiteWorkshop.Properties.Resources.Undo_small;
            this.toolStripButtonCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancel.Name = "toolStripButtonCancel";
            this.toolStripButtonCancel.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCancel.Text = "toolStripButtonCancel";
            this.toolStripButtonCancel.ToolTipText = "Cancel the current change";
            this.toolStripButtonCancel.Click += new System.EventHandler(this.toolStripButtonCancel_Click);
            // 
            // toolStripButtonCommit
            // 
            this.toolStripButtonCommit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCommit.Image = global::SQLiteWorkshop.Properties.Resources.Save;
            this.toolStripButtonCommit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCommit.Name = "toolStripButtonCommit";
            this.toolStripButtonCommit.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCommit.Text = "toolStripButtonCommit";
            this.toolStripButtonCommit.ToolTipText = "Commit the current changes.";
            this.toolStripButtonCommit.Click += new System.EventHandler(this.toolStripButtonCommit_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::SQLiteWorkshop.Properties.Resources.Reload_small;
            this.btnRefresh.Location = new System.Drawing.Point(736, 14);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(30, 24);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // RecordEditTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panelSeparator);
            this.Controls.Add(this.panelBody);
            this.Controls.Add(this.panelTop);
            this.Name = "RecordEditTabControl";
            this.Size = new System.Drawing.Size(837, 476);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBody;
        private System.Windows.Forms.RichTextBox richTextWhere;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelSeparator;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveFirst;
        private System.Windows.Forms.ToolStripButton toolStripButtonMovePrevious;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxCurrentItem;
        private System.Windows.Forms.ToolStripLabel toolStripLabelTotalRecords;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveNext;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveLast;
        private System.Windows.Forms.ToolStripButton toolStripButtonInsert;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancel;
        private System.Windows.Forms.ToolStripButton toolStripButtonCommit;
        private System.Windows.Forms.ToolStripLabel toolStripLabelStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Button btnRefresh;
    }
}
