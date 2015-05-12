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
    public class RoadDesign
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("PROFILMENFEZ")]
        public static void DrawCulvertsOnProfile()
        {
            using (DrawCulvertForm form = new DrawCulvertForm())
            {
                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                    Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                    Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));

                    List<DrawCulvertForm.CulvertInfo> data = form.GetData();
                    Point3d basePt = form.BasePoint;
                    double baseCH = form.BaseChainage;
                    double baseLevel = form.BaseLevel;
                    double scale = form.ProfileScale;
                    bool drawCulvertInfo = form.DrawCulvertInfo;

                    string layerName = form.LayerName;
                    double textHeight = form.TextHeight;
                    double hatchScale = form.HatchScale;

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    using (LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead))
                    using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                    {
                        try
                        {
                            // Get layer
                            ObjectId layerId = ObjectId.Null;

                            if (lt.Has(layerName))
                            {
                                layerId = lt[layerName];
                            }
                            else
                            {
                                LayerTableRecord ltr = new LayerTableRecord();
                                ltr.Name = layerName;
                                lt.UpgradeOpen();
                                layerId = lt.Add(ltr);
                                tr.AddNewlyCreatedDBObject(ltr, true);
                            }

                            // Process columns
                            foreach (DrawCulvertForm.CulvertInfo culvert in data)
                            {
                                Point3d midPt = new Point3d(basePt.X + culvert.Chainage - baseCH, basePt.Y + (culvert.Level - baseLevel) * scale, 0);

                                // Outer polyline
                                Polyline outerPoly = CreatePolyLine(true,
                                    new Point3d(midPt.X - culvert.Width / 2 - culvert.Wall, midPt.Y - culvert.BottomSlab * scale, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X + culvert.Width / 2 + culvert.Wall, midPt.Y - culvert.BottomSlab * scale, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X + culvert.Width / 2 + culvert.Wall, midPt.Y + culvert.Height * scale + culvert.TopSlab * scale, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X - culvert.Width / 2 - culvert.Wall, midPt.Y + culvert.Height * scale + culvert.TopSlab * scale, 0).TransformBy(ucs2wcs)
                                    );
                                outerPoly.LayerId = layerId;
                                ObjectId outerPolyId = btr.AppendEntity(outerPoly);
                                tr.AddNewlyCreatedDBObject(outerPoly, true);

                                // Inner polyline
                                Polyline innerPoly = CreatePolyLine(true,
                                    new Point3d(midPt.X - culvert.Width / 2, midPt.Y, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X + culvert.Width / 2, midPt.Y, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X + culvert.Width / 2, midPt.Y + culvert.Height * scale, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X - culvert.Width / 2, midPt.Y + culvert.Height * scale, 0).TransformBy(ucs2wcs)
                                    );
                                innerPoly.LayerId = layerId;
                                ObjectId innerPolyId = btr.AppendEntity(innerPoly);
                                tr.AddNewlyCreatedDBObject(innerPoly, true);

                                // Hatch
                                Hatch hatch = CreateHatch("ANSI31", hatchScale, 0);
                                hatch.LayerId = layerId;
                                btr.AppendEntity(hatch);
                                tr.AddNewlyCreatedDBObject(hatch, true);
                                hatch.Associative = true;

                                hatch.AppendLoop(HatchLoopTypes.External, new ObjectIdCollection { outerPolyId });
                                hatch.AppendLoop(HatchLoopTypes.Default, new ObjectIdCollection { innerPolyId });

                                hatch.EvaluateHatch(true);

                                // Axis
                                Line axis = CreateLine(new Point3d(midPt.X, basePt.Y, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X, midPt.Y + culvert.Height * scale + culvert.TopSlab * scale + 4 * scale, 0).TransformBy(ucs2wcs)
                                    );
                                axis.LayerId = layerId;
                                btr.AppendEntity(axis);
                                tr.AddNewlyCreatedDBObject(axis, true);

                                // Texts
                                Point3d textBase = new Point3d(midPt.X - textHeight * 1.25, midPt.Y + culvert.Height * scale + culvert.TopSlab * scale + 0.5 * scale, 0);
                                string text = "KM: " + DrawCulvertForm.CulvertInfo.ChainageToString(culvert.Chainage) +
                                    "\\P" + "(" + culvert.Width.ToString("F2") + "x" + culvert.Height.ToString("F2") + ") KUTU MENFEZ";
                                if (drawCulvertInfo)
                                {
                                    text = text + "\\P" + "L=" + culvert.Length.ToString("F2") + " m" + " Fl=" + culvert.Level.ToString("F2") + " m" + " My=%" + culvert.Grade.ToString("F2") +
                                        "\\P" + "Verevlilik=" + culvert.Skew.ToString("F2") + " g";
                                }

                                MText mtext = CreateMText(textBase.TransformBy(ucs2wcs), text, textHeight, 90);
                                mtext.LayerId = layerId;
                                btr.AppendEntity(mtext);
                                tr.AddNewlyCreatedDBObject(mtext, true);
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

        private static DBText CreateText(Point3d pt, string text, double textHeight, double rotation, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode, ObjectId textStyleId)
        {
            DBText dbtext = new DBText();
            dbtext.TextString = text;
            dbtext.Position = pt;
            dbtext.Height = textHeight;
            dbtext.Rotation = rotation * Math.PI / 180;
            dbtext.HorizontalMode = horizontalMode;
            dbtext.VerticalMode = verticalMode;

            dbtext.AlignmentPoint = pt;
            if (!textStyleId.IsNull) dbtext.TextStyleId = textStyleId;

            return dbtext;
        }

        private static DBText CreateText(Point3d pt, string text, double textHeight, double rotation, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode)
        {
            return CreateText(pt, text, textHeight, rotation, horizontalMode, verticalMode, ObjectId.Null);
        }

        private static DBText CreateText(Point3d pt, string text, double textHeight, double rotation)
        {
            return CreateText(pt, text, textHeight, rotation, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null);
        }

        private static DBText CreateText(Point3d pt, string text, double textHeight)
        {
            return CreateText(pt, text, textHeight, 0, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null);
        }

        private static Line CreateLine(Point3d pt1, Point3d pt2)
        {
            Line line = new Line();
            line.StartPoint = pt1;
            line.EndPoint = pt2;

            return line;
        }

        private static Polyline CreatePolyLine(bool closed, params Point3d[] points)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));

            Polyline pline = new Polyline(1);
            pline.Normal = db.Ucsxdir.CrossProduct(db.Ucsydir);
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

        private static Polyline CreatePolyLine(params Point3d[] points)
        {
            return CreatePolyLine(false, points);
        }

        private static Hatch CreateHatch(string patternName, double patternScale, double patternAngle)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));

            Hatch hatch = new Hatch();

            hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);

            hatch.Normal = db.Ucsxdir.CrossProduct(db.Ucsydir);
            hatch.Elevation = 0.0;
            hatch.PatternScale = patternScale;
            hatch.PatternAngle = patternAngle;

            hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);

            return hatch;
        }

        private static MText CreateMText(Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint, ObjectId textStyleId)
        {
            MText mtext = new MText();
            mtext.Contents = text;
            mtext.Location = pt;
            mtext.TextHeight = textHeight;
            mtext.Rotation = rotation * Math.PI / 180;
            mtext.Attachment = attachmentPoint;

            if (!textStyleId.IsNull) mtext.TextStyleId = textStyleId;

            return mtext;
        }

        private static MText CreateMText(Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint)
        {
            return CreateMText(pt, text, textHeight, rotation, attachmentPoint, ObjectId.Null);
        }

        private static MText CreateMText(Point3d pt, string text, double textHeight, double rotation)
        {
            return CreateMText(pt, text, textHeight, rotation, AttachmentPoint.TopLeft, ObjectId.Null);
        }

        private static MText CreateMText(Point3d pt, string text, double textHeight)
        {
            return CreateMText(pt, text, textHeight, 0, AttachmentPoint.TopLeft, ObjectId.Null);
        }
    }
}
