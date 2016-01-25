using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;

namespace RebarPosCommands
{
    public partial class GroupForm : VersionDisplayForm
    {
        private class GroupCopy
        {
            public PosGroup.DrawingUnits DrawingUnit;
            public PosGroup.DrawingUnits DisplayUnit;
            public int Precision;
            public double MaxBarLength;
            public bool Bending;

            public List<int> StandardDiameters;
        }

        private GroupCopy mCopy;

        public GroupForm()
        {
            InitializeComponent();
            Width = 340;

            mCopy = new GroupCopy();
        }

        public bool Init()
        {
            PosGroup group = PosGroup.Current;
            if (group == null) return false;

            mCopy.DrawingUnit = group.DrawingUnit;
            mCopy.DisplayUnit = group.DisplayUnit;
            mCopy.Precision = group.Precision;
            mCopy.MaxBarLength = group.MaxBarLength;
            mCopy.Bending = group.Bending;

            mCopy.StandardDiameters = group.StandardDiameters;

            SetGroup();

            return true;
        }

        public void SetGroup()
        {
            cbDrawingUnit.SelectedIndex = (mCopy.DrawingUnit == PosGroup.DrawingUnits.Millimeter ? 0 : 1);
            cbDisplayUnit.SelectedIndex = (mCopy.DisplayUnit == PosGroup.DrawingUnits.Millimeter ? 0 : 1);
            udPrecision.Value = mCopy.Precision;
            txtMaxLength.Text = mCopy.MaxBarLength.ToString();
            chkBending.Checked = mCopy.Bending;
            txtDiameterList.Text = string.Join<int>(" ", mCopy.StandardDiameters);
        }

        private void cbDrawingUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCopy == null) return;
            mCopy.DrawingUnit = (cbDrawingUnit.SelectedIndex == 0 ? PosGroup.DrawingUnits.Millimeter : PosGroup.DrawingUnits.Centimeter);
        }

        private void cbDisplayUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mCopy == null) return;
            mCopy.DisplayUnit = (cbDisplayUnit.SelectedIndex == 0 ? PosGroup.DrawingUnits.Millimeter : PosGroup.DrawingUnits.Centimeter);
        }

        private void udPrecision_ValueChanged(object sender, EventArgs e)
        {
            if (mCopy == null) return;
            mCopy.Precision = (int)udPrecision.Value;
        }

        private void chkBending_CheckedChanged(object sender, EventArgs e)
        {
            if (mCopy == null) return;
            mCopy.Bending = chkBending.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Apply changes

            PosGroup group = PosGroup.Current;

            group.DrawingUnit = mCopy.DrawingUnit;
            group.DisplayUnit = mCopy.DisplayUnit;
            group.Precision = mCopy.Precision;
            group.MaxBarLength = mCopy.MaxBarLength;
            group.Bending = mCopy.Bending;

            group.StandardDiameters = mCopy.StandardDiameters;

            group.Save();

            RebarPos.RefreshAllPos();
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtMaxLength_Validating(object sender, CancelEventArgs e)
        {
            double val;
            if (!double.TryParse(txtMaxLength.Text, out val))
            {
                e.Cancel = true;
                errorProvider.SetError(txtMaxLength, "Lütfen bir reel sayı girin.");
            }
            else
                errorProvider.SetError(txtMaxLength, "");
        }

        private void txtMaxLength_Validated(object sender, EventArgs e)
        {
            if (mCopy == null) return;
            double val;
            if (double.TryParse(txtMaxLength.Text, out val))
            {
                mCopy.MaxBarLength = val;
            }
        }

        private void txtDiameterList_Validating(object sender, CancelEventArgs e)
        {
            foreach (string ds in txtDiameterList.Text.Split(new char[] { ' ', ',', ';', ':', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int d = 0;
                if (!int.TryParse(ds, out d))
                {
                    errorProvider.SetError(txtDiameterList, "Çaplar tam sayı olarak girilip boşluk karakteri ile ayrılmalıdır.");
                    e.Cancel = true;
                }
                else
                    errorProvider.SetError(txtDiameterList, "");
            }
        }

        private void txtDiameterList_Validated(object sender, EventArgs e)
        {
            if (mCopy == null) return;
            List<int> diameters = new List<int>();
            foreach (string ds in txtDiameterList.Text.Split(new char[] { ' ', ',', ';', ':', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int d = 0;
                if (int.TryParse(ds, out d) && d != 0)
                {
                    diameters.Add(d);
                }
            }
            mCopy.StandardDiameters = diameters;
        }
    }
}
