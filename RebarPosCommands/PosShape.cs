using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Drawing;
using Autodesk.AutoCAD.GraphicsSystem;
using Autodesk.AutoCAD.GraphicsInterface;

namespace RebarPosCommands
{
    public class PosShape
    {
        public string Name { get; set; }
        public int Fields { get; set; }
        public string Formula { get; set; }
        public string FormulaBending { get; set; }
        public int Priority { get; set; }

        public bool IsBuiltIn { get; private set; }
        public bool IsUnknown { get; private set; }

        public List<Shape> Items { get; private set; }

        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string E { get; set; }
        public string F { get; set; }

        public PosShape()
        {
            Name = "";
            Fields = 1;
            Formula = "";
            FormulaBending = "";
            Priority = 0;

            IsBuiltIn = false;
            IsUnknown = false;

            Items = new List<Shape>();
        }

        public void SetShapeTexts(string a, string b, string c, string d, string e, string f)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;
        }

        public void ClearShapeTexts()
        {
            A = "";
            B = "";
            C = "";
            D = "";
            E = "";
            F = "";
        }

        public Extents3d Bounds
        {
            get
            {
                Extents3d ext = new Extents3d();
                foreach (Shape obj in Items)
                {
                    if (obj is ShapeLine)
                    {
                        ShapeLine line = (ShapeLine)obj;
                        ext.AddPoint(new Point3d(line.X1, line.Y1, 0));
                        ext.AddPoint(new Point3d(line.X2, line.Y2, 0));
                    }
                    else if (obj is ShapeArc)
                    {
                        ShapeArc arc = (ShapeArc)obj;

                        double da = (arc.EndAngle - arc.StartAngle) / 10.0;
                        int i = 0;
                        double a = arc.StartAngle;
                        for (i = 0; i < 10; i++)
                        {
                            double x = arc.X + Math.Cos(a) * arc.R;
                            double y = arc.Y + Math.Sin(a) * arc.R;
                            ext.AddPoint(new Point3d(x, y, 0));
                            a += da;
                        }
                    }
                    else if (obj is ShapeCircle)
                    {
                        ShapeCircle circle = (ShapeCircle)obj;
                        ext.AddPoint(new Point3d(circle.X - circle.R, circle.Y - circle.R, 0));
                        ext.AddPoint(new Point3d(circle.X - circle.R, circle.Y + circle.R, 0));
                        ext.AddPoint(new Point3d(circle.X + circle.R, circle.Y + circle.R, 0));
                        ext.AddPoint(new Point3d(circle.X + circle.R, circle.Y - circle.R, 0));
                    }
                    else if (obj is ShapeText)
                    {
                        ShapeText text = (ShapeText)obj;

                        string txt = text.Text;
                        if (!string.IsNullOrEmpty(A)) txt = txt.Replace("A", A);
                        if (!string.IsNullOrEmpty(B)) txt = txt.Replace("B", B);
                        if (!string.IsNullOrEmpty(C)) txt = txt.Replace("C", C);
                        if (!string.IsNullOrEmpty(D)) txt = txt.Replace("D", D);
                        if (!string.IsNullOrEmpty(E)) txt = txt.Replace("E", E);
                        if (!string.IsNullOrEmpty(F)) txt = txt.Replace("F", F);

                        Autodesk.AutoCAD.GraphicsInterface.TextStyle style = new Autodesk.AutoCAD.GraphicsInterface.TextStyle(text.Font, "", text.Height, text.Width, 0, 0, false, false, false, false, false, false, "");
                        Extents2d extents = style.ExtentsBox(txt, false, true, null);
                        double width = extents.MaxPoint.X - extents.MinPoint.X;
                        double height = extents.MaxPoint.Y - extents.MinPoint.Y;

                        double xoff = 0.0;
                        if (text.HorizontalAlignment == TextHorizontalMode.TextLeft)
                            xoff = 0.0;
                        else if (text.HorizontalAlignment == TextHorizontalMode.TextRight)
                            xoff = -width;
                        else // horizontal center
                            xoff = -width / 2.0;

                        double yoff = 0.0;
                        if (text.VerticalAlignment == TextVerticalMode.TextTop)
                            yoff = -height;
                        else if (text.VerticalAlignment == TextVerticalMode.TextBase || text.VerticalAlignment == TextVerticalMode.TextBottom)
                            yoff = 0.0;
                        else // vertical middle
                            yoff = -height / 2.0;

                        ext.AddPoint(new Point3d(text.X + xoff, text.Y + yoff, 0));
                        ext.AddPoint(new Point3d(text.X + xoff + width, text.Y + yoff + height, 0));
                    }
                }
                return ext;
            }
        }

