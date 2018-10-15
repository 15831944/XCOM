using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace XCOM.Commands.Annotation
{
    public partial class LevelMainForm : AcadUtility.WinForms.VersionDisplayForm
    {
        private bool basePointSelected;
        private Point3d basePt;

        public bool UnitMeter { get { return rbMeter.Checked; } set { rbMeter.Checked = value; rbCentimeter.Checked = !value; rbMillimeter.Checked = !value; } }
        public bool UnitCentimeter { get { return rbCentimeter.Checked; } set { rbCentimeter.Checked = value; rbMeter.Checked = !value; rbMillimeter.Checked = !value; } }
        public bool UnitMillimeter { get { return rbMillimeter.Checked; } set { rbMillimeter.Checked = value; rbCentimeter.Checked = !value; rbMeter.Checked = !value; } }

        public int Precision { get { return cbPrecision.Precision; } set { cbPrecision.Precision = value; } }

        public Point3d BasePoint { get { return basePt; } set { basePointSelected = true; basePt = value; txtX.Text = basePt.X.ToString(); txtY.Text = basePt.Y.ToString(); txtZ.Text = basePt.Z.ToString(); } }

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

            basePointSelected = false;
            basePt = Point3d.Origin;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (!basePointSelected)
            {
                MessageBox.Show("Baz kotunu seçin.", "Level", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnPickBasePoint_Click(object sender, EventArgs e)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(this))
            {
                PromptPointResult ptRes = ed.GetPoint("\nBaz noktası: ");
                if (ptRes.Status == PromptStatus.OK) BasePoint = ptRes.Value;
            }
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
