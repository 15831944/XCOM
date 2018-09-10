namespace XCOM.Commands.Block
{
    partial class RenameBlockForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenameBlockForm));
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.btnRename = new System.Windows.Forms.Button();
            this.lvBlocks = new System.Windows.Forms.ListView();
            this.chOldName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chNewName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtOldName = new System.Windows.Forms.TextBox();
            this.txtBlockName = new System.Windows.Forms.TextBox();
            this.cbShowAnon = new System.Windows.Forms.CheckBox();
            this.lvImageList = new System.Windows.Forms.ImageList(this.components);
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(297, 405);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(216, 405);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.pbPreview);
            this.groupBox2.Controls.Add(this.btnRename);
            this.groupBox2.Controls.Add(this.lvBlocks);
            this.groupBox2.Controls.Add(this.txtOldName);
            this.groupBox2.Controls.Add(this.txtBlockName);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(360, 376);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Blok Listesi";
            // 
            // pbPreview
            // 
            this.pbPreview.Location = new System.Drawing.Point(20, 312);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(32, 32);
            this.pbPreview.TabIndex = 0;
            this.pbPreview.TabStop = false;
            // 
            // btnRename
            // 
            this.btnRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRename.Location = new System.Drawing.Point(267, 336);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(75, 23);
            this.btnRename.TabIndex = 3;
            this.btnRename.Text = "Değiştir";
            this.btnRename.UseVisualStyleBackColor = true;
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // lvBlocks
            // 
            this.lvBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvBlocks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chOldName,
            this.chNewName});
            this.lvBlocks.FullRowSelect = true;
            this.lvBlocks.HideSelection = false;
            this.lvBlocks.Location = new System.Drawing.Point(20, 25);
            this.lvBlocks.MultiSelect = false;
            this.lvBlocks.Name = "lvBlocks";
            this.lvBlocks.Size = new System.Drawing.Size(322, 281);
            this.lvBlocks.SmallImageList = this.lvImageList;
            this.lvBlocks.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvBlocks.TabIndex = 0;
            this.lvBlocks.UseCompatibleStateImageBehavior = false;
            this.lvBlocks.View = System.Windows.Forms.View.Details;
            this.lvBlocks.SelectedIndexChanged += new System.EventHandler(this.lvBlocks_SelectedIndexChanged);
            // 
            // chOldName
            // 
            this.chOldName.Text = "Eski Blok Adı";
            this.chOldName.Width = 150;
            // 
            // chNewName
            // 
            this.chNewName.Text = "Yeni Blok Adı";
            this.chNewName.Width = 150;
            // 
            // txtOldName
            // 
            this.txtOldName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOldName.Location = new System.Drawing.Point(62, 312);
            this.txtOldName.Name = "txtOldName";
            this.txtOldName.ReadOnly = true;
            this.txtOldName.Size = new System.Drawing.Size(199, 20);
            this.txtOldName.TabIndex = 1;
            // 
            // txtBlockName
            // 
            this.txtBlockName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBlockName.Location = new System.Drawing.Point(62, 338);
            this.txtBlockName.Name = "txtBlockName";
            this.txtBlockName.Size = new System.Drawing.Size(199, 20);
            this.txtBlockName.TabIndex = 2;
            // 
            // cbShowAnon
            // 
            this.cbShowAnon.AutoSize = true;
            this.cbShowAnon.Location = new System.Drawing.Point(12, 409);
            this.cbShowAnon.Name = "cbShowAnon";
            this.cbShowAnon.Size = new System.Drawing.Size(146, 17);
            this.cbShowAnon.TabIndex = 1;
            this.cbShowAnon.Text = "Anonymous blokları listele";
            this.cbShowAnon.UseVisualStyleBackColor = true;
            this.cbShowAnon.CheckedChanged += new System.EventHandler(this.cbShowAnon_CheckedChanged);
            // 
            // lvImageList
            // 
            this.lvImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("lvImageList.ImageStream")));
            this.lvImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.lvImageList.Images.SetKeyName(0, "error");
            this.lvImageList.Images.SetKeyName(1, "tick");
            // 
            // RenameBlockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(384, 440);
            this.Controls.Add(this.cbShowAnon);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameBlockForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Numaralandırma";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtBlockName;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.ListView lvBlocks;
        private System.Windows.Forms.ColumnHeader chOldName;
        private System.Windows.Forms.ColumnHeader chNewName;
        private System.Windows.Forms.TextBox txtOldName;
        private System.Windows.Forms.CheckBox cbShowAnon;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.ImageList lvImageList;
    }
}