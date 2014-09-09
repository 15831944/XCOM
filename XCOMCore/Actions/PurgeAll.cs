using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOMCore
{
    public class PurgeAll : IXCOMAction
    {
        public string Name { get { return "Purge All"; } }
        public int Order { get { return 10000; } }
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

            ObjectId[] tables = new ObjectId[] {
                db.BlockTableId,
                db.DimStyleTableId,
                db.LayerTableId,
                db.LinetypeTableId,
                db.TextStyleTableId,
                db.UcsTableId,
                db.ViewportTableId,
                db.ViewTableId
            };

            ObjectId[] dictionaries = new ObjectId[] {
                db.GroupDictionaryId,
                db.MaterialDictionaryId,
                db.MLStyleDictionaryId,
                db.MLeaderStyleDictionaryId,
                db.PlotStyleNameDictionaryId,
                db.TableStyleDictionaryId,
                db.VisualStyleDictionaryId
            };

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    ObjectIdCollection idList = new ObjectIdCollection();

                    // Symbol tables
                    foreach (ObjectId tableID in tables)
                    {
                        foreach (ObjectId id in CollectTableIds(tr, db, tableID))
                        {
                            idList.Add(id);
                        }
                    }

                    // Dictionary entries
                    foreach (ObjectId dictionaryID in dictionaries)
                    {
                        foreach (ObjectId id in CollectDictionaryIds(tr, db, dictionaryID))
                        {
                            idList.Add(id);
                        }
                    }

                    // Filter purgables
                    db.Purge(idList);

                    // Step through the returned ObjectIdCollection
                    // and erase each unreferenced symbol
                    foreach (ObjectId id in idList)
                    {
                        DBObject obj = tr.GetObject(id, OpenMode.ForWrite);

                        try
                        {
                            obj.Erase(true);
                        }
                        catch (System.Exception ex)
                        {
                            errors.Add(ex.Message);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    errors.Add(ex.Message);
                }

                tr.Commit();
            }

            return errors.ToArray();
        }

        private IEnumerable<ObjectId> CollectTableIds(Transaction tr, Database db, ObjectId tableID)
        {
            List<ObjectId> idList = new List<ObjectId>();

            SymbolTable table = (SymbolTable)tr.GetObject(tableID, OpenMode.ForRead);

            foreach (ObjectId id in table)
            {
                idList.Add(id);
            }

            return idList;
        }

        private IEnumerable<ObjectId> CollectDictionaryIds(Transaction tr, Database db, ObjectId dictionaryID)
        {
            List<ObjectId> idList = new List<ObjectId>();

            DBDictionary dictionary = (DBDictionary)tr.GetObject(dictionaryID, OpenMode.ForRead);

            foreach (DBDictionaryEntry entry in dictionary)
            {
                idList.Add(entry.Value);
            }

            return idList;
        }
    }
}