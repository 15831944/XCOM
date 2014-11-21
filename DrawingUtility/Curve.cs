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

                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
                Matrix3d wcs2ucs = Matrix3d.AlignCoordinateSystem(db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir), Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis);
                mt2 = t2Res.Value.TransformBy(wcs2ucs);

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
                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                Polyline pline = new Polyline(1);
                pline.Normal = db.Ucsxdir.CrossProduct(db.Ucsydir);
                pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);

                return pline;
            }

            private void UpdatePolyline()
            {
                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                Plane ucsPlane = new Plane(Point3d.Origin, Vector3d.ZAxis);
                Point2d p1 = Intersect(mp0.Convert2d(ucsPlane), mt0.Convert2d(ucsPlane), mp2.Convert2d(ucsPlane), mt2.Convert2d(ucsPlane));
                
                Point3dCollection points = new Point3dCollection();
                for (int i = 0; i <= mSegments; i++)
                {
                    double t = (double)i / (double)mSegments;
                    // Quadratic bezier curve with control vertices p0, p1 and p2
                    double x = (1 - t) * (1 - t) * mp0.X + 2 * (1 - t) * t * p1.X + t * t * mp2.X;
                    double y = (1 - t) * (1 - t) * mp0.Y + 2 * (1 - t) * t * p1.Y + t * t * mp2.Y;
                    points.Add(new Point3d(x, y, 0)); // Coordinates in UCS
                }

                Polyline pline = Entity as Polyline;
                Plane plinePlane = new Plane(Point3d.Origin, pline.Normal);
                pline.Reset(false, points.Count);
                foreach (Point3d pt in points)
                {
                    Point3d wcsPt = pt.TransformBy(ucs2wcs); // Convert to WCS
                    Point2d ecsPt = plinePlane.ParameterOf(wcsPt); // Convert to ECS
                    pline.AddVertexAt(pline.NumberOfVertices, ecsPt, 0, 0, 0);
                }
            }

            private static Point2d Intersect(Point2d p1, Point2d p2, Point2d p3, Point2d p4)
            {
                Point2d midPoint = new Point2d((p1.X + p3.X) / 2, (p1.Y + p3.Y) / 2);
                using (Line2d l1 = new Line2d(p1, p2))
                using (Line2d l2 = new Line2d(p3, p4))
                {
                    if (l1 != null && l2 != null)
                    {
                        Point2d[] intersections = l1.IntersectWith(l2);
                        if (intersections != null && intersections.Length > 0)
                            return intersections[0];
                        else
                            return midPoint;
                    }
                    else
                    {
                        return midPoint;
                    }
                }
            }
        }
    }
}
