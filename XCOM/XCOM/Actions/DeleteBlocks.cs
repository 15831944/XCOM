using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;

namespace XCOM.Commands.XCommand
{
    public class DeleteBlocks : XCOMActionBase
    {
        public override string Name { get { return "Blok Sil"; } }
        public override int Order { get { return 200; } }
        public override ActionInterface Interface { get { return ActionInterface.Dialog; } }

        protected List<string> blockNames = new List<string>();

        protected bool applyToAllBlockDefinitons = false;
        protected bool applyModel = true;
        protected bool applyLayouts = true;

        public override void Run(string filename, Database db)
        {
            if (blockNames.Count == 0) return;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    ObjectIdCollection blockIDs = new ObjectIdCollection();

                    if (applyToAllBlockDefinitons)
                    {
                        // Apply to all block definitions 
                        BlockTable blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        foreach (ObjectId id in blockTable)
                        {
                            if (id == SymbolUtilityServices.GetBlockModelSpaceId(db)) continue;

                            BlockTableRecord block = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
                            if (block.IsLayout) continue;

                            blockIDs.Add(id);
                        }
                    }
                    else
                    {
                        // Model space (or entire drawing)
                        if (applyModel)
                        {
                            blockIDs.Add(SymbolUtilityServices.GetBlockModelSpaceId(db));
                        }

                        // Layouts (or entire drawing)
                        if (applyLayouts)
                        {
                            DBDictionary layoutDict = (DBDictionary)tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead);
                            foreach (DBDictionaryEntry entry in layoutDict)
                            {
                                if (entry.Key.ToUpperInvariant() != "MODEL")
                                {
                                    Layout layout = (Layout)tr.GetObject(entry.Value, OpenMode.ForRead);
                                    blockIDs.Add(layout.BlockTableRecordId);
                                }
                            }
                        }
                    }

                    foreach (ObjectId spaceId in blockIDs)
                    {
                        // Open the block space
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(spaceId, OpenMode.ForRead);
                        foreach (ObjectId id in btr)
                        {
                            if (id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(BlockReference))))
                            {
                                BlockReference bref = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                                bool canDelete = false;
                                foreach (string name in blockNames)
                                {
                                    if (string.Compare(name, bref.Name, StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        canDelete = true;
                                    }
                                }
                                if (canDelete)
                                {
                                    bref.UpgradeOpen();
                                    bref.Erase(true);
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    OnError(ex);
                }

                tr.Commit();
            }
        }

        public override bool ShowDialog()
        {
            string[] names;

            using (DeleteBlockForm form = new DeleteBlockForm())
            {
                names = new string[blockNames.Count];
                for (int i = 0; i < blockNames.Count; i++)
                {
                    names[i] = blockNames[i];
                }
                form.SetBlockNames(names);

                form.SetSearchScope(applyModel, applyLayouts, applyToAllBlockDefinitons);

                if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

                form.GetBlockNames(out names);

                blockNames.Clear();
                for (int i = 0; i < names.Length; i++)
                {
                    blockNames.Add(names[i]);
                }

                form.GetSearchScope(out applyModel, out applyLayouts, out applyToAllBlockDefinitons);

                return true;
            }
        }
    }
}