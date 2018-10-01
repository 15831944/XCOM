using AcadUtility;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XCOM.Commands.Bridge
{
    public class Command_DRAWAXES
    {
        private readonly double DefaultAxisDistance = 30;

        private enum InputState
        {
            OK,
            Exit,
            Continue
        }

        private enum AxisSelectionMethod
        {
            Distance,
            Point,
            Chainage
        }

        private enum AxisDrawingType
        {
            Line,
            Block
        }

        private AxisDrawingType DrawingType { get; set; }

        private double AxisLineLength { get; set; }
        private double TextHeight { get; set; }
        private string TextStyleName { get; set; }

        private string BlockName { get; set; }
        private string AxisAttribute { get; set; }
        private string ChAttribute { get; set; }
        private string ChPrefix { get; set; }
        private int ChPrecision { get; set; }

        private string Prefix { get; set; }
        private string Suffix { get; set; }
        private int Number { get; set; }
        private string AxisName => Prefix + Number.ToString() + Suffix;

        private Bridge.AlignmentType Alignment { get; set; }
        private ObjectId CenterlineId { get; set; }
        private Point3d StartPoint { get; set; }
        private double StartCH { get; set; }
        private AxisSelectionMethod SelectionMethod { get; set; }
        private double AxisDistance { get; set; }
        private Point3d LastAxisPoint { get; set; }
        private bool FirstRun { get; set; }

        public Command_DRAWAXES()
        {
            DrawingType = (Properties.Settings.Default.Command_DRAWAXES_DrawOnlyLine ? AxisDrawingType.Line : AxisDrawingType.Block);

            AxisLineLength = Properties.Settings.Default.Command_DRAWAXES_AxisLineLength;
            TextHeight = Properties.Settings.Default.Command_DRAWAXES_TextHeight;
            TextStyleName = Properties.Settings.Default.Command_DRAWAXES_TextStyleName;

            BlockName = Properties.Settings.Default.Command_DRAWAXES_BlockName;
            AxisAttribute = Properties.Settings.Default.Command_DRAWAXES_AxisAttribute;
            ChAttribute = Properties.Settings.Default.Command_DRAWAXES_ChAttribute;
            ChPrefix = Properties.Settings.Default.Command_DRAWAXES_ChPrefix;
            ChPrecision = Properties.Settings.Default.Command_DRAWAXES_Precision;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("DRAWAXES")]
        public void DrawAxes()
        {
            if (!CheckLicense.Check())
            {
                return;
            }

            // Reset parameters
            Alignment = Bridge.AlignmentType.None;
            CenterlineId = ObjectId.Null;
            StartPoint = Point3d.Origin;
            StartCH = 0;
            SelectionMethod = AxisSelectionMethod.Point;
            AxisDistance = DefaultAxisDistance;
            LastAxisPoint = Point3d.Origin;

            Prefix = "A";
            Suffix = "";
            Number = 1;

            FirstRun = true;

            // Pick alignment
            if (!GetAlignmentParameters())
            {
                return;
            }

            // Print axes
            while (true)
            {
                // Calculate axis insertion point and chainage
                if (GetAxisInsertionPoint(out Point3d axisPoint, out double axisDistanceFromStartPoint, out Vector3d axisDirection))
                {
                    // Axis name
                    if (!GetAxisName())
                    {
                        return;
                    }

                    if (DrawingType == AxisDrawingType.Line)
                    {
                        DrawAxisLine(axisPoint, axisDirection, StartCH + axisDistanceFromStartPoint);
                    }
                    else
                    {
                        DrawAxisBlock(axisPoint, axisDirection, StartCH + axisDistanceFromStartPoint);
                    }

                    // Increment axis number
                    FirstRun = false;
                    Number += 1;
                    LastAxisPoint = axisPoint;
                }
                else
                {
                    break;
                }
            }
        }

        private bool GetAlignmentParameters()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var ed = doc.Editor;

            Matrix3d ucs2wcs = AcadGraphics.UcsToWcs;
            Matrix3d wcs2ucs = AcadGraphics.WcsToUcs;

            // Alignment type
            Alignment = Bridge.PickAlignment();
            if (Alignment == Bridge.AlignmentType.None)
            {
                return false;
            }

            // Alignment line
            PromptEntityOptions entityOpts = new PromptEntityOptions("\nEksen: ");
            entityOpts.SetRejectMessage("\nSelect a curve.");
            entityOpts.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult entityRes = ed.GetEntity(entityOpts);
            if (entityRes.Status == PromptStatus.OK)
            {
                CenterlineId = entityRes.ObjectId;
            }
            else
            {
                return false;
            }

            // Start point
            PromptPointResult ptRes = ed.GetPoint("\nBaşlangıç noktası: ");
            if (ptRes.Status == PromptStatus.OK)
            {
                StartPoint = ptRes.Value.TransformBy(ucs2wcs);
            }
            else
            {
                return false;
            }

            // Start CH
            AcadEditor.PromptChainageOptions chOpts = new AcadEditor.PromptChainageOptions("\nBaşlangıç kilometresi: ");
            chOpts.DefaultValue = AcadText.ChainageToString(StartCH, ChPrecision);
            chOpts.UseDefaultValue = true;
            AcadEditor.PromptChainageResult chRes = ed.GetChainage(chOpts);
            if (chRes.Status == PromptStatus.OK)
            {
                StartCH = chRes.DoubleResult;
            }
            else if (chRes.Status == PromptStatus.None)
            {
                // Use default
            }
            else if (chRes.Status != PromptStatus.OK)
            {
                return false;
            }

            return true;
        }

        private bool GetAxisInsertionPoint(out Point3d axisPoint, out double axisDistanceFromStartPoint, out Vector3d direction)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            axisPoint = Point3d.Origin;
            axisDistanceFromStartPoint = 0;
            direction = Vector3d.YAxis;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    Curve centerline = tr.GetObject(CenterlineId, OpenMode.ForRead) as Curve;
                    StartPoint = centerline.GetClosestPointTo(StartPoint, false);

                    while (true)
                    {
                        if (FirstRun && SelectionMethod == AxisSelectionMethod.Distance)
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
                                    if (Alignment == Bridge.AlignmentType.Plan)
                                    {
                                        axisDistanceFromStartPoint = centerline.GetDistAtPoint(LastAxisPoint) + AxisDistance;
                                        axisPoint = centerline.GetPointAtDist(axisDistanceFromStartPoint);
                                    }
                                    else
                                    {
                                        using (Plane horizontal = new Plane(Point3d.Origin, Vector3d.YAxis))
                                        {
                                            Curve planCurve = centerline.GetOrthoProjectedCurve(horizontal);
                                            Point3d lastAxisPointOnPlan = planCurve.GetClosestPointTo(LastAxisPoint, false);
                                            axisDistanceFromStartPoint = planCurve.GetDistAtPoint(lastAxisPointOnPlan) + AxisDistance;
                                            Point3d axisPointOnPlan = planCurve.GetPointAtDist(axisDistanceFromStartPoint);
                                            axisPoint = centerline.GetClosestPointTo(axisPointOnPlan, Vector3d.YAxis, false);
                                        }
                                    }
                                }
                                break;
                            case AxisSelectionMethod.Chainage:
                                state = GetAxisByChainage(out double chainage);
                                if (state == InputState.OK)
                                {
                                    if (Alignment == Bridge.AlignmentType.Plan)
                                    {
                                        axisPoint = centerline.GetPointAtDist(chainage - StartCH);
                                        axisDistanceFromStartPoint = chainage - StartCH;
                                        AxisDistance = FirstRun ? DefaultAxisDistance : axisDistanceFromStartPoint - centerline.GetDistAtPoint(LastAxisPoint);
                                    }
                                    else
                                    {
                                        using (Plane horizontal = new Plane(Point3d.Origin, Vector3d.YAxis))
                                        {
                                            Curve planCurve = centerline.GetOrthoProjectedCurve(horizontal);
                                            axisDistanceFromStartPoint = chainage - StartCH;
                                            Point3d axisPointOnPlan = planCurve.GetPointAtDist(axisDistanceFromStartPoint);
                                            axisPoint = centerline.GetClosestPointTo(axisPointOnPlan, Vector3d.YAxis, false);
                                            if (FirstRun)
                                            {
                                                AxisDistance = DefaultAxisDistance;
                                            }
                                            else
                                            {
                                                Point3d lastAxisPointOnPlan = planCurve.GetClosestPointTo(LastAxisPoint, false);
                                                AxisDistance = chainage - planCurve.GetDistAtPoint(lastAxisPointOnPlan);
                                            }
                                        }
                                    }
                                }
                                break;
                            case AxisSelectionMethod.Point:
                                state = GetAxisByPoint(out Point3d point);
                                if (state == InputState.OK)
                                {
                                    axisPoint = point;
                                    if (Alignment == Bridge.AlignmentType.Plan)
                                    {
                                        axisDistanceFromStartPoint = centerline.GetDistAtPoint(point);
                                        AxisDistance = FirstRun ? DefaultAxisDistance : axisDistanceFromStartPoint - centerline.GetDistAtPoint(LastAxisPoint);
                                    }
                                    else
                                    {
                                        using (Plane horizontal = new Plane(Point3d.Origin, Vector3d.YAxis))
                                        {
                                            Curve planCurve = centerline.GetOrthoProjectedCurve(horizontal);
                                            if (FirstRun)
                                            {
                                                AxisDistance = DefaultAxisDistance;
                                            }
                                            else
                                            {
                                                Point3d axisPointOnPlan = planCurve.GetClosestPointTo(axisPoint, false);
                                                Point3d lastAxisPointOnPlan = planCurve.GetClosestPointTo(LastAxisPoint, false);
                                                Point3d axisPointPlan = centerline.GetClosestPointTo(point, Vector3d.YAxis, false);
                                                axisDistanceFromStartPoint = planCurve.GetDistAtPoint(axisPointPlan);
                                                AxisDistance = axisDistanceFromStartPoint - planCurve.GetDistAtPoint(lastAxisPointOnPlan);
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                tr.Commit();
                                return false;
                        }

                        if (state == InputState.OK)
                        {
                            break;
                        }
                        else if (state == InputState.Exit)
                        {
                            tr.Commit();
                            return false;
                        }
                    }

                    if (Alignment == Bridge.AlignmentType.Plan)
                    {
                        double param = centerline.GetParameterAtPoint(axisPoint);
                        Vector3d tangent = centerline.GetFirstDerivative(param).GetNormal();
                        direction = tangent.RotateBy(Math.PI / 2, Vector3d.ZAxis);
                    }
                    else
                    {
                        using (Plane horizontal = new Plane(Point3d.Origin, Vector3d.YAxis))
                        {
                            Curve planCurve = centerline.GetOrthoProjectedCurve(horizontal);
                            Point3d axisPointPlan = planCurve.GetClosestPointTo(axisPoint, false);
                            double param = planCurve.GetParameterAtPoint(axisPointPlan);
                            Vector3d tangent = planCurve.GetFirstDerivative(param).GetNormal();
                            direction = tangent.RotateBy(Math.PI / 2, Vector3d.ZAxis);
                        }
                    }

                    tr.Commit();
                    return true;
                }
                catch
                {
                    ed.WriteMessage("Eksen üzerinde aks noktası hesaplanamadı.");

                    tr.Commit();
                    return false;
                }
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

        private InputState GetAxisByPoint(out Point3d point)
        {
            Matrix3d ucs2wcs = AcadGraphics.UcsToWcs;
            Matrix3d wcs2ucs = AcadGraphics.WcsToUcs;

            point = Point3d.Origin;
            PromptPointOptions opts;
            if (!FirstRun)
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

            point = res.Value.TransformBy(ucs2wcs);
            return InputState.OK;
        }

        private InputState GetAxisByChainage(out double ch)
        {
            ch = 0;
            AcadEditor.PromptChainageOptions opts;
            if (!FirstRun)
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

        private void DrawAxisLine(Point3d point, Vector3d direction, double chainage)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                // Draw axis
                ObjectId textStyleId = ObjectId.Null;
                if (tt.Has(TextStyleName))
                {
                    textStyleId = tt[TextStyleName];
                }

                // Line
                Point3d pt1 = point - direction * AxisLineLength / 2;
                Point3d pt2 = point + direction * AxisLineLength / 2;
                Line line = AcadEntity.CreateLine(db, pt1, pt2);
                btr.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);

                // Axis text
                Point3d ptt = pt2 + direction * TextHeight / 2;
                double rotation = Vector3d.YAxis.GetAngleTo(direction, Vector3d.ZAxis);
                DBText text = AcadEntity.CreateText(db, ptt, AxisName, TextHeight, rotation, 1, TextHorizontalMode.TextCenter, TextVerticalMode.TextBottom, textStyleId);

                btr.AppendEntity(text);
                tr.AddNewlyCreatedDBObject(text, true);

                tr.Commit();
            }
        }

        private void DrawAxisBlock(Point3d point, Vector3d direction, double chainage)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                // Draw block
                ObjectId blockId = ObjectId.Null;
                if (bt.Has(BlockName))
                {
                    blockId = bt[BlockName];
                }

                // Chainage string
                string chainageString = ChPrefix + AcadText.ChainageToString(chainage, ChPrecision);

                // Insert block
                BlockReference bref = new BlockReference(point, blockId);
                bref.Rotation = Vector3d.YAxis.GetAngleTo(direction, Vector3d.ZAxis);
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
                            attRef.TextString = chainageString;
                        }
                        bref.AttributeCollection.AppendAttribute(attRef);
                        tr.AddNewlyCreatedDBObject(attRef, true);
                    }
                }

                tr.Commit();
            }
        }

        private bool ShowSettings()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            using (DrawAxesForm form = new DrawAxesForm())
            {
                IEnumerable<string> blockNames = AcadEntity.GetBlocks(db).Select(p => p.Key);
                IEnumerable<string> styleNames = AcadEntity.GetTextStyles(db).Select(p => p.Key);
                form.SetBlockNames(blockNames.ToArray());
                form.SetTextStyleNames(styleNames.ToArray());

                // Read settings
                form.DrawOnlyLine = (DrawingType == AxisDrawingType.Line);

                form.AxisLineLength = AxisLineLength;
                form.TextHeight = TextHeight;
                form.TextStyleName = TextStyleName;

                form.BlockName = BlockName;
                form.AxisAttribute = AxisAttribute;
                form.ChAttribute = ChAttribute;
                form.ChPrefix = ChPrefix;
                form.Precision = ChPrecision;

                form.UpdateUI();

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    DrawingType = form.DrawOnlyLine ? AxisDrawingType.Line : AxisDrawingType.Block;

                    AxisLineLength = form.AxisLineLength;
                    TextHeight = form.TextHeight;
                    TextStyleName = form.TextStyleName;

                    BlockName = form.BlockName;
                    AxisAttribute = form.AxisAttribute;
                    ChAttribute = form.ChAttribute;
                    ChPrefix = form.ChPrefix;
                    ChPrecision = form.Precision;

                    // Save changes
                    Properties.Settings.Default.Command_DRAWAXES_DrawOnlyLine = (DrawingType == AxisDrawingType.Line);

                    Properties.Settings.Default.Command_DRAWAXES_AxisLineLength = AxisLineLength;
                    Properties.Settings.Default.Command_DRAWAXES_TextHeight = TextHeight;
                    Properties.Settings.Default.Command_DRAWAXES_TextStyleName = TextStyleName;

                    Properties.Settings.Default.Command_DRAWAXES_BlockName = BlockName;
                    Properties.Settings.Default.Command_DRAWAXES_AxisAttribute = AxisAttribute;
                    Properties.Settings.Default.Command_DRAWAXES_ChAttribute = ChAttribute;
                    Properties.Settings.Default.Command_DRAWAXES_ChPrefix = ChPrefix;
                    Properties.Settings.Default.Command_DRAWAXES_Precision = ChPrecision;

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