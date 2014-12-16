using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void NewPos()
        {
            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptPointResult result = ed.GetPoint("Baz noktası: ");
            if (result.Status == PromptStatus.OK)
            {
                Database db = HostApplicationServices.WorkingDatabase;
                Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));
                Point3d pt = result.Value.TransformBy(ucs2wcs);
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        ObjectId blockId = ObjectId.Null;
                        using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
                        {
                            if (bt.Has(BlockName))
                            {
                                blockId = bt[BlockName];
                            }
                        }
                        if (blockId.IsNull)
                        {
                            MessageBox.Show("Poz bloğu '" + BlockName + "' bulunamadı.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
                        {
                            BlockReference bref = new BlockReference(pt, blockId);

                            btr.AppendEntity(bref);
                            tr.AddNewlyCreatedDBObject(bref, true);

                            Dictionary<AttributeReference, AttributeDefinition> dict = new Dictionary<AttributeReference, AttributeDefinition>();

                            BlockTableRecord blockDef = tr.GetObject(blockId, OpenMode.ForRead) as BlockTableRecord;
                            foreach (ObjectId id in blockDef)
                            {
                                AttributeDefinition attDef = tr.GetObject(id, OpenMode.ForRead) as AttributeDefinition;
                                if ((attDef != null) && (!attDef.Constant))
                                {
                                    // Create a new AttributeReference
                                    AttributeReference attRef = new AttributeReference();
                                    dict.Add(attRef, attDef);
                                    attRef.SetAttributeFromBlock(attDef, bref.BlockTransform);
                                    bref.AttributeCollection.AppendAttribute(attRef);
                                    tr.AddNewlyCreatedDBObject(attRef, true);
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    tr.Commit();
                }
            }
        }
    }
}
