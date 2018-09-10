using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace XCOM.Commands.Topography
{
    public class Command_DRAPE
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("DRAPE")]
        public void Drape()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = Topography.PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!Topography.EnsureSurfaceNotEmpty(surface)) return;

            // Pick polyline
            PromptEntityOptions entityOpts = new PromptEntityOptions("\nEğri: ");
            entityOpts.SetRejectMessage("\nSelect a curve.");
            entityOpts.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);

            if (entityRes.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    Curve curve = tr.GetObject(entityRes.ObjectId, OpenMode.ForRead) as Curve;

                    Topography topo = Topography.Instance;
                    Point3dCollection points = topo.DrapeCurve(curve, surface);

                    Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, curve.Closed, points);
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                    tr.Commit();
                }
            }
        }
    }
}
