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
        private enum InputState
        {
            OK,
            Exit,
            Continue
        }

        private enum AlignmentType
        {
            None,
            Plan,
            Profile
        }

        private enum AxisSelectionMethod
        {
            Distance,
            Point,
            Chainage
        }

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
        private AxisSelectionMethod SelectionMethod { get; set; }
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
            AxisDistance = 40;
            SelectionMethod = AxisSelectionMethod.Point;
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
            AlignmentType alignmentType = GetAlignmentType();
            if (alignmentType == AlignmentType.None)
            {
                return;
            }

            // Pick alignment
            ObjectId centerlineId = GetAlignment();
            if (centerlineId.IsNull)
            {
                return;
            }

            // Start point
            Point3d startPoint;
            PromptPointResult ptRes = ed.GetPoint("\nBaşlangıç noktası: ");
            if (ptRes.Status == PromptStatus.OK)
            {
                startPoint = ptRes.Value.TransformBy(ucs2wcs);
            }
            else
            {
                return;
            }

            // Start CH
            AcadEditor.PromptChainageOptions chOpts = new AcadEditor.PromptChainageOptions("\nBaşlangıç kilometresi: ");
            chOpts.DefaultValue = StartCH;
            chOpts.UseDefaultValue = true;
            AcadEditor.PromptChainageResult chRes = ed.GetChainage(chOpts);
            string chStr = chRes.StringResult;
            if (chRes.Status == PromptStatus.None)
            {
                chStr = StartCH;
            }
            else if (chRes.Status != PromptStatus.OK)
            {
                return;
            }
            StartCH = chStr;

            // Print axes
            double totalDistance = 0;
            bool flag = true;
            bool hasFirstPoint = false;
            while (flag)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
                using (TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    // Calculate axis insertion point and chainage
                    if (GetAxisInsertionPoint(tr, alignmentType, centerlineId, startPoint, hasFirstPoint, totalDistance, out double currentDistance, out Point3d axisPoint, out Vector3d tangentVector))
                    {
                        string ch = ChPrefix + AcadText.ChainageToString(currentDistance + AcadText.ChainageFromString(StartCH), Precision);
                        Vector3d perpendicularVector = tangentVector.RotateBy(Math.PI / 2, Vector3d.ZAxis);
                        double rotation = Vector3d.XAxis.GetAngleTo(tangentVector, Vector3d.ZAxis);

                        // Axis name
                        if (!GetAxisName())
                        {
                            return;
                        }

                        if (DrawOnlyLine)
                        {
                            // Draw axis
                            ObjectId textStyleId = ObjectId.Null;
                            if (tt.Has(TextStyleName))
                            {
                                textStyleId = tt[TextStyleName];
                            }

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
                            // Draw block
                            ObjectId blockId = ObjectId.Null;
                            if (bt.Has(BlockName))
                            {
                                blockId = bt[BlockName];
                            }

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

                        hasFirstPoint = true;

                        tr.Commit();
                    }
                    else
                    {
                        tr.Commit();
                        return;
                    }
                }

                // Increment axis number
                Number += 1;
                totalDistance += AxisDistance;
            }
        }

        private AlignmentType GetAlignmentType()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            PromptKeywordOptions opts = new PromptKeywordOptions("\nYerleşim [Plan/pRofil] <Plan>: ", "Plan pRofile");
            opts.Keywords.Default = "Plan";
            opts.AllowNone = true;
            PromptResult res = ed.GetKeywords(opts);
            string alignmentStr = res.StringResult;
            if (res.Status == PromptStatus.None)
            {
                return AlignmentType.Plan;
            }
            else if (res.Status != PromptStatus.OK)
            {
                return AlignmentType.None;
            }

            return (alignmentStr == "Plan" ? AlignmentType.Plan : AlignmentType.Profile);
        }

        private ObjectId GetAlignment()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            PromptEntityOptions entityOpts = new PromptEntityOptions("\nEksen: ");
            entityOpts.SetRejectMessage("\nSelect a curve.");
            entityOpts.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult entityRes = ed.GetEntity(entityOpts);
            if (entityRes.Status == PromptStatus.OK)
            {
                return entityRes.ObjectId;
            }
            else
            {
                return ObjectId.Null;
            }
        }

        private bool GetAxisInsertionPoint(Transaction tr, AlignmentType alignmentType, ObjectId curveId, Point3d startPoint, bool allowDistance, double totalDistance, out double currentDistance, out Point3d axisPoint, out Vector3d tangent)
        {
            try
            {
                Curve centerline = tr.GetObject(curveId, OpenMode.ForRead) as Curve;
                startPoint = centerline.GetClosestPointTo(startPoint, false);

                while (true)
                {
                    if (!allowDistance && SelectionMethod == AxisSelectionMethod.Distance)
                    {
                        SelectionMethod = AxisSelectionMethod.Point;
                    }

                    InputState state;
                    switch (SelectionMethod)
                    {
                        case AxisSelectionMethod.Distance:
                            state = GetAxisByDistance(out double distance);
                            if (state == InputState.OK)
                            {
                                AxisDistance = distance;
                            }
                            break;
                        case AxisSelectionMethod.Chainage:
                            state = GetAxisByChainage(allowDistance, out double chainage);
                            if (state == InputState.OK)
                            {
                                AxisDistance = chainage - AcadText.ChainageFromString(StartCH) - totalDistance;
                            }
                            break;
                        case AxisSelectionMethod.Point:
                            state = GetAxisByPoint(allowDistance, out Point3d point);
                            if (state == InputState.OK)
                            {
                                if (alignmentType == AlignmentType.Plan)
                                {
                                    AxisDistance = centerline.GetDistAtPoint(point) - centerline.GetDistAtPoint(startPoint) - totalDistance;
                                }
                                else
                                {
                                    using (Plane horizontal = new Plane(Point3d.Origin, Vector3d.YAxis))
                                    {
                                        Curve planCurve = centerline.GetOrthoProjectedCurve(horizontal);
                                        startPoint = planCurve.GetClosestPointTo(startPoint, false);
                                        double startDistance = planCurve.GetDistAtPoint(startPoint);

                                        Point3d axisPointPlan = centerline.GetClosestPointTo(point, Vector3d.YAxis, false);
                                        AxisDistance = planCurve.GetDistAtPoint(axisPointPlan) - startDistance - totalDistance;
                                    }
                                }
                            }
                            break;
                        default:
                            state = InputState.Exit;
                            break;
                    }

                    if (state == InputState.Exit)
                    {
                        currentDistance = 0;
                        axisPoint = Point3d.Origin;
                        tangent = Vector3d.XAxis;
                        return false;
                    }
                    else if (state == InputState.OK)
                    {
                        break;
                    }
                }

                if (alignmentType == AlignmentType.Plan)
                {
                    double startDistance = centerline.GetDistAtPoint(startPoint);
                    currentDistance = startDistance + totalDistance + AxisDistance;
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
                        currentDistance = startDistance + totalDistance + AxisDistance;
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

        private InputState GetAxisByDistance(out double distance)
        {
            distance = 0;
            PromptDoubleOptions opts = new PromptDoubleOptions("\nAks mesafesi [Nokta/Kilometre/Seçenekler/çıKış]: ", "Point Chainage Settings Exit");
            opts.AllowNegative = false;
            opts.DefaultValue = AxisDistance;
            opts.UseDefaultValue = true;
            PromptDoubleResult res = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(opts);
            if (res.Status == PromptStatus.Keyword && res.StringResult == "Point")
            {
                SelectionMethod = AxisSelectionMethod.Point;
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Chainage")
            {
                SelectionMethod = AxisSelectionMethod.Chainage;
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Settings")
            {
                ShowSettings();
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Exit")
            {
                return InputState.Exit;
            }
            else if (res.Status != PromptStatus.OK)
            {
                return InputState.Exit;
            }

            distance = res.Value;
            return InputState.OK;
        }

        private InputState GetAxisByPoint(bool allowDistance, out Point3d point)
        {
            point = Point3d.Origin;
            PromptPointOptions opts;
            if (allowDistance)
            {
                opts = new PromptPointOptions("\nEksen üzerinde aks yeri [Mesafe/Kilometre/Seçenekler/çıKış]: ", "Distance Chainage Settings Exit");
            }
            else
            {
                opts = new PromptPointOptions("\nEksen üzerinde aks yeri [Kilometre/Seçenekler/çıKış]: ", "Chainage Settings Exit");
            }

            PromptPointResult res = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(opts);
            if (res.Status == PromptStatus.Keyword && res.StringResult == "Distance")
            {
                SelectionMethod = AxisSelectionMethod.Distance;
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Chainage")
            {
                SelectionMethod = AxisSelectionMethod.Chainage;
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Settings")
            {
                ShowSettings();
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Exit")
            {
                return InputState.Exit;
            }
            else if (res.Status != PromptStatus.OK)
            {
                return InputState.Exit;
            }

            point = res.Value;
            return InputState.OK;
        }

        private InputState GetAxisByChainage(bool allowDistance, out double ch)
        {
            ch = 0;
            AcadEditor.PromptChainageOptions opts;
            if (allowDistance)
            {
                opts = new AcadEditor.PromptChainageOptions("\nAks kilometresi [Mesafe/Nokta/Seçenekler/çıKış]: ", "Distance Point Settings Exit");
            }
            else
            {
                opts = new AcadEditor.PromptChainageOptions("\nAks kilometresi [Nokta/Seçenekler/çıKış]: ", "Point Settings Exit");
            }

            AcadEditor.PromptChainageResult res = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetChainage(opts);
            if (res.Status == PromptStatus.Keyword && res.StringResult == "Distance")
            {
                SelectionMethod = AxisSelectionMethod.Distance;
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Point")
            {
                SelectionMethod = AxisSelectionMethod.Point;
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Settings")
            {
                ShowSettings();
                return InputState.Continue;
            }
            else if (res.Status == PromptStatus.Keyword && res.StringResult == "Exit")
            {
                return InputState.Exit;
            }
            else if (res.Status != PromptStatus.OK)
            {
                return InputState.Exit;
            }

            if (AcadUtility.AcadText.TryChainageFromString(res.StringResult, out ch))
            {
                return InputState.OK;
            }
            else
            {
                return InputState.Continue;
            }
        }

        private bool GetAxisName()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            PromptStringOptions opts = new PromptStringOptions("Aks adı: ");
            opts.DefaultValue = AxisName;
            PromptResult res = ed.GetString(opts);
            string name = res.StringResult;
            if (res.Status == PromptStatus.None)
            {
                name = AxisName;
            }
            else if (res.Status != PromptStatus.OK)
            {
                return false;
            }

            // Break axis name into parts
            var regex = new System.Text.RegularExpressions.Regex(@"\d");
            if (regex.IsMatch(name))
            {
                var match = regex.Match(name);
                Prefix = name.Substring(0, match.Index);
                Number = int.Parse(name.Substring(match.Index, match.Length));
                Suffix = name.Substring(match.Index + match.Length);
                return true;
            }

            return false;
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