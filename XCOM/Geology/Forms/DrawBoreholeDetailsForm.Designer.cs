namespace XCOM.Commands.Geology
{
    partial class DrawBoreholeDetailsForm
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
            this.boreholeGrid = new SourceGrid.Grid();
            this.cbGroundwater = new System.Windows.Forms.CheckBox();
            this.txtGroundwater = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(739, 494);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "İptal";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(658, 494);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Tamam";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // boreholeGrid
            // 
            this.boreholeGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.boreholeGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.boreholeGrid.ClipboardMode = ((SourceGrid.ClipboardMode)((((SourceGrid.ClipboardMode.Copy | SourceGrid.ClipboardMode.Cut) 
            | SourceGrid.ClipboardMode.Paste) 
            | SourceGrid.ClipboardMode.Delete)));
            this.boreholeGrid.EnableSort = true;
            this.boreholeGrid.Location = new System.Drawing.Point(12, 12);
            this.boreholeGrid.Name = "boreholeGrid";
            this.boreholeGrid.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.boreholeGrid.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.boreholeGrid.Size = new System.Drawing.Size(802, 463);
            this.boreholeGrid.TabIndex = 0;
            this.boreholeGrid.TabStop = true;
            this.boreholeGrid.ToolTipText = "";
            this.boreholeGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.boreholeGrid_MouseDown);
            this.boreholeGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.boreholeGrid_MouseMove);
            this.boreholeGrid.MouseUp += new System.Windows.Forms.MouseEventHandler(this.boreholeGrid_MouseUp);
            // 
            // cbGroundwater
            // 
            this.cbGroundwater.AutoSize = true;
            this.cbGroundwater.Location = new System.Drawing.Point(12, 498);
            this.cbGroundwater.Name = "cbGroundwater";
            this.cbGroundwater.Size = new System.Drawing.Size(122, 17);
            this.cbGroundwater.TabIndex = 1;
            this.cbGroundwater.Text = "Yeraltı Suyu Derinliği";
            this.cbGroundwater.UseVisualStyleBackColor = true;
            // 
            // txtGroundwater
            // 
            this.txtGroundwater.Location = new System.Drawing.Point(140, 496);
            this.txtGroundwater.Name = "txtGroundwater";
            this.txtGroundwater.Size = new System.Drawing.Size(89, 20);
            this.txtGroundwater.TabIndex = 2;
            this.txtGroundwater.Text = "0";
            this.txtGroundwater.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // DrawBoreholeDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(826, 529);
            this.Controls.Add(this.txtGroundwater);
            this.Controls.Add(this.cbGroundwater);
            this.Controls.Add(this.boreholeGrid);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DrawBoreholeDetailsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sondaj Detayları";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox cbGroundwater;
        private System.Windows.Forms.TextBox txtGroundwater;
        private SourceGrid.Grid boreholeGrid;
    }
}