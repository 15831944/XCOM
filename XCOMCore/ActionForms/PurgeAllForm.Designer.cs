namespace XCOMCore.ActionForms
{
    partial class PurgeAllForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbViews = new System.Windows.Forms.CheckBox();
            this.cbViewports = new System.Windows.Forms.CheckBox();
            this.cbUCSSettings = new System.Windows.Forms.CheckBox();
            this.cbVisualStyles = new System.Windows.Forms.CheckBox();
            this.cbTextStyles = new System.Windows.Forms.CheckBox();
            this.cbTableStyles = new System.Windows.Forms.CheckBox();
            this.cbShapes = new System.Windows.Forms.CheckBox();
            this.cbPlotStyles = new System.Windows.Forms.CheckBox();
            this.cbMultileaderStyles = new System.Windows.Forms.CheckBox();
            this.cbMlineStyles = new System.Windows.Forms.CheckBox();
            this.cbMaterials = new System.Windows.Forms.CheckBox();
            this.cbLinetypes = new System.Windows.Forms.CheckBox();
            this.cbLayers = new System.Windows.Forms.CheckBox();
            this.cbGroups = new System.Windows.Forms.CheckBox();
            this.cbDimensionStyles = new System.Windows.Forms.CheckBox();
            this.cbBlocks = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbZeroLengthGeometry = new System.Windows.Forms.CheckBox();
            this.cbEmptyTexts = new System.Windows.Forms.CheckBox();
            this.cbRegApps = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(279, 362);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 4;
            this.cmdCancel.Text = "İptal";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(198, 362);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 3;
            this.cmdOK.Text = "Tamam";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbViews);
            this.groupBox1.Controls.Add(this.cbViewports);
            this.groupBox1.Controls.Add(this.cbUCSSettings);
            this.groupBox1.Controls.Add(this.cbVisualStyles);
            this.groupBox1.Controls.Add(this.cbTextStyles);
            this.groupBox1.Controls.Add(this.cbTableStyles);
            this.groupBox1.Controls.Add(this.cbShapes);
            this.groupBox1.Controls.Add(this.cbPlotStyles);
            this.groupBox1.Controls.Add(this.cbMultileaderStyles);
            this.groupBox1.Controls.Add(this.cbMlineStyles);
            this.groupBox1.Controls.Add(this.cbMaterials);
            this.groupBox1.Controls.Add(this.cbLinetypes);
            this.groupBox1.Controls.Add(this.cbLayers);
            this.groupBox1.Controls.Add(this.cbGroups);
            this.groupBox1.Controls.Add(this.cbDimensionStyles);
            this.groupBox1.Controls.Add(this.cbBlocks);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(340, 233);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Çizim Veritabanı";
            // 
            // cbViews
            // 
            this.cbViews.AutoSize = true;
            this.cbViews.Checked = true;
            this.cbViews.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbViews.Location = new System.Drawing.Point(193, 171);
            this.cbViews.Name = "cbViews";
            this.cbViews.Size = new System.Drawing.Size(54, 17);
            this.cbViews.TabIndex = 14;
            this.cbViews.Text = "Views";
            this.cbViews.UseVisualStyleBackColor = true;
            // 
            // cbViewports
            // 
            this.cbViewports.AutoSize = true;
            this.cbViewports.Checked = true;
            this.cbViewports.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbViewports.Location = new System.Drawing.Point(193, 148);
            this.cbViewports.Name = "cbViewports";
            this.cbViewports.Size = new System.Drawing.Size(72, 17);
            this.cbViewports.TabIndex = 13;
            this.cbViewports.Text = "Viewports";
            this.cbViewports.UseVisualStyleBackColor = true;
            // 
            // cbUCSSettings
            // 
            this.cbUCSSettings.AutoSize = true;
            this.cbUCSSettings.Checked = true;
            this.cbUCSSettings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUCSSettings.Location = new System.Drawing.Point(193, 125);
            this.cbUCSSettings.Name = "cbUCSSettings";
            this.cbUCSSettings.Size = new System.Drawing.Size(89, 17);
            this.cbUCSSettings.TabIndex = 12;
            this.cbUCSSettings.Text = "UCS Settings";
            this.cbUCSSettings.UseVisualStyleBackColor = true;
            // 
            // cbVisualStyles
            // 
            this.cbVisualStyles.AutoSize = true;
            this.cbVisualStyles.Checked = true;
            this.cbVisualStyles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVisualStyles.Location = new System.Drawing.Point(193, 194);
            this.cbVisualStyles.Name = "cbVisualStyles";
            this.cbVisualStyles.Size = new System.Drawing.Size(85, 17);
            this.cbVisualStyles.TabIndex = 15;
            this.cbVisualStyles.Text = "Visual Styles";
            this.cbVisualStyles.UseVisualStyleBackColor = true;
            // 
            // cbTextStyles
            // 
            this.cbTextStyles.AutoSize = true;
            this.cbTextStyles.Checked = true;
            this.cbTextStyles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTextStyles.Location = new System.Drawing.Point(193, 102);
            this.cbTextStyles.Name = "cbTextStyles";
            this.cbTextStyles.Size = new System.Drawing.Size(78, 17);
            this.cbTextStyles.TabIndex = 11;
            this.cbTextStyles.Text = "Text Styles";
            this.cbTextStyles.UseVisualStyleBackColor = true;
            // 
            // cbTableStyles
            // 
            this.cbTableStyles.AutoSize = true;
            this.cbTableStyles.Checked = true;
            this.cbTableStyles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTableStyles.Location = new System.Drawing.Point(193, 79);
            this.cbTableStyles.Name = "cbTableStyles";
            this.cbTableStyles.Size = new System.Drawing.Size(84, 17);
            this.cbTableStyles.TabIndex = 10;
            this.cbTableStyles.Text = "Table Styles";
            this.cbTableStyles.UseVisualStyleBackColor = true;
            // 
            // cbShapes
            // 
            this.cbShapes.AutoSize = true;
            this.cbShapes.Enabled = false;
            this.cbShapes.Location = new System.Drawing.Point(193, 56);
            this.cbShapes.Name = "cbShapes";
            this.cbShapes.Size = new System.Drawing.Size(62, 17);
            this.cbShapes.TabIndex = 9;
            this.cbShapes.Text = "Shapes";
            this.cbShapes.UseVisualStyleBackColor = true;
            // 
            // cbPlotStyles
            // 
            this.cbPlotStyles.AutoSize = true;
            this.cbPlotStyles.Checked = true;
            this.cbPlotStyles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPlotStyles.Location = new System.Drawing.Point(193, 33);
            this.cbPlotStyles.Name = "cbPlotStyles";
            this.cbPlotStyles.Size = new System.Drawing.Size(75, 17);
            this.cbPlotStyles.TabIndex = 8;
            this.cbPlotStyles.Text = "Plot Styles";
            this.cbPlotStyles.UseVisualStyleBackColor = true;
            // 
            // cbMultileaderStyles
            // 
            this.cbMultileaderStyles.AutoSize = true;
            this.cbMultileaderStyles.Checked = true;
            this.cbMultileaderStyles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMultileaderStyles.Location = new System.Drawing.Point(19, 194);
            this.cbMultileaderStyles.Name = "cbMultileaderStyles";
            this.cbMultileaderStyles.Size = new System.Drawing.Size(108, 17);
            this.cbMultileaderStyles.TabIndex = 7;
            this.cbMultileaderStyles.Text = "Multileader Styles";
            this.cbMultileaderStyles.UseVisualStyleBackColor = true;
            // 
            // cbMlineStyles
            // 
            this.cbMlineStyles.AutoSize = true;
            this.cbMlineStyles.Checked = true;
            this.cbMlineStyles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMlineStyles.Location = new System.Drawing.Point(19, 171);
            this.cbMlineStyles.Name = "cbMlineStyles";
            this.cbMlineStyles.Size = new System.Drawing.Size(82, 17);
            this.cbMlineStyles.TabIndex = 6;
            this.cbMlineStyles.Text = "Mline Styles";
            this.cbMlineStyles.UseVisualStyleBackColor = true;
            // 
            // cbMaterials
            // 
            this.cbMaterials.AutoSize = true;
            this.cbMaterials.Checked = true;
            this.cbMaterials.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMaterials.Location = new System.Drawing.Point(19, 148);
            this.cbMaterials.Name = "cbMaterials";
            this.cbMaterials.Size = new System.Drawing.Size(68, 17);
            this.cbMaterials.TabIndex = 5;
            this.cbMaterials.Text = "Materials";
            this.cbMaterials.UseVisualStyleBackColor = true;
            // 
            // cbLinetypes
            // 
            this.cbLinetypes.AutoSize = true;
            this.cbLinetypes.Checked = true;
            this.cbLinetypes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLinetypes.Location = new System.Drawing.Point(19, 125);
            this.cbLinetypes.Name = "cbLinetypes";
            this.cbLinetypes.Size = new System.Drawing.Size(71, 17);
            this.cbLinetypes.TabIndex = 4;
            this.cbLinetypes.Text = "Linetypes";
            this.cbLinetypes.UseVisualStyleBackColor = true;
            // 
            // cbLayers
            // 
            this.cbLayers.AutoSize = true;
            this.cbLayers.Checked = true;
            this.cbLayers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLayers.Location = new System.Drawing.Point(19, 102);
            this.cbLayers.Name = "cbLayers";
            this.cbLayers.Size = new System.Drawing.Size(57, 17);
            this.cbLayers.TabIndex = 3;
            this.cbLayers.Text = "Layers";
            this.cbLayers.UseVisualStyleBackColor = true;
            // 
            // cbGroups
            // 
            this.cbGroups.AutoSize = true;
            this.cbGroups.Checked = true;
            this.cbGroups.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGroups.Location = new System.Drawing.Point(19, 79);
            this.cbGroups.Name = "cbGroups";
            this.cbGroups.Size = new System.Drawing.Size(60, 17);
            this.cbGroups.TabIndex = 2;
            this.cbGroups.Text = "Groups";
            this.cbGroups.UseVisualStyleBackColor = true;
            // 
            // cbDimensionStyles
            // 
            this.cbDimensionStyles.AutoSize = true;
            this.cbDimensionStyles.Checked = true;
            this.cbDimensionStyles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbDimensionStyles.Location = new System.Drawing.Point(19, 56);
            this.cbDimensionStyles.Name = "cbDimensionStyles";
            this.cbDimensionStyles.Size = new System.Drawing.Size(106, 17);
            this.cbDimensionStyles.TabIndex = 1;
            this.cbDimensionStyles.Text = "Dimension Styles";
            this.cbDimensionStyles.UseVisualStyleBackColor = true;
            // 
            // cbBlocks
            // 
            this.cbBlocks.AutoSize = true;
            this.cbBlocks.Checked = true;
            this.cbBlocks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBlocks.Location = new System.Drawing.Point(19, 33);
            this.cbBlocks.Name = "cbBlocks";
            this.cbBlocks.Size = new System.Drawing.Size(58, 17);
            this.cbBlocks.TabIndex = 0;
            this.cbBlocks.Text = "Blocks";
            this.cbBlocks.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbZeroLengthGeometry);
            this.groupBox2.Controls.Add(this.cbEmptyTexts);
            this.groupBox2.Location = new System.Drawing.Point(12, 260);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(167, 85);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Çizim Nesneleri";
            // 
            // cbZeroLengthGeometry
            // 
            this.cbZeroLengthGeometry.AutoSize = true;
            this.cbZeroLengthGeometry.Checked = true;
            this.cbZeroLengthGeometry.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbZeroLengthGeometry.Location = new System.Drawing.Point(19, 29);
            this.cbZeroLengthGeometry.Name = "cbZeroLengthGeometry";
            this.cbZeroLengthGeometry.Size = new System.Drawing.Size(132, 17);
            this.cbZeroLengthGeometry.TabIndex = 0;
            this.cbZeroLengthGeometry.Text = "Zero Length Geometry";
            this.cbZeroLengthGeometry.UseVisualStyleBackColor = true;
            // 
            // cbEmptyTexts
            // 
            this.cbEmptyTexts.AutoSize = true;
            this.cbEmptyTexts.Checked = true;
            this.cbEmptyTexts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEmptyTexts.Location = new System.Drawing.Point(19, 52);
            this.cbEmptyTexts.Name = "cbEmptyTexts";
            this.cbEmptyTexts.Size = new System.Drawing.Size(84, 17);
            this.cbEmptyTexts.TabIndex = 1;
            this.cbEmptyTexts.Text = "Empty Texts";
            this.cbEmptyTexts.UseVisualStyleBackColor = true;
            // 
            // cbRegApps
            // 
            this.cbRegApps.AutoSize = true;
            this.cbRegApps.Checked = true;
            this.cbRegApps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRegApps.Location = new System.Drawing.Point(20, 29);
            this.cbRegApps.Name = "cbRegApps";
            this.cbRegApps.Size = new System.Drawing.Size(70, 17);
            this.cbRegApps.TabIndex = 0;
            this.cbRegApps.Text = "RegApps";
            this.cbRegApps.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbRegApps);
            this.groupBox3.Location = new System.Drawing.Point(185, 260);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(167, 85);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Diğer Seçenekler";
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Image = global::XCOMCore.Properties.Resources.lightbulb;
            this.btnCheckAll.Location = new System.Drawing.Point(12, 362);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(23, 23);
            this.btnCheckAll.TabIndex = 5;
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Image = global::XCOMCore.Properties.Resources.lightbulb_off;
            this.btnUncheckAll.Location = new System.Drawing.Point(34, 362);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(23, 23);
            this.btnUncheckAll.TabIndex = 5;
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // PurgeAllForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(366, 397);
            this.Controls.Add(this.btnUncheckAll);
            this.Controls.Add(this.btnCheckAll);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PurgeAllForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Purge All";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbBlocks;
        private System.Windows.Forms.CheckBox cbVisualStyles;
        private System.Windows.Forms.CheckBox cbTextStyles;
        private System.Windows.Forms.CheckBox cbTableStyles;
        private System.Windows.Forms.CheckBox cbShapes;
        private System.Windows.Forms.CheckBox cbPlotStyles;
        private System.Windows.Forms.CheckBox cbMultileaderStyles;
        private System.Windows.Forms.CheckBox cbMlineStyles;
        private System.Windows.Forms.CheckBox cbMaterials;
        private System.Windows.Forms.CheckBox cbLinetypes;
        private System.Windows.Forms.CheckBox cbLayers;
        private System.Windows.Forms.CheckBox cbGroups;
        private System.Windows.Forms.CheckBox cbDimensionStyles;
        private System.Windows.Forms.CheckBox cbUCSSettings;
        private System.Windows.Forms.CheckBox cbViews;
        private System.Windows.Forms.CheckBox cbViewports;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbZeroLengthGeometry;
        private System.Windows.Forms.CheckBox cbEmptyTexts;
        private System.Windows.Forms.CheckBox cbRegApps;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnUncheckAll;
    }
}