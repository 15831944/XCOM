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

namespace CoordinateLabel
{
    public partial class SelectGroupForm : Form
    {
        public bool HasXY { get; set; }
        public bool UseXY { get { return rbXY.Checked; } set { rbXY.Checked = value; rbPrefix.Checked = !value; } }
        public bool UsePrefix { get { return rbPrefix.Checked; } set { rbPrefix.Checked = value; rbXY.Checked = !value; } }
        public string Prefix { get { return cbPrefix.Items.Count == 0 ? "" : (string)cbPrefix.SelectedItem; } }

        public SelectGroupForm()
        {
            InitializeComponent();

            Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            rbXY.Enabled = HasXY;

            lblPrefix.Enabled = rbPrefix.Checked;
            cbPrefix.Enabled = rbPrefix.Checked;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        public void SetPrefixes(string[] prefixes)
        {
            cbPrefix.Items.Clear();
            for (int i = 0; i < prefixes.Length; i++)
            {
                cbPrefix.Items.Add(prefixes[i]);
            }
            cbPrefix.SelectedIndex = 0;
        }
    }
}
