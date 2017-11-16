using System;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadUtility
{
    // Geometry utilities
    public class AcadGeometry
    {
        public static Point3d Polar(Point3d pt, double angle, double distance)
        {
            return new Point3d(pt.X + distance * Math.Cos(angle), pt.Y + distance * Math.Sin(angle), pt.Z);
        }

        public static Extents2d Limits(Point2dCollection points)
        {
            double xmin = double.MaxValue;
            double xmax = double.MinValue;
            double ymin = double.MaxValue;
            double ymax = double.MinValue;
            foreach (Point2d pt in points)
            {
                xmin = Math.Min(xmin, pt.X);
                xmax = Math.Max(xmax, pt.X);
                ymin = Math.Min(ymin, pt.Y);
                ymax = Math.Max(ymax, pt.Y);
            }
            return new Extents2d(xmin, ymin, xmax, ymax);
        }

        public static Extents2d Limits(Extents2d baseExtents, Point2dCollection points)
        {
            Extents2d ex = Limits(points);
            return new Extents2d(new Point2d(Math.Min(ex.MinPoint.X, baseExtents.MinPoint.X), Math.Min(ex.MinPoint.Y, baseExtents.MinPoint.Y)),
                new Point2d(Math.Max(ex.MaxPoint.X, baseExtents.MaxPoint.X), Math.Max(ex.MaxPoint.Y, baseExtents.MaxPoint.Y)));
        }

        public static Extents3d Limits(Point3dCollection points)
        {
            double xmin = double.MaxValue;
            double xmax = double.MinValue;
            double ymin = double.MaxValue;
            double ymax = double.MinValue;
            double zmin = double.MaxValue;
            double zmax = double.MinValue;
            foreach (Point3d pt in points)
            {
                xmin = Math.Min(xmin, pt.X);
                xmax = Math.Max(xmax, pt.X);
                ymin = Math.Min(ymin, pt.Y);
                ymax = Math.Max(ymax, pt.Y);
                zmin = Math.Min(zmin, pt.Z);
                zmax = Math.Max(zmax, pt.Z);
            }
            return new Extents3d(new Point3d(xmin, ymin, zmin), new Point3d(xmax, ymax, zmax));
        }

        public static Extents3d Limits(Extents3d baseExtents, Point3dCollection points)
        {
            Extents3d ex = Limits(points);
            return new Extents3d(new Point3d(Math.Min(ex.MinPoint.X, baseExtents.MinPoint.X), Math.Min(ex.MinPoint.Y, baseExtents.MinPoint.Y), Math.Min(ex.MinPoint.Z, baseExtents.MinPoint.Z)),
                new Point3d(Math.Max(ex.MaxPoint.X, baseExtents.MaxPoint.X), Math.Max(ex.MaxPoint.Y, baseExtents.MaxPoint.Y), Math.Max(ex.MaxPoint.Z, baseExtents.MaxPoint.Z)));
        }
    }
}
