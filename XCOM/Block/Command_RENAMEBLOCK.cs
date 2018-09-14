using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XCOM.Commands.Block
{
    public class Command_RENAMEBLOCK
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("BLOCKRENAMER", CommandFlags.UsePickSet)]
        public void RenameBlock()
        {
            if (!CheckLicense.Check()) return;

            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            // Select block name in dialog if there is one picked
            string selectBlockName = "";
            PromptSelectionResult selRes = doc.Editor.SelectImplied();
            if (selRes.Status == PromptStatus.OK)
            {
                var selSet = selRes.Value;
                if (selSet.Count > 0)
                {
                    var id = selSet[0].ObjectId;
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockReference bref = tr.GetObject(id, OpenMode.ForRead) as BlockReference;
                        if (bref != null)
                        {
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                            selectBlockName = btr.Name;
                        }
                    }
                }
            }

            Dictionary<string, string> tempNames = new Dictionary<string, string>();
            int tempCount = 1;

            using (RenameBlockForm form = new RenameBlockForm())
            {
                // Read block names
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    foreach (ObjectId id in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);

                        if (btr.IsLayout || btr.IsFromExternalReference || btr.IsFromOverlayReference)
                            continue;

                        form.AddBlockName(btr.Name, btr.IsAnonymous, (btr.HasPreviewIcon ? btr.PreviewIcon : null));

                        // Create a temporary name for this block
                        string tempName = "_TMP_BLOCK_NAME_" + tempCount.ToString();
                        while (bt.Has(tempName))
                        {
                            tempCount++;
                            tempName = "_TMP_BLOCK_NAME_" + tempCount.ToString();
                        }
                        tempNames.Add(btr.Name, tempName);
                        tempCount++;
                    }

                    tr.Commit();
                }
                form.SelectBlock(selectBlockName);

                // Rename blocks
                if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    var names = form.BlockNames;

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
                    {
                        try
                        {
                            // Set temporary names first to prevent name collisions
                            foreach (var pair in names)
                            {
                                ObjectId id = bt[pair.Key];
                                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForWrite);
                                btr.Name = tempNames[pair.Key];
                            }
                            // Set final names now
                            foreach (var pair in names)
                            {
                                ObjectId id = bt[tempNames[pair.Key]];
                                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForWrite);
                                btr.Name = pair.Value;
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        tr.Commit();
                    }
                }
            }
        }
    }
}
