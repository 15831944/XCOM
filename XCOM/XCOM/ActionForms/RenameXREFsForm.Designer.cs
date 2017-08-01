namespace XCOM.Commands.XCommand
{
    partial class RenameXREFsForm
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
            this.lvFindReplace = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearFindReplace = new System.Windows.Forms.Button();
            this.btnAddFindReplace = new System.Windows.Forms.Button();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFind = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cbRenamePaths = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lvFindReplace
            // 
            this.lvFindReplace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvFindReplace.Location = new System.Drawing.Point(12, 82);
            this.lvFindReplace.Name = "lvFindReplace";
            this.lvFindReplace.Size = new System.Drawing.Size(490, 182);
            this.lvFindReplace.TabIndex = 6;
            this.lvFindReplace.UseCompatibleStateImageBehavior = false;
            this.lvFindReplace.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Eski";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Yeni";
            this.columnHeader2.Width = 200;
            // 
            // btnClearFindReplace
            // 
            this.btnClearFindReplace.Image = global::XCOM.Properties.Resources.cross;
            this.btnClearFindReplace.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClearFindReplace.Location = new System.Drawing.Point(410, 41);
            this.btnClearFindReplace.Name = "btnClearFindReplace";
            this.btnClearFindReplace.Size = new System.Drawing.Size(92, 23);
            this.btnClearFindReplace.TabIndex = 5;
            this.btnClearFindReplace.Text = "Temizle";
            this.btnClearFindReplace.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClearFindReplace.UseVisualStyleBackColor = true;
            this.btnClearFindReplace.Click += new System.EventHandler(this.btnClearFindReplace_Click);
            // 
            // btnAddFindReplace
            // 
            this.btnAddFindReplace.Image = global::XCOM.Properties.Resources.add;
            this.btnAddFindReplace.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddFindReplace.Location = new System.Drawing.Point(410, 12);
            this.btnAddFindReplace.Name = "btnAddFindReplace";
            this.btnAddFindReplace.Size = new System.Drawing.Size(92, 23);
            this.btnAddFindReplace.TabIndex = 4;
            this.btnAddFindReplace.Text = "Ekle";
            this.btnAddFindReplace.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddFindReplace.UseVisualStyleBackColor = true;
            this.btnAddFindReplace.Click += new System.EventHandler(this.btnAddFindReplace_Click);
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(93, 43);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(298, 20);
            this.txtReplace.TabIndex = 3;
            this.txtReplace.TextChanged += new System.EventHandler(this.txtReplace_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(9, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Yeni:";
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(93, 14);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(298, 20);
            this.txtFind.TabIndex = 1;
            this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Eski:";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(427, 278);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 9;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(346, 278);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 8;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cbRenamePaths
            // 
            this.cbRenamePaths.AutoSize = true;
            this.cbRenamePaths.Checked = true;
            this.cbRenamePaths.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRenamePaths.Location = new System.Drawing.Point(12, 282);
            this.cbRenamePaths.Name = "cbRenamePaths";
            this.cbRenamePaths.Size = new System.Drawing.Size(161, 17);
            this.cbRenamePaths.TabIndex = 7;
            this.cbRenamePaths.Text = "XREF dosya adını da değiştir";
            this.cbRenamePaths.UseVisualStyleBackColor = true;
            // 
            // RenameXREFsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(518, 313);
            this.Controls.Add(this.cbRenamePaths);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.lvFindReplace);
            this.Controls.Add(this.btnClearFindReplace);
            this.Controls.Add(this.btnAddFindReplace);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameXREFsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "XREF Adını Değiştir";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnClearFindReplace;
        private System.Windows.Forms.Button btnAddFindReplace;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.ListView lvFindReplace;
        private System.Windows.Forms.CheckBox cbRenamePaths;
    }
}