﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using AcadUtility;

namespace XCOM.Commands.RoadDesign
{
    /// <summary>
    /// Represents original and modified surface topography.
    /// </summary>
    public class Topography
    {
        /// <summary>
        /// Maximum length of discretized curve segments.
        /// </summary>
        public double CurveDiscritizationLength { get; private set; }

        /// <summary>
        /// The type of surface being operated on.
        /// </summary>
        public enum SurfaceType
        {
            Original,
            Proposed
        }

        private TriangleNet.Mesh originalSurface;
        private TriangleNet.Mesh proposedSurface;

        /// <summary>
        /// Original surface.
        /// </summary>
        public TriangleNet.Mesh OriginalTIN { get { return originalSurface; } }
        /// <summary>
        /// Modified surface.
        /// </summary>
        public TriangleNet.Mesh ProposedTIN { get { return proposedSurface; } }

        /// <summary>
        /// Creates a new topography.
        /// </summary>
        public Topography(double curveDiscritizationLength = 1.0)
        {
            CurveDiscritizationLength = curveDiscritizationLength;

            originalSurface = new TriangleNet.Mesh();
            proposedSurface = new TriangleNet.Mesh();
        }

        /// <summary>
        /// Creates a surface from the given points.
        /// </summary>
        /// <param name="points">Input points</param>
        /// <param name="surface">The type of surface being operated on</param>
        public void SurfaceFromPoints(Point3dCollection points, SurfaceType surface)
        {
            TriangleNet.Mesh mesh = new TriangleNet.Mesh();
            TriangleNet.Geometry.InputGeometry geometry = new TriangleNet.Geometry.InputGeometry(points.Count);
            foreach (Point3d point in points)
            {
                geometry.AddPoint(point.X, point.Y, 0, point.Z);
            }
            mesh.Triangulate(geometry);

            if (surface == SurfaceType.Original)
                originalSurface = mesh;
            else
                proposedSurface = mesh;
        }

        /// <summary>
        /// Calculates the minimum and maximum elevations on the given surface.
        /// </summary>
        /// <param name="surface">The type of surface being operated on</param>
        /// <param name="zmin">Minimum surface elevation</param>
        /// <param name="zmax">Maximum surface elevation</param>
        public void ElevationLimits(SurfaceType surface, out double zmin, out double zmax)
        {
            zmin = double.MaxValue;
            zmax = double.MinValue;
            foreach (TriangleNet.Data.Vertex v in ((surface == SurfaceType.Original) ? originalSurface : proposedSurface).Vertices)
            {
                double z = v.Attributes[0];
                zmin = Math.Min(zmin, z);
                zmax = Math.Max(zmax, z);
            }
        }

        /// <summary>
        /// Drapes the curve over the given surface.
        /// </summary>
        /// <param name="curve">The curve to drape</param>
        /// <param name="surface">The type of surface being operated on</param>
        public Point3dCollection DrapeCurve(Curve curve, SurfaceType surface)
        {
            SortedList<double, Point3d> results = new SortedList<double, Point3d>();

            // Divide the curve into equal segments
            double sp = curve.StartParam;
            double ep = curve.EndParam;
            double len = curve.GetLength();
            int nmax = (int)Math.Ceiling(len / CurveDiscritizationLength);
            double distStep = len / ((double)nmax);

            // Intersects slope vectors with surface triangles
            Vector3d dir = new Vector3d(0, 0, -1);
            foreach (TriangleNet.Data.Triangle tri in ((surface == SurfaceType.Original) ? originalSurface : proposedSurface).Triangles)
            {
                TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                TriangleNet.Data.Vertex v3 = tri.GetVertex(2);
                Point3d p1 = new Point3d(v1.X, v1.Y, v1.Attributes[0]);
                Point3d p2 = new Point3d(v2.X, v2.Y, v2.Attributes[0]);
                Point3d p3 = new Point3d(v3.X, v3.Y, v3.Attributes[0]);

                double dist = 0.0;
                for (int i = 0; i <= (curve.Closed ? nmax - 1 : nmax); i++)
                {
                    double param = curve.GetParameterAtDistance(dist);
                    if (param < sp) param = sp;
                    if (param > ep) param = ep;
                    Point3d pt = curve.GetPointAtParameter(param);

                    double t = 0;
                    Point3d ptOut = Point3d.Origin;

                    if (RayTriangleIntersection(pt, dir, p1, p2, p3, out t, out ptOut))
                    {
                        results.Add(dist, ptOut);
                    }

                    dist += distStep;
                }
            }

            return new Point3dCollection(results.Values.ToArray());
        }

        /// <summary>
        /// Returns the contour polylines at the given elevation.
        /// </summary>
        /// <param name="surface">The type of surface being operated on</param>
        /// <param name="z">Elevation</param>
        public IEnumerable<Polyline> ContourAt(SurfaceType surface, double z)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            Point3d planePoint = new Point3d(0, 0, z);
            Vector3d planeNormal = Vector3d.ZAxis;

            // List of segments 
            Queue<Tuple<Point3d, Point3d>> segments = new Queue<Tuple<Point3d, Point3d>>();
            foreach (TriangleNet.Data.Triangle tri in ((surface == SurfaceType.Original) ? originalSurface : proposedSurface).Triangles)
            {
                TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                TriangleNet.Data.Vertex v3 = tri.GetVertex(2);
                Point3d p1 = new Point3d(v1.X, v1.Y, v1.Attributes[0]);
                Point3d p2 = new Point3d(v2.X, v2.Y, v2.Attributes[0]);
                Point3d p3 = new Point3d(v3.X, v3.Y, v3.Attributes[0]);

                Point3d ptOut1 = Point3d.Origin;
                Point3d ptOut2 = Point3d.Origin;
                if (PlaneTriangleIntersection(planePoint, planeNormal, p1, p2, p3, out ptOut1, out ptOut2))
                {
                    segments.Enqueue(new Tuple<Point3d, Point3d>(ptOut1, ptOut2));
                }
            }

