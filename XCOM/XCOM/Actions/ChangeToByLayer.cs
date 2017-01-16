using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Colors;

namespace XCOM.Commands.XCommand
{
    public class ChangeToByLayer : XCOMActionBase
    {
        public override string Name { get { return "Layer 0 + ByLayer"; } }
        public override int Order { get { return 100000; } }
        public override bool Recommended { get { return true; } }

        public override void Run(string filename, Database db)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // Set the current layer to "0" and all drawing settings to BYLAYER
                    db.Clayer = db.LayerZero;
                    db.Cecolor = Color.FromColorIndex(ColorMethod.ByLayer, 256);
                    db.Celtype = db.ByLayerLinetype;
                    db.Celweight = LineWeight.ByLayer;
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