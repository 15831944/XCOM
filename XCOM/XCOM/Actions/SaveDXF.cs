using Autodesk.AutoCAD.DatabaseServices;
using System.IO;

namespace XCOM.Commands.XCommand
{
    public class SaveDXF : XCOMActionBase
    {
        public override string Name => "DXF Kaydet";
        public override int Order => int.MaxValue - 1;
        public override ActionInterface Interface => ActionInterface.Both;
        public override string HelpText => "Listedeki komutları çalıştırdıktan sonra dosyayı DXF olarak kaydeder.";

        protected bool keepCurrentVersion = true;
        protected DwgVersion version = DwgVersion.AC1024;

        public override void Run(string filename, Database db)
        {
            string saveFilename = GetDXFFilename(filename);
            try
            {
                db.DxfOut(saveFilename, 16, version);
            }
            catch (System.Exception ex)
            {
                OnError(ex);
            }
        }

        private string GetDXFFilename(string filename)
        {
            return Path.ChangeExtension(filename, ".dxf");
        }

        public override bool ShowDialog()
        {
            using (SaveDXFForm form = new SaveDXFForm())
            {
                form.DXFVersion = version;

                if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

                version = form.DXFVersion;

                return true;
            }
        }
    }
}