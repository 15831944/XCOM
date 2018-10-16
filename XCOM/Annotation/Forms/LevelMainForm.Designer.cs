namespace XCOM.Commands.Annotation
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
            this.cbBlock = new AcadUtility.WinForms.BlockComboBox();
            this.rbCentimeter = new System.Windows.Forms.RadioButton();
            this.cbPrecision = new AcadUtility.WinForms.PrecisionComboBox();
            this.rbMeter = new System.Windows.Forms.RadioButton();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pickBasePoint = new AcadUtility.WinForms.PickCoordinateControl();
            this.txtMultiplier = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBaseLevel = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
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
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Blok ölçeği";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 141);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Kot boğu";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Basamak sayısı";
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
            this.cbBlock.Database = null;
            this.cbBlock.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbBlock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBlock.FormattingEnabled = true;
            this.cbBlock.HasIcons = true;
            this.cbBlock.Location = new System.Drawing.Point(103, 138);
            this.cbBlock.Name = "cbBlock";
            this.cbBlock.Size = new System.Drawing.Size(100, 21);
            this.cbBlock.TabIndex = 6;
            this.cbBlock.Text = null;
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
            this.cbPrecision.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbPrecision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrecision.DropDownWidth = 121;
            this.cbPrecision.FormattingEnabled = true;
            this.cbPrecision.Items.AddRange(new object[] {
            "0 - 0",
            "1 - 0.0",
            "2 - 0.00",
            "3 - 0.000",
            "4 - 0.0000",
            "5 - 0.00000",
            "6 - 0.000000",
            "7 - 0.0000000",
            "8 - 0.00000000",
            "0 - 0",
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
            this.cbPrecision.Precision = 0;
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
            this.groupBox1.Controls.Add(this.pickBasePoint);
            this.groupBox1.Controls.Add(this.txtMultiplier);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtBaseLevel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(239, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(194, 206);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Baz Noktası";
            // 
            // pickBasePoint
            // 
            this.pickBasePoint.DividerLocation = 53;
            this.pickBasePoint.Location = new System.Drawing.Point(20, 26);
            this.pickBasePoint.Name = "pickBasePoint";
            this.pickBasePoint.Size = new System.Drawing.Size(153, 96);
            this.pickBasePoint.TabIndex = 0;
            // 
            // txtMultiplier
            // 
            this.txtMultiplier.Location = new System.Drawing.Point(73, 165);
            this.txtMultiplier.Name = "txtMultiplier";
            this.txtMultiplier.Size = new System.Drawing.Size(100, 20);
            this.txtMultiplier.TabIndex = 4;
            this.txtMultiplier.TextChanged += new System.EventHandler(this.txtMultiplier_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 169);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Çarpan";
            // 
            // txtBaseLevel
            // 
            this.txtBaseLevel.Location = new System.Drawing.Point(73, 138);
            this.txtBaseLevel.Name = "txtBaseLevel";
            this.txtBaseLevel.Size = new System.Drawing.Size(100, 20);
            this.txtBaseLevel.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 142);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Baz kotu";
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
        private AcadUtility.WinForms.PrecisionComboBox cbPrecision;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbMeter;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.RadioButton rbMillimeter;
        private System.Windows.Forms.RadioButton rbCentimeter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBaseLevel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private AcadUtility.WinForms.BlockComboBox cbBlock;
        private System.Windows.Forms.TextBox txtScale;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtMultiplier;
        private System.Windows.Forms.Label label9;
        private AcadUtility.WinForms.PickCoordinateControl pickBasePoint;
    }
}