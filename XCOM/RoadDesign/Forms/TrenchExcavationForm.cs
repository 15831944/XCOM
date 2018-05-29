using System;

namespace XCOM.Commands.RoadDesign
{
    public partial class TrenchExcavationForm : XCOM.Utility.VersionDisplayForm
    {
        public double BottomWidth { get { double v = 0; double.TryParse(txtWidth.Text, out v); return v; } set { txtWidth.Text = value.ToString(); } }
        public double H { get { double v = 0; double.TryParse(txtH.Text, out v); return v; } set { txtH.Text = value.ToString(); } }
        public double V { get { double v = 0; double.TryParse(txtV.Text, out v); return v; } set { txtV.Text = value.ToString(); } }

        public TrenchExcavationForm()
        {
            InitializeComponent();
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
