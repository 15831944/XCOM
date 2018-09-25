using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XCOM.Commands.Utility
{
    public class Command_PRINTENTCOORDS
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("NESYAZ")]
        public void PrintEntityCoordinates()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;
            Matrix3d wcs2ucs = AcadUtility.AcadGraphics.WcsToUcs;

            using (PrintEntitiesForm form = new PrintEntitiesForm())
            {
                // Read settings
                form.SelectPoint = Properties.Settings.Default.Command_PRINTENTCOORDS_SelectPoint;
                form.SelectCircle = Properties.Settings.Default.Command_PRINTENTCOORDS_SelectCircle;
                form.SelectLine = Properties.Settings.Default.Command_PRINTENTCOORDS_SelectLine;
                form.SelectPolyline = Properties.Settings.Default.Command_PRINTENTCOORDS_SelectPolyline;
                form.Select3DFace = Properties.Settings.Default.Command_PRINTENTCOORDS_Select3DFace;
                form.SelectText = Properties.Settings.Default.Command_PRINTENTCOORDS_SelectText;
                form.SelectBlock = Properties.Settings.Default.Command_PRINTENTCOORDS_SelectBlock;

                form.UseUCS = Properties.Settings.Default.Command_PRINTENTCOORDS_UCS;

                form.LineFormat = Properties.Settings.Default.Command_PRINTENTCOORDS_LineFormat;
                form.Precision = Properties.Settings.Default.Command_PRINTENTCOORDS_Precision;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    // Save changes
                    Properties.Settings.Default.Command_PRINTENTCOORDS_SelectPoint = form.SelectPoint;
                    Properties.Settings.Default.Command_PRINTENTCOORDS_SelectCircle = form.SelectCircle;
                    Properties.Settings.Default.Command_PRINTENTCOORDS_SelectLine = form.SelectLine;
                    Properties.Settings.Default.Command_PRINTENTCOORDS_SelectPolyline = form.SelectPolyline;
                    Properties.Settings.Default.Command_PRINTENTCOORDS_Select3DFace = form.Select3DFace;
                    Properties.Settings.Default.Command_PRINTENTCOORDS_SelectText = form.SelectText;
                    Properties.Settings.Default.Command_PRINTENTCOORDS_SelectBlock = form.SelectBlock;

                    Properties.Settings.Default.Command_PRINTENTCOORDS_UCS = form.UseUCS;

                    Properties.Settings.Default.Command_PRINTENTCOORDS_LineFormat = form.LineFormat;
                    Properties.Settings.Default.Command_PRINTENTCOORDS_Precision = form.Precision;

                    Properties.Settings.Default.Save();

                    // Select objects
                    List<TypedValue> tvs = new List<TypedValue>();
                    tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
                    if (form.SelectPoint) tvs.Add(new TypedValue((int)DxfCode.Start, "POINT"));
                    if (form.SelectCircle) tvs.Add(new TypedValue((int)DxfCode.Start, "CIRCLE"));
                    if (form.SelectLine) tvs.Add(new TypedValue((int)DxfCode.Start, "LINE"));
                    if (form.SelectPolyline) tvs.Add(new TypedValue((int)DxfCode.Start, "POLYLINE"));
                    if (form.SelectPolyline) tvs.Add(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"));
                    if (form.Select3DFace) tvs.Add(new TypedValue((int)DxfCode.Start, "3DFACE"));
                    if (form.SelectText) tvs.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
                    if (form.SelectText) tvs.Add(new TypedValue((int)DxfCode.Start, "MTEXT"));
                    if (form.SelectBlock) tvs.Add(new TypedValue((int)DxfCode.Start, "INSERT"));
                    tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
                    SelectionFilter filter = new SelectionFilter(tvs.ToArray());

                    PromptSelectionResult selRes = doc.Editor.GetSelection(filter);
                    if (selRes.Status == PromptStatus.OK)
                    {
                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            try
                            {
                                var listPoints = new List<Point3d>();
                                var listCircles = new List<Point3d>();
                                var listTexts = new List<Point3d>();
                                var listBlocks = new List<Point3d>();
                                var listLines = new List<Tuple<Point3d, Point3d>>();
                                var listPolylines = new List<Point3dCollection>();
                                var list3DFaces = new List<Tuple<Point3d, Point3d, Point3d, Point3d>>();

                                Point3d GetPoint(Point3d pt) =>
                                    form.UseUCS ? pt : pt.TransformBy(wcs2ucs);

                                // Read objects
                                foreach (ObjectId id in selRes.Value.GetObjectIds())
                                {
                                    if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBPoint)).UnmanagedObject)
                                    {
                                        DBPoint obj = tr.GetObject(id, OpenMode.ForRead) as DBPoint;
                                        listPoints.Add(GetPoint(obj.Position));
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Circle)).UnmanagedObject)
                                    {
                                        Circle obj = tr.GetObject(id, OpenMode.ForRead) as Circle;
                                        listCircles.Add(GetPoint(obj.Center));
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBText)).UnmanagedObject)
                                    {
                                        DBText obj = tr.GetObject(id, OpenMode.ForRead) as DBText;
                                        listTexts.Add(GetPoint(obj.Position));
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(MText)).UnmanagedObject)
                                    {
                                        MText obj = tr.GetObject(id, OpenMode.ForRead) as MText;
                                        listTexts.Add(GetPoint(obj.Location));
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(BlockReference)).UnmanagedObject)
                                    {
                                        BlockReference obj = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                                        listBlocks.Add(GetPoint(obj.Position));
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Line)).UnmanagedObject)
                                    {
                                        Line obj = tr.GetObject(id, OpenMode.ForRead) as Line;
                                        listLines.Add(new Tuple<Point3d, Point3d>(GetPoint(obj.StartPoint), GetPoint(obj.EndPoint)));
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Face)).UnmanagedObject)
                                    {
                                        Face obj = tr.GetObject(id, OpenMode.ForRead) as Face;
                                        list3DFaces.Add(new Tuple<Point3d, Point3d, Point3d, Point3d>(
                                            GetPoint(obj.GetVertexAt(0)), GetPoint(obj.GetVertexAt(1)),
                                            GetPoint(obj.GetVertexAt(2)), GetPoint(obj.GetVertexAt(3))));
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline)).UnmanagedObject)
                                    {
                                        Polyline obj = tr.GetObject(id, OpenMode.ForRead) as Polyline;
                                        Point3dCollection points = new Point3dCollection();
                                        for (int i = 0; i < obj.NumberOfVertices; i++)
                                        {
                                            points.Add(GetPoint(obj.GetPoint3dAt(i)));
                                        }
                                        listPolylines.Add(points);
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline2d)).UnmanagedObject)
                                    {
                                        Polyline2d obj = tr.GetObject(id, OpenMode.ForRead) as Polyline2d;
                                        Point3dCollection points = new Point3dCollection();
                                        foreach (ObjectId vId in obj)
                                        {
                                            Vertex2d vertex = (Vertex2d)tr.GetObject(vId, OpenMode.ForRead);
                                            points.Add(GetPoint(vertex.Position));
                                        }
                                        listPolylines.Add(points);
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline3d)).UnmanagedObject)
                                    {
                                        Polyline3d obj = tr.GetObject(id, OpenMode.ForRead) as Polyline3d;
                                        Point3dCollection points = new Point3dCollection();
                                        foreach (ObjectId vId in obj)
                                        {
                                            PolylineVertex3d vertex = (PolylineVertex3d)tr.GetObject(vId, OpenMode.ForRead);
                                            points.Add(GetPoint(vertex.Position));
                                        }
                                        listPolylines.Add(points);
                                    }
                                }

                                // Write text
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(form.OutputFilename))
                                {
                                    string PrintCoord(double coord) =>
                                        coord.ToString(form.Precision == 0 ? "0" : "0." + new string('0', form.Precision));

                                    string ClearLine(string line) =>
                                        line.Replace("[X]", "").Replace("[Y]", "").Replace("[Z]", "")
                                            .Replace("[X1]", "").Replace("[Y1]", "").Replace("[Z1]", "")
                                            .Replace("[X2]", "").Replace("[Y2]", "").Replace("[Z2]", "")
                                            .Replace("[X3]", "").Replace("[Y3]", "").Replace("[Z3]", "")
                                            .Replace("[X4]", "").Replace("[Y4]", "").Replace("[Z4]", "");

                                    var headerStr = form.LineFormat.Replace("[", "").Replace("]", "");

                                    void PrintPointList(List<Point3d> points)
                                    {
                                        file.WriteLine(headerStr);
                                        int i = 1;
                                        foreach (var pt in points)
                                        {
                                            var str = form.LineFormat;
                                            str = str.Replace("[#]", i.ToString());
                                            str = str.Replace("[X]", PrintCoord(pt.X));
                                            str = str.Replace("[Y]", PrintCoord(pt.Y));
                                            str = str.Replace("[Z]", PrintCoord(pt.Z));
                                            str = str.Replace("[X1]", PrintCoord(pt.X));
                                            str = str.Replace("[Y1]", PrintCoord(pt.Y));
                                            str = str.Replace("[Z1]", PrintCoord(pt.Z));
                                            str = ClearLine(str);
                                            file.WriteLine(str);
                                            i++;
                                        }
                                    }

                                    // Point
                                    if (listPoints.Count > 0)
                                    {
                                        file.WriteLine("POINT({0})", listPoints.Count);
                                        PrintPointList(listPoints);
                                    }
                                    // Circle
                                    if (listCircles.Count > 0)
                                    {
                                        file.WriteLine("CIRCLE({0})", listCircles.Count);
                                        PrintPointList(listCircles);
                                    }
                                    // Line
                                    if (listLines.Count > 0)
                                    {
                                        file.WriteLine("LINE({0})", listLines.Count);
                                        file.WriteLine(headerStr);
                                        int i = 1;
                                        foreach (var points in listLines)
                                        {
                                            var pt1 = points.Item1;
                                            var pt2 = points.Item2;

                                            var str = form.LineFormat;
                                            str = str.Replace("[#]", i.ToString());
                                            str = str.Replace("[X1]", PrintCoord(pt1.X));
                                            str = str.Replace("[Y1]", PrintCoord(pt1.Y));
                                            str = str.Replace("[Z1]", PrintCoord(pt1.Z));
                                            str = str.Replace("[X2]", PrintCoord(pt2.X));
                                            str = str.Replace("[Y2]", PrintCoord(pt2.Y));
                                            str = str.Replace("[Z2]", PrintCoord(pt2.Z));
                                            str = ClearLine(str);
                                            file.WriteLine(str);
                                            i++;
                                        }
                                    }
                                    // Polyline
                                    if (listPolylines.Count > 0)
                                    {
                                        file.WriteLine("POLYLINE({0})", listPolylines.Count);
                                        int i = 1;
                                        foreach (var polyline in listPolylines)
                                        {
                                            file.WriteLine("POLYLINE-{0}", i);
                                            file.WriteLine(headerStr);

                                            for (int j = 0; j < polyline.Count; j++)
                                            {
                                                var str = form.LineFormat;
                                                str = str.Replace("[#]", (j + 1).ToString());
                                                str = str.Replace("[X]", PrintCoord(polyline[j].X));
                                                str = str.Replace("[Y]", PrintCoord(polyline[j].Y));
                                                str = str.Replace("[Z]", PrintCoord(polyline[j].Z));
                                                str = str.Replace("[X1]", PrintCoord(polyline[j].X));
                                                str = str.Replace("[Y1]", PrintCoord(polyline[j].Y));
                                                str = str.Replace("[Z1]", PrintCoord(polyline[j].Z));
                                            str = ClearLine(str);
                                                file.WriteLine(str);
                                            }

                                            i++;
                                        }
                                    }
                                    // 3DFace
                                    if (list3DFaces.Count > 0)
                                    {
                                        file.WriteLine("3DFACE({0})", list3DFaces.Count);
                                        file.WriteLine(headerStr);
                                        int i = 1;
                                        foreach (var points in list3DFaces)
                                        {
                                            var pt1 = points.Item1;
                                            var pt2 = points.Item2;
                                            var pt3 = points.Item3;
                                            var pt4 = points.Item4;

                                            var str = form.LineFormat;
                                            str = str.Replace("[#]", i.ToString());
                                            str = str.Replace("[X1]", PrintCoord(pt1.X));
                                            str = str.Replace("[Y1]", PrintCoord(pt1.Y));
                                            str = str.Replace("[Z1]", PrintCoord(pt1.Z));
                                            str = str.Replace("[X2]", PrintCoord(pt2.X));
                                            str = str.Replace("[Y2]", PrintCoord(pt2.Y));
                                            str = str.Replace("[Z2]", PrintCoord(pt2.Z));
                                            str = str.Replace("[X3]", PrintCoord(pt3.X));
                                            str = str.Replace("[Y3]", PrintCoord(pt3.Y));
                                            str = str.Replace("[Z3]", PrintCoord(pt3.Z));
                                            str = str.Replace("[X4]", PrintCoord(pt4.X));
                                            str = str.Replace("[Y4]", PrintCoord(pt4.Y));
                                            str = str.Replace("[Z4]", PrintCoord(pt4.Z));
                                            str = ClearLine(str);
                                            file.WriteLine(str);
                                            i++;
                                        }
                                    }
                                    // Text
                                    if (listTexts.Count > 0)
                                    {
                                        file.WriteLine("TEXT({0})", listTexts.Count);
                                        PrintPointList(listTexts);
                                    }
                                    // Block
                                    if (listBlocks.Count > 0)
                                    {
                                        file.WriteLine("BLOCK({0})", listBlocks.Count);
                                        PrintPointList(listBlocks);
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            tr.Commit();
                        }
                    }
                }
            }
        }
    }
}