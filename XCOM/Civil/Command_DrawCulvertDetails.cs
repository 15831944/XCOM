using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;

namespace XCOM.Commands.Civil
{
    public class Command_DrawCulvertDetails
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("TEKGOZMENFEZDONATI")]
        public static void DrawSingleCellCulvertDetails()
        {
            if (!CheckLicense.Check()) return;

            using (DrawSingleCellCulvertForm form = new DrawSingleCellCulvertForm())
            {
                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                    Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                    PromptPointResult res = doc.Editor.GetPoint("Sondaj çizimi başlangıç noktası: ");
                    Point3d pt = res.Value;
                }
            }
        }
    }
}
