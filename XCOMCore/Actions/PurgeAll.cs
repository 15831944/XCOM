using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace XCOMCore
{
    public class PurgeAll : IXCOMAction
    {
        public string Name { get { return "Purge All"; } }
        public int Order { get { return 10000; } }
        public bool Recommended { get { return true; } }
        public ActionInterface Interface { get { return ActionInterface.Command; } }
        public bool ShowDialog() { return true; }

        public bool purgeRegApps = true;
        public bool purgeEmptyTexts = true;
        public bool purgeZeroLength = true;
        public double zeroLengthGeometryTolerance = 1e-6;

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

            // TODO - Purge shapes

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

                    // Reg apps
                    if (purgeRegApps)
                    {
                        RegAppTable regAppTable = (RegAppTable)tr.GetObject(db.RegAppTableId, OpenMode.ForRead);
                        foreach (ObjectId id in regAppTable)
                        {
                            if (id.IsValid)
                            {
                                idList.Add(id);
                            }
                        }
                    }

                    // Empty text objects or zero length geometry
                    if (purgeEmptyTexts | purgeZeroLength)
                    {
                        ObjectIdCollection blockIDs = new ObjectIdCollection();
                        blockIDs.Add(SymbolUtilityServices.GetBlockModelSpaceId(db));
                        DBDictionary layoutDict = (DBDictionary)tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead);
                        foreach (DBDictionaryEntry entry in layoutDict)
                        {
                            if (entry.Key.ToUpperInvariant() != "MODEL")
                            {
                                Layout layout = (Layout)tr.GetObject(entry.Value, OpenMode.ForRead);
                                blockIDs.Add(layout.BlockTableRecordId);
                            }
                        }

                        foreach (ObjectId blockID in blockIDs)
                        {
                            var btr = (BlockTableRecord)tr.GetObject(blockID, OpenMode.ForRead);
                            foreach (ObjectId id in btr)
                            {
                                // Empty text objects
                                if (purgeEmptyTexts && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(DBText))))
                                {
                                    DBText text = (DBText)tr.GetObject(id, OpenMode.ForRead);
                                    if (string.IsNullOrEmpty(text.TextString))
                                    {
                                        text.UpgradeOpen();
                                        text.Erase(true);
                                    }
                                }
                                else if (purgeEmptyTexts && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(MText))))
                                {
                                    MText text = (MText)tr.GetObject(id, OpenMode.ForRead);
                                    if (string.IsNullOrEmpty(text.Text))
                                    {
                                        text.UpgradeOpen();
                                        text.Erase(true);
                                    }
                                }
                                // Zero length geometry
                                else if (purgeZeroLength && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Curve))))
                                {
                                    Curve curve = (Curve)tr.GetObject(id, OpenMode.ForRead);
                                    double len = Math.Abs(curve.GetDistanceAtParameter(curve.EndParam) - curve.GetDistanceAtParameter(curve.StartParam));
                                    if (len < zeroLengthGeometryTolerance)
                                    {
                                        curve.UpgradeOpen();
                                        curve.Erase(true);
                                    }
                                }
                            }
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