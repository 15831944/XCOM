using AcadUtility.WinForms;
using System;

namespace XCOM.Commands.Annotation
{
    public partial class PrintChainageForm : AcadUtility.WinForms.VersionDisplayForm
    {
        public double TextHeight { get { double.TryParse(txtTextHeight.Text, out double v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public int Precision { get { return cbPrecision.SelectedIndex; } set { cbPrecision.SelectedIndex = Math.Min(cbPrecision.Items.Count - 1, Math.Max(0, value)); } }
        public string Prefix { get { return txtPrefix.Text; } set { txtPrefix.Text = value; } }
        public double Interval { get { double.TryParse(txtInterval.Text, out double v); return v; } set { txtInterval.Text = value.ToString(); } }

        public string TextStyleName
        {
            get => (string)cbTextStyle.SelectedItem;
            set => cbTextStyle.SetSelectedItemFromText(value);
        }

        public PrintChainageForm()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        public void SetTextStyleNames(string[] names)
        {
            cbTextStyle.Items.Clear();
            for (int i = 0; i < names.Length; i++)
            {
                cbTextStyle.Items.Add(names[i]);
                if (string.Compare(names[i], TextStyleName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cbTextStyle.SelectedIndex = i;
                }
            }
        }
    }
}
