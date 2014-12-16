﻿using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using Autodesk.AutoCAD.EditorInput;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private bool FindReplace(bool usePickSet)
        {
            // Pos error check
            PromptSelectionResult sel = DWGUtility.SelectAllPosUser(true);
            if (sel.Status != PromptStatus.OK) return false;
            ObjectId[] items = sel.Value.GetObjectIds();
            List<PosCheckResult> check = PosCheckResult.CheckAllInSelection(items, true, false);
            if (check.Count != 0)
            {
                PosCheckResult.ConsoleOut(check);
                Autodesk.AutoCAD.ApplicationServices.Application.DisplayTextScreen = true;
                return false;
            }

            using (FindReplaceForm form = new FindReplaceForm())
            {
                if (form.Init(items))
                {
                    if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == System.Windows.Forms.DialogResult.OK)
                    {
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("Çizimde poz bulunamadı.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            return false;
        }
    }
}
