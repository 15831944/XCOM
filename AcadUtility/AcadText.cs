﻿using System;

namespace AcadUtility
{
    // Text utilities
    public static class AcadText
    {
        public static double ChainageFromString(string text)
        {
            double result = 0;
            if (!string.IsNullOrEmpty(text))
            {
                text = text.Replace(',', '.');
                text = text.Replace("+", "");
                double.TryParse(text, out result);
            }
            return result;
        }

        public static bool TryChainageFromString(string text, out double value)
        {
            value = 0;
            if (string.IsNullOrEmpty(text)) return false;

            text = text.Replace(',', '.');
            text = text.Replace("+", "");

            return double.TryParse(text, out value);
        }

        public static string ChainageToString(double value)
        {
            return ChainageToString(value, 2);
        }

        public static string ChainageToString(double value, int precision)
        {
            double km = Math.Floor(value / 1000);
            double m = value - km * 1000;
            string format = "{0:0}+{1:000" + (precision == 0 ? "" : "." + new string('0', precision)) + "}";
            return string.Format(format, km, m);
        }

        public static string LevelToString(double value)
        {
            return LevelToString(value, 2);
        }

        public static string LevelToString(double value, int precision)
        {
            string format = (precision == 0 ? "0" : "0." + new string('0', precision));

            string str = Math.Abs(value).ToString(format);

            if (str == format)
                str = "%%p" + str;
            else if (value < 0)
                str = "-" + str;
            else
                str = "+" + str;

            return str;
        }
    }
}