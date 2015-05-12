namespace LevelLabel
{
    partial class LevelMainForm
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
            this.txtScale = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rbMillimeter = new System.Windows.Forms.RadioButton();
            this.cbBlock = new System.Windows.Forms.ComboBox();
            this.rbCentimeter = new System.Windows.Forms.RadioButton();
            this.cbPrecision = new System.Windows.Forms.ComboBox();
            this.rbMeter = new System.Windows.Forms.RadioButton();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBaseLevel = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtZ = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnPickBasePoint = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtScale);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.rbMillimeter);
            this.groupBox2.Controls.Add(this.cbBlock);
            this.groupBox2.Controls.Add(this.rbCentimeter);
            this.groupBox2.Controls.Add(this.cbPrecision);
            this.groupBox2.Controls.Add(this.rbMeter);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(221, 206);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Çizim Birimi";
            // 
            // txtScale
            // 
            this.txtScale.Location = new System.Drawing.Point(103, 165);
            this.txtScale.Name = "txtScale";
            this.txtScale.Size = new System.Drawing.Size(100, 20);
            this.txtScale.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(16, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Blok Ölçeği";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 141);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Kot Bloğu";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Basamak Sayısı";
            // 
            // rbMillimeter
            // 
            this.rbMillimeter.AutoSize = true;
            this.rbMillimeter.Location = new System.Drawing.Point(19, 72);
            this.rbMillimeter.Name = "rbMillimeter";
            this.rbMillimeter.Size = new System.Drawing.Size(66, 17);
            this.rbMillimeter.TabIndex = 2;
            this.rbMillimeter.TabStop = true;
            this.rbMillimeter.Text = "Milimetre";
            this.rbMillimeter.UseVisualStyleBackColor = true;
            // 
            // cbBlock
            // 
            this.cbBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBlock.FormattingEnabled = true;
            this.cbBlock.Location = new System.Drawing.Point(103, 138);
            this.cbBlock.Name = "cbBlock";
            this.cbBlock.Size = new System.Drawing.Size(100, 21);
            this.cbBlock.TabIndex = 6;
            // 
            // rbCentimeter
            // 
            this.rbCentimeter.AutoSize = true;
            this.rbCentimeter.Location = new System.Drawing.Point(19, 49);
            this.rbCentimeter.Name = "rbCentimeter";
            this.rbCentimeter.Size = new System.Drawing.Size(75, 17);
            this.rbCentimeter.TabIndex = 1;
            this.rbCentimeter.TabStop = true;
            this.rbCentimeter.Text = "Santimetre";
            this.rbCentimeter.UseVisualStyleBackColor = true;
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
            this.cbPrecision.Location = new System.Drawing.Point(103, 99);
            this.cbPrecision.Name = "cbPrecision";
            this.cbPrecision.Size = new System.Drawing.Size(100, 21);
            this.cbPrecision.TabIndex = 4;
            // 
            // rbMeter
            // 
            this.rbMeter.AutoSize = true;
            this.rbMeter.Location = new System.Drawing.Point(19, 26);
            this.rbMeter.Name = "rbMeter";
            this.rbMeter.Size = new System.Drawing.Size(52, 17);
            this.rbMeter.TabIndex = 0;
            this.rbMeter.TabStop = true;
            this.rbMeter.Text = "Metre";
            this.rbMeter.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(361, 233);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(280, 233);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBaseLevel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtZ);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtY);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtX);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnPickBasePoint);
            this.groupBox1.Location = new System.Drawing.Point(239, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(194, 206);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Baz Noktası";
            // 
            // txtBaseLevel
            // 
            this.txtBaseLevel.Location = new System.Drawing.Point(73, 138);
            this.txtBaseLevel.Name = "txtBaseLevel";
            this.txtBaseLevel.Size = new System.Drawing.Size(100, 20);
            this.txtBaseLevel.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 142);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Baz Kotu";
            // 
            // txtZ
            // 
            this.txtZ.Location = new System.Drawing.Point(73, 99);
            this.txtZ.Name = "txtZ";
            this.txtZ.ReadOnly = true;
            this.txtZ.Size = new System.Drawing.Size(100, 20);
            this.txtZ.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Z";
            // 
            // txtY
            // 
            this.txtY.Location = new System.Drawing.Point(73, 73);
            this.txtY.Name = "txtY";
            this.txtY.ReadOnly = true;
            this.txtY.Size = new System.Drawing.Size(100, 20);
            this.txtY.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Y";
            // 
            // txtX
            // 
            this.txtX.Location = new System.Drawing.Point(73, 47);
            this.txtX.Name = "txtX";
            this.txtX.ReadOnly = true;
            this.txtX.Size = new System.Drawing.Size(100, 20);
            this.txtX.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "X";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Baz Noktasını Seç";
            // 
            // btnPickBasePoint
            // 
            this.btnPickBasePoint.Image = global::XCOM.Properties.Resources.pick;
            this.btnPickBasePoint.Location = new System.Drawing.Point(18, 23);
            this.btnPickBasePoint.Name = "btnPickBasePoint";
            this.btnPickBasePoint.Size = new System.Drawing.Size(23, 23);
            this.btnPickBasePoint.TabIndex = 0;
            this.btnPickBasePoint.UseVisualStyleBackColor = true;
            this.btnPickBasePoint.Click += new System.EventHandler(this.btnPickBasePoint_Click);
            // 
            // LevelMainForm
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(448, 268);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LevelMainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Level";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbPrecision;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbMeter;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.RadioButton rbMillimeter;
        private System.Windows.Forms.RadioButton rbCentimeter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPickBasePoint;
        private System.Windows.Forms.TextBox txtBaseLevel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtZ;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbBlock;
        private System.Windows.Forms.TextBox txtScale;
        private System.Windows.Forms.Label label8;
    }
}