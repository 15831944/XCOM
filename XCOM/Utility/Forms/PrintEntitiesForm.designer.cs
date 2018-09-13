namespace XCOM.Commands.Utility
{
    partial class PrintEntitiesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintEntitiesForm));
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rtlHelp = new XCOM.CustomControls.RichTextLabel();
            this.lblPrecision = new System.Windows.Forms.Label();
            this.cbPrecision = new System.Windows.Forms.ComboBox();
            this.txtLineFormat = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chText = new System.Windows.Forms.CheckBox();
            this.ch3DFace = new System.Windows.Forms.CheckBox();
            this.chBlock = new System.Windows.Forms.CheckBox();
            this.chPolyline = new System.Windows.Forms.CheckBox();
            this.chLine = new System.Windows.Forms.CheckBox();
            this.chCircle = new System.Windows.Forms.CheckBox();
            this.chPoint = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.sfdOutput = new System.Windows.Forms.SaveFileDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbWCS = new System.Windows.Forms.RadioButton();
            this.rbUCS = new System.Windows.Forms.RadioButton();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(366, 471);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(285, 471);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 4;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.rtlHelp);
            this.groupBox4.Controls.Add(this.lblPrecision);
            this.groupBox4.Controls.Add(this.cbPrecision);
            this.groupBox4.Controls.Add(this.txtLineFormat);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(13, 123);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(428, 265);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Seçenekler";
            // 
            // rtlHelp
            // 
            this.rtlHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtlHelp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtlHelp.Location = new System.Drawing.Point(23, 54);
            this.rtlHelp.Name = "rtlHelp";
            this.rtlHelp.ReadOnly = true;
            this.rtlHelp.RtfResource = "PrintEntityCoordsHelp";
            this.rtlHelp.Size = new System.Drawing.Size(387, 170);
            this.rtlHelp.TabIndex = 18;
            this.rtlHelp.TabStop = false;
            this.rtlHelp.Text = resources.GetString("rtlHelp.Text");
            // 
            // lblPrecision
            // 
            this.lblPrecision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPrecision.AutoSize = true;
            this.lblPrecision.Location = new System.Drawing.Point(20, 233);
            this.lblPrecision.Name = "lblPrecision";
            this.lblPrecision.Size = new System.Drawing.Size(81, 13);
            this.lblPrecision.TabIndex = 16;
            this.lblPrecision.Text = "Basamak Sayısı";
            // 
            // cbPrecision
            // 
            this.cbPrecision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbPrecision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrecision.FormattingEnabled = true;
            this.cbPrecision.Items.AddRange(new object[] {
            "0 - 0.",
            "1 - 0.0",
            "2 - 0.00",
            "3 - 0.000",
            "4 - 0.0000",
            "5 - 0.00000",
            "6 - 0.000000",
            "7 - 0.0000000",
            "8 - 0.00000000"});
            this.cbPrecision.Location = new System.Drawing.Point(138, 230);
            this.cbPrecision.Name = "cbPrecision";
            this.cbPrecision.Size = new System.Drawing.Size(100, 21);
            this.cbPrecision.TabIndex = 17;
            // 
            // txtLineFormat
            // 
            this.txtLineFormat.AcceptsTab = true;
            this.txtLineFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLineFormat.Location = new System.Drawing.Point(138, 24);
            this.txtLineFormat.Name = "txtLineFormat";
            this.txtLineFormat.Size = new System.Drawing.Size(273, 20);
            this.txtLineFormat.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Satır Formatı";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chText);
            this.groupBox2.Controls.Add(this.ch3DFace);
            this.groupBox2.Controls.Add(this.chBlock);
            this.groupBox2.Controls.Add(this.chPolyline);
            this.groupBox2.Controls.Add(this.chLine);
            this.groupBox2.Controls.Add(this.chCircle);
            this.groupBox2.Controls.Add(this.chPoint);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(301, 105);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Seçilecek Nesneler";
            // 
            // chText
            // 
            this.chText.AutoSize = true;
            this.chText.Location = new System.Drawing.Point(179, 50);
            this.chText.Name = "chText";
            this.chText.Size = new System.Drawing.Size(47, 17);
            this.chText.TabIndex = 5;
            this.chText.Text = "Text";
            this.chText.UseVisualStyleBackColor = true;
            // 
            // ch3DFace
            // 
            this.ch3DFace.AutoSize = true;
            this.ch3DFace.Location = new System.Drawing.Point(100, 50);
            this.ch3DFace.Name = "ch3DFace";
            this.ch3DFace.Size = new System.Drawing.Size(64, 17);
            this.ch3DFace.TabIndex = 4;
            this.ch3DFace.Text = "3DFace";
            this.ch3DFace.UseVisualStyleBackColor = true;
            // 
            // chBlock
            // 
            this.chBlock.AutoSize = true;
            this.chBlock.Location = new System.Drawing.Point(20, 73);
            this.chBlock.Name = "chBlock";
            this.chBlock.Size = new System.Drawing.Size(53, 17);
            this.chBlock.TabIndex = 6;
            this.chBlock.Text = "Block";
            this.chBlock.UseVisualStyleBackColor = true;
            // 
            // chPolyline
            // 
            this.chPolyline.AutoSize = true;
            this.chPolyline.Location = new System.Drawing.Point(20, 50);
            this.chPolyline.Name = "chPolyline";
            this.chPolyline.Size = new System.Drawing.Size(62, 17);
            this.chPolyline.TabIndex = 3;
            this.chPolyline.Text = "Polyline";
            this.chPolyline.UseVisualStyleBackColor = true;
            // 
            // chLine
            // 
            this.chLine.AutoSize = true;
            this.chLine.Location = new System.Drawing.Point(180, 26);
            this.chLine.Name = "chLine";
            this.chLine.Size = new System.Drawing.Size(46, 17);
            this.chLine.TabIndex = 2;
            this.chLine.Text = "Line";
            this.chLine.UseVisualStyleBackColor = true;
            // 
            // chCircle
            // 
            this.chCircle.AutoSize = true;
            this.chCircle.Location = new System.Drawing.Point(100, 26);
            this.chCircle.Name = "chCircle";
            this.chCircle.Size = new System.Drawing.Size(52, 17);
            this.chCircle.TabIndex = 1;
            this.chCircle.Text = "Circle";
            this.chCircle.UseVisualStyleBackColor = true;
            // 
            // chPoint
            // 
            this.chPoint.AutoSize = true;
            this.chPoint.Location = new System.Drawing.Point(20, 26);
            this.chPoint.Name = "chPoint";
            this.chPoint.Size = new System.Drawing.Size(50, 17);
            this.chPoint.TabIndex = 0;
            this.chPoint.Text = "Point";
            this.chPoint.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnOpenFile);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtFilename);
            this.groupBox1.Location = new System.Drawing.Point(12, 394);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(428, 59);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Çıktı Dosyası";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenFile.Image = global::XCOM.Properties.Resources.folder;
            this.btnOpenFile.Location = new System.Drawing.Point(388, 23);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(23, 23);
            this.btnOpenFile.TabIndex = 2;
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(17, 27);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(55, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Dosya Adı";
            // 
            // txtFilename
            // 
            this.txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilename.Location = new System.Drawing.Point(138, 24);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(244, 20);
            this.txtFilename.TabIndex = 1;
            // 
            // sfdOutput
            // 
            this.sfdOutput.DefaultExt = "txt";
            this.sfdOutput.Filter = "Metin Dosyaları (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.rbWCS);
            this.groupBox3.Controls.Add(this.rbUCS);
            this.groupBox3.Location = new System.Drawing.Point(320, 13);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(120, 104);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Koordinat Sistemi";
            // 
            // rbWCS
            // 
            this.rbWCS.AutoSize = true;
            this.rbWCS.Location = new System.Drawing.Point(20, 48);
            this.rbWCS.Name = "rbWCS";
            this.rbWCS.Size = new System.Drawing.Size(50, 17);
            this.rbWCS.TabIndex = 1;
            this.rbWCS.TabStop = true;
            this.rbWCS.Text = "WCS";
            this.rbWCS.UseVisualStyleBackColor = true;
            // 
            // rbUCS
            // 
            this.rbUCS.AutoSize = true;
            this.rbUCS.Location = new System.Drawing.Point(20, 26);
            this.rbUCS.Name = "rbUCS";
            this.rbUCS.Size = new System.Drawing.Size(47, 17);
            this.rbUCS.TabIndex = 0;
            this.rbUCS.TabStop = true;
            this.rbUCS.Text = "UCS";
            this.rbUCS.UseVisualStyleBackColor = true;
            // 
            // PrintEntitiesForm
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(453, 506);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintEntitiesForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nesne Koordinatlarını Yaz";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtLineFormat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chText;
        private System.Windows.Forms.CheckBox ch3DFace;
        private System.Windows.Forms.CheckBox chPolyline;
        private System.Windows.Forms.CheckBox chLine;
        private System.Windows.Forms.CheckBox chCircle;
        private System.Windows.Forms.CheckBox chPoint;
        private System.Windows.Forms.Label lblPrecision;
        private System.Windows.Forms.ComboBox cbPrecision;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.SaveFileDialog sfdOutput;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbWCS;
        private System.Windows.Forms.RadioButton rbUCS;
        private System.Windows.Forms.CheckBox chBlock;
        private CustomControls.RichTextLabel rtlHelp;
    }
}