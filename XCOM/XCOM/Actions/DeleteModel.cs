using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace XCOM.Commands.XCommand
{
    public class DeleteModel : IXCOMAction
    {
        public string Name { get { return "Delete Model Space"; } }
        public int Order { get { return 300; } }
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
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);
                    foreach (ObjectId id in btr)
                    {
                        DBObject obj = tr.GetObject(id, OpenMode.ForWrite);
                        obj.Erase(true);
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