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
        public ActionInterface Interface { get { return ActionInterface.Both; } }

        private bool purgeBlocks = true;
        private bool purgeDimensionStyles = true;
        private bool purgeLayers = true;
        private bool purgeLinetypes = true;
        private bool purgeTextStyles = true;
        private bool purgeUCSSettings = true;
        private bool purgeViewports = true;
        private bool purgeViews = true;

        private bool purgeGroups = true;
        private bool purgeMaterials = true;
        private bool purgeMlineStyles = true;
        private bool purgeMultileaderStyles = true;
        private bool purgePlotStyles = true;
        private bool purgeTableStyles = true;

        private bool purgeShapes = false;

        private bool purgeZeroLengthGeometry = true;
        private bool purgeEmptyTexts = true;

        private bool purgeRegApps = true;

        private double zeroLengthGeometryTolerance = 1e-6;

        public override string ToString()
        {
            return Name;
        }

        public string[] Run(string filename, Database db)
        {
            List<string> errors = new List<string>();

            List<ObjectId> tables = new List<ObjectId>();
            if (purgeBlocks) tables.Add(db.BlockTableId);
            if (purgeDimensionStyles) tables.Add(db.DimStyleTableId);
            if (purgeLayers) tables.Add(db.LayerTableId);
            if (purgeLinetypes) tables.Add(db.LinetypeTableId);
            if (purgeTextStyles) tables.Add(db.TextStyleTableId);
            if (purgeUCSSettings) tables.Add(db.UcsTableId);
            if (purgeViewports) tables.Add(db.ViewportTableId);
            if (purgeViews) tables.Add(db.ViewTableId);

            List<ObjectId> dictionaries = new List<ObjectId>();
            if (purgeGroups) dictionaries.Add(db.GroupDictionaryId);
            if (purgeMaterials) dictionaries.Add(db.MaterialDictionaryId);
            if (purgeMlineStyles) dictionaries.Add(db.MLStyleDictionaryId);
            if (purgeMultileaderStyles) dictionaries.Add(db.MLeaderStyleDictionaryId);
            if (purgePlotStyles) dictionaries.Add(db.PlotStyleNameDictionaryId);
            if (purgeTableStyles) dictionaries.Add(db.TableStyleDictionaryId);

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
                    if (purgeEmptyTexts | purgeZeroLengthGeometry)
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
                                else if (purgeZeroLengthGeometry && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Curve))))
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
                        obj.Erase(true);
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

            bool isMaterialDictionary = (dictionaryID == db.MaterialDictionaryId);

            DBDictionary dictionary = (DBDictionary)tr.GetObject(dictionaryID, OpenMode.ForRead);
            foreach (DBDictionaryEntry entry in dictionary)
            {
                if (isMaterialDictionary)
                {
                    string name = entry.Key.ToUpperInvariant();
                    // Default materials cannot be purged
                    if (name == "BYLAYER" || name == "BYBLOCK" || name == "GLOBAL") continue;
                }

                idList.Add(entry.Value);
            }

            return idList;
        }

        public bool ShowDialog()
        {
            XCOMCore.ActionForms.PurgeAllForm form = new ActionForms.PurgeAllForm();

            form.PurgeBlocks = purgeBlocks;
            form.PurgeTextStyles = purgeTextStyles;
            form.PurgeTableStyles = purgeTableStyles;
            form.PurgeShapes = purgeShapes;
            form.PurgePlotStyles = purgePlotStyles;
            form.PurgeMultileaderStyles = purgeMultileaderStyles;
            form.PurgeMlineStyles = purgeMlineStyles;
            form.PurgeMaterials = purgeMaterials;
            form.PurgeLinetypes = purgeLinetypes;
            form.PurgeLayers = purgeLayers;
            form.PurgeGroups = purgeGroups;
            form.PurgeDimensionStyles = purgeDimensionStyles;
            form.PurgeUCSSettings = purgeUCSSettings;
            form.PurgeViews = purgeViews;
            form.PurgeViewports = purgeViewports;
            form.PurgeZeroLengthGeometry = purgeZeroLengthGeometry;
            form.PurgeEmptyTexts = purgeEmptyTexts;
            form.PurgeRegApps = purgeRegApps;

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

            purgeBlocks = form.PurgeBlocks;
            purgeTextStyles = form.PurgeTextStyles;
            purgeTableStyles = form.PurgeTableStyles;
            purgeShapes = form.PurgeShapes;
            purgePlotStyles = form.PurgePlotStyles;
            purgeMultileaderStyles = form.PurgeMultileaderStyles;
            purgeMlineStyles = form.PurgeMlineStyles;
            purgeMaterials = form.PurgeMaterials;
            purgeLinetypes = form.PurgeLinetypes;
            purgeLayers = form.PurgeLayers;
            purgeGroups = form.PurgeGroups;
            purgeDimensionStyles = form.PurgeDimensionStyles;
            purgeUCSSettings = form.PurgeUCSSettings;
            purgeViews = form.PurgeViews;
            purgeViewports = form.PurgeViewports;
            purgeZeroLengthGeometry = form.PurgeZeroLengthGeometry;
            purgeEmptyTexts = form.PurgeEmptyTexts;
            purgeRegApps = form.PurgeRegApps;

            return true;
        }
    }
}