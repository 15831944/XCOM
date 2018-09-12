using System;

namespace XCOM.Commands.Annotation
{
    public partial class NumberingForm : XCOM.CustomControls.VersionDisplayForm
    {
        public enum SelectNumberingObjects
        {
            Text,
            Block,
        }

        public enum CoordinateOrdering
        {
            IncreasingX,
            DecreasingX,
            IncreasingY,
            DecreasingY,
        }

        public SelectNumberingObjects SelectObjects
        {
            get
            {
                if (rbSelectText.Checked)
                    return SelectNumberingObjects.Text;
                else // if (rbSelectBlock.Checked)
                    return SelectNumberingObjects.Block;
            }
            set
            {
                switch (value)
                {
                    case SelectNumberingObjects.Text:
                        rbSelectText.Checked = true;
                        break;
                    default: // SelectNumberingObjects.Block
                        rbSelectBlock.Checked = true;
                        break;
                }
            }
        }

        public CoordinateOrdering Ordering
        {
            get
            {
                if (rbOrderXInc.Checked)
                    return CoordinateOrdering.IncreasingX;
                else if (rbOrderXDec.Checked)
                    return CoordinateOrdering.DecreasingX;
                else if (rbOrderYInc.Checked)
                    return CoordinateOrdering.IncreasingY;
                else // if (rbOrderYDec.Checked)
                    return CoordinateOrdering.DecreasingY;
            }
            set
            {
                switch (value)
                {
                    case CoordinateOrdering.IncreasingX:
                        rbOrderXInc.Checked = true;
                        break;
                    case CoordinateOrdering.DecreasingX:
                        rbOrderXDec.Checked = true;
                        break;
                    case CoordinateOrdering.IncreasingY:
                        rbOrderYInc.Checked = true;
                        break;
                    default: // CoordinateOrdering.DecreasingY
                        rbOrderYDec.Checked = true;
                        break;
                }
            }
        }

        public string Prefix { get { return txtPrefix.Text; } set { txtPrefix.Text = value; } }
        public double StartNumber { get { return double.Parse(numStart.Text); } set { numStart.Text = value.ToString(); } }
        public double Increment { get { return double.Parse(numIncrement.Text); } set { numIncrement.Text = value.ToString(); } }
        public string Format { get { return numFormat.Text; } set { numFormat.Text = value; } }
        public string Suffix { get { return txtSuffix.Text; } set { txtSuffix.Text = value; } }
        public string AttributeName { get { return txtAttributeName.Text; } set { txtAttributeName.Text = value; } }

        public NumberingForm()
        {
            InitializeComponent();
        }

        private void rbSelectText_CheckedChanged(object sender, EventArgs e)
        {
            txtAttributeName.Enabled = false;
        }

        private void rbSelectBlock_CheckedChanged(object sender, EventArgs e)
        {
            txtAttributeName.Enabled = true;
        }
    }
}
