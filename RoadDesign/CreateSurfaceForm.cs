using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadDesign
{
    public partial class CreateSurfaceForm : Form
    {
        public bool SelectPoints { get { return chSelectPoint.Checked; } set { chSelectPoint.Checked = value; } }
        public bool SelectLines { get { return chSelectLine.Checked; } set { chSelectLine.Checked = value; } }
        public bool SelectPolylines { get { return chSelectPolyline.Checked; } set { chSelectPolyline.Checked = value; } }
        public bool SelectTexts { get { return chSelectText.Checked; } set { chSelectText.Checked = value; } }
        public bool SelectTextsWithZ { get { return chSelectTextZ.Checked; } set { chSelectTextZ.Checked = value; } }
        public bool SelectBlocks { get { return chSelectBlock.Checked; } set { chSelectBlock.Checked = value; } }
        public bool Select3DFace { get { return chSelect3DFace.Checked; } set { chSelect3DFace.Checked = value; } }
        public bool SelectSolid { get { return chSelectSolid.Checked; } set { chSelectSolid.Checked = value; } }
        public bool EraseEntities { get { return chEraseEntities.Checked; } set { chEraseEntities.Checked = value; } }

        public CreateSurfaceForm()
        {
            InitializeComponent();

            Text = "Arazi Yüzeyi Oluştur v" + typeof(CreateSurfaceForm).Assembly.GetName().Version.ToString(2);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
