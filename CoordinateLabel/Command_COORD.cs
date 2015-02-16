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
        private bool AutoRotateText { get; set; }

        private bool AutoLine { get; set; }
        private double LineLength { get; set; }

        private int Precision { get; set; }

        private bool AutoNumbering { get; set; }
        private int CurrentNumber { get; set; }
        private string Prefix { get; set; }
        private string TextStyleName { get; set; }

        private class TextPositioner
        {
            private Matrix3d wcs2ucs;
            private Matrix3d ucs2wcs;
            private double rotation;
            private double length;
            private List<Point3d> points;

            public TextPositioner(Matrix3d ucs, double textRotation, double lineLength)
            {
                points = new List<Point3d>();
                ucs2wcs = ucs;
                wcs2ucs = wcs2ucs.Inverse();
                rotation = textRotation;
                length = lineLength;
            }

            public void AddPoint(Point3d point)
            {
                points.Add(point);
            }

            public void ClearPoints()
            {
                points.Clear();
            }

            public SelectedObjectCoordinate[] GetPositions()
            {
                List<SelectedObjectCoordinate> positions = new List<SelectedObjectCoordinate>();

                foreach (Point3d basePoint in points)
                {
                    double rot = rotation + Math.PI / 4;
                    Point3d textPoint = basePoint.TransformBy(wcs2ucs).Add(Vector3d.XAxis.RotateBy(rot, Vector3d.ZAxis) * length).TransformBy(ucs2wcs);
                    double maxdis = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        Point3d candidatePoint = basePoint.TransformBy(wcs2ucs).Add(Vector3d.XAxis.RotateBy(rot, Vector3d.ZAxis) * length).TransformBy(ucs2wcs);
                        // Select the point farthest away from the other points
                        double dis = 0;
                        foreach (Point3d comparePoint in points)
                        {
                            dis += Math.Abs(comparePoint.DistanceTo(candidatePoint));
                        }
                        if (dis > maxdis)
                        {
                            maxdis = dis;
                            textPoint = candidatePoint;
                        }
                        rot += Math.PI / 2;
                    }

                    bool textToLeft = (textPoint.TransformBy(wcs2ucs).X < basePoint.TransformBy(wcs2ucs).X);
                    positions.Add(new SelectedObjectCoordinate(basePoint, textPoint, textToLeft));
                }

                return positions.ToArray();
            }

            public SelectedObjectCoordinate GetPosition(Point3d basePoint)
            {
                Point3d textPoint = basePoint.TransformBy(wcs2ucs).Add(Vector3d.XAxis.RotateBy(rotation + Math.PI / 4, Vector3d.ZAxis) * length).TransformBy(ucs2wcs);
                bool textToLeft = (textPoint.TransformBy(wcs2ucs).X < basePoint.TransformBy(wcs2ucs).X);
                return new SelectedObjectCoordinate(basePoint, textPoint, textToLeft);
            }

            public class SelectedObjectCoordinate
            {
                public Point3d BasePoint { get; set; }
                public Point3d TextPoint { get; set; }
                public bool TextToLeft { get; set; }
                public SelectedObjectCoordinate(Point3d basePoint, Point3d textPoint, bool textToLeft)
                {
                    BasePoint = basePoint;
                    TextPoint = textPoint;
                    TextToLeft = textToLeft;
                }
            }
        }

        public Command_COORD()
        {
            init = false;
            points = new List<CoordPoint>();

            TextHeight = 0.25;

            TextRotation = 0.0;
            AutoRotateText = false;

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

            while (flag)
            {
                PromptPointResult pointRes = null;
                if (AutoNumbering)
                {
                    PromptPointOptions pointOpts = new PromptPointOptions("\n" + CurrentNumber.ToString() + ". Koordinat yeri: [Reset/Liste/Seç]", "Reset List Select");
                    pointRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(pointOpts);
                }
                else
                {
                    PromptPointOptions pointOpts = new PromptPointOptions("\nKoordinat yeri: [Reset/Seç]", "Reset Select");
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
                else if (pointRes.Status == PromptStatus.Keyword && pointRes.StringResult == "Select")
                {
                    SelectObjectsForm form = new SelectObjectsForm();
                    if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
                    {
                        // Select objects
                        List<TypedValue> tvs = new List<TypedValue>();
                        switch (form.SelectObjects)
                        {
                            case SelectObjectsForm.SelectCoordinateObjects.Polyline:
                                tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
                                tvs.Add(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"));
                                tvs.Add(new TypedValue((int)DxfCode.Start, "POLYLINE"));
                                tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
                                break;
                            case SelectObjectsForm.SelectCoordinateObjects.Circle:
                                tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
                                tvs.Add(new TypedValue((int)DxfCode.Start, "CIRCLE"));
                                tvs.Add(new TypedValue((int)DxfCode.Start, "ARC"));
                                tvs.Add(new TypedValue((int)DxfCode.Start, "ELLIPSE"));
                                tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
                                break;
                            case SelectObjectsForm.SelectCoordinateObjects.Block:
                                tvs.Add(new TypedValue((int)DxfCode.Start, "INSERT"));
                                break;
                            case SelectObjectsForm.SelectCoordinateObjects.Point:
                                tvs.Add(new TypedValue((int)DxfCode.Start, "POINT"));
                                break;
                            case SelectObjectsForm.SelectCoordinateObjects.Line:
                                tvs.Add(new TypedValue((int)DxfCode.Start, "LINE"));
                                break;
                        }
                        SelectionFilter filter = new SelectionFilter(tvs.ToArray());

                        PromptSelectionResult selRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection(filter);
                        if (selRes.Status == PromptStatus.OK)
                        {
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                            {
                                TextPositioner positioner = new TextPositioner(ucs2wcs, TextRotation * Math.PI / 180, LineLength);

                                // Read object coordinates
                                List<TextPositioner.SelectedObjectCoordinate> objectPoints = new List<TextPositioner.SelectedObjectCoordinate>();
                                foreach (ObjectId id in selRes.Value.GetObjectIds())
                                {
                                    if (id.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(Autodesk.AutoCAD.DatabaseServices.Polyline)))
                                    {
                                        Autodesk.AutoCAD.DatabaseServices.Polyline obj = tr.GetObject(id, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Polyline;

                                        positioner.ClearPoints();
                                        for (int i = 0; i < obj.NumberOfVertices; i++)
                                        {
                                            positioner.AddPoint(obj.GetPoint3dAt(i));
                                        }
                                        objectPoints.AddRange(positioner.GetPositions());
                                    }
                                    else if (id.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(Autodesk.AutoCAD.DatabaseServices.Circle)))
                                    {
                                        Circle obj = tr.GetObject(id, OpenMode.ForRead) as Circle;
                                        objectPoints.Add(positioner.GetPosition(obj.Center));
                                    }
                                    else if (id.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(Autodesk.AutoCAD.DatabaseServices.Arc)))
                                    {
                                        Arc obj = tr.GetObject(id, OpenMode.ForRead) as Arc;
                                        objectPoints.Add(positioner.GetPosition(obj.Center));
                                    }
                                    else if (id.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(Autodesk.AutoCAD.DatabaseServices.Ellipse)))
                                    {
                                        Ellipse obj = tr.GetObject(id, OpenMode.ForRead) as Ellipse;
                                        objectPoints.Add(positioner.GetPosition(obj.Center));
                                    }
                                    else if (id.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(Autodesk.AutoCAD.DatabaseServices.BlockReference)))
                                    {
                                        BlockReference obj = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                                        objectPoints.Add(positioner.GetPosition(obj.Position));
                                    }
                                    else if (id.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(Autodesk.AutoCAD.DatabaseServices.DBPoint)))
                                    {
                                        DBPoint obj = tr.GetObject(id, OpenMode.ForRead) as DBPoint;
                                        objectPoints.Add(positioner.GetPosition(obj.Position));

                                    }
                                    else if (id.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(Autodesk.AutoCAD.DatabaseServices.Line)))
                                    {
                                        Line obj = tr.GetObject(id, OpenMode.ForRead) as Line;
                                        positioner.ClearPoints();
                                        positioner.AddPoint(obj.StartPoint);
                                        positioner.AddPoint(obj.EndPoint);
                                        objectPoints.AddRange(positioner.GetPositions());
                                    }
                                }
                                // Sort coordinates
                                objectPoints.Sort((p1, p2) =>
                                {
                                    switch (form.Ordering)
                                    {
                                        case SelectObjectsForm.CoordinateOrdering.IncreasingX:
                                            return (p1.BasePoint.X < p2.BasePoint.X ? -1 : 1);
                                        case SelectObjectsForm.CoordinateOrdering.IncreasingY:
                                            return (p1.BasePoint.Y < p2.BasePoint.Y ? -1 : 1);
                                        case SelectObjectsForm.CoordinateOrdering.DecreasingX:
                                            return (p1.BasePoint.X > p2.BasePoint.X ? -1 : 1);
                                        case SelectObjectsForm.CoordinateOrdering.DecreasingY:
                                            return (p1.BasePoint.Y > p2.BasePoint.Y ? -1 : 1);
                                        default:
                                            return 0;
                                    }
                                });
                                // Write coordinates
                                foreach (TextPositioner.SelectedObjectCoordinate coord in objectPoints)
                                {
                                    MText mtext = CreateText(textStyleId, coord.TextPoint);
                                    btr.AppendEntity(mtext);
                                    tr.AddNewlyCreatedDBObject(mtext, true);

                                    // Rotate text
                                    if (coord.TextToLeft)
                                        mtext.Attachment = (AutoNumbering ? AttachmentPoint.BottomRight : AttachmentPoint.MiddleRight);
                                    else
                                        mtext.Attachment = (AutoNumbering ? AttachmentPoint.BottomLeft : AttachmentPoint.MiddleLeft);

                                    Line line = new Line();
                                    line.StartPoint = coord.BasePoint;
                                    line.EndPoint = coord.TextPoint;
                                    btr.AppendEntity(line);
                                    tr.AddNewlyCreatedDBObject(line, true);

                                    points.Add(new CoordPoint(CurrentNumber, coord.BasePoint));
                                    if (AutoNumbering) CurrentNumber = CurrentNumber + 1;
                                }

                                tr.Commit();
                            }
                        }
                    }
                    return;
                }
                else
                {
                    Point3d pCoord = pointRes.Value.TransformBy(ucs2wcs);

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                    {
                        MText mtext = CreateText(textStyleId, pCoord);

                        btr.AppendEntity(mtext);
                        tr.AddNewlyCreatedDBObject(mtext, true);

                        if (CoordinateJig.Jig(pointRes.Value, mtext, AutoLine, LineLength, AutoRotateText, TextRotation))
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
            form.AutoRotateText = AutoRotateText;

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
                AutoRotateText = form.AutoRotateText;

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
            if (!AutoRotateText)
            {
                mtext.Rotation = TextRotation * Math.PI / 180;
            }
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
            private bool mAutoRotateText = false;
            private double mTextRotation = 0;
            private bool mAutoLine = false;
            private double mLineLength = 1.0;
            private Line line = null;

            private CoordinateJig(Entity en, Point3d pBase, bool autoLine, double lineLength, bool autoRotateText, double textRotation)
                : base(en)
            {
                mpBase = pBase;
                mpText = pBase.Add(Vector3d.XAxis);
                mAutoLine = autoLine;
                mLineLength = lineLength;
                mAutoRotateText = autoRotateText;
                mTextRotation = textRotation;
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

            public static bool Jig(Point3d pBase, MText mtext, bool autoLine, double lineLength, bool autoRotateText, double textRotation)
            {
                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

                Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                Point3d pBaseWorld = pBase.TransformBy(ucs2wcs);

                CoordinateJig jigger = new CoordinateJig(mtext, pBase, autoLine, lineLength, autoRotateText, textRotation);

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
                Matrix3d wcs2ucs = ucs2wcs.Inverse();
                Point3d pBaseWorld = mpBase.TransformBy(ucs2wcs);
                Point3d pTextWorld = mpText.TransformBy(ucs2wcs);

                MText mtext = Entity as MText;
                Vector3d dir = (mpText - mpBase);

                // Text attachment
                mtext.Location = pTextWorld;
                bool singleLine = (mtext.Attachment == AttachmentPoint.BottomLeft || mtext.Attachment == AttachmentPoint.BottomRight);

                // Text rotation and attachment
                if (mAutoRotateText)
                {
                    double lineRotation = mTextRotation * Math.PI / 180 + Vector3d.XAxis.GetAngleTo(dir, Vector3d.ZAxis);
                    double rot = lineRotation * 180 / Math.PI;

                    if (rot > 90.0 && rot < 270.0)
                    {
                        lineRotation = lineRotation + Math.PI;
                        mtext.Attachment = (singleLine ? AttachmentPoint.BottomRight : AttachmentPoint.MiddleRight);
                    }
                    else
                    {
                        mtext.Attachment = (singleLine ? AttachmentPoint.BottomLeft : AttachmentPoint.MiddleLeft);
                    }

                    mtext.Rotation = lineRotation;
                }
                else
                {
                    mtext.Rotation = mTextRotation * Math.PI / 180;
                }

                // Text to the right or left
                double textlineAngle = Vector3d.XAxis.RotateBy(mtext.Rotation, Vector3d.XAxis).GetAngleTo(dir) * 180 / Math.PI;

                if (textlineAngle > 90.0 && textlineAngle < 270.0)
                    mtext.Attachment = (singleLine ? AttachmentPoint.BottomRight : AttachmentPoint.MiddleRight);
                else
                    mtext.Attachment = (singleLine ? AttachmentPoint.BottomLeft : AttachmentPoint.MiddleLeft);

                // Create and update line
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
