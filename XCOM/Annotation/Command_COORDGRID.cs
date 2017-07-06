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
using System.Globalization;

namespace XCOM.Commands.Annotation
{
    public class Command_COORDGRID
    {
        private static string TextStyleName = "MTT";
        private static string TextLayerName = "COORDGRID_TEXT";
        private static string LineLayerName = "COORDGRID_LINE";
        private static string BlockName = "COORDGRID_PT";

        private double TextHeight { get; set; }
        private int Interval { get; set; }

        [Autodesk.AutoCAD.Runtime.CommandMethod("COORDGRID")]
        public void MakeCoordGrid()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

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

            ObjectId textLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, TextLayerName, Color.FromColorIndex(ColorMethod.ByAci, 1));
            ObjectId lineLayerId = AcadUtility.AcadEntity.GetOrCreateLayer(db, LineLayerName, Color.FromColorIndex(ColorMethod.ByAci, 3));

            ObjectId blockId = GetOrCreateBlock(db, BlockName, textLayerId, lineLayerId, textStyleId);

            Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;
            Matrix3d wcs2ucs = AcadUtility.AcadGraphics.WcsToUcs;

            // Pick polyline
            Point3d[] points = new Point3d[4];
            int xmin = int.MaxValue;
            int xmax = int.MinValue;
            int ymin = int.MaxValue;
            int ymax = int.MinValue;
            for (int i = 0; i < 4; i++)
            {
                PromptPointResult ptRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nKöşe noktası: ");

                if (ptRes.Status == PromptStatus.OK)
                {
                    Point3d pt = ptRes.Value;
                    points[i] = pt;
                    xmin = Math.Min(xmin, (int)pt.X); xmax = Math.Max(xmax, (int)pt.X);
                    ymin = Math.Min(ymin, (int)pt.Y); ymax = Math.Max(ymax, (int)pt.Y);
                }
                else
                {
                    return;
                }
            }

            // Interval
            PromptIntegerOptions intOpts = new PromptIntegerOptions("\nAralık: ");
            intOpts.AllowNegative = false;
            intOpts.AllowZero = false;
            intOpts.AllowNone = false;
            intOpts.DefaultValue = Properties.Settings.Default.Command_COORDGRID_Interval;
            intOpts.UseDefaultValue = true;
            PromptIntegerResult intRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(intOpts);
            if (intRes.Status == PromptStatus.OK)
            {
                Interval = intRes.Value;
            }
            else
            {
                return;
            }

            // Round limits to multiples of the interval
            xmin = (int)Math.Floor((double)xmin / Interval) * Interval;
            xmax = (int)Math.Ceiling((double)xmax / Interval) * Interval;
            ymin = (int)Math.Floor((double)ymin / Interval) * Interval;
            ymax = (int)Math.Ceiling((double)ymax / Interval) * Interval;

            // Text height            
            PromptDoubleOptions thOpts = new PromptDoubleOptions("\nYazı yüksekliği: ");
            thOpts.AllowNegative = false;
            thOpts.AllowZero = false;
            thOpts.AllowNone = false;
            thOpts.DefaultValue = Properties.Settings.Default.Command_COORDGRID_TextHeight;
            thOpts.UseDefaultValue = true;
            PromptDoubleResult thRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble(thOpts);
            if (thRes.Status == PromptStatus.OK)
            {
                TextHeight = thRes.Value;
            }
            else
            {
                return;
            }

            // Save settings
            Properties.Settings.Default.Command_COORDGRID_TextHeight = TextHeight;
            Properties.Settings.Default.Command_COORDGRID_Interval = Interval;
            Properties.Settings.Default.Save();

            // Print grid
            NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";
            nfi.NumberDecimalDigits = 0;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                BlockTableRecord blockDef = (BlockTableRecord)tr.GetObject(blockId, OpenMode.ForRead);

