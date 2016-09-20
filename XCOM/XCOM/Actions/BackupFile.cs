using System.IO;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace XCOM.Commands.XCommand
{
    public class BackupFile : IXCOMAction
    {
        public string Name { get { return "Dosyayı Yedekle"; } }
        public int Order { get { return 1; } }
        public bool Recommended { get { return false; } }
        public ActionInterface Interface { get { return ActionInterface.Command; } }
        public bool ShowDialog() { return true; }

        protected string backupFolder = "_backup";

        public override string ToString()
        {
            return Name;
        }

        public void Run(string filename, Database db)
        {
            // Copy to backup folder
            try
            {
                string backupFilename = GetBackupFilename(filename);
                File.Copy(filename, backupFilename, true);
            }
            catch (System.Exception ex)
            {
                OnError(ex);
            }
        }

        private string GetBackupFilename(string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            string name = Path.GetFileName(filename);

            string backupDir = Path.Combine(dir, backupFolder);
            Directory.CreateDirectory(backupDir);

            return Path.Combine(backupDir, name);
        }

        public event EventHandler<ActionProgressEventArgs> Progress;
        public event EventHandler<ActionErrorEventArgs> Error;

        protected void OnProgress(string message)
        {
            EventHandler<ActionProgressEventArgs> handler = Progress;
            if (handler != null)
            {
                handler(this, new ActionProgressEventArgs(message));
            }
        }

        protected void OnError(Exception error)
        {
            EventHandler<ActionErrorEventArgs> handler = Error;
            if (handler != null)
            {
                handler(this, new ActionErrorEventArgs(error));
            }
        }
    }
}