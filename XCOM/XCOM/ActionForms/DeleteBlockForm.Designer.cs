namespace XCOM.Commands.XCommand
{
    partial class DeleteBlockForm
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
            this.lvBlockNames = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearBlockNames = new System.Windows.Forms.Button();
            this.btnAddBlockName = new System.Windows.Forms.Button();
            this.txtBlockName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cbBlock = new System.Windows.Forms.CheckBox();
            this.cbWhere = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lvBlockNames
            // 
            this.lvBlockNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvBlockNames.Location = new System.Drawing.Point(12, 82);
            this.lvBlockNames.Name = "lvBlockNames";
            this.lvBlockNames.Size = new System.Drawing.Size(490, 182);
            this.lvBlockNames.TabIndex = 4;
            this.lvBlockNames.UseCompatibleStateImageBehavior = false;
            this.lvBlockNames.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Blok Adı";
            this.columnHeader1.Width = 200;
            // 
            // btnClearBlockNames
            // 
            this.btnClearBlockNames.Image = global::XCOM.Properties.Resources.cross;
            this.btnClearBlockNames.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnClearBlockNames.Location = new System.Drawing.Point(410, 41);
            this.btnClearBlockNames.Name = "btnClearBlockNames";
            this.btnClearBlockNames.Size = new System.Drawing.Size(92, 23);
            this.btnClearBlockNames.TabIndex = 3;
            this.btnClearBlockNames.Text = "Temizle";
            this.btnClearBlockNames.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClearBlockNames.UseVisualStyleBackColor = true;
            this.btnClearBlockNames.Click += new System.EventHandler(this.btnClearBlockNames_Click);
            // 
            // btnAddBlockName
            // 
            this.btnAddBlockName.Image = global::XCOM.Properties.Resources.add;
            this.btnAddBlockName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnAddBlockName.Location = new System.Drawing.Point(410, 12);
            this.btnAddBlockName.Name = "btnAddBlockName";
            this.btnAddBlockName.Size = new System.Drawing.Size(92, 23);
            this.btnAddBlockName.TabIndex = 2;
            this.btnAddBlockName.Text = "Ekle";
            this.btnAddBlockName.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddBlockName.UseVisualStyleBackColor = true;
            this.btnAddBlockName.Click += new System.EventHandler(this.btnAddBlockName_Click);
            // 
            // txtBlockName
            // 
            this.txtBlockName.Location = new System.Drawing.Point(93, 14);
            this.txtBlockName.Name = "txtBlockName";
            this.txtBlockName.Size = new System.Drawing.Size(298, 20);
            this.txtBlockName.TabIndex = 1;
            this.txtBlockName.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(9, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Blok Adı:";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(427, 311);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 8;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(346, 311);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 7;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cbBlock
            // 
            this.cbBlock.AutoSize = true;
            this.cbBlock.Location = new System.Drawing.Point(12, 282);
            this.cbBlock.Name = "cbBlock";
            this.cbBlock.Size = new System.Drawing.Size(78, 17);
            this.cbBlock.TabIndex = 5;
            this.cbBlock.Text = "Blok İçeriği";
            this.cbBlock.UseVisualStyleBackColor = true;
            // 
            // cbWhere
            // 
            this.cbWhere.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbWhere.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWhere.FormattingEnabled = true;
            this.cbWhere.Items.AddRange(new object[] {
            "Tüm Çizim",
            "Sadece Model",
            "Sadece Layout\'lar"});
            this.cbWhere.Location = new System.Drawing.Point(12, 313);
            this.cbWhere.Name = "cbWhere";
            this.cbWhere.Size = new System.Drawing.Size(142, 21);
            this.cbWhere.TabIndex = 6;
            // 
            // DeleteBlockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(518, 346);
            this.Controls.Add(this.cbBlock);
            this.Controls.Add(this.cbWhere);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.lvBlockNames);
            this.Controls.Add(this.btnClearBlockNames);
            this.Controls.Add(this.btnAddBlockName);
            this.Controls.Add(this.txtBlockName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteBlockForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Block Sil";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnClearBlockNames;
        private System.Windows.Forms.Button btnAddBlockName;
        private System.Windows.Forms.TextBox txtBlockName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.ListView lvBlockNames;
        private System.Windows.Forms.ComboBox cbWhere;
        private System.Windows.Forms.CheckBox cbBlock;

    }
}