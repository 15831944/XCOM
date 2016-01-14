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
        public class PromptRebarSelectionResult
        {
            public PromptStatus Status { get; private set; }
            public SelectionSet Value { get; private set; }

            public PromptRebarSelectionResult(PromptStatus s, SelectionSet v)
            {
                Status = s;
                Value = v;
            }
        }

        private static SelectionFilter SSPosFilter(bool includeDetached)
        {
            if (includeDetached)
            {
                TypedValue[] tvs = new TypedValue[] {
                    new TypedValue((int)DxfCode.Start, "INSERT")
                };
                return new SelectionFilter(tvs);
            }
            else
            {
                TypedValue[] tvs = new TypedValue[] {
                    new TypedValue((int)DxfCode.Start, "INSERT")
                };
                return new SelectionFilter(tvs);
            }
        }

        private static SelectionFilter SSPosFilter()
        {
            return SSPosFilter(false);
        }

        public static PromptRebarSelectionResult SelectAllPosUser(bool includeDetached)
        {
            try
            {
                PromptSelectionResult res = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection(SSPosFilter(includeDetached));
                if (res.Status == PromptStatus.OK)
                {
                    IEnumerable<ObjectId> idList = FilterBlocks(res.Value.GetObjectIds());
                    SelectionSet set = SelectionSet.FromObjectIds(new List<ObjectId>(idList).ToArray());
                    return new PromptRebarSelectionResult(res.Status, set);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return new PromptRebarSelectionResult(PromptStatus.Error, SelectionSet.FromObjectIds(new ObjectId[0]));
        }

        public static PromptRebarSelectionResult SelectAllPosUser()
        {
            return SelectAllPosUser(false);
        }

        private static string GetBlockName(ObjectId id)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockReference blockRef = tr.GetObject(id, OpenMode.ForRead) as BlockReference;

                    BlockTableRecord block = null;
                    if (blockRef.IsDynamicBlock)
                    {
                        block = tr.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

                    }
                    else
                    {
                        block = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                    }

                    if (block != null) return block.Name;
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return "";
        }

        private static IEnumerable<ObjectId> FilterBlocks(IEnumerable<ObjectId> idList)
        {
            List<ObjectId> result = new List<ObjectId>();

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in idList)
                    {
                        BlockReference blockRef = tr.GetObject(id, OpenMode.ForRead) as BlockReference;

                        BlockTableRecord block = null;
                        if (blockRef.IsDynamicBlock)
                        {
                            block = tr.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        }
                        else
                        {
                            block = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        }

                        if (string.Compare(block.Name, MyCommands.BlockName, StringComparison.OrdinalIgnoreCase) == 0) result.Add(id);
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return result;
        }

        public static IEnumerable<ObjectId> GetAllPos()
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
                            if (it.Current.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(BlockReference)))
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

            return FilterBlocks(list);
        }

        public static IEnumerable<ObjectId> GetPosWithShape(string shape)
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

            return list;
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

        public static void DrawShape(string name, bool showInvisible, Point3d inspt, double height, double rotation)
        {
            DrawShape(PosShape.Shapes[name], showInvisible, inspt, height, rotation);
        }

        public static void DrawShape(PosShape shape, bool showInvisible, Point3d inspt, double height, double rotation)
        {
            IEnumerable<Entity> items = shape.ToDrawable(showInvisible, inspt, height, rotation);

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                    foreach (Entity en in items)
                    {
                        btr.AppendEntity(en);
                        tr.AddNewlyCreatedDBObject(en, true);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString(), "RebarPos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }

        public static Point3d Polar(Point3d pt, double angle, double distance)
        {
            return new Point3d(pt.X + distance * Math.Cos(angle), pt.Y + distance * Math.Sin(angle), pt.Z);
        }

        // Graphics utilities
        public static class Graphics
        {
            public static Matrix3d UcsToWcs
            {
                get
                {
                    Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                    return doc.Editor.CurrentUserCoordinateSystem;
                }
            }

            public static Matrix3d WcsToUcs
            {
                get
                {
                    return UcsToWcs.Inverse();
                }
            }
        }

        // Entity creation
        public static class CreateEntity
        {
            // Returns the id of the Defpoints layer (Creates the layer if it does not exist)
            public static ObjectId GetOrCreateDefpointsLayer()
            {
                return GetOrCreateLayer("Defpoints", null);
            }

            // Returns the id of the layer (Creates the layer if it does not exist)
            public static ObjectId GetOrCreateLayer(string layerName)
            {
                return GetOrCreateLayer(layerName, null);
            }

            // Returns the id of the layer (Creates the layer if it does not exist)
            public static ObjectId GetOrCreateLayer(string layerName, Autodesk.AutoCAD.Colors.Color color)
            {
                ObjectId id = ObjectId.Null;

                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        LayerTable table = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                        if (table.Has(layerName))
                        {
                            id = table[layerName];
                        }
                        else
                        {
                            table.UpgradeOpen();
                            LayerTableRecord layer = new LayerTableRecord();
                            layer.Name = layerName;
                            if (color != null) layer.Color = color;
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

            public static DBText CreateText(Point3d pt, string text, double textHeight, double rotation, double widthFactor, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode, ObjectId textStyleId, ObjectId layerId)
            {
                DBText dbtext = new DBText();
                dbtext.TextString = text;
                dbtext.Height = textHeight;
                dbtext.Rotation = rotation;
                dbtext.WidthFactor = widthFactor;
                dbtext.HorizontalMode = horizontalMode;
                dbtext.VerticalMode = verticalMode;
                /*
                 * If vertical mode is AcDb::kTextBase and horizontal mode is either AcDb::kTextLeft, AcDb::kTextAlign, or AcDb::kTextFit, 
                 * then the position point (DXF group code 10) is the insertion point for the text object and, for AcDb::kTextLeft, 
                 * the alignment point is automatically calculated based on the other parameters in the text object. Any setting made by this function are 
                 * replaced by the newly calculated value.
                 * 
                 * For all other vertical and horizontal mode combinations, the alignment point is used as the insertion point of the text 
                 * and the position point is automatically calculated based on the other parameters in the text object.
                */

                if ((verticalMode == TextVerticalMode.TextBase) &&
                    (horizontalMode == TextHorizontalMode.TextLeft || horizontalMode == TextHorizontalMode.TextAlign || horizontalMode == TextHorizontalMode.TextFit))
                {
                    dbtext.Position = pt;
                }
                else
                {
                    dbtext.AlignmentPoint = pt;
                }

                if (!textStyleId.IsNull) dbtext.TextStyleId = textStyleId;
                if (!layerId.IsNull) dbtext.LayerId = layerId;

                return dbtext;
            }

            public static DBText CreateText(Point3d pt, string text, double textHeight, double rotation, double widthFactor, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode, ObjectId textStyleId)
            {
                return CreateText(pt, text, textHeight, rotation, widthFactor, horizontalMode, verticalMode, ObjectId.Null, ObjectId.Null);
            }

            public static DBText CreateText(Point3d pt, string text, double textHeight, double rotation, double widthFactor, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode)
            {
                return CreateText(pt, text, textHeight, rotation, widthFactor, horizontalMode, verticalMode, ObjectId.Null, ObjectId.Null);
            }

            public static DBText CreateText(Point3d pt, string text, double textHeight, double rotation, double widthFactor)
            {
                return CreateText(pt, text, textHeight, rotation, widthFactor, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null, ObjectId.Null);
            }

            public static DBText CreateText(Point3d pt, string text, double textHeight, double rotation)
            {
                return CreateText(pt, text, textHeight, rotation, 1.0, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null, ObjectId.Null);
            }

            public static DBText CreateText(Point3d pt, string text, double textHeight)
            {
                return CreateText(pt, text, textHeight, 0.0, 1.0, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null, ObjectId.Null);
            }

            public static MText CreateMText(Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint, ObjectId textStyleId, ObjectId layerId)
            {
                MText mtext = new MText();
                mtext.Contents = text;
                mtext.Location = pt;
                mtext.TextHeight = textHeight;
                mtext.Rotation = rotation;
                mtext.Attachment = attachmentPoint;

                if (!textStyleId.IsNull) mtext.TextStyleId = textStyleId;
                if (!layerId.IsNull) mtext.LayerId = layerId;

                return mtext;
            }

            public static MText CreateMText(Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint, ObjectId textStyleId)
            {
                return CreateMText(pt, text, textHeight, rotation, attachmentPoint, ObjectId.Null, ObjectId.Null);
            }

            public static MText CreateMText(Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint)
            {
                return CreateMText(pt, text, textHeight, rotation, attachmentPoint, ObjectId.Null, ObjectId.Null);
            }

            public static MText CreateMText(Point3d pt, string text, double textHeight, double rotation)
            {
                return CreateMText(pt, text, textHeight, rotation, AttachmentPoint.TopLeft, ObjectId.Null, ObjectId.Null);
            }

            public static MText CreateMText(Point3d pt, string text, double textHeight)
            {
                return CreateMText(pt, text, textHeight, 0, AttachmentPoint.TopLeft, ObjectId.Null, ObjectId.Null);
            }

            public static Line CreateLine(Point3d pt1, Point3d pt2, ObjectId layerId)
            {
                Line line = new Line();
                line.StartPoint = pt1;
                line.EndPoint = pt2;
                if (!layerId.IsNull) line.LayerId = layerId;

                return line;
            }

            public static Line CreateLine(Point3d pt1, Point3d pt2)
            {
                return CreateLine(pt1, pt2, ObjectId.Null);
            }

            public static Line[] CreateMLine(Point3d pt1, Point3d pt2, double thickness, ObjectId layerId)
            {
                Vector3d dir = (pt2 - pt1).GetPerpendicularVector();
                if (dir.Length > double.Epsilon) dir /= dir.Length;
                dir *= thickness / 2.0;

                Line line1 = new Line();
                line1.StartPoint = pt1 + dir;
                line1.EndPoint = pt2 + dir;
                if (!layerId.IsNull) line1.LayerId = layerId;

                Line line2 = new Line();
                line2.StartPoint = pt1 - dir;
                line2.EndPoint = pt2 - dir;
                if (!layerId.IsNull) line2.LayerId = layerId;

                return new Line[] { line1, line2 };
            }

            public static Line[] CreateMLine(Point3d pt1, Point3d pt2, double thickness)
            {
                return CreateMLine(pt1, pt2, thickness, ObjectId.Null);
            }
        }
    }
}
