namespace XCOM.Commands.RoadDesign
{
    partial class CreateSurfaceForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chSelectSolid = new System.Windows.Forms.CheckBox();
            this.chSelect3DFace = new System.Windows.Forms.CheckBox();
            this.chSelectBlock = new System.Windows.Forms.CheckBox();
            this.chSelectTextZ = new System.Windows.Forms.CheckBox();
            this.chSelectText = new System.Windows.Forms.CheckBox();
            this.chSelectPolyline = new System.Windows.Forms.CheckBox();
            this.chSelectLine = new System.Windows.Forms.CheckBox();
            this.chSelectPoint = new System.Windows.Forms.CheckBox();
            this.chEraseEntities = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(156, 286);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "İptal";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(75, 286);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Tamam";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chSelectSolid);
            this.groupBox1.Controls.Add(this.chSelect3DFace);
            this.groupBox1.Controls.Add(this.chSelectBlock);
            this.groupBox1.Controls.Add(this.chSelectTextZ);
            this.groupBox1.Controls.Add(this.chSelectText);
            this.groupBox1.Controls.Add(this.chSelectPolyline);
            this.groupBox1.Controls.Add(this.chSelectLine);
            this.groupBox1.Controls.Add(this.chSelectPoint);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 224);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seçilecek Nesneler";
            // 
            // chSelectSolid
            // 
            this.chSelectSolid.AutoSize = true;
            this.chSelectSolid.Location = new System.Drawing.Point(17, 190);
            this.chSelectSolid.Name = "chSelectSolid";
            this.chSelectSolid.Size = new System.Drawing.Size(49, 17);
            this.chSelectSolid.TabIndex = 7;
            this.chSelectSolid.Text = "Solid";
            this.chSelectSolid.UseVisualStyleBackColor = true;
            // 
            // chSelect3DFace
            // 
            this.chSelect3DFace.AutoSize = true;
            this.chSelect3DFace.Location = new System.Drawing.Point(17, 167);
            this.chSelect3DFace.Name = "chSelect3DFace";
            this.chSelect3DFace.Size = new System.Drawing.Size(64, 17);
            this.chSelect3DFace.TabIndex = 6;
            this.chSelect3DFace.Text = "3DFace";
            this.chSelect3DFace.UseVisualStyleBackColor = true;
            // 
            // chSelectBlock
            // 
            this.chSelectBlock.AutoSize = true;
            this.chSelectBlock.Location = new System.Drawing.Point(17, 144);
            this.chSelectBlock.Name = "chSelectBlock";
            this.chSelectBlock.Size = new System.Drawing.Size(53, 17);
            this.chSelectBlock.TabIndex = 5;
            this.chSelectBlock.Text = "Block";
            this.chSelectBlock.UseVisualStyleBackColor = true;
            // 
            // chSelectTextZ
            // 
            this.chSelectTextZ.AutoSize = true;
            this.chSelectTextZ.Location = new System.Drawing.Point(17, 121);
            this.chSelectTextZ.Name = "chSelectTextZ";
            this.chSelectTextZ.Size = new System.Drawing.Size(151, 17);
            this.chSelectTextZ.TabIndex = 4;
            this.chSelectTextZ.Text = "Text (Z koordinatı yazıdan)";
            this.chSelectTextZ.UseVisualStyleBackColor = true;
            // 
            // chSelectText
            // 
            this.chSelectText.AutoSize = true;
            this.chSelectText.Location = new System.Drawing.Point(17, 98);
            this.chSelectText.Name = "chSelectText";
            this.chSelectText.Size = new System.Drawing.Size(47, 17);
            this.chSelectText.TabIndex = 3;
            this.chSelectText.Text = "Text";
            this.chSelectText.UseVisualStyleBackColor = true;
            // 
            // chSelectPolyline
            // 
            this.chSelectPolyline.AutoSize = true;
            this.chSelectPolyline.Location = new System.Drawing.Point(17, 75);
            this.chSelectPolyline.Name = "chSelectPolyline";
            this.chSelectPolyline.Size = new System.Drawing.Size(62, 17);
            this.chSelectPolyline.TabIndex = 2;
            this.chSelectPolyline.Text = "Polyline";
            this.chSelectPolyline.UseVisualStyleBackColor = true;
            // 
            // chSelectLine
            // 
            this.chSelectLine.AutoSize = true;
            this.chSelectLine.Location = new System.Drawing.Point(17, 52);
            this.chSelectLine.Name = "chSelectLine";
            this.chSelectLine.Size = new System.Drawing.Size(46, 17);
            this.chSelectLine.TabIndex = 1;
            this.chSelectLine.Text = "Line";
            this.chSelectLine.UseVisualStyleBackColor = true;
            // 
            // chSelectPoint
            // 
            this.chSelectPoint.AutoSize = true;
            this.chSelectPoint.Checked = true;
            this.chSelectPoint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chSelectPoint.Location = new System.Drawing.Point(17, 29);
            this.chSelectPoint.Name = "chSelectPoint";
            this.chSelectPoint.Size = new System.Drawing.Size(50, 17);
            this.chSelectPoint.TabIndex = 0;
            this.chSelectPoint.Text = "Point";
            this.chSelectPoint.UseVisualStyleBackColor = true;
            // 
            // chEraseEntities
            // 
            this.chEraseEntities.AutoSize = true;
            this.chEraseEntities.Location = new System.Drawing.Point(12, 255);
            this.chEraseEntities.Name = "chEraseEntities";
            this.chEraseEntities.Size = new System.Drawing.Size(122, 17);
            this.chEraseEntities.TabIndex = 1;
            this.chEraseEntities.Text = "Seçilen Nesneleri Sil";
            this.chEraseEntities.UseVisualStyleBackColor = true;
            // 
            // CreateSurfaceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(243, 321);
            this.Controls.Add(this.chEraseEntities);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateSurfaceForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Arazi Yüzeyi Oluştur";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chSelectBlock;
        private System.Windows.Forms.CheckBox chSelectTextZ;
        private System.Windows.Forms.CheckBox chSelectText;
        private System.Windows.Forms.CheckBox chSelectPolyline;
        private System.Windows.Forms.CheckBox chSelectLine;
        private System.Windows.Forms.CheckBox chSelectPoint;
        private System.Windows.Forms.CheckBox chSelect3DFace;
        private System.Windows.Forms.CheckBox chSelectSolid;
        private System.Windows.Forms.CheckBox chEraseEntities;
    }
}