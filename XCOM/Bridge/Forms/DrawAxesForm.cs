using System;

namespace XCOM.Commands.Bridge
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
            get => cbTextStyle.Text;
            set => cbTextStyle.Text = value;
        }

        public string BlockName
        {
            get => cbBlockName.Text;
            set => cbBlockName.Text = value;
        }
        public string AxisAttribute { get => txtAxisAttribute.Text; set => txtAxisAttribute.Text = value; }
        public string ChAttribute { get => txtChAttribute.Text; set => txtChAttribute.Text = value; }
        public string ChPrefix { get => txtChPrefix.Text; set => txtChPrefix.Text = value; }
        public int Precision
        {
            get => cbPrecision.Precision;
            set => cbPrecision.Precision = value;
        }

        public DrawAxesForm()
        {
            InitializeComponent();
        }

        private void DrawingType_Check_Changed(object sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateUI()
        {
            pnlDraw.Enabled = rbAxisLine.Checked;
            rbAxisBlock.Enabled = (cbBlockName.Items.Count > 0);
            pnlBlock.Enabled = rbAxisBlock.Checked;
        }
    }
}
