using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;

namespace GeologyUtilities
{
    public class GeologyUtilities
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("SONDAJDETAY")]
        public static void DrawBoreholeDetails()
        {
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
                    bool hasGW = form.HasGroundwater;
                    double gwLevel = form.GroundwaterLevel;

                    Matrix3d ucs2wcs = doc.Editor.CurrentUserCoordinateSystem;

                    double xSpacing = 4;
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    using (LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead))
                    using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                    {
                        // Get layer
                        ObjectId layerId = ObjectId.Null;
                        Color headerColor = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        Color textColor = Color.FromColorIndex(ColorMethod.ByAci, 1);
                        Color gwColor = Color.FromColorIndex(ColorMethod.ByAci, 4);
                        double tickSize = 0.4;

                        if (lt.Has("SONDAJ TEXT"))
                        {
                            layerId = lt["SONDAJ TEXT"];
                        }
                        else
                        {
                            LayerTableRecord ltr = new LayerTableRecord();
                            ltr.Name = "SONDAJ TEXT";
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
                            Point3d pt1 = new Point3d(pt.X - xSpacing, pt.Y - 0.25, pt.Z);
                            Point3d pt2 = new Point3d(pt.X + xSpacing * data.Count - xSpacing, pt.Y - 0.25, pt.Z);
                            Line line = XCOM.Common.CreateLine(pt1.TransformBy(ucs2wcs), pt2.TransformBy(ucs2wcs));
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
                                Point3d ptText = new Point3d(pt.X + xSpacing * j - xSpacing / 2, pt.Y - depths[0] + 0.5 + lines.Length * 1.0, pt.Z);
                                foreach (string line in lines)
                                {
                                    DBText dbtext = XCOM.Common.CreateText(ptText.TransformBy(ucs2wcs), line, 0.75, 0, TextHorizontalMode.TextMid, TextVerticalMode.TextVerticalMid);
                                    dbtext.LayerId = layerId;
                                    dbtext.Color = headerColor;
                                    btr.AppendEntity(dbtext);
                                    tr.AddNewlyCreatedDBObject(dbtext, true);
                                    ptText = new Point3d(ptText.X, ptText.Y - 1.0, ptText.Z);
                                }
                            }

                            // Item texts for this column
                            int i = 0;
                            foreach (string text in item.Value)
                            {
                                if (!string.IsNullOrEmpty(text))
                                {
                                    // Item text
                                    double depth = depths[i];
                                    Point3d ptText = new Point3d(pt.X + xSpacing * j, pt.Y - depth, pt.Z);
                                    DBText dbtext = XCOM.Common.CreateText(ptText.TransformBy(ucs2wcs), text, 0.75, 0, TextHorizontalMode.TextRight, TextVerticalMode.TextVerticalMid);
                                    dbtext.LayerId = layerId;
                                    btr.AppendEntity(dbtext);
                                    tr.AddNewlyCreatedDBObject(dbtext, true);

                                    // Horizontal depth ticks
                                    if (string.Compare(key, "Depth", StringComparison.InvariantCultureIgnoreCase) == 0)
                                    {
                                        Point3d pt1 = new Point3d(ptText.X + 0.25, ptText.Y, ptText.Z);
                                        Point3d pt2 = new Point3d(ptText.X + 0.25 + tickSize, ptText.Y, ptText.Z);
                                        Line line = XCOM.Common.CreateLine(pt1.TransformBy(ucs2wcs), pt2.TransformBy(ucs2wcs));
                                        line.LayerId = layerId;
                                        btr.AppendEntity(line);
                                        tr.AddNewlyCreatedDBObject(line, true);

                                        if (depthLineTop.GetAsVector().IsZeroLength()) depthLineTop = pt1;
                                        depthLineBottom = pt1;
                                    }
                                }
                                i++;
                            }
                            j++;
                        }

                        // Vertical depth line
                        if (!depthLineTop.GetAsVector().IsZeroLength())
                        {
                            Point3d pt1 = new Point3d(depthLineTop.X + tickSize / 2, depthLineTop.Y, depthLineTop.Z);
                            Point3d pt2 = new Point3d(depthLineBottom.X + tickSize / 2, depthLineBottom.Y, depthLineBottom.Z);
                            Line line = XCOM.Common.CreateLine(pt1.TransformBy(ucs2wcs), pt2.TransformBy(ucs2wcs));
                            line.LayerId = layerId;
                            btr.AppendEntity(line);
                            tr.AddNewlyCreatedDBObject(line, true);
                        }

                        // Ground water
                        if (hasGW)
                        {
                            // Horizontal line
                            Point3d pt1 = new Point3d(pt.X + 0.25 - 5, pt.Y - gwLevel, pt.Z);
                            Point3d pt2 = new Point3d(pt.X + 0.25, pt.Y - gwLevel, pt.Z);
                            Line line = XCOM.Common.CreateLine(pt1.TransformBy(ucs2wcs), pt2.TransformBy(ucs2wcs));
                            line.LayerId = layerId;
                            line.Color = gwColor;
                            btr.AppendEntity(line);
                            tr.AddNewlyCreatedDBObject(line, true);

                            // GW marker
                            Polyline pline = new Polyline(1);
                            pline.Normal = ucs2wcs.CoordinateSystem3d.Zaxis;
                            pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
                            Plane plinePlane = new Plane(Point3d.Origin, pline.Normal);
                            pline.Reset(false, 3);
                            Point3d wcsPt1 = new Point3d(pt.X + 0.25 - 3.5, pt.Y - gwLevel, pt.Z).TransformBy(ucs2wcs); // Convert to WCS
                            Point2d ecsPt1 = plinePlane.ParameterOf(wcsPt1); // Convert to ECS
                            pline.AddVertexAt(pline.NumberOfVertices, ecsPt1, 0, 0, 0);
                            Point3d wcsPt2 = new Point3d(pt.X + 0.25 - 3.5 - 0.6, pt.Y - gwLevel + 0.6, pt.Z).TransformBy(ucs2wcs); // Convert to WCS
                            Point2d ecsPt2 = plinePlane.ParameterOf(wcsPt2); // Convert to ECS
                            pline.AddVertexAt(pline.NumberOfVertices, ecsPt2, 0, 0, 0);
                            Point3d wcsPt3 = new Point3d(pt.X + 0.25 - 3.5 + 0.6, pt.Y - gwLevel + 0.6, pt.Z).TransformBy(ucs2wcs); // Convert to WCS
                            Point2d ecsPt3 = plinePlane.ParameterOf(wcsPt3); // Convert to ECS
                            pline.AddVertexAt(pline.NumberOfVertices, ecsPt3, 0, 0, 0);
                            pline.Closed = true;
                            pline.LayerId = layerId;
                            pline.Color = gwColor;
                            btr.AppendEntity(pline);
                            tr.AddNewlyCreatedDBObject(pline, true);

                            // GW text
                            Point3d ptText = new Point3d(pt.X + 0.25 - 5 + 1.5, pt.Y - gwLevel + 0.6, pt.Z);
                            DBText dbtext = XCOM.Common.CreateText(ptText.TransformBy(ucs2wcs), gwLevel.ToString(), 0.75, 0, TextHorizontalMode.TextCenter, TextVerticalMode.TextBottom);
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
