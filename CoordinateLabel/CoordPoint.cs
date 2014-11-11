using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;

namespace CoordinateLabel
{
    public class CoordPoint
    {
        public int N { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public CoordPoint(int n, double x, double y, double z)
        {
            N = n;

            X = x;
            Y = y;
            Z = z;
        }

        public CoordPoint(int n, Point3d p)
            : this(n, p.X, p.Y, p.Z)
        {
            ;
        }

        public string ToString(string prefix, int nwidth, int coordprecision, int coordwidth)
        {
            string n = prefix + N.ToString();
            if (n.Length < nwidth) n = new string(' ', nwidth - n.Length) + n;

            string format = "0." + new string('0', coordprecision);
            if (coordprecision == 0) format = "0";

            string xtext = X.ToString(format);
            string ytext = Y.ToString(format);

            if (xtext.Length < coordwidth) xtext = new string(' ', coordwidth - xtext.Length) + xtext;
            if (ytext.Length < coordwidth) ytext = new string(' ', coordwidth - ytext.Length) + ytext;

            return n + xtext + ytext;
        }
    }
}
