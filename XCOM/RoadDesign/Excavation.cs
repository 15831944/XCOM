using AcadUtility;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace XCOM.Commands.RoadDesign
{
    public abstract class Excavation
    {
        public enum Side
        {
            Right,
            Left
        }

        private static double LengthTolerance = 0.0001;

        public TriangleNet.Mesh Mesh { get; protected set; }
        public double ExcavationStepSize { get; protected set; }
        public Curve ExcavationBottom { get; protected set; }
        public List<ExcavationSection> OutputSections { get; protected set; }

        protected List<double> distances;
        protected List<ExcavationSection> sections;

        public Excavation(TriangleNet.Mesh surfaceMesh, Curve excavationBottom, double excavationStepSize = 1)
        {
            ExcavationBottom = excavationBottom;
            OutputSections = new List<RoadDesign.ExcavationSection>();
            ExcavationStepSize = excavationStepSize;

            Mesh = surfaceMesh;
            distances = new List<double>();
            sections = new List<ExcavationSection>();
        }

        public abstract void Excavate();

        public void AddSection(double distance, ExcavationSection section)
        {
            distances.Add(distance);
            sections.Add(section);
        }

        public void ClearSections()
        {
            distances.Clear();
            sections.Clear();
        }

        protected ExcavationSection GetSection(Curve excavationBottom, double distance)
        {
            double curveLength = excavationBottom.GetLength();

            if (sections.Count == 0) throw new InvalidOperationException("No sections defined.");

            if (sections.Count == 1) return sections[0];

            // Find the section before and closest to the given distance
            int startIndex = 0;
            double minDiff = double.MaxValue;
            for (int i = 0; i < distances.Count; i++)
            {
                double diff = distance - distances[i];
                if (diff < 0) diff += curveLength;
                if (diff < minDiff)
                {
                    minDiff = diff;
                    startIndex = i;
                }
            }
            if (Math.Abs(minDiff) < LengthTolerance) return sections[startIndex];

            // Find the section after and closest to the given distance
            int endIndex = 0;
            minDiff = double.MaxValue;
            for (int i = 0; i < distances.Count; i++)
            {
                double diff = distances[i] - distance;
                if (diff < 0) diff += curveLength;
                if (diff < minDiff)
                {
                    minDiff = diff;
                    endIndex = i;
                }
            }
            if (Math.Abs(minDiff) < LengthTolerance) return sections[endIndex];

            // Interpolate between start and end sections
            double startDistance = distances[startIndex];
            double endDistance = distances[endIndex];
            if (startDistance > endDistance) startDistance -= curveLength;
            if (Math.Abs(endDistance - startDistance) < LengthTolerance) return sections[startIndex];

            double ratio = (distance - startDistance) / (endDistance - startDistance);
            return ExcavationSection.Interpolate(sections[startIndex], sections[endIndex], ratio);
        }

        protected List<ExcavationSection> ExcavateCurve(Side slopeSide = Side.Right, double offset = 0)
        {
            double len = ExcavationBottom.GetLength();
            int nmax = (int)Math.Ceiling(len / ExcavationStepSize);
            double dist = 0.0;
            double distStep = len / ((double)nmax);
            if (!ExcavationBottom.Closed) nmax++;

            List<ExcavationSection> outputSections = new List<ExcavationSection>();
            for (int i = 0; i < nmax; i++)
            {
                double param = dist > len ? ExcavationBottom.EndParam : dist < 0 ? ExcavationBottom.StartParam : ExcavationBottom.GetParameterAtDistance(dist);
                Point3d bottomPoint = ExcavationBottom.GetPointAtParameter(param);
                Vector3d normal = ExcavationBottom.GetNormalVector(param);
                ExcavationSection section = GetSection(ExcavationBottom, dist);
                Point3d pt = bottomPoint + (slopeSide == Side.Right ? 1 : -1) * normal * Math.Abs(offset);
                Vector3d dir = (slopeSide == Side.Right ? 1 : -1) * normal;
                ExcavationSection outSection = ExcavateSlopeAtPoint(Mesh, section, pt, dir);
                outputSections.Add(outSection);
                dist += distStep;
            }

            return outputSections;
        }

        protected static ExcavationSection ExcavateSlopeAtPoint(TriangleNet.Mesh surfaceMesh, ExcavationSection section, Point3d bottomPoint, Vector3d normal)
        {
            double xyLen = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
            ExcavationSection outSection = new ExcavationSection();

            foreach (ExcavationSectionElement inputelement in section.Elements)
            {
                ExcavationSectionElement element = (ExcavationSectionElement)inputelement.Clone();
                element.HasIntersection = false;
                element.HasTopPoint = false;
                element.CurrentRatio = double.MaxValue;
                element.BottomPoint = bottomPoint;

                // Calculate 3D slope
                Vector3d slope = new Vector3d(normal.X / xyLen * element.H, normal.Y / xyLen * element.H, element.V);
                slope = slope / slope.Length;
                element.Slope = slope;

                // Intersects slope vectors with surface triangles
                foreach (TriangleNet.Data.Triangle tri in surfaceMesh.Triangles)
                {
                    TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                    TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                    TriangleNet.Data.Vertex v3 = tri.GetVertex(2);

                    // Intersect the 3D ray from excavation bottom with the triangle
                    double t;
                    Point3d topPoint;
                    bool intersect = RayTriangleIntersection(bottomPoint, slope,
                        new Point3d(v1.X, v1.Y, v1.Attributes[0]), new Point3d(v2.X, v2.Y, v2.Attributes[0]), new Point3d(v3.X, v3.Y, v3.Attributes[0]),
                        out t, out topPoint);
                    double xyDist = double.MaxValue;
                    double zDist = double.MaxValue;
                    if (intersect)
                    {
                        xyDist = new Vector3d(topPoint.X - bottomPoint.X, topPoint.Y - bottomPoint.Y, 0).Length;
                        zDist = topPoint.Z - bottomPoint.Z;
                    }
                    if (!intersect ||
                        (element.HeightLimit > 0 && zDist > element.HeightLimit) ||
                        (element.LengthLimit > 0 && xyDist > element.LengthLimit))
                    {
                        t = double.MaxValue;
                    }
                    if (t < element.CurrentRatio)
                    {
                        element.CurrentRatio = t;
                        element.TopPoint = topPoint;
                        element.HasIntersection = true;
                    }
                }

                // Create the output section
                if (element.HasIntersection)
                {
                    // This element intersected the surface
                    // Add to output section and end calculation
                    element.HasTopPoint = true;
                    outSection.Elements.Add(element);
                    break;
                }
                else
                {
                    // No intersections
                    // Add to output section and continue calculation
                    // Calculate the top point for this element (bottom point for the next element)
                    if (element.HeightLimit > 0)
                    {
                        element.TopPoint = element.BottomPoint + slope * element.HeightLimit / element.V;
                        element.HasTopPoint = true;
                    }
                    else if (element.LengthLimit > 0)
                    {
                        element.TopPoint = element.BottomPoint + slope * element.LengthLimit / element.H;
                        element.HasTopPoint = true;
                    }
                    else
                    {
                        element.HasTopPoint = false;
                    }

                    outSection.Elements.Add(element);
                    bottomPoint = element.TopPoint;
                }
            }

            return outSection;
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
        /// u + v <= 1.0
        /// 
        /// The parametric equation of the line is:
        /// point(t) = p + t * d
        /// where
        /// p is a point in the line
        /// d is a vector that provides the line's direction
        /// </summary>
        protected static bool RayTriangleIntersection(Point3d rayStart, Vector3d rayDir, Point3d vert0, Point3d vert1, Point3d vert2, out double t, out Point3d pt)
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

    public class PadExcavation : Excavation
    {
        public PadExcavation(TriangleNet.Mesh surfaceMesh, Curve excavationBottom, double excavationStepSize = 1) : base(surfaceMesh, excavationBottom, excavationStepSize)
        {
            ;
        }

        public override void Excavate()
        {
            OutputSections = ExcavateCurve();
        }
    }

    public class TrenchExcavation : Excavation
    {
        public double Width { get; private set; }

        public TrenchExcavation(TriangleNet.Mesh surfaceMesh, Curve excavationBottom, double width, double excavationStepSize = 1) : base(surfaceMesh, excavationBottom, excavationStepSize)
        {
            Width = width;
        }

        public override void Excavate()
        {
            OutputSections = new List<ExcavationSection>();

            List<ExcavationSection> rightSide = ExcavateCurve(Side.Right, Width / 2.0);
            List<ExcavationSection> leftSide = ExcavateCurve(Side.Left, Width / 2.0);
            leftSide.Reverse();

            OutputSections.AddRange(rightSide);
            OutputSections.AddRange(leftSide);
        }
    }

    public class ExcavationSection
    {
        public List<ExcavationSectionElement> Elements { get; private set; }

        public ExcavationSection()
        {
            Elements = new List<ExcavationSectionElement>();
        }

        public void AddSlope(double h, double v, double heightLimit = -1)
        {
            ExcavationSectionElement ele = new ExcavationSectionElement(h, v);
            ele.HeightLimit = heightLimit;
            Elements.Add(ele);
        }

        public void AddOffset(double lengthLimit)
        {
            ExcavationSectionElement ele = new ExcavationSectionElement(1, 0);
            ele.LengthLimit = lengthLimit;
            Elements.Add(ele);
        }

        public static ExcavationSection Interpolate(ExcavationSection startSection, ExcavationSection endSection, double ratio)
        {
            ExcavationSection result = new ExcavationSection();

            for (int i = 0; i < Math.Max(startSection.Elements.Count, endSection.Elements.Count); i++)
            {
                int startIndex = Math.Min(i, startSection.Elements.Count - 1);
                int endIndex = Math.Min(i, endSection.Elements.Count - 1);

                double hStart = startSection.Elements[startIndex].H;
                double vStart = startSection.Elements[startIndex].V;
                double hlStart = startSection.Elements[startIndex].HeightLimit;
                double llStart = startSection.Elements[startIndex].LengthLimit;

                double hEnd = startSection.Elements[endIndex].H;
                double vEnd = startSection.Elements[endIndex].V;
                double hlEnd = startSection.Elements[endIndex].HeightLimit;
                double llEnd = startSection.Elements[endIndex].LengthLimit;

                double h = hStart + ratio * (hEnd - hStart);
                double v = vStart + ratio * (vEnd - vStart);

                double hl = ((hlStart > 0 && hlEnd > 0) ? hlStart + ratio * (hlEnd - hlStart) : Math.Max(hlStart, hlEnd));
                double ll = ((llStart > 0 && llEnd > 0) ? llStart + ratio * (llEnd - llStart) : Math.Max(llStart, llEnd));

                ExcavationSectionElement ele = new ExcavationSectionElement(h, v);
                ele.HeightLimit = hl;
                ele.LengthLimit = ll;
                result.Elements.Add(ele);
            }

            return result;
        }
    }

    public class ExcavationSectionElement : ICloneable
    {
        public double H { get; private set; }
        public double V { get; private set; }

        public double HeightLimit { get; set; }
        public double LengthLimit { get; set; }

        public Point3d BottomPoint { get; set; }
        public Point3d TopPoint { get; set; }
        public Vector3d Slope { get; set; }

        public double CurrentRatio { get; set; }
        public bool HasIntersection { get; set; }
        public bool HasTopPoint { get; set; }

        public ExcavationSectionElement(double h, double v)
        {
            H = h;
            V = v;
            HeightLimit = -1;
            LengthLimit = -1;

            CurrentRatio = double.MaxValue;
            HasIntersection = false;
            HasTopPoint = false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
