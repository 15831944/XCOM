using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;

namespace XCOMCore
{
    public class ChangeToByLayer : IXCOMAction
    {
        public string Name { get { return "Layer 0 + ByLayer"; } }
        public int Order { get { return 100000; } }
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
                    // Set the current layer to "0" and all drawing settings to BYLAYER
                    db.Clayer = db.LayerZero;
                    db.Cecolor = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                    db.Celtype = db.ByLayerLinetype;
                    db.Celweight = LineWeight.ByLayer;

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