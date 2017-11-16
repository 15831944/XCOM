using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XCOM.Commands.RoadDesign
{
    public partial class ProfileSettingsForm : XCOM.Utility.VersionDisplayForm
    {
        public double GridH { get { double v = 0; double.TryParse(txtH.Text, out v); return v; } set { txtH.Text = value.ToString(); } }
        public double GridV { get { double v = 0; double.TryParse(txtV.Text, out v); return v; } set { txtV.Text = value.ToString(); } }
        public double VScale { get { double v = 0; double.TryParse(txtVScale.Text, out v); return v; } set { txtVScale.Text = value.ToString(); } }

        public double TextHeight { get { double v = 0; double.TryParse(txtTextHeight.Text, out v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public int Precision { get { return cbPrecision.SelectedIndex; } set { cbPrecision.SelectedIndex = Math.Min(cbPrecision.Items.Count - 1, Math.Max(0, value)); } }

        public ProfileSettingsForm()
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
