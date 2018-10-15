namespace XCOM.Commands.Bridge
{
    partial class DrawAxesForm
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
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pnlBlock = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.lblPrecision = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbPrecision = new AcadUtility.WinForms.PrecisionComboBox();
            this.txtAxisAttribute = new System.Windows.Forms.TextBox();
            this.cbBlockName = new AcadUtility.WinForms.BlockComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtChPrefix = new System.Windows.Forms.TextBox();
            this.txtChAttribute = new System.Windows.Forms.TextBox();
            this.pnlDraw = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTextHeight = new System.Windows.Forms.TextBox();
            this.cbTextStyle = new AcadUtility.WinForms.TextStyleComboBox();
            this.rbAxisLine = new System.Windows.Forms.RadioButton();
            this.rbAxisBlock = new System.Windows.Forms.RadioButton();
            this.txtAxisLineLength = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.pnlBlock.SuspendLayout();
            this.pnlDraw.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(240, 315);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(159, 315);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.pnlBlock);
            this.groupBox3.Controls.Add(this.pnlDraw);
            this.groupBox3.Controls.Add(this.rbAxisLine);
            this.groupBox3.Controls.Add(this.rbAxisBlock);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(301, 286);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Seçenekler";
            // 
            // pnlBlock
            // 
            this.pnlBlock.Controls.Add(this.label5);
            this.pnlBlock.Controls.Add(this.lblPrecision);
            this.pnlBlock.Controls.Add(this.label2);
            this.pnlBlock.Controls.Add(this.cbPrecision);
            this.pnlBlock.Controls.Add(this.txtAxisAttribute);
            this.pnlBlock.Controls.Add(this.cbBlockName);
            this.pnlBlock.Controls.Add(this.label6);
            this.pnlBlock.Controls.Add(this.label4);
            this.pnlBlock.Controls.Add(this.txtChPrefix);
            this.pnlBlock.Controls.Add(this.txtChAttribute);
            this.pnlBlock.Location = new System.Drawing.Point(40, 153);
            this.pnlBlock.Name = "pnlBlock";
            this.pnlBlock.Size = new System.Drawing.Size(242, 130);
            this.pnlBlock.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(0, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Blok adı";
            // 
            // lblPrecision
            // 
            this.lblPrecision.AutoSize = true;
            this.lblPrecision.Location = new System.Drawing.Point(0, 108);
            this.lblPrecision.Name = "lblPrecision";
            this.lblPrecision.Size = new System.Drawing.Size(97, 13);
            this.lblPrecision.TabIndex = 8;
            this.lblPrecision.Text = "KM basamak sayısı";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Aks attribute";
            // 
            // cbPrecision
            // 
            this.cbPrecision.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbPrecision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrecision.FormattingEnabled = true;
            this.cbPrecision.Location = new System.Drawing.Point(107, 105);
            this.cbPrecision.Name = "cbPrecision";
            this.cbPrecision.Size = new System.Drawing.Size(132, 21);
            this.cbPrecision.TabIndex = 9;
            // 
            // txtAxisAttribute
            // 
            this.txtAxisAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAxisAttribute.Location = new System.Drawing.Point(107, 27);
            this.txtAxisAttribute.Name = "txtAxisAttribute";
            this.txtAxisAttribute.Size = new System.Drawing.Size(132, 20);
            this.txtAxisAttribute.TabIndex = 3;
            // 
            // cbBlockName
            // 
            this.cbBlockName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBlockName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBlockName.FormattingEnabled = true;
            this.cbBlockName.Location = new System.Drawing.Point(107, 0);
            this.cbBlockName.Name = "cbBlockName";
            this.cbBlockName.Size = new System.Drawing.Size(132, 21);
            this.cbBlockName.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(0, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "KM ön eki";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "KM attribute";
            // 
            // txtChPrefix
            // 
            this.txtChPrefix.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChPrefix.Location = new System.Drawing.Point(107, 79);
            this.txtChPrefix.Name = "txtChPrefix";
            this.txtChPrefix.Size = new System.Drawing.Size(132, 20);
            this.txtChPrefix.TabIndex = 7;
            // 
            // txtChAttribute
            // 
            this.txtChAttribute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChAttribute.Location = new System.Drawing.Point(107, 53);
            this.txtChAttribute.Name = "txtChAttribute";
            this.txtChAttribute.Size = new System.Drawing.Size(132, 20);
            this.txtChAttribute.TabIndex = 5;
            // 
            // pnlDraw
            // 
            this.pnlDraw.Controls.Add(this.label7);
            this.pnlDraw.Controls.Add(this.label1);
            this.pnlDraw.Controls.Add(this.label3);
            this.pnlDraw.Controls.Add(this.txtAxisLineLength);
            this.pnlDraw.Controls.Add(this.txtTextHeight);
            this.pnlDraw.Controls.Add(this.cbTextStyle);
            this.pnlDraw.Location = new System.Drawing.Point(40, 48);
            this.pnlDraw.Name = "pnlDraw";
            this.pnlDraw.Size = new System.Drawing.Size(242, 76);
            this.pnlDraw.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Yazı yüksekliği";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Yazı stili";
            // 
            // txtTextHeight
            // 
            this.txtTextHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTextHeight.Location = new System.Drawing.Point(107, 26);
            this.txtTextHeight.Name = "txtTextHeight";
            this.txtTextHeight.Size = new System.Drawing.Size(132, 20);
            this.txtTextHeight.TabIndex = 3;
            // 
            // cbTextStyle
            // 
            this.cbTextStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTextStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTextStyle.FormattingEnabled = true;
            this.cbTextStyle.Location = new System.Drawing.Point(107, 52);
            this.cbTextStyle.Name = "cbTextStyle";
            this.cbTextStyle.Size = new System.Drawing.Size(132, 21);
            this.cbTextStyle.TabIndex = 5;
            // 
            // rbAxisLine
            // 
            this.rbAxisLine.AutoSize = true;
            this.rbAxisLine.Checked = true;
            this.rbAxisLine.Location = new System.Drawing.Point(20, 25);
            this.rbAxisLine.Name = "rbAxisLine";
            this.rbAxisLine.Size = new System.Drawing.Size(113, 17);
            this.rbAxisLine.TabIndex = 0;
            this.rbAxisLine.TabStop = true;
            this.rbAxisLine.Text = "Sadece aks çizgisi";
            this.rbAxisLine.UseVisualStyleBackColor = true;
            this.rbAxisLine.CheckedChanged += new System.EventHandler(this.DrawingType_Check_Changed);
            // 
            // rbAxisBlock
            // 
            this.rbAxisBlock.AutoSize = true;
            this.rbAxisBlock.Location = new System.Drawing.Point(20, 130);
            this.rbAxisBlock.Name = "rbAxisBlock";
            this.rbAxisBlock.Size = new System.Drawing.Size(72, 17);
            this.rbAxisBlock.TabIndex = 2;
            this.rbAxisBlock.Text = "Aks bloğu";
            this.rbAxisBlock.UseVisualStyleBackColor = true;
            this.rbAxisBlock.CheckedChanged += new System.EventHandler(this.DrawingType_Check_Changed);
            // 
            // txtAxisLineLength
            // 
            this.txtAxisLineLength.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAxisLineLength.Location = new System.Drawing.Point(107, 0);
            this.txtAxisLineLength.Name = "txtAxisLineLength";
            this.txtAxisLineLength.Size = new System.Drawing.Size(132, 20);
            this.txtAxisLineLength.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Çizgi uzunluğu";
            // 
            // DrawAxesForm
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(327, 350);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DrawAxesForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Aks Çizimi";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.pnlBlock.ResumeLayout(false);
            this.pnlBlock.PerformLayout();
            this.pnlDraw.ResumeLayout(false);
            this.pnlDraw.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbAxisLine;
        private System.Windows.Forms.RadioButton rbAxisBlock;
        private AcadUtility.WinForms.BlockComboBox cbBlockName;
        private System.Windows.Forms.Label label3;
        private AcadUtility.WinForms.TextStyleComboBox cbTextStyle;
        private System.Windows.Forms.Label lblPrecision;
        private AcadUtility.WinForms.PrecisionComboBox cbPrecision;
        private System.Windows.Forms.TextBox txtTextHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlBlock;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAxisAttribute;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtChAttribute;
        private System.Windows.Forms.Panel pnlDraw;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtChPrefix;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAxisLineLength;
    }
}