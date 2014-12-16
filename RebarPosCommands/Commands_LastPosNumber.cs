using System;
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
        private int GetLastPosNumber(IEnumerable<ObjectId> list)
        {
            int num = -1;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in list)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, id);
                        if (pos != null)
                        {
                            int i = -1;
                            if (int.TryParse(pos.Pos, out i))
                            {
                                num = Math.Max(i, num);
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

            return num;
        }
    }
}
