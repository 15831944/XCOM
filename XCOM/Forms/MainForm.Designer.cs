namespace XCOM.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lvSourceFiles = new System.Windows.Forms.ListView();
            this.filename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filepath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbSource = new System.Windows.Forms.Panel();
            this.browseDWG = new System.Windows.Forms.OpenFileDialog();
            this.cmdClose = new System.Windows.Forms.Button();
            this.cmdStart = new System.Windows.Forms.Button();
            this.lbActions = new XCOM.CheckedListBoxWithButtons();
            this.cmdClearFiles = new System.Windows.Forms.Button();
            this.cmdRemoveFile = new System.Windows.Forms.Button();
            this.cmdAddFolder = new System.Windows.Forms.Button();
            this.cmdAddFile = new System.Windows.Forms.Button();
            this.browseFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.gbSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvSourceFiles
            // 
            this.lvSourceFiles.AllowDrop = true;
            resources.ApplyResources(this.lvSourceFiles, "lvSourceFiles");
            this.lvSourceFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.filename,
            this.filepath});
            this.lvSourceFiles.Name = "lvSourceFiles";
            this.lvSourceFiles.UseCompatibleStateImageBehavior = false;
            this.lvSourceFiles.View = System.Windows.Forms.View.Details;
            this.lvSourceFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvSourceFiles_DragDrop);
            this.lvSourceFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvSourceFiles_DragEnter);
            // 
            // filename
            // 
            resources.ApplyResources(this.filename, "filename");
            // 
            // filepath
            // 
            resources.ApplyResources(this.filepath, "filepath");
            // 
            // gbSource
            // 
            resources.ApplyResources(this.gbSource, "gbSource");
            this.gbSource.Controls.Add(this.lvSourceFiles);
            this.gbSource.Controls.Add(this.cmdClearFiles);
            this.gbSource.Controls.Add(this.cmdRemoveFile);
            this.gbSource.Controls.Add(this.cmdAddFolder);
            this.gbSource.Controls.Add(this.cmdAddFile);
            this.gbSource.Name = "gbSource";
            // 
            // browseDWG
            // 
            resources.ApplyResources(this.browseDWG, "browseDWG");
            this.browseDWG.Multiselect = true;
            // 
            // cmdClose
            // 
            resources.ApplyResources(this.cmdClose, "cmdClose");
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // cmdStart
            // 
            resources.ApplyResources(this.cmdStart, "cmdStart");
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.UseVisualStyleBackColor = true;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // lbActions
            // 
            resources.ApplyResources(this.lbActions, "lbActions");
            this.lbActions.CheckOnClick = true;
            this.lbActions.FormattingEnabled = true;
            this.lbActions.Name = "lbActions";
            this.lbActions.ButtonClick += new XCOM.CheckedListBoxWithButtons.ButtonClickEventHandler(this.lbActions_ButtonClick);
            // 
            // cmdClearFiles
            // 
            resources.ApplyResources(this.cmdClearFiles, "cmdClearFiles");
            this.cmdClearFiles.Image = global::XCOM.Properties.Resources.cross;
            this.cmdClearFiles.Name = "cmdClearFiles";
            this.cmdClearFiles.UseVisualStyleBackColor = true;
            this.cmdClearFiles.Click += new System.EventHandler(this.cmdClearFiles_Click);
            // 
            // cmdRemoveFile
            // 
            resources.ApplyResources(this.cmdRemoveFile, "cmdRemoveFile");
            this.cmdRemoveFile.Image = global::XCOM.Properties.Resources.delete;
            this.cmdRemoveFile.Name = "cmdRemoveFile";
            this.cmdRemoveFile.UseVisualStyleBackColor = true;
            this.cmdRemoveFile.Click += new System.EventHandler(this.cmdRemoveFile_Click);
            // 
            // cmdAddFolder
            // 
            resources.ApplyResources(this.cmdAddFolder, "cmdAddFolder");
            this.cmdAddFolder.Image = global::XCOM.Properties.Resources.folder_add;
            this.cmdAddFolder.Name = "cmdAddFolder";
            this.cmdAddFolder.UseVisualStyleBackColor = true;
            this.cmdAddFolder.Click += new System.EventHandler(this.cmdAddFolder_Click);
            // 
            // cmdAddFile
            // 
            resources.ApplyResources(this.cmdAddFile, "cmdAddFile");
            this.cmdAddFile.Image = global::XCOM.Properties.Resources.add;
            this.cmdAddFile.Name = "cmdAddFile";
            this.cmdAddFile.UseVisualStyleBackColor = true;
            this.cmdAddFile.Click += new System.EventHandler(this.cmdAddFile_Click);
            // 
            // browseFolder
            // 
            this.browseFolder.ShowNewFolderButton = false;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdClose;
            this.Controls.Add(this.lbActions);
            this.Controls.Add(this.gbSource);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.cmdStart);
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.gbSource.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader filename;
        private System.Windows.Forms.ColumnHeader filepath;
        private System.Windows.Forms.Panel gbSource;
        private System.Windows.Forms.Button cmdClearFiles;
        private System.Windows.Forms.Button cmdRemoveFile;
        private System.Windows.Forms.Button cmdAddFile;
        private System.Windows.Forms.OpenFileDialog browseDWG;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.ListView lvSourceFiles;
        private XCOM.CheckedListBoxWithButtons lbActions;
        private System.Windows.Forms.Button cmdAddFolder;
        private System.Windows.Forms.FolderBrowserDialog browseFolder;


    }
}