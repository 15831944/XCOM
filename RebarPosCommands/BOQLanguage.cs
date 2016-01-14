using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace RebarPosCommands
{
    public class BOQLanguage
    {
        public static string ParagraphSeparator = "\\P";
        public static string ColumnSeparator = " / ";
        public static string LengthUnit = "(m)";
        public static string UnitWeightUnit = "(kg/m)";
        public static string WeightUnit = "(kg)";

        public string Code { get; private set; }
        public string Name { get; private set; }

        public string MultiplierHeader { get; private set; }

        public string PosColumnLabel { get; private set; }
        public string DiameterColumnLabel { get; private set; }
        public string CountColumnLabel { get; private set; }
        public string RebarLengthColumnLabel { get; private set; }
        public string ShapeColumnLabel { get; private set; }
        public string LengthByDiameterColumnLabel { get; private set; }

        public string TotalLengthByDiameterRowLabel { get; private set; }
        public string UnitWeightRowLabel { get; private set; }
        public string TotalWeightByDiameterRowLabel { get; private set; }
        public string TotalWeightRowLabel { get; private set; }

        public string GrossTotalWeightByDiameterRowLabel { get; private set; }
        public string GrossTotalWeightRowLabel { get; private set; }

        public string MultiplierBy1Label { get; private set; }
        public string MultiplierByNLabel { get; private set; }

        private BOQLanguage(string code)
        {
            Code = code;

            MultiplierBy1Label = "x1";
            MultiplierByNLabel = "x{0}";
        }

        public BOQLanguage(string code, string[] rows)
            : this(code)
        {
            Name = rows[0];

            MultiplierHeader = rows[1];

            PosColumnLabel = rows[2];
            DiameterColumnLabel = rows[3];
            CountColumnLabel = rows[4];
            RebarLengthColumnLabel = rows[5];
            ShapeColumnLabel = rows[6];
            LengthByDiameterColumnLabel = rows[7];

            TotalLengthByDiameterRowLabel = rows[8];
            UnitWeightRowLabel = rows[9];
            TotalWeightByDiameterRowLabel = rows[10];
            TotalWeightRowLabel = rows[11];

            GrossTotalWeightByDiameterRowLabel = rows[12];
            GrossTotalWeightRowLabel = rows[13];
        }

        public static BOQLanguage[] FromResource(string keyPrefix)
        {
            List<BOQLanguage> languages = new List<BOQLanguage>();

            ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, false, false);
            SortedList<int, BOQLanguage> lbItems = new SortedList<int, BOQLanguage>();
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = (string)entry.Key;
                if (resourceKey.StartsWith(keyPrefix))
                {
                    string code = resourceKey.Substring(keyPrefix.Length);
                    // Replace uppercase Turkish i's and s's
                    string[] rows = ((string)entry.Value).Replace('İ', 'I').Replace('Ş', 'S').Replace('Ğ', 'G').Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    BOQLanguage lang = new BOQLanguage(code, rows);
                    languages.Add(lang);
                }
            }

            return languages.ToArray();
        }

        public static BOQLanguage[] FromResource(string keyPrefix, IEnumerable<string> order)
        {
            List<BOQLanguage> orderedLanguages = new List<BOQLanguage>();

            Dictionary<string, BOQLanguage> lookup = FromResource(keyPrefix).ToDictionary(lang => lang.Code);
            foreach (string id in order)
            {
                BOQLanguage lang = null;
                if (lookup.TryGetValue(id, out lang))
                {
                    orderedLanguages.Add(lang);
                    lookup.Remove(id);
                }
            }
            foreach (BOQLanguage lang in lookup.Values)
            {
                orderedLanguages.Add(lang);
            }

            return orderedLanguages.ToArray();
        }

        public static BOQLanguage GetEffectiveLanguage(IEnumerable<BOQLanguage> languages, int multiplier, string rebarLengthUnit)
        {
            BOQLanguage lang = new BOQLanguage(string.Join(", ", languages.Select(e => e.Code)));

            lang.MultiplierHeader = string.Format(string.Join(ColumnSeparator, languages.Select(e => e.MultiplierHeader)), multiplier);

            lang.PosColumnLabel = string.Join(ParagraphSeparator, languages.Select(e => e.PosColumnLabel));
            lang.DiameterColumnLabel = string.Join(ParagraphSeparator, languages.Select(e => e.DiameterColumnLabel));
            lang.CountColumnLabel = string.Join(ParagraphSeparator, languages.Select(e => e.CountColumnLabel));
            lang.RebarLengthColumnLabel = string.Join(ParagraphSeparator, languages.Select(e => e.RebarLengthColumnLabel)) + ParagraphSeparator + rebarLengthUnit;
            lang.ShapeColumnLabel = string.Join(ParagraphSeparator, languages.Select(e => e.ShapeColumnLabel));
            lang.LengthByDiameterColumnLabel = string.Join(ColumnSeparator, languages.Select(e => e.LengthByDiameterColumnLabel)) + " " + LengthUnit;

            lang.TotalLengthByDiameterRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.TotalLengthByDiameterRowLabel)) + " " + LengthUnit;
            lang.UnitWeightRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.UnitWeightRowLabel)) + " " + UnitWeightUnit;

            if (multiplier == 1)
            {
                lang.TotalWeightByDiameterRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.TotalWeightByDiameterRowLabel)) + " " + WeightUnit;
                lang.TotalWeightRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.TotalWeightRowLabel)) + " " + WeightUnit;

                lang.GrossTotalWeightByDiameterRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.GrossTotalWeightByDiameterRowLabel)) + " " + WeightUnit;
                lang.GrossTotalWeightRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.GrossTotalWeightRowLabel)) + " " + WeightUnit;
            }
            else
            {
                lang.TotalWeightByDiameterRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.TotalWeightByDiameterRowLabel)) + " " + WeightUnit;
                lang.TotalWeightRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.TotalWeightRowLabel)) + " " + WeightUnit;

                lang.GrossTotalWeightByDiameterRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.GrossTotalWeightByDiameterRowLabel)) + " " + WeightUnit;
                lang.GrossTotalWeightRowLabel = string.Join(ColumnSeparator, languages.Select(e => e.GrossTotalWeightRowLabel)) + " " + WeightUnit;
            }

            lang.MultiplierByNLabel = string.Format(lang.MultiplierByNLabel, multiplier);

            return lang;
        }

        public override string ToString()
        {
            return Code + " (" + Name + ")";
        }
    }
}
