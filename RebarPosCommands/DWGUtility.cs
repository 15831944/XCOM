using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace RebarPosCommands
{
    public static class DWGUtility
    {
        private static SelectionFilter SSPosFilter(bool includeDetached)
        {
            if (includeDetached)
            {
                TypedValue[] tvs = new TypedValue[] {
                    new TypedValue((int)DxfCode.Start, "INSERT"),
                    new TypedValue((int)(DxfCode.BlockName), MyCommands.BlockName)
                };
                return new SelectionFilter(tvs);
            }
            else
            {
                TypedValue[] tvs = new TypedValue[] {
                    new TypedValue((int)DxfCode.Start, "INSERT"),
                    new TypedValue((int)(DxfCode.BlockName), MyCommands.BlockName)
                };
                return new SelectionFilter(tvs);
            }
        }

        private static SelectionFilter SSPosFilter()
        {
            return SSPosFilter(false);
        }

        public static PromptSelectionResult SelectAllPosUser(bool includeDetached)
        {
            try
            {
                return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection(SSPosFilter(includeDetached));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public static PromptSelectionResult SelectAllPosUser()
        {
            return SelectAllPosUser(false);
        }

        public static ObjectId[] GetAllPos()
        {
            List<ObjectId> list = new List<ObjectId>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    using (BlockTableRecordEnumerator it = btr.GetEnumerator())
                    {
                        while (it.MoveNext())
                        {
                            if (it.Current.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(RebarPos)))
                            {
                                list.Add(it.Current);
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }
            return list.ToArray();
        }

        public static ObjectId[] GetPosWithShape(string shape)
        {
            List<ObjectId> list = new List<ObjectId>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    using (BlockTableRecordEnumerator it = btr.GetEnumerator())
                    {
                        while (it.MoveNext())
                        {
                            RebarPos pos = RebarPos.FromObjectId(tr, it.Current);
                            if (pos != null)
                            {
                                if (pos.ShapeName == shape)
                                {
                                    list.Add(it.Current);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }
            return list.ToArray();
        }

        // Returns the maximum pos number within the given pos blocks.
        public static int GetMaximumPosNumber(IEnumerable<ObjectId> ids)
        {
            int num = -1;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in ids)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, id);
                        if (pos != null)
                        {
                            int i = -1;
                            if (int.TryParse(pos.Pos, out i))
                            {
                                num = Math.Max(i, num);
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return num;
        }

        // Returns all text styles
        public static Dictionary<string, ObjectId> GetTextStyles()
        {
            Dictionary<string, ObjectId> list = new Dictionary<string, ObjectId>();

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    TextStyleTable table = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    SymbolTableEnumerator it = table.GetEnumerator();
                    while (it.MoveNext())
                    {
                        ObjectId id = it.Current;
                        TextStyleTableRecord style = (TextStyleTableRecord)tr.GetObject(id, OpenMode.ForRead);
                        list.Add(style.Name, id);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString(), "RebarPos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                tr.Commit();
            }

            return list;
        }

        // Creates a new text style
        public static ObjectId CreateTextStyle(string name, string filename, double scale)
        {
            ObjectId id = ObjectId.Null;

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    TextStyleTable table = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                    if (table.Has(name))
                    {
                        id = table[name];
                    }
                    else
                    {
                        table.UpgradeOpen();
                        TextStyleTableRecord style = new TextStyleTableRecord();
                        style.Name = name;
                        style.FileName = filename;
                        style.XScale = scale;
                        id = table.Add(style);
                        tr.AddNewlyCreatedDBObject(style, true);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString(), "RebarPos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                tr.Commit();
            }

            return id;
        }

        // Returns the id of the Defpoints layer
        public static ObjectId GetDefpointsLayer()
        {
            ObjectId id = ObjectId.Null;

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    LayerTable table = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                    if (table.Has("Defpoints"))
                    {
                        id = table["Defpoints"];
                    }
                    else
                    {
                        table.UpgradeOpen();
                        LayerTableRecord layer = new LayerTableRecord();
                        layer.Name = "Defpoints";
                        id = table.Add(layer);
                        tr.AddNewlyCreatedDBObject(layer, true);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString(), "RebarPos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                tr.Commit();
            }

            return id;
        }

        // Refreshes given items
        public static void RefreshPos(IEnumerable<ObjectId> ids)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId posid in ids)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, posid);
                        pos.UpdateProperties();
                        pos.Save(tr);
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }
        }

        // Refreshes all items with the given shape
        public static void RefreshPosWithShape(string name)
        {
            RefreshPos(GetPosWithShape(name));
        }

        // Refreshes all items
        public static void RefreshAllPos()
        {
            RefreshPos(GetAllPos());
        }

        // Regenerates the drawing window
        public static void Regen()
        {
            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            ed.Regen();
        }

        // Gets the model space background color
        public static System.Drawing.Color ModelBackgroundColor()
        {
            try
            {
                dynamic pref = Autodesk.AutoCAD.ApplicationServices.Application.Preferences;
                uint indexColor = pref.Display.GraphicsWinModelBackgrndColor;
                return System.Drawing.ColorTranslator.FromOle((int)indexColor);
            }
            catch
            {
                return System.Drawing.Color.Black;
            }
        }

        // Zooms to given objects
        public static void ZoomToObjects(IEnumerable<ObjectId> ids)
        {
            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            Extents3d outerext = new Extents3d();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in ids)
                    {
                        Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                        Extents3d ext = ent.GeometricExtents;
                        outerext.AddExtents(ext);
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            outerext.TransformBy(ed.CurrentUserCoordinateSystem.Inverse());
            Point2d min2d = new Point2d(outerext.MinPoint.X, outerext.MinPoint.Y);
            Point2d max2d = new Point2d(outerext.MaxPoint.X, outerext.MaxPoint.Y);

            ViewTableRecord view = new ViewTableRecord();

            view.CenterPoint = min2d + ((max2d - min2d) / 2.0);
            view.Height = max2d.Y - min2d.Y;
            view.Width = max2d.X - min2d.X;

            ed.SetCurrentView(view);
        }

        // Returns the list of standard diamaters
        public static List<int> GetStandardDiameters()
        {
            return PosGroup.Current.StandardDiameters;
        }

        // Returns the maximum bar length in m
        public static double GetMaximumBarLength()
        {
            return PosGroup.Current.MaxBarLength;
        }

        public static void DrawShape(string name, Point3d inspt, double height)
        {
            DrawShape(PosShape.Shapes[name], inspt, height);
        }

        public static void DrawShape(PosShape shape, Point3d inspt, double height)
        {
            Extents3d bounds = shape.Bounds;
            Point3d p1 = bounds.MinPoint;
            Point3d p2 = bounds.MaxPoint;
            double scale = height / (p2.Y - p1.Y);
            Matrix3d trans = Matrix3d.AlignCoordinateSystem(p1, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis,
                inspt, Vector3d.XAxis * scale, Vector3d.YAxis * scale, Vector3d.ZAxis * scale);

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                    Point3dCollection vertices = new Point3dCollection(new Point3d[]{
                            new Point3d(p1.X, p1.Y, 0),
                            new Point3d(p2.X, p1.Y, 0),
                            new Point3d(p2.X, p2.Y, 0),
                            new Point3d(p1.X, p2.Y, 0),
                        });
                    Polyline2d rec = new Polyline2d(Poly2dType.SimplePoly, vertices, 0, true, 0, 0, null);
                    rec.TransformBy(trans);
                    btr.AppendEntity(rec);
                    tr.AddNewlyCreatedDBObject(rec, true);

                    ObjectId hiddenLayer = GetDefpointsLayer();

                    foreach (PosShape.Shape item in shape.Items)
                    {
                        Entity en = null;

                        if (item is PosShape.ShapeLine)
                        {
                            PosShape.ShapeLine line = (PosShape.ShapeLine)item;
                            en = new Line(new Point3d(line.X1, line.Y1, 0), new Point3d(line.X2, line.Y2, 0));
                        }
                        else if (item is PosShape.ShapeArc)
                        {
                            PosShape.ShapeArc arc = (PosShape.ShapeArc)item;
                            en = new Arc(new Point3d(arc.X, arc.Y, 0), arc.R, arc.StartAngle, arc.EndAngle);
                        }
                        else if (item is PosShape.ShapeCircle)
                        {
                            PosShape.ShapeCircle circle = (PosShape.ShapeCircle)item;
                            en = new Circle(new Point3d(circle.X, circle.Y, 0), Vector3d.ZAxis, circle.R);
                        }
                        else if (item is PosShape.ShapeText)
                        {
                            PosShape.ShapeText text = (PosShape.ShapeText)item;
                            DBText dbobj = new DBText();
                            dbobj.TextString = text.Text;
                            dbobj.Position = new Point3d(text.X, text.Y, 0);
                            dbobj.TextStyleId = CreateTextStyle("ShapeDump_" + shape.Name, text.Font, text.Width);
                            dbobj.Height = text.Height;
                            dbobj.WidthFactor = text.Width;
                            dbobj.HorizontalMode = text.HorizontalAlignment;
                            if (text.VerticalAlignment == TextVerticalMode.TextBottom)
                                dbobj.VerticalMode = TextVerticalMode.TextBase;
                            else
                                dbobj.VerticalMode = text.VerticalAlignment;

                            if (dbobj.HorizontalMode != TextHorizontalMode.TextLeft || dbobj.VerticalMode != TextVerticalMode.TextBase)
                            {
                                dbobj.AlignmentPoint = new Point3d(text.X, text.Y, 0);
                            }
                            en = dbobj;
                        }

                        if (en != null)
                        {
                            en.Color = item.Color;
                            if (!item.Visible) en.LayerId = hiddenLayer;
                            en.TransformBy(trans);

                            btr.AppendEntity(en);
                            tr.AddNewlyCreatedDBObject(en, true);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString(), "RebarPos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }
    }
}
