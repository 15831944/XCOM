using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XCOM.Commands.XCommand
{
    public class RenameXREFs : XCOMActionBase
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

        public override string Name => "Rename XREFs"; 
        public override int Order => 52; 
        public override ActionInterface Interface => ActionInterface.Dialog;
        public override string HelpText => "XREF dosya isimlerinde verilen metni arar ve başka bir metinle değiştirir.";

        protected List<FindReplaceOptions> options = new List<FindReplaceOptions>();
        protected bool renamePaths = true;

        public override bool ShowDialog()
        {
            using (RenameXREFsForm form = new RenameXREFsForm())
            {
                string[] find = new string[options.Count];
                string[] replace = new string[options.Count];
                for (int i = 0; i < options.Count; i++)
                {
                    find[i] = options[i].Find;
                    replace[i] = options[i].Replace;
                }
                form.SetFindReplaceStrings(find, replace);
                form.RenamePaths = renamePaths;

                if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

                form.GetFindReplaceStrings(out find, out replace);
                options = new List<FindReplaceOptions>();
                for (int i = 0; i < find.Length; i++)
                {
                    options.Add(new FindReplaceOptions(find[i], replace[i]));
                }
                renamePaths = form.RenamePaths;

                return true;
            }
        }

        public override void Run(string filename, Database db)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // List xrefs
                    ObjectIdCollection xrefs = new ObjectIdCollection();
                    XrefGraph graph = db.GetHostDwgXrefGraph(false);

                    if (!graph.IsEmpty && graph.NumNodes > 1)
                    {
                        for (int i = 0; i < graph.NumNodes; i++)
                        {
                            XrefGraphNode node = graph.GetXrefNode(i);

                            // Skip if cannot read
                            if (node == null)
                            {
                                OnError(new Exception("XREF okunamadı."));
                                continue;
                            }

                            // Skip if it is the root node (current drawing)
                            if (node == graph.HostDrawing) continue;

                            // Skip if nested
                            if (node.IsNested) continue;

                            xrefs.Add(node.BlockTableRecordId);
                        }

                        // Rename
                        foreach (ObjectId id in xrefs)
                        {
                            BlockTableRecord block = (BlockTableRecord)tr.GetObject(id, OpenMode.ForWrite);
                            if (block.IsFromExternalReference)
                            {
                                foreach (FindReplaceOptions opt in options)
                                {
                                    if (string.Compare(block.Name, opt.Find, StringComparison.CurrentCultureIgnoreCase) == 0)
                                    {
                                        block.Name = opt.Replace;
                                    }

                                    if (renamePaths && Regex.IsMatch(block.PathName, Regex.Escape(opt.Find + ".dwg"), RegexOptions.IgnoreCase))
                                    {
                                        block.PathName = Regex.Replace(block.PathName, Regex.Escape(opt.Find + ".dwg"), opt.Replace + ".dwg", RegexOptions.IgnoreCase);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnError(ex);
                }

                tr.Commit();
            }
        }
    }
}