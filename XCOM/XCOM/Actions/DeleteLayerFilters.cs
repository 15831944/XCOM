using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.LayerManager;

namespace XCOM.Commands.XCommand
{
    public class DeleteLayerFilters : IXCOMAction
    {
        public string Name { get { return "Layer Filtrelerini Sil"; } }
        public int Order { get { return 100; } }
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
                    LayerFilterTree lft = db.LayerFilters;
                    LayerFilterCollection lfc = lft.Root.NestedFilters;
                    List<LayerFilter> todelete = new List<LayerFilter>();

                    foreach (LayerFilter lf in lfc)
                    {
                        if (lf.AllowDelete) todelete.Add(lf);
                    }
                    foreach (LayerFilter lf in todelete)
                    {
                        lfc.Remove(lf);
                    }

                    db.LayerFilters = lft;
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
            EventHandler<ActionProgressEventArgs> handler = Progress;
            if (handler != null)
            {
                handler(this, new ActionProgressEventArgs(message));
            }
        }

        protected void OnError(Exception error)
        {
            EventHandler<ActionErrorEventArgs> handler = Error;
            if (handler != null)
            {
                handler(this, new ActionErrorEventArgs(error));
            }
        }
    }
}