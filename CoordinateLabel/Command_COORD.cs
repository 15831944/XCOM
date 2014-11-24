using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;

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

                    Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                    Point3d pCoord = pointRes.Value.TransformBy(ucs2wcs);

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                    {
                        MText mtext = CreateText(textStyleId, pCoord);

                        btr.AppendEntity(mtext);
                        tr.AddNewlyCreatedDBObject(mtext, true);

                        if (CoordinateJig.Jig(pointRes.Value, mtext, AutoLine, LineLength))
                        {
                            points.Add(new CoordPoint(CurrentNumber, pCoord));
                            if (AutoNumbering) CurrentNumber = CurrentNumber + 1;

                            Line line = new Line();
                            line.StartPoint = pCoord;
                            line.EndPoint = mtext.Location;

                            btr.AppendEntity(line);
                            tr.AddNewlyCreatedDBObject(line, true);

                            tr.Commit();
                        }
                        else
                        {
                            mtext.Dispose();
                            tr.Abort();
                            return;
                        }
                    }
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
                                string text = GetCoordText(new Point3d(item.X, item.Y, 0));
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

        private MText CreateText(ObjectId textStyleId, Point3d pt)
        {
            MText mtext = new MText();
            mtext.Contents = GetCoordText(pt);
            mtext.Location = pt;
            mtext.Attachment = (AutoNumbering ? AttachmentPoint.BottomLeft : AttachmentPoint.MiddleLeft);
            mtext.TextHeight = TextHeight;
            mtext.Rotation = TextRotation * Math.PI / 180;
            if (!textStyleId.IsNull) mtext.TextStyleId = textStyleId;

            return mtext;
        }

        private string GetCoordText(Point3d pt)
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

                string xtext = "X=" + pt.X.ToString(format);
                string ytext = "Y=" + pt.Y.ToString(format);

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

        private class CoordinateJig : EntityJig, IDisposable
        {
            private Point3d mpBase = new Point3d();
            private Point3d mpText = new Point3d();
            private bool mAutoLine = false;
            private double mLineLength = 1.0;
            private Line line = null;

            private CoordinateJig(Entity en, Point3d pBase, bool autoLine, double lineLength)
                : base(en)
            {
                mpBase = pBase;
                mpText = pBase.Add(Vector3d.XAxis);
                mAutoLine = autoLine;
                mLineLength = lineLength;
            }

            protected override bool Update()
            {
                UpdateText();
                return true;
            }

            protected override SamplerStatus Sampler(JigPrompts prompts)
            {
                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                Matrix3d wcs2ucs = Matrix3d.AlignCoordinateSystem(db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir), Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis);

                if (mAutoLine)
                {
                    JigPromptPointOptions textOpts = new JigPromptPointOptions("\nYazı yönü: ");
                    textOpts.BasePoint = mpBase;
                    textOpts.UseBasePoint = true;
                    PromptPointResult textRes = prompts.AcquirePoint(textOpts);
                    if (textRes.Status != PromptStatus.OK) return SamplerStatus.Cancel;
                    Point3d pt = textRes.Value.TransformBy(wcs2ucs);
                    Vector3d dir = (pt - mpBase);
                    dir = dir / dir.Length * mLineLength;
                    mpText = mpBase + dir;
                }
                else
                {
                    JigPromptPointOptions textOpts = new JigPromptPointOptions("\nYazı yeri: ");
                    textOpts.BasePoint = mpBase;
                    textOpts.UseBasePoint = true;
                    PromptPointResult textRes = prompts.AcquirePoint(textOpts);
                    mpText = textRes.Value.TransformBy(wcs2ucs);
                }

                return SamplerStatus.OK;
            }

            public static bool Jig(Point3d pBase, MText mtext, bool autoLine, double lineLength)
            {
                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                Point3d pBaseWorld = pBase.TransformBy(ucs2wcs);

                CoordinateJig jigger = new CoordinateJig(mtext, pBase, autoLine, lineLength);

                PromptResult res = doc.Editor.Drag(jigger);

                jigger.EraseLine();

                if (res.Status == PromptStatus.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private void UpdateText()
            {
                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                Point3d pBaseWorld = mpBase.TransformBy(ucs2wcs);
                Point3d pTextWorld = mpText.TransformBy(ucs2wcs);

                MText mtext = Entity as MText;

                // Text to the right or left
                Vector3d rotatedVertical = Vector3d.YAxis.RotateBy(mtext.Rotation, Vector3d.ZAxis);
                Vector3d dir = (mpText - mpBase);
                double rot = dir.GetAngleTo(rotatedVertical, Vector3d.ZAxis) * 180 / Math.PI;
                bool right = (rot > 0.0 && rot < 180.0);

                mtext.Location = pTextWorld;
                bool singleLine = (mtext.Attachment == AttachmentPoint.BottomLeft || mtext.Attachment == AttachmentPoint.BottomRight);
                if (right)
                    mtext.Attachment = (singleLine ? AttachmentPoint.BottomLeft : AttachmentPoint.MiddleLeft);
                else
                    mtext.Attachment = (singleLine ? AttachmentPoint.BottomRight : AttachmentPoint.MiddleRight);

                if (line == null)
                {
                    line = new Line();
                    TransientManager.CurrentTransientManager.AddTransient(line, TransientDrawingMode.DirectShortTerm, 0, new IntegerCollection());
                }
                line.StartPoint = pBaseWorld;
                line.EndPoint = pTextWorld;
                TransientManager.CurrentTransientManager.UpdateTransient(line, new IntegerCollection());
            }

            private static Point3d Intersect(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
            {
                using (Line3d l1 = new Line3d(p1, p2))
                using (Line3d l2 = new Line3d(p3, p4))
                {
                    return l1.IntersectWith(l2)[0];
                }
            }

            public void EraseLine()
            {
                if (line != null)
                {
                    TransientManager.CurrentTransientManager.EraseTransient(line, new IntegerCollection());
                    line.Dispose();
                    line = null;
                }
            }

            public void Dispose()
            {
                EraseLine();
            }
        }
    }
}
