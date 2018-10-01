using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XCOM.Commands.Utility
{
    public partial class PrintEntitiesForm : AcadUtility.WinForms.VersionDisplayForm
    {
        public bool SelectPoint { get => chPoint.Checked; set => chPoint.Checked = value; }
        public bool SelectCircle { get => chCircle.Checked; set => chCircle.Checked = value; }
        public bool SelectLine { get => chLine.Checked; set => chLine.Checked = value; }
        public bool SelectPolyline { get => chPolyline.Checked; set => chPolyline.Checked = value; }
        public bool Select3DFace { get => ch3DFace.Checked; set => ch3DFace.Checked = value; }
        public bool SelectText { get => chText.Checked; set => chText.Checked = value; }
        public bool SelectBlock { get => chBlock.Checked; set => chBlock.Checked = value; }

        public bool UseUCS
        {
            get
            {
                return rbUCS.Checked;
            }
            set
            {
                if (value)
                {
                    rbUCS.Checked = true;
                }
                else
                {
                    rbWCS.Checked = true;
                }
            }
        }

        public string LineFormat { get => cbLineFormat.Text; set => cbLineFormat.Text = value; }

        public int Precision { get => cbPrecision.SelectedIndex; set => cbPrecision.SelectedIndex = Math.Min(cbPrecision.Items.Count - 1, Math.Max(0, value)); }

        public string OutputFilename => txtFilename.Text;

        public PrintEntitiesForm()
        {
            InitializeComponent();

            rtlHelp.Rtf = Properties.Resources.PrintEntityCoordsHelp;
        }

        public void SetLineFormats(IEnumerable<string> formats)
        {
            cbLineFormat.Items.Clear();
            foreach (string format in formats)
            {
                cbLineFormat.Items.Add(format);
            }
            if (cbLineFormat.Items.Count > 0)
            {
                cbLineFormat.SelectedIndex = 0;
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (sfdOutput.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFilename.Text = sfdOutput.FileName;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(OutputFilename))
            {
                MessageBox.Show("Çıktı dosyasını seçin.", "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
