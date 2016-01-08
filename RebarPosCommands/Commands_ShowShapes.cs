using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace RebarPosCommands
{
    public partial class MyCommands
    {
        private void ShowShapes(bool show)
        {
            ShowShapesOverrule.Instance.ShowShapes = show;
            DWGUtility.Regen();
        }
    }
}
