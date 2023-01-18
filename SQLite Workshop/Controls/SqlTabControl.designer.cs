namespace SQLiteWorkshop
{
    partial class SqlTabControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.SqlStatusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripResult = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripRowCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripClock = new System.Windows.Forms.ToolStripStatusLabel();
            this.hSplitter = new System.Windows.Forms.Splitter();
            this.tabResults = new System.Windows.Forms.TabControl();
            this.tabResultsPage = new System.Windows.Forms.TabPage();
            this.gvResults = new System.Windows.Forms.DataGridView();
            this.txtSqlResults = new System.Windows.Forms.TextBox();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.richTextErrors = new System.Windows.Forms.RichTextBox();
            this.txtSqlStatement = new System.Windows.Forms.RichTextBox();
            this.SqlStatusStrip.SuspendLayout();
            this.tabResults.SuspendLayout();
            this.tabResultsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvResults)).BeginInit();
            this.tabErrors.SuspendLayout();
            this.SuspendLayout();
            // 
            // SqlStatusStrip
            // 
            this.SqlStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripResult,
            this.toolStripRowCount,
            this.toolStripClock});
            this.SqlStatusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.SqlStatusStrip.Location = new System.Drawing.Point(0, 439);
            this.SqlStatusStrip.Name = "SqlStatusStrip";
            this.SqlStatusStrip.Size = new System.Drawing.Size(652, 22);
            this.SqlStatusStrip.TabIndex = 0;
            this.SqlStatusStrip.Text = "statusStrip1";
            // 
            // toolStripResult
            // 
            this.toolStripResult.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripResult.Name = "toolStripResult";
            this.toolStripResult.Size = new System.Drawing.Size(39, 17);
            this.toolStripResult.Text = "Result";
            this.toolStripResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripResult.ToolTipText = "SQL Execution Results";
            // 
            // toolStripRowCount
            // 
            this.toolStripRowCount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripRowCount.Name = "toolStripRowCount";
            this.toolStripRowCount.Size = new System.Drawing.Size(66, 17);
            this.toolStripRowCount.Text = "Row Count";
            // 
            // toolStripClock
            // 
            this.toolStripClock.Name = "toolStripClock";
            this.toolStripClock.Size = new System.Drawing.Size(37, 17);
            this.toolStripClock.Text = "Clock";
            // 
            // hSplitter
            // 
            this.hSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.hSplitter.Location = new System.Drawing.Point(0, 96);
            this.hSplitter.Name = "hSplitter";
            this.hSplitter.Size = new System.Drawing.Size(652, 3);
            this.hSplitter.TabIndex = 2;
            this.hSplitter.TabStop = false;
            // 
            // tabResults
            // 
            this.tabResults.Controls.Add(this.tabResultsPage);
            this.tabResults.Controls.Add(this.tabErrors);
            this.tabResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabResults.Location = new System.Drawing.Point(0, 99);
            this.tabResults.Name = "tabResults";
            this.tabResults.SelectedIndex = 0;
            this.tabResults.Size = new System.Drawing.Size(652, 340);
            this.tabResults.TabIndex = 3;
            // 
            // tabResultsPage
            // 
            this.tabResultsPage.Controls.Add(this.gvResults);
            this.tabResultsPage.Controls.Add(this.txtSqlResults);
            this.tabResultsPage.Location = new System.Drawing.Point(4, 22);
            this.tabResultsPage.Name = "tabResultsPage";
            this.tabResultsPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabResultsPage.Size = new System.Drawing.Size(644, 314);
            this.tabResultsPage.TabIndex = 0;
            this.tabResultsPage.Text = "Results";
            this.tabResultsPage.UseVisualStyleBackColor = true;
            // 
            // gvResults
            // 
            this.gvResults.AllowUserToAddRows = false;
            this.gvResults.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvResults.EnableHeadersVisualStyles = false;
            this.gvResults.Location = new System.Drawing.Point(3, 3);
            this.gvResults.Name = "gvResults";
            this.gvResults.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.InactiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvResults.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gvResults.RowHeadersWidth = 60;
            this.gvResults.Size = new System.Drawing.Size(638, 308);
            this.gvResults.TabIndex = 1;
            this.gvResults.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.gvResults_RowPostPaint);
            // 
            // txtSqlResults
            // 
            this.txtSqlResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSqlResults.Location = new System.Drawing.Point(3, 3);
            this.txtSqlResults.Multiline = true;
            this.txtSqlResults.Name = "txtSqlResults";
            this.txtSqlResults.Size = new System.Drawing.Size(638, 308);
            this.txtSqlResults.TabIndex = 0;
            this.txtSqlResults.Visible = false;
            // 
            // tabErrors
            // 
            this.tabErrors.Controls.Add(this.richTextErrors);
            this.tabErrors.Location = new System.Drawing.Point(4, 22);
            this.tabErrors.Name = "tabErrors";
            this.tabErrors.Size = new System.Drawing.Size(644, 314);
            this.tabErrors.TabIndex = 1;
            this.tabErrors.Text = "Errors";
            this.tabErrors.UseVisualStyleBackColor = true;
            // 
            // richTextErrors
            // 
            this.richTextErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextErrors.Location = new System.Drawing.Point(0, 0);
            this.richTextErrors.Name = "richTextErrors";
            this.richTextErrors.ReadOnly = true;
            this.richTextErrors.Size = new System.Drawing.Size(644, 314);
            this.richTextErrors.TabIndex = 0;
            this.richTextErrors.Text = "";
            // 
            // txtSqlStatement
            // 
            this.txtSqlStatement.AcceptsTab = true;
            this.txtSqlStatement.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSqlStatement.Location = new System.Drawing.Point(0, 0);
            this.txtSqlStatement.Name = "txtSqlStatement";
            this.txtSqlStatement.Size = new System.Drawing.Size(652, 96);
            this.txtSqlStatement.TabIndex = 4;
            this.txtSqlStatement.Text = "";
            this.txtSqlStatement.TextChanged += new System.EventHandler(this.txtSqlStatement_TextChanged);
            this.txtSqlStatement.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSqlStatement_KeyDown);
            // 
            // SqlTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabResults);
            this.Controls.Add(this.SqlStatusStrip);
            this.Controls.Add(this.hSplitter);
            this.Controls.Add(this.txtSqlStatement);
            this.Name = "SqlTabControl";
            this.Size = new System.Drawing.Size(652, 461);
            this.Leave += new System.EventHandler(this.SqlTabControl_Leave);
            this.SqlStatusStrip.ResumeLayout(false);
            this.SqlStatusStrip.PerformLayout();
            this.tabResults.ResumeLayout(false);
            this.tabResultsPage.ResumeLayout(false);
            this.tabResultsPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvResults)).EndInit();
            this.tabErrors.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip SqlStatusStrip;
        private System.Windows.Forms.Splitter hSplitter;
        private System.Windows.Forms.TabControl tabResults;
        private System.Windows.Forms.TabPage tabResultsPage;
        private System.Windows.Forms.DataGridView gvResults;
        private System.Windows.Forms.ToolStripStatusLabel toolStripResult;
        private System.Windows.Forms.ToolStripStatusLabel toolStripRowCount;
        private System.Windows.Forms.ToolStripStatusLabel toolStripClock;
        private System.Windows.Forms.TextBox txtSqlResults;
        private System.Windows.Forms.RichTextBox txtSqlStatement;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.RichTextBox richTextErrors;
    }
}
