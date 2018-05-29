using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM.Commands.XCommand
{
    public class HideGrids : XCOMActionBase
    {
        public override string Name { get { return "Grid'leri Gizle"; } }
        public override int Order { get { return 152000; } }
        public override bool Recommended { get { return true; } }

        public override void Run(string filename, Database db)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    DBDictionary layoutDict = (DBDictionary)tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead);
                    foreach (DBDictionaryEntry entry in layoutDict)
                    {
                        if (entry.Key.ToUpperInvariant() != "MODEL")
                        {
                            Layout layout = (Layout)tr.GetObject(entry.Value, OpenMode.ForRead);
                            foreach (ObjectId vpId in layout.GetViewports())
                            {
                                Viewport vp = (Viewport)tr.GetObject(vpId, OpenMode.ForWrite);
                                vp.GridOn = false;
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    OnError(ex);
                }

                tr.Commit();
            }
        }
    }
}