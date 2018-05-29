using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace XCOM.Commands.XCommand
{
    public class ChangeToLayout : XCOMActionBase
    {
        public override string Name { get { return "Layout'a Geç"; } }
        public override int Order { get { return 150000; } }

        public override void Run(string filename, Database db)
        {
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
                catch (Exception ex)
                {
                    OnError(ex);
                }

                tr.Commit();
            }
        }
    }
}