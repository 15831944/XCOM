namespace RebarPosCommands
{
    partial class SetingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetingsForm));
            this.gbOptions = new System.Windows.Forms.GroupBox();
            this.udPrecision = new System.Windows.Forms.NumericUpDown();
            this.cbDisplayUnit = new System.Windows.Forms.ComboBox();
            this.cbDrawingUnit = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDiameterList = new System.Windows.Forms.TextBox();
            this.txtMaxLength = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkBending = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lGroups = new System.Windows.Forms.ImageList(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.gbOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPrecision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // gbOptions
            // 
            this.gbOptions.Controls.Add(this.udPrecision);
            this.gbOptions.Controls.Add(this.cbDisplayUnit);
            this.gbOptions.Controls.Add(this.cbDrawingUnit);
            this.gbOptions.Controls.Add(this.label4);
            this.gbOptions.Controls.Add(this.label3);
            this.gbOptions.Controls.Add(this.txtDiameterList);
            this.gbOptions.Controls.Add(this.txtMaxLength);
            this.gbOptions.Controls.Add(this.label2);
            this.gbOptions.Controls.Add(this.label17);
            this.gbOptions.Controls.Add(this.label1);
            this.gbOptions.Controls.Add(this.chkBending);
            this.gbOptions.Location = new System.Drawing.Point(12, 12);
            this.gbOptions.Name = "gbOptions";
            this.gbOptions.Size = new System.Drawing.Size(308, 355);
            this.gbOptions.TabIndex = 0;
            this.gbOptions.TabStop = false;
            this.gbOptions.Text = "S&eçenekler";
            // 
            // udPrecision
            // 
            this.udPrecision.Location = new System.Drawing.Point(188, 74);
            this.udPrecision.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.udPrecision.Name = "udPrecision";
            this.udPrecision.Size = new System.Drawing.Size(100, 20);
            this.udPrecision.TabIndex = 5;
            this.udPrecision.ValueChanged += new System.EventHandler(this.udPrecision_ValueChanged);
            // 
            // cbDisplayUnit
            // 
            this.cbDisplayUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDisplayUnit.FormattingEnabled = true;
            this.cbDisplayUnit.Items.AddRange(new object[] {
            "Milimetre",
            "Santimetre"});
            this.cbDisplayUnit.Location = new System.Drawing.Point(188, 46);
            this.cbDisplayUnit.Name = "cbDisplayUnit";
            this.cbDisplayUnit.Size = new System.Drawing.Size(100, 21);
            this.cbDisplayUnit.TabIndex = 3;
            this.cbDisplayUnit.SelectedIndexChanged += new System.EventHandler(this.cbDisplayUnit_SelectedIndexChanged);
            // 
            // cbDrawingUnit
            // 
            this.cbDrawingUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDrawingUnit.FormattingEnabled = true;
            this.cbDrawingUnit.Items.AddRange(new object[] {
            "Milimetre",
            "Santimetre"});
            this.cbDrawingUnit.Location = new System.Drawing.Point(188, 19);
            this.cbDrawingUnit.Name = "cbDrawingUnit";
            this.cbDrawingUnit.Size = new System.Drawing.Size(100, 21);
            this.cbDrawingUnit.TabIndex = 1;
            this.cbDrawingUnit.SelectedIndexChanged += new System.EventHandler(this.cbDrawingUnit_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Basamak &Sayısı";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "&Görüntülenen Birim";
            // 
            // txtDiameterList
            // 
            this.txtDiameterList.Location = new System.Drawing.Point(20, 275);
            this.txtDiameterList.Multiline = true;
            this.txtDiameterList.Name = "txtDiameterList";
            this.txtDiameterList.Size = new System.Drawing.Size(268, 59);
            this.txtDiameterList.TabIndex = 10;
            this.txtDiameterList.Validating += new System.ComponentModel.CancelEventHandler(this.txtDiameterList_Validating);
            this.txtDiameterList.Validated += new System.EventHandler(this.txtDiameterList_Validated);
            // 
            // txtMaxLength
            // 
            this.txtMaxLength.Location = new System.Drawing.Point(188, 142);
            this.txtMaxLength.Name = "txtMaxLength";
            this.txtMaxLength.Size = new System.Drawing.Size(100, 20);
            this.txtMaxLength.TabIndex = 7;
            this.txtMaxLength.Validating += new System.ComponentModel.CancelEventHandler(this.txtMaxLength_Validating);
            this.txtMaxLength.Validated += new System.EventHandler(this.txtMaxLength_Validated);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Çizim &Birimi";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(17, 259);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(104, 13);
            this.label17.TabIndex = 9;
            this.label17.Text = "&Standart Çap Listesi:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Maksimum &Demir Boyu (m)";
            // 
            // chkBending
            // 
            this.chkBending.AutoSize = true;
            this.chkBending.Location = new System.Drawing.Point(20, 202);
            this.chkBending.Name = "chkBending";
            this.chkBending.Size = new System.Drawing.Size(162, 17);
            this.chkBending.TabIndex = 8;
            this.chkBending.Text = "&Demir Bükümlerini Dikkate Al";
            this.chkBending.UseVisualStyleBackColor = true;
            this.chkBending.CheckedChanged += new System.EventHandler(this.chkBending_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(167, 379);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Tamam";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(248, 379);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "İptal";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lGroups
            // 
            this.lGroups.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("lGroups.ImageStream")));
            this.lGroups.TransparentColor = System.Drawing.Color.Transparent;
            this.lGroups.Images.SetKeyName(0, "page.png");
            this.lGroups.Images.SetKeyName(1, "page_white.png");
            this.lGroups.Images.SetKeyName(2, "page_white_add.png");
            this.lGroups.Images.SetKeyName(3, "page_white_delete.png");
            this.lGroups.Images.SetKeyName(4, "page_go.png");
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            this.errorProvider.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider.Icon")));
            // 
            // GroupForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(335, 414);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.gbOptions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GroupForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Poz Ayarları";
            this.gbOptions.ResumeLayout(false);
            this.gbOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udPrecision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbOptions;
        private System.Windows.Forms.CheckBox chkBending;
        private System.Windows.Forms.TextBox txtMaxLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDisplayUnit;
        private System.Windows.Forms.ComboBox cbDrawingUnit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown udPrecision;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ImageList lGroups;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.TextBox txtDiameterList;
        private System.Windows.Forms.Label label17;
    }
}