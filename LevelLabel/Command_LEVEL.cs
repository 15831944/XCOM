using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using System.Windows.Forms;

namespace LevelLabel
{
    public class Command_LEVEL
    {
        private bool init;

        private enum Units
        {
            Meters,
            Centimeters,
            Millimeters
        }

        private Units DrawingUnit { get; set; }
        private double Scale
        {
            get
            {
                switch (DrawingUnit)
                {
                    case Units.Centimeters:
                        return 0.01;
                    case Units.Millimeters:
                        return 0.001;
                    case Units.Meters:
                        return 1.0;
                    default:
                        return 1.0;
                }
            }
        }
        private double BaseLevel { get; set; }
        private Point3d BasePoint { get; set; }
        private int Precision { get; set; }
        private string BlockName { get; set; }
        private double BlockScale { get; set; }

        public Command_LEVEL()
        {
            init = false;

            DrawingUnit = Units.Meters;
            BaseLevel = 0;
            BasePoint = Point3d.Origin;
            Precision = 2;
            BlockName = "LEVEL";
            BlockScale = 1.0;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("LEVEL")]
        public void LevelLabel()
        {
            if (!init)
            {
                if (!ShowSettings()) return;
            }

            while (true)
            {
                PromptPointOptions ptOpts = new PromptPointOptions("\nKot noktası: [Reset/Güncelle]", "Reset Update");
                PromptPointResult ptRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(ptOpts);
                if (ptRes.Status == PromptStatus.Cancel)
                {
                    return;
                }
                else if (ptRes.Status == PromptStatus.Keyword && ptRes.StringResult == "Reset")
                {
                    Reset();
                    return;
                }
                else if (ptRes.Status == PromptStatus.Keyword && ptRes.StringResult == "Update")
                {
                    PromptSelectionResult selRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection();
                    if (selRes.Status != PromptStatus.OK) return;

                    UpdateLevelBlocks(selRes.Value.GetObjectIds());
                    return;
                }

                PromptPointOptions textOpts = new PromptPointOptions("\nYazı noktası: ");
                textOpts.BasePoint = ptRes.Value;
                textOpts.UseBasePoint = true;
                PromptPointResult textRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(textOpts);
                if (textRes.Status != PromptStatus.OK) return;

                Point3d pt = ptRes.Value;
                Point3d ptt = new Point3d(textRes.Value.X, pt.Y, pt.Z);
                string level = GetLevel(pt);

                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
                Vector3d zAxis = db.Ucsxdir.CrossProduct(db.Ucsydir);
                Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, zAxis);
                double rotation = Math.Atan2(db.Ucsxdir.Y, db.Ucsxdir.X);

                Point3d ptw = pt.TransformBy(ucs2wcs);
                Point3d pttw = ptt.TransformBy(ucs2wcs);

                ObjectId blockId = ObjectId.Null;
                bool needsMirror = false;

                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    Line line = new Line();
                    line.StartPoint = ptw;
                    line.EndPoint = pttw;

                    btr.AppendEntity(line);
                    tr.AddNewlyCreatedDBObject(line, true);

                    BlockTable bt = db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;
                    if (bt.Has(BlockName))
                    {
                        BlockTableRecord blockDef = bt[BlockName].GetObject(OpenMode.ForRead) as BlockTableRecord;

                        BlockReference bref = new BlockReference(pttw, blockDef.ObjectId);
                        bref.Rotation = rotation;
                        bref.ScaleFactors = new Scale3d(BlockScale);

                        blockId = btr.AppendEntity(bref);
                        tr.AddNewlyCreatedDBObject(bref, true);

                        foreach (ObjectId id in blockDef)
                        {
                            AttributeDefinition attDef = tr.GetObject(id, OpenMode.ForRead) as AttributeDefinition;
                            if ((attDef != null) && (!attDef.Constant))
                            {
                                //Create a new AttributeReference
                                using (AttributeReference attRef = new AttributeReference())
                                {
                                    attRef.SetAttributeFromBlock(attDef, bref.BlockTransform);
                                    attRef.TextString = "";
                                    bref.AttributeCollection.AppendAttribute(attRef);
                                    tr.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }
                        if (ptt.X < pt.X)
                        {
                            needsMirror = true;
                        }

                        UpdateBlock(tr, db, bref, level);
                    }
                    else
                    {
                        MessageBox.Show("Kot bloğu '" + BlockName + "' bulunamadı.", "Level", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    tr.Commit();
                }

                if (needsMirror) MirrorBlock(blockId);
            }
        }

        private string GetLevel(Point3d ucsPt)
        {
            double level = (ucsPt.Y - BasePoint.Y) * Scale + BaseLevel;

            string format = (Precision == 0 ? "0" : "0." + new string('0', Precision));

            string str = Math.Abs(level).ToString(format);

            if (str == format)
                str = "%%p" + str;
            else if (level < 0)
                str = "-" + str;
            else
                str = "+" + str;

            return "%%U" + str;
        }

        private void UpdateLevelBlocks(ObjectId[] items)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            Vector3d zAxis = db.Ucsxdir.CrossProduct(db.Ucsydir);
            Matrix3d wcs2ucs = Matrix3d.AlignCoordinateSystem(db.Ucsorg, db.Ucsxdir, db.Ucsydir, zAxis, Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in items)
                {
                    BlockReference bref = tr.GetObject(id, OpenMode.ForWrite) as BlockReference;
                    if (bref != null && string.Compare(bref.Name, BlockName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        string level = GetLevel(bref.Position.TransformBy(wcs2ucs));
                        UpdateBlock(tr, db, bref, level);

                        AttributeReference attRef = (AttributeReference)tr.GetObject(bref.AttributeCollection[0], OpenMode.ForWrite);
                    }
                }
                tr.Commit();
            }
        }

        private void MirrorBlock(ObjectId id)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            Vector3d zAxis = db.Ucsxdir.CrossProduct(db.Ucsydir);
            Matrix3d wcs2ucs = Matrix3d.AlignCoordinateSystem(db.Ucsorg, db.Ucsxdir, db.Ucsydir, zAxis, Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis);

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                BlockReference bRef = tr.GetObject(id, OpenMode.ForWrite) as BlockReference;
                if (bRef != null && string.Compare(bRef.Name, BlockName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    for (int i = 0; i < bRef.AttributeCollection.Count; i++)
                    {
                        AttributeReference attRef = (AttributeReference)tr.GetObject(bRef.AttributeCollection[i], OpenMode.ForWrite);
                        Extents3d ex = attRef.GeometricExtents;
                        Point3d midPoint = new Point3d((ex.MinPoint.X + ex.MaxPoint.X) / 2, (ex.MinPoint.Y + ex.MaxPoint.Y) / 2, (ex.MinPoint.Z + ex.MaxPoint.Z) / 2);
                        using (Line3d mirrorLine = new Line3d(midPoint, midPoint + db.Ucsydir))
                        {
                            Matrix3d mirroring = Matrix3d.Mirroring(mirrorLine);
                            attRef.TransformBy(mirroring);
                        }
                    }
                }

                using (Line3d mirrorLine = new Line3d(bRef.Position, bRef.Position + db.Ucsydir))
                {
                    Matrix3d mirroring = Matrix3d.Mirroring(mirrorLine);
                    bRef.ScaleFactors = new Scale3d(-BlockScale, BlockScale, BlockScale);
                    bRef.TransformBy(mirroring);
                }

                tr.Commit();
            }
        }

