using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace RebarPosCommands
{
    public class PosCopy
    {
        public List<ObjectId> list;

        public string key;

        public string pos;
        public string newpos;

        public int count;
        public int priority;
        public string diameter;

        public string length;
        public string a;
        public string b;
        public string c;
        public string d;
        public string e;
        public string f;

        public string shapename;

        public double scale;

        public double x;
        public double y;

        public int fieldCount;
        public bool isVarA;
        public bool isVarB;
        public bool isVarC;
        public bool isVarD;
        public bool isVarE;
        public bool isVarF;
        public double maxA;
        public double maxB;
        public double maxC;
        public double maxD;
        public double maxE;
        public double maxF;
        public double minA;
        public double minB;
        public double minC;
        public double minD;
        public double minE;
        public double minF;

        public double length1;
        public double length2;
        public bool isVarLength;

        public bool existing;

        public PosCopy()
        {
            list = new List<ObjectId>();

            key = string.Empty;

            pos = string.Empty;
            newpos = string.Empty;

            count = 0;
            priority = -1;
            diameter = string.Empty;

            length = string.Empty;
            a = string.Empty;
            b = string.Empty;
            c = string.Empty;
            d = string.Empty;
            e = string.Empty;
            f = string.Empty;

            shapename = string.Empty;

            scale = 0;
            x = double.MaxValue;
            y = double.MaxValue;

            length1 = 0.0;
            length2 = 0.0;
            isVarLength = false;

            existing = false;
        }

        public enum PosGrouping
        {
            None = 0,
            PosKey = 1,
            PosKeyVarLength = 2,
            PosMarker = 3,
            PosKeyDifferentMarker = 4,
            PosKeyDifferentMarkerVarLength = 5
        }

        public static List<PosCopy> ReadAllInSelection(IEnumerable<ObjectId> items, bool skipEmpty, PosGrouping grouping)
        {
            List<PosCopy> poslist = new List<PosCopy>();

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId id in items)
                {
                    RebarPos pos = RebarPos.FromObjectId(tr, id);
                    BlockReference bref = (BlockReference)tr.GetObject(id, OpenMode.ForRead);

                    if (pos != null)
                    {
                        // Skip empty pos numbers
                        if (skipEmpty && string.IsNullOrEmpty(pos.Pos)) continue;
                        // Skip detached pos
                        if (pos.Detached) continue;

                        PosCopy copy = null;
                        if (grouping == PosGrouping.PosKey)
                        {
                            copy = poslist.Find(p => p.key == pos.PosKey);
                        }
                        else if (grouping == PosGrouping.PosKeyVarLength && !pos.Properties.Length.IsVariable)
                        {
                            copy = poslist.Find(p => p.key == pos.PosKey);
                        }
                        else if (grouping == PosGrouping.PosMarker)
                        {
                            copy = poslist.Find(p => p.pos == pos.Pos);
                        }
                        else if (grouping == PosGrouping.PosKeyDifferentMarker)
                        {
                            copy = poslist.Find(p => p.key == pos.PosKey && p.pos == pos.Pos);
                        }
                        else if (grouping == PosGrouping.PosKeyDifferentMarkerVarLength && !pos.Properties.Length.IsVariable)
                        {
                            copy = poslist.Find(p => p.key == pos.PosKey && p.pos == pos.Pos);
                        }

                        if (copy != null)
                        {
                            copy.list.Add(id);
                            if (pos.Multiplier > 0)
                            {
                                copy.count += pos.Properties.Count * pos.Multiplier;
                            }
                            copy.scale = Math.Max(copy.scale, Math.Abs(bref.ScaleFactors[0]));
                            copy.x = Math.Min(copy.x, bref.Position.X);
                            copy.y = Math.Min(copy.y, bref.Position.Y);
                        }
                        else
                        {
                            copy = new PosCopy();
                            copy.key = pos.PosKey;
                            copy.list.Add(id);
                            copy.pos = pos.Pos;
                            copy.newpos = pos.Pos;
                            copy.existing = true;
                            if (pos.Multiplier > 0)
                            {
                                copy.count = pos.Properties.Count * pos.Multiplier;
                            }
                            copy.diameter = pos.Diameter;

                            copy.a = pos.A;
                            copy.b = pos.B;
                            copy.c = pos.C;
                            copy.d = pos.D;
                            copy.e = pos.E;
                            copy.f = pos.F;

                            RebarPos.CalculatedProperties calc = pos.Properties;
                            copy.fieldCount = PosShape.Shapes[pos.ShapeName].Fields;
                            copy.minA = calc.A.Minimum;
                            copy.minB = calc.B.Minimum;
                            copy.minC = calc.C.Minimum;
                            copy.minD = calc.D.Minimum;
                            copy.minE = calc.E.Minimum;
                            copy.minF = calc.F.Minimum;
                            copy.maxA = calc.A.Maximum;
                            copy.maxB = calc.B.Maximum;
                            copy.maxC = calc.C.Maximum;
                            copy.maxD = calc.D.Maximum;
                            copy.maxE = calc.E.Maximum;
                            copy.maxF = calc.F.Maximum;
                            copy.isVarA = calc.A.IsVariable;
                            copy.isVarB = calc.B.IsVariable;
                            copy.isVarC = calc.C.IsVariable;
                            copy.isVarD = calc.D.IsVariable;
                            copy.isVarE = calc.E.IsVariable;
                            copy.isVarF = calc.F.IsVariable;
                            copy.length1 = calc.Length.Minimum;
                            copy.length2 = calc.Length.Maximum;
                            copy.isVarLength = calc.Length.IsVariable;
                            copy.length = calc.Length.ConvertToString(0, '~');

                            copy.scale = Math.Abs(bref.ScaleFactors[0]);
                            copy.x = bref.Position.X;
                            copy.y = bref.Position.Y;
                            copy.shapename = pos.ShapeName;
                            PosShape shape = PosShape.Shapes[copy.shapename];
                            if (shape != null)
                            {
                                copy.priority = shape.Priority;
                            }
                            poslist.Add(copy);
                        }
                    }
                }
                tr.Commit();
            }

            return poslist;
        }

        public static List<PosCopy> ReadAll(PosGrouping grouping, bool skipEmpty)
        {
            List<ObjectId> items = new List<ObjectId>();

            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                using (BlockTableRecordEnumerator it = btr.GetEnumerator())
                {
                    while (it.MoveNext())
                    {
                        if (it.Current.ObjectClass == RXObject.GetClass(typeof(RebarPos)))
                        {
                            items.Add(it.Current);
                        }
                    }
                }
            }

            return ReadAllInSelection(items, skipEmpty, grouping);
        }
    }
}
