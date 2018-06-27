using Autodesk.AutoCAD.DatabaseServices;
using System.IO;

namespace XCOM.Commands.XCommand
{
    public class BackupFile : XCOMActionBase
    {
        public override string Name => "Dosyayı Yedekle";
        public override int Order => 1;
        public override string HelpText => "Dosyayı işlemeden önce kendi klasörü içinde '" + backupFolder + "' alt klasörüne yedekler.";

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