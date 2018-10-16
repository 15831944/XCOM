namespace XCOM.Commands.Annotation
{
    partial class SelectObjectsForm
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
            this.rbSelectCircle = new System.Windows.Forms.RadioButton();
            this.rbSelectPolyline = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rbSelectBlock = new System.Windows.Forms.RadioButton();
            this.rbSelectLine = new System.Windows.Forms.RadioButton();
            this.rbSelectPoint = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbOrderXDec = new System.Windows.Forms.RadioButton();
            this.rbOrderYInc = new System.Windows.Forms.RadioButton();
            this.rbOrderXInc = new System.Windows.Forms.RadioButton();
            this.rbOrderYDec = new System.Windows.Forms.RadioButton();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(320, 177);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 3;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(239, 177);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 2;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            // 
            // rbSelectCircle
            // 
            this.rbSelectCircle.AutoSize = true;
            this.rbSelectCircle.Location = new System.Drawing.Point(20, 48);
            this.rbSelectCircle.Name = "rbSelectCircle";
            this.rbSelectCircle.Size = new System.Drawing.Size(112, 17);
            this.rbSelectCircle.TabIndex = 1;
            this.rbSelectCircle.Text = "Circle (Arc, Ellipse)";
            this.rbSelectCircle.UseVisualStyleBackColor = true;
            // 
            // rbSelectPolyline
            // 
            this.rbSelectPolyline.AutoSize = true;
            this.rbSelectPolyline.Checked = true;
            this.rbSelectPolyline.Location = new System.Drawing.Point(20, 25);
            this.rbSelectPolyline.Name = "rbSelectPolyline";
            this.rbSelectPolyline.Size = new System.Drawing.Size(61, 17);
            this.rbSelectPolyline.TabIndex = 0;
            this.rbSelectPolyline.TabStop = true;
            this.rbSelectPolyline.Text = "Polyline";
            this.rbSelectPolyline.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rbSelectBlock);
            this.groupBox4.Controls.Add(this.rbSelectLine);
            this.groupBox4.Controls.Add(this.rbSelectPoint);
            this.groupBox4.Controls.Add(this.rbSelectPolyline);
            this.groupBox4.Controls.Add(this.rbSelectCircle);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(188, 147);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Seçilecek Nesneler";
            // 
            // rbSelectBlock
            // 
            this.rbSelectBlock.AutoSize = true;
            this.rbSelectBlock.Location = new System.Drawing.Point(20, 71);
            this.rbSelectBlock.Name = "rbSelectBlock";
            this.rbSelectBlock.Size = new System.Drawing.Size(52, 17);
            this.rbSelectBlock.TabIndex = 2;
            this.rbSelectBlock.Text = "Block";
            this.rbSelectBlock.UseVisualStyleBackColor = true;
            // 
            // rbSelectLine
            // 
            this.rbSelectLine.AutoSize = true;
            this.rbSelectLine.Location = new System.Drawing.Point(20, 117);
            this.rbSelectLine.Name = "rbSelectLine";
            this.rbSelectLine.Size = new System.Drawing.Size(45, 17);
            this.rbSelectLine.TabIndex = 4;
            this.rbSelectLine.Text = "Line";
            this.rbSelectLine.UseVisualStyleBackColor = true;
            // 
            // rbSelectPoint
            // 
            this.rbSelectPoint.AutoSize = true;
            this.rbSelectPoint.Location = new System.Drawing.Point(20, 94);
            this.rbSelectPoint.Name = "rbSelectPoint";
            this.rbSelectPoint.Size = new System.Drawing.Size(49, 17);
            this.rbSelectPoint.TabIndex = 3;
            this.rbSelectPoint.Text = "Point";
            this.rbSelectPoint.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbOrderXDec);
            this.groupBox1.Controls.Add(this.rbOrderYInc);
            this.groupBox1.Controls.Add(this.rbOrderXInc);
            this.groupBox1.Controls.Add(this.rbOrderYDec);
            this.groupBox1.Location = new System.Drawing.Point(206, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(188, 147);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sıralama";
            // 
            // rbOrderXDec
            // 
            this.rbOrderXDec.AutoSize = true;
            this.rbOrderXDec.Location = new System.Drawing.Point(20, 71);
            this.rbOrderXDec.Name = "rbOrderXDec";
            this.rbOrderXDec.Size = new System.Drawing.Size(106, 17);
            this.rbOrderXDec.TabIndex = 2;
            this.rbOrderXDec.Text = "Sağdan sola (- X)";
            this.rbOrderXDec.UseVisualStyleBackColor = true;
            // 
            // rbOrderYInc
            // 
            this.rbOrderYInc.AutoSize = true;
            this.rbOrderYInc.Location = new System.Drawing.Point(20, 94);
            this.rbOrderYInc.Name = "rbOrderYInc";
            this.rbOrderYInc.Size = new System.Drawing.Size(136, 17);
            this.rbOrderYInc.TabIndex = 3;
            this.rbOrderYInc.Text = "Aşağıdan yukarıya (+ Y)";
            this.rbOrderYInc.UseVisualStyleBackColor = true;
            // 
            // rbOrderXInc
            // 
            this.rbOrderXInc.AutoSize = true;
            this.rbOrderXInc.Checked = true;
            this.rbOrderXInc.Location = new System.Drawing.Point(20, 25);
            this.rbOrderXInc.Name = "rbOrderXInc";
            this.rbOrderXInc.Size = new System.Drawing.Size(109, 17);
            this.rbOrderXInc.TabIndex = 0;
            this.rbOrderXInc.TabStop = true;
            this.rbOrderXInc.Text = "Soldan sağa (+ X)";
            this.rbOrderXInc.UseVisualStyleBackColor = true;
            // 
            // rbOrderYDec
            // 
            this.rbOrderYDec.AutoSize = true;
            this.rbOrderYDec.Location = new System.Drawing.Point(20, 48);
            this.rbOrderYDec.Name = "rbOrderYDec";
            this.rbOrderYDec.Size = new System.Drawing.Size(134, 17);
            this.rbOrderYDec.TabIndex = 1;
            this.rbOrderYDec.Text = "Yukarıdan aşağıya (- Y)";
            this.rbOrderYDec.UseVisualStyleBackColor = true;
            // 
            // SelectObjectsForm
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(407, 212);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectObjectsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nesne Seç";
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.RadioButton rbSelectCircle;
        private System.Windows.Forms.RadioButton rbSelectPolyline;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rbSelectBlock;
        private System.Windows.Forms.RadioButton rbSelectLine;
        private System.Windows.Forms.RadioButton rbSelectPoint;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbOrderXDec;
        private System.Windows.Forms.RadioButton rbOrderYInc;
        private System.Windows.Forms.RadioButton rbOrderXInc;
        private System.Windows.Forms.RadioButton rbOrderYDec;
    }
}