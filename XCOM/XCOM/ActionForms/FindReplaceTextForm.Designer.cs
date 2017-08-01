namespace XCOM.Commands.XCommand
{
    partial class FindReplaceTextForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbBlock = new System.Windows.Forms.CheckBox();
            this.cbTable = new System.Windows.Forms.CheckBox();
            this.cbLeader = new System.Windows.Forms.CheckBox();
            this.cbDimension = new System.Windows.Forms.CheckBox();
            this.cbAttribute = new System.Windows.Forms.CheckBox();
            this.cbText = new System.Windows.Forms.CheckBox();
            this.cbWhere = new System.Windows.Forms.ComboBox();
            this.cbCaseSensitive = new System.Windows.Forms.CheckBox();
            this.cbWholeWords = new System.Windows.Forms.CheckBox();
            this.cbWildcards = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.columnHeader1.Text = "Bul";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Değiştir";
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
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Değiştir:";
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
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bul:";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(427, 405);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 11;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(346, 405);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 10;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbBlock);
            this.groupBox1.Controls.Add(this.cbTable);
            this.groupBox1.Controls.Add(this.cbLeader);
            this.groupBox1.Controls.Add(this.cbDimension);
            this.groupBox1.Controls.Add(this.cbAttribute);
            this.groupBox1.Controls.Add(this.cbText);
            this.groupBox1.Location = new System.Drawing.Point(12, 282);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(267, 109);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Aranacak Çizim Nesneleri";
            // 
            // cbBlock
            // 
            this.cbBlock.AutoSize = true;
            this.cbBlock.Location = new System.Drawing.Point(148, 77);
            this.cbBlock.Name = "cbBlock";
            this.cbBlock.Size = new System.Drawing.Size(84, 17);
            this.cbBlock.TabIndex = 5;
            this.cbBlock.Text = "Block İçeriği";
            this.cbBlock.UseVisualStyleBackColor = true;
            // 
            // cbTable
            // 
            this.cbTable.AutoSize = true;
            this.cbTable.Location = new System.Drawing.Point(148, 54);
            this.cbTable.Name = "cbTable";
            this.cbTable.Size = new System.Drawing.Size(53, 17);
            this.cbTable.TabIndex = 4;
            this.cbTable.Text = "Table";
            this.cbTable.UseVisualStyleBackColor = true;
            // 
            // cbLeader
            // 
            this.cbLeader.AutoSize = true;
            this.cbLeader.Location = new System.Drawing.Point(148, 31);
            this.cbLeader.Name = "cbLeader";
            this.cbLeader.Size = new System.Drawing.Size(59, 17);
            this.cbLeader.TabIndex = 3;
            this.cbLeader.Text = "Leader";
            this.cbLeader.UseVisualStyleBackColor = true;
            // 
            // cbDimension
            // 
            this.cbDimension.AutoSize = true;
            this.cbDimension.Location = new System.Drawing.Point(17, 77);
            this.cbDimension.Name = "cbDimension";
            this.cbDimension.Size = new System.Drawing.Size(75, 17);
            this.cbDimension.TabIndex = 2;
            this.cbDimension.Text = "Dimension";
            this.cbDimension.UseVisualStyleBackColor = true;
            // 
            // cbAttribute
            // 
            this.cbAttribute.AutoSize = true;
            this.cbAttribute.Location = new System.Drawing.Point(17, 54);
            this.cbAttribute.Name = "cbAttribute";
            this.cbAttribute.Size = new System.Drawing.Size(65, 17);
            this.cbAttribute.TabIndex = 1;
            this.cbAttribute.Text = "Attribute";
            this.cbAttribute.UseVisualStyleBackColor = true;
            // 
            // cbText
            // 
            this.cbText.AutoSize = true;
            this.cbText.Checked = true;
            this.cbText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbText.Location = new System.Drawing.Point(17, 31);
            this.cbText.Name = "cbText";
            this.cbText.Size = new System.Drawing.Size(82, 17);
            this.cbText.TabIndex = 0;
            this.cbText.Text = "Text/MText";
            this.cbText.UseVisualStyleBackColor = true;
            // 
            // cbWhere
            // 
            this.cbWhere.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWhere.FormattingEnabled = true;
            this.cbWhere.Items.AddRange(new object[] {
            "Tüm Çizim",
            "Sadece Model",
            "Sadece Layout\'lar"});
            this.cbWhere.Location = new System.Drawing.Point(12, 407);
            this.cbWhere.Name = "cbWhere";
            this.cbWhere.Size = new System.Drawing.Size(142, 21);
            this.cbWhere.TabIndex = 9;
            // 
            // cbCaseSensitive
            // 
            this.cbCaseSensitive.AutoSize = true;
            this.cbCaseSensitive.Location = new System.Drawing.Point(23, 31);
            this.cbCaseSensitive.Name = "cbCaseSensitive";
            this.cbCaseSensitive.Size = new System.Drawing.Size(150, 17);
            this.cbCaseSensitive.TabIndex = 0;
            this.cbCaseSensitive.Text = "Büyük/Küçük Harf Duyarlı";
            this.cbCaseSensitive.UseVisualStyleBackColor = true;
            // 
            // cbWholeWords
            // 
            this.cbWholeWords.AutoSize = true;
            this.cbWholeWords.Location = new System.Drawing.Point(23, 54);
            this.cbWholeWords.Name = "cbWholeWords";
            this.cbWholeWords.Size = new System.Drawing.Size(152, 17);
            this.cbWholeWords.TabIndex = 1;
            this.cbWholeWords.Text = "Sadece Tam Kelimeleri Bul";
            this.cbWholeWords.UseVisualStyleBackColor = true;
            // 
            // cbWildcards
            // 
            this.cbWildcards.AutoSize = true;
            this.cbWildcards.Location = new System.Drawing.Point(23, 77);
            this.cbWildcards.Name = "cbWildcards";
            this.cbWildcards.Size = new System.Drawing.Size(92, 17);
            this.cbWildcards.TabIndex = 2;
            this.cbWildcards.Text = "Wildcards: * ?";
            this.cbWildcards.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbCaseSensitive);
            this.groupBox2.Controls.Add(this.cbWholeWords);
            this.groupBox2.Controls.Add(this.cbWildcards);
            this.groupBox2.Location = new System.Drawing.Point(302, 282);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 109);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Seçenekler";
            // 
            // FindReplaceTextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(518, 440);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cbWhere);
            this.Controls.Add(this.groupBox1);
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
            this.Name = "FindReplaceTextForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bul & Değiştir";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbDimension;
        private System.Windows.Forms.CheckBox cbAttribute;
        private System.Windows.Forms.CheckBox cbText;
        private System.Windows.Forms.CheckBox cbLeader;
        private System.Windows.Forms.CheckBox cbTable;
        private System.Windows.Forms.CheckBox cbCaseSensitive;
        private System.Windows.Forms.CheckBox cbWildcards;
        private System.Windows.Forms.CheckBox cbWholeWords;
        private System.Windows.Forms.ListView lvFindReplace;
        private System.Windows.Forms.ComboBox cbWhere;
        private System.Windows.Forms.CheckBox cbBlock;

    }
}