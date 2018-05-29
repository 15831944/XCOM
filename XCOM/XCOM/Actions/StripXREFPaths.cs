using Autodesk.AutoCAD.DatabaseServices;
using System;

namespace XCOM.Commands.XCommand
{
    public class StripXREFPaths : XCOMActionBase
    {
        public override string Name { get { return "XREF Sadece Dosya Adı"; } }
        public override int Order { get { return 51; } }

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
    }
}