using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace RebarPosCommands
{
    public static class DWGUtility
    {
        public class PromptRebarSelectionResult
        {
            public PromptStatus Status { get; private set; }
            public SelectionSet Value { get; private set; }

            public PromptRebarSelectionResult(PromptStatus s, SelectionSet v)
            {
                Status = s;
                Value = v;
            }
        }

        private static SelectionFilter SSPosFilter(bool includeDetached)
        {
            if (includeDetached)
            {
                TypedValue[] tvs = new TypedValue[] {
                    new TypedValue((int)DxfCode.Start, "INSERT")
                };
                return new SelectionFilter(tvs);
            }
            else
            {
                TypedValue[] tvs = new TypedValue[] {
                    new TypedValue((int)DxfCode.Start, "INSERT")
                };
                return new SelectionFilter(tvs);
            }
        }

        private static SelectionFilter SSPosFilter()
        {
            return SSPosFilter(false);
        }

        public static PromptRebarSelectionResult SelectAllPosUser(bool includeDetached)
        {
            try
            {
                PromptSelectionResult res = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection(SSPosFilter(includeDetached));
                if (res.Status == PromptStatus.OK)
                {
                    IEnumerable<ObjectId> idList = FilterBlocks(res.Value.GetObjectIds());
                    SelectionSet set = SelectionSet.FromObjectIds(new List<ObjectId>(idList).ToArray());
                    return new PromptRebarSelectionResult(res.Status, set);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return new PromptRebarSelectionResult(PromptStatus.Error, SelectionSet.FromObjectIds(new ObjectId[0]));
        }

        public static PromptRebarSelectionResult SelectAllPosUser()
        {
            return SelectAllPosUser(false);
        }

        private static IEnumerable<ObjectId> FilterBlocks(IEnumerable<ObjectId> idList)
        {
            List<ObjectId> result = new List<ObjectId>();

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in idList)
                    {
                        BlockReference blockRef = tr.GetObject(id, OpenMode.ForRead) as BlockReference;

                        BlockTableRecord block = null;
                        if (blockRef.IsDynamicBlock)
                        {
                            block = tr.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        }
                        else
                        {
                            block = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                        }

                        if (string.Compare(block.Name, MyCommands.BlockName, StringComparison.OrdinalIgnoreCase) == 0) result.Add(id);
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return result;
        }

        public static IEnumerable<ObjectId> GetAllPos()
        {
            List<ObjectId> list = new List<ObjectId>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    using (BlockTableRecordEnumerator it = btr.GetEnumerator())
                    {
                        while (it.MoveNext())
                        {
                            if (it.Current.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(BlockReference)))
                            {
                                list.Add(it.Current);
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return FilterBlocks(list);
        }

        public static IEnumerable<ObjectId> GetPosWithShape(string shape)
        {
            List<ObjectId> list = new List<ObjectId>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    using (BlockTableRecordEnumerator it = btr.GetEnumerator())
                    {
                        while (it.MoveNext())
                        {
                            RebarPos pos = RebarPos.FromObjectId(tr, it.Current);
                            if (pos != null)
                            {
                                if (pos.ShapeName == shape)
                                {
                                    list.Add(it.Current);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return list;
        }

        // Returns the maximum pos number within the given pos blocks.
        public static int GetMaximumPosNumber(IEnumerable<ObjectId> ids)
        {
            int num = -1;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in ids)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, id);
                        if (pos != null)
                        {
                            int i = -1;
                            if (int.TryParse(pos.Pos, out i))
                            {
                                num = Math.Max(i, num);
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return num;
        }

        // Refreshes given items
        public static void RefreshPos(IEnumerable<ObjectId> ids)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId posid in ids)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, posid);
                        pos.UpdateProperties();
                        pos.Save(tr);
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }
        }

        // Refreshes all items with the given shape
        public static void RefreshPosWithShape(string name)
        {
            RefreshPos(GetPosWithShape(name));
        }

        // Refreshes all items
        public static void RefreshAllPos()
        {
            RefreshPos(GetAllPos());
        }

        // Returns the list of standard diamaters
        public static List<int> GetStandardDiameters()
        {
            return PosGroup.Current.StandardDiameters;
        }

        // Returns the maximum bar length in m
        public static double GetMaximumBarLength()
        {
            return PosGroup.Current.MaxBarLength;
        }

        public static void DrawShape(Database db, string name, bool showInvisible, Point3d inspt, double height, double rotation)
        {
            DrawShape(db, PosShape.Shapes[name], showInvisible, inspt, height, rotation);
        }

        public static void DrawShape(Database db, PosShape shape, bool showInvisible, Point3d inspt, double height, double rotation)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    IEnumerable<Entity> items = shape.ToEntitites(db, inspt, height, rotation, showInvisible);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                    foreach (Entity en in items)
                    {
                        btr.AppendEntity(en);
                        tr.AddNewlyCreatedDBObject(en, true);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: " + ex.ToString(), "RebarPos", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }
    }
}
