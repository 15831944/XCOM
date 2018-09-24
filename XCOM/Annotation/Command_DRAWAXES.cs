using AcadUtility;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XCOM.Commands.Annotation
{
    public class Command_DRAWAXES
    {
        private bool DrawOnlyLine { get; set; }
        private double TextHeight { get; set; }
        private string TextStyleName { get; set; }
        private string BlockName { get; set; }
        private string AxisAttribute { get; set; }
        private string ChAttribute { get; set; }
        private string ChPrefix { get; set; }
        private int Precision { get; set; }

        private string Prefix { get; set; }
        private string Suffix { get; set; }
        private int Number { get; set; }
        private string AxisName => Prefix + Number.ToString() + Suffix;
        private double AxisDistance { get; set; }
        private string StartCH { get; set; }
        private double AxisLength { get; set; }

        public Command_DRAWAXES()
        {
            DrawOnlyLine = Properties.Settings.Default.Command_DRAWAXES_DrawOnlyLine;
            TextHeight = Properties.Settings.Default.Command_DRAWAXES_TextHeight;
            TextStyleName = Properties.Settings.Default.Command_DRAWAXES_TextStyleName;
            BlockName = Properties.Settings.Default.Command_DRAWAXES_BlockName;
            AxisAttribute = Properties.Settings.Default.Command_DRAWAXES_AxisAttribute;
            ChAttribute = Properties.Settings.Default.Command_DRAWAXES_ChAttribute;
            Precision = Properties.Settings.Default.Command_DRAWAXES_Precision;

            Prefix = "A";
            Suffix = "";
            Number = 1;
            AxisDistance = 0;
            StartCH = "0+000.00";
            AxisLength = 20;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("DRAWAXES")]
        public void DrawAxes()
        {
            if (!CheckLicense.Check())
            {
                return;
            }

            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            Matrix3d ucs2wcs = AcadGraphics.UcsToWcs;
            Matrix3d wcs2ucs = AcadGraphics.WcsToUcs;

            // Alignment type
            PromptKeywordOptions opts = new PromptKeywordOptions("\nYerleşim [Plan/pRofil] <Plan>: ", "Plan pRofile");
            opts.Keywords.Default = "Plan";
            opts.AllowNone = true;
            PromptResult res = doc.Editor.GetKeywords(opts);
            string alignmentStr = res.StringResult;
            if (res.Status == PromptStatus.None)
            {
                alignmentStr = "Plan";
            }
            else if (res.Status != PromptStatus.OK)
            {
                return;
            }
            bool planAlignment = (alignmentStr == "Plan");

            // Pick polyline
            ObjectId centerlineId = ObjectId.Null;
            PromptEntityOptions entityOpts = new PromptEntityOptions("\nEksen: ");
            entityOpts.SetRejectMessage("\nSelect a curve.");
            entityOpts.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult entityRes = ed.GetEntity(entityOpts);
            if (entityRes.Status == PromptStatus.OK)
            {
                centerlineId = entityRes.ObjectId;
            }
            else
            {
                return;
            }

            // Start point
            Point3d startPoint;
            PromptPointResult ptRes = ed.GetPoint("\nBaşlangıç Noktası: ");
            if (ptRes.Status == PromptStatus.OK)
            {
                startPoint = ptRes.Value.TransformBy(ucs2wcs);
            }
            else
            {
                return;
            }

            // Start CH
            PromptStringOptions chOpts = new PromptStringOptions("\nBaşlangıç KM: ");
            chOpts.DefaultValue = StartCH;
            PromptResult stringRes = ed.GetString(chOpts);
            string chRes = stringRes.StringResult;
            if (stringRes.Status == PromptStatus.None)
            {
                chRes = StartCH;
            }
            else if (stringRes.Status != PromptStatus.OK)
            {
                return;
            }
            StartCH = chRes;

            // Print axes
            double totalDistance = 0;
            bool flag = true;
            while (flag)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
                using (TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    // Read block and text style settings
                    ObjectId blockId = ObjectId.Null;
                    ObjectId textStyleId = ObjectId.Null;
                    if (bt.Has(BlockName))
                    {
                        blockId = bt[BlockName];
                    }
                    if (tt.Has(TextStyleName))
                    {
                        textStyleId = tt[TextStyleName];
                    }

                    // Axis distance
                    PromptDoubleOptions distOpts = new PromptDoubleOptions("\nAks mesafesi [Seçenekler/çıKış]: ", "Settings Exit");
                    distOpts.AllowNegative = false;
                    distOpts.DefaultValue = AxisDistance;
                    distOpts.UseDefaultValue = true;
                    PromptDoubleResult distRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(distOpts);
                    if (distRes.Status == PromptStatus.Keyword && distRes.StringResult == "Settings")
                    {
                        ShowSettings();
                        continue;
                    }
                    else if (distRes.Status == PromptStatus.Keyword && distRes.StringResult == "Exit")
                    {
                        break;
                    }
                    else if (distRes.Status != PromptStatus.OK)
                    {
                        break;
                    }

                    // Calcuate axis insertion point and chainage
                    AxisDistance = distRes.Value;
                    if (GetAxisInsertionPoint(tr, centerlineId, startPoint, AxisDistance, totalDistance, planAlignment, out double currentDistance, out Point3d axisPoint, out Vector3d tangentVector))
                    {
                        string ch = ChPrefix + AcadText.ChainageToString(currentDistance + AcadText.ChainageFromString(StartCH), Precision);
                        Vector3d perpendicularVector = tangentVector.RotateBy(Math.PI / 2, Vector3d.ZAxis);
                        double rotation = Vector3d.XAxis.GetAngleTo(tangentVector, Vector3d.ZAxis);

                        // Axis name
                        PromptStringOptions strOpts = new PromptStringOptions("Aks Adı: ");
                        strOpts.DefaultValue = AxisName;
                        PromptResult strRes = ed.GetString(strOpts);
                        string nameRes = strRes.StringResult;
                        if (strRes.Status == PromptStatus.None)
                        {
                            nameRes = AxisName;
                        }
                        else if (strRes.Status != PromptStatus.OK)
                        {
                            break;
                        }

                        // Break axis name into parts
                        var regex = new System.Text.RegularExpressions.Regex(@"\d");
                        if (regex.IsMatch(nameRes))
                        {
                            var match = regex.Match(nameRes);
                            Prefix = nameRes.Substring(0, match.Index);
                            Number = int.Parse(nameRes.Substring(match.Index, match.Length));
                            Suffix = nameRes.Substring(match.Index + match.Length);
                        }

                        if (DrawOnlyLine)
                        {
                            // Draw axis
                            // Line
                            Point3d pt1 = axisPoint - perpendicularVector * AxisLength / 2;
                            Point3d pt2 = axisPoint + perpendicularVector * AxisLength / 2;
                            Line line = AcadEntity.CreateLine(db, pt1, pt2);
                            btr.AppendEntity(line);
                            tr.AddNewlyCreatedDBObject(line, true);
                            // Axis text
                            Point3d ptt = pt2 + perpendicularVector * TextHeight / 2;
                            DBText text = AcadEntity.CreateText(db, ptt, AxisName, TextHeight, rotation, 1, TextHorizontalMode.TextCenter, TextVerticalMode.TextBottom, textStyleId);

                            btr.AppendEntity(text);
                            tr.AddNewlyCreatedDBObject(text, true);
                        }
                        else
                        {
                            // Insert block
                            BlockReference bref = new BlockReference(axisPoint, blockId);
                            bref.Rotation = rotation;
                            bref.ScaleFactors = new Scale3d(1);

                            btr.AppendEntity(bref);
                            tr.AddNewlyCreatedDBObject(bref, true);

                            Dictionary<AttributeReference, AttributeDefinition> dict = new Dictionary<AttributeReference, AttributeDefinition>();

                            BlockTableRecord blockDef = tr.GetObject(blockId, OpenMode.ForRead) as BlockTableRecord;
                            foreach (ObjectId id in blockDef)
                            {
                                AttributeDefinition attDef = tr.GetObject(id, OpenMode.ForRead) as AttributeDefinition;
                                if ((attDef != null) && (!attDef.Constant))
                                {
                                    // Create a new AttributeReference
                                    AttributeReference attRef = new AttributeReference();
                                    dict.Add(attRef, attDef);
                                    attRef.SetAttributeFromBlock(attDef, bref.BlockTransform);
                                    if (string.Compare(attDef.Tag, AxisAttribute, true) == 0)
                                    {
                                        attRef.TextString = AxisName;
                                    }
                                    else if (string.Compare(attDef.Tag, ChAttribute, true) == 0)
                                    {
                                        attRef.TextString = ch;
                                    }
                                    bref.AttributeCollection.AppendAttribute(attRef);
                                    tr.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }

                    }

                    tr.Commit();
                }

                // Increment axis number
                Number += 1;
                totalDistance += AxisDistance;
            }
        }

        private bool GetAxisInsertionPoint(Transaction tr, ObjectId curveId, Point3d startPoint, double axisDistance, double totalDistance, bool isPlan, out double currentDistance, out Point3d axisPoint, out Vector3d tangent)
        {
            try
            {
                Curve centerline = tr.GetObject(curveId, OpenMode.ForRead) as Curve;
                startPoint = centerline.GetClosestPointTo(startPoint, false);

                if (isPlan)
                {
                    double startDistance = centerline.GetDistAtPoint(startPoint);
                    currentDistance = startDistance + totalDistance + axisDistance;
                    double currentParam = centerline.GetParameterAtDistance(currentDistance);
                    axisPoint = centerline.GetPointAtParameter(currentParam);
                    tangent = centerline.GetFirstDerivative(currentParam).GetNormal();
                }
                else
                {
                    using (Plane horizontal = new Plane(Point3d.Origin, Vector3d.YAxis))
                    {
                        Curve planCurve = centerline.GetOrthoProjectedCurve(horizontal);
                        startPoint = planCurve.GetClosestPointTo(startPoint, false);

                        double startDistance = planCurve.GetDistAtPoint(startPoint);
                        currentDistance = startDistance + totalDistance + axisDistance;
                        double currentParam = planCurve.GetParameterAtDistance(currentDistance);
                        Point3d axisPointPlan = planCurve.GetPointAtParameter(currentParam);
                        axisPoint = centerline.GetClosestPointTo(axisPointPlan, Vector3d.YAxis, false);
                        tangent = planCurve.GetFirstDerivative(currentParam).GetNormal();
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Eksen üzerinde aks noktası hesaplanamadı. Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);

                currentDistance = 0;
                axisPoint = Point3d.Origin;
                tangent = Vector3d.XAxis;

                return false;
            }
        }

        private bool ShowSettings()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            using (DrawAxesForm form = new DrawAxesForm())
            {
                List<string> blockNames = new List<string>();
                List<string> styleNames = new List<string>();
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
                using (TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
                {
                    foreach (ObjectId id in bt)
                    {
                        BlockTableRecord blockDef = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
                        if (blockDef.IsLayout || blockDef.IsFromOverlayReference || blockDef.IsFromExternalReference || blockDef.IsAnonymous)
                        {
                            continue;
                        }

                        blockNames.Add(blockDef.Name);
                    }
                    foreach (ObjectId id in tt)
                    {
                        TextStyleTableRecord styleDef = (TextStyleTableRecord)tr.GetObject(id, OpenMode.ForRead);

                        styleNames.Add(styleDef.Name);
                    }
                    tr.Commit();
                }
                form.SetBlockNames(blockNames.ToArray());
                form.SetTextStyleNames(styleNames.ToArray());

                // Read settings
                form.DrawOnlyLine = DrawOnlyLine;
                form.TextHeight = TextHeight;
                form.TextStyleName = TextStyleName;
                form.BlockName = BlockName;
                form.AxisAttribute = AxisAttribute;
                form.ChAttribute = ChAttribute;
                form.ChPrefix = ChPrefix;
                form.Precision = Precision;
                form.UpdateUI();

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    DrawOnlyLine = form.DrawOnlyLine;
                    TextHeight = form.TextHeight;
                    TextStyleName = form.TextStyleName;
                    BlockName = form.BlockName;
                    AxisAttribute = form.AxisAttribute;
                    ChAttribute = form.ChAttribute;
                    ChPrefix = form.ChPrefix;
                    Precision = form.Precision;

                    // Save changes
                    Properties.Settings.Default.Command_DRAWAXES_DrawOnlyLine = DrawOnlyLine;
                    Properties.Settings.Default.Command_DRAWAXES_TextHeight = TextHeight;
                    Properties.Settings.Default.Command_DRAWAXES_TextStyleName = TextStyleName;
                    Properties.Settings.Default.Command_DRAWAXES_BlockName = BlockName;
                    Properties.Settings.Default.Command_DRAWAXES_AxisAttribute = AxisAttribute;
                    Properties.Settings.Default.Command_DRAWAXES_ChAttribute = ChAttribute;
                    Properties.Settings.Default.Command_DRAWAXES_ChPrefix = ChPrefix;
                    Properties.Settings.Default.Command_DRAWAXES_Precision = Precision;
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