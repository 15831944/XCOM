using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private bool DrawBOQ()
        {
            using (DrawBOQForm form = new DrawBOQForm())
            {
                // Pos error check
                RebarPos.PromptRebarSelectionResult sel = RebarPos.SelectAllPosUser();
                if (sel.Status != PromptStatus.OK) return false;
                ObjectId[] items = sel.Value.GetObjectIds();

                List<PosCheckResult> errors = PosCheckResult.CheckAllInSelection(items, true, false);
                List<PosCheckResult> warnings = PosCheckResult.CheckAllInSelection(items, false, true);

                if (errors.Count != 0) PosCheckResult.ConsoleOut(errors);
                if (warnings.Count != 0) PosCheckResult.ConsoleOut(warnings);

                if (errors.Count != 0)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.DisplayTextScreen = true;
                    return false;
                }

                // Pos similarity check
                if (warnings.Count != 0)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.DisplayTextScreen = true;
                    PromptKeywordOptions opts = new PromptKeywordOptions("\nMetraja devam edilsin mi? [Evet/Hayir]", "Yes No");
                    PromptResult res = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetKeywords(opts);
                    if (res.Status != PromptStatus.OK || res.StringResult == "No")
                    {
                        return true;
                    }
                }

                List<PosCopy> posList = new List<PosCopy>();
                try
                {
                    posList = PosCopy.ReadAllInSelection(items, true, PosCopy.PosGrouping.PosMarker);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (posList.Count == 0)
                {
                    MessageBox.Show("Seçilen grupta poz mevcut değil.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if (!form.Init(posList[0].scale * 25.0))
                    return false;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) != System.Windows.Forms.DialogResult.OK)
                    return true;

                posList = RemoveEmpty(posList);
                if (!form.HideMissing)
                {
                    posList = AddMissing(posList);
                }
                posList = SortList(posList);

                PromptPointResult result = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("Baz noktası: ");
                if (result.Status != PromptStatus.OK)
                    return true;
                Point3d ptBaseWCS = result.Value.TransformBy(AcadUtility.AcadGraphics.UcsToWcs);
                Point3d ptBase = Point3d.Origin;

                // Diameter list
                HashSet<string> diameterList = new HashSet<string>();
                foreach (PosCopy copy in posList)
                {
                    int dia = 0;
                    if (int.TryParse(copy.diameter, out dia) && !diameterList.Contains(copy.diameter)) diameterList.Add(copy.diameter);
                }
                if (!form.HideUnusedDiameters)
                {
                    diameterList.UnionWith(PosGroup.Current.StandardDiameters.Select(p => p.ToString()));
                }
                List<string> diameters = diameterList.ToList();
                diameters.Sort((a, b) => (int.Parse(a) < int.Parse(b) ? -1 : 1));
                List<double> diameterWidths = new List<double>();
                for (int i = 0; i < diameters.Count; i++)
                {
                    diameterWidths.Add(diameters.Count == 1 ? 14.0 : diameters.Count == 2 ? 6.75 : 5.0);
                }
                double diameterTotalWidth = diameterWidths.Sum();

                // Table size
                double tableWidth = 20 + (form.DrawShapes ? 19 : 0) + diameterTotalWidth;

                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    try
                    {
                        // Text styles
                        ObjectId headerStyleId = AcadUtility.AcadEntity.GetOrCreateTextStyle(db, "BOQ_Header", "Arial", 1.0);
                        ObjectId cellStyleId = AcadUtility.AcadEntity.GetOrCreateTextStyle(db, "BOQ_Cell", "romans.shx", 0.7);
                        ObjectId diameterStyleId = AcadUtility.AcadEntity.GetOrCreateTextStyle(db, "BOQ_Diameter", "twdin.shx", 0.7);

                        // Layers
                        ObjectId headerLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "S-METRAJ_YAZI3", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 1));
                        ObjectId textLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "S-METRAJ_YAZI1", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 1));
                        ObjectId lineLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "S-METRAJ_CIZGI1", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 4));
                        ObjectId innerLineLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "S-METRAJ_CIZGI2", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 3));
                        ObjectId shapeLineLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "S-METRAJ_CIZGI3", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 4));
                        ObjectId shapeTextLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "S-METRAJ_YAZI2", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 1));

                        // Draw table
                        BOQLanguage lang = form.EffectiveLanguage;
                        Point3d pt = ptBase;
                        List<Entity> entities = new List<Entity>();

                        // Increase header row height if the diameter by length label does not fit in its cell
                        // or the rebar length column label  does not fit in its cell
                        bool largeHeader = false;
                        double additionalHeaderHeight = 0;
                        using (MText diaByLengthText = AcadUtility.AcadEntity.CreateMText(db, Point3d.Origin, lang.LengthByDiameterColumnLabel, 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId))
                        using (MText rebarLengthText = AcadUtility.AcadEntity.CreateMText(db, Point3d.Origin, lang.RebarLengthColumnLabel, 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId))
                        {
                            if (diaByLengthText.ActualWidth > diameterTotalWidth || rebarLengthText.ActualHeight > 5.075)
                            {
                                largeHeader = true;
                                additionalHeaderHeight = 3.0;
                            }
                        }

                        // Header
                        if (!string.IsNullOrEmpty(form.TableHeader))
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, tableWidth / 2.0), form.TableHeader, 1.5, 0, AttachmentPoint.BottomCenter, headerStyleId, headerLayerId));
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        }
                        // Multiplier
                        if (form.Multiplier > 1)
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, pt, lang.MultiplierHeader, 1.0, 0, AttachmentPoint.BottomLeft, cellStyleId, textLayerId));
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.2);
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, additionalHeaderHeight / 2.0);
                        // Column headers
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 2), lang.PosColumnLabel, 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 6), lang.DiameterColumnLabel, 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 11), lang.CountColumnLabel, 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 17), lang.RebarLengthColumnLabel, 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                        double colOffset = 0;
                        if (form.DrawShapes)
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 29.5), lang.ShapeColumnLabel, 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                            colOffset = 19;
                        }
                        string diastr = lang.LengthByDiameterColumnLabel;
                        if (largeHeader) diastr = diastr.Replace(BOQLanguage.ColumnSeparator, BOQLanguage.ParagraphSeparator);
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth / 2.0), 0.5 * Math.PI, 1.25), diastr, 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 1.2875 + additionalHeaderHeight / 2.0);
                        double diaOffset = 0;
                        int j = 0;
                        foreach (string dia in diameters)
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diaOffset + diameterWidths[j] / 2.0), RebarPos.DiameterSymbol + dia, 1.0, 0, AttachmentPoint.MiddleCenter, diameterStyleId, textLayerId));
                            diaOffset += diameterWidths[j];
                            j++;
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 2.9375);

                        double lengthScale = PosGroup.ConvertLength(1.0, PosGroup.Current.DrawingUnit, PosGroup.Current.DisplayUnit);

                        // Add rows
                        foreach (PosCopy copy in posList)
                        {
                            if (copy.existing)
                            {
                                string a = string.Empty;
                                string b = string.Empty;
                                string c = string.Empty;
                                string d = string.Empty;
                                string e = string.Empty;
                                string f = string.Empty;

                                // Piece lengths in display units
                                if (copy.isVarA)
                                    a = (copy.minA * lengthScale).ToString("F0") + "~" + (copy.maxA * lengthScale).ToString("F0");
                                else
                                    a = (copy.minA * lengthScale).ToString("F0");
                                if (copy.isVarB)
                                    b = (copy.minB * lengthScale).ToString("F0") + "~" + (copy.maxB * lengthScale).ToString("F0");
                                else
                                    b = (copy.minB * lengthScale).ToString("F0");
                                if (copy.isVarC)
                                    c = (copy.minC * lengthScale).ToString("F0") + "~" + (copy.maxC * lengthScale).ToString("F0");
                                else
                                    c = (copy.minC * lengthScale).ToString("F0");
                                if (copy.isVarD)
                                    d = (copy.minD * lengthScale).ToString("F0") + "~" + (copy.maxD * lengthScale).ToString("F0");
                                else
                                    d = (copy.minD * lengthScale).ToString("F0");
                                if (copy.isVarE)
                                    e = (copy.minE * lengthScale).ToString("F0") + "~" + (copy.maxE * lengthScale).ToString("F0");
                                else
                                    e = (copy.minE * lengthScale).ToString("F0");
                                if (copy.isVarF)
                                    f = (copy.minF * lengthScale).ToString("F0") + "~" + (copy.maxF * lengthScale).ToString("F0");
                                else
                                    f = (copy.minF * lengthScale).ToString("F0");

                                // Average length in drawing units
                                double aveLength = (copy.length1 + copy.length2) / 2.0;
                                // Length in display units
                                string length = (aveLength * lengthScale).ToString("F0");
                                // Total length in M
                                string totalLengthM = PosGroup.ConvertLength(((double)copy.count) * aveLength, PosGroup.Current.DrawingUnit, PosGroup.DrawingUnits.Meter).ToString("F" + form.Precision);

                                entities.Add(AcadUtility.AcadEntity.CreateText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 2), copy.pos, 1.0, 0, 0.7, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, cellStyleId, textLayerId));
                                entities.Add(AcadUtility.AcadEntity.CreateText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 6), copy.diameter, 1.0, 0, 0.7, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, cellStyleId, textLayerId));
                                entities.Add(AcadUtility.AcadEntity.CreateText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 11), copy.count.ToString(), 1.0, 0, 0.7, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, cellStyleId, textLayerId));
                                entities.Add(AcadUtility.AcadEntity.CreateText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 17), length, 1.0, 0, 0.7, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, cellStyleId, textLayerId));
                                int diaIndex = diameters.IndexOf(copy.diameter);
                                double off = 0;
                                for (int i = 0; i < diaIndex; i++)
                                {
                                    off += diameterWidths[i];
                                }
                                off += diameterWidths[diaIndex] / 2.0;
                                entities.Add(AcadUtility.AcadEntity.CreateText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + off), totalLengthM, 1.0, 0, 0.7, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, cellStyleId, textLayerId));

                                // Draw the shape
                                if (form.DrawShapes)
                                {
                                    PosShape shape = PosShape.Shapes[copy.shapename];
                                    Point3d position = AcadUtility.AcadGeometry.Polar(AcadUtility.AcadGeometry.Polar(pt, 0, 20), 1.5 * Math.PI, 1.5);

                                    shape.SetShapeTexts(a, b, c, d, e, f);
                                    entities.AddRange(shape.ToEntitites(db, position, 3.0, 0, false, shapeLineLayerId, shapeTextLayerId));
                                    shape.ClearShapeTexts();
                                }
                            }
                            else if (!form.HideMissing)
                            {
                                entities.Add(AcadUtility.AcadEntity.CreateText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 2), copy.pos, 1.0, 0, 0.7, TextHorizontalMode.TextCenter, TextVerticalMode.TextVerticalMid, cellStyleId, textLayerId));
                            }

                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 0.25);

                        // Footer
                        // Total length by diameter
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 1), lang.TotalLengthByDiameterRowLabel, 1.0, 0, AttachmentPoint.MiddleLeft, cellStyleId, textLayerId));
                        diaOffset = 0;
                        j = 0;
                        foreach (string dia in diameters)
                        {
                            double sum = posList.Sum(p => (p.diameter == dia ? PosGroup.ConvertLength(((double)p.count) * (p.length1 + p.length2) / 2.0, PosGroup.Current.DrawingUnit, PosGroup.DrawingUnits.Meter) : 0));
                            if (sum > double.Epsilon)
                            {
                                entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diaOffset + diameterWidths[j] / 2.0), sum.ToString("F" + form.Precision), 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                            }
                            diaOffset += diameterWidths[j];
                            j++;
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        // Unit weight by diameter
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 1), lang.UnitWeightRowLabel, 1.0, 0, AttachmentPoint.MiddleLeft, cellStyleId, textLayerId));
                        diaOffset = 0;
                        j = 0;
                        foreach (string dia in diameters)
                        {
                            double d = double.Parse(dia);
                            double uw = (d * d * Math.PI / 4 * 7850.0 * 0.000001);
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diaOffset + diameterWidths[j] / 2.0), uw.ToString("F3"), 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                            diaOffset += diameterWidths[j];
                            j++;
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        // Weight by diameter
                        double totalWeight = 0;
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 1), lang.TotalWeightByDiameterRowLabel, 1.0, 0, AttachmentPoint.MiddleLeft, cellStyleId, textLayerId));
                        if (form.Multiplier > 1)
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset - 1), lang.MultiplierBy1Label, 1.0, 0, AttachmentPoint.MiddleRight, cellStyleId, textLayerId));
                        }
                        diaOffset = 0;
                        j = 0;
                        foreach (string dia in diameters)
                        {
                            double d = double.Parse(dia);
                            double uw = (d * d * Math.PI / 4 * 7850.0 * 0.000001);
                            double sum = posList.Sum(p => (p.diameter == dia ? PosGroup.ConvertLength(((double)p.count) * (p.length1 + p.length2) / 2.0, PosGroup.Current.DrawingUnit, PosGroup.DrawingUnits.Meter) : 0));
                            if (sum > double.Epsilon)
                            {
                                sum *= uw;
                                totalWeight += sum;
                                entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diaOffset + diameterWidths[j] / 2.0), sum.ToString("F" + form.Precision), 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                            }
                            diaOffset += diameterWidths[j];
                            j++;
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        // Total weight
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 1), lang.TotalWeightRowLabel, 1.0, 0, AttachmentPoint.MiddleLeft, cellStyleId, textLayerId));
                        if (form.Multiplier > 1)
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset - 1), lang.MultiplierBy1Label, 1.0, 0, AttachmentPoint.MiddleRight, cellStyleId, textLayerId));
                        }
                        entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth / 2.0), totalWeight.ToString("F" + form.Precision), 1.2, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        // Gross totals
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 0.25);
                        if (form.Multiplier > 1)
                        {
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 0.25);
                            // Gross weight by diameter
                            totalWeight = 0;
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 1), lang.GrossTotalWeightByDiameterRowLabel, 1.0, 0, AttachmentPoint.MiddleLeft, cellStyleId, textLayerId));
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset - 1), lang.MultiplierByNLabel, 1.0, 0, AttachmentPoint.MiddleRight, cellStyleId, textLayerId));
                            diaOffset = 0;
                            j = 0;
                            foreach (string dia in diameters)
                            {
                                double d = double.Parse(dia);
                                double uw = (d * d * Math.PI / 4 * 7850.0 * 0.000001);
                                double sum = posList.Sum(p => (p.diameter == dia ? PosGroup.ConvertLength(((double)p.count) * (p.length1 + p.length2) / 2.0, PosGroup.Current.DrawingUnit, PosGroup.DrawingUnits.Meter) : 0));
                                if (sum > double.Epsilon)
                                {
                                    sum *= uw * (double)form.Multiplier;
                                    totalWeight += sum;
                                    entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diaOffset + diameterWidths[j] / 2.0), sum.ToString("F" + form.Precision), 1.0, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                                }
                                diaOffset += diameterWidths[j];
                                j++;
                            }
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                            // Gross total weight
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 1), lang.GrossTotalWeightRowLabel, 1.0, 0, AttachmentPoint.MiddleLeft, cellStyleId, textLayerId));
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset - 1), lang.MultiplierByNLabel, 1.0, 0, AttachmentPoint.MiddleRight, cellStyleId, textLayerId));
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth / 2.0), totalWeight.ToString("F" + form.Precision), 1.2, 0, AttachmentPoint.MiddleCenter, cellStyleId, textLayerId));
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 4.0);
                        }
                        // Footer
                        if (!string.IsNullOrEmpty(form.TableFooter))
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateMText(db, pt, form.TableFooter, 1.2, 0, AttachmentPoint.MiddleLeft, headerStyleId, headerLayerId));
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.6);
                        }

                        // Lines
                        pt = ptBase;
                        if (!string.IsNullOrEmpty(form.TableHeader))
                        {
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        }
                        // Multiplier
                        if (form.Multiplier > 1)
                        {
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.2);
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 0.5 * Math.PI, 2.6);
                        // Column headers
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 2.7 + additionalHeaderHeight);
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset), AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 2.5);
                        entities.AddRange(AcadUtility.AcadEntity.CreateMLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), 0.25, lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.125);
                        // Pos lines
                        for (int i = 0; i < posList.Count - 1; i++)
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), innerLineLayerId));
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 0.125);
                        // Total rows
                        entities.AddRange(AcadUtility.AcadEntity.CreateMLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), 0.25, lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.125);
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), lineLayerId));
                        if (form.Multiplier > 1)
                        {
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.125);
                            entities.AddRange(AcadUtility.AcadEntity.CreateMLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), 0.25, lineLayerId));
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.125);
                            entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), lineLayerId));
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                            entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), lineLayerId));
                        }
                        else
                        {
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                            entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 0, 20 + colOffset + diameterTotalWidth), lineLayerId));
                        }

                        // Vertical lines
                        double totalHeight = 17.575 + additionalHeaderHeight + posList.Count * 3.0;
                        if (form.Multiplier > 1) totalHeight += 6.25;
                        double heightToTotalRow = 5.325 + additionalHeaderHeight + posList.Count * 3.0;
                        pt = ptBase;
                        if (!string.IsNullOrEmpty(form.TableHeader))
                        {
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.0);
                        }
                        // Multiplier
                        if (form.Multiplier > 1)
                        {
                            pt = AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 3.2);
                        }
                        pt = AcadUtility.AcadGeometry.Polar(pt, 0.5 * Math.PI, 2.6);
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, totalHeight), lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 0, 4);
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, heightToTotalRow), lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 0, 4);
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, heightToTotalRow), lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 0, 6);
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, heightToTotalRow), lineLayerId));
                        pt = AcadUtility.AcadGeometry.Polar(pt, 0, 6.0);
                        if (form.DrawShapes)
                        {
                            entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, heightToTotalRow), lineLayerId));
                            pt = AcadUtility.AcadGeometry.Polar(pt, 0, colOffset);
                        }
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, totalHeight), lineLayerId));
                        double diaLinesHeight = 2.625 + posList.Count * 3.0 + 9.25;
                        for (int i = 0; i < diameters.Count; i++)
                        {
                            pt = AcadUtility.AcadGeometry.Polar(pt, 0, diameterWidths[i]);
                            if (i < diameters.Count - 1)
                            {
                                entities.Add(AcadUtility.AcadEntity.CreateLine(db, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 2.7 + additionalHeaderHeight), AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 2.7 + additionalHeaderHeight + diaLinesHeight), lineLayerId));
                                if (form.Multiplier > 1)
                                {
                                    entities.Add(AcadUtility.AcadEntity.CreateLine(db, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 2.7 + additionalHeaderHeight + diaLinesHeight + 3.25), AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, 2.7 + additionalHeaderHeight + diaLinesHeight + 3.25 + 3.0), lineLayerId));
                                }
                            }
                        }
                        entities.Add(AcadUtility.AcadEntity.CreateLine(db, pt, AcadUtility.AcadGeometry.Polar(pt, 1.5 * Math.PI, totalHeight), lineLayerId));

                        // Create all entitites
                        Matrix3d trans = Matrix3d.Identity;
                        trans = trans.PreMultiplyBy(Matrix3d.Displacement(ptBaseWCS - Point3d.Origin));
                        trans = trans.PreMultiplyBy(Matrix3d.Scaling(form.TextHeight, ptBaseWCS));
                        Vector3d ucsx = Vector3d.XAxis.TransformBy(AcadUtility.AcadGraphics.UcsToWcs);
                        trans = trans.PreMultiplyBy(Matrix3d.Rotation(ucsx.GetAngleTo(Vector3d.XAxis), Vector3d.ZAxis, ptBaseWCS));

                        // Add table to db
                        foreach (Entity en in entities)
                        {
                            en.TransformBy(trans);
                            btr.AppendEntity(en);
                            tr.AddNewlyCreatedDBObject(en, true);
                            if (en.Id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(MText)).UnmanagedObject)
                            {
                                MText text = (MText)en;
                                text.Direction = ucsx;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    tr.Commit();
                }
            }

            return true;
        }



        private List<PosCopy> AddMissing(List<PosCopy> list)
        {
            list = RemoveEmpty(list);

            int lastpos = 0;
            foreach (PosCopy copy in list)
            {
                int posno;
                if (int.TryParse(copy.pos, out posno))
                {
                    lastpos = Math.Max(lastpos, posno);
                }
            }
            for (int i = 1; i <= lastpos; i++)
            {
                if (!list.Exists(p => p.pos == i.ToString()))
                {
                    PosCopy copy = new PosCopy();
                    copy.pos = i.ToString();
                    list.Add(copy);
                }
            }

            return SortList(list);
        }

        private List<PosCopy> RemoveEmpty(List<PosCopy> list)
        {
            list.RemoveAll(p => p.existing == false);
            return list;
        }

        private List<PosCopy> SortList(List<PosCopy> list)
        {
            list.Sort(new CompareByPosNumber());
            return list;
        }

        private class CompareByPosNumber : IComparer<PosCopy>
        {
            public int Compare(PosCopy e1, PosCopy e2)
            {
                int p1 = 0;
                int p2 = 0;
                int.TryParse(e1.pos, out p1);
                int.TryParse(e2.pos, out p2);

                return (p1 == p2 ? 0 : (p1 < p2 ? -1 : 1));
            }
        }
    }
}
