using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace XCOM.Commands.XCommand
{
    public class HideGrids : IXCOMAction
    {
        public string Name { get { return "Grid'leri Gizle"; } }
        public int Order { get { return 152000; } }
        public bool Recommended { get { return true; } }
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

        public event EventHandler<ActionProgressEventArgs> Progress;
        public event EventHandler<ActionErrorEventArgs> Error;

        protected void OnProgress(string message)
        {
            Progress?.Invoke(this, new ActionProgressEventArgs(message));
        }

        protected void OnError(System.Exception error)
        {
            Error?.Invoke(this, new ActionErrorEventArgs(error));
        }
    }
}