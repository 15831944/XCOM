using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XCOM.Commands.Annotation
{
    public class Command_RESETDIMVIEWTWIST
    {
        [CommandMethod("RESETDIMVIEWTWIST", CommandFlags.UsePickSet)]
        public void ResetViewTwist()
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            List<TypedValue> tvs = new List<TypedValue>();
            tvs.Add(new TypedValue((int)DxfCode.Start, "DIMENSION"));
            SelectionFilter filter = new SelectionFilter(tvs.ToArray());

            PromptSelectionResult selRes = doc.Editor.GetSelection(filter);
            if (selRes.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                using (ViewTableRecord view = doc.Editor.GetCurrentView())
                {
                    try
                    {
                        foreach (ObjectId id in selRes.Value.GetObjectIds())
                        {
                            Dimension dim = tr.GetObject(id, OpenMode.ForWrite) as Dimension;
                            dim.HorizontalRotation = view.ViewTwist;
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