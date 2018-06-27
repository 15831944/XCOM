using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace XCOM.Commands.XCommand
{
    public class SetDWGVariables : XCOMActionBase
    {
        public override string Name => "Set DWG Variables";
        public override int Order => 152000;
        public override bool Recommended => true;
        public override string HelpText => "Aşağıdaki listedeki DWG değişkenlerini sıkça kullanılan değerleri ile değiştirir." + Environment.NewLine +
            "    REGENMODE: 1 (Zoom ve Pan yaparken gerektiğinde otomatik olarak REGEN yapılmasını sağlar.)";

        public override void Run(string filename, Database db)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    db.Regenmode = true;
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