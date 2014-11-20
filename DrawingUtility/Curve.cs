using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;

namespace DrawingUtility
{
    public partial class DrawingUtility
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("PARABOLA", CommandFlags.UsePickSet)]
        public void DrawParabola()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            PromptPointResult p0Res;
            while (true)
            {
                PromptPointOptions p0Opts = new PromptPointOptions("\nStart Point: [Seçenekler]", "Settings");
                p0Res = doc.Editor.GetPoint(p0Opts);
                if (p0Res.Status == PromptStatus.Keyword && p0Res.StringResult == "Settings")
                {
                    PromptIntegerOptions opts = new PromptIntegerOptions("Eğri segment sayısı: ");
                    opts.AllowNone = true;
                    opts.AllowZero = false;
                    opts.AllowNegative = false;
                    opts.LowerLimit = 1;
                    opts.UpperLimit = 100;
                    opts.DefaultValue = CurveSegments;
                    opts.UseDefaultValue = true;
                    PromptIntegerResult res = doc.Editor.GetInteger(opts);
                    if (res.Status == PromptStatus.Cancel)
                    {
                        return;
                    }
                    else if (res.Status == PromptStatus.OK)
                    {
                        CurveSegments = res.Value;
                    }
                }
                else if (p0Res.Status != PromptStatus.OK)
                {
                    return;
                }
                else
                {
                    break;
                }
            }
            PromptPointOptions t0Opts = new PromptPointOptions("\nStart Tangent: ");
            t0Opts.BasePoint = p0Res.Value;
            t0Opts.UseBasePoint = true;
            PromptPointResult t0Res = doc.Editor.GetPoint(t0Opts);
            if (t0Res.Status != PromptStatus.OK) return;
            PromptPointResult p2Res = doc.Editor.GetPoint("\nEnd Point: ");
            if (p2Res.Status != PromptStatus.OK) return;

            ParabolaJig.Jig(p0Res.Value, t0Res.Value, p2Res.Value, CurveSegments);
        }

        private class ParabolaJig : EntityJig
        {
            private Point3d mp0 = new Point3d();
            private Point3d mt0 = new Point3d();
            private Point3d mp2 = new Point3d();
            private Point3d mt2 = new Point3d();
            private int mSegments = 1;

            private ParabolaJig(Entity en, Point3d p0, Point3d t0, Point3d p2, int segments)
                : base(en)
            {
                mp0 = p0;
                mt0 = t0;
                mp2 = p2;
                mt2 = mp2.Add(Vector3d.XAxis);
                mSegments = segments;
            }

            protected override bool Update()
            {
                UpdatePolyline();
                return true;
            }

            protected override SamplerStatus Sampler(JigPrompts prompts)
            {
                JigPromptPointOptions t2Opts = new JigPromptPointOptions("\nEnd Tangent: ");
                t2Opts.BasePoint = mp2;
                t2Opts.UseBasePoint = true;
                PromptPointResult t2Res = prompts.AcquirePoint(t2Opts);
                if (t2Res.Status != PromptStatus.OK) return SamplerStatus.Cancel;
                mt2 = t2Res.Value;

                return SamplerStatus.OK;
            }

            public static bool Jig(Point3d p0, Point3d t0, Point3d p2, int segments)
            {
                ParabolaJig jigger = new ParabolaJig(CreatePolyline(), p0, t0, p2, segments);

                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                PromptResult res = doc.Editor.Drag(jigger);

                if (res.Status == PromptStatus.OK)
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                    {
                        btr.AppendEntity(jigger.Entity);
                        tr.AddNewlyCreatedDBObject(jigger.Entity, true);

                        tr.Commit();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            private static Polyline CreatePolyline()
            {
                Polyline pline = new Polyline(1);
                pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);

                return pline;
            }

            private void UpdatePolyline()
            {
                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                Point3d p0 = mp0.TransformBy(ucs2wcs);
                Point3d t0 = mt0.TransformBy(ucs2wcs);
                Point3d p2 = mp2.TransformBy(ucs2wcs);
                Point3d t2 = mt2.TransformBy(ucs2wcs);
                Point3d p1 = Intersect(p0, t0, p2, t2);

                Point2dCollection points = new Point2dCollection();
                for (int i = 0; i <= mSegments; i++)
                {
                    double t = (double)i / (double)mSegments;
                    // Quadratic bezier curve with control vertices p0, p1 and p2
                    double x = (1 - t) * (1 - t) * p0.X + 2 * (1 - t) * t * p1.X + t * t * p2.X;
                    double y = (1 - t) * (1 - t) * p0.Y + 2 * (1 - t) * t * p1.Y + t * t * p2.Y;
                    points.Add(new Point2d(x, y));
                }

                Polyline pline = Entity as Polyline;
                pline.Reset(false, points.Count);
                int n = 0;
                foreach (Point2d pt in points)
                {
                    pline.AddVertexAt(n, pt, 0, 0, 0);
                    n++;
                }
            }

            private static Point3d Intersect(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
            {
                using (Line3d l1 = new Line3d(p1, p2))
                using (Line3d l2 = new Line3d(p3, p4))
                {
                    return l1.IntersectWith(l2)[0];
                }
            }
        }
    }
}
