namespace SQLiteWorkshop
{
    partial class Options
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
            this.lblError = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.tabmain = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.lblEncryptKey = new System.Windows.Forms.Label();
            this.chkOpenDB = new System.Windows.Forms.CheckBox();
            this.txtEncryptKey = new System.Windows.Forms.TextBox();
            this.tabPageImport = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.chkIgnoreErrrors = new System.Windows.Forms.CheckBox();
            this.upDownMaxErrors = new System.Windows.Forms.NumericUpDown();
            this.chkSaveImportCredentials = new System.Windows.Forms.CheckBox();
            this.tabPageFunctions = new System.Windows.Forms.TabPage();
            this.dgFunctions = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelBottom.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.tabmain.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageImport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownMaxErrors)).BeginInit();
            this.tabPageFunctions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgFunctions)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.statusStrip1.Location = new System.Drawing.Point(1, 369);
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
            this.panelBottom.Controls.Add(this.lblError);
            this.panelBottom.Controls.Add(this.btnSave);
            this.panelBottom.Controls.Add(this.btnClose);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(1, 324);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(649, 45);
            this.panelBottom.TabIndex = 2;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Location = new System.Drawing.Point(27, 11);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(35, 13);
            this.lblError.TabIndex = 2;
            this.lblError.Text = "label2";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(450, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.SystemColors.Window;
            this.panelMain.Controls.Add(this.tabmain);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(1, 29);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(649, 295);
            this.panelMain.TabIndex = 3;
            // 
            // tabmain
            // 
            this.tabmain.Controls.Add(this.tabPageGeneral);
            this.tabmain.Controls.Add(this.tabPageImport);
            this.tabmain.Controls.Add(this.tabPageFunctions);
            this.tabmain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabmain.Location = new System.Drawing.Point(0, 0);
            this.tabmain.Name = "tabmain";
            this.tabmain.SelectedIndex = 0;
            this.tabmain.Size = new System.Drawing.Size(649, 295);
            this.tabmain.TabIndex = 3;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.lblEncryptKey);
            this.tabPageGeneral.Controls.Add(this.chkOpenDB);
            this.tabPageGeneral.Controls.Add(this.txtEncryptKey);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(641, 269);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // lblEncryptKey
            // 
            this.lblEncryptKey.AutoSize = true;
            this.lblEncryptKey.Location = new System.Drawing.Point(6, 17);
            this.lblEncryptKey.Name = "lblEncryptKey";
            this.lblEncryptKey.Size = new System.Drawing.Size(81, 13);
            this.lblEncryptKey.TabIndex = 0;
            this.lblEncryptKey.Text = "Encryption Key;";
            // 
            // chkOpenDB
            // 
            this.chkOpenDB.AutoSize = true;
            this.chkOpenDB.Location = new System.Drawing.Point(9, 59);
            this.chkOpenDB.Name = "chkOpenDB";
            this.chkOpenDB.Size = new System.Drawing.Size(142, 17);
            this.chkOpenDB.TabIndex = 2;
            this.chkOpenDB.Text = "Open Last DB at Startup";
            this.chkOpenDB.UseVisualStyleBackColor = true;
            // 
            // txtEncryptKey
            // 
            this.txtEncryptKey.Location = new System.Drawing.Point(107, 17);
            this.txtEncryptKey.MaxLength = 32;
            this.txtEncryptKey.Name = "txtEncryptKey";
            this.txtEncryptKey.Size = new System.Drawing.Size(249, 20);
            this.txtEncryptKey.TabIndex = 1;
            // 
            // tabPageImport
            // 
            this.tabPageImport.Controls.Add(this.label2);
            this.tabPageImport.Controls.Add(this.chkIgnoreErrrors);
            this.tabPageImport.Controls.Add(this.upDownMaxErrors);
            this.tabPageImport.Controls.Add(this.chkSaveImportCredentials);
            this.tabPageImport.Location = new System.Drawing.Point(4, 22);
            this.tabPageImport.Name = "tabPageImport";
            this.tabPageImport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImport.Size = new System.Drawing.Size(641, 269);
            this.tabPageImport.TabIndex = 2;
            this.tabPageImport.Text = "Import";
            this.tabPageImport.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(195, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Maximum Allowable Errors During Import";
            // 
            // chkIgnoreErrrors
            // 
            this.chkIgnoreErrrors.AutoSize = true;
            this.chkIgnoreErrrors.Location = new System.Drawing.Point(26, 57);
            this.chkIgnoreErrrors.Name = "chkIgnoreErrrors";
            this.chkIgnoreErrrors.Size = new System.Drawing.Size(152, 17);
            this.chkIgnoreErrrors.TabIndex = 6;
            this.chkIgnoreErrrors.Text = "Ignore Errors During Import";
            this.chkIgnoreErrrors.UseVisualStyleBackColor = true;
            this.chkIgnoreErrrors.CheckedChanged += new System.EventHandler(this.chkIgnoreErrrors_CheckedChanged);
            // 
            // upDownMaxErrors
            // 
            this.upDownMaxErrors.Location = new System.Drawing.Point(26, 89);
            this.upDownMaxErrors.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.upDownMaxErrors.Name = "upDownMaxErrors";
            this.upDownMaxErrors.Size = new System.Drawing.Size(44, 20);
            this.upDownMaxErrors.TabIndex = 5;
            // 
            // chkSaveImportCredentials
            // 
            this.chkSaveImportCredentials.AutoSize = true;
            this.chkSaveImportCredentials.Location = new System.Drawing.Point(26, 26);
            this.chkSaveImportCredentials.Name = "chkSaveImportCredentials";
            this.chkSaveImportCredentials.Size = new System.Drawing.Size(199, 17);
            this.chkSaveImportCredentials.TabIndex = 4;
            this.chkSaveImportCredentials.Text = "Save Imported Database Credentials";
            this.chkSaveImportCredentials.UseVisualStyleBackColor = true;
            // 
            // tabPageFunctions
            // 
            this.tabPageFunctions.Controls.Add(this.dgFunctions);
            this.tabPageFunctions.Controls.Add(this.panel1);
            this.tabPageFunctions.Location = new System.Drawing.Point(4, 22);
            this.tabPageFunctions.Name = "tabPageFunctions";
            this.tabPageFunctions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFunctions.Size = new System.Drawing.Size(641, 269);
            this.tabPageFunctions.TabIndex = 1;
            this.tabPageFunctions.Text = "Functions";
            this.tabPageFunctions.UseVisualStyleBackColor = true;
            // 
            // dgFunctions
            // 
            this.dgFunctions.AllowUserToAddRows = false;
            this.dgFunctions.AllowUserToDeleteRows = false;
            this.dgFunctions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgFunctions.Location = new System.Drawing.Point(3, 38);
            this.dgFunctions.MultiSelect = false;
            this.dgFunctions.Name = "dgFunctions";
            this.dgFunctions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgFunctions.Size = new System.Drawing.Size(635, 228);
            this.dgFunctions.TabIndex = 1;
            this.dgFunctions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgFunctions_CellContentClick);
            this.dgFunctions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgFunctions_CellValueChanged);
            this.dgFunctions.VisibleChanged += new System.EventHandler(this.dgFunctions_VisibleChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(635, 35);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Custom SQLite Functions";
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(651, 392);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Options";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Options";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Options_FormClosed);
            this.Load += new System.EventHandler(this.Options_Load);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.tabmain.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.tabPageImport.ResumeLayout(false);
            this.tabPageImport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownMaxErrors)).EndInit();
            this.tabPageFunctions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgFunctions)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkOpenDB;
        private System.Windows.Forms.TextBox txtEncryptKey;
        private System.Windows.Forms.Label lblEncryptKey;
        private System.Windows.Forms.TabControl tabmain;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageFunctions;
        private System.Windows.Forms.DataGridView dgFunctions;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.TabPage tabPageImport;
        private System.Windows.Forms.CheckBox chkSaveImportCredentials;
        private System.Windows.Forms.NumericUpDown upDownMaxErrors;
        private System.Windows.Forms.CheckBox chkIgnoreErrrors;
        private System.Windows.Forms.Label label2;
    }
}