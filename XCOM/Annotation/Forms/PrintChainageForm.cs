using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace XCOM.Commands.Annotation
{
    public partial class PrintChainageForm : AcadUtility.WinForms.VersionDisplayForm
    {
        public double TextHeight { get { double.TryParse(txtTextHeight.Text, out double v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public int Precision { get { return cbPrecision.Precision; } set { cbPrecision.Precision = value; } }
        public string Prefix { get { return txtPrefix.Text; } set { txtPrefix.Text = value; } }
        public double Interval { get { double.TryParse(txtInterval.Text, out double v); return v; } set { txtInterval.Text = value.ToString(); } }

        public string TextStyleName
        {
            get => cbTextStyle.Text;
            set => cbTextStyle.Text = value;
        }

        public PrintChainageForm()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
