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
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Matrix3d ucs2wcs = AcadUtility.AcadGraphics.UcsToWcs;
            Matrix3d wcs2ucs = AcadUtility.AcadGraphics.WcsToUcs;

            using (NumberingForm form = new NumberingForm())
            {
                // Read settings
                form.SelectObjects = (NumberingForm.SelectNumberingObjects)Properties.Settings.Default.Command_NUMARALANDIR_SelectObjects;
                form.AttributeName = Properties.Settings.Default.Command_NUMARALANDIR_AttributeName;
                form.Ordering = (NumberingForm.CoordinateOrdering)Properties.Settings.Default.Command_NUMARALANDIR_Order;
                form.Prefix = Properties.Settings.Default.Command_NUMARALANDIR_Prefix;
                form.StartNumber = Properties.Settings.Default.Command_NUMARALANDIR_StartNumber;
                form.Increment = Properties.Settings.Default.Command_NUMARALANDIR_Increment;
                form.Format = Properties.Settings.Default.Command_NUMARALANDIR_Format;
                form.Suffix = Properties.Settings.Default.Command_NUMARALANDIR_Suffix;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    // Save changes
                    Properties.Settings.Default.Command_NUMARALANDIR_SelectObjects = (int)form.SelectObjects;
                    Properties.Settings.Default.Command_NUMARALANDIR_AttributeName = form.AttributeName;
                    Properties.Settings.Default.Command_NUMARALANDIR_Order = (int)form.Ordering;
                    Properties.Settings.Default.Command_NUMARALANDIR_Prefix = form.Prefix;
                    Properties.Settings.Default.Command_NUMARALANDIR_StartNumber = form.StartNumber;
                    Properties.Settings.Default.Command_NUMARALANDIR_Increment = form.Increment;
                    Properties.Settings.Default.Command_NUMARALANDIR_Format = form.Format;
                    Properties.Settings.Default.Command_NUMARALANDIR_Suffix = form.Suffix;

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
                                // Read objects
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
                                // Sort items by coordinates
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
                                // Write numbering text
                                double num = form.StartNumber;
                                string format = form.Format;
                                foreach (Tuple<ObjectId, Point3d> item in items)
                                {
                                    bool found = false;
                                    ObjectId id = item.Item1;
                                    string numstr = num.ToString(format);
                                    string text = form.Prefix + numstr + form.Suffix;
                                    if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(DBText)).UnmanagedObject)
                                    {
                                        DBText obj = tr.GetObject(id, OpenMode.ForWrite) as DBText;
                                        obj.TextString = text;
                                        found = true;
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(MText)).UnmanagedObject)
                                    {
                                        MText obj = tr.GetObject(id, OpenMode.ForWrite) as MText;
                                        obj.Contents = text;
                                        found = true;
                                    }
                                    else if (id.ObjectClass.UnmanagedObject == RXClass.GetClass(typeof(BlockReference)).UnmanagedObject)
                                    {
                                        BlockReference obj = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                                        foreach (ObjectId attId in obj.AttributeCollection)
                                        {
                                            AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                                            if (string.Compare(attRef.Tag, form.AttributeName, StringComparison.OrdinalIgnoreCase) == 0)
                                            {
                                                attRef.UpgradeOpen();
                                                attRef.TextString = text;
                                                found = true;
                                            }
                                        }
                                    }
                                    if (found)
                                    {
                                        // Only increment if a change was made
                                        num += form.Increment;
                                    }
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
}