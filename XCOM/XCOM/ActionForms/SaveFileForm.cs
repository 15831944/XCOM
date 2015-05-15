using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XCOM.Commands.XCommand
{
    public partial class SaveFileForm : Form
    {
        public SaveFileForm()
        {
            InitializeComponent();

            cbVersion.SelectedIndex = 0;

            Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            cbVersion.Enabled = rbChangeVersion.Checked;
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

        public bool KeepCurrentDwgVersion
        {
            get
            {
                return rbKeepCurrentVersion.Checked;
            }
            set
            {
                rbKeepCurrentVersion.Checked = value;
                rbChangeVersion.Checked = !value;
            }
        }

        public Autodesk.AutoCAD.DatabaseServices.DwgVersion DwgVersion
        {
            get
            {
                // Dwg version
                Autodesk.AutoCAD.DatabaseServices.DwgVersion version;
                switch (cbVersion.SelectedIndex)
                {
                    case 0: // 2013
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1027;
                        break;
                    case 1: // 2010
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1024;
                        break;
                    case 2: // 2007
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1021;
                        break;
                    case 3: // 2004
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1800;
                        break;
                    case 4: // 2000
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1015;
                        break;
                    case 5: // R14
                        version = Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1014;
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
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1027: // 2013
                        cbVersion.SelectedIndex = 0;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1024: // 2010
                        cbVersion.SelectedIndex = 1;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1021: // 2007
                        cbVersion.SelectedIndex = 2;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1800: // 2004
                        cbVersion.SelectedIndex = 3;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1015: // 2000
                        cbVersion.SelectedIndex = 4;
                        break;
                    case Autodesk.AutoCAD.DatabaseServices.DwgVersion.AC1014: // R14
                        cbVersion.SelectedIndex = 5;
                        break;
                    default:
                        cbVersion.SelectedIndex = 1;
                        break;
                }
            }
        }

        public string FilenameSuffix
        {
            get
            {
                return txSuffix.Text;
            }
            set
            {
                txSuffix.Text = value;
            }
        }
    }
}
