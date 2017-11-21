using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;
using AcadUtility;

namespace XCOM.Commands.RoadDesign
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

        private bool ShowSettingsPoolExcavation()
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
                    ShowSettingsPoolExcavation();
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
                List<SlopeSection> sections = new List<SlopeSection>();

                // Divide the curve into equal segments and calculate the 3D slope at each point
                Curve centerline = tr.GetObject(centerlineId, OpenMode.ForRead) as Curve;
                double len = centerline.GetLength();
                int nmax = (int)Math.Ceiling(len / ExcavationStepSize);
                double dist = 0.0;
                double distStep = len / ((double)nmax);
                for (int i = 0; i < nmax; i++)
                {
                    double param = centerline.GetParameterAtDistance(dist);
                    Point3d pt = centerline.GetPointAtParameter(param);
                    Vector3d slope = SlopeAtParam(centerline, param, ExcavationH, ExcavationV);

                    SlopeSection s = new SlopeSection(pt, slope);

                    sections.Add(s);

                    dist += distStep;
                }

                // Intersects slope vectors with surface triangles
                foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
                {
                    TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                    TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                    TriangleNet.Data.Vertex v3 = tri.GetVertex(2);

                    foreach (SlopeSection section in sections)
                    {
                        double t;
                        Point3d pt;
                        if (RayTriangleIntersection(section.BottomPoint, section.Slope,
                            new Point3d(v1.X, v1.Y, v1.Attributes[0]), new Point3d(v2.X, v2.Y, v2.Attributes[0]), new Point3d(v3.X, v3.Y, v3.Attributes[0]),
                            out t, out pt))
                        {
                            if (!section.HasTopPoint || section.CurrentRatio > t)
                            {
                                section.TopPoint = pt;
                                section.CurrentRatio = t;
                            }
                        }
                    }
                }

                // Draw excavation boundries
                Point3dCollection bounds = new Point3dCollection();
                bool closed = true;
                foreach (SlopeSection section in sections)
                {
                    if (section.HasTopPoint)
                    {
                        bounds.Add(section.TopPoint);

                        Line line = AcadUtility.AcadEntity.CreateLine(db, section.BottomPoint, section.TopPoint);
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

        private class SlopeSection
        {
            private Point3d bottomPoint;
            private Point3d topPoint;
            private Vector3d slope;
            private double currentRatio;
            private bool hasTopPoint;

            public Point3d BottomPoint { get { return bottomPoint; } }
            public Point3d TopPoint { get { return topPoint; } set { topPoint = value; hasTopPoint = true; } }
            public Vector3d Slope { get { return slope; } }
            public double CurrentRatio { get { return currentRatio; } set { currentRatio = value; } }
            public bool HasTopPoint { get { return hasTopPoint; } }

            public SlopeSection(Point3d pt, Vector3d s)
            {
                bottomPoint = pt;
                slope = s;
                hasTopPoint = false;
                currentRatio = 0.0;
            }
        }

        private Vector3d SlopeAtParam(Curve curve, double param, double h, double v)
        {
            Vector3d normal = curve.GetNormalVector(param);
            double xyLen = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
            Vector3d slope = new Vector3d(normal.X / xyLen * h, normal.Y / xyLen * h, v);

            return slope / slope.Length;
        }

        /// <summary>
        /// Ray-Triangle Intersection Test Routines
        /// Different optimizations of my and Ben Trumbore's
        /// code from journals of graphics tools(JGT)
        /// http://www.acm.org/jgt/
        /// by Tomas Moller, May 2000
        /// 
        /// the original jgt code
        /// http://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/raytri/raytri.c
        /// 
        /// A point in a triangle can be defined as:
        /// point(u,v) = (1-u-v)*p0 + u*p1 + v*p2
        /// where
        /// p0, p1, p2 are the vertices of the triangle
        /// u >= 0
        /// v >= 0
        /// u + v &lt;= 1.0
        /// 
        /// The parametric equation of the line is:
        /// point(t) = p + t * d
        /// where
        /// p is a point in the line
        /// d is a vector that provides the line's direction
        /// </summary>
        private bool RayTriangleIntersection(Point3d rayStart, Vector3d rayDir, Point3d vert0, Point3d vert1, Point3d vert2, out double t, out Point3d pt)
        {
            t = 0;
            pt = Point3d.Origin;

            double epsilon = 0.000001;

            /* find vectors for two edges sharing vert0 */
            Vector3d edge1 = vert1 - vert0;
            Vector3d edge2 = vert2 - vert0;

            /* begin calculating determinant - also used to calculate U parameter */
            Vector3d pvec = rayDir.CrossProduct(edge2);

            /* if determinant is near zero, ray lies in plane of triangle */
            double det = edge1.DotProduct(pvec);

            if (det > -epsilon && det < epsilon) return false;
            double inv_det = 1.0 / det;

            /* calculate distance from vert0 to ray origin */
            Vector3d tvec = rayStart - vert0;

            /* calculate U parameter and test bounds */
            double u = tvec.DotProduct(pvec) * inv_det;
            if (u < 0.0 || u > 1.0) return false;

            /* prepare to test V parameter */
            Vector3d qvec = tvec.CrossProduct(edge1);

            /* calculate V parameter and test bounds */
            double v = rayDir.DotProduct(qvec) * inv_det;
            if (v < 0.0 || u + v > 1.0) return false;

            /* calculate t, ray intersects triangle */
            t = edge2.DotProduct(qvec) * inv_det;

            /* intersection point from line equation: point(t) = p + t * d */
            pt = rayStart + t * rayDir;

            return true;
        }
    }
}
