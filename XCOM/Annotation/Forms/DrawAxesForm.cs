using AcadUtility.WinForms;
using System;

namespace XCOM.Commands.Annotation
{
    public partial class DrawAxesForm : AcadUtility.WinForms.VersionDisplayForm
    {
        public bool DrawOnlyLine
        {
            get
            {
                return rbAxisLine.Checked;
            }
            set
            {
                rbAxisLine.Checked = value;
                rbAxisBlock.Checked = !value;
            }
        }

        public double AxisLineLength { get { double.TryParse(txtAxisLineLength.Text, out double v); return v; } set { txtAxisLineLength.Text = value.ToString(); } }
        public double TextHeight { get { double.TryParse(txtTextHeight.Text, out double v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public string TextStyleName
        {
            get => (string)cbTextStyle.SelectedItem;
            set => cbTextStyle.SetSelectedItemFromText(value);
        }

        public string BlockName
        {
            get => (string)cbBlockName.SelectedItem;
            set => cbBlockName.SetSelectedItemFromText(value);
        }
        public string AxisAttribute { get => txtAxisAttribute.Text; set => txtAxisAttribute.Text = value; }
        public string ChAttribute { get => txtChAttribute.Text; set => txtChAttribute.Text = value; }
        public string ChPrefix { get => txtChPrefix.Text; set => txtChPrefix.Text = value; }
        public int Precision { get { return cbPrecision.SelectedIndex; } set { cbPrecision.SelectedIndex = Math.Min(cbPrecision.Items.Count - 1, Math.Max(0, value)); } }

        public DrawAxesForm()
        {
            InitializeComponent();
        }

        public void SetBlockNames(string[] names)
        {
            cbBlockName.Items.Clear();
            for (int i = 0; i < names.Length; i++)
            {
                cbBlockName.Items.Add(names[i]);
                if (string.Compare(names[i], BlockName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cbBlockName.SelectedIndex = i;
                }
            }
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

        private void DrawingType_Check_Changed(object sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            pnlDraw.Enabled = rbAxisLine.Checked;
            pnlBlock.Enabled = rbAxisBlock.Checked;
        }
    }
}
