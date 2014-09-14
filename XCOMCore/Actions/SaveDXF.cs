using System;
using System.IO;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.LayerManager;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Geometry;

namespace XCOMCore
{
    public class SaveDXF : IXCOMAction
    {
        public string Name { get { return "DXF Kaydet"; } }
        public int Order { get { return 500000; } }
        public bool Recommended { get { return false; } }
        public ActionInterface Interface { get { return ActionInterface.Both; } }

        protected bool keepCurrentVersion = true;
        protected DwgVersion version = DwgVersion.AC1024;

        public override string ToString()
        {
            return Name;
        }

        public string[] Run(string filename, Database db)
        {
            List<string> errors = new List<string>();

            string saveFilename = GetDXFFilename(filename);
            try
            {
                db.DxfOut(saveFilename, 16, version);
            }
            catch (System.Exception ex)
            {
                errors.Add(ex.Message);
                return errors.ToArray();
            }

            return errors.ToArray();
        }

        private string GetDXFFilename(string filename)
        {
            string dir = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);

            return Path.Combine(dir, name + ".dxf");
        }

        public bool ShowDialog()
        {
            XCOMCore.ActionForms.SaveDXFForm form = new ActionForms.SaveDXFForm();
            form.DXFVersion = version;

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

            version = form.DXFVersion;

            return true;
        }
    }
}