using AcadUtility;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Windows.Forms;

namespace XCOM.Commands.Bridge
{
    public class Command_DRAWDECK
    {
        private Bridge.AlignmentType Alignment { get; set; }
        private ObjectId CenterlineId { get; set; }

        private Point3d StartPoint { get; set; }
        private Point3d EndPoint { get; set; }
        private double OverhangDistance { get; set; } = 0.5;

        private double DeckWidth { get; set; } = 10;

        private double AsphaltThickness { get; set; } = 0.06;
        private double DeckThickness { get; set; } = 0.25;
        private double SidewalkThickness { get; set; } = 0.25;

        private static readonly string DeckLayerName = "K_DOSEME";
        private static readonly string SidewalkLayerName = "K_KALDIRIM";
        private static readonly string HatchLayerName = "K_TARAMA";
        private static readonly string HatchPattern = "SOLID";
        private static readonly double HatchScale = 1;

        [Autodesk.AutoCAD.Runtime.CommandMethod("DRAWDECK")]
        public void DrawDeck()
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

            Alignment = Bridge.PickAlignment();
            if (Alignment == Bridge.AlignmentType.None)
            {
                return;
            }

            PromptEntityOptions entityOpts = new PromptEntityOptions("\nEksen: ");
            entityOpts.SetRejectMessage("\nSelect a curve.");
            entityOpts.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult entityRes = ed.GetEntity(entityOpts);
            if (entityRes.Status != PromptStatus.OK)
            {
                return;
            }
            CenterlineId = entityRes.ObjectId;

            var resp1 = ed.GetPoint("\nBaşlangıç noktası: ");
            if (resp1.Status != PromptStatus.OK)
            {
                return;
            }
            StartPoint = resp1.Value.TransformBy(ucs2wcs);

            var resp2 = ed.GetPoint("\nBitiş noktası: ");
            if (resp2.Status != PromptStatus.OK)
            {
                return;
            }
            EndPoint = resp2.Value.TransformBy(ucs2wcs);

            var opts = new PromptDistanceOptions("\nAsktan taşma mesafesi: ");
            opts.AllowNegative = false;
            opts.AllowZero = false;
            opts.DefaultValue = OverhangDistance;
            opts.UseDefaultValue = true;
            opts.BasePoint = resp1.Value;
            var res = ed.GetDistance(opts);
            if (res.Status != PromptStatus.OK)
            {
                return;
            }
            OverhangDistance = res.Value;

            if (Alignment == Bridge.AlignmentType.Plan)
            {
                var opts1 = new PromptDistanceOptions("\nTabliye genişliği: ");
                opts1.AllowNegative = false;
                opts1.AllowZero = false;
                opts1.DefaultValue = DeckWidth;
                opts1.UseDefaultValue = true;
                opts1.BasePoint = resp1.Value;
                var reso1 = ed.GetDistance(opts1);
                if (reso1.Status != PromptStatus.OK)
                {
                    return;
                }
                DeckWidth = reso1.Value;
            }
            else
            {
                var opts1 = new PromptDistanceOptions("\nAsfalt kalınlığı: ");
                opts1.AllowNegative = false;
                opts1.AllowZero = false;
                opts1.DefaultValue = AsphaltThickness;
                opts1.UseDefaultValue = true;
                var reso1 = ed.GetDistance(opts1);
                if (reso1.Status != PromptStatus.OK)
                {
                    return;
                }
                AsphaltThickness = reso1.Value;

                var opts2 = new PromptDistanceOptions("\nTabliye kalınlığı: ");
                opts2.AllowNegative = false;
                opts2.AllowZero = false;
                opts2.DefaultValue = DeckThickness;
                opts2.UseDefaultValue = true;
                var reso2 = ed.GetDistance(opts2);
                if (reso2.Status != PromptStatus.OK)
                {
                    return;
                }
                DeckThickness = reso2.Value;

                var opts3 = new PromptDistanceOptions("\nKaldırım kalınlığı: ");
                opts3.AllowNegative = false;
                opts3.AllowZero = false;
                opts3.DefaultValue = SidewalkThickness;
                opts3.UseDefaultValue = true;
                var reso3 = ed.GetDistance(opts3);
                if (reso3.Status != PromptStatus.OK)
                {
                    return;
                }
                SidewalkThickness = reso3.Value;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                try
                {
                    ObjectId lineLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, DeckLayerName, Color.FromColorIndex(ColorMethod.ByAci, 4));
                    ObjectId hatchLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, HatchLayerName, Color.FromColorIndex(ColorMethod.ByAci, 31));
                    ObjectId sidewalkLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, SidewalkLayerName, Color.FromColorIndex(ColorMethod.ByAci, 21));

