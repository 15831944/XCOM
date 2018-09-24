using AcadUtility;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;

namespace XCOM.Commands.Annotation
{
    public class Command_KMYAZ
    {
        private double TextHeight { get; set; }
        private int Precision { get; set; }
        private string Prefix { get; set; }
        private string TextStyleName { get; set; }
        private double Interval { get; set; }

        [Autodesk.AutoCAD.Runtime.CommandMethod("KMYAZ")]
        public void PrintChainage()
        {
            if (!CheckLicense.Check())
            {
                return;
            }

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            ObjectId textStyleId = ObjectId.Null;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
            {
                if (tt.Has(TextStyleName))
                {
                    textStyleId = tt[TextStyleName];
                }
                tr.Commit();
            }

            Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;
            Matrix3d wcs2ucs = AcadUtility.AcadGraphics.WcsToUcs;

            // Pick polyline
            bool flag = true;
            ObjectId centerlineId = ObjectId.Null;
            while (flag)
            {
                PromptEntityOptions entityOpts = new PromptEntityOptions("\nEksen: [Seçenekler]", "Settings");
                entityOpts.SetRejectMessage("\nSelect a curve.");
                entityOpts.AddAllowedClass(typeof(Curve), false);
                PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);

                if (entityRes.Status == PromptStatus.Keyword && entityRes.StringResult == "Settings")
                {
                    ShowSettings();
                    continue;
                }
                else if (entityRes.Status == PromptStatus.OK)
                {
                    centerlineId = entityRes.ObjectId;
                    break;
                }
                else
                {
                    return;
                }
            }

            // Start point
            Point3d startPoint;
            PromptPointResult pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nBaşlangıç Noktası: ");
            if (pointRes.Status == PromptStatus.OK)
            {
                startPoint = pointRes.Value.TransformBy(ucs2wcs);
            }
            else
            {
                return;
            }

            // Start CH
            string startCH = "";
            PromptResult stringRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetString("\nBaşlangıç KM: <0+000.00>");
            startCH = stringRes.StringResult;
            if (stringRes.Status == PromptStatus.None)
            {
                startCH = "0+000.00";
            }
            else if (stringRes.Status != PromptStatus.OK)
            {
                return;
            }

            // Print chainages
            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            using (TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
            {
                Autodesk.AutoCAD.DatabaseServices.Curve centerline = tr.GetObject(centerlineId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Curve;
                if (centerline != null)
                {
                    double curveStartCH = AcadText.ChainageFromString(startCH) - centerline.GetDistAtPoint(startPoint);
                    double distToNearestInterval = Math.Ceiling(curveStartCH / Interval) * Interval - curveStartCH;
                    double currentDistance = distToNearestInterval;
                    double curveLength = centerline.GetDistanceAtParameter(centerline.EndParam);
                    while (currentDistance < curveLength)
                    {
                        string currentCH = AcadText.ChainageToString(currentDistance + curveStartCH, Precision);
                        Point3d currentPoint = centerline.GetPointAtDist(currentDistance);
                        Vector3d perp = centerline.GetFirstDerivative(currentPoint).RotateBy(Math.PI / 2.0, Vector3d.ZAxis);
                        perp /= perp.Length;
                        Point3d lineStart = currentPoint + perp * TextHeight / 2.0;
                        Point3d lineEnd = currentPoint - perp * TextHeight / 2.0;
                        Point3d textStart = currentPoint + perp * TextHeight / 2.0 * 1.2;

                        // Tick mark
                        Line line = new Line();
                        line.StartPoint = lineStart;
                        line.EndPoint = lineEnd;
                        btr.AppendEntity(line);
                        tr.AddNewlyCreatedDBObject(line, true);

                        // CH text
                        double textRotation = Vector3d.XAxis.GetAngleTo(perp, Vector3d.ZAxis);
                        double rot = textRotation * 180 / Math.PI;
                        TextHorizontalMode textHorizontalMode = TextHorizontalMode.TextLeft;
                        if (rot > 90.0 && rot < 270.0)
                        {
                            textRotation = textRotation + Math.PI;
                            textHorizontalMode = TextHorizontalMode.TextRight;
                        }
                        textStyleId = ObjectId.Null;
                        if (tt.Has(TextStyleName))
                        {
                            textStyleId = tt[TextStyleName];
                        }
                        DBText text = AcadEntity.CreateText(db, textStart, currentCH, TextHeight, textRotation, 0.8, textHorizontalMode, TextVerticalMode.TextVerticalMid, textStyleId);

                        btr.AppendEntity(text);
                        tr.AddNewlyCreatedDBObject(text, true);

                        currentDistance += Interval;
                    }
                }

                tr.Commit();
            }
        }

        public Command_KMYAZ()
        {
            TextHeight = Properties.Settings.Default.Command_KMYAZ_TextHeight;
            Precision = Properties.Settings.Default.Command_KMYAZ_Precision;
            Prefix = Properties.Settings.Default.Command_KMYAZ_Prefix;
            Interval = Properties.Settings.Default.Command_KMYAZ_Interval;
            TextStyleName = "MTT";
        }

        private bool ShowSettings()
        {
            using (PrintChainageForm form = new PrintChainageForm())
            {
                form.TextHeight = TextHeight;
                form.Precision = Precision;
                form.Prefix = Prefix;
                form.Interval = Interval;

                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                List<string> styleNames = new List<string>();
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (TextStyleTable bt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
                {
                    foreach (ObjectId id in bt)
                    {
                        TextStyleTableRecord style = (TextStyleTableRecord)tr.GetObject(id, OpenMode.ForRead);

                        styleNames.Add(style.Name);
                    }
                    tr.Commit();
                }
                form.SetTextStyleNames(styleNames.ToArray());
                form.TextStyleName = TextStyleName;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    TextHeight = form.TextHeight;
                    Precision = form.Precision;
                    Prefix = form.Prefix;
                    TextStyleName = form.TextStyleName;
                    Interval = form.Interval;

                    // Save changes
                    Properties.Settings.Default.Command_KMYAZ_TextHeight = form.TextHeight;
                    Properties.Settings.Default.Command_KMYAZ_Prefix = form.Prefix;
                    Properties.Settings.Default.Command_KMYAZ_Precision = form.Precision;
                    Properties.Settings.Default.Command_KMYAZ_Interval = form.Interval;
                    Properties.Settings.Default.Save();

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