using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.Runtime;

namespace DrawingUtility
{
    public partial class DrawingUtility
    {
        private int CurveSegments { get; set; }
        private int Precision { get; set; }

        public DrawingUtility()
        {
            CurveSegments = 40;
            Precision = 2;
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("0", CommandFlags.UsePickSet)]
        public static void ChangeLayerToZero()
        {
            PromptSelectionResult selRes = SelectWithPickFirst();

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                if (selRes.Status == PromptStatus.OK)
                {
                    foreach (ObjectId id in selRes.Value.GetObjectIds())
                    {
                        Entity en = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                        if (en != null)
                        {
                            en.LayerId = db.LayerZero;
                        }
                    }
                }
                db.Clayer = db.LayerZero;

                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("BY", CommandFlags.UsePickSet)]
        public static void ChangeToByLayer()
        {
            PromptSelectionResult selRes = SelectWithPickFirst();

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                if (selRes.Status == PromptStatus.OK)
                {
                    foreach (ObjectId id in selRes.Value.GetObjectIds())
                    {
                        Entity en = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                        if (en != null)
                        {
                            en.Color = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                            en.LinetypeId = db.ByLayerLinetype;
                            en.LineWeight = LineWeight.ByLayer;
                        }
                    }
                }

                db.Cecolor = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                db.Celtype = db.ByLayerLinetype;
                db.Celweight = LineWeight.ByLayer;

                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("RD", CommandFlags.UsePickSet)]
        public static void Rotate90CCW()
        {
            PromptSelectionResult selRes = SelectWithPickFirst();
            if (selRes.Status != PromptStatus.OK) return;

            PromptPointResult ptRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nBase point: ");
            if (ptRes.Status != PromptStatus.OK) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Vector3d zAxis = db.Ucsxdir.CrossProduct(db.Ucsydir);
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, zAxis);
            Matrix3d trans = Matrix3d.Rotation(Math.PI / 2, zAxis, ptRes.Value.TransformBy(ucs2wcs));
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in selRes.Value.GetObjectIds())
                {
                    Entity en = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                    if (en != null)
                    {
                        en.TransformBy(trans);
                    }
                }
                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("RDD", CommandFlags.UsePickSet)]
        public static void Rotate90CW()
        {
            PromptSelectionResult selRes = SelectWithPickFirst();
            if (selRes.Status != PromptStatus.OK) return;

            PromptPointResult ptRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nBase point: ");
            if (ptRes.Status != PromptStatus.OK) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Vector3d zAxis = db.Ucsxdir.CrossProduct(db.Ucsydir);
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, zAxis);
            Matrix3d trans = Matrix3d.Rotation(-Math.PI / 2, zAxis, ptRes.Value.TransformBy(ucs2wcs));
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in selRes.Value.GetObjectIds())
                {
                    Entity en = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                    if (en != null)
                    {
                        en.TransformBy(trans);
                    }
                }
                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("RX", CommandFlags.UsePickSet)]
        public static void Rotate180()
        {
            PromptSelectionResult selRes = SelectWithPickFirst();
            if (selRes.Status != PromptStatus.OK) return;

            PromptPointResult ptRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nBase point: ");
            if (ptRes.Status != PromptStatus.OK) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Vector3d zAxis = db.Ucsxdir.CrossProduct(db.Ucsydir);
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, zAxis);
            Matrix3d trans = Matrix3d.Rotation(Math.PI, zAxis, ptRes.Value.TransformBy(ucs2wcs));
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in selRes.Value.GetObjectIds())
                {
                    Entity en = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                    if (en != null)
                    {
                        en.TransformBy(trans);
                    }
                }
                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("SF")]
        public static void ScaleFixed()
        {
            PromptDoubleResult scaleRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetDouble("\nScale factor: ");
            if (scaleRes.Status != PromptStatus.OK) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Vector3d zAxis = db.Ucsxdir.CrossProduct(db.Ucsydir);
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, zAxis);
            while (true)
            {
                PromptSelectionResult selRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection();
                if (selRes.Status != PromptStatus.OK) return;

                PromptPointResult ptRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("\nBase point: ");
                if (ptRes.Status != PromptStatus.OK) return;

                Matrix3d trans = Matrix3d.Scaling(scaleRes.Value, ptRes.Value.TransformBy(ucs2wcs));
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    foreach (ObjectId id in selRes.Value.GetObjectIds())
                    {
                        Entity en = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                        if (en != null)
                        {
                            en.TransformBy(trans);
                        }
                    }
                    tr.Commit();
                }
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("TOPLA")]
        public void AddTextValues()
        {
            PromptSelectionResult selRes = SelectWithPickFirst();
            if (selRes.Status != PromptStatus.OK) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                double sum = 0.0;
                foreach (ObjectId id in selRes.Value.GetObjectIds())
                {
                    DBText text = tr.GetObject(id, OpenMode.ForRead) as DBText;
                    if (text != null)
                    {
                        string str = text.TextString;
                        double val = 0.0;
                        if (double.TryParse(str, out val))
                        {
                            sum += val;
                        }
                    }
                }

                PromptEntityResult entRes;
                while (true)
                {
                    PromptEntityOptions opts = new PromptEntityOptions("\nSonucun yazılacağı yazı (" + sum.ToString("F" + Precision) + ") [Basamak sayısı]: ", "Precision");
                    entRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(opts);

                    if (entRes.Status == PromptStatus.Keyword && entRes.StringResult == "Precision")
                    {
                        PromptIntegerOptions intOpts = new PromptIntegerOptions("Basamak sayısı: ");
                        intOpts.AllowNone = true;
                        intOpts.AllowZero = true;
                        intOpts.AllowNegative = false;
                        intOpts.LowerLimit = 0;
                        intOpts.UpperLimit = 16;
                        intOpts.DefaultValue = Precision;
                        intOpts.UseDefaultValue = true;
                        PromptIntegerResult res = doc.Editor.GetInteger(intOpts);
                        if (res.Status == PromptStatus.Cancel)
                        {
                            return;
                        }
                        else if (res.Status == PromptStatus.OK)
                        {
                            Precision = res.Value;
                        }
                    }
                    else if (entRes.Status != PromptStatus.OK)
                    {
                        return;
                    }
                    else
                    {
                        break;
                    }
                }

                DBText restext = tr.GetObject(entRes.ObjectId, OpenMode.ForWrite) as DBText;
                if (restext != null)
                {
                    restext.TextString = sum.ToString("F" + Precision.ToString());
                }

                tr.Commit();
            }
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("CARP")]
        public void MultiplyTextValues()
        {
            PromptSelectionResult selRes = SelectWithPickFirst();
            if (selRes.Status != PromptStatus.OK) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                double sum = 1.0;
                foreach (ObjectId id in selRes.Value.GetObjectIds())
                {
                    DBText text = tr.GetObject(id, OpenMode.ForRead) as DBText;
                    if (text != null)
                    {
                        string str = text.TextString;
                        double val = 0.0;
                        if (double.TryParse(str, out val))
                        {
                            sum *= val;
                        }
                    }
                }

                PromptEntityResult entRes;
                while (true)
                {
                    PromptEntityOptions opts = new PromptEntityOptions("\nSonucun yazılacağı yazı (" + sum.ToString("F" + Precision) + ") [Basamak sayısı]: ", "Precision");
                    entRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(opts);

                    if (entRes.Status == PromptStatus.Keyword && entRes.StringResult == "Precision")
                    {
                        PromptIntegerOptions intOpts = new PromptIntegerOptions("\nBasamak sayısı: ");
                        intOpts.AllowNone = true;
                        intOpts.AllowZero = true;
                        intOpts.AllowNegative = false;
                        intOpts.LowerLimit = 0;
                        intOpts.UpperLimit = 16;
                        intOpts.DefaultValue = Precision;
                        intOpts.UseDefaultValue = true;
                        PromptIntegerResult res = doc.Editor.GetInteger(intOpts);
                        if (res.Status == PromptStatus.Cancel)
                        {
                            return;
                        }
                        else if (res.Status == PromptStatus.OK)
                        {
                            Precision = res.Value;
                        }
                    }
                    else if (entRes.Status != PromptStatus.OK)
                    {
                        return;
                    }
                    else
                    {
                        break;
                    }
                }

                DBText restext = tr.GetObject(entRes.ObjectId, OpenMode.ForWrite) as DBText;
                if (restext != null)
                {
                    restext.TextString = sum.ToString("F" + Precision.ToString());
                }

                tr.Commit();
            }
        }

        private static PromptSelectionResult SelectWithPickFirst()
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult selRes = ed.SelectImplied();
            if (selRes.Status == PromptStatus.Error)
            {
                selRes = ed.GetSelection();
            }
            else
            {
                ed.SetImpliedSelection(new ObjectId[0]);
            }

            return selRes;
        }
    }
}
