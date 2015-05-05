using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;
using System.Windows.Forms;

namespace RoadDesign
{
    public class SurfaceCommands
    {
        TriangleNet.Mesh mesh;
        List<Autodesk.AutoCAD.GraphicsInterface.Drawable> temporaryGraphics;

        private bool selectPoints = true;
        private bool selectLines = false;
        private bool selectPolylines = false;
        private bool selectTexts = false;
        private bool selectTextsWithZ = false;
        private bool selectBlocks = false;
        private bool select3DFace = false;
        private bool selectSolid = false;
        private bool eraseEntities = false;

        public SurfaceCommands()
        {
            mesh = new TriangleNet.Mesh();
            temporaryGraphics = new List<Autodesk.AutoCAD.GraphicsInterface.Drawable>();
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.CommandWillStart += MdiActiveDocument_CommandWillStart;
        }

        void MdiActiveDocument_CommandWillStart(object sender, Autodesk.AutoCAD.ApplicationServices.CommandEventArgs e)
        {
            if (string.Compare(e.GlobalCommandName, "REGEN") == 0)
            {
                EraseTemporaryGraphics();
            }
        }

        private bool ShowSettings()
        {
            using (CreateSurfaceForm form = new CreateSurfaceForm())
            {
                form.SelectPoints = selectPoints;
                form.SelectLines = selectLines;
                form.SelectPolylines = selectPolylines;
                form.SelectTexts = selectTexts;
                form.SelectTextsWithZ = selectTextsWithZ;
                form.SelectBlocks = selectBlocks;
                form.Select3DFace = select3DFace;
                form.SelectSolid = selectSolid;
                form.EraseEntities = eraseEntities;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    selectPoints = form.SelectPoints;
                    selectLines = form.SelectLines;
                    selectTexts = form.SelectPolylines;
                    selectTexts = form.SelectTexts;
                    selectTextsWithZ = form.SelectTextsWithZ;
                    selectBlocks = form.SelectBlocks;
                    select3DFace = form.Select3DFace;
                    selectSolid = form.SelectSolid;
                    eraseEntities = form.EraseEntities;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        IEnumerable<ObjectId> SelectEntitites()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            List<TypedValue> tvs = new List<TypedValue>();
            tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            if (selectPoints) tvs.Add(new TypedValue((int)DxfCode.Start, "POINT"));
            if (selectLines) tvs.Add(new TypedValue((int)DxfCode.Start, "LINE"));
            if (selectPolylines) tvs.Add(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"));
            if (selectPolylines) tvs.Add(new TypedValue((int)DxfCode.Start, "POLYLINE"));
            if (selectTexts || selectTextsWithZ) tvs.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            if (selectBlocks) tvs.Add(new TypedValue((int)DxfCode.Start, "INSERT"));
            if (select3DFace) tvs.Add(new TypedValue((int)DxfCode.Start, "3DFACE"));
            if (selectSolid) tvs.Add(new TypedValue((int)DxfCode.Start, "SOLID"));
            tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            if (tvs.Count == 2) return new ObjectId[0];

            SelectionFilter ssf = new SelectionFilter(tvs.ToArray());
            PromptSelectionResult res = doc.Editor.GetSelection(ssf);
            if (res.Status != PromptStatus.OK) return new ObjectId[0];

            return res.Value.GetObjectIds();
        }

        IEnumerable<Point3d> ReadPoints(IEnumerable<ObjectId> items)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            HashSet<Point3d> points = new HashSet<Point3d>(new Point3dComparer(Tolerance.Global));

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in items)
                    {
                        // Point
                        if (selectPoints && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBPoint)).UnmanagedObject)
                        {
                            DBPoint item = (DBPoint)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.Position);
                        }
                        // Line
                        else if (selectLines && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Line)).UnmanagedObject)
                        {
                            Line item = (Line)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.StartPoint);
                            points.Add(item.EndPoint);
                        }
                        // LW Polyline
                        else if (selectPolylines && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline)).UnmanagedObject)
                        {
                            Polyline item = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                            for (int i = 0; i < item.NumberOfVertices; i++)
                            {
                                points.Add(item.GetPoint3dAt(i));
                            }
                        }
                        // 2D Polyline
                        else if (selectPolylines && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline2d)).UnmanagedObject)
                        {
                            Polyline2d item = (Polyline2d)tr.GetObject(id, OpenMode.ForRead);
                            foreach (ObjectId vId in item)
                            {
                                Vertex2d vertex = (Vertex2d)tr.GetObject(vId, OpenMode.ForRead);
                                points.Add(vertex.Position);
                            }
                        }
                        // 3D Polyline
                        else if (selectPolylines && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline3d)).UnmanagedObject)
                        {
                            Polyline3d item = (Polyline3d)tr.GetObject(id, OpenMode.ForRead);
                            foreach (ObjectId vId in item)
                            {
                                PolylineVertex3d vertex = (PolylineVertex3d)tr.GetObject(vId, OpenMode.ForRead);
                                points.Add(vertex.Position);
                            }
                        }
                        // Text
                        else if (selectTexts && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBText)).UnmanagedObject)
                        {
                            DBText item = (DBText)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.Position);
                        }
                        // Text with Z
                        else if (selectTextsWithZ && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBText)).UnmanagedObject)
                        {
                            DBText item = (DBText)tr.GetObject(id, OpenMode.ForRead);
                            double z = 0;
                            if (double.TryParse(item.TextString, out z))
                            {
                                Point3d pt = item.Position;
                                points.Add(new Point3d(pt.X, pt.Y, z));
                            }
                        }
                        // Blocks
                        else if (selectBlocks && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(BlockReference)).UnmanagedObject)
                        {
                            BlockReference item = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.Position);
                        }
                        // 3DFace
                        else if (select3DFace && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Face)).UnmanagedObject)
                        {
                            Face item = (Face)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.GetVertexAt(0));
                            points.Add(item.GetVertexAt(1));
                            points.Add(item.GetVertexAt(2));
                            points.Add(item.GetVertexAt(3));
                        }
                        // Solid (2D)
                        else if (selectSolid && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Solid)).UnmanagedObject)
                        {
                            Solid item = (Solid)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.GetPointAt(0));
                            points.Add(item.GetPointAt(1));
                            points.Add(item.GetPointAt(3));
                            points.Add(item.GetPointAt(2));
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                tr.Commit();
            }

            return points;
        }

        private class Point3dComparer : IEqualityComparer<Point3d>
        {
            private Tolerance tolerance;

            public Point3dComparer(Tolerance tol)
            {
                tolerance = tol;
            }

            public bool Equals(Point3d a, Point3d b)
            {
                return a.IsEqualTo(b, tolerance);
            }

            public int GetHashCode(Point3d point)
            {
                return (int)(point.X / tolerance.EqualPoint) ^
                       (int)(point.Y / tolerance.EqualPoint) ^
                       (int)(point.Z / tolerance.EqualPoint);
            }
        }

        private void EraseEntities(IEnumerable<ObjectId> items)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in items)
                    {
                        DBObject item = (DBObject)tr.GetObject(id, OpenMode.ForWrite);
                        item.Erase();
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("YUZEYDEMO")]
        public void CreateDemoSurface()
        {
            try
            {
                // Read demo surface from resource
                List<Tuple<double, double, double>> points = new List<Tuple<double, double, double>>();
                using (System.IO.StringReader reader = new System.IO.StringReader(Properties.Resources.DemoSurface))
                {
                    string line = reader.ReadLine(); // Skip header
                    while (line != null)
                    {
                        line = reader.ReadLine();
                        if (line != null)
                        {
                            string[] parts = line.Split('\t');
                            double x = 0;
                            double y = 0;
                            double z = 0;
                            if (double.TryParse(parts[1], out x) && double.TryParse(parts[2], out y) && double.TryParse(parts[3], out z))
                            {
                                points.Add(new Tuple<double, double, double>(x, y, z));
                            }
                        }
                    }
                }

                // Create mesh
                mesh = new TriangleNet.Mesh();
                TriangleNet.Geometry.InputGeometry geometry = new TriangleNet.Geometry.InputGeometry(points.Count);
                foreach (Tuple<double, double, double> point in points)
                {
                    geometry.AddPoint(point.Item1, point.Item2, 0, point.Item3);
                }
                mesh.Triangulate(geometry);
                MessageBox.Show(mesh.Triangles.Count.ToString() + " adet üçgen oluşturuldu.", "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("YUZEYOLUSTUR")]
        public void CreateSurface()
        {
            if (ShowSettings())
            {
                try
                {
                    IEnumerable<ObjectId> items = SelectEntitites();
                    IEnumerable<Point3d> points = ReadPoints(items);
                    int count = points.Count();

                    if (count > 0)
                    {
                        mesh = new TriangleNet.Mesh();
                        TriangleNet.Geometry.InputGeometry geometry = new TriangleNet.Geometry.InputGeometry(count);
                        foreach (Point3d pt in points)
                        {
                            geometry.AddPoint(pt.X, pt.Y, 0, pt.Z);
                        }
                        mesh.Triangulate(geometry);
                        MessageBox.Show(mesh.Triangles.Count.ToString() + " adet üçgen oluşturuldu.", "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    if (eraseEntities)
                    {
                        EraseEntities(items);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("NOKTABULUTU")]
        public void CreatePointCloud()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            if (ShowSettings())
            {
                IEnumerable<ObjectId> items = SelectEntitites();
                IEnumerable<Point3d> points = ReadPoints(items);

                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    try
                    {
                        foreach (Point3d v in points)
                        {
                            DBPoint point = new DBPoint(new Point3d(v.X, v.Y, v.Z));

                            btr.AppendEntity(point);
                            tr.AddNewlyCreatedDBObject(point, true);
                        }

                        if (eraseEntities)
                        {
                            EraseEntities(items);
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

        [Autodesk.AutoCAD.Runtime.CommandMethod("YUZEYCIZ")]
        public void DrawSurface()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            // Object type
            PromptKeywordOptions opts = new PromptKeywordOptions("\nYüzey türü [Geçici/3dFace/Point/polyfaceMesh] <Geçici>: ", "Temporary 3dFace Point polyfaceMesh");
            opts.AllowNone = true;
            PromptResult res = doc.Editor.GetKeywords(opts);
            string outputType = res.StringResult;
            if (res.Status == PromptStatus.None)
            {
                outputType = "Temporary";
            }
            else if (res.Status != PromptStatus.OK)
            {
                return;
            }

            // Color option
            opts = new PromptKeywordOptions("\nRenkli çizim [E/H] <E>: ", "Yes No");
            opts.AllowNone = true;
            res = doc.Editor.GetKeywords(opts);
            bool useColor = true;
            if (res.Status == PromptStatus.None)
            {
                useColor = true;
            }
            else if (res.Status != PromptStatus.OK)
            {
                return;
            }
            else
            {
                useColor = (string.Compare(res.StringResult, "Yes", StringComparison.OrdinalIgnoreCase) == 0);
            }
            double zmin = double.MaxValue; double zmax = double.MinValue;
            foreach (TriangleNet.Data.Vertex v in mesh.Vertices)
            {
                zmin = Math.Min(zmin, v.Attributes[0]);
                zmax = Math.Max(zmax, v.Attributes[0]);
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                try
                {
                    if (string.Compare(outputType, "Temporary", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        EraseTemporaryGraphics();
                        foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
                        {
                            TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                            TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                            TriangleNet.Data.Vertex v3 = tri.GetVertex(2);
                            Face face = new Face(new Point3d(v1.X, v1.Y, v1.Attributes[0]), new Point3d(v2.X, v2.Y, v2.Attributes[0]), new Point3d(v3.X, v3.Y, v3.Attributes[0]), true, true, true, true);
                            if (useColor)
                            {
                                face.Color = ColorFromZ((v1.Attributes[0] + v2.Attributes[0] + v3.Attributes[0]) / 3, zmin, zmax);
                            }
                            temporaryGraphics.Add(face);
                        }
                        DrawTemporaryGraphics();
                    }
                    else if (string.Compare(outputType, "3dFace", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
                        {
                            TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                            TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                            TriangleNet.Data.Vertex v3 = tri.GetVertex(2);
                            Face face = new Face(new Point3d(v1.X, v1.Y, v1.Attributes[0]), new Point3d(v2.X, v2.Y, v2.Attributes[0]), new Point3d(v3.X, v3.Y, v3.Attributes[0]), true, true, true, true);
                            if (useColor)
                            {
                                face.Color = ColorFromZ((v1.Attributes[0] + v2.Attributes[0] + v3.Attributes[0]) / 3, zmin, zmax);
                            }

                            btr.AppendEntity(face);
                            tr.AddNewlyCreatedDBObject(face, true);
                        }
                    }
                    else if (string.Compare(outputType, "Point", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        foreach (TriangleNet.Data.Vertex v in mesh.Vertices)
                        {
                            DBPoint point = new DBPoint(new Point3d(v.X, v.Y, v.Attributes[0]));
                            if (useColor)
                            {
                                point.Color = ColorFromZ(v.Attributes[0], zmin, zmax);
                            }

                            btr.AppendEntity(point);
                            tr.AddNewlyCreatedDBObject(point, true);
                        }
                    }
                    else if (string.Compare(outputType, "polyfaceMesh", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        PolyFaceMesh pfm = new PolyFaceMesh();
                        btr.AppendEntity(pfm);
                        tr.AddNewlyCreatedDBObject(pfm, true);

                        // Vertices
                        SortedDictionary<int, Point3d> vertices = new SortedDictionary<int, Point3d>();
                        foreach (TriangleNet.Data.Vertex v in mesh.Vertices)
                        {
                            vertices.Add(v.ID, new Point3d(v.X, v.Y, v.Attributes[0]));
                        }
                        foreach (KeyValuePair<int, Point3d> v in vertices)
                        {
                            PolyFaceMeshVertex point = new PolyFaceMeshVertex(v.Value);
                            pfm.AppendVertex(point);
                            tr.AddNewlyCreatedDBObject(point, true);
                        }

                        // Faces
                        foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
                        {
                            FaceRecord face = new FaceRecord((short)(tri.P0 + 1), (short)(tri.P1 + 1), (short)(tri.P2 + 1), 0);
                            if (useColor)
                            {
                                face.Color = ColorFromZ((tri.GetVertex(0).Attributes[0] + tri.GetVertex(1).Attributes[0] + tri.GetVertex(2).Attributes[0]) / 3, zmin, zmax);
                            }
                            pfm.AppendFaceRecord(face);
                            tr.AddNewlyCreatedDBObject(face, true);
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

        private void DrawTemporaryGraphics()
        {
            Autodesk.AutoCAD.GraphicsInterface.TransientManager man = Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager;
            foreach (Autodesk.AutoCAD.GraphicsInterface.Drawable item in temporaryGraphics)
            {
                man.AddTransient(item, Autodesk.AutoCAD.GraphicsInterface.TransientDrawingMode.DirectShortTerm, 128, new IntegerCollection());
            }
        }

        private void EraseTemporaryGraphics()
        {
            Autodesk.AutoCAD.GraphicsInterface.TransientManager man = Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager;
            foreach (Autodesk.AutoCAD.GraphicsInterface.Drawable item in temporaryGraphics)
            {
                man.EraseTransient(item, new IntegerCollection());
                item.Dispose();
            }
            temporaryGraphics.Clear();
        }

        private Color ColorFromZ(double z, double zmin, double zmax)
        {
            Color maxColor = Color.FromRgb(224, 243, 219);
            Color midColor = Color.FromRgb(168, 221, 181);
            Color minColor = Color.FromRgb(67, 162, 202);

            double ratio = (z - zmin) / (zmax - zmin);

            if (zmax == zmin)
            {
                return minColor;
            }
            else if (ratio > 0.5)
            {
                ratio = (ratio - 0.5) * 2;
                byte r = (byte)(ratio * (double)(maxColor.Red - midColor.Red) + (double)midColor.Red);
                byte g = (byte)(ratio * (double)(maxColor.Green - midColor.Green) + (double)midColor.Green);
                byte b = (byte)(ratio * (double)(maxColor.Blue - midColor.Blue) + (double)midColor.Blue);
                return Color.FromRgb(r, g, b);
            }
            else
            {
                ratio = ratio * 2;
                byte r = (byte)(ratio * (double)(midColor.Red - minColor.Red) + (double)minColor.Red);
                byte g = (byte)(ratio * (double)(midColor.Green - minColor.Green) + (double)minColor.Green);
                byte b = (byte)(ratio * (double)(midColor.Blue - minColor.Blue) + (double)minColor.Blue);
                return Color.FromRgb(r, g, b);
            }
        }
    }
}
