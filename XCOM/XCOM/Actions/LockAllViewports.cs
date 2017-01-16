using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace XCOM.Commands.XCommand
{
    public class LockAllViewports : XCOMActionBase
    {
        public override string Name { get { return "Viewport'ları Kilitle"; } }
        public override int Order { get { return 151000; } }
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
                                vp.Locked = true;
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