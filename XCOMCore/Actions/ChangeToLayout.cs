using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOMCore
{
    public class ChangeToLayout : IXCOMAction
    {
        public string Name { get { return "Layout'a Geç"; } }
        public int Order { get { return 150000; } }
        public bool Recommended { get { return false; } }
        public ActionInterface Interface { get { return ActionInterface.Command; } }
        public bool ShowDialog() { return true; }
 
        public override string ToString()
        {
            return Name;
        }

        public string[] Run(string filename, Database db)
        {
            List<string> errors = new List<string>();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    if (db.TileMode)
                    {
                        DBDictionary layoutDictionary = (DBDictionary)tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead);
                        if (layoutDictionary.Count != 0)
                        {
                            foreach (DBDictionaryEntry entry in layoutDictionary)
                            {
                                Layout layout = tr.GetObject(entry.Value, OpenMode.ForRead, false) as Layout;
                                if (layout == null || layout.LayoutName == "Model" || layout.GetViewports().Count == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    db.TileMode = false;

                                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite);
                                    btr.LayoutId = layout.Id;

                                    break;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    errors.Add(ex.Message);
                }

                tr.Commit();
            }

            return errors.ToArray();
        }
    }
}