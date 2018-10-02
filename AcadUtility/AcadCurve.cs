using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace AcadUtility
{
    // Curve utilities
    public static class AcadCurve
    {
        /// <summary>
        /// Determines the length of the curve.
        /// </summary>
        public static double GetLength(this Curve curve)
        {
            if (curve is Ray || curve is Xline)
            {
                return double.PositiveInfinity;
            }

            return (curve.GetDistanceAtParameter(curve.EndParam) - curve.GetDistanceAtParameter(curve.StartParam));
        }

        /// <summary>
        /// Calculates a normal vector of the curve at the given point
        /// on the XY plane.
        /// </summary>
        public static Vector3d GetNormalVector(this Curve curve, double param)
        {
            return curve.GetNormalVector(param, new Plane(Point3d.Origin, Vector3d.ZAxis));
        }

        /// <summary>
        /// Calculates a normal vector of the curve at the given point
        /// on the given plane. The normal is always in the plane of the curve
        /// and points to the outside of the curve.
        /// </summary>
        public static Vector3d GetNormalVector(this Curve curve, double param, Plane plane)
        {
            Vector3d deriv = curve.GetFirstDerivative(param);
            Vector3d normal = deriv.CrossProduct(plane.Normal);
            normal /= normal.Length;
            Point3d pt = curve.GetPointAtParameter(param);

            // Check if the normal points to inside the curve (ray-casting)
            Ray ray = new Ray();
            ray.BasePoint = pt;
            ray.SecondPoint = pt + normal;
            Point3dCollection pts = new Point3dCollection();
            ray.IntersectWith(curve, Intersect.OnBothOperands, plane, pts, IntPtr.Zero, IntPtr.Zero);

            // Remove duplicate points
            Point3dCollection toRemove = new Point3dCollection();
            for (int i = 0; i < pts.Count; i++)
            {
                for (int j = 0; j < pts.Count; j++)
                {
                    if (i != j)
                    {
                        if (pts[i] == pts[j] && !toRemove.Contains(pts[i]))
                        {
                            toRemove.Add(pts[i]);
                        }
                    }
                }
            }
            foreach (Point3d p in toRemove)
            {
                pts.Remove(p);
            }

            // Flip if required
            if (pts.Count % 2 == 0)
            {
                normal = normal.RotateBy(Math.PI, plane.Normal);
            }

            return normal;
        }

        /// <summary>
        /// Offsets the curve in the direction of the given point
        /// </summary>
        public static DBObjectCollection GetOffsetCurves(this Curve curve, Point3d point)
        {
            Vector3d normal = Vector3d.ZAxis;
            Vector3d viewDir = ((Point3d)Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("VIEWDIR")).GetAsVector();
            if (curve.IsPlanar)
            {
                Plane plane = curve.GetPlane();
                normal = plane.Normal;
                point = point.Project(plane, viewDir);
            }

            Point3d pointOnCurve = curve.GetClosestPointTo(point, true);
            Vector3d offsetDir = point - pointOnCurve;
            double offsetDist = offsetDir.Length;
            Vector3d deriv = curve.GetFirstDerivative(pointOnCurve);
            if (normal.CrossProduct(deriv).DotProduct(offsetDir) > 0)
            {
                offsetDist = -offsetDist;
            }

            return curve.GetOffsetCurves(offsetDist);
        }

        /// <summary>
        /// Trims the curve at its start and end points
        /// </summary>
        public static Curve GetTrimmedCurve(this Curve curve, Point3d startPoint, Point3d endPoint, bool extend)
        {
            startPoint = curve.GetClosestPointTo(startPoint, extend);
            endPoint = curve.GetClosestPointTo(endPoint, extend);
            var splitCurves = curve.GetSplitCurves(new Point3dCollection(new Point3d[] { startPoint, endPoint }));
            if (splitCurves.Count == 1)
            {
                return splitCurves[0] as Curve;
            }
            else if (splitCurves.Count == 2)
            {
                if ((splitCurves[0] as Curve).StartPoint.IsEqualTo(startPoint))
                {
                    return splitCurves[0] as Curve;
                }
                else
                {
                    return splitCurves[1] as Curve;
                }
            }
            else if (splitCurves.Count == 3)
            {
                return splitCurves[1] as Curve;
            }

            throw new InvalidOperationException("Could not split curve.");
        }
    }
}
