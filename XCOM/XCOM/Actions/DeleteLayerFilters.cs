using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.LayerManager;

namespace XCOM.Commands.XCommand
{
    public class DeleteLayerFilters : XCOMActionBase
    {
        public override string Name { get { return "Layer Filtrelerini Sil"; } }
        public override int Order { get { return 100; } }

        public override void Run(string filename, Database db)
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
    }
}