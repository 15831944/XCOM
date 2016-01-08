using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace RebarPosCommands
{
    public class ShowShapesOverrule : DrawableOverrule
    {
        private static ShowShapesOverrule instance;

        public bool ShowShapes { get; set; }

        private ShowShapesOverrule()
        {
            ;
        }

        public static ShowShapesOverrule Instance
        {
            get
            {
                if (instance == null) instance = new ShowShapesOverrule();
                return instance;
            }
        }

        public override bool WorldDraw(Drawable drawable, WorldDraw wd)
        {
            if (ShowShapes)
            {
                // Convert to pos
                BlockReference bref = (BlockReference)drawable;
                RebarPos pos = null;
                Database db = HostApplicationServices.WorkingDatabase;
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    try
                    {
                        pos = RebarPos.FromObjectId(tr, bref.Id);
                        if (pos != null)
                        {
                            // Draw the shape
                            double scale = 50.0 * bref.ScaleFactors[0];
                            double rotation = bref.Rotation;
                            Point3d position = DWGUtility.Polar(bref.Position, rotation + Math.PI / 2.0, scale);

                            PosShape shape = pos.Shape;
                            shape.SetShapeTexts(pos.A, pos.B, pos.C, pos.D, pos.E, pos.F);
                            IEnumerable<Entity> entities = shape.ToDrawable(false, position, scale, rotation);
                            foreach (Entity en in entities)
                            {
                                wd.Geometry.Draw(en);
                                en.Dispose();
                            }
                            foreach (ObjectId attId in bref.AttributeCollection)
                            {
                                AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                                Extents3d ex = attRef.GeometricExtents;
                                wd.Geometry.Draw(new Line(ex.MinPoint, ex.MaxPoint));
                            }
                        }
                    }
                    catch
                    {
                        ;
                    }

                    tr.Commit();
                }
            }

            return base.WorldDraw(drawable, wd);
        }

        public override bool IsApplicable(Autodesk.AutoCAD.Runtime.RXObject overruledSubject)
        {
            if (!ShowShapes) return false;

            BlockReference bref = overruledSubject as BlockReference;
            if (bref == null) return false;

            return (string.Compare(bref.Name, MyCommands.BlockName, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}
