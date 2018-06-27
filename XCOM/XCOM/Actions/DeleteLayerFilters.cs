using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.LayerManager;
using System;
using System.Collections.Generic;

namespace XCOM.Commands.XCommand
{
    public class DeleteLayerFilters : XCOMActionBase
    {
        public override string Name => "Layer Filtrelerini Sil";
        public override int Order => 100;
        public override string HelpText => "Tüm layer filtrelerini siler.";

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