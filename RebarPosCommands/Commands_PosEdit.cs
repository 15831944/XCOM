using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void PosEdit(ObjectId id, Point3d pt)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    RebarPos pos = RebarPos.FromObjectId(tr, id);
                    if (pos != null)
                    {
                        if (pos.Detached)
                        {
                            using (EditDetachedPosForm form = new EditDetachedPosForm())
                            {
                                if (form.Init(pos, pt))
                                {
                                    if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                                    {
                                        pos.Save(tr);
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (EditPosForm form = new EditPosForm())
                            {
                                if (form.Init(pos, pt))
                                {
                                    if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                                    {
                                        pos.Save(tr);
                                    }
                                }
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
