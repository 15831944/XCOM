using System;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM.Commands.XCommand
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

        public void Run(string filename, Database db)
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

        public bool ShowDialog()
        {
            using (SaveDXFForm form = new SaveDXFForm())
            {
                form.DXFVersion = version;

                if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

                version = form.DXFVersion;

                return true;
            }
        }

        public event EventHandler<ActionProgressEventArgs> Progress;
        public event EventHandler<ActionErrorEventArgs> Error;

        protected void OnProgress(string message)
        {
            Progress?.Invoke(this, new ActionProgressEventArgs(message));
        }

        protected void OnError(Exception error)
        {
            Error?.Invoke(this, new ActionErrorEventArgs(error));
        }
    }
}