using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOMCore
{
    public class BindXREFs : IXCOMAction
    {
        public string Name { get { return "Bind XREFs"; } }
        public int Order { get { return 50; } }
        public bool Recommended { get { return false; } }
        public ActionInterface Interface { get { return ActionInterface.Both; } }

        public override string ToString()
        {
            return Name;
        }

        private bool resolveXREFs = true;
        private bool insertMode = false;

        public bool ShowDialog() 
        {
            XCOMCore.ActionForms.BindXREFsForm form = new ActionForms.BindXREFsForm();
            form.ResolveXREFs = resolveXREFs;
            form.InsertMode = insertMode;

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return false;

            resolveXREFs = form.ResolveXREFs;
            insertMode = form.InsertMode;

            return true;
        }

        public string[] Run(string filename, Database db)
        {
            List<string> errors = new List<string>();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // Resolve XREFs
                    if (resolveXREFs) db.ResolveXrefs(true, false);

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
                                errors.Add("XREF okunamadı.");
                                continue;
                            }

                            // Skip if it is the root node (current drawing)
                            if (node == graph.HostDrawing) continue;

                            // Skip if nested
                            if (node.IsNested) continue;

                            // Skip if XREF is not resolved
                            if (node.XrefStatus != XrefStatus.Resolved)
                            {
                                errors.Add("XREF bulunamadı: " + node.Name);
                                continue;
                            }

                            xrefs.Add(node.BlockTableRecordId);
                        }

                        // Bind
                        if (xrefs.Count != 0)
                        {
                            db.BindXrefs(xrefs, insertMode);
                        }
                    }

                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return errors.ToArray();
        }
    }
}