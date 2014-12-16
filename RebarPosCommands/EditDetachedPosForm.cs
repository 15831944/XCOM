using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace RebarPosCommands
{
    public partial class EditDetachedPosForm : Form
    {
        RebarPos m_Pos;

        public EditDetachedPosForm()
        {
            InitializeComponent();

            m_Pos = null;
        }

        public bool Init(RebarPos pos, Point3d pt)
        {
            m_Pos = pos;
            txtPosMarker.Text = pos.Pos;

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool haserror = false;
            if (!CheckPosMarker()) haserror = true;

            if (haserror)
            {
                MessageBox.Show("Lütfen hatalı değerleri düzeltin.", "RebarPos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            m_Pos.Pos = txtPosMarker.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtPosMarker_Validating(object sender, CancelEventArgs e)
        {
            CheckPosMarker();
        }

        private bool CheckPosMarker()
        {
            int val = 0;
            if (string.IsNullOrEmpty(txtPosMarker.Text) || int.TryParse(txtPosMarker.Text, out val))
            {
                errorProvider.SetError(txtPosMarker, "");
                return true;
            }
            else
            {
                errorProvider.SetError(txtPosMarker, "Poz numarası tam sayı olmalıdır.");
                errorProvider.SetIconAlignment(txtPosMarker, ErrorIconAlignment.MiddleLeft);
                return false;
            }
        }
    }
}
