using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace XCOM.Commands.Topography
{
    public class Command_HAVUZKAZISI
    {
        private double ExcavationStepSize { get; set; }

        private double ExcavationH { get; set; }
        private double ExcavationV { get; set; }

        public Command_HAVUZKAZISI()
        {
            ExcavationStepSize = 1.0;

            ExcavationH = 1.0;
            ExcavationV = 1.0;
        }

        private bool ShowSettings()
        {
            using (ExcavationSlopeForm form = new ExcavationSlopeForm())
            {
                form.H = ExcavationH;
                form.V = ExcavationV;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    ExcavationH = form.H;
                    ExcavationV = form.V;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("HAVUZKAZISI")]
        public void PoolExcavation()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = Topography.PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!Topography.EnsureSurfaceNotEmpty(surface)) return;
            Topography topo = Topography.Instance;
            TriangleNet.Mesh mesh = (surface == Topography.SurfaceType.Original ? topo.OriginalTIN : topo.ProposedTIN);

            // Pick polyline
            bool flag = true;
            ObjectId centerlineId = ObjectId.Null;
            while (flag)
            {
                PromptEntityOptions entityOpts = new PromptEntityOptions("\nKazı tabanı [Seçenekler]:", "Settings");
                entityOpts.SetRejectMessage("\nSelect a curve.");
                entityOpts.AddAllowedClass(typeof(Curve), false);
                PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);
                if (entityRes.Status == PromptStatus.Keyword && entityRes.StringResult == "Settings")
                {
                    ShowSettings();
                    continue;
                }
                if (entityRes.Status != PromptStatus.OK)
                {
                    return;
                }

                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead))
                {
                    Curve centerline = tr.GetObject(entityRes.ObjectId, OpenMode.ForRead) as Curve;
                    if (centerline != null)
                    {
                        if (centerline.Closed)
                        {
                            centerlineId = entityRes.ObjectId;

                            tr.Commit();
                            break;
                        }
                        else
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nCurve must be closed.");
                            tr.Commit();
                        }
                    }
                }
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                Curve centerline = tr.GetObject(centerlineId, OpenMode.ForRead) as Curve;

                // Excavate
                PadExcavation ex = new PadExcavation(mesh, centerline, ExcavationStepSize);
                ExcavationSection slope = new ExcavationSection();
                slope.AddSlope(ExcavationH, ExcavationV);
                ex.AddSection(0, slope);
                ex.Excavate();

                // Draw excavation boundries
                Point3dCollection bounds = new Point3dCollection();
                bool closed = true;
                foreach (ExcavationSection section in ex.OutputSections)
                {
                    if (section.Elements[section.Elements.Count - 1].HasTopPoint)
                    {
                        bounds.Add(section.Elements[section.Elements.Count - 1].TopPoint);

                        Line line = AcadUtility.AcadEntity.CreateLine(db, section.Elements[0].BottomPoint, section.Elements[section.Elements.Count - 1].TopPoint);
                        line.ColorIndex = 11;
                        btr.AppendEntity(line);
                        tr.AddNewlyCreatedDBObject(line, true);
                    }
                    else
                    {
                        if (bounds.Count > 1)
                        {
                            Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, bounds);
                            btr.AppendEntity(pline);
                            tr.AddNewlyCreatedDBObject(pline, true);
                        }

                        closed = false;
                        bounds = new Point3dCollection();
                    }
                }
                if (bounds.Count > 1)
                {
                    Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, closed, bounds);
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                }

                tr.Commit();
            }
        }
    }
}