                    // Adjust start and end point to account for overhang
                    Curve centerline = tr.GetObject(CenterlineId, OpenMode.ForRead) as Curve;
                    if (Alignment == Bridge.AlignmentType.Plan)
                    {
                        StartPoint = centerline.GetClosestPointTo(StartPoint, false);
                        double startDistance = centerline.GetDistAtPoint(StartPoint) - OverhangDistance;
                        StartPoint = centerline.GetPointAtDist(startDistance);

                        EndPoint = centerline.GetClosestPointTo(EndPoint, false);
                        double endDistance = centerline.GetDistAtPoint(EndPoint) + OverhangDistance;
                        EndPoint = centerline.GetPointAtDist(endDistance);

                        using (Plane horizontal = new Plane(Point3d.Origin, Vector3d.ZAxis))
                        {
                            Curve planCurve = centerline.GetOrthoProjectedCurve(horizontal);
                            Vector3d dir = planCurve.GetFirstDerivative(planCurve.StartParam).CrossProduct(Vector3d.ZAxis);
                            dir /= dir.Length;

                            var rightCurve = planCurve.GetOffsetCurves(planCurve.StartPoint + dir * DeckWidth / 2)[0] as Curve;
                            rightCurve = rightCurve.GetTrimmedCurve(StartPoint, EndPoint, true);

                            var leftCurve = planCurve.GetOffsetCurves(planCurve.StartPoint - dir * DeckWidth / 2)[0] as Curve;
                            leftCurve = leftCurve.GetTrimmedCurve(StartPoint, EndPoint, true);

                            // Join curves and close ends with lines
                            var finalCurve = AcadEntity.CreatePolyLine(db, true,
                                rightCurve,
                                AcadEntity.CreateLine(db, rightCurve.StartPoint, leftCurve.StartPoint),
                                leftCurve,
                                AcadEntity.CreateLine(db, rightCurve.EndPoint, leftCurve.EndPoint));
                            var finalCurveId = btr.AppendEntity(finalCurve);
                            tr.AddNewlyCreatedDBObject(finalCurve, true);
                            finalCurve.LayerId = lineLayerId;

                            // Hatch inside
                            var hatch = new Hatch();
                            btr.AppendEntity(hatch);
                            tr.AddNewlyCreatedDBObject(hatch, true);
                            hatch.LayerId = hatchLayerId;
                            hatch.Associative = true;
                            hatch.AppendLoop(HatchLoopTypes.Outermost, new ObjectIdCollection() { finalCurveId });
                            hatch.PatternScale = HatchScale;
                            hatch.SetHatchPattern(HatchPatternType.PreDefined, HatchPattern);
                            hatch.EvaluateHatch(true);
                        }
                    }
                    else
                    {
                        Vector3d upDir = db.Ucsydir;
                        var topline = centerline.GetTransformedCopy(Matrix3d.Displacement(upDir * -AsphaltThickness)) as Curve;
                        var bottomline = topline.GetTransformedCopy(Matrix3d.Displacement(upDir * -DeckThickness)) as Curve;
                        var sidewalkline = topline.GetTransformedCopy(Matrix3d.Displacement(upDir * SidewalkThickness)) as Curve;

                        using (Plane horizontal = new Plane(Point3d.Origin, upDir))
                        {
                            Curve planCurve = centerline.GetOrthoProjectedCurve(horizontal);

                            Point3d startPointOnPlan = planCurve.GetClosestPointTo(StartPoint, true);
                            double startDistance = planCurve.GetDistAtPoint(startPointOnPlan) - OverhangDistance;
                            startPointOnPlan = planCurve.GetPointAtDist(startDistance);
                            StartPoint = centerline.GetClosestPointTo(startPointOnPlan, upDir, true);

                            Point3d endPointOnPlan = planCurve.GetClosestPointTo(EndPoint, true);
                            double endDistance = planCurve.GetDistAtPoint(endPointOnPlan) + OverhangDistance;
                            endPointOnPlan = planCurve.GetPointAtDist(endDistance);
                            EndPoint = centerline.GetClosestPointTo(endPointOnPlan, upDir, true);
                        }

                        topline = topline.GetTrimmedCurve(topline.GetClosestPointTo(StartPoint, upDir, true),
                            topline.GetClosestPointTo(EndPoint, upDir, true), true);
                        bottomline = bottomline.GetTrimmedCurve(bottomline.GetClosestPointTo(StartPoint, upDir, true),
                            bottomline.GetClosestPointTo(EndPoint, upDir, true), true);
                        sidewalkline = sidewalkline.GetTrimmedCurve(sidewalkline.GetClosestPointTo(StartPoint, upDir, true),
                            sidewalkline.GetClosestPointTo(EndPoint, upDir, true), true);

                        // Sidewalk
                        var finalSWCurve = AcadEntity.CreatePolyLine(db, false,
                            AcadEntity.CreateLine(db, topline.StartPoint, sidewalkline.StartPoint),
                            sidewalkline,
                            AcadEntity.CreateLine(db, topline.EndPoint, sidewalkline.EndPoint));
                        btr.AppendEntity(finalSWCurve);
                        tr.AddNewlyCreatedDBObject(finalSWCurve, true);
                        finalSWCurve.LayerId = sidewalkLayerId;

                        // Deck polyline
                        var finalCurve = AcadEntity.CreatePolyLine(db, true,
                            topline,
                            AcadEntity.CreateLine(db, topline.StartPoint, bottomline.StartPoint),
                            bottomline,
                            AcadEntity.CreateLine(db, topline.EndPoint, bottomline.EndPoint));
                        ObjectId finalCurveId = btr.AppendEntity(finalCurve);
                        tr.AddNewlyCreatedDBObject(finalCurve, true);
                        finalCurve.LayerId = lineLayerId;

                        // Hatch inside
                        var hatch = new Hatch();
                        btr.AppendEntity(hatch);
                        tr.AddNewlyCreatedDBObject(hatch, true);
                        hatch.LayerId = hatchLayerId;
                        hatch.Associative = true;
                        hatch.AppendLoop(HatchLoopTypes.Outermost, new ObjectIdCollection() { finalCurveId });
                        hatch.PatternScale = HatchScale;
                        hatch.SetHatchPattern(HatchPatternType.PreDefined, HatchPattern);
                        hatch.EvaluateHatch(true);
                    }
                }
                catch (System.Exception e)
                {
                    MessageBox.Show(e.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }
    }
}