using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using AcadUtility;

namespace XCOM.Commands.RoadDesign
{
    public abstract class Excavation
    {
        private static double LengthTolerance = 0.0001;

        public Curve ExcavationBottom { get; protected set; }

        protected List<double> distances;
        protected List<ExcavationSection> sections;
        protected double curveLength;

        public Excavation(Curve excavationBottom)
        {
            ExcavationBottom = excavationBottom;

            curveLength = excavationBottom.GetLength();
            distances = new List<double>();
            sections = new List<ExcavationSection>();
        }

        public abstract void Excavate(TriangleNet.Mesh surfaceMesh);

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

        public ExcavationSection GetSection(double distance)
        {
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
    }

    public class PadExcavation : Excavation
    {
        public PadExcavation(Curve excavationBottom) : base(excavationBottom)
        {
            ;
        }

        public override void Excavate(TriangleNet.Mesh surfaceMesh)
        {

        }
    }

    public class ExcavationSection
    {
        public List<ExcavationSectionElement> Elements { get; private set; }
        public Point3d StartPoint { get; set; }
        public Point3d EndPoint { get; set; }

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

    public class ExcavationSectionElement
    {
        public double H { get; private set; }
        public double V { get; private set; }

        public double HeightLimit { get; set; }
        public double LengthLimit { get; set; }

        public ExcavationSectionElement(double h, double v)
        {
            H = h;
            V = v;
            HeightLimit = -1;
            LengthLimit = -1;
        }
    }
}
