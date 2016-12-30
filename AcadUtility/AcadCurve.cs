using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

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
                return double.PositiveInfinity;

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
                        if (pts[i] == pts[j] && !toRemove.Contains(pts[i])) toRemove.Add(pts[i]);
                    }
                }
            }
            foreach(Point3d p in toRemove )
            {
                pts.Remove(p);
            }

            // Flip if required
            if (pts.Count % 2 == 0) normal = normal.RotateBy(Math.PI, plane.Normal);

            return normal;
        }
    }
}
