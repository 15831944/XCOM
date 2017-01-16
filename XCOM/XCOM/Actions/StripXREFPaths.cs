using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM.Commands.XCommand
{
    public class StripXREFPaths : IXCOMAction
    {
        public string Name { get { return "XREF Sadece Dosya Adı"; } }
        public int Order { get { return 51; } }
        public bool Recommended { get { return false; } }
        public ActionInterface Interface { get { return ActionInterface.Command; } }

        public override string ToString()
        {
            return Name;
        }

        public bool ShowDialog()
        {
            return false;
        }

        public void Run(string filename, Database db)
        {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    // Resolve XREFs
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

                            // Skip if XREF is not resolved
                            if (node.XrefStatus != XrefStatus.Resolved)
                            {
                                OnError(new Exception("XREF bulunamadı: " + node.Name));
                                continue;
                            }

                            xrefs.Add(node.BlockTableRecordId);
                        }

                        // Strip paths
                        foreach (ObjectId id in xrefs)
                        {
                            BlockTableRecord block = (BlockTableRecord)tr.GetObject(id, OpenMode.ForWrite);
                            if (block.IsFromExternalReference)
                            {
                                block.PathName = StripPath(block.PathName);
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

        private string StripPath(string path)
        {
            int i = path.LastIndexOf('\\');
            if (i != -1)
                path = path.Substring(i + 1);
            return path;
        }

        public event EventHandler<ActionProgressEventArgs> Progress;
        public event EventHandler<ActionErrorEventArgs> Error;

        protected void OnProgress(string message)
        {
            Progress?.Invoke(this, new ActionProgressEventArgs(message));
        }

        protected void OnError(Exception error)
        {
            Error?.Invoke(this, new ActionErrorEventArgs(error));
        }
    }
}