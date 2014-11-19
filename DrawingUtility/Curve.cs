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
            PromptPointOptions t2Opts = new PromptPointOptions("\nEnd Tangent: ");
            t2Opts.BasePoint = p2Res.Value;
            t2Opts.UseBasePoint = true;
            PromptPointResult t2Res = doc.Editor.GetPoint(t2Opts);
            if (t2Res.Status != PromptStatus.OK) return;

            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
            Point3d p0 = p0Res.Value.TransformBy(ucs2wcs);
            Point3d t0 = t0Res.Value.TransformBy(ucs2wcs);
            Point3d p2 = p2Res.Value.TransformBy(ucs2wcs);
            Point3d t2 = t2Res.Value.TransformBy(ucs2wcs);
            Point3d p1 = Intersect(p0, t0, p2, t2);

            Point2dCollection points = new Point2dCollection();
            for (int i = 0; i <= CurveSegments; i++)
            {
                double t = (double)i / (double)CurveSegments;
                // Quadratic bezier curve with control vertices p0, p1 and p2
                double x = (1 - t) * (1 - t) * p0.X + 2 * (1 - t) * t * p1.X + t * t * p2.X;
                double y = (1 - t) * (1 - t) * p0.Y + 2 * (1 - t) * t * p1.Y + t * t * p2.Y;
                points.Add(new Point2d(x, y));
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                Polyline pline = new Polyline(points.Count);
                int i = 0;
                foreach (Point2d pt in points)
                {
                    pline.AddVertexAt(0, pt, 0, 0, 0);
                    i++;
                }

                btr.AppendEntity(pline);
                tr.AddNewlyCreatedDBObject(pline, true);

                tr.Commit();
            }
        }

        public static Point3d Intersect(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
        {
            using (Line3d l1 = new Line3d(p1, p2))
            using (Line3d l2 = new Line3d(p3, p4))
            {
                return l1.IntersectWith(l2)[0];
            }
        }
    }
}
