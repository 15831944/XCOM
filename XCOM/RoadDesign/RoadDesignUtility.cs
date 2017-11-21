using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM.Commands.RoadDesign
{
    public static class RoadDesignUtility
    {
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
