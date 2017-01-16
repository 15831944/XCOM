using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace XCOM.Commands.XCommand
{
    public class DeleteModel : XCOMActionBase
    {
        public override string Name { get { return "Delete Model Space"; } }
        public override int Order { get { return 300; } }

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