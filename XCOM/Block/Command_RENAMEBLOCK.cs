using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AcadUtility;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

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
                            if (bref.IsDynamicBlock)
                            {
                                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bref.DynamicBlockTableRecord, OpenMode.ForRead);
                                selectBlockName = btr.Name;
                            }
                            else
                            {
                                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bref.BlockTableRecord, OpenMode.ForRead);
                                selectBlockName = btr.Name;
                            }
                        }
                    }
                }
            }

            var allNames = AcadSymbolTable.GetBlockTableRecords(db);
            var tempNames = new Dictionary<string, string>();
            int tempCount = 1;

            using (RenameBlockForm form = new RenameBlockForm())
            {
                foreach (var item in AcadSymbolTable.GetBlockTableRecords(db,
                    p => !p.IsLayout && !p.IsFromExternalReference && !p.IsFromOverlayReference,
                    p => new { p.Name, p.IsAnonymous, p.PreviewIcon }))
                {
                    form.AddBlockName(item.Name, item.IsAnonymous, item.PreviewIcon);

                    // Create a temporary name for this block
                    string tempName = "_TMP_BLOCK_NAME_" + tempCount.ToString();
                    while (allNames.ContainsKey(tempName))
                    {
                        tempCount++;
                        tempName = "_TMP_BLOCK_NAME_" + tempCount.ToString();
                    }
                    tempNames.Add(item.Name, tempName);
                    tempCount++;
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