        private void Reset()
        {
            init = false;
        }

        private void UpdateBlock(Transaction tr, Database db, BlockReference bref, string level)
        {
            AttributeReference attRef = (AttributeReference)tr.GetObject(bref.AttributeCollection[0], OpenMode.ForWrite);
            attRef.TextString = level;
        }

        private bool ShowSettings()
        {
            LevelMainForm form = new LevelMainForm();

            form.UnitMeter = (DrawingUnit == Units.Meters);
            form.UnitCentimeter = (DrawingUnit == Units.Centimeters);
            form.UnitMillimeter = (DrawingUnit == Units.Millimeters);

            form.Precision = Precision;

            form.BasePoint = BasePoint;
            form.BaseLevel = BaseLevel;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            List<string> blockNames = new List<string>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
            {
                foreach (ObjectId id in bt)
                {
                    BlockTableRecord block = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);

                    if (string.Compare(block.Name, BlockTableRecord.ModelSpace, StringComparison.OrdinalIgnoreCase) == 0) continue;
                    if (block.IsLayout) continue;

                    blockNames.Add(block.Name);
                }
            }
            form.SetBlockNames(blockNames.ToArray());
            form.BlockName = BlockName;
            form.BlockScale = BlockScale;

            if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
            {
                if (form.UnitMeter)
                    DrawingUnit = Units.Meters;
                else if (form.UnitCentimeter)
                    DrawingUnit = Units.Centimeters;
                else if (form.UnitMillimeter)
                    DrawingUnit = Units.Millimeters;

                Precision = form.Precision;

                BasePoint = form.BasePoint;
                BaseLevel = form.BaseLevel;

                BlockName = form.BlockName;
                BlockScale = form.BlockScale;

                init = true;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
