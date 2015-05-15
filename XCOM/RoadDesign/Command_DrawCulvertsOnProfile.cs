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
    public class Command_DrawCulvertsOnProfile
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

                    Matrix3d ucs2wcs = XCOM.Utility.Graphics.UcsToWcs();

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
                                Polyline outerPoly = XCOM.Utility.Entity.CreatePolyLine(true,
                                    new Point3d(midPt.X - culvert.Width / 2 - culvert.Wall, midPt.Y - culvert.BottomSlab * scale, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X + culvert.Width / 2 + culvert.Wall, midPt.Y - culvert.BottomSlab * scale, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X + culvert.Width / 2 + culvert.Wall, midPt.Y + culvert.Height * scale + culvert.TopSlab * scale, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X - culvert.Width / 2 - culvert.Wall, midPt.Y + culvert.Height * scale + culvert.TopSlab * scale, 0).TransformBy(ucs2wcs)
                                    );
                                outerPoly.LayerId = layerId;
                                ObjectId outerPolyId = btr.AppendEntity(outerPoly);
                                tr.AddNewlyCreatedDBObject(outerPoly, true);

                                // Inner polyline
                                Polyline innerPoly = XCOM.Utility.Entity.CreatePolyLine(true,
                                    new Point3d(midPt.X - culvert.Width / 2, midPt.Y, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X + culvert.Width / 2, midPt.Y, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X + culvert.Width / 2, midPt.Y + culvert.Height * scale, 0).TransformBy(ucs2wcs),
                                    new Point3d(midPt.X - culvert.Width / 2, midPt.Y + culvert.Height * scale, 0).TransformBy(ucs2wcs)
                                    );
                                innerPoly.LayerId = layerId;
                                ObjectId innerPolyId = btr.AppendEntity(innerPoly);
                                tr.AddNewlyCreatedDBObject(innerPoly, true);

                                // Hatch
                                Hatch hatch = XCOM.Utility.Entity.CreateHatch("ANSI31", hatchScale, 0);
                                hatch.LayerId = layerId;
                                btr.AppendEntity(hatch);
                                tr.AddNewlyCreatedDBObject(hatch, true);
                                hatch.Associative = true;

                                hatch.AppendLoop(HatchLoopTypes.External, new ObjectIdCollection { outerPolyId });
                                hatch.AppendLoop(HatchLoopTypes.Default, new ObjectIdCollection { innerPolyId });

                                hatch.EvaluateHatch(true);

                                // Axis
                                Line axis = XCOM.Utility.Entity.CreateLine(new Point3d(midPt.X, basePt.Y, 0).TransformBy(ucs2wcs),
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

                                MText mtext = XCOM.Utility.Entity.CreateMText(textBase.TransformBy(ucs2wcs), text, textHeight, 90);
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
    }
}
