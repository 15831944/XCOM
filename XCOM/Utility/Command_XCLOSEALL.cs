using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace XCOM.Commands.Utility
{
    public class Command_XCLOSEALL
    {
        [CommandMethod("XCLOSEALL", CommandFlags.Session)]
        public static void XCloseAll()
        {
            if (!CheckLicense.Check()) return;

            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptKeywordOptions kopts = new PromptKeywordOptions("\nTüm dosyalar kaydedilmeden kapatılacak. Devam edilsin mi? [Evet/Hayır] <Hayir>: ", "Yes No");
            kopts.AllowNone = true;
            kopts.Keywords.Default = "No";
            PromptResult kres = ed.GetKeywords(kopts);
            if (kres.Status != PromptStatus.OK || kres.StringResult != "Yes")
            {
                return;
            }

            foreach (Document doc in Application.DocumentManager)
            {
                if (doc.CommandInProgress != "" && doc.CommandInProgress != "XCLOSEALL")
                {
                    doc.SendStringToExecute("\x03\x03", false, false, false);
                }

                DocumentExtension.CloseAndDiscard(doc);
            }
        }
    }
}
