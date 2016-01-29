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
    public class Command_SEVTARAMA
    {
        private double LineSpacing { get; set; }

        public Command_SEVTARAMA()
        {
            LineSpacing = 1;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("SEVTARAMA")]
        public void DrawGradeLines()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Editor ed = doc.Editor;

            PromptEntityResult topRes = ed.GetEntity("\nŞev üstü ");
            if (topRes.Status != PromptStatus.OK) return;
            if (!topRes.ObjectId.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Curve)))) return;
            PromptEntityResult botRes = ed.GetEntity("\nŞev altı ");
            if (botRes.Status != PromptStatus.OK) return;
            if (!botRes.ObjectId.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Curve)))) return;
            PromptDistanceOptions disOpts = new PromptDistanceOptions("Tarama sıklığı ");
            disOpts.AllowNegative = false;
            disOpts.AllowZero = false;
            disOpts.DefaultValue = LineSpacing;
            disOpts.UseDefaultValue = true;
            PromptDoubleResult disRes = ed.GetDistance(disOpts);
            if (disRes.Status != PromptStatus.OK) return;
            LineSpacing = disRes.Value;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                try
                {
                    Curve top = (Curve)tr.GetObject(topRes.ObjectId, OpenMode.ForRead);
                    Curve bot = (Curve)tr.GetObject(botRes.ObjectId, OpenMode.ForRead);

                    double botLen = Math.Abs(bot.GetDistanceAtParameter(bot.EndParam) - bot.GetDistanceAtParameter(bot.StartParam));
                    double topLen = Math.Abs(top.GetDistanceAtParameter(top.EndParam) - top.GetDistanceAtParameter(bot.StartParam));
                    double len = Math.Max(botLen, topLen);
                    bool rev = (botLen > topLen);
                    int count = (int)Math.Round(len / disRes.Value);

                    if (count > 0)
                    {
                        double startDist = rev ? bot.GetDistanceAtParameter(bot.StartParam) : top.GetDistanceAtParameter(top.StartParam);
                        double endDist = rev ? bot.GetDistanceAtParameter(bot.EndParam) : top.GetDistanceAtParameter(top.EndParam);
                        double distStep = (endDist - startDist) / (double)count;
                        double dist = startDist;
                        bool half = false;

                        for (int i = 0; i <= count; i++)
                        {
                            Point3d pt1, pt2;
                            try
                            {
                                pt1 = (rev ? bot : top).GetPointAtDist(dist);
                                pt2 = (rev ? top : bot).GetClosestPointTo(pt1, false);
                            }
                            catch (System.Exception)
                            {
                                continue;
                            }

                            Point3d startPt = rev ? pt2 : pt1;
                            Point3d endPt = rev ? pt1 : pt2;
                            if (half) endPt = new LineSegment3d(startPt, endPt).MidPoint;

                            Line ln = AcadUtility.AcadEntity.CreateLine(db, startPt, endPt);
                            btr.AppendEntity(ln);
                            tr.AddNewlyCreatedDBObject(ln, true);

                            dist += distStep;
                            half = !half;
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
    }
}
