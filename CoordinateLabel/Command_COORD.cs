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
        private bool init;
        private List<CoordPoint> points;

        private double TextHeight { get; set; }
        private double TextRotation { get; set; }
        private bool AutoLine { get; set; }
        private double LineLength { get; set; }

        private int Precision { get; set; }

        private bool AutoNumbering { get; set; }
        private int CurrentNumber { get; set; }
        private string Prefix { get; set; }
        private string TextStyleName { get; set; }

        public Command_COORD()
        {
            init = false;
            points = new List<CoordPoint>();

            TextHeight = 0.25;
            TextRotation = 0.0;
            AutoLine = false;
            LineLength = 1.0;

            Precision = 3;

            AutoNumbering = false;
            CurrentNumber = 1;
            Prefix = "";

            CurrentNumber = 1;

            TextStyleName = "MTT";
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("KOORDINAT")]
        public void Coord()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            if (!init)
            {
                if (!ShowSettings()) return;
            }

            bool flag = true;

            while (flag)
            {
                PromptPointResult pointRes = null;
                if (AutoNumbering)
                {
                    PromptPointOptions pointOpts = new PromptPointOptions("\n" + CurrentNumber.ToString() + ". Koordinat yeri: [Reset/Liste]", "Reset List");
                    pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(pointOpts);
                }
                else
                {
                    PromptPointOptions pointOpts = new PromptPointOptions("\nKoordinat yeri: [Reset]", "Reset");
                    pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(pointOpts);
                }

                if (pointRes.Status == PromptStatus.Cancel)
                {
                    return;
                }
                else if (pointRes.Status == PromptStatus.Keyword && pointRes.StringResult == "Reset")
                {
                    Reset();
                    return;
                }
                else if (pointRes.Status == PromptStatus.Keyword && pointRes.StringResult == "List")
                {
                    PromptPointOptions listOpts = new PromptPointOptions("\nListe yeri: ");
                    PromptPointResult listRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(listOpts);
                    ShowList(db, listRes.Value);
                    return;
                }
                else
                {
                    Point3d coordPt = pointRes.Value;
                    Point3d textPt;
                    if (AutoLine)
                    {
                        PromptAngleOptions textOpts = new PromptAngleOptions("\nYazı yönü: ");
                        textOpts.BasePoint = pointRes.Value;
                        textOpts.UseBasePoint = true;
                        PromptDoubleResult textRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetAngle(textOpts);
                        if (textRes.Status == PromptStatus.Cancel)
                            return;
                        else
                            textPt = coordPt + new Vector3d(Math.Cos(textRes.Value), Math.Sin(textRes.Value), 0) * LineLength;
                    }
                    else
                    {
                        PromptPointOptions textOpts = new PromptPointOptions("\nYazı yeri: ");
                        textOpts.BasePoint = pointRes.Value;
                        textOpts.UseBasePoint = true;
                        PromptPointResult textRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(textOpts);
                        if (textRes.Status == PromptStatus.Cancel)
                            return;
                        else
                            textPt = textRes.Value;
                    }

                    AddPoint(db, coordPt, textPt);
                }
            }
        }

        private bool ShowSettings()
        {
            CoordMainForm form = new CoordMainForm();

            form.TextHeight = TextHeight;
            form.TextRotation = TextRotation;
            form.AutoLineLength = AutoLine;
            form.LineLength = LineLength;

            form.Precision = Precision;

            form.AutoNumbering = AutoNumbering;
            form.StartingNumber = CurrentNumber;
            form.Prefix = Prefix;

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

            if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
            {
                TextHeight = form.TextHeight;
                TextRotation = form.TextRotation;
                AutoLine = form.AutoLineLength;
                LineLength = form.LineLength;

                Precision = form.Precision;

                AutoNumbering = form.AutoNumbering;
                CurrentNumber = form.StartingNumber;
                Prefix = form.Prefix;

                TextStyleName = form.TextStyleName;

                if (form.CoordsFromDWG != null && form.CoordsFromDWG.Length > 0)
                {
                    bool xy = form.CoordsFromDWG[0].IsXYText;
                    if (xy)
                    {
                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            foreach (CoordMainForm.CoordItem item in form.CoordsFromDWG)
                            {
                                string text = GetCoordText(item.X, item.Y);
                                MText mtext = (MText)tr.GetObject(item.ID, OpenMode.ForWrite);
                                mtext.Contents = text;
                            }

                            tr.Commit();
                        }
                    }
                    else
                    {
                        points.Clear();
                        foreach (CoordMainForm.CoordItem item in form.CoordsFromDWG)
                        {
                            points.Add(new CoordPoint(item.Number, item.X, item.Y, item.Z));
                        }
                    }
                }

                init = true;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddPoint(Database db, Point3d pBase, Point3d pText)
        {
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
            Point3d pBaseWorld = pBase.TransformBy(ucs2wcs);
            Point3d pTextWorld = pText.TransformBy(ucs2wcs);

            Point3d pCoord = pBaseWorld;

            points.Add(new CoordPoint(CurrentNumber, pCoord));

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                Line line = new Line();
                line.StartPoint = pBaseWorld;
                line.EndPoint = pTextWorld;

                btr.AppendEntity(line);
                tr.AddNewlyCreatedDBObject(line, true);

                string text = GetCoordText(pCoord.X, pCoord.Y);
                int lineCount = (AutoNumbering ? 1 : 2);
                bool right = (pText.X > pBase.X); // Text to the right

                MText mtext = new MText();
                mtext.Contents = text;
                mtext.Location = pTextWorld;
                mtext.TextHeight = TextHeight;
                mtext.Rotation = TextRotation;
                if (right)
                    mtext.Attachment = (lineCount == 1 ? AttachmentPoint.BottomLeft : AttachmentPoint.MiddleLeft);
                else
                    mtext.Attachment = (lineCount == 1 ? AttachmentPoint.BottomRight : AttachmentPoint.MiddleRight);

                TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
                if (tt.Has(TextStyleName))
                {
                    mtext.TextStyleId = tt[TextStyleName];
                }

                btr.AppendEntity(mtext);
                tr.AddNewlyCreatedDBObject(mtext, true);

                tr.Commit();
            }

            if (AutoNumbering)
            {
                CurrentNumber = CurrentNumber + 1;
            }
        }

        private string GetCoordText(double x, double y)
        {
            string text = "";
            if (AutoNumbering)
            {
                text = "{\\L" + Prefix + CurrentNumber.ToString() + "}";
            }
            else
            {
                string format = "0." + new string('0', Precision);
                if (Precision == 0) format = "0";

                string xtext = "X=" + x.ToString(format);
                string ytext = "Y=" + y.ToString(format);

                int maxlen = Math.Max(xtext.Length, ytext.Length);
                xtext += new string(' ', maxlen - xtext.Length);
                ytext += new string(' ', maxlen - ytext.Length);

                text += "{\\L" + xtext + "}" + "\\P" + ytext;
            }

            return text;
        }

        private void Reset()
        {
            init = false;
            points.Clear();
            CurrentNumber = 1;
        }

        private void ShowList(Database db, Point3d pBase)
        {
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
            double rotation = 2.0 * Math.PI - Vector3d.XAxis.TransformBy(ucs2wcs).GetAngleTo(Vector3d.XAxis);

            double height = TextHeight;
            double margin = 0.2 * TextHeight;
            double row = height + 2 * margin;

            Point3d ptt = new Point3d(pBase.X + margin, pBase.Y - margin - height, pBase.Z);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                // Header
                MakeDBText(tr, db, btr, ptt.TransformBy(ucs2wcs), "NOKTA NO         X KOORD         Y KOORD", height, rotation);
                ptt = new Point3d(ptt.X, ptt.Y - row, ptt.Z);

                // Lines
                foreach (CoordPoint pt in points)
                {
                    string text = pt.ToString(Prefix, 8, Precision, 16);
                    MakeDBText(tr, db, btr, ptt.TransformBy(ucs2wcs), text, height, rotation);
                    ptt = new Point3d(ptt.X, ptt.Y - row, ptt.Z);
                }

                tr.Commit();
            }
        }

        private DBText MakeDBText(Transaction tr, Database db, BlockTableRecord btr, Point3d position, string text, double height, double rotation)
        {
            DBText dbtext = new DBText();
            dbtext.Position = position;
            dbtext.Height = height;
            dbtext.Rotation = rotation;
            dbtext.TextString = text;
            TextStyleTable tt = (TextStyleTable)tr.GetObject(db.TextStyleTableId, OpenMode.ForRead);
            if (tt.Has(TextStyleName))
            {
                dbtext.TextStyleId = tt[TextStyleName];
            }
            dbtext.WidthFactor = 0.8;

            btr.AppendEntity(dbtext);
            tr.AddNewlyCreatedDBObject(dbtext, true);

            return dbtext;
        }
    }
}
