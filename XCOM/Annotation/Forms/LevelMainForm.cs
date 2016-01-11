using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM.Commands.Annotation
{
    public partial class LevelMainForm : XCOM.Utility.VersionDisplayForm
    {
        private bool basePointSelected;
        private Point3d basePt;

        public bool UnitMeter { get { return rbMeter.Checked; } set { rbMeter.Checked = value; rbCentimeter.Checked = !value; rbMillimeter.Checked = !value; } }
        public bool UnitCentimeter { get { return rbCentimeter.Checked; } set { rbCentimeter.Checked = value; rbMeter.Checked = !value; rbMillimeter.Checked = !value; } }
        public bool UnitMillimeter { get { return rbMillimeter.Checked; } set { rbMillimeter.Checked = value; rbCentimeter.Checked = !value; rbMeter.Checked = !value; } }

        public int Precision { get { return cbPrecision.SelectedIndex; } set { cbPrecision.SelectedIndex = Math.Min(cbPrecision.Items.Count - 1, Math.Max(0, value)); } }

        public Point3d BasePoint { get { return basePt; } set { basePointSelected = true; basePt = value; txtX.Text = basePt.X.ToString(); txtY.Text = basePt.Y.ToString(); txtZ.Text = basePt.Z.ToString(); } }

        public double BaseLevel { get { double v = 0; double.TryParse(txtBaseLevel.Text, out v); return v; } set { txtBaseLevel.Text = value.ToString(); } }
        public string BlockName
        {
            get
            {
                return (string)cbBlock.SelectedItem;
            }
            set
            {
                for (int i = 0; i < cbBlock.Items.Count; i++)
                {
                    if (string.Compare((string)cbBlock.Items[i], value, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        cbBlock.SelectedIndex = i;
                        return;
                    }
                }

                if (cbBlock.SelectedIndex == -1 && cbBlock.Items.Count > 0) cbBlock.SelectedIndex = 0;
            }
        }
        public double BlockScale { get { double v = 0; double.TryParse(txtScale.Text, out v); return v; } set { txtScale.Text = value.ToString(); } }

        public LevelMainForm()
        {
            InitializeComponent();

            basePointSelected = false;
            basePt = Point3d.Origin;
        }

        public void SetBlockNames(string[] names)
        {
            cbBlock.Items.Clear();
            for (int i = 0; i < names.Length; i++)
            {
                cbBlock.Items.Add(names[i]);
                if (string.Compare(names[i], BlockName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cbBlock.SelectedIndex = i;
                }
            }
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
    }
}
