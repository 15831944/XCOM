using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;

namespace XCOM.Commands.Topography
{
    public class Command_KONTUR
    {
        private double ContourInterval { get; set; }

        public Command_KONTUR()
        {
            ContourInterval = 1;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("KONTUR")]
        public void DrawContourMap()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            // Surface type
            Topography.SurfaceType surface = Topography.PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!Topography.EnsureSurfaceNotEmpty(surface)) return;

            // Pick interval
            Topography topo = Topography.Instance;
            PromptDoubleOptions dblOpts = new PromptDoubleOptions("\nKontur aralığı: ");
            dblOpts.AllowNegative = false;
            dblOpts.AllowZero = false;
            dblOpts.DefaultValue = ContourInterval;
            dblOpts.UseDefaultValue = true;
            PromptDoubleResult dblRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(dblOpts);

            if (dblRes.Status == PromptStatus.OK)
            {
                ContourInterval = dblRes.Value;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    IEnumerable<Polyline> contours = topo.ContourMap(surface, ContourInterval);

                    foreach (Polyline pline in contours)
                    {
                        btr.AppendEntity(pline);
                        tr.AddNewlyCreatedDBObject(pline, true);
                    }
                    tr.Commit();
                }
            }
        }
    }
}
