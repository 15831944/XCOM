using AcadUtility;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System.Windows.Forms;

namespace XCOM.Commands.Bridge
{
    public class Command_DRAWGIRDER
    {
        private enum GirderAlignment
        {
            BottomSurface,
            TopSurface
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("DRAWGIRDER")]
        public void DrawGirder()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;

            Matrix3d ucs2wcs = AcadGraphics.UcsToWcs;
            Matrix3d wcs2ucs = AcadGraphics.WcsToUcs;

            PromptEntityOptions centerlineOpts = new PromptEntityOptions("\nEksen: ");
            centerlineOpts.SetRejectMessage("\nSelect a curve.");
            centerlineOpts.AddAllowedClass(typeof(Curve), false);
            PromptEntityResult centerlineRes = ed.GetEntity(centerlineOpts);
            if (centerlineRes.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId centerlineId = centerlineRes.ObjectId;

            PromptEntityOptions blockOpts = new PromptEntityOptions("\nBlok: ");
            blockOpts.SetRejectMessage("\nSelect a block reference.");
            blockOpts.AddAllowedClass(typeof(BlockReference), false);
            PromptEntityResult blockRes = ed.GetEntity(blockOpts);
            if (blockRes.Status != PromptStatus.OK)
            {
                return;
            }
            ObjectId blockReferenceId = blockRes.ObjectId;

            var resp1 = ed.GetPoint("\nBlok üzerinde hizalanacak ilk nokta: ");
            if (resp1.Status != PromptStatus.OK)
            {
                return;
            }
            Point3d alignmentPoint1 = resp1.Value.TransformBy(ucs2wcs);

            var resp2 = ed.GetPoint("\nBlok üzerinde hizalanacak ikinci nokta: ");
            if (resp2.Status != PromptStatus.OK)
            {
                return;
            }
            Point3d alignmentPoint2 = resp2.Value.TransformBy(ucs2wcs);

            GirderAlignment alignment = GirderAlignment.TopSurface;
            PromptKeywordOptions kopts = new PromptKeywordOptions("\nHizalama yöntemi [üst Yüzey/Alt yüzey]: ", "Top Bottom");
            kopts.AllowNone = true;
            kopts.Keywords.Default = "Top";
            var resk = ed.GetKeywords(kopts);
            if (resk.Status == PromptStatus.OK && resk.StringResult == "Bottom")
            {
                alignment = GirderAlignment.BottomSurface;
            }
            else if (resk.Status == PromptStatus.OK && resk.StringResult == "Top")
            {
                alignment = GirderAlignment.TopSurface;
            }
            else if (resk.Status == PromptStatus.None)
            {
                alignment = GirderAlignment.TopSurface;
            }
            else
            {
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite))
            {
                try
                {
                    Curve centerline = tr.GetObject(centerlineId, OpenMode.ForRead) as Curve;

                    BlockReference blockRef = tr.GetObject(blockReferenceId, OpenMode.ForRead) as BlockReference;
                    ObjectId blockDefinitionId = AcadEntity.GetBlockDefinitionFromReference(db, blockReferenceId);
                    Point3d blockInsertionPoint = blockRef.Position;
                    Scale3d blockScale = blockRef.ScaleFactors;
                    blockInsertionPoint = blockInsertionPoint.TransformBy(blockRef.BlockTransform.Inverse());
                    alignmentPoint1 = alignmentPoint1.TransformBy(blockRef.BlockTransform.Inverse());
                    alignmentPoint2 = alignmentPoint2.TransformBy(blockRef.BlockTransform.Inverse());
                    Vector3d dist1 = alignmentPoint1 - blockInsertionPoint;
                    Vector3d dist2 = alignmentPoint2 - blockInsertionPoint;

                    while (true)
                    {
                        var opts = new PromptPointOptions("\nInsertion point: ");
                        opts.AllowNone = true;
                        var res = ed.GetPoint(opts);
                        if (res.Status != PromptStatus.OK)
                        {
                            break;
                        }
                        Point3d insertionPoint = res.Value.TransformBy(ucs2wcs);

                        // 10 iterations to approximate rotation
                        Point3d centerlinePoint1 = Point3d.Origin;
                        Point3d centerlinePoint2 = Point3d.Origin;
                        double rotation = 0;
                        double ucsRotation = 0;
                        for (int i = 0; i < 10; i++)
                        {
                            centerlinePoint1 = centerline.GetClosestPointTo((insertionPoint.TransformBy(wcs2ucs) + dist1.RotateBy(ucsRotation, Vector3d.ZAxis)).TransformBy(ucs2wcs), true);
                            centerlinePoint2 = centerline.GetClosestPointTo((insertionPoint.TransformBy(wcs2ucs) + dist2.RotateBy(ucsRotation, Vector3d.ZAxis)).TransformBy(ucs2wcs), true);
                            ucsRotation = Vector3d.XAxis.GetAngleTo(centerlinePoint2.TransformBy(wcs2ucs) - centerlinePoint1.TransformBy(wcs2ucs), Vector3d.ZAxis);
                            rotation = Vector3d.XAxis.GetAngleTo(centerlinePoint2 - centerlinePoint1, Vector3d.ZAxis);
                        }

                        if (alignment == GirderAlignment.TopSurface)
                        {
                            insertionPoint = (centerlinePoint1.TransformBy(wcs2ucs) - dist1.RotateBy(ucsRotation, Vector3d.ZAxis)).TransformBy(ucs2wcs);
                        }

                        BlockReference newBlock = AcadEntity.CreateBlockReference(db, blockDefinitionId, insertionPoint, blockScale, rotation);
                        btr.AppendEntity(newBlock);
                        tr.AddNewlyCreatedDBObject(newBlock, true);

                        tr.TransactionManager.QueueForGraphicsFlush();
                    }
                }
                catch (System.Exception e)
                {
                    MessageBox.Show(e.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }
    }
}