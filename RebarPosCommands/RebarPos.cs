using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

namespace RebarPosCommands
{
    public class RebarPos
    {
        public static string BlockName { get { return "POZ_5"; } }

        public static char DiameterSymbol = 'ƒ';
        public static char SpacingSymbol = '/';

        private static string MultiplierDPName = "METRAJ_CARPAN";
        private static string MultiplierDPValue_0 = "METRAJA_DAHIL_DEGIL";
        private static string MultiplierDPValue_1 = "METRAJA_DAHIL";

        public enum HitTestResult
        {
            None = 0,
            Pos = 1,
            Count = 2,
            Diameter = 3,
            Spacing = 4,
            Group = 5,
            Multiplier = 6,
            Length = 7,
            Note = 8
        }

        public ObjectId ID { get; private set; }
        public ObjectId NoteID { get; private set; }
        public ObjectId LengthID { get; private set; }

        public string Pos { get; set; }
        public string Diameter { get; set; }
        public string Count { get; set; }
        public string Spacing { get; set; }
        public string Note { get; set; }
        public Version BlockVersion { get; private set; }

        public string ShapeName { get; set; }
        public PosShape Shape
        {
            get
            {
                PosShape shape;
                if (PosShape.Shapes.TryGetValue(ShapeName, out shape))
                    return shape;
                else
                    return PosShape.UnknownPosShape;
            }
        }

        public string PosKey
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("T");
                sb.Append(Diameter);
                sb.Append(":S");
                sb.Append(ShapeName);

                int fields = PosShape.Shapes[ShapeName].Fields;
                if (fields >= 1) { sb.Append(":A"); sb.Append(A); }
                if (fields >= 2) { sb.Append(":B"); sb.Append(B); }
                if (fields >= 3) { sb.Append(":C"); sb.Append(C); }
                if (fields >= 4) { sb.Append(":D"); sb.Append(D); }
                if (fields >= 5) { sb.Append(":E"); sb.Append(E); }
                if (fields >= 6) { sb.Append(":F"); sb.Append(F); }

