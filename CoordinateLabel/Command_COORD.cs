using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace CoordinateLabel
{
    public class Command_COORD
    {
        private enum CoordSystem
        {
            WCS,
            UCS
        }

        private class CoordPoint
        {
            public int N { get; set; }
            public double X1 { get; set; }
            public double Y1 { get; set; }
            public double Z1 { get; set; }
            public double X2 { get; set; }
            public double Y2 { get; set; }
            public double Z2 { get; set; }

            public CoordPoint(int n, double x1, double y1, double z1, double x2, double y2, double z2)
            {
                N = n;
                X1 = x1;
                Y1 = y1;
                Z1 = z1;
                X2 = x2;
                Y2 = y2;
                Z2 = z2;
            }

            public CoordPoint(int n, Point3d p1, Point3d p2)
            {
                N = n;
                X1 = p1.X;
                Y1 = p1.Y;
                Z1 = p1.Z;
                X2 = p2.X;
                Y2 = p2.Y;
                Z2 = p2.Z;
            }
        }

        private List<CoordPoint> points;

        private CoordSystem CoordinateSystem { get; set; }

        private double TextHeight { get; set; }
        private double TextRotation { get; set; }
        private bool AutoLineLength { get; set; }
        private double LineLength { get; set; }

        private int Precision { get; set; }

        private bool AutoNumbering { get; set; }
        private int CurrentNumber { get; set; }
        private string Prefix { get; set; }

        private bool UseX { get; set; }
        private string XLabel { get; set; }
        private bool UseY { get; set; }
        private string YLabel { get; set; }
        private bool UseZ { get; set; }
        private string ZLabel { get; set; }

        public Command_COORD()
        {
            points = new List<CoordPoint>();

            CoordinateSystem = CoordSystem.WCS;

            TextHeight = 0.25;
            TextRotation = 0.0;
            AutoLineLength = false;
            LineLength = 1.0;

            Precision = 3;

            AutoNumbering = false;
            CurrentNumber = 1;
            Prefix = "";

            UseX = true;
            XLabel = "X";
            UseY = true;
            YLabel = "Y";
            UseZ = false;
            ZLabel = "Z";

            CurrentNumber = 1;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("KOORDINAT")]
        public void Coord()
        {
            bool flag = true;

            while (flag)
            {
                PromptPointResult pointRes = null;
                if (AutoNumbering)
                {
                    PromptPointOptions pointOpts = new PromptPointOptions("\n" + CurrentNumber.ToString() + ". Koordinat yeri: [Seçenekler/Reset/Liste]", "Settings Reset List");
                    pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(pointOpts);
                }
                else
                {
                    PromptPointOptions pointOpts = new PromptPointOptions("\nKoordinat yeri: [Seçenekler/Reset/Liste]", "Settings Reset List");
                    pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(pointOpts);
                }

                if (pointRes.Status == PromptStatus.Cancel)
                {
                    return;
                }
                else if (pointRes.Status == PromptStatus.Keyword && pointRes.StringResult == "Settings")
                {
                    ShowSettings();
                    continue;
                }
                else if (pointRes.Status == PromptStatus.Keyword && pointRes.StringResult == "Reset")
                {
                    Reset();
                    return;
                }
                else if (pointRes.Status == PromptStatus.Keyword && pointRes.StringResult == "List")
                {
                    ShowList();
                    continue;
                }
                else
                {
                    PromptPointOptions textOpts = new PromptPointOptions("\nYazı yeri: ");
                    textOpts.BasePoint = pointRes.Value;
                    textOpts.UseBasePoint = true;
                    PromptPointResult textRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(textOpts);

                    if (textRes.Status == PromptStatus.Cancel)
                    {
                        return;
                    }
                    else
                    {
                        AddPoint(pointRes.Value, textRes.Value);
                    }
                }
            }
        }

        private void ShowSettings()
        {
            CoordMainForm form = new CoordMainForm();

            form.UseWCS = (CoordinateSystem == CoordSystem.WCS);

            form.TextHeight = TextHeight;
            form.TextRotation = TextRotation;
            form.AutoLineLength = AutoLineLength;
            form.LineLength = LineLength;

            form.Precision = Precision;

            form.AutoNumbering = AutoNumbering;
            form.StartingNumber = CurrentNumber;
            form.Prefix = Prefix;

            form.UseX = UseX;
            form.XLabel = XLabel;
            form.UseY = UseY;
            form.YLabel = YLabel;
            form.UseZ = UseZ;
            form.ZLabel = ZLabel;

            if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
            {
                CoordinateSystem = (form.UseWCS ? CoordSystem.WCS : CoordSystem.UCS);

                TextHeight = form.TextHeight;
                TextRotation = form.TextRotation;
                AutoLineLength = form.AutoLineLength;
                LineLength = form.LineLength;

                Precision = form.Precision;

                AutoNumbering = form.AutoNumbering;
                CurrentNumber = form.StartingNumber;
                Prefix = form.Prefix;

                UseX = form.UseX;
                XLabel = form.XLabel;
                UseY = form.UseY;
                YLabel = form.YLabel;
                UseZ = form.UseZ;
                ZLabel = form.ZLabel;
            }
        }

        private void AddPoint(Point3d p1, Point3d p2)
        {
            points.Add(new CoordPoint(CurrentNumber, p1, p2));

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord blockTableRecord = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                Line line = new Line();
                line.StartPoint = p1;
                line.EndPoint = p2;

                blockTableRecord.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);

                tr.Commit();
            }

            if (AutoNumbering)
            {
                CurrentNumber = CurrentNumber + 1;
            }
        }

        private void Reset()
        {
            points.Clear();
            CurrentNumber = 1;
        }

        private void ShowList()
        {

        }
    }
}
