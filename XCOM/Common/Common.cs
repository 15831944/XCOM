using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM
{
    public static class Common
    {
        public static DBText CreateText(Point3d pt, string text, double textHeight, double rotation, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode, ObjectId textStyleId)
        {
            DBText dbtext = new DBText();
            dbtext.TextString = text;
            dbtext.Position = pt;
            dbtext.Height = textHeight;
            dbtext.Rotation = rotation * Math.PI / 180;
            dbtext.HorizontalMode = horizontalMode;
            dbtext.VerticalMode = verticalMode;

            dbtext.AlignmentPoint = pt;
            if (!textStyleId.IsNull) dbtext.TextStyleId = textStyleId;

            return dbtext;
        }

        public static DBText CreateText(Point3d pt, string text, double textHeight, double rotation, TextHorizontalMode horizontalMode, TextVerticalMode verticalMode)
        {
            return CreateText(pt, text, textHeight, rotation, horizontalMode, verticalMode, ObjectId.Null);
        }

        public static DBText CreateText(Point3d pt, string text, double textHeight, double rotation)
        {
            return CreateText(pt, text, textHeight, rotation, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null);
        }

        public static DBText CreateText(Point3d pt, string text, double textHeight)
        {
            return CreateText(pt, text, textHeight, 0, TextHorizontalMode.TextLeft, TextVerticalMode.TextBase, ObjectId.Null);
        }

        public static Line CreateLine(Point3d pt1, Point3d pt2)
        {
            Line line = new Line();
            line.StartPoint = pt1;
            line.EndPoint = pt2;

            return line;
        }

        public static MText CreateMText(Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint, ObjectId textStyleId)
        {
            MText mtext = new MText();
            mtext.Contents = text;
            mtext.Location = pt;
            mtext.TextHeight = textHeight;
            mtext.Rotation = rotation * Math.PI / 180;
            mtext.Attachment = attachmentPoint;

            if (!textStyleId.IsNull) mtext.TextStyleId = textStyleId;

            return mtext;
        }

        public static MText CreateMText(Point3d pt, string text, double textHeight, double rotation, AttachmentPoint attachmentPoint)
        {
            return CreateMText(pt, text, textHeight, rotation, attachmentPoint, ObjectId.Null);
        }

        public static MText CreateMText(Point3d pt, string text, double textHeight, double rotation)
        {
            return CreateMText(pt, text, textHeight, rotation, AttachmentPoint.TopLeft, ObjectId.Null);
        }

        public static MText CreateMText(Point3d pt, string text, double textHeight)
        {
            return CreateMText(pt, text, textHeight, 0, AttachmentPoint.TopLeft, ObjectId.Null);
        }

        public static Polyline CreatePolyLine(bool closed, params Point3d[] points)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));

            Polyline pline = new Polyline(1);
            pline.Normal = db.Ucsxdir.CrossProduct(db.Ucsydir);
            pline.AddVertexAt(0, new Point2d(0, 0), 0, 0, 0);
            Plane plinePlane = new Plane(Point3d.Origin, pline.Normal);
            pline.Reset(false, points.Length);
            foreach (Point3d pt in points)
            {
                Point2d ecsPt = plinePlane.ParameterOf(pt); // Convert to ECS
                pline.AddVertexAt(pline.NumberOfVertices, ecsPt, 0, 0, 0);
            }
            pline.Closed = closed;

            return pline;
        }

        public static Polyline CreatePolyLine(params Point3d[] points)
        {
            return CreatePolyLine(false, points);
        }

        public static Hatch CreateHatch(string patternName, double patternScale, double patternAngle)
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            Matrix3d ucs2wcs = Matrix3d.AlignCoordinateSystem(Point3d.Origin, Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, db.Ucsorg, db.Ucsxdir, db.Ucsydir, db.Ucsxdir.CrossProduct(db.Ucsydir));

            Hatch hatch = new Hatch();

            hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);

            hatch.Normal = db.Ucsxdir.CrossProduct(db.Ucsydir);
            hatch.Elevation = 0.0;
            hatch.PatternScale = patternScale;
            hatch.PatternAngle = patternAngle;

            hatch.SetHatchPattern(HatchPatternType.PreDefined, patternName);

            return hatch;
        }
    }
}
