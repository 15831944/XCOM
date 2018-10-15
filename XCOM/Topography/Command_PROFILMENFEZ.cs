using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace XCOM.Commands.Topography
{
    public class Command_PROFILMENFEZ
    {
        private Point3d BasePoint { get; set; }
        private double BaseChainage { get; set; }
        private double BaseLevel { get; set; }
        private double ProfileScale { get; set; }
        private bool DrawCulvertInfo { get; set; }

        private string LayerName { get; set; }
        private double TextHeight { get; set; }
        private double HatchScale { get; set; }

        private IEnumerable<DrawCulvertForm.CulvertInfo> Data { get; set; }

        public Command_PROFILMENFEZ()
        {
            BaseChainage = 0;
            BaseLevel = 0;
            ProfileScale = 10;
            DrawCulvertInfo = true;

            LayerName = "Menfez Profil";
            TextHeight = 2.5;
            HatchScale = 1;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("PROFILMENFEZ")]
        public void DrawCulvertsOnProfile()
        {
            if (!CheckLicense.Check()) return;

            if (!ShowSettings()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead))
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                try
                {
                    // Get layer
                    ObjectId layerId = ObjectId.Null;

                    if (lt.Has(LayerName))
                    {
                        layerId = lt[LayerName];
                    }
                    else
                    {
                        LayerTableRecord ltr = new LayerTableRecord();
                        ltr.Name = LayerName;
                        lt.UpgradeOpen();
                        layerId = lt.Add(ltr);
                        tr.AddNewlyCreatedDBObject(ltr, true);
                    }

                    // Process columns
                    foreach (DrawCulvertForm.CulvertInfo culvert in Data)
                    {
                        Point3d midPt = new Point3d(BasePoint.X + culvert.Chainage - BaseChainage, BasePoint.Y + (culvert.Level - BaseLevel) * this.ProfileScale, 0);

                        // Outer polyline
                        Polyline outerPoly = AcadUtility.AcadEntity.CreatePolyLine(db, true,
                            new Point3d(midPt.X - culvert.Width / 2 - culvert.Wall, midPt.Y - culvert.BottomSlab * ProfileScale, 0).TransformBy(ucs2wcs),
                            new Point3d(midPt.X + culvert.Width / 2 + culvert.Wall, midPt.Y - culvert.BottomSlab * ProfileScale, 0).TransformBy(ucs2wcs),
                            new Point3d(midPt.X + culvert.Width / 2 + culvert.Wall, midPt.Y + culvert.Height * ProfileScale + culvert.TopSlab * ProfileScale, 0).TransformBy(ucs2wcs),
                            new Point3d(midPt.X - culvert.Width / 2 - culvert.Wall, midPt.Y + culvert.Height * ProfileScale + culvert.TopSlab * ProfileScale, 0).TransformBy(ucs2wcs)
                            );
                        outerPoly.LayerId = layerId;
                        ObjectId outerPolyId = btr.AppendEntity(outerPoly);
                        tr.AddNewlyCreatedDBObject(outerPoly, true);

                        // Inner polyline
                        Polyline innerPoly = AcadUtility.AcadEntity.CreatePolyLine(db, true,
                            new Point3d(midPt.X - culvert.Width / 2, midPt.Y, 0).TransformBy(ucs2wcs),
                            new Point3d(midPt.X + culvert.Width / 2, midPt.Y, 0).TransformBy(ucs2wcs),
                            new Point3d(midPt.X + culvert.Width / 2, midPt.Y + culvert.Height * ProfileScale, 0).TransformBy(ucs2wcs),
                            new Point3d(midPt.X - culvert.Width / 2, midPt.Y + culvert.Height * ProfileScale, 0).TransformBy(ucs2wcs)
                            );
                        innerPoly.LayerId = layerId;
                        ObjectId innerPolyId = btr.AppendEntity(innerPoly);
                        tr.AddNewlyCreatedDBObject(innerPoly, true);

                        // Hatch
                        Hatch hatch = AcadUtility.AcadEntity.CreateHatch(db, "ANSI31", HatchScale, 0);
                        hatch.LayerId = layerId;
                        btr.AppendEntity(hatch);
                        tr.AddNewlyCreatedDBObject(hatch, true);
                        hatch.Associative = true;

                        hatch.AppendLoop(HatchLoopTypes.External, new ObjectIdCollection { outerPolyId });
                        hatch.AppendLoop(HatchLoopTypes.Default, new ObjectIdCollection { innerPolyId });

                        hatch.EvaluateHatch(true);

                        // Axis
                        Line axis = AcadUtility.AcadEntity.CreateLine(db, new Point3d(midPt.X, BasePoint.Y, 0).TransformBy(ucs2wcs),
                            new Point3d(midPt.X, midPt.Y + culvert.Height * ProfileScale + culvert.TopSlab * ProfileScale + 4 * ProfileScale, 0).TransformBy(ucs2wcs)
                            );
                        axis.LayerId = layerId;
                        btr.AppendEntity(axis);
                        tr.AddNewlyCreatedDBObject(axis, true);

                        // Texts
                        Point3d textBase = new Point3d(midPt.X - TextHeight * 1.25, midPt.Y + culvert.Height * ProfileScale + culvert.TopSlab * ProfileScale + 0.5 * ProfileScale, 0);
                        string text = "KM: " + AcadUtility.AcadText.ChainageToString(culvert.Chainage) +
                            "\\P" + "(" + culvert.Width.ToString("F2") + "x" + culvert.Height.ToString("F2") + ") KUTU MENFEZ";
                        if (DrawCulvertInfo)
                        {
                            text = text + "\\P" + "L=" + culvert.Length.ToString("F2") + " m" + " FL=" + culvert.Level.ToString("F2") + " m" + " My=%" + culvert.Grade.ToString("F2") + "\\P";

                            if (culvert.Skew == 0)
                                text = text + "V=DİK";
                            else
                                text = text + "V=" + culvert.Skew.ToString("F2") + " g";

                            if (culvert.WellLength != 0)
                                text = text + " Kuyu Boyu=" + culvert.WellLength.ToString("F2") + " m";
                        }

                        MText mtext = AcadUtility.AcadEntity.CreateMText(db, textBase.TransformBy(ucs2wcs), text, TextHeight, Math.PI / 2.0);
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

        private bool ShowSettings()
        {
            using (DrawCulvertForm form = new DrawCulvertForm())
            {
                form.BaseChainage = BaseChainage;
                form.BaseLevel = BaseLevel;
                form.ProfileScale = ProfileScale;
                form.DrawCulvertInfo = DrawCulvertInfo;
                form.TextHeight = TextHeight;
                form.HatchScale = HatchScale;

                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                form.LayerName = LayerName;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    BasePoint = form.BasePoint;
                    BaseChainage = form.BaseChainage;
                    BaseLevel = form.BaseLevel;
                    ProfileScale = form.ProfileScale;
                    DrawCulvertInfo = form.DrawCulvertInfo;
                    LayerName = form.LayerName;
                    TextHeight = form.TextHeight;
                    HatchScale = form.HatchScale;
                    LayerName = form.LayerName;

                    Data = form.GetData();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
