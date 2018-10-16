namespace XCOM.Commands.Topography
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
            this.pickBasePoint = new AcadUtility.WinForms.PickCoordinateControl();
            this.txtBaseCH = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtScale = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBaseLevel = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbLayer = new AcadUtility.WinForms.LayerComboBox();
            this.txtHatchScale = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtTextHeight = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(790, 434);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "İptal";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(709, 434);
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
            this.culvertGrid.Size = new System.Drawing.Size(619, 406);
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
            this.cbDrawCulvertInfo.Size = new System.Drawing.Size(153, 17);
            this.cbDrawCulvertInfo.TabIndex = 7;
            this.cbDrawCulvertInfo.Text = "Menfez bilgilerini profile yaz";
            this.cbDrawCulvertInfo.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.pickBasePoint);
            this.groupBox1.Controls.Add(this.txtBaseCH);
            this.groupBox1.Controls.Add(this.cbDrawCulvertInfo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtScale);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtBaseLevel);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(648, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 280);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Seçenekler";
            // 
            // pickBasePoint
            // 
            this.pickBasePoint.Location = new System.Drawing.Point(18, 23);
            this.pickBasePoint.Name = "pickBasePoint";
            this.pickBasePoint.Size = new System.Drawing.Size(183, 96);
            this.pickBasePoint.TabIndex = 0;
            // 
            // txtBaseCH
            // 
            this.txtBaseCH.Location = new System.Drawing.Point(73, 140);
            this.txtBaseCH.Name = "txtBaseCH";
            this.txtBaseCH.Size = new System.Drawing.Size(128, 20);
            this.txtBaseCH.TabIndex = 2;
            this.txtBaseCH.Text = "0+000.00";
            this.txtBaseCH.Validating += new System.ComponentModel.CancelEventHandler(this.txtBaseCH_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Baz KM";
            // 
            // txtScale
            // 
            this.txtScale.Location = new System.Drawing.Point(73, 206);
            this.txtScale.Name = "txtScale";
            this.txtScale.Size = new System.Drawing.Size(128, 20);
            this.txtScale.TabIndex = 6;
            this.txtScale.Text = "10";
            this.txtScale.Validating += new System.ComponentModel.CancelEventHandler(this.txtScale_Validating);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Ölçek";
            // 
            // txtBaseLevel
            // 
            this.txtBaseLevel.Location = new System.Drawing.Point(73, 166);
            this.txtBaseLevel.Name = "txtBaseLevel";
            this.txtBaseLevel.Size = new System.Drawing.Size(128, 20);
            this.txtBaseLevel.TabIndex = 4;
            this.txtBaseLevel.Text = "0";
            this.txtBaseLevel.Validating += new System.ComponentModel.CancelEventHandler(this.txtBaseLevel_Validating);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 170);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Baz kotu";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cbLayer);
            this.groupBox2.Controls.Add(this.txtHatchScale);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.txtTextHeight);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Location = new System.Drawing.Point(648, 298);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(217, 120);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Görünüm";
            // 
            // cbLayer
            // 
            this.cbLayer.Database = null;
            this.cbLayer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbLayer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLayer.FormattingEnabled = true;
            this.cbLayer.HasIcons = true;
            this.cbLayer.IconSize = new System.Drawing.Size(16, 16);
            this.cbLayer.Location = new System.Drawing.Point(101, 31);
            this.cbLayer.Name = "cbLayer";
            this.cbLayer.Size = new System.Drawing.Size(100, 21);
            this.cbLayer.TabIndex = 8;
            this.cbLayer.Text = null;
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
            this.label10.Size = new System.Drawing.Size(71, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Tarama sıklığı";
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
            this.label9.Size = new System.Drawing.Size(76, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Yazı yüksekliği";
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
            this.ClientSize = new System.Drawing.Size(877, 469);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.culvertGrid);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DrawCulvertForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
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
        private System.Windows.Forms.TextBox txtBaseCH;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtScale;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtHatchScale;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtTextHeight;
        private System.Windows.Forms.Label label9;
        private AcadUtility.WinForms.LayerComboBox cbLayer;
        private AcadUtility.WinForms.PickCoordinateControl pickBasePoint;
    }
}