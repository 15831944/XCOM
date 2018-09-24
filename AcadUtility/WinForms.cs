using System;

namespace AcadUtility
{
    public static class WinForms
    {
        public static void SetSelectedItemFromText(this System.Windows.Forms.ComboBox comboBox, string value)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (string.Compare((string)comboBox.Items[i], value, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }

            if (comboBox.SelectedIndex == -1 && comboBox.Items.Count > 0) comboBox.SelectedIndex = 0;
        }
    }
}
