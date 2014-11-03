using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CoordinateLabel
{
    public partial class CoordMainForm : Form
    {
        public bool UseWCS { get { return rbWCS.Checked; } set { rbWCS.Checked = value; } }
        public bool UseUCS { get { return rbUCS.Checked; } set { rbUCS.Checked = value; } }

        public double TextHeight { get { double v = 0; double.TryParse(txtTextHeight.Text, out v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public double TextRotation { get { double v = 0; double.TryParse(txtTextAngle.Text, out v); return v; } set { txtTextAngle.Text = value.ToString(); } }
        public bool AutoLineLength { get { return cbDirection.Checked; } set { cbDirection.Checked = value; } }
        public double LineLength { get { double v = 0; double.TryParse(txtLineLength.Text, out v); return v; } set { txtLineLength.Text = value.ToString(); } }

        public int Precision { get { return cbPrecision.SelectedIndex; } set { cbPrecision.SelectedIndex = Math.Min(cbPrecision.Items.Count - 1, Math.Max(0, value)); } }

        public bool AutoNumbering { get { return rbAutoNumber.Checked; } set { rbAutoNumber.Checked = value; } }
        public int StartingNumber { get { int v = 0; int.TryParse(txtStartNum.Text, out v); return v; } set { txtStartNum.Text = value.ToString(); } }
        public string Prefix { get { return txtPrefix.Text; } set { txtPrefix.Text = value; } }

        public bool UseX { get { return rbUseX.Checked; } set { rbUseX.Checked = value; } }
        public string XLabel { get { return txtXLabel.Text; } set { txtXLabel.Text = value; } }
        public bool UseY { get { return rbUseY.Checked; } set { rbUseY.Checked = value; } }
        public string YLabel { get { return txtYLabel.Text; } set { txtYLabel.Text = value; } }
        public bool UseZ { get { return rbUseZ.Checked; } set { rbUseZ.Checked = value; } }
        public string ZLabel { get { return txtZLabel.Text; } set { txtZLabel.Text = value; } }

        public CoordMainForm()
        {
            Text = "Koordinat v" + typeof(CoordMainForm).Assembly.GetName().Version.ToString(2);

            Application.Idle += new EventHandler(Application_Idle);

            InitializeComponent();
        }

        void Application_Idle(object sender, EventArgs e)
        {
            lblLineLength.Enabled = cbDirection.Checked;
            txtLineLength.Enabled = cbDirection.Checked;

            lblStartNum.Enabled = rbAutoNumber.Checked;
            txtStartNum.Enabled = rbAutoNumber.Checked;
            lblPrefix.Enabled = rbAutoNumber.Checked;
            txtPrefix.Enabled = rbAutoNumber.Checked;

            rbUseX.Enabled = rbNoNumbering.Checked;
            rbUseY.Enabled = rbNoNumbering.Checked;
            rbUseZ.Enabled = rbNoNumbering.Checked;
            txtXLabel.Enabled = rbNoNumbering.Checked && rbUseX.Checked;
            txtYLabel.Enabled = rbNoNumbering.Checked && rbUseY.Checked;
            txtZLabel.Enabled = rbNoNumbering.Checked && rbUseZ.Checked;
            lblPrecision.Enabled = rbNoNumbering.Checked;
            cbPrecision.Enabled = rbNoNumbering.Checked;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
