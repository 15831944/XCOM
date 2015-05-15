namespace XCOM.Commands.RoadDesign
{
    partial class DrawCulvertForm
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
            this.culvertGrid = new SourceGrid.Grid();
            this.cbDrawCulvertInfo = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBaseCH = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtScale = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtHatchScale = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtTextHeight = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtLayer = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(788, 494);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "İptal";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(707, 494);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Tamam";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // culvertGrid
            // 
            this.culvertGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.culvertGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.culvertGrid.ClipboardMode = ((SourceGrid.ClipboardMode)((((SourceGrid.ClipboardMode.Copy | SourceGrid.ClipboardMode.Cut) 
            | SourceGrid.ClipboardMode.Paste) 
            | SourceGrid.ClipboardMode.Delete)));
            this.culvertGrid.EnableSort = true;
            this.culvertGrid.Location = new System.Drawing.Point(12, 12);
            this.culvertGrid.Name = "culvertGrid";
            this.culvertGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.culvertGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.culvertGrid.Size = new System.Drawing.Size(617, 463);
            this.culvertGrid.TabIndex = 0;
            this.culvertGrid.TabStop = true;
            this.culvertGrid.ToolTipText = "";
            // 
            // cbDrawCulvertInfo
            // 
            this.cbDrawCulvertInfo.AutoSize = true;
            this.cbDrawCulvertInfo.Checked = true;
            this.cbDrawCulvertInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDrawCulvertInfo.Location = new System.Drawing.Point(18, 246);
            this.cbDrawCulvertInfo.Name = "cbDrawCulvertInfo";
            this.cbDrawCulvertInfo.Size = new System.Drawing.Size(157, 17);
            this.cbDrawCulvertInfo.TabIndex = 14;
            this.cbDrawCulvertInfo.Text = "Menfez Bilgilerini Profile Yaz";
            this.cbDrawCulvertInfo.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtBaseCH);
            this.groupBox1.Controls.Add(this.cbDrawCulvertInfo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtScale);
            this.groupBox1.Controls.Add(this.label7);
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
            this.groupBox1.Location = new System.Drawing.Point(646, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 280);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seçenekler";
            // 
            // txtBaseCH
            // 
            this.txtBaseCH.Location = new System.Drawing.Point(73, 140);
            this.txtBaseCH.Name = "txtBaseCH";
            this.txtBaseCH.Size = new System.Drawing.Size(128, 20);
            this.txtBaseCH.TabIndex = 9;
            this.txtBaseCH.Text = "0+000.00";
            this.txtBaseCH.Validating += new System.ComponentModel.CancelEventHandler(this.txtBaseCH_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Baz KM";
            // 
            // txtScale
            // 
            this.txtScale.Location = new System.Drawing.Point(73, 206);
            this.txtScale.Name = "txtScale";
            this.txtScale.Size = new System.Drawing.Size(128, 20);
            this.txtScale.TabIndex = 13;
            this.txtScale.Text = "10";
            this.txtScale.Validating += new System.ComponentModel.CancelEventHandler(this.txtScale_Validating);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Ölçek";
            // 
            // txtBaseLevel
            // 
            this.txtBaseLevel.Location = new System.Drawing.Point(73, 166);
            this.txtBaseLevel.Name = "txtBaseLevel";
            this.txtBaseLevel.Size = new System.Drawing.Size(128, 20);
            this.txtBaseLevel.TabIndex = 11;
            this.txtBaseLevel.Text = "0";
            this.txtBaseLevel.Validating += new System.ComponentModel.CancelEventHandler(this.txtBaseLevel_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Baz Kotu";
            // 
            // txtZ
            // 
            this.txtZ.Location = new System.Drawing.Point(73, 99);
            this.txtZ.Name = "txtZ";
            this.txtZ.ReadOnly = true;
            this.txtZ.Size = new System.Drawing.Size(128, 20);
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
            this.txtY.Size = new System.Drawing.Size(128, 20);
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
            this.txtX.Size = new System.Drawing.Size(128, 20);
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
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtHatchScale);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtTextHeight);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.txtLayer);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(646, 298);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(217, 120);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Görünüm";
            // 
            // txtHatchScale
            // 
            this.txtHatchScale.Location = new System.Drawing.Point(101, 83);
            this.txtHatchScale.Name = "txtHatchScale";
            this.txtHatchScale.Size = new System.Drawing.Size(100, 20);
            this.txtHatchScale.TabIndex = 5;
            this.txtHatchScale.Text = "1";
            this.txtHatchScale.Validating += new System.ComponentModel.CancelEventHandler(this.txtHatchScale_Validating);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(17, 86);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Tarama Sıklığı";
            // 
            // txtTextHeight
            // 
            this.txtTextHeight.Location = new System.Drawing.Point(101, 57);
            this.txtTextHeight.Name = "txtTextHeight";
            this.txtTextHeight.Size = new System.Drawing.Size(100, 20);
            this.txtTextHeight.TabIndex = 3;
            this.txtTextHeight.Text = "3";
            this.txtTextHeight.Validating += new System.ComponentModel.CancelEventHandler(this.txtTextHeight_Validating);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 60);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(78, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Yazı Yüksekliği";
            // 
            // txtLayer
            // 
            this.txtLayer.Location = new System.Drawing.Point(101, 31);
            this.txtLayer.Name = "txtLayer";
            this.txtLayer.Size = new System.Drawing.Size(100, 20);
            this.txtLayer.TabIndex = 1;
            this.txtLayer.Text = "MENFEZ PROFIL";
            this.txtLayer.Validating += new System.ComponentModel.CancelEventHandler(this.txtLayer_Validating);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Layer";
            // 
            // DrawCulvertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(875, 529);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.culvertGrid);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DrawCulvertForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Güzergah Profili Üzerinde Menfez Çizimi";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox cbDrawCulvertInfo;
        private SourceGrid.Grid culvertGrid;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBaseLevel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtZ;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtY;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtX;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnPickBasePoint;
        private System.Windows.Forms.TextBox txtBaseCH;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtScale;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtLayer;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtHatchScale;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtTextHeight;
        private System.Windows.Forms.Label label9;
    }
}