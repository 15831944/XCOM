using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM.Commands.XCommand
{
    public class HideGrids : XCOMActionBase
    {
        public override string Name => "Grid'leri Gizle";
        public override int Order => 152000;
        public override bool Recommended => true;
        public override string HelpText => "Grid çizgilerini gizler.";

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