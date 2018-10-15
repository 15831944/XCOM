using System;

namespace AcadUtility.WinForms
{
    public class PrecisionComboBox : StringListComboBox
    {
        public int Precision
        {
            get => SelectedIndex;
            set => SelectedIndex = Math.Min(this.Items.Count - 1, Math.Max(0, value));
        }

        public PrecisionComboBox()
        {
            this.SetItems(new string[] {
                "0 - 0",
                "1 - 0.0",
                "2 - 0.00",
                "3 - 0.000",
                "4 - 0.0000",
                "5 - 0.00000",
                "6 - 0.000000",
                "7 - 0.0000000",
                "8 - 0.00000000"});
        }
    }
}
