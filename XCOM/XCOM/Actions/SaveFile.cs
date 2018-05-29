using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.IO;

namespace XCOM.Commands.XCommand
{
    public class SaveFile : XCOMActionBase
    {
        public override string Name { get { return "Dosyayı Kaydet"; } }
        public override int Order { get { return int.MaxValue; } }
        public override bool Recommended { get { return true; } }
        public override ActionInterface Interface { get { return ActionInterface.Both; } }

        protected bool keepCurrentVersion = true;
        protected DwgVersion version = DwgVersion.Current;
        protected string suffix = string.Empty;

        public override void Run(string filename, Database db)
        {
            // Save file under temporary filename
            string tempFilename = Path.GetDirectoryName(filename) + "\\____xcom_save.tmp";
            try
            {
                db.SaveAs(tempFilename, (keepCurrentVersion ? db.OriginalFileVersion : version));
            }
            catch (System.Exception ex)
            {
                OnError(ex);
                File.Delete(tempFilename);
                return;
            }

            // Move the file to its final name
            string saveFilename = GetSaveFilename(filename);
            try
            {
                File.Copy(tempFilename, saveFilename, true);
                File.Delete(tempFilename);
            }
            catch (System.Exception ex)
            {
                OnError(ex);
                File.Delete(tempFilename);
                return;
            }
        }

        private string GetSaveFilename(string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            string ext = Path.GetExtension(filename);

            // Remove suffix if exists
            if (string.Compare(name.Substring(name.Length - suffix.Length, suffix.Length), suffix, StringComparison.OrdinalIgnoreCase) == 0)
            {
                name = name.Substring(0, name.Length - suffix.Length);
            }

            return Path.Combine(dir, name + suffix + ext);
        }

        public override bool ShowDialog()
        {
            using (SaveFileForm form = new SaveFileForm())
            {
                form.KeepCurrentDwgVersion = keepCurrentVersion;
                form.DwgVersion = version;
                form.FilenameSuffix = suffix;

                if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

                keepCurrentVersion = form.KeepCurrentDwgVersion;
                version = form.DwgVersion;
                suffix = form.FilenameSuffix;

                return true;
            }
        }
    }
}