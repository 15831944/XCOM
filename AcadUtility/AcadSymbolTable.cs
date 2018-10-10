using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace AcadUtility
{
    // Symbol table utilities
    public static class AcadSymbolTable
    {
        #region Iterate Generic Symbol Table
        private static IEnumerable<TResult> GetSymbolTableRecords<TResult, TSymbolTableRecord>(Database db, ObjectId symbolTableId, Func<TSymbolTableRecord, bool> predicate, Func<TSymbolTableRecord, TResult> selector) where TSymbolTableRecord : SymbolTableRecord
        {
            List<TResult> list = new List<TResult>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                SymbolTable table = (SymbolTable)tr.GetObject(symbolTableId, OpenMode.ForRead);
                SymbolTableEnumerator it = table.GetEnumerator();
                while (it.MoveNext())
                {
                    ObjectId id = it.Current;
                    TSymbolTableRecord block = (TSymbolTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    if (predicate(block))
                    {
                        list.Add(selector(block));
                    }
                }

                tr.Commit();
            }

            return list;
        }

        private static IEnumerable<ObjectId> GetSymbolTableRecords<TSymbolTableRecord>(Database db, ObjectId symbolTableId, Func<TSymbolTableRecord, bool> predicate) where TSymbolTableRecord : SymbolTableRecord
        {
            return GetSymbolTableRecords<ObjectId, TSymbolTableRecord>(db, symbolTableId, predicate, p => p.ObjectId);
        }

        private static IEnumerable<TResult> GetSymbolTableRecords<TResult, TSymbolTableRecord>(Database db, ObjectId symbolTableId, Func<TSymbolTableRecord, TResult> selector) where TSymbolTableRecord : SymbolTableRecord
        {
            return GetSymbolTableRecords<TResult, TSymbolTableRecord>(db, symbolTableId, p => true, selector);
        }

        private static IDictionary<string, ObjectId> GetSymbolTableRecords(Database db, ObjectId symbolTableId)
        {
            var list = new SortedDictionary<string, ObjectId>();
            foreach (var entry in GetSymbolTableRecords< KeyValuePair<string, ObjectId>, SymbolTableRecord >(db, symbolTableId, p => true, p => new KeyValuePair<string, ObjectId>(p.Name, p.ObjectId)))
            {
                list.Add(entry.Key, entry.Value);
            }
            return list;
        }
        #endregion

        #region Iterate Block Table
        public static IEnumerable<TResult> GetBlockTableRecords<TResult>(Database db, Func<BlockTableRecord, bool> predicate, Func<BlockTableRecord, TResult> selector)
        {
            return GetSymbolTableRecords(db, db.BlockTableId, predicate, selector);
        }

        public static IEnumerable<ObjectId> GetBlockTableRecords(Database db, Func<BlockTableRecord, bool> predicate)
        {
            return GetBlockTableRecords(db, predicate, p => p.ObjectId);
        }

        public static IEnumerable<TResult> GetBlockTableRecords<TResult>(Database db, Func<BlockTableRecord, TResult> selector)
        {
            return GetBlockTableRecords(db, p => true, selector);
        }

        public static IDictionary<string, ObjectId> GetBlockTableRecords(Database db)
        {
            return GetSymbolTableRecords(db, db.BlockTableId);
        }
        #endregion

        #region Iterate Text Style Table
        public static IEnumerable<TResult> GetTextStyleTableRecords<TResult>(Database db, Func<TextStyleTableRecord, bool> predicate, Func<TextStyleTableRecord, TResult> selector)
        {
            return GetSymbolTableRecords(db, db.TextStyleTableId, predicate, selector);
        }

        public static IEnumerable<ObjectId> GetTextStyleTableRecords(Database db, Func<TextStyleTableRecord, bool> predicate)
        {
            return GetTextStyleTableRecords(db, predicate, p => p.ObjectId);
        }

        public static IEnumerable<TResult> GetTextStyleTableRecords<TResult>(Database db, Func<TextStyleTableRecord, TResult> selector)
        {
            return GetTextStyleTableRecords(db, p => true, selector);
        }

        public static IDictionary<string, ObjectId> GetTextStyleTableRecords(Database db)
        {
            return GetSymbolTableRecords(db, db.TextStyleTableId);
        }
        #endregion

        #region Iterate Layer Table
        public static IEnumerable<TResult> GetLayerTableRecords<TResult>(Database db, Func<LayerTableRecord, bool> predicate, Func<LayerTableRecord, TResult> selector)
        {
            return GetSymbolTableRecords(db, db.LayerTableId, predicate, selector);
        }

        public static IEnumerable<ObjectId> GetLayerTableRecords(Database db, Func<LayerTableRecord, bool> predicate)
        {
            return GetLayerTableRecords(db, predicate, p => p.ObjectId);
        }

        public static IEnumerable<TResult> GetLayerTableRecords<TResult>(Database db, Func<LayerTableRecord, TResult> selector)
        {
            return GetLayerTableRecords(db, p => true, selector);
        }

        public static IDictionary<string, ObjectId> GetLayerTableRecords<TResult>(Database db)
        {
            return GetSymbolTableRecords(db, db.LayerTableId);
        }
        #endregion
    }
}