                return sb.ToString();
            }
        }

        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string E { get; set; }
        public string F { get; set; }

        public int Multiplier { get; set; }
        public bool ShowLength { get; set; }

        public CalculatedProperties Properties { get; private set; }

        public bool HasLengthAttribute { get; private set; }
        public bool HasNoteAttribute { get; private set; }

        public bool Detached
        {
            get
            {
                return string.IsNullOrEmpty(ShapeName);
            }
            set
            {
                if (value)
                {
                    Diameter = "";
                    Count = "";
                    Spacing = "";
                    Note = "";

                    ShapeName = "";
                    A = "";
                    B = "";
                    C = "";
                    D = "";
                    E = "";
                    F = "";

                    Multiplier = 0;
                    ShowLength = false;
                }
            }
        }

        private RebarPos(ObjectId id)
        {
            ID = id;
            NoteID = ObjectId.Null;
            LengthID = ObjectId.Null;
        }

        public HitTestResult HitTest(Point3d pt)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockReference bref = tr.GetObject(ID, OpenMode.ForRead) as BlockReference;
                    if (bref == null) return HitTestResult.None;
                    foreach (ObjectId attId in bref.AttributeCollection)
                    {
                        AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                        Extents3d ex = attRef.GeometricExtents;
                        if (pt.X > ex.MinPoint.X && pt.X < ex.MaxPoint.X && pt.Y > ex.MinPoint.Y && pt.Y < ex.MaxPoint.Y)
                        {
                            switch (attRef.Tag.ToLowerInvariant())
                            {
                                case "poz":
                                    return HitTestResult.Pos;
                                case "boy":
                                    return HitTestResult.Length;
                                case "not":
                                    return HitTestResult.Note;
                                case "yazi":
                                    return HitTestResult.Count;
                            }
                        }
                    }

                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return HitTestResult.None;
        }

        public static RebarPos FromObjectId(Transaction tr, ObjectId id)
        {
            BlockReference bref = tr.GetObject(id, OpenMode.ForRead) as BlockReference;

            if (bref == null)
            {
                return null;
            }
            else
            {
                RebarPos pos = new RebarPos(bref.Id);

                foreach (ObjectId attId in bref.AttributeCollection)
                {
                    AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                    switch (attRef.Tag.ToLowerInvariant())
                    {
                        case "poz":
                            pos.Pos = attRef.TextString;
                            break;
                        case "boya":
                            pos.A = attRef.TextString;
                            break;
                        case "boyb":
                            pos.B = attRef.TextString;
                            break;
                        case "boyc":
                            pos.C = attRef.TextString;
                            break;
                        case "boyd":
                            pos.D = attRef.TextString;
                            break;
                        case "boye":
                            pos.E = attRef.TextString;
                            break;
                        case "boyf":
                            pos.F = attRef.TextString;
                            break;
                        case "sekil":
                            pos.ShapeName = attRef.TextString;
                            break;
                        case "carpan":
                            int val;
                            if (int.TryParse(attRef.TextString, out val))
                            {
                                pos.Multiplier = val;
                            }
                            break;
                        case "boygoster":
                            pos.ShowLength = (attRef.TextString == "1");
                            break;
                        case "not":
                            pos.Note = attRef.TextString;
                            pos.HasNoteAttribute = true;
                            pos.NoteID = attId;
                            break;
                        case "version":
                            pos.BlockVersion = new Version(attRef.TextString);
                            break;
                        case "yazi":
                            string txt = attRef.TextString;
                            // Remove all text after note text
                            int i = txt.LastIndexOf(" L=");
                            if (i != -1)
                            {
                                txt = txt.Substring(0, i);
                            }
                            // Remove all text after a space (length or note)
                            i = txt.IndexOf(' ');
                            if (i != -1)
                            {
                                pos.Note = txt.Substring(i + 1);
                                txt = txt.Substring(0, i);
                            }
                            // Explode at spacing symbol
                            i = txt.LastIndexOf(SpacingSymbol);
                            if (i != -1)
                            {
                                pos.Spacing = txt.Substring(i + 1);
                                txt = txt.Substring(0, i);
                            }
                            // Explode at diameter symbol
                            string[] parts = txt.Split(DiameterSymbol);
                            if (parts.Length == 1)
                            {
                                pos.Diameter = parts[0];
                            }
                            else if (parts.Length > 1)
                            {
                                pos.Count = parts[0];
                                pos.Diameter = parts[1];
                            }
                            break;
                        case "boy":
                            pos.HasLengthAttribute = true;
                            pos.LengthID = attId;
                            break;
                    }
                }

                pos.UpdateProperties();
                pos.Detached = (string.IsNullOrEmpty(pos.ShapeName));

                return pos;
            }
        }

        public static bool GetTotalLengths(string formula, int fields, string a, string b, string c, string d, string e, string f, string diameter, out double minLengthMM, out double maxLengthMM, out bool isVarLength)
        {
            isVarLength = false;
            minLengthMM = 0;
            maxLengthMM = 0;

            double dia = 0;
            if (double.TryParse(diameter, out dia))
            {
                string ds = PosSettings.ConvertLength(dia, PosSettings.DrawingUnits.Millimeter, PosSettings.Current.DisplayUnit).ToString();
                a = a.Replace("d", ds).Replace("D", ds);
                b = b.Replace("d", ds).Replace("D", ds);
                c = c.Replace("d", ds).Replace("D", ds);
                d = d.Replace("d", ds).Replace("D", ds);
                e = e.Replace("d", ds).Replace("D", ds);
                f = f.Replace("d", ds).Replace("D", ds);
            }

            VariableValue propsA = new VariableValue(a);
            VariableValue propsB = new VariableValue(b);
            VariableValue propsC = new VariableValue(c);
            VariableValue propsD = new VariableValue(d);
            VariableValue propsE = new VariableValue(e);
            VariableValue propsF = new VariableValue(f);

            isVarLength = propsA.IsVariable || propsB.IsVariable || propsC.IsVariable || propsD.IsVariable || propsE.IsVariable || propsF.IsVariable;

            if (isVarLength)
            {
                string minFormula = formula.Replace("A", propsA.Minimum.ToString()).Replace("B", propsB.Minimum.ToString())
                    .Replace("C", propsC.Minimum.ToString()).Replace("D", propsD.Minimum.ToString())
                    .Replace("E", propsE.Minimum.ToString()).Replace("F", propsF.Minimum.ToString());
                string maxFormula = formula.Replace("A", propsA.Maximum.ToString()).Replace("B", propsB.Maximum.ToString())
                    .Replace("C", propsC.Maximum.ToString()).Replace("D", propsD.Maximum.ToString())
                    .Replace("E", propsE.Maximum.ToString()).Replace("F", propsF.Maximum.ToString());
                double len1 = 0;
                double len2 = 0;
                Calculator.TryEvaluate(minFormula, out len1);
                Calculator.TryEvaluate(maxFormula, out len2);
                VariableValue propsLength = new VariableValue(len1, len2);
                minLengthMM = PosSettings.ConvertLength(propsLength.Minimum, PosSettings.Current.DisplayUnit, PosSettings.DrawingUnits.Millimeter);
                maxLengthMM = PosSettings.ConvertLength(propsLength.Maximum, PosSettings.Current.DisplayUnit, PosSettings.DrawingUnits.Millimeter);
            }
            else
            {
                string consFormula = formula.Replace("A", propsA.Minimum.ToString()).Replace("B", propsB.Minimum.ToString())
                    .Replace("C", propsC.Minimum.ToString()).Replace("D", propsD.Minimum.ToString())
                    .Replace("E", propsE.Minimum.ToString()).Replace("F", propsF.Minimum.ToString());
                double len = 0;
                Calculator.TryEvaluate(consFormula, out len);
                VariableValue propsLength = new VariableValue(len);
                minLengthMM = PosSettings.ConvertLength(propsLength.Minimum, PosSettings.Current.DisplayUnit, PosSettings.DrawingUnits.Millimeter);
                maxLengthMM = PosSettings.ConvertLength(propsLength.Maximum, PosSettings.Current.DisplayUnit, PosSettings.DrawingUnits.Millimeter);
            }

            return true;
        }

        public void Save(Transaction tr)
        {
            UpdateProperties();

            string len = Properties.Length.ConvertToString(PosSettings.Current.Precision, '~');

            using (BlockReference blockRef = (BlockReference)tr.GetObject(ID, OpenMode.ForRead))
            {
                // Set attributes
                foreach (ObjectId attId in blockRef.AttributeCollection)
                {
                    AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForWrite);
                    switch (attRef.Tag.ToLowerInvariant())
                    {
                        case "poz":
                            attRef.TextString = Pos;
                            break;
                        case "boya":
                            attRef.TextString = A;
                            break;
                        case "boyb":
                            attRef.TextString = B;
                            break;
                        case "boyc":
                            attRef.TextString = C;
                            break;
                        case "boyd":
                            attRef.TextString = D;
                            break;
                        case "boye":
                            attRef.TextString = E;
                            break;
                        case "boyf":
                            attRef.TextString = F;
                            break;
                        case "sekil":
                            attRef.TextString = ShapeName;
                            break;
                        case "carpan":
                            attRef.TextString = Multiplier.ToString();
                            break;
                        case "boygoster":
                            attRef.TextString = (ShowLength ? "1" : "0");
                            break;
                        case "not":
                            attRef.TextString = Note;
                            break;
                        case "boy":
                            if (ShowLength && !string.IsNullOrEmpty(len))
                            {
                                attRef.TextString = "L=" + len;
                            }
                            else
                            {
                                attRef.TextString = "";
                            }
                            break;
                        case "yazi":
                            string txt = Count;
                            if (!string.IsNullOrEmpty(Diameter)) txt += DiameterSymbol + Diameter;
                            if (!string.IsNullOrEmpty(Spacing)) txt += SpacingSymbol + Spacing;
                            if (!HasNoteAttribute && !string.IsNullOrEmpty(Note))
                            {
                                txt += " " + Note;
                            }
                            if (!HasLengthAttribute && ShowLength && !string.IsNullOrEmpty(len))
                            {
                                txt += " L=" + len;
                            }
                            attRef.TextString = txt;

                            break;
                    }
                }
                // Set dynamic block peroperties
                if (blockRef.IsDynamicBlock)
                {
                    foreach (DynamicBlockReferenceProperty prop in blockRef.DynamicBlockReferencePropertyCollection)
                    {
                        if (!prop.ReadOnly && prop.PropertyName == MultiplierDPName)
                        {
                            if (Multiplier == 0)
                                prop.Value = MultiplierDPValue_0;
                            else
                                prop.Value = MultiplierDPValue_1;
                        }
                    }
                }
            }
        }

        public void UpdateProperties()
        {
            CalculatedProperties props = new CalculatedProperties();
            int n;
            double d;

            n = 0;
            if (int.TryParse(Pos, out n)) props.Pos = n;

            d = 0;
            if (double.TryParse(Diameter, out d)) props.Diameter = d;

            d = 0;
            if (Calculator.TryEvaluate(Count, out d)) props.Count = (int)d;
            props.Spacing = new VariableValue(Spacing);

            string ds = PosSettings.ConvertLength(props.Diameter, PosSettings.DrawingUnits.Millimeter, PosSettings.Current.DrawingUnit).ToString();
            props.A = new VariableValue(A.Replace("d", ds).Replace("D", ds));
            props.B = new VariableValue(B.Replace("d", ds).Replace("D", ds));
            props.C = new VariableValue(C.Replace("d", ds).Replace("D", ds));
            props.D = new VariableValue(D.Replace("d", ds).Replace("D", ds));
            props.E = new VariableValue(E.Replace("d", ds).Replace("D", ds));
            props.F = new VariableValue(F.Replace("d", ds).Replace("D", ds));

            string formula = (PosSettings.Current.Bending ? Shape.FormulaBending : Shape.Formula);
            bool isVar = props.A.IsVariable || props.B.IsVariable || props.C.IsVariable || props.D.IsVariable || props.E.IsVariable || props.F.IsVariable;
            if (isVar)
            {
                string minFormula = formula.Replace("A", props.A.Minimum.ToString()).Replace("B", props.B.Minimum.ToString())
                    .Replace("C", props.C.Minimum.ToString()).Replace("D", props.D.Minimum.ToString())
                    .Replace("E", props.E.Minimum.ToString()).Replace("F", props.F.Minimum.ToString());
                string maxFormula = formula.Replace("A", props.A.Maximum.ToString()).Replace("B", props.B.Maximum.ToString())
                    .Replace("C", props.C.Maximum.ToString()).Replace("D", props.D.Maximum.ToString())
                    .Replace("E", props.E.Maximum.ToString()).Replace("F", props.F.Maximum.ToString());
                double len1 = 0;
                double len2 = 0;
                Calculator.TryEvaluate(minFormula, out len1);
                Calculator.TryEvaluate(maxFormula, out len2);
                props.Length = new VariableValue(len1, len2);
            }
            else
            {
                string consFormula = formula.Replace("A", props.A.Minimum.ToString()).Replace("B", props.B.Minimum.ToString())
                    .Replace("C", props.C.Minimum.ToString()).Replace("D", props.D.Minimum.ToString())
                    .Replace("E", props.E.Minimum.ToString()).Replace("F", props.F.Minimum.ToString());
                double len = 0;
                Calculator.TryEvaluate(consFormula, out len);
                props.Length = new VariableValue(len);
            }

            Properties = props;
        }

        public struct VariableValue
        {
            public double Minimum { get; private set; }
            public double Maximum { get; private set; }
            public bool IsVariable { get; private set; }

            public VariableValue(double value)
                : this()
            {
                Minimum = value;
                Maximum = value;
                IsVariable = false;
            }

            public VariableValue(double minimum, double maximum)
                : this()
            {
                Minimum = minimum;
                Maximum = maximum;
                IsVariable = true;
            }

            public VariableValue(string value, params char[] separator)
                : this()
            {
                if (value == null) value = "";
                List<double> vals = new List<double>();
                string[] parts = value.Split(separator);
                foreach (string part in parts)
                {
                    double partval = 0;
                    if (Calculator.TryEvaluate(part, out partval))
                    {
                        vals.Add(partval);
                    }
                }

                if (vals.Count == 0)
                {
                    Minimum = 0;
                    Maximum = 0;
                    IsVariable = false;
                }
                else if (vals.Count == 1)
                {
                    Minimum = vals[0];
                    Maximum = vals[0];
                    IsVariable = false;
                }
                else // if (vals.Count == 2)
                {
                    Minimum = vals[0];
                    Maximum = vals[1];
                    IsVariable = true;
                }
            }

            public VariableValue(string value)
                : this(value, '-', '~')
            {
                ;
            }

            public string ConvertToString(int precision, char separator)
            {
                string format = "F" + precision.ToString();
                return (IsVariable ? Minimum.ToString(format) + separator + Maximum.ToString(format) : Minimum.ToString(format));
            }
        }

        public struct CalculatedProperties
        {
            public int Pos { get; set; }
            public double Diameter { get; set; }
            public int Count { get; set; }
            public VariableValue Spacing { get; set; }

            public VariableValue A { get; set; }
            public VariableValue B { get; set; }
            public VariableValue C { get; set; }
            public VariableValue D { get; set; }
            public VariableValue E { get; set; }
            public VariableValue F { get; set; }

            public VariableValue Length { get; set; }
        }

        #region Static Functions
        public static bool EnsureBlockExists()
        {
            bool exists = false;
            // Find and insert the pos block
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            using (BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead))
            {
                if (bt.Has(BlockName))
                {
                    exists = true;
                }
                else
                {
                    try
                    {
                        string blockPath = HostApplicationServices.Current.FindFile(BlockName + ".dwg", null, FindFileHint.Default);
                        using (Database dbBlock = new Database(false, true))
                        {
                            dbBlock.ReadDwgFile(blockPath, System.IO.FileShare.Read, true, "");
                            db.Insert(RebarPos.BlockName, dbBlock, true);
                        }
                        exists = true;
                    }
                    catch
                    {
                        ;
                    }
                }
                tr.Commit();
            }

            return exists;
        }

        public class PromptRebarSelectionResult
        {
            public PromptStatus Status { get; private set; }
            public SelectionSet Value { get; private set; }

            public PromptRebarSelectionResult(PromptStatus s, SelectionSet v)
            {
                Status = s;
                Value = v;
            }
        }

        public static PromptRebarSelectionResult SelectAllPosUser()
        {
            try
            {
                TypedValue[] tvs = new TypedValue[] {
                    new TypedValue((int)DxfCode.Start, "INSERT")
                };
                PromptSelectionResult res = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.GetSelection(new SelectionFilter(tvs));
                if (res.Status == PromptStatus.OK)
                {
                    IEnumerable<ObjectId> idList = FilterBlocks(res.Value.GetObjectIds());
                    SelectionSet set = SelectionSet.FromObjectIds(new List<ObjectId>(idList).ToArray());
                    return new PromptRebarSelectionResult(res.Status, set);
                }
            }
            catch
            {
                ;
            }

            return new PromptRebarSelectionResult(PromptStatus.Error, SelectionSet.FromObjectIds(new ObjectId[0]));
        }

        private static IEnumerable<ObjectId> FilterBlocks(IEnumerable<ObjectId> idList)
        {
            List<ObjectId> result = new List<ObjectId>();

            Database db = HostApplicationServices.WorkingDatabase;
            foreach (ObjectId id in idList)
            {
                if (string.Compare(AcadUtility.AcadEntity.GetBlockName(db, id), RebarPos.BlockName, StringComparison.OrdinalIgnoreCase) == 0) result.Add(id);
            }

            return result;
        }

        public static IEnumerable<ObjectId> GetAllPos()
        {
            List<ObjectId> list = new List<ObjectId>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    using (BlockTableRecordEnumerator it = btr.GetEnumerator())
                    {
                        while (it.MoveNext())
                        {
                            if (it.Current.ObjectClass == Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(BlockReference)))
                            {
                                list.Add(it.Current);
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return FilterBlocks(list);
        }

        public static IEnumerable<ObjectId> GetPosWithShape(string shape)
        {
            List<ObjectId> list = new List<ObjectId>();
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForRead);
                    using (BlockTableRecordEnumerator it = btr.GetEnumerator())
                    {
                        while (it.MoveNext())
                        {
                            RebarPos pos = RebarPos.FromObjectId(tr, it.Current);
                            if (pos != null)
                            {
                                if (pos.ShapeName == shape)
                                {
                                    list.Add(it.Current);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return list;
        }

        // Returns the maximum pos number within the given pos blocks.
        public static int GetMaximumPosNumber(IEnumerable<ObjectId> ids)
        {
            int num = -1;
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId id in ids)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, id);
                        if (pos != null)
                        {
                            int i = -1;
                            if (int.TryParse(pos.Pos, out i))
                            {
                                num = Math.Max(i, num);
                            }
                        }
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }

            return num;
        }

        // Refreshes given items
        public static void RefreshPos(IEnumerable<ObjectId> ids)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                try
                {
                    foreach (ObjectId posid in ids)
                    {
                        RebarPos pos = RebarPos.FromObjectId(tr, posid);
                        pos.UpdateProperties();
                        pos.Save(tr);
                    }
                }
                catch
                {
                    ;
                }

                tr.Commit();
            }
        }

        // Refreshes all items with the given shape
        public static void RefreshPosWithShape(string name)
        {
            RefreshPos(GetPosWithShape(name));
        }

        // Refreshes all items
        public static void RefreshAllPos()
        {
            RefreshPos(GetAllPos());
        }
        #endregion
    }
}