            // Create curves from segments
            double epsilon = 0.000001;
            Tolerance tol = new Tolerance(epsilon, epsilon);
            List<Polyline> contours = new List<Polyline>();
            while (segments.Count > 0)
            {
                LinkedList<Point3d> curvePoints = new LinkedList<Point3d>();
                Tuple<Point3d, Point3d> firstSegment = segments.Dequeue();
                curvePoints.AddFirst(firstSegment.Item1);
                curvePoints.AddLast(firstSegment.Item2);
                bool added = false;
                do
                {
                    int n = segments.Count;
                    added = false;
                    for (int i = 0; i < n; i++)
                    {
                        Tuple<Point3d, Point3d> segment = segments.Dequeue();
                        Point3d p1 = segment.Item1;
                        Point3d p2 = segment.Item2;
                        Point3d startPoint = curvePoints.First.Value;
                        Point3d endPoint = curvePoints.Last.Value;
                        if (startPoint.IsEqualTo(p1, tol))
                        {
                            curvePoints.AddFirst(p2);
                            added = true;
                        }
                        else if (startPoint.IsEqualTo(p2, tol))
                        {
                            curvePoints.AddFirst(p1);
                            added = true;
                        }
                        else if (endPoint.IsEqualTo(p1, tol))
                        {
                            curvePoints.AddLast(p2);
                            added = true;
                        }
                        else if (endPoint.IsEqualTo(p2, tol))
                        {
                            curvePoints.AddLast(p1);
                            added = true;
                        }
                        else
                            segments.Enqueue(segment);
                    }
                } while (added);
                Polyline pline = AcadEntity.CreatePolyLine(db, curvePoints.First().IsEqualTo(curvePoints.Last(), tol), curvePoints.ToArray());
                pline.ColorIndex = 31;
                pline.Elevation = z;
                contours.Add(pline);
            }

            return contours;
        }

        /// <summary>
        /// Returns the contour polylines.
        /// </summary>
        /// <param name="surface">The type of surface being operated on</param>
        /// <param name="interval">Contour interval</param>
        public IEnumerable<Polyline> ContourMap(SurfaceType surface, double interval)
        {
            double zmin = 0;
            double zmax = 0;
            ElevationLimits(surface, out zmin, out zmax);
            zmin = Math.Ceiling(zmin / interval);
            zmax = Math.Floor(zmax / interval);
            int count = (int)((zmax - zmin) / interval);
            double z = zmin;

            List<Polyline> contours = new List<Polyline>();

            for (int i = 0; i < count; i++)
            {
                IEnumerable<Polyline> contoursAtLevel = ContourAt(surface, z);
                contours.AddRange(contoursAtLevel);
                z = z + (double)interval;
            }

            return contours;
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

        /// <summary>
        /// Ray-Plane Intersection Test Routine
        /// 
        /// http://geomalgorithms.com/a06-_intersect-2.html
        /// 
        /// The parametric equation of the line is:
        /// point(t) = p + t * d
        /// where
        /// p is a point in the line
        /// d is a vector that provides the line's direction
        /// 
        /// If the plane passes through point p0 with normal n
        /// The intersection occurs at:
        /// t = n . (p0 - p) / (n . d)
        /// </summary>
        private bool RayPlaneIntersection(Point3d rayStart, Vector3d rayDir, Point3d planePoint, Vector3d planeDir, out double t, out Point3d pt)
        {
            t = 0;
            pt = Point3d.Origin;

            double epsilon = 0.000001;

            double det = planeDir.DotProduct(rayDir);

            if (det > -epsilon && det < epsilon) return false;

            double dist = planeDir.DotProduct(planePoint - rayStart);

            t = dist / det;

            /* intersection point from line equation: point(t) = p + t * d */
            pt = rayStart + t * rayDir;

            return true;
        }


        /// <summary>
        /// Plane-Triangle Intersection Test Routine
        /// 
        /// http://www.realtimerendering.com/intersections.html
        /// </summary>
        private bool PlaneTriangleIntersection(Point3d planeStart, Vector3d planeNormal, Point3d vert0, Point3d vert1, Point3d vert2, out Point3d pt1, out Point3d pt2)
        {
            pt1 = Point3d.Origin;
            pt2 = Point3d.Origin;

            Vector3d edge1 = vert1 - vert0;
            Vector3d edge2 = vert2 - vert0;
            Vector3d edge3 = vert2 - vert1;
            Point3d p1, p2, p3;
            double t1, t2, t3;

            bool int1 = RayPlaneIntersection(vert0, edge1, planeStart, planeNormal, out t1, out p1);
            bool int2 = RayPlaneIntersection(vert0, edge2, planeStart, planeNormal, out t2, out p2);
            bool int3 = RayPlaneIntersection(vert1, edge3, planeStart, planeNormal, out t3, out p3);

            // Disregard intersections outside vertices
            if (t1 < 0.0 || t1 > 1.0) int1 = false;
            if (t2 < 0.0 || t2 > 1.0) int2 = false;
            if (t3 < 0.0 || t3 > 1.0) int3 = false;

            // intersection points
            if (int1 && int2)
            {
                pt1 = p1;
                pt2 = p2;
                return true;
            }
            else if (int1 && int3)
            {
                pt1 = p1;
                pt2 = p3;
                return true;
            }
            else if (int2 && int3)
            {
                pt1 = p2;
                pt2 = p3;
                return true;
            }

            return false;
        }
    }
}
