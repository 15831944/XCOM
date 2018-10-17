namespace XCOM.Commands.Annotation
{
    partial class SelectGroupForm
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
            this.lblPrefix = new System.Windows.Forms.Label();
            this.cbPrefix = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbXY = new System.Windows.Forms.RadioButton();
            this.rbPrefix = new System.Windows.Forms.RadioButton();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPrefix
            // 
            this.lblPrefix.AutoSize = true;
            this.lblPrefix.Location = new System.Drawing.Point(39, 77);
            this.lblPrefix.Name = "lblPrefix";
            this.lblPrefix.Size = new System.Drawing.Size(36, 13);
            this.lblPrefix.TabIndex = 2;
            this.lblPrefix.Text = "Ön ek";
            // 
            // cbPrefix
            // 
            this.cbPrefix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrefix.FormattingEnabled = true;
            this.cbPrefix.Location = new System.Drawing.Point(124, 74);
            this.cbPrefix.Name = "cbPrefix";
            this.cbPrefix.Size = new System.Drawing.Size(100, 21);
            this.cbPrefix.TabIndex = 3;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblPrefix);
            this.groupBox4.Controls.Add(this.rbXY);
            this.groupBox4.Controls.Add(this.rbPrefix);
            this.groupBox4.Controls.Add(this.cbPrefix);
            this.groupBox4.Location = new System.Drawing.Point(15, 57);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(245, 113);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Koordinat Grupları";
            // 
            // rbXY
            // 
            this.rbXY.AutoSize = true;
            this.rbXY.Location = new System.Drawing.Point(20, 25);
            this.rbXY.Name = "rbXY";
            this.rbXY.Size = new System.Drawing.Size(123, 17);
            this.rbXY.TabIndex = 0;
            this.rbXY.TabStop = true;
            this.rbXY.Text = "X Y koordinat yazıları";
            this.rbXY.UseVisualStyleBackColor = true;
            // 
            // rbPrefix
            // 
            this.rbPrefix.AutoSize = true;
            this.rbPrefix.Location = new System.Drawing.Point(20, 51);
            this.rbPrefix.Name = "rbPrefix";
            this.rbPrefix.Size = new System.Drawing.Size(139, 17);
            this.rbPrefix.TabIndex = 1;
            this.rbPrefix.TabStop = true;
            this.rbPrefix.Text = "Ön ekli koordinat yazıları";
            this.rbPrefix.UseVisualStyleBackColor = true;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(187, 186);
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
            this.cmdOK.Location = new System.Drawing.Point(106, 186);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(248, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "Seçilen yazılarda birden fazla koordinat grubu bulundu. Uygulanacak koordinat gru" +
    "bunu seçin.";
            // 
            // SelectGroupForm
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(274, 221);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectGroupForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Koordinat Grubu Seç";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbXY;
        private System.Windows.Forms.RadioButton rbPrefix;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.ComboBox cbPrefix;
        private System.Windows.Forms.Label label1;
    }
}