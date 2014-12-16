using System;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void PosShapes()
        {
            using (PosShapesForm form = new PosShapesForm())
            {
                if (form.Init(ShowShapes))
                {
                    if (Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(null, form, false) == DialogResult.OK)
                    {
                        ShowShapes = form.ShowShapes;

                        DWGUtility.RefreshAllPos();
                    }
                }
            }
        }
    }
}
