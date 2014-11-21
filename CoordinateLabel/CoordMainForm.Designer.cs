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
            this.label3 = new System.Windows.Forms.Label();
            this.lblPrecision = new System.Windows.Forms.Label();
            this.cbDirection = new System.Windows.Forms.CheckBox();
            this.cbTextStyle = new System.Windows.Forms.ComboBox();
            this.cbPrecision = new System.Windows.Forms.ComboBox();
            this.txtLineLength = new System.Windows.Forms.TextBox();
            this.lblLineLength = new System.Windows.Forms.Label();
            this.txtTextAngle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTextHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtPrefix = new System.Windows.Forms.TextBox();
            this.lblPrefix = new System.Windows.Forms.Label();
            this.txtStartNum = new System.Windows.Forms.TextBox();
            this.lblStartNum = new System.Windows.Forms.Label();
            this.rbNoNumbering = new System.Windows.Forms.RadioButton();
            this.rbAutoNumber = new System.Windows.Forms.RadioButton();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.btnReadCoords = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lblPrecision);
            this.groupBox2.Controls.Add(this.cbDirection);
            this.groupBox2.Controls.Add(this.cbTextStyle);
            this.groupBox2.Controls.Add(this.cbPrecision);
            this.groupBox2.Controls.Add(this.txtLineLength);
            this.groupBox2.Controls.Add(this.lblLineLength);
            this.groupBox2.Controls.Add(this.txtTextAngle);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtTextHeight);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 190);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Yazı";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Yazı Stili";
            // 
            // lblPrecision
            // 
            this.lblPrecision.AutoSize = true;
            this.lblPrecision.Location = new System.Drawing.Point(19, 107);
            this.lblPrecision.Name = "lblPrecision";
            this.lblPrecision.Size = new System.Drawing.Size(81, 13);
            this.lblPrecision.TabIndex = 6;
            this.lblPrecision.Text = "Basamak Sayısı";
            // 
            // cbDirection
            // 
            this.cbDirection.AutoSize = true;
            this.cbDirection.Location = new System.Drawing.Point(22, 131);
            this.cbDirection.Name = "cbDirection";
            this.cbDirection.Size = new System.Drawing.Size(123, 17);
            this.cbDirection.TabIndex = 8;
            this.cbDirection.Text = "Yön Belirleyerek Yaz";
            this.cbDirection.UseVisualStyleBackColor = true;
            // 
            // cbTextStyle
            // 
            this.cbTextStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextStyle.FormattingEnabled = true;
            this.cbTextStyle.Items.AddRange(new object[] {
            "0 - 0.",
            "1 - 0.0",
            "2 - 0.00",
            "3 - 0.000",
            "4 - 0.0000",
            "5 - 0.00000",
            "6 - 0.000000",
            "7 - 0.0000000",
            "8 - 0.00000000"});
            this.cbTextStyle.Location = new System.Drawing.Point(123, 77);
            this.cbTextStyle.Name = "cbTextStyle";
            this.cbTextStyle.Size = new System.Drawing.Size(100, 21);
            this.cbTextStyle.TabIndex = 5;
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
            this.cbPrecision.Location = new System.Drawing.Point(123, 104);
            this.cbPrecision.Name = "cbPrecision";
            this.cbPrecision.Size = new System.Drawing.Size(100, 21);
            this.cbPrecision.TabIndex = 7;
            // 
            // txtLineLength
            // 
            this.txtLineLength.Location = new System.Drawing.Point(141, 154);
            this.txtLineLength.Name = "txtLineLength";
            this.txtLineLength.Size = new System.Drawing.Size(84, 20);
            this.txtLineLength.TabIndex = 10;
            // 
            // lblLineLength
            // 
            this.lblLineLength.AutoSize = true;
            this.lblLineLength.Location = new System.Drawing.Point(35, 157);
            this.lblLineLength.Name = "lblLineLength";
            this.lblLineLength.Size = new System.Drawing.Size(77, 13);
            this.lblLineLength.TabIndex = 9;
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtPrefix);
            this.groupBox4.Controls.Add(this.lblPrefix);
            this.groupBox4.Controls.Add(this.txtStartNum);
            this.groupBox4.Controls.Add(this.lblStartNum);
            this.groupBox4.Controls.Add(this.rbNoNumbering);
            this.groupBox4.Controls.Add(this.rbAutoNumber);
            this.groupBox4.Location = new System.Drawing.Point(263, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(245, 190);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Numaralandırma";
            // 
            // txtPrefix
            // 
            this.txtPrefix.Location = new System.Drawing.Point(137, 103);
            this.txtPrefix.Name = "txtPrefix";
            this.txtPrefix.Size = new System.Drawing.Size(84, 20);
            this.txtPrefix.TabIndex = 5;
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Location = new System.Drawing.Point(31, 106);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(37, 13);
            this.lblPrefix.TabIndex = 4;
            this.lblPrefix.Text = "Ön Ek";
            // 
            // txtStartNum
            // 
            this.txtStartNum.Location = new System.Drawing.Point(137, 77);
            this.txtStartNum.Name = "txtStartNum";
            this.txtStartNum.Size = new System.Drawing.Size(84, 20);
            this.txtStartNum.TabIndex = 3;
            // 
            // lblStartNum
            // 
            this.lblStartNum.AutoSize = true;
            this.lblStartNum.Location = new System.Drawing.Point(29, 80);
            this.lblStartNum.Name = "lblStartNum";
            this.lblStartNum.Size = new System.Drawing.Size(100, 13);
            this.lblStartNum.TabIndex = 2;
            this.lblStartNum.Text = "Başlangıç Numarası";
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
            this.rbAutoNumber.Location = new System.Drawing.Point(20, 51);
            this.rbAutoNumber.Name = "rbAutoNumber";
            this.rbAutoNumber.Size = new System.Drawing.Size(146, 17);
            this.rbAutoNumber.TabIndex = 1;
            this.rbAutoNumber.TabStop = true;
            this.rbAutoNumber.Text = "Otomatik Numaralandırma";
            this.rbAutoNumber.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(435, 216);
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
            this.cmdOK.Location = new System.Drawing.Point(354, 216);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // btnReadCoords
            // 
            this.btnReadCoords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReadCoords.Image = global::CoordinateLabel.Properties.Resources.pick_point;
            this.btnReadCoords.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReadCoords.Location = new System.Drawing.Point(12, 216);
            this.btnReadCoords.Name = "btnReadCoords";
            this.btnReadCoords.Size = new System.Drawing.Size(112, 23);
            this.btnReadCoords.TabIndex = 2;
            this.btnReadCoords.Text = "Çizimden Oku";
            this.btnReadCoords.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReadCoords.UseVisualStyleBackColor = true;
            this.btnReadCoords.Click += new System.EventHandler(this.btnReadCoords_Click);
            // 
            // CoordMainForm
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(522, 251);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.btnReadCoords);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoordMainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Koordinat";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbTextStyle;
        private System.Windows.Forms.Button btnReadCoords;
    }
}