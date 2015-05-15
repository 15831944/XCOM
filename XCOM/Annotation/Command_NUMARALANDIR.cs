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

namespace XCOM.Commands.Annotation
{
    public partial class Command_NUMARALANDIR
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("NUMARALANDIR", CommandFlags.UsePickSet)]
        public void Numbering()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Matrix3d ucs2wcs = XCOM.Utility.Graphics.UcsToWcs();
            Matrix3d wcs2ucs = XCOM.Utility.Graphics.WcsToUcs();

            NumberingForm form = new NumberingForm();
            if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
            {
                // Select objects
                List<TypedValue> tvs = new List<TypedValue>();
                switch (form.SelectObjects)
                {
                    case NumberingForm.SelectNumberingObjects.Text:
                        tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
                        tvs.Add(new TypedValue((int)DxfCode.Start, "TEXT"));
                        tvs.Add(new TypedValue((int)DxfCode.Start, "MTEXT"));
                        tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
                        break;
                    default:
                        tvs.Add(new TypedValue((int)DxfCode.Operator, "<OR"));
                        tvs.Add(new TypedValue((int)DxfCode.Start, "INSERT"));
                        tvs.Add(new TypedValue((int)DxfCode.Operator, "OR>"));
                        break;
                }
                SelectionFilter filter = new SelectionFilter(tvs.ToArray());

                PromptSelectionResult selRes = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection(filter);
                if (selRes.Status == PromptStatus.OK)
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        try
                        {
                            List<Tuple<ObjectId, Point3d>> items = new List<Tuple<ObjectId, Point3d>>();
                            // Read object coordinates
                            foreach (ObjectId id in selRes.Value.GetObjectIds())
                            {
                                if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBText)).UnmanagedObject)
                                {
                                    DBText obj = tr.GetObject(id, OpenMode.ForRead) as DBText;
                                    items.Add(new Tuple<ObjectId, Point3d>(id, obj.Position.TransformBy(wcs2ucs)));
                                }
                                else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(MText)).UnmanagedObject)
                                {
                                    MText obj = tr.GetObject(id, OpenMode.ForRead) as MText;
                                    items.Add(new Tuple<ObjectId, Point3d>(id, obj.Location.TransformBy(wcs2ucs)));
                                }
                                else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(BlockReference)).UnmanagedObject)
                                {
                                    BlockReference obj = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                                    items.Add(new Tuple<ObjectId, Point3d>(id, obj.Position.TransformBy(wcs2ucs)));
                                }
                            }
                            // Sort coordinates
                            items.Sort((p1, p2) =>
                            {
                                switch (form.Ordering)
                                {
                                    case NumberingForm.CoordinateOrdering.IncreasingX:
                                        return (p1.Item2.X < p2.Item2.X ? -1 : 1);
                                    case NumberingForm.CoordinateOrdering.IncreasingY:
                                        return (p1.Item2.Y < p2.Item2.Y ? -1 : 1);
                                    case NumberingForm.CoordinateOrdering.DecreasingX:
                                        return (p1.Item2.X > p2.Item2.X ? -1 : 1);
                                    case NumberingForm.CoordinateOrdering.DecreasingY:
                                        return (p1.Item2.Y > p2.Item2.Y ? -1 : 1);
                                    default:
                                        return 0;
                                }
                            });
                            // Write coordinates
                            int num = form.StartNumber;
                            int digits = form.Digits;
                            foreach (Tuple<ObjectId, Point3d> item in items)
                            {
                                ObjectId id = item.Item1;
                                string numstr = num.ToString();
                                while (numstr.Length < digits) numstr = "0" + numstr;
                                string text = form.Prefix + numstr + form.Suffix;
                                if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBText)).UnmanagedObject)
                                {
                                    DBText obj = tr.GetObject(id, OpenMode.ForWrite) as DBText;
                                    obj.TextString = text;
                                }
                                else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(MText)).UnmanagedObject)
                                {
                                    MText obj = tr.GetObject(id, OpenMode.ForWrite) as MText;
                                    obj.Contents = text;
                                }
                                else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(BlockReference)).UnmanagedObject)
                                {
                                    BlockReference obj = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                                    AttributeReference attRef = (AttributeReference)tr.GetObject(obj.AttributeCollection[0], OpenMode.ForWrite);
                                    attRef.TextString = text;
                                }
                                num += form.Increment;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        tr.Commit();
                    }
                }
            }
        }
    }
}