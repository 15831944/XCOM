using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using Autodesk.AutoCAD.EditorInput;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private bool DrawBOQ()
        {
            using (DrawBOQForm form = new DrawBOQForm())
            {
                // Pos error check
                DWGUtility.PromptRebarSelectionResult sel = DWGUtility.SelectAllPosUser();
                if (sel.Status != PromptStatus.OK) return false;
                ObjectId[] items = sel.Value.GetObjectIds();

                List<PosCheckResult> errors = PosCheckResult.CheckAllInSelection(items, true, false);
                List<PosCheckResult> warnings = PosCheckResult.CheckAllInSelection(items, false, true);

                if (errors.Count != 0) PosCheckResult.ConsoleOut(errors);
                if (warnings.Count != 0) PosCheckResult.ConsoleOut(warnings);

                if (errors.Count != 0)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.DisplayTextScreen = true;
                    return false;
                }

                // Pos similarity check
                if (warnings.Count != 0)
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.DisplayTextScreen = true;
                    PromptKeywordOptions opts = new PromptKeywordOptions("\nMetraja devam edilsin mi? [Evet/Hayir]", "Yes No");
                    PromptResult res = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetKeywords(opts);
                    if (res.Status != PromptStatus.OK || res.StringResult == "No")
                    {
                        return true;
                    }
                }

                if (!form.Init())
                    return false;

                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) != System.Windows.Forms.DialogResult.OK)
                    return true;

                List<PosCopy> posList = new List<PosCopy>();
                try
                {
                    posList = PosCopy.ReadAllInSelection(items, true, PosCopy.PosGrouping.PosMarker);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (posList.Count == 0)
                {
                    MessageBox.Show("Seçilen grupta poz mevcut değil.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                posList = RemoveEmpty(posList);
                if (!form.HideMissing)
                {
                    posList = AddMissing(posList);
                }
                posList = SortList(posList);

                PromptPointResult result = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetPoint("Baz noktası: ");
                if (result.Status != PromptStatus.OK)
                    return true;

                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                {
                    try
                    {
                        double lengthScale = 1.0;

                        // Create table
                        Table tb = new Table();
                        tb.TableStyle = db.Tablestyle;
                        tb.SetSize(posList.Count, 3);
                        tb.SetRowHeight(3);
                        tb.SetColumnWidth(15);
                        tb.Position = result.Value;

                        // Add rows
                        int i = 0;
                        foreach (PosCopy copy in posList)
                        {
                            if (copy.existing)
                            {
                                string a = string.Empty;
                                string b = string.Empty;
                                string c = string.Empty;
                                string d = string.Empty;
                                string e = string.Empty;
                                string f = string.Empty;

                                if (copy.isVarA)
                                    a = (copy.minA * lengthScale).ToString("F0") + "~" + (copy.maxA * lengthScale).ToString("F0");
                                else
                                    a = (copy.minA * lengthScale).ToString("F0");
                                if (copy.isVarB)
                                    b = (copy.minB * lengthScale).ToString("F0") + "~" + (copy.maxB * lengthScale).ToString("F0");
                                else
                                    b = (copy.minB * lengthScale).ToString("F0");
                                if (copy.isVarC)
                                    c = (copy.minC * lengthScale).ToString("F0") + "~" + (copy.maxC * lengthScale).ToString("F0");
                                else
                                    c = (copy.minC * lengthScale).ToString("F0");
                                if (copy.isVarD)
                                    d = (copy.minD * lengthScale).ToString("F0") + "~" + (copy.maxD * lengthScale).ToString("F0");
                                else
                                    d = (copy.minD * lengthScale).ToString("F0");
                                if (copy.isVarE)
                                    e = (copy.minE * lengthScale).ToString("F0") + "~" + (copy.maxE * lengthScale).ToString("F0");
                                else
                                    e = (copy.minE * lengthScale).ToString("F0");
                                if (copy.isVarF)
                                    f = (copy.minF * lengthScale).ToString("F0") + "~" + (copy.maxF * lengthScale).ToString("F0");
                                else
                                    f = (copy.minF * lengthScale).ToString("F0");

                                // rows.Add(new BOQRow(int.Parse(copy.pos), copy.count, double.Parse(copy.diameter), copy.length1, copy.length2, copy.isVarLength, copy.shapename, a, b, c, d, e, f));

                                tb.Cells[i, 0].TextString = copy.pos;
                                tb.Cells[i, 0].TextHeight = 1;
                                tb.Cells[i, 0].Alignment = CellAlignment.MiddleCenter;
                                i++;
                            }
                            else
                            {
                                // rows.Add(new BOQRow(int.Parse(copy.pos)));
                            }
                        }

                        // Add table to db
                        btr.AppendEntity(tb);
                        tr.AddNewlyCreatedDBObject(tb, true);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    tr.Commit();
                }
            }

            return true;
        }

        private List<PosCopy> AddMissing(List<PosCopy> list)
        {
            list = RemoveEmpty(list);

            int lastpos = 0;
            foreach (PosCopy copy in list)
            {
                int posno;
                if (int.TryParse(copy.pos, out posno))
                {
                    lastpos = Math.Max(lastpos, posno);
                }
            }
            for (int i = 1; i <= lastpos; i++)
            {
                if (!list.Exists(p => p.pos == i.ToString()))
                {
                    PosCopy copy = new PosCopy();
                    copy.pos = i.ToString();
                    list.Add(copy);
                }
            }

            return SortList(list);
        }

        private List<PosCopy> RemoveEmpty(List<PosCopy> list)
        {
            list.RemoveAll(p => p.existing == false);
            return list;
        }

        private List<PosCopy> SortList(List<PosCopy> list)
        {
            list.Sort(new CompareByPosNumber());
            return list;
        }

        private class CompareByPosNumber : IComparer<PosCopy>
        {
            public int Compare(PosCopy e1, PosCopy e2)
            {
                int p1 = 0;
                int p2 = 0;
                int.TryParse(e1.pos, out p1);
                int.TryParse(e2.pos, out p2);

                return (p1 == p2 ? 0 : (p1 < p2 ? -1 : 1));
            }
        }
    }
}