        public IEnumerable<Entity> ToDrawable(Point3d basePoint, double scale, double rotation, bool showInvisible, ObjectId lineLayerId, ObjectId textLayerId)
        {
            List<Entity> res = new List<Entity>();
            ObjectId hiddenLayerId = DWGUtility.CreateEntity.GetOrCreateDefpointsLayer();

            foreach (Shape obj in Items)
            {
                if (!showInvisible && !obj.Visible) continue;

                Entity en = null;

                if (obj is ShapeLine)
                {
                    ShapeLine line = (ShapeLine)obj;
                    en = new Line(new Point3d(line.X1, line.Y1, 0), new Point3d(line.X2, line.Y2, 0));

                    if (!lineLayerId.IsNull) 
                        en.LayerId = lineLayerId;
                    else
                        en.Color = obj.Color;
                }
                else if (obj is ShapeArc)
                {
                    ShapeArc arc = (ShapeArc)obj;
                    en = new Arc(new Point3d(arc.X, arc.Y, 0), Vector3d.ZAxis, arc.R, arc.StartAngle, arc.EndAngle);
                    if (!lineLayerId.IsNull)
                        en.LayerId = lineLayerId;
                    else
                        en.Color = obj.Color;
                }
                else if (obj is ShapeCircle)
                {
                    ShapeCircle circle = (ShapeCircle)obj;
                    en = new Circle(new Point3d(circle.X, circle.Y, 0), Vector3d.ZAxis, circle.R);
                    if (!lineLayerId.IsNull)
                        en.LayerId = lineLayerId;
                    else
                        en.Color = obj.Color;
                }
                else if (obj is ShapeText)
                {
                    ShapeText text = (ShapeText)obj;

                    string txt = text.Text;
                    if (!string.IsNullOrEmpty(A)) txt = txt.Replace("A", A);
                    if (!string.IsNullOrEmpty(B)) txt = txt.Replace("B", B);
                    if (!string.IsNullOrEmpty(C)) txt = txt.Replace("C", C);
                    if (!string.IsNullOrEmpty(D)) txt = txt.Replace("D", D);
                    if (!string.IsNullOrEmpty(E)) txt = txt.Replace("E", E);
                    if (!string.IsNullOrEmpty(F)) txt = txt.Replace("F", F);

                    DBText dtext = new DBText();
                    dtext.TextString = txt;
                    dtext.Position = new Point3d(text.X, text.Y, 0);
                    dtext.TextStyleId = DWGUtility.CreateEntity.CreateTextStyle("PosShapeTextStyle_" + Name, text.Font, text.Width);
                    dtext.Height = text.Height;
                    dtext.WidthFactor = text.Width;
                    dtext.HorizontalMode = text.HorizontalAlignment;
                    if (text.VerticalAlignment == TextVerticalMode.TextBottom)
                        dtext.VerticalMode = TextVerticalMode.TextBase;
                    else
                        dtext.VerticalMode = text.VerticalAlignment;

                    if (dtext.HorizontalMode != TextHorizontalMode.TextLeft || dtext.VerticalMode != TextVerticalMode.TextBase)
                    {
                        dtext.AlignmentPoint = new Point3d(text.X, text.Y, 0);
                    }

                    en = dtext;
                    if (!textLayerId.IsNull)
                        en.LayerId = textLayerId;
                    else
                        en.Color = obj.Color;
                }

                if (en != null)
                {
                    if (!obj.Visible) en.LayerId = hiddenLayerId;
                    res.Add(en);
                }
            }

            Extents3d bounds = Bounds;
            Point3d p1 = bounds.MinPoint;
            Point3d p2 = bounds.MaxPoint;
            if (Math.Abs(p2.Y - p1.Y) > double.Epsilon)
            {
                scale = scale / (p2.Y - p1.Y);
            }

            Matrix3d trans = Matrix3d.Identity;
            trans = trans.PreMultiplyBy(Matrix3d.Displacement(basePoint - p1));
            trans = trans.PreMultiplyBy(Matrix3d.Scaling(scale, basePoint));
            trans = trans.PreMultiplyBy(Matrix3d.Rotation(rotation, Vector3d.ZAxis, basePoint));

            foreach (Entity en in res)
            {
                en.TransformBy(trans);
            }

            return res;
        }

