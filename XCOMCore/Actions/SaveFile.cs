using System;
using System.IO;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOMCore
{
    public class SaveFile : IXCOMAction
    {
        public string Name { get { return "Dosyayı Kaydet"; } }
        public int Order { get { return int.MaxValue; } }
        public bool Recommended { get { return true; } }
        public ActionInterface Interface { get { return ActionInterface.Both; } }

        protected bool keepCurrentVersion = true;
        protected DwgVersion version = DwgVersion.Current;
        protected string suffix = string.Empty;

        public override string ToString()
        {
            return Name;
        }

        public string[] Run(string filename, Database db)
        {
            List<string> errors = new List<string>();

            // Save file under temporary filename
            string tempFilename = Path.GetDirectoryName(filename) + "\\____xcom_save.tmp";
            try
            {
                db.SaveAs(tempFilename, (keepCurrentVersion ? DwgVersion.Current : version));
            }
            catch (System.Exception ex)
            {
                errors.Add(ex.Message);
                File.Delete(tempFilename);
                return errors.ToArray();
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
                errors.Add(ex.Message);
                File.Delete(tempFilename);
                return errors.ToArray();
            }

            return errors.ToArray();
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

        public bool ShowDialog()
        {
            XCOMCore.ActionForms.SaveFileForm form = new ActionForms.SaveFileForm();
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