using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM.Commands.XCommand
{
    public class DeleteModel : XCOMActionBase
    {
        public override string Name => "Delete Model Space";
        public override int Order => 300;
        public override string HelpText => "Model'deki tüm çizim nesnelerini siler. Bu işlemden sonra çizimde sadece layout nesneleri kalacaktır. " +
            "BU İŞLEM ÇİZİMDEKİ MODEL NESNELERİNİN TÜMÜNÜ SİLECEKTİR.";

        public override void Run(string filename, Database db)
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
    }
}