using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace XCOM.Commands.Annotation
{
    public partial class LevelMainForm : AcadUtility.WinForms.VersionDisplayForm
    {
        public bool UnitMeter { get { return rbMeter.Checked; } set { rbMeter.Checked = value; rbCentimeter.Checked = !value; rbMillimeter.Checked = !value; } }
        public bool UnitCentimeter { get { return rbCentimeter.Checked; } set { rbCentimeter.Checked = value; rbMeter.Checked = !value; rbMillimeter.Checked = !value; } }
        public bool UnitMillimeter { get { return rbMillimeter.Checked; } set { rbMillimeter.Checked = value; rbCentimeter.Checked = !value; rbMeter.Checked = !value; } }

        public int Precision { get { return cbPrecision.Precision; } set { cbPrecision.Precision = value; } }

        public Point3d BasePoint { get { return pickBasePoint.PickPoint; } set { pickBasePoint.PickPoint = value; } }

        public double BaseLevel { get { double.TryParse(txtBaseLevel.Text, out double v); return v; } set { txtBaseLevel.Text = value.ToString(); } }
        public double Multiplier { get { double.TryParse(txtMultiplier.Text, out double v); return v; } set { txtMultiplier.Text = value.ToString(); } }

        public string BlockName
        {
            get => cbBlock.Text;
            set => cbBlock.Text = value;
        }
        public double BlockScale { get { double.TryParse(txtScale.Text, out double v); return v; } set { txtScale.Text = value.ToString(); } }

        public LevelMainForm()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (!pickBasePoint.IsPointSet)
            {
                MessageBox.Show("Baz kotunu seçin.", "Level", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void txtMultiplier_TextChanged(object sender, EventArgs e)
        {
            bool isDefault = false;
            if (double.TryParse(txtMultiplier.Text, out double val))
            {
                if (Math.Abs(val - 1.0) < double.Epsilon) isDefault = true;
            }
            txtMultiplier.BackColor = (isDefault ? SystemColors.Window : Color.MistyRose);
        }
    }
}
