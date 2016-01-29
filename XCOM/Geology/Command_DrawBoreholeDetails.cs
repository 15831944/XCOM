using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;

namespace XCOM.Commands.Geology
{
    public class Command_DrawBoreholeDetails
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("SONDAJDETAY")]
        public static void DrawBoreholeDetails()
        {
            if (!CheckLicense.Check()) return;

            using (DrawBoreholeDetailsForm form = new DrawBoreholeDetailsForm())
            {
                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                    Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                    PromptPointResult res = doc.Editor.GetPoint("Sondaj çizimi başlangıç noktası: ");
                    Point3d pt = res.Value;

                    Dictionary<string, string> headers = form.GetHeaders();
                    Dictionary<string, List<string>> data = form.GetData();
                    List<double> depths = form.GetDepths();
                    string layerName = form.LayerName;
                    double textHeight = form.TextHeight;
                    bool hasGW = form.HasGroundwater;
                    double gwLevel = form.GroundwaterLevel;

                    Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;

                    double xSpacing = 5.3 * textHeight;
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    using (LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead))
                    using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                    {
                        // Get layer
                        ObjectId layerId = ObjectId.Null;
                        Color headerColor = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        Color textColor = Color.FromColorIndex(ColorMethod.ByAci, 1);
                        Color gwColor = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        double tickSize = 0.5 * textHeight;

                        if (lt.Has(layerName))
                        {
                            layerId = lt[layerName];
                        }
                        else
                        {
                            LayerTableRecord ltr = new LayerTableRecord();
                            ltr.Name = layerName;
                            ltr.Color = textColor;
                            lt.UpgradeOpen();
                            layerId = lt.Add(ltr);
                            tr.AddNewlyCreatedDBObject(ltr, true);
                        }

                        Point3d depthLineTop = Point3d.Origin;
                        Point3d depthLineBottom = Point3d.Origin;

                        // Header underline
                        if (data.Count > 0)
                        {
                            Point3d pt1 = new Point3d(pt.X - xSpacing, pt.Y + 0.9 * textHeight, pt.Z);
                            Point3d pt2 = new Point3d(pt.X + xSpacing * data.Count - xSpacing, pt.Y + 0.9 * textHeight, pt.Z);
                            Line line = AcadUtility.AcadEntity.CreateLine(db, pt1.TransformBy(ucs2wcs), pt2.TransformBy(ucs2wcs));
                            line.LayerId = layerId;
                            line.Color = headerColor;
                            btr.AppendEntity(line);
                            tr.AddNewlyCreatedDBObject(line, true);
                        }

                        // Process columns
                        int j = 0;
                        foreach (KeyValuePair<string, List<string>> item in data)
                        {
                            string key = item.Key;

                            // Header text of this column
                            string headerText = headers[key];
                            if (!string.IsNullOrEmpty(headerText))
                            {
                                string[] lines = headerText.Split(' ');
                                Point3d ptText = new Point3d(pt.X + xSpacing * j - xSpacing / 2, pt.Y + 0.6 * textHeight + lines.Length * 1.3 * textHeight, pt.Z);
                                foreach (string line in lines)
                                {
                                    DBText dbtext = AcadUtility.AcadEntity.CreateText(db, ptText.TransformBy(ucs2wcs), line, textHeight, 0, 1, TextHorizontalMode.TextMid, TextVerticalMode.TextVerticalMid);
                                    dbtext.LayerId = layerId;
                                    dbtext.Color = headerColor;
                                    btr.AppendEntity(dbtext);
                                    tr.AddNewlyCreatedDBObject(dbtext, true);
                                    ptText = new Point3d(ptText.X, ptText.Y - textHeight * 1.3, ptText.Z);
                                }
                            }

                            // Item texts for this column
                            int i = 0;
                            foreach (string text in item.Value)
                            {
                                if (!string.IsNullOrEmpty(text))
                                {
                                    if (string.Compare(key, "Depth", StringComparison.InvariantCultureIgnoreCase) == 0)
                                    {
                                        // Item text
                                        double depth = depths[i];
                                        Point3d ptText = new Point3d(pt.X + xSpacing * j, pt.Y - depth, pt.Z);
                                        DBText dbtext = AcadUtility.AcadEntity.CreateText(db, ptText.TransformBy(ucs2wcs), text, textHeight, 0, 1, TextHorizontalMode.TextRight, TextVerticalMode.TextVerticalMid);
                                        dbtext.LayerId = layerId;
                                        btr.AppendEntity(dbtext);
                                        tr.AddNewlyCreatedDBObject(dbtext, true);

                                        // Horizontal depth ticks
                                        Point3d pt1 = new Point3d(ptText.X + 0.25, ptText.Y, ptText.Z);
                                        Point3d pt2 = new Point3d(ptText.X + 0.25 + tickSize, ptText.Y, ptText.Z);
                                        Line line = AcadUtility.AcadEntity.CreateLine(db, pt1.TransformBy(ucs2wcs), pt2.TransformBy(ucs2wcs));
                                        line.LayerId = layerId;
                                        btr.AppendEntity(line);
                                        tr.AddNewlyCreatedDBObject(line, true);

                                        if (depthLineTop.GetAsVector().IsZeroLength()) depthLineTop = pt1;
                                        depthLineBottom = pt1;
                                    }
                                    else
                                    {
                                        // Item text
                                        double depth = depths[i];
                                        Point3d ptText = new Point3d(pt.X + xSpacing * j - xSpacing / 2, pt.Y - depth, pt.Z);
                                        DBText dbtext = AcadUtility.AcadEntity.CreateText(db, ptText.TransformBy(ucs2wcs), text, textHeight, 0, 1, TextHorizontalMode.TextMid, TextVerticalMode.TextVerticalMid);
                                        dbtext.LayerId = layerId;
                                        btr.AppendEntity(dbtext);
                                        tr.AddNewlyCreatedDBObject(dbtext, true);
                                    }
                                }
                                i++;
                            }
                            j++;
                        }

                        // Vertical depth line
                        if (!depthLineTop.GetAsVector().IsZeroLength())
                        {
                            Point3d pt1 = new Point3d(depthLineTop.X + tickSize / 2, pt.Y, depthLineTop.Z);
                            Point3d pt2 = new Point3d(depthLineBottom.X + tickSize / 2, depthLineBottom.Y, depthLineBottom.Z);
                            Line line = AcadUtility.AcadEntity.CreateLine(db, pt1.TransformBy(ucs2wcs), pt2.TransformBy(ucs2wcs));
                            line.LayerId = layerId;
                            btr.AppendEntity(line);
                            tr.AddNewlyCreatedDBObject(line, true);
                        }

                        // Ground water
                        if (hasGW)
                        {
                            // Horizontal line
                            Point3d pt1 = new Point3d(pt.X + 0.25 - 6.6 * textHeight, pt.Y - gwLevel, pt.Z);
                            Point3d pt2 = new Point3d(pt.X + 0.25, pt.Y - gwLevel, pt.Z);
                            Line line = AcadUtility.AcadEntity.CreateLine(db, pt1.TransformBy(ucs2wcs), pt2.TransformBy(ucs2wcs));
                            line.LayerId = layerId;
                            line.Color = gwColor;
                            btr.AppendEntity(line);
                            tr.AddNewlyCreatedDBObject(line, true);

                            // GW marker
                            double markerSize = 0.8 * textHeight;

                            Polyline pline = new Polyline(1);
                            pline.Normal = ucs2wcs.CoordinateSystem3d.Zaxis;
                            pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                            Plane plinePlane = new Plane(Point3d.Origin, pline.Normal);
                            pline.Reset(false, 3);
                            Point3d wcsPt1 = new Point3d(pt.X + 0.25 - 4.6 * textHeight, pt.Y - gwLevel, pt.Z).TransformBy(ucs2wcs); // Convert to WCS
                            Point2d ecsPt1 = plinePlane.ParameterOf(wcsPt1); // Convert to ECS
                            pline.AddVertexAt(pline.NumberOfVertices, ecsPt1, 0, 0, 0);
                            Point3d wcsPt2 = new Point3d(pt.X + 0.25 - 4.6 * textHeight - markerSize, pt.Y - gwLevel + markerSize, pt.Z).TransformBy(ucs2wcs); // Convert to WCS
                            Point2d ecsPt2 = plinePlane.ParameterOf(wcsPt2); // Convert to ECS
                            pline.AddVertexAt(pline.NumberOfVertices, ecsPt2, 0, 0, 0);
                            Point3d wcsPt3 = new Point3d(pt.X + 0.25 - 4.6 * textHeight + markerSize, pt.Y - gwLevel + markerSize, pt.Z).TransformBy(ucs2wcs); // Convert to WCS
                            Point2d ecsPt3 = plinePlane.ParameterOf(wcsPt3); // Convert to ECS
                            pline.AddVertexAt(pline.NumberOfVertices, ecsPt3, 0, 0, 0);
                            pline.Closed = true;
                            pline.LayerId = layerId;
                            pline.Color = gwColor;
                            btr.AppendEntity(pline);
                            tr.AddNewlyCreatedDBObject(pline, true);

                            // GW text
                            Point3d ptText = new Point3d(pt.X + 0.25 - 4.6 * textHeight, pt.Y - gwLevel + markerSize, pt.Z);
                            DBText dbtext = AcadUtility.AcadEntity.CreateText(db, ptText.TransformBy(ucs2wcs), gwLevel.ToString(), textHeight, 0, 1, TextHorizontalMode.TextCenter, TextVerticalMode.TextBottom);
                            dbtext.LayerId = layerId;
                            dbtext.Color = gwColor;
                            btr.AppendEntity(dbtext);
                            tr.AddNewlyCreatedDBObject(dbtext, true);
                        }

                        tr.Commit();
                    }
                }
            }
        }
    }
}
