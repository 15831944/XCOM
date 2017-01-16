using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM.Commands.XCommand
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

        public void Run(string filename, Database db)
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

        public event EventHandler<ActionProgressEventArgs> Progress;
        public event EventHandler<ActionErrorEventArgs> Error;

        protected void OnProgress(string message)
        {
            Progress?.Invoke(this, new ActionProgressEventArgs(message));
        }

        protected void OnError(Exception error)
        {
            Error?.Invoke(this, new ActionErrorEventArgs(error));
        }
    }
}