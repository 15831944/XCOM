using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XCOM.Commands.XCommand
{
    public class FindReplaceText : XCOMActionBase
    {
        protected class FindReplaceOptions
        {
            public string Find { get; private set; }
            public string Replace { get; private set; }

            public FindReplaceOptions(string find, string replace)
            {
                Find = find;
                Replace = replace;
            }
        }

        public override string Name { get { return "Find & Replace Text"; } }
        public override int Order { get { return 150000; } }
        public override ActionInterface Interface { get { return ActionInterface.Dialog; } }

        protected List<FindReplaceOptions> options = new List<FindReplaceOptions>();

        protected bool frText = true;
        protected bool frAttribute = false;
        protected bool frDimension = false;
        protected bool frLeader = false;
        protected bool frTable = false;

        protected bool optCaseSensitive = false;
        protected bool optMatchWholeWords = false;
        protected bool optUseWildcards = false;

        protected bool applyToAllBlockDefinitons = false;
        protected bool applyModel = true;
        protected bool applyLayouts = true;

        public override void Run(string filename, Database db)
        {
            if (options.Count == 0) return;

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

                    foreach (ObjectId id in blockIDs)
                    {
                        FindReplaceBlock(tr, id);
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
            using (FindReplaceTextForm form = new FindReplaceTextForm())
            {
                form.SearchText = frText;
                form.SearchAttribute = frAttribute;
                form.SearchDimension = frDimension;
                form.SearchLeader = frLeader;
                form.SearchTable = frTable;

                form.CaseSensitive = optCaseSensitive;
                form.MatchWholeWords = optMatchWholeWords;
                form.UseWildcards = optUseWildcards;

                string[] find = new string[options.Count];
                string[] replace = new string[options.Count];
                for (int i = 0; i < options.Count; i++)
                {
                    find[i] = options[i].Find;
                    replace[i] = options[i].Replace;
                }
                form.SetFindReplaceStrings(find, replace);

                form.SetSearchScope(applyModel, applyLayouts, applyToAllBlockDefinitons);

                if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

                frText = form.SearchText;
                frAttribute = form.SearchAttribute;
                frDimension = form.SearchDimension;
                frLeader = form.SearchLeader;
                frTable = form.SearchTable;
                form.GetFindReplaceStrings(out find, out replace);

                optCaseSensitive = form.CaseSensitive;
                optMatchWholeWords = form.MatchWholeWords;
                optUseWildcards = form.UseWildcards;

                options = new List<FindReplaceOptions>();
                for (int i = 0; i < find.Length; i++)
                {
                    options.Add(new FindReplaceOptions(find[i], replace[i]));
                }

                form.GetSearchScope(out applyModel, out applyLayouts, out applyToAllBlockDefinitons);

                return true;
            }
        }

        private void FindReplaceBlock(Transaction tr, ObjectId blockid)
        {
            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(blockid, OpenMode.ForRead);
            foreach (ObjectId id in btr)
            {
                FindReplaceItem(tr, id);
            }
        }

        private void FindReplaceItem(Transaction tr, ObjectId id)
        {
            try
            {
                foreach (FindReplaceOptions item in options)
                {
                    string replacedText = string.Empty;

                    if (frText && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(DBText))))
                    {
                        // Single line text
                        DBText text = (DBText)tr.GetObject(id, OpenMode.ForRead);
                        if (GetReplacedText(item.Find, item.Replace, text.TextString, out replacedText))
                        {
                            text.UpgradeOpen();
                            text.TextString = replacedText;
                        }
                    }
                    else if (frText && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(MText))))
                    {
                        // Multiline text
                        MText mtext = (MText)tr.GetObject(id, OpenMode.ForRead);
                        if (GetReplacedText(item.Find, item.Replace, mtext.Contents, out replacedText))
                        {
                            mtext.UpgradeOpen();
                            mtext.Contents = replacedText;
                        }
                    }
                    else if (frDimension && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Dimension))))
                    {
                        // Dimension text
                        Dimension dimension = (Dimension)tr.GetObject(id, OpenMode.ForRead);
                        if (GetReplacedText(item.Find, item.Replace, dimension.DimensionText, out replacedText))
                        {
                            dimension.UpgradeOpen();
                            dimension.DimensionText = replacedText;
                        }
                    }
                    else if (frLeader && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(MLeader))))
                    {
                        // Multileader
                        MLeader leader = (MLeader)tr.GetObject(id, OpenMode.ForRead);
                        if (leader.ContentType == ContentType.MTextContent)
                        {
                            if (GetReplacedText(item.Find, item.Replace, leader.MText.Contents, out replacedText))
                            {
                                leader.UpgradeOpen();
                                leader.MText.Contents = replacedText;
                            }
                        }
                    }
                    else if (frAttribute && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(BlockReference))))
                    {
                        // Block attributes
                        BlockReference blockRef = (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        foreach (ObjectId attid in blockRef.AttributeCollection)
                        {
                            AttributeReference attRef = (AttributeReference)tr.GetObject(attid, OpenMode.ForRead);
                            if (GetReplacedText(item.Find, item.Replace, attRef.TextString, out replacedText))
                            {
                                attRef.UpgradeOpen();
                                attRef.TextString = replacedText;
                            }
                        }
                    }
                    else if (frTable && id.ObjectClass.IsDerivedFrom(RXObject.GetClass(typeof(Table))))
                    {
                        // Table text
                        Table table = (Table)tr.GetObject(id, OpenMode.ForWrite);
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            for (int j = 0; j < table.Columns.Count; j++)
                            {
                                if (GetReplacedText(item.Find, item.Replace, table.Cells[i, j].TextString, out replacedText))
                                {
                                    table.Cells[i, j].TextString = replacedText;
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                OnError(ex);
            }
        }

        private bool GetReplacedText(string find, string replace, string oldText, out string newText)
        {
            if (optUseWildcards)
            {
                find = find.Replace("*", ".*");
                find = find.Replace("?", ".?");
            }

            if (optMatchWholeWords)
            {
                find = "\\b" + find + "\\b";
            }

            Regex myRegex = new Regex(find, optCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
            newText = myRegex.Replace(oldText, replace);

            return (newText != oldText);
        }
    }
}