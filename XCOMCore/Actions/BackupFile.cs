using System.IO;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOMCore
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

        public string[] Run(string filename, Database db)
        {
            List<string> errors = new List<string>();

            // Copy to backup folder
            try
            {
                string backupFilename = GetBackupFilename(filename);
                File.Copy(filename, backupFilename, true);
            }
            catch (System.Exception ex)
            {
                errors.Add(ex.Message);
                return errors.ToArray();
            }

            return errors.ToArray();
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