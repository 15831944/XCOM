namespace XCOM
{
    partial class ProgressForm
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
            this.pgTotal = new System.Windows.Forms.ProgressBar();
            this.pgFile = new System.Windows.Forms.ProgressBar();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.lvProgress = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txErrors = new System.Windows.Forms.TextBox();
            this.cmdClose = new System.Windows.Forms.Button();
            this.pnProgress = new System.Windows.Forms.Panel();
            this.pnProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgTotal
            // 
            this.pgTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgTotal.Location = new System.Drawing.Point(3, 27);
            this.pgTotal.Name = "pgTotal";
            this.pgTotal.Size = new System.Drawing.Size(608, 23);
            this.pgTotal.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgTotal.TabIndex = 1;
            // 
            // pgFile
            // 
            this.pgFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgFile.Location = new System.Drawing.Point(3, 88);
            this.pgFile.Name = "pgFile";
            this.pgFile.Size = new System.Drawing.Size(608, 23);
            this.pgFile.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgFile.TabIndex = 3;
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Location = new System.Drawing.Point(3, 7);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(27, 13);
            this.lblItem.TabIndex = 0;
            this.lblItem.Text = "Item";
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(3, 68);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(37, 13);
            this.lblAction.TabIndex = 2;
            this.lblAction.Text = "Action";
            // 
            // lvProgress
            // 
            this.lvProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvProgress.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvProgress.Location = new System.Drawing.Point(12, 12);
            this.lvProgress.Name = "lvProgress";
            this.lvProgress.Size = new System.Drawing.Size(614, 257);
            this.lvProgress.TabIndex = 0;
            this.lvProgress.UseCompatibleStateImageBehavior = false;
            this.lvProgress.View = System.Windows.Forms.View.Details;
            this.lvProgress.SelectedIndexChanged += new System.EventHandler(this.lvProgress_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Dosya Adı";
            this.columnHeader1.Width = 490;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Sonuç";
            this.columnHeader2.Width = 100;
            // 
            // txErrors
            // 
            this.txErrors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txErrors.Location = new System.Drawing.Point(12, 291);
            this.txErrors.Multiline = true;
            this.txErrors.Name = "txErrors";
            this.txErrors.Size = new System.Drawing.Size(614, 118);
            this.txErrors.TabIndex = 8;
            this.txErrors.Text = "Hataları görüntülemek için listeden bir satır seçin.";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdClose.Location = new System.Drawing.Point(551, 428);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 2;
            this.cmdClose.Text = "Kapat";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // pnProgress
            // 
            this.pnProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnProgress.Controls.Add(this.lblItem);
            this.pnProgress.Controls.Add(this.pgTotal);
            this.pnProgress.Controls.Add(this.pgFile);
            this.pnProgress.Controls.Add(this.lblAction);
            this.pnProgress.Location = new System.Drawing.Point(12, 291);
            this.pnProgress.Name = "pnProgress";
            this.pnProgress.Size = new System.Drawing.Size(614, 118);
            this.pnProgress.TabIndex = 1;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdClose;
            this.ClientSize = new System.Drawing.Size(638, 468);
            this.Controls.Add(this.pnProgress);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.txErrors);
            this.Controls.Add(this.lvProgress);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.Text = "XCOM";
            this.pnProgress.ResumeLayout(false);
            this.pnProgress.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.ProgressBar pgTotal;
        private System.Windows.Forms.ProgressBar pgFile;
        public System.Windows.Forms.ListView lvProgress;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox txErrors;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Panel pnProgress;

    }
}