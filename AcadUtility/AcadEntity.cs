using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.ComponentModel;

namespace AcadUtility
{
    // Entity utilities
    public static class AcadEntity
    {
        /// <summary>
        /// Dumps the object to the console.
        /// </summary>
        /// <param name="obj"></param>
        public static void Dump(this object obj)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.EditorInput.Editor ed = doc.Editor;
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                ed.WriteMessage("\n{0}={1}", name, value);
            }
        }

        // Matches display properties of dest entity to that of the source entity
        public static void MatchEntity(Entity source, Entity dest)
        {
            dest.Color = source.Color;
            dest.CastShadows = source.CastShadows;
            dest.EdgeStyleId = source.EdgeStyleId;
            dest.FaceStyleId = source.EdgeStyleId;
            dest.LayerId = source.LayerId;
            dest.LinetypeId = source.LinetypeId;
            dest.LinetypeScale = source.LinetypeScale;
            dest.LineWeight = source.LineWeight;
            dest.MaterialId = source.MaterialId;
            dest.VisualStyleId = source.VisualStyleId;
        }

        // Returns the id of the Defpoints layer (Creates the layer if it does not exist)
        public static ObjectId GetOrCreateDefpointsLayer(Database db)
        {
            return GetOrCreateLayer(db, "Defpoints", null);
        }

        // Returns the id of the layer (Creates the layer if it does not exist)
        public static ObjectId GetOrCreateLayer(Database db, string layerName)
        {
            return GetOrCreateLayer(db, layerName, null);
        }

        // Returns the id of the layer (Creates the layer if it does not exist)
        public static ObjectId GetOrCreateLayer(Database db, string layerName, Autodesk.AutoCAD.Colors.Color color)
        {
            ObjectId id = ObjectId.Null;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                LayerTable table = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                if (table.Has(layerName))
                {
                    id = table[layerName];
                }
                else
                {
                    LayerTableRecord layer = new LayerTableRecord();
                    layer.Name = layerName;
                    if (color != null)
                    {
                        layer.Color = color;
                    }

                    table.UpgradeOpen();
                    id = table.Add(layer);
                    table.DowngradeOpen();
                    tr.AddNewlyCreatedDBObject(layer, true);
                }

                tr.Commit();
            }

            return id;
        }

        // Creates a new text style
        public static ObjectId GetOrCreateTextStyle(Database db, string name, string filename, double scale)
        {
            ObjectId id = ObjectId.Null;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable table = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                if (table.Has(name))
                {
                    id = table[name];
                }
                else
                {
                    TextStyleTableRecord style = new TextStyleTableRecord();
                    style.Name = name;
                    style.FileName = filename;
                    style.XScale = scale;
                    table.UpgradeOpen();
                    id = table.Add(style);
                    table.DowngradeOpen();
                    tr.AddNewlyCreatedDBObject(style, true);
                }

                tr.Commit();
            }

            return id;
        }

        public static void AddRegAppTableRecord(Database db, string regAppName)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                RegAppTable rat = (RegAppTable)tr.GetObject(db.RegAppTableId, OpenMode.ForRead, false);
                if (!rat.Has(regAppName))
                {
                    rat.UpgradeOpen();
                    RegAppTableRecord ratr = new RegAppTableRecord();
                    ratr.Name = regAppName;
                    rat.Add(ratr);
                    tr.AddNewlyCreatedDBObject(ratr, true);
                }
                tr.Commit();
            }
        }

        public static void AttachXData(DBObject obj, string regAppName, string data)
        {
            using (ResultBuffer rb = new ResultBuffer(new TypedValue((int)DxfCode.ExtendedDataRegAppName, regAppName), new TypedValue((int)DxfCode.ExtendedDataAsciiString, data)))
            {
                obj.XData = rb;
            }
        }

        public static string GetXData(DBObject obj, string regAppName)
        {
            using (ResultBuffer rb = obj.GetXDataForApplication(regAppName))
            {
                if (rb != null)
                {
                    foreach (TypedValue tv in rb)
                    {
                        if (tv.TypeCode == (int)DxfCode.ExtendedDataAsciiString)
                        {
                            return tv.Value.ToString();
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static void EraseEntities(Database db, IEnumerable<ObjectId> items)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        foreach (ObjectId id in items)
                        {
                            DBObject item = tr.GetObject(id, OpenMode.ForWrite);
                            item.Erase();
                        }
                    }
                    catch
                    {
                        ;
                    }
                    tr.Commit();
                }
            }
        }

        public static DBText CreateText(Database db, Point3d pt, string text, double textHeight, double rotation, double widthFactor, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode, ObjectId textStyleId, ObjectId layerId)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                DBText dbtext = new DBText();
                dbtext.SetDatabaseDefaults(db);

                dbtext.TextString = text;
                dbtext.Position = pt;

                dbtext.Height = textHeight;
                dbtext.Rotation = rotation;
                dbtext.WidthFactor = widthFactor;

                if (horizontalMode == TextHorizontalMode.TextLeft)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        dbtext.Justify = AttachmentPoint.TopLeft;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        dbtext.Justify = AttachmentPoint.BaseLeft;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        dbtext.Justify = AttachmentPoint.BottomLeft;
                    }
                    else
                    {
                        dbtext.Justify = AttachmentPoint.MiddleLeft;
                    }
                }
                else if (horizontalMode == TextHorizontalMode.TextRight)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        dbtext.Justify = AttachmentPoint.TopRight;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        dbtext.Justify = AttachmentPoint.BaseRight;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        dbtext.Justify = AttachmentPoint.BottomRight;
                    }
                    else
                    {
                        dbtext.Justify = AttachmentPoint.MiddleRight;
                    }
                }
                else if (horizontalMode == TextHorizontalMode.TextMid || horizontalMode == TextHorizontalMode.TextCenter)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        dbtext.Justify = AttachmentPoint.TopCenter;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        dbtext.Justify = AttachmentPoint.BaseCenter;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        dbtext.Justify = AttachmentPoint.BottomCenter;
                    }
                    else
                    {
                        dbtext.Justify = AttachmentPoint.MiddleCenter;
                    }
                }
                else if (horizontalMode == TextHorizontalMode.TextAlign)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        dbtext.Justify = AttachmentPoint.TopAlign;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        dbtext.Justify = AttachmentPoint.BaseAlign;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        dbtext.Justify = AttachmentPoint.BottomAlign;
                    }
                    else
                    {
                        dbtext.Justify = AttachmentPoint.MiddleAlign;
                    }
                }
                else if (horizontalMode == TextHorizontalMode.TextFit)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        dbtext.Justify = AttachmentPoint.TopFit;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        dbtext.Justify = AttachmentPoint.BaseFit;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        dbtext.Justify = AttachmentPoint.BottomFit;
                    }
                    else
                    {
                        dbtext.Justify = AttachmentPoint.MiddleFit;
                    }
                }

                if (horizontalMode != TextHorizontalMode.TextLeft || verticalMode != TextVerticalMode.TextBase)
                {
                    dbtext.AlignmentPoint = pt;
                    dbtext.AdjustAlignment(db);
                }

                if (!textStyleId.IsNull)
                {
                    dbtext.TextStyleId = textStyleId;
                }

                if (!layerId.IsNull)
                {
                    dbtext.LayerId = layerId;
                }

                return dbtext;
            }
        }

        public static DBText CreateText(Database db, Point3d pt, string text, double textHeight, double rotation, double widthFactor, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode, ObjectId textStyleId)
        {
            return CreateText(db, pt, text, textHeight, rotation, widthFactor, horizontalMode, verticalMode, textStyleId, ObjectId.Null);
        }

        public static DBText CreateText(Database db, Point3d pt, string text, double textHeight, double rotation, double widthFactor, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode)
        {
            return CreateText(db, pt, text, textHeight, rotation, widthFactor, horizontalMode, verticalMode, ObjectId.Null, ObjectId.Null);
        }

        public static DBText CreateText(Database db, Point3d pt, string text, double textHeight, double rotation, double widthFactor)
        {
            return CreateText(db, pt, text, textHeight, rotation, widthFactor, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null, ObjectId.Null);
        }

        public static DBText CreateText(Database db, Point3d pt, string text, double textHeight, double rotation)
        {
            return CreateText(db, pt, text, textHeight, rotation, 1.0, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null, ObjectId.Null);
        }

        public static DBText CreateText(Database db, Point3d pt, string text, double textHeight)
        {
            return CreateText(db, pt, text, textHeight, 0.0, 1.0, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null, ObjectId.Null);
        }

        public static MText CreateMText(Database db, Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint, ObjectId textStyleId, ObjectId layerId)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                MText mtext = new MText();
                mtext.SetDatabaseDefaults(db);

                mtext.Contents = text;
                mtext.Location = pt;
                mtext.TextHeight = textHeight;
                mtext.Rotation = rotation;
                mtext.Attachment = attachmentPoint;

                if (!textStyleId.IsNull)
                {
                    mtext.TextStyleId = textStyleId;
                }

                if (!layerId.IsNull)
                {
                    mtext.LayerId = layerId;
                }

                return mtext;
            }
        }

        public static MText CreateMText(Database db, Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint, ObjectId textStyleId)
        {
            return CreateMText(db, pt, text, textHeight, rotation, attachmentPoint, ObjectId.Null, ObjectId.Null);
        }

        public static MText CreateMText(Database db, Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint)
        {
            return CreateMText(db, pt, text, textHeight, rotation, attachmentPoint, ObjectId.Null, ObjectId.Null);
        }

        public static MText CreateMText(Database db, Point3d pt, string text, double textHeight, double rotation)
        {
            return CreateMText(db, pt, text, textHeight, rotation, AttachmentPoint.TopLeft, ObjectId.Null, ObjectId.Null);
        }

        public static MText CreateMText(Database db, Point3d pt, string text, double textHeight)
        {
            return CreateMText(db, pt, text, textHeight, 0, AttachmentPoint.TopLeft, ObjectId.Null, ObjectId.Null);
        }

        public static AttributeDefinition CreateAttribute(Database db, Point3d pt, string tag, string prompt, string text, double textHeight, double rotation, double widthFactor, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode, ObjectId textStyleId, ObjectId layerId)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                AttributeDefinition attdef = new AttributeDefinition();
                attdef.SetDatabaseDefaults(db);

                attdef.Tag = tag;
                attdef.Prompt = prompt;
                attdef.TextString = text;
                attdef.Position = pt;

                attdef.Height = textHeight;
                attdef.Rotation = rotation;
                attdef.WidthFactor = widthFactor;

                if (horizontalMode == TextHorizontalMode.TextLeft)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        attdef.Justify = AttachmentPoint.TopLeft;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        attdef.Justify = AttachmentPoint.BaseLeft;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        attdef.Justify = AttachmentPoint.BottomLeft;
                    }
                    else
                    {
                        attdef.Justify = AttachmentPoint.MiddleLeft;
                    }
                }
                else if (horizontalMode == TextHorizontalMode.TextRight)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        attdef.Justify = AttachmentPoint.TopRight;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        attdef.Justify = AttachmentPoint.BaseRight;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        attdef.Justify = AttachmentPoint.BottomRight;
                    }
                    else
                    {
                        attdef.Justify = AttachmentPoint.MiddleRight;
                    }
                }
                else if (horizontalMode == TextHorizontalMode.TextMid || horizontalMode == TextHorizontalMode.TextCenter)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        attdef.Justify = AttachmentPoint.TopCenter;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        attdef.Justify = AttachmentPoint.BaseCenter;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        attdef.Justify = AttachmentPoint.BottomCenter;
                    }
                    else
                    {
                        attdef.Justify = AttachmentPoint.MiddleCenter;
                    }
                }
                else if (horizontalMode == TextHorizontalMode.TextAlign)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        attdef.Justify = AttachmentPoint.TopAlign;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        attdef.Justify = AttachmentPoint.BaseAlign;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        attdef.Justify = AttachmentPoint.BottomAlign;
                    }
                    else
                    {
                        attdef.Justify = AttachmentPoint.MiddleAlign;
                    }
                }
                else if (horizontalMode == TextHorizontalMode.TextFit)
                {
                    if (verticalMode == TextVerticalMode.TextTop)
                    {
                        attdef.Justify = AttachmentPoint.TopFit;
                    }
                    else if (verticalMode == TextVerticalMode.TextBase)
                    {
                        attdef.Justify = AttachmentPoint.BaseFit;
                    }
                    else if (verticalMode == TextVerticalMode.TextBottom)
                    {
                        attdef.Justify = AttachmentPoint.BottomFit;
                    }
                    else
                    {
                        attdef.Justify = AttachmentPoint.MiddleFit;
                    }
                }

                if (horizontalMode != TextHorizontalMode.TextLeft || verticalMode != TextVerticalMode.TextBase)
                {
                    attdef.AlignmentPoint = pt;
                    attdef.AdjustAlignment(db);
                }

                if (!textStyleId.IsNull)
                {
                    attdef.TextStyleId = textStyleId;
                }

                if (!layerId.IsNull)
                {
                    attdef.LayerId = layerId;
                }

                return attdef;
            }
        }

        public static Line CreateLine(Database db, Point3d pt1, Point3d pt2, ObjectId layerId)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Line line = new Line();
                line.SetDatabaseDefaults(db);

                line.StartPoint = pt1;
                line.EndPoint = pt2;

                if (!layerId.IsNull)
                {
                    line.LayerId = layerId;
                }

                return line;
            }
        }

        public static Line CreateLine(Database db, Point3d pt1, Point3d pt2)
        {
            return CreateLine(db, pt1, pt2, ObjectId.Null);
        }

        public static Line[] CreateMLine(Database db, Point3d pt1, Point3d pt2, double thickness, ObjectId layerId)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Vector3d dir = (pt2 - pt1).GetPerpendicularVector().GetNormal() * thickness / 2.0;

                return new Line[] {
                    CreateLine(db, pt1 + dir, pt2 + dir, layerId),
                    CreateLine(db, pt1 - dir, pt2 - dir, layerId)
                };
            }
        }

        public static Line[] CreateMLine(Database db, Point3d pt1, Point3d pt2, double thickness)
        {
            return CreateMLine(db, pt1, pt2, thickness, ObjectId.Null);
        }

        public static Circle CreateCircle(Database db, Point3d center, double radius, Vector3d normal, ObjectId layerId)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Circle circle = new Circle();
                circle.SetDatabaseDefaults(db);

                circle.Center = center;
                circle.Radius = radius;
                circle.Normal = normal;

                if (!layerId.IsNull)
                {
                    circle.LayerId = layerId;
                }

                return circle;
            }
        }

        public static Circle CreateCircle(Database db, Point3d center, double radius, ObjectId layerId)
        {
            return CreateCircle(db, center, radius, Vector3d.ZAxis, layerId);
        }

        public static Circle CreateCircle(Database db, Point3d center, double radius)
        {
            return CreateCircle(db, center, radius, Vector3d.ZAxis, ObjectId.Null);
        }

        public static Circle CreateCircle(Database db, Point3d center, double radius, Vector3d normal)
        {
            return CreateCircle(db, center, radius, normal, ObjectId.Null);
        }

        public static Arc CreateArc(Database db, Point3d center, double radius, double startAngle, double endAngle, Vector3d normal, ObjectId layerId)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Arc arc = new Arc(center, normal, radius, startAngle, endAngle);
                arc.SetDatabaseDefaults(db);

                arc.Center = center;
                arc.Radius = radius;
                arc.Normal = normal;
                arc.StartAngle = startAngle;
                arc.EndAngle = endAngle;

                if (!layerId.IsNull)
                {
                    arc.LayerId = layerId;
                }

                return arc;
            }
        }

        public static Arc CreateArc(Database db, Point3d center, double radius, double startAngle, double endAngle, ObjectId layerId)
        {
            return CreateArc(db, center, radius, startAngle, endAngle, Vector3d.ZAxis, layerId);
        }

        public static Arc CreateArc(Database db, Point3d center, double radius, double startAngle, double endAngle)
        {
            return CreateArc(db, center, radius, startAngle, endAngle, Vector3d.ZAxis, ObjectId.Null);
        }

        public static Arc CreateArc(Database db, Point3d center, double radius, double startAngle, double endAngle, Vector3d normal)
        {
            return CreateArc(db, center, radius, startAngle, endAngle, normal, ObjectId.Null);
        }

        public static Polyline CreatePolyLine(Database db, bool closed, params Point3d[] points)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Matrix3d ucs2wcs = AcadGraphics.UcsToWcs;

                Polyline pline = new Polyline(1);
                pline.SetDatabaseDefaults(db);
                pline.Normal = ucs2wcs.CoordinateSystem3d.Zaxis;
                pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                Plane plinePlane = new Plane(Point3d.Origin, pline.Normal);
                pline.Reset(false, points.Length);
                foreach (Point3d pt in points)
                {
                    Point2d ecsPt = plinePlane.ParameterOf(pt); // Convert to ECS
                    pline.AddVertexAt(pline.NumberOfVertices, ecsPt, 0, 0, 0);
                }
                pline.Closed = closed;

                return pline;
            }
        }

        public static Polyline CreatePolyLine(Database db, params Point3d[] points)
        {
            return CreatePolyLine(db, false, points);
        }

        public static Polyline CreatePolyLine(Database db, bool closed, Point2dCollection points)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Matrix3d ucs2wcs = AcadGraphics.UcsToWcs;

                Polyline pline = new Polyline(1);
                pline.SetDatabaseDefaults(db);
                pline.Normal = ucs2wcs.CoordinateSystem3d.Zaxis;
                pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                Plane plinePlane = new Plane(Point3d.Origin, pline.Normal);
                pline.Reset(false, points.Count);
                foreach (Point2d pt in points)
                {
                    Point2d ecsPt = plinePlane.ParameterOf(new Point3d(pt.X, pt.Y, 0)); // Convert to ECS
                    pline.AddVertexAt(pline.NumberOfVertices, ecsPt, 0, 0, 0);
                }
                pline.Closed = closed;

                return pline;
            }
        }

        public static Polyline CreatePolyLine(Database db, bool closed, params Curve[] entities)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Polyline pline = new Polyline(1);
                pline.TransformBy(entities[0].Ecs);
                Point3d start = entities[0].StartPoint.TransformBy(pline.Ecs.Inverse());
                pline.AddVertexAt(0, new Point2d(start.X, start.Y), 0, 0, 0);

                pline.JoinEntities(entities);

                pline.Closed = closed;

                pline.SetDatabaseDefaults(db);
                pline.SetPropertiesFrom(entities[0]);

                return pline;
            }
        }

        public static Polyline3d CreatePolyLine3d(Database db, bool closed, Point3dCollection points)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Polyline3d pline = new Polyline3d(Poly3dType.SimplePoly, points, closed);
                pline.SetDatabaseDefaults(db);
                return pline;
            }
        }

        public static Polyline3d CreatePolyLine3d(Database db, Point3dCollection points)
        {
            return CreatePolyLine3d(db, false, points);
        }

        public static Polyline3d CreatePolyLine3d(Database db, bool closed, params Point3d[] points)
        {
            return CreatePolyLine3d(db, closed, new Point3dCollection(points));
        }

        public static Polyline3d CreatePolyLine3d(Database db, params Point3d[] points)
        {
            return CreatePolyLine3d(db, false, new Point3dCollection(points));
        }

        public static Hatch CreateHatch(Database db, string patternName, double patternScale, double patternAngle)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                Matrix3d ucs2wcs = AcadGraphics.UcsToWcs;

                Hatch hatch = new Hatch();
                hatch.SetDatabaseDefaults(db);

                hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);

                hatch.Normal = ucs2wcs.CoordinateSystem3d.Zaxis;
                hatch.Elevation = 0.0;
                hatch.PatternScale = patternScale;
                hatch.PatternAngle = patternAngle;

                hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);

                return hatch;
            }
        }

        public static BlockReference CreateBlockReference(Database db, ObjectId blockId, Point3d insertionPoint, Scale3d scaleFactors, double rotation)
        {
            using (CurrentDB curr = new CurrentDB(db))
            {
                BlockReference blockRef = new BlockReference(insertionPoint, blockId);
                blockRef.SetDatabaseDefaults(db);

                blockRef.ScaleFactors = scaleFactors;
                blockRef.Rotation = rotation;

                return blockRef;
            }
        }

        public static BlockReference CreateBlockReference(Database db, ObjectId blockId, Point3d insertionPoint, double scale, double rotation)
        {
            return CreateBlockReference(db, blockId, insertionPoint, new Scale3d(scale), rotation);
        }

        // Returns all block definitions
        public static IEnumerable<KeyValuePair<string, ObjectId>> GetBlocks(Database db)
        {
            SortedDictionary<string, ObjectId> list = new SortedDictionary<string, ObjectId>();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable table = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                SymbolTableEnumerator it = table.GetEnumerator();
                while (it.MoveNext())
                {
                    ObjectId id = it.Current;
                    BlockTableRecord block = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    if (block.IsFromExternalReference || block.IsFromOverlayReference || block.IsLayout)
                    {
                        continue;
                    }
                    list.Add(block.Name, id);
                }

                tr.Commit();
            }

            return list;
        }

        // Returns all text styles
        public static IEnumerable<KeyValuePair<string, ObjectId>> GetTextStyles(Database db)
        {
            SortedDictionary<string, ObjectId> list = new SortedDictionary<string, ObjectId>();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                TextStyleTable table = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                SymbolTableEnumerator it = table.GetEnumerator();
                while (it.MoveNext())
                {
                    ObjectId id = it.Current;
                    TextStyleTableRecord style = (TextStyleTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    list.Add(style.Name, id);
                }

                tr.Commit();
            }

            return list;
        }

        // Returns the name of the block
        public static string GetBlockName(Database db, ObjectId id)
        {
            string name = "";

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

                    if (block != null)
                    {
                        name = block.Name;
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return name;
        }

        // Returns the block definition for the given reference
        public static ObjectId GetBlockDefinitionFromReference(Database db, ObjectId referenceId)
        {
            ObjectId definitionId = ObjectId.Null;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockReference blockRef = tr.GetObject(referenceId, OpenMode.ForRead) as BlockReference;
                    if (blockRef.IsDynamicBlock)
                    {
                        definitionId = blockRef.DynamicBlockTableRecord;

                    }
                    else
                    {
                        definitionId = blockRef.BlockTableRecord;
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return definitionId;
        }

        // Determines if the block with the given name exists
        public static bool BlockExists(Database db, string name)
        {
            bool exists = false;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable table = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                exists = table.Has(name);

                tr.Commit();
            }
            return exists;
        }
    }
}
