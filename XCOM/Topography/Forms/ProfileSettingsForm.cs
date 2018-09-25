using System;

namespace XCOM.Commands.Topography
{
    public partial class ProfileSettingsForm : AcadUtility.WinForms.VersionDisplayForm
    {
        public double GridH { get { double v = 0; double.TryParse(txtH.Text, out v); return v; } set { txtH.Text = value.ToString(); } }
        public double GridV { get { double v = 0; double.TryParse(txtV.Text, out v); return v; } set { txtV.Text = value.ToString(); } }
        public double VScale { get { double v = 0; double.TryParse(txtVScale.Text, out v); return v; } set { txtVScale.Text = value.ToString(); } }

        public double TextHeight { get { double v = 0; double.TryParse(txtTextHeight.Text, out v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public int Precision { get { return cbPrecision.SelectedIndex; } set { cbPrecision.SelectedIndex = Math.Min(cbPrecision.Items.Count - 1, Math.Max(0, value)); } }

        public string TextStyleName
        {
            get
            {
                return (string)cbTextStyle.SelectedItem;
            }
            set
            {
                for (int i = 0; i < cbTextStyle.Items.Count; i++)
                {
                    if (string.Compare((string)cbTextStyle.Items[i], value, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        cbTextStyle.SelectedIndex = i;
                        return;
                    }
                }

                if (cbTextStyle.SelectedIndex == -1 && cbTextStyle.Items.Count > 0) cbTextStyle.SelectedIndex = 0;
            }
        }

        public ProfileSettingsForm()
        {
            InitializeComponent();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
