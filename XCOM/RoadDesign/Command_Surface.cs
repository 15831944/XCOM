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
using AcadUtility;

namespace XCOM.Commands.RoadDesign
{
    public class Command_Surface
    {
        Topography topo;
        List<Autodesk.AutoCAD.GraphicsInterface.Drawable> temporaryGraphics;

        private bool SelectPoints { get; set; }
        private bool SelectLines { get; set; }
        private bool SelectPolylines { get; set; }
        private bool SelectTexts { get; set; }
        private bool SelectTextsWithZ { get; set; }
        private bool SelectBlocks { get; set; }
        private bool Select3DFace { get; set; }
        private bool SelectSolid { get; set; }
        private bool SelectPolyfaceMesh { get; set; }
        private bool EraseEntities { get; set; }

        private double ExcavationStepSize { get; set; }

        private double ExcavationH { get; set; }
        private double ExcavationV { get; set; }

        private double ProfileGridH { get; set; }
        private double ProfileGridV { get; set; }
        private double ProfileVScale { get; set; }
        private double TextHeight { get; set; }
        private int Precision { get; set; }

        public Command_Surface()
        {
            topo = new Topography();
            temporaryGraphics = new List<Autodesk.AutoCAD.GraphicsInterface.Drawable>();
            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.CommandWillStart += MdiActiveDocument_CommandWillStart;

            SelectPoints = true;
            SelectPoints = false;
            SelectLines = false;
            SelectPolylines = false;
            SelectTexts = false;
            SelectTextsWithZ = false;
            SelectBlocks = false;
            Select3DFace = false;
            SelectSolid = false;
            SelectPolyfaceMesh = false;
            EraseEntities = false;

            ExcavationStepSize = 1.0;

            ExcavationH = 1.0;
            ExcavationV = 1.0;

            ProfileGridH = 10;
            ProfileGridV = 5;
            ProfileVScale = 1;
            TextHeight = 1;
            Precision = 2;
        }

        void MdiActiveDocument_CommandWillStart(object sender, Autodesk.AutoCAD.ApplicationServices.CommandEventArgs e)
        {
            if (string.Compare(e.GlobalCommandName, "REGEN") == 0)
            {
                EraseTemporaryGraphics();
            }
        }

        private Topography.SurfaceType PickSurface()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            PromptKeywordOptions opts = new PromptKeywordOptions("\nYüzey türü [Orijinal/Tamamlanmış] <Orijinal>: ", "Original Proposed");
            opts.Keywords.Default = "Original";
            opts.AllowNone = true;
            PromptResult res = doc.Editor.GetKeywords(opts);

            string surfaceType = res.StringResult;
            if (res.Status == PromptStatus.None)
            {
                surfaceType = "Original";
            }
            else if (res.Status != PromptStatus.OK)
            {
                return Topography.SurfaceType.None;
            }
            return (surfaceType == "Original" ? Topography.SurfaceType.Original : Topography.SurfaceType.Proposed);
        }

