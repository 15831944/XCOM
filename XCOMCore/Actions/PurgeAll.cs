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

            List<ObjectId> tables = new List<ObjectId>() {
                db.BlockTableId,
                db.DimStyleTableId,
                db.LayerTableId,
                db.LinetypeTableId,
                db.TextStyleTableId,
                db.UcsTableId,
                db.ViewportTableId,
                db.ViewTableId
            };

            List<ObjectId> dictionaries = new List<ObjectId>() {
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
                foreach (ObjectId tableID in tables)
                {
                    List<string> tableErrors = PurgeTable(tr, db, tableID);
                    errors.AddRange(tableErrors);
                }

                foreach (ObjectId dictionaryID in dictionaries)
                {
                    List<string> dictionaryErrors = PurgeDictionary(tr, db, dictionaryID);
                    errors.AddRange(dictionaryErrors);
                }

                tr.Commit();
            }

            return errors.ToArray();
        }

        private List<string> PurgeTable(Transaction tr, Database db, ObjectId tableID)
        {
            List<string> errors = new List<string>();

            try
            {
                ObjectIdCollection idList = new ObjectIdCollection();

                SymbolTable table = (SymbolTable)tr.GetObject(tableID, OpenMode.ForRead);

                foreach (ObjectId acObjId in table)
                {
                    idList.Add(acObjId);
                }

                db.Purge(idList);

                // Step through the returned ObjectIdCollection
                // and erase each unreferenced symbol
                foreach (ObjectId id in idList)
                {
                    SymbolTableRecord tableRecord = (SymbolTableRecord)tr.GetObject(id, OpenMode.ForWrite);

                    try
                    {
                        tableRecord.Erase(true);
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

            return errors;
        }

        private List<string> PurgeDictionary(Transaction tr, Database db, ObjectId dictionaryID)
        {
            List<string> errors = new List<string>();

            try
            {
                ObjectIdCollection idList = new ObjectIdCollection();

                DBDictionary dictionary = (DBDictionary)tr.GetObject(dictionaryID, OpenMode.ForRead);

                foreach (DBDictionaryEntry entry in dictionary)
                {
                    idList.Add(entry.Value);
                }

                db.Purge(idList);

                // Step through the returned ObjectIdCollection
                // and erase each unreferenced symbol
                foreach (ObjectId id in idList)
                {
                    DBObject dictionaryRecord = (DBObject)tr.GetObject(id, OpenMode.ForWrite);

                    try
                    {
                        dictionaryRecord.Erase(true);
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

            return errors;
        }
    }
}