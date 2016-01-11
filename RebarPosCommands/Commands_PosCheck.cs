using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void PosCheck()
        {
            DWGUtility.PromptRebarSelectionResult sel = DWGUtility.SelectAllPosUser(true);
            if (sel.Status != PromptStatus.OK) return;

            using (CheckForm form = new CheckForm())
            {
                if (form.Init(sel.Value.GetObjectIds()))
                {
                    Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false);
                }
            }
        }
    }
}
