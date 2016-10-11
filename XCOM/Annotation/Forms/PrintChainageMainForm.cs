using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using System.Text.RegularExpressions;

namespace XCOM.Commands.Annotation
{
    public partial class PrintChainageMainForm : XCOM.Utility.VersionDisplayForm
    {
        public double TextHeight { get { double v = 0; double.TryParse(txtTextHeight.Text, out v); return v; } set { txtTextHeight.Text = value.ToString(); } }
        public int Precision { get { return cbPrecision.SelectedIndex; } set { cbPrecision.SelectedIndex = Math.Min(cbPrecision.Items.Count - 1, Math.Max(0, value)); } }
        public string Prefix { get { return txtPrefix.Text; } set { txtPrefix.Text = value; } }
        public double Interval { get { double v = 0; double.TryParse(txtInterval.Text, out v); return v; } set { txtInterval.Text = value.ToString(); } }

        public string TextStyleName
        {
            get
            {
                return (string)cbTextStyle.SelectedItem;
            }
            set
            {
                for (int i = 0; i < cbTextStyle.Items.Count; i++)
                {
                    if (string.Compare((string)cbTextStyle.Items[i], value, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        cbTextStyle.SelectedIndex = i;
                        return;
                    }
                }

                if (cbTextStyle.SelectedIndex == -1 && cbTextStyle.Items.Count > 0) cbTextStyle.SelectedIndex = 0;
            }
        }

        public PrintChainageMainForm()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        public void SetTextStyleNames(string[] names)
        {
            cbTextStyle.Items.Clear();
            for (int i = 0; i < names.Length; i++)
            {
                cbTextStyle.Items.Add(names[i]);
                if (string.Compare(names[i], TextStyleName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    cbTextStyle.SelectedIndex = i;
                }
            }
        }
    }
}
