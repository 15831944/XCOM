using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void DeleteBOQ()
        {
            AcadUtility.AcadEntity.AddRegAppTableRecord(HostApplicationServices.WorkingDatabase, RegAppName);

            TypedValue[] tvs = new TypedValue[] {
                new TypedValue((int)DxfCode.ExtendedDataRegAppName, RegAppName)
            };
            PromptSelectionResult result = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection(new SelectionFilter(tvs));

            if (result.Status == PromptStatus.OK)
            {
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        foreach (ObjectId id in result.Value.GetObjectIds())
                        {
                            DBObject obj = tr.GetObject(id, OpenMode.ForWrite) as DBObject;
                            if (obj != null && !obj.IsErased && string.Compare(AcadUtility.AcadEntity.GetXData(obj, RegAppName), BOQGroupName, System.StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                obj.Erase();
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
