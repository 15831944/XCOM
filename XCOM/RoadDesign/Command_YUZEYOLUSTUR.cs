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

namespace XCOM.Commands.RoadDesign
{
    public class Command_YUZEYOLUSTUR
    {
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

        public Command_YUZEYOLUSTUR()
        {
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
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("YUZEYOLUSTUR")]
        public void CreateSurface()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = Topography.PickSurface();
            if (surface == Topography.SurfaceType.None) return;

            if (ShowSettings())
            {
                try
                {
                    IEnumerable<ObjectId> items = SelectEntitites();
                    Point3dCollection points = ReadPoints(items);

                    if (points.Count > 0)
                    {
                        Topography topo = Topography.Instance;
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
    }
}
