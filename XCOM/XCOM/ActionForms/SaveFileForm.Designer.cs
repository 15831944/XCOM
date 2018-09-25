namespace XCOM.Commands.XCommand
{
    partial class SaveFileForm
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
            this.txSuffix = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbVersion = new System.Windows.Forms.ComboBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbChangeVersion = new System.Windows.Forms.RadioButton();
            this.rbKeepCurrentVersion = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txSuffix
            // 
            this.txSuffix.Location = new System.Drawing.Point(218, 102);
            this.txSuffix.Name = "txSuffix";
            this.txSuffix.Size = new System.Drawing.Size(135, 20);
            this.txSuffix.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(19, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Dosya adına ek";
            // 
            // cbVersion
            // 
            this.cbVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersion.FormattingEnabled = true;
            this.cbVersion.Items.AddRange(new object[] {
            "Autocad 2013",
            "Autocad 2010",
            "Autocad 2007",
            "Autocad 2004",
            "Autocad 2000",
            "Autocad R14"});
            this.cbVersion.Location = new System.Drawing.Point(201, 25);
            this.cbVersion.Name = "cbVersion";
            this.cbVersion.Size = new System.Drawing.Size(135, 21);
            this.cbVersion.TabIndex = 2;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(318, 177);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(237, 177);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 1;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txSuffix);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(378, 141);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Kaydetme Seçenekleri";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbVersion);
            this.panel1.Controls.Add(this.rbChangeVersion);
            this.panel1.Controls.Add(this.rbKeepCurrentVersion);
            this.panel1.Location = new System.Drawing.Point(17, 32);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(348, 54);
            this.panel1.TabIndex = 0;
            // 
            // rbChangeVersion
            // 
            this.rbChangeVersion.AutoSize = true;
            this.rbChangeVersion.Location = new System.Drawing.Point(5, 26);
            this.rbChangeVersion.Name = "rbChangeVersion";
            this.rbChangeVersion.Size = new System.Drawing.Size(151, 17);
            this.rbChangeVersion.TabIndex = 1;
            this.rbChangeVersion.Text = "Dosya versiyonunu değiştir";
            this.rbChangeVersion.UseVisualStyleBackColor = true;
            // 
            // rbKeepCurrentVersion
            // 
            this.rbKeepCurrentVersion.AutoSize = true;
            this.rbKeepCurrentVersion.Checked = true;
            this.rbKeepCurrentVersion.Location = new System.Drawing.Point(5, 3);
            this.rbKeepCurrentVersion.Name = "rbKeepCurrentVersion";
            this.rbKeepCurrentVersion.Size = new System.Drawing.Size(176, 17);
            this.rbKeepCurrentVersion.TabIndex = 0;
            this.rbKeepCurrentVersion.TabStop = true;
            this.rbKeepCurrentVersion.Text = "Mevcut dosya versiyonunu koru";
            this.rbKeepCurrentVersion.UseVisualStyleBackColor = true;
            // 
            // SaveFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(405, 212);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SaveFileForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kaydet";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txSuffix;
        private System.Windows.Forms.ComboBox cbVersion;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbChangeVersion;
        private System.Windows.Forms.RadioButton rbKeepCurrentVersion;
    }
}