using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace XCOM.Commands.XCommand
{
    public class ChangeToByLayer : XCOMActionBase
    {
        public override string Name => "Layer 0 + ByLayer";
        public override int Order => 100000;
        public override bool Recommended => true;
        public override string HelpText => "Geçerli layer 0 ve geçerli layer ayarlarını BYLAYER olarak ayarlar.";

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