        public IEnumerable<Entity> ToDrawable(Point3d basePoint, double scale, double rotation, bool showInvisible)
        {
            return ToDrawable(basePoint, scale, rotation, showInvisible, ObjectId.Null, ObjectId.Null);
        }

        public IEnumerable<Entity> ToDrawable(Point3d basePoint, double scale, double rotation)
        {
            return ToDrawable(basePoint, scale, rotation, false, ObjectId.Null, ObjectId.Null);
        }

        public IEnumerable<Entity> ToDrawable(Point3d basePoint, double scale)
        {
            return ToDrawable(basePoint, scale, 0, false, ObjectId.Null, ObjectId.Null);
        }

        public IEnumerable<Entity> ToDrawable(Point3d basePoint)
        {
            return ToDrawable(basePoint, 1.0, 0, false, ObjectId.Null, ObjectId.Null);
        }

        #region Static Methods
        public static PosShape UnknownPosShape
        {
            get
            {
                PosShape shape = new PosShape();

                shape.Name = "HATA";
                shape.Fields = 1;
                shape.Formula = "A";
                shape.FormulaBending = "A";
                shape.IsBuiltIn = true;
                shape.IsUnknown = true;

                return shape;
            }
        }

        private static SortedDictionary<string, PosShape> shapes = null;
        public static SortedDictionary<string, PosShape> Shapes
        {
            get
            {
                if (shapes == null)
                {
                    shapes = new SortedDictionary<string, PosShape>(new PosNameComparer());
                    ClearAllPosShapes();
                    ReadPosShapesFromResource();
                    ReadUserPosShapes();
                }
                return shapes;
            }
        }

        protected static void ClearAllPosShapes()
        {
            Shapes.Clear();
        }

        public static void ClearUserPosShapes()
        {
            List<string> toRemove = new List<string>();
            foreach (KeyValuePair<string, PosShape> item in Shapes)
            {
                if (!item.Value.IsBuiltIn) toRemove.Add(item.Key);
            }
            foreach (string key in toRemove)
            {
                Shapes.Remove(key);
            }
        }

        protected static void ReadPosShapesFromResource()
        {
            ReadPosShapesFromString(Properties.Resources.Shapes, true);
        }

