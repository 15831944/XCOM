using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void CopyPos()
        {
            PromptEntityOptions opts = new PromptEntityOptions("Select source object: ");
            opts.AllowNone = false;
            PromptEntityResult result = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(opts);
            if (result.Status != PromptStatus.OK) return;

            PromptEntityOptions destopts = new PromptEntityOptions("Select destination object: ");
            destopts.AllowNone = false;
            PromptEntityResult destresult = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(destopts);
            if (destresult.Status != PromptStatus.OK) return;
            if (result.ObjectId == destresult.ObjectId) return;

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    RebarPos source = RebarPos.FromObjectId(tr, result.ObjectId);
                    if (source == null) return;
                    if (source.Detached) return;

                    RebarPos dest = RebarPos.FromObjectId(tr, destresult.ObjectId);
                    if (dest != null)
                    {
                        if (dest.Detached)
                        {
                            dest.Pos = source.Pos;
                        }
                        else
                        {
                            dest.Pos = source.Pos;
                            dest.Count = source.Count;
                            dest.Diameter = source.Diameter;
                            dest.Spacing = source.Spacing;
                            dest.ShapeName = source.ShapeName;
                            dest.A = source.A;
                            dest.B = source.B;
                            dest.C = source.C;
                            dest.D = source.D;
                            dest.E = source.E;
                            dest.F = source.F;
                            dest.Note = source.Note;
                            dest.ShowLength = source.ShowLength;
                            dest.Multiplier = source.Multiplier;
                        }
                        dest.Save(tr);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }

        private void CopyPosNumber()
        {
            PromptEntityOptions opts = new PromptEntityOptions("Select source object: ");
            opts.AllowNone = false;
            PromptEntityResult result = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(opts);
            if (result.Status != PromptStatus.OK) return;

            PromptEntityOptions destopts = new PromptEntityOptions("Select destination object: ");
            destopts.AllowNone = false;
            PromptEntityResult destresult = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(destopts);
            if (destresult.Status != PromptStatus.OK) return;
            if (result.ObjectId == destresult.ObjectId) return;

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    RebarPos source = RebarPos.FromObjectId(tr, result.ObjectId);
                    if (source == null) return;
                    if (source.Detached) return;

                    RebarPos dest = RebarPos.FromObjectId(tr, destresult.ObjectId);
                    if (dest != null)
                    {
                        dest.Pos = source.Pos;
                        dest.Detached = true;
                        dest.Save(tr);
                    }

                    DBText text = tr.GetObject(destresult.ObjectId, OpenMode.ForWrite) as DBText;
                    if (text != null)
                    {
                        text.TextString = source.Pos;
                    }
                    MText mtext = tr.GetObject(destresult.ObjectId, OpenMode.ForWrite) as MText;
                    if (mtext != null)
                    {
                        mtext.Contents = source.Pos;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                tr.Commit();
            }
        }

        private void CopyPosDetail()
        {
            PromptEntityOptions opts = new PromptEntityOptions("Select source object: ");
            opts.AllowNone = false;
            PromptEntityResult result = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(opts);
            if (result.Status != PromptStatus.OK) return;

            PromptEntityOptions destopts = new PromptEntityOptions("Select destination object: ");
            destopts.AllowNone = false;
            PromptEntityResult destresult = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetEntity(destopts);
            if (destresult.Status != PromptStatus.OK) return;
            if (result.ObjectId == destresult.ObjectId) return;

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    RebarPos source = RebarPos.FromObjectId(tr, result.ObjectId);
                    if (source == null) return;
                    if (source.Detached) return;

                    RebarPos dest = RebarPos.FromObjectId(tr, destresult.ObjectId);
                    if (dest != null)
                    {
                        dest.Pos = source.Pos;
                        dest.Count = source.Count;
                        dest.Diameter = source.Diameter;
                        dest.Spacing = source.Spacing;
                        dest.ShapeName = source.ShapeName;
                        dest.A = source.A;
                        dest.B = source.B;
                        dest.C = source.C;
                        dest.D = source.D;
                        dest.E = source.E;
                        dest.F = source.F;
                        dest.ShowLength = false;
                        dest.Note = "";
                        dest.Multiplier = 0;

                        dest.Save(tr);
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
