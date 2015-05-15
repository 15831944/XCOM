using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM.Utility
{
    public static class Graphics
    {
        public static Matrix3d UcsToWcs()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            return doc.Editor.CurrentUserCoordinateSystem;
        }

        public static Matrix3d WcsToUcs()
        {
            return UcsToWcs().Inverse();
        }

        public static IntegerCollection GetActiveViewportNumbers()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            // Model space
            if (db.TileMode)
            {
                return new IntegerCollection();
            }

            List<int> vpNumbers = new List<int>();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                Viewport vp = tr.GetObject(doc.Editor.ActiveViewportId, OpenMode.ForRead) as Viewport;

                // Paper space 
                if (vp != null && vp.Number == 1)
                {
                    vpNumbers.Add(1);
                }
                else
                {
                    // Floating viewport 
                    foreach (ObjectId vpId in db.GetViewports(false))
                    {
                        Viewport vpFloating = (Viewport)tr.GetObject(vpId, OpenMode.ForRead);
                        vpNumbers.Add(vp.Number);
                    }
                }

                tr.Commit();
            }

            return new IntegerCollection(vpNumbers.ToArray());
        }
    }
}