        public static void ReadUserPosShapes()
        {
            string userFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RebarPos");
            string userShapesFile = System.IO.Path.Combine(userFolder, "ShapeList.txt");

            try
            {
                if (System.IO.File.Exists(userShapesFile))
                {
                    ClearUserPosShapes();

                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(userShapesFile, Encoding.UTF8))
                    {
                        ReadPosShapesFromString(streamReader.ReadToEnd(), false);
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString(), "RebarPos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                ClearUserPosShapes();
            }
        }

        protected static void ReadPosShapesFromString(string source, bool builtIn)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(source)))
            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    while (line != null && !line.StartsWith("BEGIN"))
                        line = reader.ReadLine();

                    if (line == null) break;

                    line = reader.ReadLine();
                    string name = line.Split('\t')[1];
                    line = reader.ReadLine();
                    int fields = int.Parse(line.Split('\t')[1]);
                    line = reader.ReadLine();
                    string formula = line.Split('\t')[1];
                    line = reader.ReadLine();
                    string formulabending = line.Split('\t')[1];
                    line = reader.ReadLine();
                    int priority = int.Parse(line.Split('\t')[1]);
                    line = reader.ReadLine();
                    int count = int.Parse(line.Split('\t')[1]);

                    PosShape shape = new PosShape();
                    shape.Name = name;
                    shape.Fields = fields;
                    shape.Formula = formula;
                    shape.FormulaBending = formulabending;
                    shape.IsBuiltIn = builtIn;

                    for (int i = 0; i < count; i++)
                    {
                        line = reader.ReadLine();
                        string[] parts = line.Split('\t');
                        string fieldname = parts[0];

                        if (fieldname.StartsWith("LINE"))
                        {
                            double x1 = double.Parse(parts[1]);
                            double y1 = double.Parse(parts[2]);
                            double x2 = double.Parse(parts[3]);
                            double y2 = double.Parse(parts[4]);
                            short color = short.Parse(parts[5]);
                            bool visible = (string.Compare(parts[6], "V", StringComparison.InvariantCultureIgnoreCase) == 0);

                            ShapeLine sline = new ShapeLine(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, color), x1, y1, x2, y2, visible);
                            shape.Items.Add(sline);
                        }
                        else if (fieldname.StartsWith("ARC"))
                        {
                            double x = double.Parse(parts[1]);
                            double y = double.Parse(parts[2]);
                            double r = double.Parse(parts[3]);
                            double a1 = double.Parse(parts[4]);
                            double a2 = double.Parse(parts[5]);
                            short color = short.Parse(parts[6]);
                            bool visible = (string.Compare(parts[7], "V", StringComparison.InvariantCultureIgnoreCase) == 0);

                            ShapeArc sarc = new ShapeArc(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, color), x, y, r, a1, a2, visible);
                            shape.Items.Add(sarc);
                        }
                        else if (fieldname.StartsWith("CIRCLE"))
                        {
                            double x = double.Parse(parts[1]);
                            double y = double.Parse(parts[2]);
                            double r = double.Parse(parts[3]);
                            short color = short.Parse(parts[4]);
                            bool visible = (string.Compare(parts[5], "V", StringComparison.InvariantCultureIgnoreCase) == 0);

                            ShapeCircle scircle = new ShapeCircle(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, color), x, y, r, visible);
                            shape.Items.Add(scircle);
                        }
                        else if (fieldname.StartsWith("TEXT"))
                        {
                            double x = double.Parse(parts[1]);
                            double y = double.Parse(parts[2]);
                            double h = double.Parse(parts[3]);
                            double w = double.Parse(parts[4]);
                            string str = parts[5];
                            string font = parts[6];
                            int ha = int.Parse(parts[7]);
                            int va = int.Parse(parts[8]);
                            short color = short.Parse(parts[9]);
                            bool visible = (string.Compare(parts[10], "V", StringComparison.InvariantCultureIgnoreCase) == 0);
                            ShapeText stext = new ShapeText(Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, color), x, y, h, w, str, font, (TextHorizontalMode)ha, (TextVerticalMode)va, visible);
                            shape.Items.Add(stext);
                        }
                    }

                    if (!Shapes.ContainsKey(name))
                    {
                        Shapes.Add(name, shape);
                    }
                }
            }
        }

        public static void SaveUserPosShapes()
        {
            string userFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RebarPos");
            string userShapesFile = System.IO.Path.Combine(userFolder, "ShapeList.txt");

            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(userShapesFile, false, Encoding.UTF8))
                {
                    foreach (PosShape shape in Shapes.Values)
                    {
                        writer.WriteLine("BEGIN");
                        writer.WriteLine("Name\t" + shape.Name);
                        writer.WriteLine("Fields\t" + shape.Fields.ToString());
                        writer.WriteLine("Formula\t" + shape.Formula);
                        writer.WriteLine("FormulaBending\t" + shape.FormulaBending);
                        writer.WriteLine("Priority\t" + shape.Priority.ToString());
                        writer.WriteLine("Count\t" + shape.Items.Count.ToString());

                        foreach (Shape item in shape.Items)
                        {
                            if (item is ShapeLine)
                            {
                                ShapeLine obj = (ShapeLine)item;
                                writer.WriteLine("LINE\t" +
                                    obj.X1.ToString() + "\t" + obj.Y1.ToString() + "\t" + obj.X2.ToString() + "\t" + obj.Y2.ToString() + "\t" +
                                    obj.Color.ColorIndex.ToString() + "\t" + (obj.Visible ? "V" : "I"));
                            }
                            else if (item is ShapeArc)
                            {
                                ShapeArc obj = (ShapeArc)item;
                                writer.WriteLine("ARC\t" +
                                    obj.X.ToString() + "\t" + obj.Y.ToString() + "\t" + obj.R.ToString() + "\t" +
                                    obj.StartAngle.ToString() + "\t" + obj.EndAngle.ToString() + "\t" +
                                    obj.Color.ColorIndex.ToString() + "\t" + (obj.Visible ? "V" : "I"));
                            }
                            else if (item is ShapeCircle)
                            {
                                ShapeCircle obj = (ShapeCircle)item;
                                writer.WriteLine("CIRCLE\t" +
                                    obj.X.ToString() + "\t" + obj.Y.ToString() + "\t" + obj.R.ToString() + "\t" +
                                    obj.Color.ColorIndex.ToString() + "\t" + (obj.Visible ? "V" : "I"));
                            }
                            else if (item is ShapeText)
                            {
                                ShapeText obj = (ShapeText)item;
                                writer.WriteLine("TEXT\t" +
                                    obj.X.ToString() + "\t" + obj.Y.ToString() + "\t" + obj.Height.ToString() + "\t" + obj.Width.ToString() + "\t" +
                                    obj.Text + "\t" + obj.Font + "\t" +
                                    ((int)obj.HorizontalAlignment).ToString() + "\t" + ((int)obj.VerticalAlignment).ToString() + "\t" +
                                    obj.Color.ColorIndex.ToString() + "\t" + (obj.Visible ? "V" : "I"));
                            }
                        }

                        writer.WriteLine("END");
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString(), "RebarPos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Compare Pos Names
        private class PosNameComparer : IComparer<string>
        {
            public int Compare(string p1, string p2)
            {
                if (string.Compare(p1, p2, StringComparison.InvariantCultureIgnoreCase) == 0)
                    return 0;
                else if (string.Compare(p1, "GENEL", StringComparison.InvariantCultureIgnoreCase) == 0)
                    return -1;
                else if (string.Compare(p2, "GENEL", StringComparison.InvariantCultureIgnoreCase) == 0)
                    return 1;
                else
                {
                    int n1 = 0;
                    int n2 = 0;

                    if (int.TryParse(p1, out n1) && int.TryParse(p2, out n2))
                    {
                        return n1.CompareTo(n2);
                    }
                    else
                    {
                        return string.Compare(p1, p2, StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }
        }
        #endregion

        #region Shape Primitives
        public abstract class Shape
        {
            public Autodesk.AutoCAD.Colors.Color Color { get; set; }
            public bool Visible { get; set; }

            public abstract Shape Clone();

            protected Shape()
            {
                Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 0);
                Visible = true;
            }

            protected Shape(Autodesk.AutoCAD.Colors.Color color, bool visible)
            {
                Color = color;
                Visible = visible;
            }
        }

        public class ShapeLine : Shape
        {
            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double X2 { get; set; }
            public double Y2 { get; set; }

            public override Shape Clone()
            {
                return new ShapeLine(Color, X1, Y1, X2, Y2, Visible);
            }

            public ShapeLine()
                : base()
            {
                ;
            }

            public ShapeLine(Autodesk.AutoCAD.Colors.Color color, double x1, double y1, double x2, double y2, bool visible)
                : base(color, visible)
            {
                X1 = x1;
                Y1 = y1;
                X2 = x2;
                Y2 = y2;
            }
        }

        public class ShapeArc : Shape
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double R { get; set; }
            public double StartAngle { get; set; }
            public double EndAngle { get; set; }

            public override Shape Clone()
            {
                return new ShapeArc(Color, X, Y, R, StartAngle, EndAngle, Visible);
            }

            public ShapeArc()
                : base()
            {
                ;
            }

            public ShapeArc(Autodesk.AutoCAD.Colors.Color color, double x, double y, double r, double startAngle, double endAngle, bool visible)
                : base(color, visible)
            {
                X = x;
                Y = y;
                R = r;
                StartAngle = startAngle;
                EndAngle = endAngle;
            }
        }

        public class ShapeCircle : Shape
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double R { get; set; }

            public override Shape Clone()
            {
                return new ShapeCircle(Color, X, Y, R, Visible);
            }

            public ShapeCircle()
                : base()
            {
                ;
            }

            public ShapeCircle(Autodesk.AutoCAD.Colors.Color color, double x, double y, double r, bool visible)
                : base(color, visible)
            {
                X = x;
                Y = y;
                R = r;
            }
        }

        public class ShapeText : Shape
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Height { get; set; }
            public double Width { get; set; }
            public string Text { get; set; }
            public string Font { get; set; }
            public TextHorizontalMode HorizontalAlignment { get; set; }
            public TextVerticalMode VerticalAlignment { get; set; }

            public override Shape Clone()
            {
                return new ShapeText(Color, X, Y, Height, Width, Text, Font, HorizontalAlignment, VerticalAlignment, Visible);
            }

            public ShapeText()
                : base()
            {
                ;
            }

            public ShapeText(Autodesk.AutoCAD.Colors.Color color, double x, double y, double height, double width, string text, string font, TextHorizontalMode horizontalAlignment, TextVerticalMode verticalAlignment, bool visible)
                : base(color, visible)
            {
                X = x;
                Y = y;
                Height = height;
                Width = width;
                Text = text;
                Font = font;
                HorizontalAlignment = horizontalAlignment;
                VerticalAlignment = verticalAlignment;
            }
        }
        #endregion
    }
}
