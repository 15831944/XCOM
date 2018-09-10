using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace XCOM.Commands.Topography
{
    public class Command_HENDEKKAZISI
    {
        private double ExcavationStepSize { get; set; }

        private double Width { get; set; }
        private double ExcavationH { get; set; }
        private double ExcavationV { get; set; }

        public Command_HENDEKKAZISI()
        {
            ExcavationStepSize = 1.0;

            Width = 0;
            ExcavationH = 1.0;
            ExcavationV = 1.0;
        }

        private bool ShowSettings()
        {
            using (TrenchExcavationForm form = new TrenchExcavationForm())
            {
                form.BottomWidth = Width;
                form.H = ExcavationH;
                form.V = ExcavationV;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    ExcavationH = form.H;
                    ExcavationV = form.V;
                    Width = form.BottomWidth;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("HENDEKKAZISI")]
        public void TrenchExcavation()
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
                else if (entityRes.Status == PromptStatus.OK)
                {
                    centerlineId = entityRes.ObjectId;
                    break;
                }
                else
                {
                    return;
                }
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                Curve centerline = tr.GetObject(centerlineId, OpenMode.ForRead) as Curve;

                // Excavate
                TrenchExcavation ex = new TrenchExcavation(mesh, centerline, Width, ExcavationStepSize);
                ExcavationSection slope = new ExcavationSection();
                slope.AddSlope(ExcavationH, ExcavationV);
                ex.AddSection(0, slope);
                ex.Excavate();

                // Draw excavation boundries
                Point3dCollection bottombounds = new Point3dCollection();
                Point3dCollection topbounds = new Point3dCollection();
                bool closed = true;
                bool alt = false;
                foreach (ExcavationSection section in ex.OutputSections)
                {
                    if (section.Elements[section.Elements.Count - 1].HasTopPoint)
                    {
                        bottombounds.Add(section.Elements[0].BottomPoint);
                        topbounds.Add(section.Elements[section.Elements.Count - 1].TopPoint);

                        Point3d pt1 = section.Elements[0].BottomPoint;
                        Point3d pt2 = section.Elements[section.Elements.Count - 1].TopPoint;
                        if (alt) pt1 = pt1 + (pt2 - pt1) / 2;
                        Line line = AcadUtility.AcadEntity.CreateLine(db, pt1, pt2);
                        line.ColorIndex = 11;
                        btr.AppendEntity(line);
                        tr.AddNewlyCreatedDBObject(line, true);
                    }
                    else
                    {
                        if (bottombounds.Count > 1)
                        {
                            Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, bottombounds);
                            btr.AppendEntity(pline);
                            tr.AddNewlyCreatedDBObject(pline, true);
                        }
                        if (topbounds.Count > 1)
                        {
                            Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, topbounds);
                            btr.AppendEntity(pline);
                            tr.AddNewlyCreatedDBObject(pline, true);
                        }

                        closed = false;
                        topbounds = new Point3dCollection();
                    }

                    alt = !alt;
                }
                if (bottombounds.Count > 1)
                {
                    Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, bottombounds);
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                }
                if (topbounds.Count > 1)
                {
                    Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, closed, topbounds);
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                }

                tr.Commit();
            }
        }
    }
}
