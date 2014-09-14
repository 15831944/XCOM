using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XCOMCore.ActionForms
{
    public partial class PurgeAllForm : Form
    {
        public PurgeAllForm()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        public bool PurgeBlocks { get { return cbBlocks.Checked; } set { cbBlocks.Checked = value; } }
        public bool PurgeTextStyles { get { return cbTextStyles.Checked; } set { cbTextStyles.Checked = value; } }
        public bool PurgeTableStyles { get { return cbTableStyles.Checked; } set { cbTableStyles.Checked = value; } }
        public bool PurgeShapes { get { return cbShapes.Checked; } set { cbShapes.Checked = value; } }
        public bool PurgePlotStyles { get { return cbPlotStyles.Checked; } set { cbPlotStyles.Checked = value; } }
        public bool PurgeMultileaderStyles { get { return cbMultileaderStyles.Checked; } set { cbMultileaderStyles.Checked = value; } }
        public bool PurgeMlineStyles { get { return cbMlineStyles.Checked; } set { cbMlineStyles.Checked = value; } }
        public bool PurgeMaterials { get { return cbMaterials.Checked; } set { cbMaterials.Checked = value; } }
        public bool PurgeLinetypes { get { return cbLinetypes.Checked; } set { cbLinetypes.Checked = value; } }
        public bool PurgeLayers { get { return cbLayers.Checked; } set { cbLayers.Checked = value; } }
        public bool PurgeGroups { get { return cbGroups.Checked; } set { cbGroups.Checked = value; } }
        public bool PurgeDimensionStyles { get { return cbDimensionStyles.Checked; } set { cbDimensionStyles.Checked = value; } }
        public bool PurgeUCSSettings { get { return cbUCSSettings.Checked; } set { cbUCSSettings.Checked = value; } }
        public bool PurgeViews { get { return cbViews.Checked; } set { cbViews.Checked = value; } }
        public bool PurgeViewports { get { return cbViewports.Checked; } set { cbViewports.Checked = value; } }
        public bool PurgeZeroLengthGeometry { get { return cbZeroLengthGeometry.Checked; } set { cbZeroLengthGeometry.Checked = value; } }
        public bool PurgeEmptyTexts { get { return cbEmptyTexts.Checked; } set { cbEmptyTexts.Checked = value; } }
        public bool PurgeRegApps { get { return cbRegApps.Checked; } set { cbRegApps.Checked = value; } }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            SetChecks(this, true);
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            SetChecks(this, false);
        }

        private void SetChecks(Control control, bool check)
        {
            if (control is CheckBox)
            {
                ((CheckBox)control).Checked = check;
            }
            else if (control.Controls.Count != 0)
            {
                foreach (Control c in control.Controls)
                {
                    SetChecks(c, check);
                }
            }
        }
    }
}
