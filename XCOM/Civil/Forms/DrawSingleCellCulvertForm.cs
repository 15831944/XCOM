using System;

namespace XCOM.Commands.Civil
{
    public partial class DrawSingleCellCulvertForm : XCOM.Utility.VersionDisplayForm
    {
        public DrawSingleCellCulvertForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
