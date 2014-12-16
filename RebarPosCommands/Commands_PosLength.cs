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
        private void ShowPosLength(IEnumerable<ObjectId> list, bool show)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in list)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, id);
                        if (pos != null && !pos.Detached)
                        {
                            pos.ShowLength = show;
                            pos.Save(tr);
                        }
                    }

                    tr.Commit();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