                for (int x = xmin; x <= xmax; x += Interval)
                {
                    for (int y = ymin; y <= ymax; y += Interval)
                    {
                        Point3d v = new Point3d(x, y, 0);
                        if (!PolylineContains(points, v)) continue;

                        BlockReference blockRef = AcadUtility.AcadEntity.CreateBlockReference(db, blockId, v, TextHeight, 0);

                        btr.AppendEntity(blockRef);
                        tr.AddNewlyCreatedDBObject(blockRef, true);

                        // Set attributes
                        foreach (ObjectId id in blockDef)
                        {
                            AttributeDefinition attDef = tr.GetObject(id, OpenMode.ForRead) as AttributeDefinition;
                            if (attDef != null)
                            {
                                using (AttributeReference attRef = new AttributeReference())
                                {
                                    attRef.SetDatabaseDefaults(db);
                                    attRef.SetAttributeFromBlock(attDef, blockRef.BlockTransform);
                                    blockRef.AttributeCollection.AppendAttribute(attRef);
                                    tr.AddNewlyCreatedDBObject(attRef, true);
                                    attRef.TextString = attDef.Tag == "X" ? x.ToString("n", nfi) : y.ToString("n", nfi);
                                    attRef.AdjustAlignment(db);
                                }
                            }
                        }
                    }
                }

                tr.Commit();
            }
        }

        public Command_COORDGRID()
        {
            TextHeight = Properties.Settings.Default.Command_COORDGRID_TextHeight;
            Interval = Properties.Settings.Default.Command_COORDGRID_Interval;
        }

        private ObjectId GetOrCreateBlock(Database db, string blockName, ObjectId textLayerId, ObjectId lineLayerId, ObjectId textStyleId)
        {
            ObjectId id = ObjectId.Null;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable table = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                if (table.Has(blockName))
                {
                    id = table[blockName];
                }
                else
                {
                    BlockTableRecord blockDef = new BlockTableRecord();
                    blockDef.Name = blockName;

                    table.UpgradeOpen();
                    id = table.Add(blockDef);
                    tr.AddNewlyCreatedDBObject(blockDef, true);

                    double crossSize = 1;
                    double spacing = 0.5;
                    double th = 1;

                    Point3d pt = new Point3d(0, 0, 0);

                    Line line1 = AcadUtility.AcadEntity.CreateLine(db, pt - new Vector3d(crossSize, 0, 0), pt + new Vector3d(crossSize, 0, 0), lineLayerId);
                    Line line2 = AcadUtility.AcadEntity.CreateLine(db, pt - new Vector3d(0, crossSize, 0), pt + new Vector3d(0, crossSize, 0), lineLayerId);

                    AttributeDefinition text1 = AcadUtility.AcadEntity.CreateAttribute(db, pt + new Vector3d(crossSize + spacing, 0, 0), "Y", "Y", "0 000 000", th, 0, 1, TextHorizontalMode.TextLeft, TextVerticalMode.TextVerticalMid, textStyleId, textLayerId);
                    text1.LockPositionInBlock = true;
                    AttributeDefinition text2 = AcadUtility.AcadEntity.CreateAttribute(db, pt + new Vector3d(0, crossSize + spacing, 0), "X", "X", "0 000 000", th, Math.PI / 2, 1, TextHorizontalMode.TextLeft, TextVerticalMode.TextVerticalMid, textStyleId, textLayerId);
                    text2.LockPositionInBlock = true;

                    foreach (Entity ent in new Entity[] { line1, line2, text1, text2 })
                    {
                        blockDef.AppendEntity(ent);
                        tr.AddNewlyCreatedDBObject(ent, true);
                    }
                }

                tr.Commit();
            }

            return id;
        }

        public static double SignedTriangleArea(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            return Determinant(x1, y1, 1.0f, x2, y2, 1.0f, x3, y3, 1.0f) / 2.0f;
        }

        public static double Determinant(double a1, double a2, double a3, double b1, double b2, double b3, double c1, double c2, double c3)
        {
            return a1 * b2 * c3 - a1 * b3 * c2 - a2 * b1 * c3 + a2 * b3 * c1 + a3 * b1 * c2 - a3 * b2 * c1;
        }

        public bool PolylineContains(Point3d[] poly, Point3d v)
        {
            if (poly.Length < 3)
            {
                return false;
            }

            int signCheck = 0;
            for (int i = 0; i < poly.Length; i++)
            {
                int j = (i == poly.Length - 1 ? 0 : i + 1);
                double area = SignedTriangleArea(poly[i].X, poly[i].Y, poly[j].X, poly[j].Y, v.X, v.Y);

                int sign = Math.Sign(area);
                if (i == 0)
                {
                    signCheck = sign;
                }
                else if (sign != signCheck)
                {
                    return false;
                }
            }

            return true;
        }
    }
}