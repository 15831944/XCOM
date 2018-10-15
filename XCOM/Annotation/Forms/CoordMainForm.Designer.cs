namespace XCOM.Commands.Annotation
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
            this.cbAutoRotateText = new System.Windows.Forms.CheckBox();
            this.cbZCoord = new System.Windows.Forms.CheckBox();
            this.cbDirection = new System.Windows.Forms.CheckBox();
            this.txtLineLength = new System.Windows.Forms.TextBox();
            this.lblLineLength = new System.Windows.Forms.Label();
            this.txtTextAngle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTextHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPrecision = new System.Windows.Forms.Label();
            this.cbTextStyle = new AcadUtility.WinForms.TextStyleComboBox();
            this.cbPrecision = new AcadUtility.WinForms.PrecisionComboBox();
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbAutoRotateText);
            this.groupBox2.Controls.Add(this.cbZCoord);
            this.groupBox2.Controls.Add(this.cbDirection);
            this.groupBox2.Controls.Add(this.txtLineLength);
            this.groupBox2.Controls.Add(this.lblLineLength);
            this.groupBox2.Controls.Add(this.txtTextAngle);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtTextHeight);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(263, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 244);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Yazı";
            // 
            // cbAutoRotateText
            // 
            this.cbAutoRotateText.AutoSize = true;
            this.cbAutoRotateText.Location = new System.Drawing.Point(20, 102);
            this.cbAutoRotateText.Name = "cbAutoRotateText";
            this.cbAutoRotateText.Size = new System.Drawing.Size(157, 17);
            this.cbAutoRotateText.TabIndex = 4;
            this.cbAutoRotateText.Text = "Yazıyı çizgi yönünde döndür";
            this.cbAutoRotateText.UseVisualStyleBackColor = true;
            // 
            // cbZCoord
            // 
            this.cbZCoord.AutoSize = true;
            this.cbZCoord.Location = new System.Drawing.Point(20, 208);
            this.cbZCoord.Name = "cbZCoord";
            this.cbZCoord.Size = new System.Drawing.Size(120, 17);
            this.cbZCoord.TabIndex = 8;
            this.cbZCoord.Text = "Z koordinatını yazdır";
            this.cbZCoord.UseVisualStyleBackColor = true;
            // 
            // cbDirection
            // 
            this.cbDirection.AutoSize = true;
            this.cbDirection.Location = new System.Drawing.Point(20, 146);
            this.cbDirection.Name = "cbDirection";
            this.cbDirection.Size = new System.Drawing.Size(120, 17);
            this.cbDirection.TabIndex = 5;
            this.cbDirection.Text = "Yön belirleyerek yaz";
            this.cbDirection.UseVisualStyleBackColor = true;
            // 
            // txtLineLength
            // 
            this.txtLineLength.Location = new System.Drawing.Point(139, 169);
            this.txtLineLength.Name = "txtLineLength";
            this.txtLineLength.Size = new System.Drawing.Size(84, 20);
            this.txtLineLength.TabIndex = 7;
            // 
            // lblLineLength
            // 
            this.lblLineLength.AutoSize = true;
            this.lblLineLength.Location = new System.Drawing.Point(33, 172);
            this.lblLineLength.Name = "lblLineLength";
            this.lblLineLength.Size = new System.Drawing.Size(75, 13);
            this.lblLineLength.TabIndex = 6;
            this.lblLineLength.Text = "Çizgi uzunluğu";
            // 
            // txtTextAngle
            // 
            this.txtTextAngle.Location = new System.Drawing.Point(123, 73);
            this.txtTextAngle.Name = "txtTextAngle";
            this.txtTextAngle.Size = new System.Drawing.Size(100, 20);
            this.txtTextAngle.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Yazı açısı";
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
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Yazı yüksekliği";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Yazı stili";
            // 
            // lblPrecision
            // 
            this.lblPrecision.AutoSize = true;
            this.lblPrecision.Location = new System.Drawing.Point(17, 61);
            this.lblPrecision.Name = "lblPrecision";
            this.lblPrecision.Size = new System.Drawing.Size(79, 13);
            this.lblPrecision.TabIndex = 2;
            this.lblPrecision.Text = "Basamak sayısı";
            // 
            // cbTextStyle
            // 
            this.cbTextStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextStyle.FormattingEnabled = true;
            this.cbTextStyle.Location = new System.Drawing.Point(121, 31);
            this.cbTextStyle.Name = "cbTextStyle";
            this.cbTextStyle.Size = new System.Drawing.Size(100, 21);
            this.cbTextStyle.TabIndex = 1;
            // 
            // cbPrecision
            // 
            this.cbPrecision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrecision.FormattingEnabled = true;
            this.cbPrecision.Location = new System.Drawing.Point(121, 58);
            this.cbPrecision.Name = "cbPrecision";
            this.cbPrecision.Size = new System.Drawing.Size(100, 21);
            this.cbPrecision.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtPrefix);
            this.groupBox4.Controls.Add(this.lblPrefix);
            this.groupBox4.Controls.Add(this.txtStartNum);
            this.groupBox4.Controls.Add(this.lblStartNum);
            this.groupBox4.Controls.Add(this.rbNoNumbering);
            this.groupBox4.Controls.Add(this.rbAutoNumber);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(245, 140);
            this.groupBox4.TabIndex = 0;
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
            this.lblPrefix.Size = new System.Drawing.Size(36, 13);
            this.lblPrefix.TabIndex = 4;
            this.lblPrefix.Text = "Ön ek";
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
            this.lblStartNum.Size = new System.Drawing.Size(98, 13);
            this.lblStartNum.TabIndex = 2;
            this.lblStartNum.Text = "Başlangıç numarası";
            // 
            // rbNoNumbering
            // 
            this.rbNoNumbering.AutoSize = true;
            this.rbNoNumbering.Location = new System.Drawing.Point(20, 25);
            this.rbNoNumbering.Name = "rbNoNumbering";
            this.rbNoNumbering.Size = new System.Drawing.Size(121, 17);
            this.rbNoNumbering.TabIndex = 0;
            this.rbNoNumbering.TabStop = true;
            this.rbNoNumbering.Text = "Numaralandırma yok";
            this.rbNoNumbering.UseVisualStyleBackColor = true;
            // 
            // rbAutoNumber
            // 
            this.rbAutoNumber.AutoSize = true;
            this.rbAutoNumber.Location = new System.Drawing.Point(20, 51);
            this.rbAutoNumber.Name = "rbAutoNumber";
            this.rbAutoNumber.Size = new System.Drawing.Size(144, 17);
            this.rbAutoNumber.TabIndex = 1;
            this.rbAutoNumber.TabStop = true;
            this.rbAutoNumber.Text = "Otomatik numaralandırma";
            this.rbAutoNumber.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(435, 277);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 5;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(354, 277);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 4;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // btnReadCoords
            // 
            this.btnReadCoords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReadCoords.Image = global::XCOM.Properties.Resources.pick;
            this.btnReadCoords.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnReadCoords.Location = new System.Drawing.Point(12, 277);
            this.btnReadCoords.Name = "btnReadCoords";
            this.btnReadCoords.Size = new System.Drawing.Size(112, 23);
            this.btnReadCoords.TabIndex = 3;
            this.btnReadCoords.Text = "Çizimden Oku";
            this.btnReadCoords.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReadCoords.UseVisualStyleBackColor = true;
            this.btnReadCoords.Click += new System.EventHandler(this.btnReadCoords_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cbTextStyle);
            this.groupBox1.Controls.Add(this.lblPrecision);
            this.groupBox1.Controls.Add(this.cbPrecision);
            this.groupBox1.Location = new System.Drawing.Point(12, 158);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 98);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Yazı Stili";
            // 
            // CoordMainForm
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(522, 312);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.btnReadCoords);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoordMainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Koordinat";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private AcadUtility.WinForms.PrecisionComboBox cbPrecision;
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
        private AcadUtility.WinForms.TextStyleComboBox cbTextStyle;
        private System.Windows.Forms.Button btnReadCoords;
        private System.Windows.Forms.CheckBox cbAutoRotateText;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbZCoord;
    }
}