namespace XCOM.Forms
{
    partial class CoordMain
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbUnitMeter = new System.Windows.Forms.RadioButton();
            this.rbUnitCentimeter = new System.Windows.Forms.RadioButton();
            this.rbUnitMilimeter = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtTextHeight = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTextAngle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLineLength = new System.Windows.Forms.TextBox();
            this.cbDirection = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cbPrecision = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbUCS = new System.Windows.Forms.RadioButton();
            this.rbWCS = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbUnitMilimeter);
            this.groupBox1.Controls.Add(this.rbUnitCentimeter);
            this.groupBox1.Controls.Add(this.rbUnitMeter);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 110);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Çizim Birimi";
            // 
            // rbUnitMeter
            // 
            this.rbUnitMeter.AutoSize = true;
            this.rbUnitMeter.Location = new System.Drawing.Point(20, 25);
            this.rbUnitMeter.Name = "rbUnitMeter";
            this.rbUnitMeter.Size = new System.Drawing.Size(52, 17);
            this.rbUnitMeter.TabIndex = 0;
            this.rbUnitMeter.TabStop = true;
            this.rbUnitMeter.Text = "Metre";
            this.rbUnitMeter.UseVisualStyleBackColor = true;
            // 
            // rbUnitCentimeter
            // 
            this.rbUnitCentimeter.AutoSize = true;
            this.rbUnitCentimeter.Location = new System.Drawing.Point(20, 48);
            this.rbUnitCentimeter.Name = "rbUnitCentimeter";
            this.rbUnitCentimeter.Size = new System.Drawing.Size(75, 17);
            this.rbUnitCentimeter.TabIndex = 0;
            this.rbUnitCentimeter.TabStop = true;
            this.rbUnitCentimeter.Text = "Santimetre";
            this.rbUnitCentimeter.UseVisualStyleBackColor = true;
            // 
            // rbUnitMilimeter
            // 
            this.rbUnitMilimeter.AutoSize = true;
            this.rbUnitMilimeter.Location = new System.Drawing.Point(20, 71);
            this.rbUnitMilimeter.Name = "rbUnitMilimeter";
            this.rbUnitMilimeter.Size = new System.Drawing.Size(66, 17);
            this.rbUnitMilimeter.TabIndex = 0;
            this.rbUnitMilimeter.TabStop = true;
            this.rbUnitMilimeter.Text = "Milimetre";
            this.rbUnitMilimeter.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbDirection);
            this.groupBox2.Controls.Add(this.txtLineLength);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtTextAngle);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtTextHeight);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(263, 157);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(245, 139);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Yazı";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Yazı Yüksekliği";
            // 
            // txtTextHeight
            // 
            this.txtTextHeight.Location = new System.Drawing.Point(123, 24);
            this.txtTextHeight.Name = "txtTextHeight";
            this.txtTextHeight.Size = new System.Drawing.Size(100, 20);
            this.txtTextHeight.TabIndex = 3;
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
            // txtTextAngle
            // 
            this.txtTextAngle.Location = new System.Drawing.Point(123, 50);
            this.txtTextAngle.Name = "txtTextAngle";
            this.txtTextAngle.Size = new System.Drawing.Size(100, 20);
            this.txtTextAngle.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Çizgi Uzunluğu";
            // 
            // txtLineLength
            // 
            this.txtLineLength.Location = new System.Drawing.Point(139, 99);
            this.txtLineLength.Name = "txtLineLength";
            this.txtLineLength.Size = new System.Drawing.Size(84, 20);
            this.txtLineLength.TabIndex = 3;
            // 
            // cbDirection
            // 
            this.cbDirection.AutoSize = true;
            this.cbDirection.Location = new System.Drawing.Point(20, 76);
            this.cbDirection.Name = "cbDirection";
            this.cbDirection.Size = new System.Drawing.Size(123, 17);
            this.cbDirection.TabIndex = 4;
            this.cbDirection.Text = "Yön Belirleyerek Yaz";
            this.cbDirection.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbPrecision);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(12, 214);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(245, 60);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Yuvarlama";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Basamak Sayısı";
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
            this.cbPrecision.Location = new System.Drawing.Point(123, 24);
            this.cbPrecision.Name = "cbPrecision";
            this.cbPrecision.Size = new System.Drawing.Size(100, 21);
            this.cbPrecision.TabIndex = 4;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox2);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.textBox1);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.radioButton1);
            this.groupBox4.Controls.Add(this.radioButton3);
            this.groupBox4.Location = new System.Drawing.Point(263, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(245, 139);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Numaralandırma";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(20, 53);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(146, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Otomatik Numaralandırma";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(20, 30);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(123, 17);
            this.radioButton3.TabIndex = 0;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Numaralandırma Yok";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(139, 78);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(84, 20);
            this.textBox1.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Başlangıç Numarası";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Ön Ek";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(139, 104);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(84, 20);
            this.textBox2.TabIndex = 5;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(433, 312);
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
            this.cmdOK.Location = new System.Drawing.Point(352, 312);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.rbUCS);
            this.groupBox5.Controls.Add(this.rbWCS);
            this.groupBox5.Location = new System.Drawing.Point(12, 128);
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
            this.rbUCS.TabIndex = 0;
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
            // CoordMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(520, 347);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CoordMain";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Koordinat";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbUnitMilimeter;
        private System.Windows.Forms.RadioButton rbUnitCentimeter;
        private System.Windows.Forms.RadioButton rbUnitMeter;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbDirection;
        private System.Windows.Forms.TextBox txtLineLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTextAngle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTextHeight;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cbPrecision;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rbUCS;
        private System.Windows.Forms.RadioButton rbWCS;
    }
}