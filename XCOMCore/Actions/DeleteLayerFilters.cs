using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.LayerManager;

namespace XCOMCore
{
    public class DeleteLayerFilters : IXCOMAction
    {
        public string Name { get { return "Layer Filtrelerini Sil"; } }
        public int Order { get { return 100; } }
        public bool Recommended { get { return true; } }
        public ActionInterface Interface { get { return ActionInterface.Command; } }
        public bool ShowDialog() { return true; }

        public override string ToString()
        {
            return Name;
        }

        public string[] Run(string filename, Database db)
        {
            List<string> errors = new List<string>();

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

                    tr.Commit();

                }
                catch (System.Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return errors.ToArray();
        }
    }
}