        private bool ShowSettings()
        {
            using (CreateSurfaceForm form = new CreateSurfaceForm())
            {
                form.SelectPoints = SelectPoints;
                form.SelectLines = SelectLines;
                form.SelectPolylines = SelectPolylines;
                form.SelectTexts = SelectTexts;
                form.SelectTextsWithZ = SelectTextsWithZ;
                form.SelectBlocks = SelectBlocks;
                form.Select3DFace = Select3DFace;
                form.SelectSolid = SelectSolid;
                form.SelectPolyfaceMesh = SelectPolyfaceMesh;
                form.EraseEntities = EraseEntities;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    SelectPoints = form.SelectPoints;
                    SelectLines = form.SelectLines;
                    SelectPolylines = form.SelectPolylines;
                    SelectTexts = form.SelectTexts;
                    SelectTextsWithZ = form.SelectTextsWithZ;
                    SelectBlocks = form.SelectBlocks;
                    Select3DFace = form.Select3DFace;
                    SelectSolid = form.SelectSolid;
                    SelectPolyfaceMesh = form.SelectPolyfaceMesh;
                    EraseEntities = form.EraseEntities;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool ShowSettingsPoolExcavation()
        {
            using (ExcavationSlopeForm form = new ExcavationSlopeForm())
            {
                form.H = ExcavationH;
                form.V = ExcavationV;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    ExcavationH = form.H;
                    ExcavationV = form.V;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool ShowSettingsProfile()
        {
            using (ProfileSettingsForm form = new ProfileSettingsForm())
            {
                form.GridH = ProfileGridH;
                form.GridV = ProfileGridV;
                form.VScale = ProfileVScale;
                form.TextHeight = TextHeight;
                form.Precision = Precision;
                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    ProfileGridH = form.GridH;
                    ProfileGridV = form.GridV;
                    ProfileVScale = form.VScale;
                    TextHeight = form.TextHeight;
                    Precision = form.Precision;

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

            List<TypedValue> tvs = new List<TypedValue>();
            tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            if (SelectPoints) tvs.Add(new TypedValue((int)DxfCode.Start, "POINT"));
            if (SelectLines) tvs.Add(new TypedValue((int)DxfCode.Start, "LINE"));
            if (SelectPolylines) tvs.Add(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"));
            if (SelectPolylines) tvs.Add(new TypedValue((int)DxfCode.Start, "POLYLINE"));
            if (SelectTexts || SelectTextsWithZ) tvs.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
            if (SelectBlocks) tvs.Add(new TypedValue((int)DxfCode.Start, "INSERT"));
            if (Select3DFace) tvs.Add(new TypedValue((int)DxfCode.Start, "3DFACE"));
            if (SelectSolid) tvs.Add(new TypedValue((int)DxfCode.Start, "SOLID"));
            if (SelectPolyfaceMesh) tvs.Add(new TypedValue((int)DxfCode.Start, "POLYLINE"));
            tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            if (tvs.Count == 2) return new ObjectId[0];

            SelectionFilter ssf = new SelectionFilter(tvs.ToArray());
            PromptSelectionResult res = doc.Editor.GetSelection(ssf);
            if (res.Status != PromptStatus.OK) return new ObjectId[0];

            return res.Value.GetObjectIds();
        }

        IEnumerable<ObjectId> SelectEntititesProfile()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            List<TypedValue> tvs = new List<TypedValue>();
            tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "ARC"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "LINE"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "POLYLINE"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "SPLINE"));
            tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            SelectionFilter ssf = new SelectionFilter(tvs.ToArray());
            PromptSelectionResult res = doc.Editor.GetSelection(ssf);
            if (res.Status != PromptStatus.OK) return new ObjectId[0];

            return res.Value.GetObjectIds();
        }

        Point3dCollection ReadPoints(IEnumerable<ObjectId> items)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            HashSet<Point3d> points = new HashSet<Point3d>(new Point3dComparer(Tolerance.Global));

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in items)
                    {
                        // Point
                        if (SelectPoints && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBPoint)).UnmanagedObject)
                        {
                            DBPoint item = (DBPoint)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.Position);
                        }
                        // Line
                        else if (SelectLines && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Line)).UnmanagedObject)
                        {
                            Line item = (Line)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.StartPoint);
                            points.Add(item.EndPoint);
                        }
                        // LW Polyline
                        else if (SelectPolylines && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline)).UnmanagedObject)
                        {
                            Polyline item = (Polyline)tr.GetObject(id, OpenMode.ForRead);
                            for (int i = 0; i < item.NumberOfVertices; i++)
                            {
                                points.Add(item.GetPoint3dAt(i));
                            }
                        }
                        // 2D Polyline
                        else if (SelectPolylines && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline2d)).UnmanagedObject)
                        {
                            Polyline2d item = (Polyline2d)tr.GetObject(id, OpenMode.ForRead);
                            foreach (ObjectId vId in item)
                            {
                                Vertex2d vertex = (Vertex2d)tr.GetObject(vId, OpenMode.ForRead);
                                points.Add(vertex.Position);
                            }
                        }
                        // 3D Polyline
                        else if (SelectPolylines && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Polyline3d)).UnmanagedObject)
                        {
                            Polyline3d item = (Polyline3d)tr.GetObject(id, OpenMode.ForRead);
                            foreach (ObjectId vId in item)
                            {
                                PolylineVertex3d vertex = (PolylineVertex3d)tr.GetObject(vId, OpenMode.ForRead);
                                points.Add(vertex.Position);
                            }
                        }
                        // Text
                        else if (SelectTexts && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBText)).UnmanagedObject)
                        {
                            DBText item = (DBText)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.Position);
                        }
                        // Text with Z
                        else if (SelectTextsWithZ && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBText)).UnmanagedObject)
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
                        else if (SelectBlocks && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(BlockReference)).UnmanagedObject)
                        {
                            BlockReference item = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.Position);
                        }
                        // 3DFace
                        else if (Select3DFace && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Face)).UnmanagedObject)
                        {
                            Face item = (Face)tr.GetObject(id, OpenMode.ForRead);
                            points.Add(item.GetVertexAt(0));
                            points.Add(item.GetVertexAt(1));
                            points.Add(item.GetVertexAt(2));
                            points.Add(item.GetVertexAt(3));
                        }
                        // PolyFaceMesh
                        else if (SelectPolyfaceMesh && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(PolyFaceMesh)).UnmanagedObject)
                        {
                            PolyFaceMesh item = (PolyFaceMesh)tr.GetObject(id, OpenMode.ForRead);
                            foreach (ObjectId faceId in item)
                            {
                                DBObject obj = tr.GetObject(faceId, OpenMode.ForRead);
                                if (obj is PolyFaceMeshVertex)
                                {
                                    PolyFaceMeshVertex vertex = (PolyFaceMeshVertex)obj;
                                    points.Add(vertex.Position);
                                }
                            }
                        }
                        // Solid (2D)
                        else if (SelectSolid && id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(Solid)).UnmanagedObject)
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

            if (points.Count == 0)
                return new Point3dCollection();
            else
                return new Point3dCollection(points.ToArray());
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

        private bool EnsureSurfaceNotEmpty(Topography.SurfaceType surface)
        {
            if (surface == Topography.SurfaceType.None)
            {
                MessageBox.Show("Yüzey seçilmedi.", "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if ((surface == Topography.SurfaceType.Original ? topo.OriginalTIN : topo.ProposedTIN).Triangles.Count == 0)
            {
                MessageBox.Show("Seçilen yüzey boş.", "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("YUZEYDEMO")]
        public void CreateDemoSurface()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            try
            {
                // Read demo surface from resource
                Point3dCollection points = new Point3dCollection();
                using (System.IO.StringReader reader = new System.IO.StringReader(XCOM.Properties.Resources.DemoSurface))
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
                                points.Add(new Point3d(x, y, z));
                            }
                        }
                    }
                }

                // Create mesh
                topo.SurfaceFromPoints(points, Topography.SurfaceType.Original);
                doc.Editor.WriteMessage(topo.OriginalTIN.Triangles.Count.ToString() + " adet üçgen oluşturuldu.");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("YUZEYOLUSTUR")]
        public void CreateSurface()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = PickSurface();
            if (surface == Topography.SurfaceType.None) return;

            if (ShowSettings())
            {
                try
                {
                    IEnumerable<ObjectId> items = SelectEntitites();
                    Point3dCollection points = ReadPoints(items);

                    if (points.Count > 0)
                    {
                        topo.SurfaceFromPoints(points, surface);
                        doc.Editor.WriteMessage(topo.OriginalTIN.Triangles.Count.ToString() + " adet üçgen oluşturuldu.");
                    }

                    if (EraseEntities)
                    {
                        AcadUtility.AcadEntity.EraseEntities(db, items);
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
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            if (ShowSettings())
            {
                IEnumerable<ObjectId> items = SelectEntitites();
                Point3dCollection points = ReadPoints(items);

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

                        if (EraseEntities)
                        {
                            AcadUtility.AcadEntity.EraseEntities(db, items);
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
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!EnsureSurfaceNotEmpty(surface)) return;
            TriangleNet.Mesh mesh = (surface == Topography.SurfaceType.Original ? topo.OriginalTIN : topo.ProposedTIN);

            // Object type
            PromptKeywordOptions opts = new PromptKeywordOptions("\nÇizim nesneleri [Geçici/3dFace/Point/polyfaceMesh] <Geçici>: ", "Temporary 3dFace Point polyfaceMesh");
            opts.Keywords.Default = "Temporary";
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
            opts.Keywords.Default = "Yes";
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

        [Autodesk.AutoCAD.Runtime.CommandMethod("DRAPE")]
        public void Drape()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!EnsureSurfaceNotEmpty(surface)) return;

            // Pick polyline
            PromptEntityOptions entityOpts = new PromptEntityOptions("\nEğri: ");
            entityOpts.SetRejectMessage("\nSelect a curve.");
            entityOpts.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);

            if (entityRes.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    Autodesk.AutoCAD.DatabaseServices.Curve curve = tr.GetObject(entityRes.ObjectId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Curve;

                    Point3dCollection points = topo.DrapeCurve(curve, surface);

                    Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, curve.Closed, points);
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                    tr.Commit();
                }
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("PROFILEONCURVE")]
        public void ProfileOnCurve()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!EnsureSurfaceNotEmpty(surface)) return;

            // Pick alignment
            bool flag = true;
            ObjectId curveId = ObjectId.Null;
            while (flag)
            {
                PromptEntityOptions entityOpts = new PromptEntityOptions("\nEğri [Seçenekler]: ", "Settings");
                entityOpts.SetRejectMessage("\nSelect a curve.");
                entityOpts.AddAllowedClass(typeof(Curve), false);
                PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);
                if (entityRes.Status == PromptStatus.Keyword)
                {
                    ShowSettingsProfile();
                }
                else if (entityRes.Status == PromptStatus.OK)
                {
                    curveId = entityRes.ObjectId;
                    break;
                }
                else if (entityRes.Status == PromptStatus.Cancel)
                {
                    return;
                }
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;

                // Project curve onto surface
                Curve curve = tr.GetObject(curveId, OpenMode.ForRead) as Curve;
                Point2dCollection points = topo.ProfileOnCurve(curve, surface);

                // Base point for profile drawing
                PromptPointResult pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nProfil başlangıcı: ");
                if (pointRes.Status != PromptStatus.OK)
                {
                    return;
                }
                Point3d basePt = pointRes.Value;

                if (points.Count > 0)
                {
                    // Limits
                    Extents2d ex = AcadUtility.AcadGeometry.Limits(points);

                    // Base level for profile drawing
                    PromptDoubleOptions levelOpts = new PromptDoubleOptions("\nProfil baz kotu: ");
                    levelOpts.DefaultValue = Math.Floor(ex.MinPoint.Y / ProfileGridV) * ProfileGridV;
                    levelOpts.UseDefaultValue = true;
                    PromptDoubleResult levelRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(levelOpts);
                    if (pointRes.Status != PromptStatus.OK)
                    {
                        return;
                    }
                    double startLevel = levelRes.Value;
                    double endLevel = Math.Ceiling(ex.MaxPoint.Y / ProfileGridV + 1) * ProfileGridV;

                    // Base chainage for profile drawing
                    double startCh = 0;
                    flag = true;
                    while (flag)
                    {
                        PromptStringOptions chOpts = new PromptStringOptions("\nProfil baz KM: ");
                        chOpts.DefaultValue = AcadUtility.AcadText.ChainageToString(0, Precision);
                        chOpts.UseDefaultValue = true;
                        PromptResult chRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetString(chOpts);
                        if (chRes.Status != PromptStatus.OK)
                        {
                            return;
                        }
                        if (AcadUtility.AcadText.TryChainageFromString(chRes.StringResult, out startCh))
                        {
                            break;
                        }
                    }
                    double endCh = Math.Ceiling((startCh + ex.MaxPoint.X) / ProfileGridH) * ProfileGridH;

                    // Draw grid
                    IEnumerable<Entity> entities = RoadDesignUtility.DrawProfileFrame(db, basePt, startCh, startLevel, endCh, endLevel, ProfileGridH, ProfileGridV, ProfileVScale, TextHeight, Precision);
                    foreach (Entity ent in entities)
                    {
                        ent.TransformBy(ucs2wcs);
                        btr.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                    }

                    // Draw profile
                    ObjectId profileLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "Profil_Eksen", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 5));
                    Point2dCollection trPoints = new Point2dCollection(points.Count);
                    foreach (Point2d pt in points)
                    {
                        trPoints.Add(new Point2d(basePt.X + pt.X, basePt.Y + (pt.Y - startLevel) * ProfileVScale));
                    }
                    Polyline pline = AcadUtility.AcadEntity.CreatePolyLine(db, false, trPoints);
                    pline.TransformBy(ucs2wcs);
                    pline.LayerId = profileLayerId;
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                }

                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("MULTIPLEPROFILEONCURVE")]
        public void MultipleProfileOnCurve()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!EnsureSurfaceNotEmpty(surface)) return;

            // Pick center alignment
            bool flag = true;
            ObjectId curveId = ObjectId.Null;
            while (flag)
            {
                PromptEntityOptions entityOpts = new PromptEntityOptions("\nAna Eğri [Seçenekler]: ", "Settings");
                entityOpts.SetRejectMessage("\nSelect a curve.");
                entityOpts.AddAllowedClass(typeof(Curve), false);
                PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);
                if (entityRes.Status == PromptStatus.Keyword)
                {
                    ShowSettingsProfile();
                }
                else if (entityRes.Status == PromptStatus.OK)
                {
                    curveId = entityRes.ObjectId;
                    break;
                }
                else if (entityRes.Status == PromptStatus.Cancel)
                {
                    return;
                }
            }

            // Pick other alignments
            IEnumerable<ObjectId> curveIds = SelectEntititesProfile();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;

                // Project curve onto surface
                Curve center = tr.GetObject(curveId, OpenMode.ForRead) as Curve;
                Point2dCollection points = topo.ProfileOnCurve(center, surface);

                // Project other curves
                Dictionary<Entity, Point2dCollection> otherPoints = new Dictionary<Entity, Point2dCollection>();
                foreach (ObjectId id in curveIds)
                {
                    Curve curve = tr.GetObject(id, OpenMode.ForRead) as Curve;
                    Point2dCollection spoints = topo.ProfileOnCurve(curve, center, surface);
                    otherPoints.Add(curve, spoints);
                }

                // Base point for profile drawing
                PromptPointResult pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nProfil başlangıcı: ");
                if (pointRes.Status != PromptStatus.OK)
                {
                    return;
                }
                Point3d basePt = pointRes.Value;

                if (points.Count > 0)
                {
                    // Limits
                    Extents2d ex = AcadUtility.AcadGeometry.Limits(points);
                    foreach (Point2dCollection pts in otherPoints.Values)
                    {
                        ex = AcadUtility.AcadGeometry.Limits(ex, pts);
                    }

                    // Base level for profile drawing
                    PromptDoubleOptions levelOpts = new PromptDoubleOptions("\nProfil baz kotu: ");
                    levelOpts.DefaultValue = Math.Floor(ex.MinPoint.Y / ProfileGridV) * ProfileGridV;
                    levelOpts.UseDefaultValue = true;
                    PromptDoubleResult levelRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(levelOpts);
                    if (pointRes.Status != PromptStatus.OK)
                    {
                        return;
                    }
                    double startLevel = levelRes.Value;
                    double endLevel = Math.Ceiling(ex.MaxPoint.Y / ProfileGridV + 1) * ProfileGridV;

                    // Base chainage for profile drawing
                    double startCh = 0;
                    flag = true;
                    while (flag)
                    {
                        PromptStringOptions chOpts = new PromptStringOptions("\nProfil baz KM: ");
                        chOpts.DefaultValue = AcadUtility.AcadText.ChainageToString(0, Precision);
                        chOpts.UseDefaultValue = true;
                        PromptResult chRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetString(chOpts);
                        if (chRes.Status != PromptStatus.OK)
                        {
                            return;
                        }
                        if (AcadUtility.AcadText.TryChainageFromString(chRes.StringResult, out startCh))
                        {
                            break;
                        }
                    }
                    double endCh = Math.Ceiling((startCh + ex.MaxPoint.X) / ProfileGridH) * ProfileGridH;

                    // Draw grid
                    IEnumerable<Entity> entities = RoadDesignUtility.DrawProfileFrame(db, basePt, startCh, startLevel, endCh, endLevel, ProfileGridH, ProfileGridV, ProfileVScale, TextHeight, Precision);
                    foreach (Entity ent in entities)
                    {
                        ent.TransformBy(ucs2wcs);
                        btr.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                    }

                    // Draw center profile
                    ObjectId profileLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "Profil_Eksen", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 5));
                    Point2dCollection trPoints = new Point2dCollection(points.Count);
                    foreach (Point2d pt in points)
                    {
                        trPoints.Add(new Point2d(basePt.X + pt.X, basePt.Y + (pt.Y - startLevel) * ProfileVScale));
                    }
                    Polyline pline = AcadUtility.AcadEntity.CreatePolyLine(db, false, trPoints);
                    AcadUtility.AcadEntity.MatchEntity(center, pline);
                    pline.LayerId = profileLayerId;
                    pline.TransformBy(ucs2wcs);
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);

                    // Draw other profiles
                    foreach (KeyValuePair<Entity, Point2dCollection> item in otherPoints)
                    {
                        Entity en = item.Key;
                        Point2dCollection spoints = item.Value;
                        Point2dCollection trsPoints = new Point2dCollection(points.Count);
                        foreach (Point2d pt in spoints)
                        {
                            trsPoints.Add(new Point2d(basePt.X + pt.X, basePt.Y + (pt.Y - startLevel) * ProfileVScale));
                        }
                        Polyline spline = AcadUtility.AcadEntity.CreatePolyLine(db, false, trsPoints);
                        AcadUtility.AcadEntity.MatchEntity(en, spline);
                        spline.LayerId = profileLayerId;
                        spline.TransformBy(ucs2wcs);

                        btr.AppendEntity(spline);
                        tr.AddNewlyCreatedDBObject(spline, true);
                    }
                }

                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("KONTUR")]
        public void DrawContourMap()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            // Surface type
            Topography.SurfaceType surface = PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!EnsureSurfaceNotEmpty(surface)) return;

            // Pick interval
            PromptDoubleOptions dblOpts = new PromptDoubleOptions("\nKontur aralığı: ");
            dblOpts.AllowNegative = false;
            dblOpts.AllowZero = false;
            dblOpts.DefaultValue = 1;
            PromptDoubleResult dblRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(dblOpts);

            if (dblRes.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    IEnumerable<Polyline> contours = topo.ContourMap(surface, dblRes.Value);

                    foreach (Polyline pline in contours)
                    {
                        btr.AppendEntity(pline);
                        tr.AddNewlyCreatedDBObject(pline, true);
                    }
                    tr.Commit();
                }
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("HAVUZKAZISI")]
        public void PoolExcavation()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!EnsureSurfaceNotEmpty(surface)) return;
            TriangleNet.Mesh mesh = (surface == Topography.SurfaceType.Original ? topo.OriginalTIN : topo.ProposedTIN);

            // Pick polyline
            bool flag = true;
            ObjectId centerlineId = ObjectId.Null;
            while (flag)
            {
                PromptEntityOptions entityOpts = new PromptEntityOptions("\nKazı tabanı [Seçenekler]:", "Settings");
                entityOpts.SetRejectMessage("\nSelect a curve.");
                entityOpts.AddAllowedClass(typeof(Curve), false);
                PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);
                if (entityRes.Status == PromptStatus.Keyword && entityRes.StringResult == "Settings")
                {
                    ShowSettingsPoolExcavation();
                    continue;
                }
                if (entityRes.Status != PromptStatus.OK)
                {
                    return;
                }

                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead))
                {
                    Curve centerline = tr.GetObject(entityRes.ObjectId, OpenMode.ForRead) as Curve;
                    if (centerline != null)
                    {
                        if (centerline.Closed)
                        {
                            centerlineId = entityRes.ObjectId;

                            tr.Commit();
                            break;
                        }
                        else
                        {
                            Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nCurve must be closed.");
                            tr.Commit();
                        }
                    }
                }
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                List<SlopeSection> sections = new List<SlopeSection>();

                // Divide the curve into equal segments and calculate the 3D slope at each point
                Curve centerline = tr.GetObject(centerlineId, OpenMode.ForRead) as Curve;
                double len = centerline.GetLength();
                int nmax = (int)Math.Ceiling(len / ExcavationStepSize);
                double dist = 0.0;
                double distStep = len / ((double)nmax);
                for (int i = 0; i < nmax; i++)
                {
                    double param = centerline.GetParameterAtDistance(dist);
                    Point3d pt = centerline.GetPointAtParameter(param);
                    Vector3d slope = SlopeAtParam(centerline, param, ExcavationH, ExcavationV);

                    SlopeSection s = new SlopeSection(pt, slope);

                    sections.Add(s);

                    dist += distStep;
                }

                // Intersects slope vectors with surface triangles
                foreach (TriangleNet.Data.Triangle tri in mesh.Triangles)
                {
                    TriangleNet.Data.Vertex v1 = tri.GetVertex(0);
                    TriangleNet.Data.Vertex v2 = tri.GetVertex(1);
                    TriangleNet.Data.Vertex v3 = tri.GetVertex(2);

                    foreach (SlopeSection section in sections)
                    {
                        double t;
                        Point3d pt;
                        if (RayTriangleIntersection(section.BottomPoint, section.Slope,
                            new Point3d(v1.X, v1.Y, v1.Attributes[0]), new Point3d(v2.X, v2.Y, v2.Attributes[0]), new Point3d(v3.X, v3.Y, v3.Attributes[0]),
                            out t, out pt))
                        {
                            if (!section.HasTopPoint || section.CurrentRatio > t)
                            {
                                section.TopPoint = pt;
                                section.CurrentRatio = t;
                            }
                        }
                    }
                }

                // Draw excavation boundries
                Point3dCollection bounds = new Point3dCollection();
                bool closed = true;
                foreach (SlopeSection section in sections)
                {
                    if (section.HasTopPoint)
                    {
                        bounds.Add(section.TopPoint);

                        Line line = AcadUtility.AcadEntity.CreateLine(db, section.BottomPoint, section.TopPoint);
                        line.ColorIndex = 11;
                        btr.AppendEntity(line);
                        tr.AddNewlyCreatedDBObject(line, true);
                    }
                    else
                    {
                        if (bounds.Count > 1)
                        {
                            Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, bounds);
                            btr.AppendEntity(pline);
                            tr.AddNewlyCreatedDBObject(pline, true);
                        }

                        closed = false;
                        bounds = new Point3dCollection();
                    }
                }
                if (bounds.Count > 1)
                {
                    Polyline3d pline = AcadUtility.AcadEntity.CreatePolyLine3d(db, closed, bounds);
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                }

                tr.Commit();
            }
            // TODO: http://stackoverflow.com/questions/3142469/determining-the-intersection-of-a-triangle-and-a-plane
        }

        private class SlopeSection
        {
            private Point3d bottomPoint;
            private Point3d topPoint;
            private Vector3d slope;
            private double currentRatio;
            private bool hasTopPoint;

            public Point3d BottomPoint { get { return bottomPoint; } }
            public Point3d TopPoint { get { return topPoint; } set { topPoint = value; hasTopPoint = true; } }
            public Vector3d Slope { get { return slope; } }
            public double CurrentRatio { get { return currentRatio; } set { currentRatio = value; } }
            public bool HasTopPoint { get { return hasTopPoint; } }

            public SlopeSection(Point3d pt, Vector3d s)
            {
                bottomPoint = pt;
                slope = s;
                hasTopPoint = false;
                currentRatio = 0.0;
            }
        }

        private Vector3d SlopeAtParam(Curve curve, double param, double h, double v)
        {
            Vector3d normal = curve.GetNormalVector(param);
            double xyLen = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y);
            Vector3d slope = new Vector3d(normal.X / xyLen * h, normal.Y / xyLen * h, v);

            return slope / slope.Length;
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
        /// u + v &lt;= 1.0
        /// 
        /// The parametric equation of the line is:
        /// point(t) = p + t * d
        /// where
        /// p is a point in the line
        /// d is a vector that provides the line's direction
        /// </summary>
        bool RayTriangleIntersection(Point3d rayStart, Vector3d rayDir, Point3d vert0, Point3d vert1, Point3d vert2, out double t, out Point3d pt)
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

            if (zmax == zmin)
            {
                return minColor;
            }

            double ratio = (z - zmin) / (zmax - zmin);
            if (ratio > 0.5)
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
