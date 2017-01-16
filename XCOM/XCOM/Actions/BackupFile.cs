using System.IO;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace XCOM.Commands.XCommand
{
    public class BackupFile : XCOMActionBase
    {
        public override string Name { get { return "Dosyayı Yedekle"; } }
        public override int Order { get { return 1; } }

        protected string backupFolder = "_backup";

        public override void Run(string filename, Database db)
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
    }
}