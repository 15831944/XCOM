namespace CoordinateLabel
{
    partial class CoordMainForm
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbDirection = new System.Windows.Forms.CheckBox();
            this.txtLineLength = new System.Windows.Forms.TextBox();
            this.lblLineLength = new System.Windows.Forms.Label();
            this.txtTextAngle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTextHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbPrecision = new System.Windows.Forms.ComboBox();
            this.lblPrecision = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbUseZ = new System.Windows.Forms.CheckBox();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.rbUseY = new System.Windows.Forms.CheckBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.rbUseX = new System.Windows.Forms.CheckBox();
            this.txtZLabel = new System.Windows.Forms.TextBox();
            this.txtStartNum = new System.Windows.Forms.TextBox();
            this.txtYLabel = new System.Windows.Forms.TextBox();
            this.lblStartNum = new System.Windows.Forms.Label();
            this.txtXLabel = new System.Windows.Forms.TextBox();
            this.rbNoNumbering = new System.Windows.Forms.RadioButton();
            this.rbAutoNumber = new System.Windows.Forms.RadioButton();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbUCS = new System.Windows.Forms.RadioButton();
            this.rbWCS = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbDirection);
            this.groupBox2.Controls.Add(this.txtLineLength);
            this.groupBox2.Controls.Add(this.lblLineLength);
            this.groupBox2.Controls.Add(this.txtTextAngle);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtTextHeight);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 97);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 158);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Yazı";
            // 
            // cbDirection
            // 
            this.cbDirection.AutoSize = true;
            this.cbDirection.Location = new System.Drawing.Point(20, 102);
            this.cbDirection.Name = "cbDirection";
            this.cbDirection.Size = new System.Drawing.Size(123, 17);
            this.cbDirection.TabIndex = 4;
            this.cbDirection.Text = "Yön Belirleyerek Yaz";
            this.cbDirection.UseVisualStyleBackColor = true;
            // 
            // txtLineLength
            // 
            this.txtLineLength.Location = new System.Drawing.Point(139, 125);
            this.txtLineLength.Name = "txtLineLength";
            this.txtLineLength.Size = new System.Drawing.Size(84, 20);
            this.txtLineLength.TabIndex = 6;
            // 
            // lblLineLength
            // 
            this.lblLineLength.AutoSize = true;
            this.lblLineLength.Location = new System.Drawing.Point(33, 128);
            this.lblLineLength.Name = "lblLineLength";
            this.lblLineLength.Size = new System.Drawing.Size(77, 13);
            this.lblLineLength.TabIndex = 5;
            this.lblLineLength.Text = "Çizgi Uzunluğu";
            // 
            // txtTextAngle
            // 
            this.txtTextAngle.Location = new System.Drawing.Point(123, 50);
            this.txtTextAngle.Name = "txtTextAngle";
            this.txtTextAngle.Size = new System.Drawing.Size(100, 20);
            this.txtTextAngle.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Yazı Açısı";
            // 
            // txtTextHeight
            // 
            this.txtTextHeight.Location = new System.Drawing.Point(123, 24);
            this.txtTextHeight.Name = "txtTextHeight";
            this.txtTextHeight.Size = new System.Drawing.Size(100, 20);
            this.txtTextHeight.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Yazı Yüksekliği";
            // 
            // cbPrecision
            // 
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
            this.cbPrecision.Location = new System.Drawing.Point(137, 130);
            this.cbPrecision.Name = "cbPrecision";
            this.cbPrecision.Size = new System.Drawing.Size(84, 21);
            this.cbPrecision.TabIndex = 8;
            // 
            // lblPrecision
            // 
            this.lblPrecision.AutoSize = true;
            this.lblPrecision.Location = new System.Drawing.Point(31, 133);
            this.lblPrecision.Name = "lblPrecision";
            this.lblPrecision.Size = new System.Drawing.Size(81, 13);
            this.lblPrecision.TabIndex = 7;
            this.lblPrecision.Text = "Basamak Sayısı";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblPrecision);
            this.groupBox4.Controls.Add(this.cbPrecision);
            this.groupBox4.Controls.Add(this.rbUseZ);
            this.groupBox4.Controls.Add(this.txtPrefix);
            this.groupBox4.Controls.Add(this.rbUseY);
            this.groupBox4.Controls.Add(this.lblPrefix);
            this.groupBox4.Controls.Add(this.rbUseX);
            this.groupBox4.Controls.Add(this.txtZLabel);
            this.groupBox4.Controls.Add(this.txtStartNum);
            this.groupBox4.Controls.Add(this.txtYLabel);
            this.groupBox4.Controls.Add(this.lblStartNum);
            this.groupBox4.Controls.Add(this.txtXLabel);
            this.groupBox4.Controls.Add(this.rbNoNumbering);
            this.groupBox4.Controls.Add(this.rbAutoNumber);
            this.groupBox4.Location = new System.Drawing.Point(263, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(245, 243);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Numaralandırma";
            // 
            // rbUseZ
            // 
            this.rbUseZ.AutoSize = true;
            this.rbUseZ.Location = new System.Drawing.Point(34, 101);
            this.rbUseZ.Name = "rbUseZ";
            this.rbUseZ.Size = new System.Drawing.Size(33, 17);
            this.rbUseZ.TabIndex = 5;
            this.rbUseZ.Text = "Z";
            this.rbUseZ.UseVisualStyleBackColor = true;
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(137, 210);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(84, 20);
            this.txtPrefix.TabIndex = 13;
            // 
            // rbUseY
            // 
            this.rbUseY.AutoSize = true;
            this.rbUseY.Location = new System.Drawing.Point(34, 75);
            this.rbUseY.Name = "rbUseY";
            this.rbUseY.Size = new System.Drawing.Size(33, 17);
            this.rbUseY.TabIndex = 3;
            this.rbUseY.Text = "Y";
            this.rbUseY.UseVisualStyleBackColor = true;
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Location = new System.Drawing.Point(31, 213);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(37, 13);
            this.lblPrefix.TabIndex = 12;
            this.lblPrefix.Text = "Ön Ek";
            // 
            // rbUseX
            // 
            this.rbUseX.AutoSize = true;
            this.rbUseX.Location = new System.Drawing.Point(34, 49);
            this.rbUseX.Name = "rbUseX";
            this.rbUseX.Size = new System.Drawing.Size(33, 17);
            this.rbUseX.TabIndex = 1;
            this.rbUseX.Text = "X";
            this.rbUseX.UseVisualStyleBackColor = true;
            // 
            // txtZLabel
            // 
            this.txtZLabel.Location = new System.Drawing.Point(137, 99);
            this.txtZLabel.Name = "txtZLabel";
            this.txtZLabel.Size = new System.Drawing.Size(84, 20);
            this.txtZLabel.TabIndex = 6;
            // 
            // txtStartNum
            // 
            this.txtStartNum.Location = new System.Drawing.Point(137, 184);
            this.txtStartNum.Name = "txtStartNum";
            this.txtStartNum.Size = new System.Drawing.Size(84, 20);
            this.txtStartNum.TabIndex = 11;
            // 
            // txtYLabel
            // 
            this.txtYLabel.Location = new System.Drawing.Point(137, 73);
            this.txtYLabel.Name = "txtYLabel";
            this.txtYLabel.Size = new System.Drawing.Size(84, 20);
            this.txtYLabel.TabIndex = 4;
            // 
            // lblStartNum
            // 
            this.lblStartNum.AutoSize = true;
            this.lblStartNum.Location = new System.Drawing.Point(29, 187);
            this.lblStartNum.Name = "lblStartNum";
            this.lblStartNum.Size = new System.Drawing.Size(100, 13);
            this.lblStartNum.TabIndex = 10;
            this.lblStartNum.Text = "Başlangıç Numarası";
            // 
            // txtXLabel
            // 
            this.txtXLabel.Location = new System.Drawing.Point(137, 47);
            this.txtXLabel.Name = "txtXLabel";
            this.txtXLabel.Size = new System.Drawing.Size(84, 20);
            this.txtXLabel.TabIndex = 2;
            // 
            // rbNoNumbering
            // 
            this.rbNoNumbering.AutoSize = true;
            this.rbNoNumbering.Location = new System.Drawing.Point(20, 25);
            this.rbNoNumbering.Name = "rbNoNumbering";
            this.rbNoNumbering.Size = new System.Drawing.Size(123, 17);
            this.rbNoNumbering.TabIndex = 0;
            this.rbNoNumbering.TabStop = true;
            this.rbNoNumbering.Text = "Numaralandırma Yok";
            this.rbNoNumbering.UseVisualStyleBackColor = true;
            // 
            // rbAutoNumber
            // 
            this.rbAutoNumber.AutoSize = true;
            this.rbAutoNumber.Location = new System.Drawing.Point(20, 161);
            this.rbAutoNumber.Name = "rbAutoNumber";
            this.rbAutoNumber.Size = new System.Drawing.Size(146, 17);
            this.rbAutoNumber.TabIndex = 9;
            this.rbAutoNumber.TabStop = true;
            this.rbAutoNumber.Text = "Otomatik Numaralandırma";
            this.rbAutoNumber.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(435, 271);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(354, 271);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rbUCS);
            this.groupBox5.Controls.Add(this.rbWCS);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(245, 80);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Koordinat Sistemi";
            // 
            // rbUCS
            // 
            this.rbUCS.AutoSize = true;
            this.rbUCS.Location = new System.Drawing.Point(20, 48);
            this.rbUCS.Name = "rbUCS";
            this.rbUCS.Size = new System.Drawing.Size(47, 17);
            this.rbUCS.TabIndex = 1;
            this.rbUCS.TabStop = true;
            this.rbUCS.Text = "UCS";
            this.rbUCS.UseVisualStyleBackColor = true;
            // 
            // rbWCS
            // 
            this.rbWCS.AutoSize = true;
            this.rbWCS.Location = new System.Drawing.Point(20, 25);
            this.rbWCS.Name = "rbWCS";
            this.rbWCS.Size = new System.Drawing.Size(50, 17);
            this.rbWCS.TabIndex = 0;
            this.rbWCS.TabStop = true;
            this.rbWCS.Text = "WCS";
            this.rbWCS.UseVisualStyleBackColor = true;
            // 
            // CoordMainForm
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(522, 306);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoordMainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Koordinat";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbDirection;
        private System.Windows.Forms.TextBox txtLineLength;
        private System.Windows.Forms.Label lblLineLength;
        private System.Windows.Forms.TextBox txtTextAngle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTextHeight;
        private System.Windows.Forms.ComboBox cbPrecision;
        private System.Windows.Forms.Label lblPrecision;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.TextBox txtStartNum;
        private System.Windows.Forms.Label lblStartNum;
        private System.Windows.Forms.RadioButton rbNoNumbering;
        private System.Windows.Forms.RadioButton rbAutoNumber;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rbUCS;
        private System.Windows.Forms.RadioButton rbWCS;
        private System.Windows.Forms.CheckBox rbUseZ;
        private System.Windows.Forms.CheckBox rbUseY;
        private System.Windows.Forms.CheckBox rbUseX;
        private System.Windows.Forms.TextBox txtZLabel;
        private System.Windows.Forms.TextBox txtYLabel;
        private System.Windows.Forms.TextBox txtXLabel;
    }
}