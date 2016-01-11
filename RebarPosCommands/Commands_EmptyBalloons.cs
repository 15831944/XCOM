using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void EmptyBalloons()
        {
            DWGUtility.PromptRebarSelectionResult result = DWGUtility.SelectAllPosUser(true);
            if (result.Status == PromptStatus.OK)
            {
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        foreach (SelectedObject sel in result.Value)
                        {
                            RebarPos pos = RebarPos.FromObjectId(tr, sel.ObjectId);
                            if (pos != null)
                            {
                                pos.Pos = "";
                                pos.Save(tr);
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
