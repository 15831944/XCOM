using System.Text.RegularExpressions;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace XCOM
{
    public class ProtectDrawing : IXCOMAction
    {
        public string Name { get { return "Dosya Koruması"; } }
        public int Order { get { return 300000; } }
        public bool Recommended { get { return false; } }
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
                    // Get all items in this space
                    BlockTableRecord btrModel = (BlockTableRecord)tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForWrite);
                    ObjectIdCollection items = new ObjectIdCollection();
                    foreach (ObjectId id in btrModel)
                    {
                        items.Add(id);
                    }

                    if (items.Count > 0)
                    {
                        // Create the unexlodable anon block
                        BlockTableRecord btrProtected = new BlockTableRecord();
                        btrProtected.Name = "*U";
                        btrProtected.Explodable = false;

                        // Add the new block to the block table
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForWrite);
                        ObjectId btrId = bt.Add(btrProtected);
                        tr.AddNewlyCreatedDBObject(btrProtected, true);

                        // Add all entities in this space to the anon block
                        btrProtected.AssumeOwnershipOf(items);

                        // Add the block reference to this space
                        BlockReference br = new BlockReference(Autodesk.AutoCAD.Geometry.Point3d.Origin, btrId);
                        btrModel.AppendEntity(br);
                        tr.AddNewlyCreatedDBObject(br, true);
                    }
                }
                catch (System.Exception ex)
                {
                    errors.Add(ex.ToString());
                }

                tr.Commit();
            }

            return errors.ToArray();
        }
    }
}