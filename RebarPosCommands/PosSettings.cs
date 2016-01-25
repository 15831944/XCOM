using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RebarPosCommands
{
    public class PosSettings
    {
        public enum DrawingUnits
        {
            Millimeter = 0,
            Centimeter = 1,
            Decimeter = 2,
            Meter = 3,
        }

        public bool Bending { get; set; }
        public double MaxBarLength { get; set; }
        public int Precision { get; set; }

        public DrawingUnits DrawingUnit { get; set; }
        public DrawingUnits DisplayUnit { get; set; }

        public List<int> StandardDiameters { get; set; }

        private static PosSettings m_Current = null;
        public static PosSettings Current
        {
            get
            {
                if (m_Current == null)
                {
                    Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
                    m_Current = doc.UserData["OZOZ.RebarPosCommands.PosGroup"] as PosSettings;
                    if (m_Current == null) m_Current = new PosSettings();
                }
                return m_Current;
            }
        }

        private PosSettings()
        {
            DrawingUnit = DrawingUnits.Centimeter;
            DisplayUnit = DrawingUnits.Centimeter;
            Bending = false;
            MaxBarLength = 12;
            Precision = 0;
            StandardDiameters = new List<int>() { 10, 12, 14, 16, 18, 20, 22, 26, 32, 36 };
        }

        public void Save()
        {
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            doc.UserData["OZOZ.RebarPosCommands.PosGroup"] = this;
        }

        public static double ConvertLength(double length, DrawingUnits fromUnit, DrawingUnits toUnit)
        {
            if (fromUnit == toUnit) return length;

            double scale = 1.0;

            // Convert from fromUnit to MM
            switch (fromUnit)
            {
                case DrawingUnits.Millimeter:
                    scale *= 1.0;
                    break;
                case DrawingUnits.Centimeter:
                    scale *= 10.0;
                    break;
                case DrawingUnits.Decimeter:
                    scale *= 100.0;
                    break;
                case DrawingUnits.Meter:
                    scale *= 1000.0;
                    break;
            }

            // Convert from MM to toUnit
            switch (toUnit)
            {
                case DrawingUnits.Millimeter:
                    scale /= 1.0;
                    break;
                case DrawingUnits.Centimeter:
                    scale /= 10.0;
                    break;
                case DrawingUnits.Decimeter:
                    scale /= 100.0;
                    break;
                case DrawingUnits.Meter:
                    scale /= 1000.0;
                    break;
            }

            return length * scale;
        }
    }
}
