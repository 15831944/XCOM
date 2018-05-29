using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace AcadUtility
{
    // Graphics utilities
    public static class AcadGraphics
    {
        public static Matrix3d UcsToWcs
        {
            get
            {
                Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                return doc.Editor.CurrentUserCoordinateSystem;
            }
        }

        public static Matrix3d WcsToUcs
        {
            get
            {
                return UcsToWcs.Inverse();
            }
        }

        // Zooms to given objects
        public static void ZoomToObjects(Database db, IEnumerable<ObjectId> ids)
        {
            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;

            Extents3d outerext = new Extents3d();
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in ids)
                    {
                        Autodesk.AutoCAD.DatabaseServices.Entity ent = tr.GetObject(id, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Entity;
                        Extents3d ext = ent.GeometricExtents;
                        outerext.AddExtents(ext);
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            outerext.TransformBy(ed.CurrentUserCoordinateSystem.Inverse());
            Point2d min2d = new Point2d(outerext.MinPoint.X, outerext.MinPoint.Y);
            Point2d max2d = new Point2d(outerext.MaxPoint.X, outerext.MaxPoint.Y);

            ViewTableRecord view = new ViewTableRecord();

            view.CenterPoint = min2d + ((max2d - min2d) / 2.0);
            view.Height = max2d.Y - min2d.Y;
            view.Width = max2d.X - min2d.X;

            ed.SetCurrentView(view);
        }

        // Gets the model space background color
        public static System.Drawing.Color ModelBackgroundColor()
        {
            try
            {
                dynamic pref = Autodesk.AutoCAD.ApplicationServices.Application.Preferences;
                uint indexColor = pref.Display.GraphicsWinModelBackgrndColor;
                return System.Drawing.ColorTranslator.FromOle((int)indexColor);
            }
            catch
            {
                return System.Drawing.Color.Black;
            }
        }

        // Regenerates the drawing window
        public static void Regen()
        {
            Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            ed.Regen();
        }

        public static IntegerCollection GetActiveViewportNumbers()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
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
