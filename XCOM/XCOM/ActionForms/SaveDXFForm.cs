using System;
using System.Windows.Forms;

namespace XCOM.Commands.XCommand
{
    public partial class SaveDXFForm : Form
    {
        public SaveDXFForm()
        {
            InitializeComponent();

            cbVersion.SelectedIndex = 0;
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

        public Autodesk.AutoCAD.DatabaseServices.DwgVersion DXFVersion
        {
            get
            {
                // DXF version
                Autodesk.AutoCAD.DatabaseServices.DwgVersion version;
                switch (cbVersion.SelectedIndex)
                {
                    case 0: // 2010
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1024;
                        break;
                    case 1: // 2007
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1021;
                        break;
                    case 2: // 2004
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1800;
                        break;
                    case 3: // 2000
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1015;
                        break;
                    case 4: // R12
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1009;
                        break;
                    default:
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1024;
                        break;
                }
                return version;
            }
            set
            {
                // Dwg version
                switch (value)
                {
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1024: // 2010
                        cbVersion.SelectedIndex = 0;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1021: // 2007
                        cbVersion.SelectedIndex = 1;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1800: // 2004
                        cbVersion.SelectedIndex = 2;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1015: // 2000
                        cbVersion.SelectedIndex = 3;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1009: // R12
                        cbVersion.SelectedIndex = 4;
                        break;
                    default:
                        cbVersion.SelectedIndex = 0;
                        break;
                }
            }
        }
    }
}
