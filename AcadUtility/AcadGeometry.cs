using System;
using Autodesk.AutoCAD.Geometry;

namespace AcadUtility
{
    // Geometry utilities
    public class AcadGeometry
    {
        public static Point3d Polar(Point3d pt, double angle, double distance)
        {
            return new Point3d(pt.X + distance * Math.Cos(angle), pt.Y + distance * Math.Sin(angle), pt.Z);
        }
    }
}
