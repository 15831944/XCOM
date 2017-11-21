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
    public class Command_PROFILEONCURVE
    {
        private double ProfileGridH { get; set; }
        private double ProfileGridV { get; set; }
        private double ProfileVScale { get; set; }
        private double TextHeight { get; set; }
        private int Precision { get; set; }
        private string TextStyleName { get; set; }

        public Command_PROFILEONCURVE()
        {
            ProfileGridH = 10;
            ProfileGridV = 5;
            ProfileVScale = 1;
            TextHeight = 1;
            Precision = 2;
            TextStyleName = "Standard";
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("PROFILEONCURVE")]
        public void ProfileOnCurve()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = Topography.PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!Topography.EnsureSurfaceNotEmpty(surface)) return;

            // Pick alignment
            bool flag = true;
            ObjectId curveId = ObjectId.Null;
            while (flag)
            {
                PromptEntityOptions entityOpts = new PromptEntityOptions("\nEksen [Seçenekler]: ", "Settings");
                entityOpts.SetRejectMessage("\nSelect a curve.");
                entityOpts.AddAllowedClass(typeof(Curve), false);
                PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);
                if (entityRes.Status == PromptStatus.Keyword)
                {
                    ShowSettings();
                }
                else if (entityRes.Status == PromptStatus.OK)
                {
                    curveId = entityRes.ObjectId;
                    break;
                }
                else if (entityRes.Status == PromptStatus.Cancel)
                {
                    return;
                }
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;

                ObjectId textStyleId = ObjectId.Null;
                using (TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
                {
                    if (tt.Has(TextStyleName))
                    {
                        textStyleId = tt[TextStyleName];
                    }
                }

                // Project curve onto surface
                Topography topo = Topography.Instance;
                Curve curve = tr.GetObject(curveId, OpenMode.ForRead) as Curve;
                Point2dCollection points = topo.ProfileOnCurve(curve, surface);

                // Base point for profile drawing
                PromptPointResult pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nProfil başlangıcı: ");
                if (pointRes.Status != PromptStatus.OK)
                {
                    return;
                }
                Point3d basePt = pointRes.Value;

                if (points.Count > 0)
                {
                    // Limits
                    Extents2d ex = AcadUtility.AcadGeometry.Limits(points);

                    // Base level for profile drawing
                    PromptDoubleOptions levelOpts = new PromptDoubleOptions("\nProfil baz kotu: ");
                    levelOpts.DefaultValue = Math.Floor(ex.MinPoint.Y / ProfileGridV) * ProfileGridV;
                    levelOpts.UseDefaultValue = true;
                    PromptDoubleResult levelRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(levelOpts);
                    if (pointRes.Status != PromptStatus.OK)
                    {
                        return;
                    }
                    double startLevel = levelRes.Value;
                    double endLevel = Math.Ceiling(ex.MaxPoint.Y / ProfileGridV + 1) * ProfileGridV;

                    // Base chainage for profile drawing
                    double startCh = 0;
                    flag = true;
                    while (flag)
                    {
                        PromptStringOptions chOpts = new PromptStringOptions("\nProfil baz KM: ");
                        chOpts.DefaultValue = AcadUtility.AcadText.ChainageToString(0, Precision);
                        chOpts.UseDefaultValue = true;
                        PromptResult chRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetString(chOpts);
                        if (chRes.Status != PromptStatus.OK)
                        {
                            return;
                        }
                        if (AcadUtility.AcadText.TryChainageFromString(chRes.StringResult, out startCh))
                        {
                            break;
                        }
                    }
                    double endCh = Math.Ceiling((startCh + ex.MaxPoint.X) / ProfileGridH) * ProfileGridH;

                    // Draw grid
                    IEnumerable<Entity> entities = DrawProfileFrame(db, basePt, startCh, startLevel, endCh, endLevel, ProfileGridH, ProfileGridV, ProfileVScale, TextHeight, Precision, textStyleId);
                    foreach (Entity ent in entities)
                    {
                        ent.TransformBy(ucs2wcs);
                        btr.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                    }

                    // Draw profile
                    ObjectId profileLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "Profil_Eksen", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 5));
                    Point2dCollection trPoints = new Point2dCollection(points.Count);
                    foreach (Point2d pt in points)
                    {
                        trPoints.Add(new Point2d(basePt.X + pt.X, basePt.Y + (pt.Y - startLevel) * ProfileVScale));
                    }
                    Polyline pline = AcadUtility.AcadEntity.CreatePolyLine(db, false, trPoints);
                    pline.TransformBy(ucs2wcs);
                    pline.LayerId = profileLayerId;
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);
                }

                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("MULTIPLEPROFILEONCURVE")]
        public void MultipleProfileOnCurve()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            Topography.SurfaceType surface = Topography.PickSurface();
            if (surface == Topography.SurfaceType.None) return;
            if (!Topography.EnsureSurfaceNotEmpty(surface)) return;

            // Pick center alignment
            bool flag = true;
            ObjectId curveId = ObjectId.Null;
            while (flag)
            {
                PromptEntityOptions entityOpts = new PromptEntityOptions("\nAna eksen [Seçenekler]: ", "Settings");
                entityOpts.SetRejectMessage("\nSelect a curve.");
                entityOpts.AddAllowedClass(typeof(Curve), false);
                PromptEntityResult entityRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(entityOpts);
                if (entityRes.Status == PromptStatus.Keyword)
                {
                    ShowSettings();
                }
                else if (entityRes.Status == PromptStatus.OK)
                {
                    curveId = entityRes.ObjectId;
                    break;
                }
                else if (entityRes.Status == PromptStatus.Cancel)
                {
                    return;
                }
            }

            // Pick other alignments
            IEnumerable<ObjectId> curveIds = SelectEntititesProfile();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;

                ObjectId textStyleId = ObjectId.Null;
                using (TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead))
                {
                    if (tt.Has(TextStyleName))
                    {
                        textStyleId = tt[TextStyleName];
                    }
                }

                // Project curve onto surface
                Topography topo = Topography.Instance;
                Curve center = tr.GetObject(curveId, OpenMode.ForRead) as Curve;
                Point2dCollection points = topo.ProfileOnCurve(center, surface);

                // Project other curves
                Dictionary<Entity, Point2dCollection> otherPoints = new Dictionary<Entity, Point2dCollection>();
                foreach (ObjectId id in curveIds)
                {
                    Curve curve = tr.GetObject(id, OpenMode.ForRead) as Curve;
                    Point2dCollection spoints = topo.ProfileOnCurve(curve, center, surface);
                    otherPoints.Add(curve, spoints);
                }

                // Base point for profile drawing
                PromptPointResult pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nProfil başlangıcı: ");
                if (pointRes.Status != PromptStatus.OK)
                {
                    return;
                }
                Point3d basePt = pointRes.Value;

                if (points.Count > 0)
                {
                    // Limits
                    Extents2d ex = AcadUtility.AcadGeometry.Limits(points);
                    foreach (Point2dCollection pts in otherPoints.Values)
                    {
                        ex = AcadUtility.AcadGeometry.Limits(ex, pts);
                    }

                    // Base level for profile drawing
                    PromptDoubleOptions levelOpts = new PromptDoubleOptions("\nProfil baz kotu: ");
                    levelOpts.DefaultValue = Math.Floor(ex.MinPoint.Y / ProfileGridV) * ProfileGridV;
                    levelOpts.UseDefaultValue = true;
                    PromptDoubleResult levelRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(levelOpts);
                    if (pointRes.Status != PromptStatus.OK)
                    {
                        return;
                    }
                    double startLevel = levelRes.Value;
                    double endLevel = Math.Ceiling(ex.MaxPoint.Y / ProfileGridV + 1) * ProfileGridV;

                    // Base chainage for profile drawing
                    double startCh = 0;
                    flag = true;
                    while (flag)
                    {
                        PromptStringOptions chOpts = new PromptStringOptions("\nProfil baz KM: ");
                        chOpts.DefaultValue = AcadUtility.AcadText.ChainageToString(0, Precision);
                        chOpts.UseDefaultValue = true;
                        PromptResult chRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetString(chOpts);
                        if (chRes.Status != PromptStatus.OK)
                        {
                            return;
                        }
                        if (AcadUtility.AcadText.TryChainageFromString(chRes.StringResult, out startCh))
                        {
                            break;
                        }
                    }
                    double endCh = Math.Ceiling((startCh + ex.MaxPoint.X) / ProfileGridH) * ProfileGridH;

                    // Draw grid
                    IEnumerable<Entity> entities = DrawProfileFrame(db, basePt, startCh, startLevel, endCh, endLevel, ProfileGridH, ProfileGridV, ProfileVScale, TextHeight, Precision, textStyleId);
                    foreach (Entity ent in entities)
                    {
                        ent.TransformBy(ucs2wcs);
                        btr.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                    }

                    // Draw center profile
                    ObjectId profileLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "Profil_Eksen", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 5));
                    Point2dCollection trPoints = new Point2dCollection(points.Count);
                    foreach (Point2d pt in points)
                    {
                        trPoints.Add(new Point2d(basePt.X + pt.X, basePt.Y + (pt.Y - startLevel) * ProfileVScale));
                    }
                    Polyline pline = AcadUtility.AcadEntity.CreatePolyLine(db, false, trPoints);
                    AcadUtility.AcadEntity.MatchEntity(center, pline);
                    pline.LayerId = profileLayerId;
                    pline.TransformBy(ucs2wcs);
                    btr.AppendEntity(pline);
                    tr.AddNewlyCreatedDBObject(pline, true);

                    // Draw other profiles
                    foreach (KeyValuePair<Entity, Point2dCollection> item in otherPoints)
                    {
                        Entity en = item.Key;
                        Point2dCollection spoints = item.Value;
                        Point2dCollection trsPoints = new Point2dCollection(points.Count);
                        foreach (Point2d pt in spoints)
                        {
                            trsPoints.Add(new Point2d(basePt.X + pt.X, basePt.Y + (pt.Y - startLevel) * ProfileVScale));
                        }
                        Polyline spline = AcadUtility.AcadEntity.CreatePolyLine(db, false, trsPoints);
                        AcadUtility.AcadEntity.MatchEntity(en, spline);
                        spline.LayerId = profileLayerId;
                        spline.TransformBy(ucs2wcs);

                        btr.AppendEntity(spline);
                        tr.AddNewlyCreatedDBObject(spline, true);
                    }
                }

                tr.Commit();
            }
        }

        private bool ShowSettings()
        {
            using (ProfileSettingsForm form = new ProfileSettingsForm())
            {
                form.GridH = ProfileGridH;
                form.GridV = ProfileGridV;
                form.VScale = ProfileVScale;
                form.TextHeight = TextHeight;
                form.Precision = Precision;

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
                }
                form.SetTextStyleNames(styleNames.ToArray());
                form.TextStyleName = TextStyleName;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                {
                    ProfileGridH = form.GridH;
                    ProfileGridV = form.GridV;
                    ProfileVScale = form.VScale;
                    TextHeight = form.TextHeight;
                    Precision = form.Precision;
                    TextStyleName = form.TextStyleName;

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        IEnumerable<ObjectId> SelectEntititesProfile()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;

            List<TypedValue> tvs = new List<TypedValue>();
            tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "ARC"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "LINE"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "POLYLINE"));
            tvs.Add(new TypedValue((int)DxfCode.Start, "SPLINE"));
            tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));

            SelectionFilter ssf = new SelectionFilter(tvs.ToArray());
            PromptSelectionResult res = doc.Editor.GetSelection(ssf);
            if (res.Status != PromptStatus.OK) return new ObjectId[0];

            return res.Value.GetObjectIds();
        }

        public static IEnumerable<Entity> DrawProfileFrame(Database db, Point3d basePt, double startCh, double startLevel, double endCh, double endLevel, double chStep, double levelStep, double levelScale, double textHeight, int precision, ObjectId textStyleId)
        {
            List<Entity> entities = new List<Entity>();

            ObjectId lineLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "Profil_Cizgi", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 200));
            ObjectId textLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "Profil_Yazi", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 230));
            ObjectId gridLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "Profil_Grid", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 254));
            ObjectId tickLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, "Profil_Tick", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 0));

            // Draw lines
            // Horizontal
            SortedDictionary<double, bool> levels = new SortedDictionary<double, bool>();
            for (double z = Math.Ceiling(startLevel / levelStep) * levelStep; z <= Math.Floor(endLevel / levelStep) * levelStep; z += levelStep)
            {
                levels[z] = true;
            }
            levels[startLevel] = false;
            levels[endLevel] = false;
            foreach (double z in levels.Keys)
            {
                Point3d pt1 = new Point3d(basePt.X, basePt.Y + (z - startLevel) * levelScale, basePt.Z);
                Point3d pt2 = new Point3d(basePt.X + (endCh - startCh), basePt.Y + (z - startLevel) * levelScale, basePt.Z);
                Line line = AcadUtility.AcadEntity.CreateLine(db, pt1, pt2, (levels[z] ? gridLayerId : lineLayerId));
                entities.Add(line);
                // Tick marks
                pt1 = new Point3d(basePt.X, basePt.Y + (z - startLevel) * levelScale, basePt.Z);
                pt2 = new Point3d(basePt.X - textHeight / 2.0, basePt.Y + (z - startLevel) * levelScale, basePt.Z);
                line = AcadUtility.AcadEntity.CreateLine(db, pt1, pt2, tickLayerId);
                entities.Add(line);
                // Level text
                pt1 = new Point3d(basePt.X - textHeight, basePt.Y + (z - startLevel) * levelScale, basePt.Z);
                DBText text = AcadUtility.AcadEntity.CreateText(db, pt1, AcadUtility.AcadText.LevelToString(z, precision), textHeight, 0, 0.8, TextHorizontalMode.TextRight, TextVerticalMode.TextVerticalMid, textStyleId, textLayerId);
                entities.Add(text);
            }
            // Vertical
            SortedDictionary<double, bool> chainages = new SortedDictionary<double, bool>();
            for (double ch = Math.Ceiling(startCh / chStep) * chStep; ch <= Math.Floor(endCh / chStep) * chStep; ch += chStep)
            {
                chainages[ch] = true;
            }
            chainages[startCh] = false;
            chainages[endCh] = false;
            foreach (double ch in chainages.Keys)
            {
                Point3d pt1 = new Point3d(basePt.X + (ch - startCh), basePt.Y, basePt.Z);
                Point3d pt2 = new Point3d(basePt.X + (ch - startCh), basePt.Y + (endLevel - startLevel) * levelScale, basePt.Z);
                Line line = AcadUtility.AcadEntity.CreateLine(db, pt1, pt2, (chainages[ch] ? gridLayerId : lineLayerId));
                entities.Add(line);
                // Tick marks
                pt1 = new Point3d(basePt.X + (ch - startCh), basePt.Y, basePt.Z);
                pt2 = new Point3d(basePt.X + (ch - startCh), basePt.Y - textHeight / 2, basePt.Z);
                line = AcadUtility.AcadEntity.CreateLine(db, pt1, pt2, tickLayerId);
                entities.Add(line);
                // Chainage text
                pt1 = new Point3d(basePt.X + (ch - startCh), basePt.Y - textHeight, basePt.Z);
                DBText text = AcadUtility.AcadEntity.CreateText(db, pt1, AcadUtility.AcadText.ChainageToString(ch, precision), textHeight, Math.PI / 2, 0.8, TextHorizontalMode.TextRight, TextVerticalMode.TextVerticalMid, textStyleId, textLayerId);
                entities.Add(text);
            }

            return entities;
        }
    }
}
