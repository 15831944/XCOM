using System;
using System.ComponentModel;
using System.Windows.Forms;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace AcadUtility.WinForms
{
    [Designer(typeof(PickCoordinateControlDesigner))]
    public class PickCoordinateControl : Control
    {
        private Point3d pickPoint = new Point3d(0, 0, 0);

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point3d PickPoint
        {
            get => pickPoint;
            set
            {
                IsPointSet = true;
                pickPoint = value;
                txtX.Text = pickPoint.X.ToString();
                txtY.Text = pickPoint.Y.ToString();
                txtZ.Text = pickPoint.Z.ToString();
            }
        }

        [Browsable(false)]
        public bool IsPointSet { get; private set; } = false;

        [Category("Appearance"), DefaultValue("\nBaz noktası: ")]
        public string PickPointPrompt { get; set; }

        [Category("Appearance"), DefaultValue("&Baz noktasını seç")]
        public string PickButtonText
        {
            get => lblPick.Text;
            set => lblPick.Text = value;
        }

        [Category("Appearance"), DefaultValue("&X")]
        public string XLabelText
        {
            get => lblX.Text;
            set => lblX.Text = value;
        }

        [Category("Appearance"), DefaultValue("&Y")]
        public string YLabelText
        {
            get => lblY.Text;
            set => lblY.Text = value;
        }

        [Category("Appearance"), DefaultValue("&Z")]
        public string ZLabelText
        {
            get => lblZ.Text;
            set => lblZ.Text = value;
        }

        private TextBox txtZ;
        private Label lblZ;
        private TextBox txtY;
        private Label lblY;
        private TextBox txtX;
        private Label lblX;
        private Label lblPick;
        private Button btnPick;

        [Category("Appearance"), DefaultValue(55)]
        public int DividerLocation
        {
            get => txtX.Left;
            set
            {
                int newLoc = Math.Min(value, ClientRectangle.Width - 20);

                txtX.Left = newLoc;
                txtY.Left = newLoc;
                txtZ.Left = newLoc;

                txtX.Width = Math.Max(20, ClientRectangle.Width - txtX.Left);
                txtY.Width = Math.Max(20, ClientRectangle.Width - txtY.Left);
                txtZ.Width = Math.Max(20, ClientRectangle.Width - txtZ.Left);
            }
        }

        public PickCoordinateControl()
        {
            InitializeComponent();

            PickButtonText = "&Baz noktasını seç";
            XLabelText = "&X";
            YLabelText = "&Y";
            ZLabelText = "&Z";
            PickPointPrompt = "\nBaz noktası: ";

            DividerLocation = 55;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            height = 96;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void OnResize(EventArgs e)
        {
            txtX.Width = Math.Max(20, ClientRectangle.Width - txtX.Left);
            txtY.Width = Math.Max(20, ClientRectangle.Width - txtY.Left);
            txtZ.Width = Math.Max(20, ClientRectangle.Width - txtZ.Left);

            base.OnResize(e);
        }

        private void InitializeComponent()
        {
            this.btnPick = new System.Windows.Forms.Button();
            this.lblPick = new System.Windows.Forms.Label();

            this.lblX = new System.Windows.Forms.Label();
            this.txtX = new System.Windows.Forms.TextBox();

            this.lblY = new System.Windows.Forms.Label();
            this.txtY = new System.Windows.Forms.TextBox();

            this.lblZ = new System.Windows.Forms.Label();
            this.txtZ = new System.Windows.Forms.TextBox();

            this.SuspendLayout();

            // btnPick
            this.btnPick.Location = new System.Drawing.Point(0, 0);
            this.btnPick.Image = Properties.Resources.pick;
            this.btnPick.Name = "btnPick";
            this.btnPick.Size = new System.Drawing.Size(23, 23);
            this.btnPick.TabIndex = 0;
            this.btnPick.UseVisualStyleBackColor = true;
            this.btnPick.Click += BtnPick_Click;
            // lblPick
            this.lblPick.AutoSize = true;
            this.lblPick.Location = new System.Drawing.Point(29, 5);
            this.lblPick.Name = "lblPick";
            this.lblPick.Size = new System.Drawing.Size(90, 13);
            this.lblPick.TabIndex = 1;
            this.lblPick.Text = "&Baz noktasını seç";

            // lblX
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(0, 28);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(14, 13);
            this.lblX.TabIndex = 2;
            this.lblX.Text = "&X";
            // txtX
            this.txtX.Location = new System.Drawing.Point(55, 24);
            this.txtX.Name = "txtX";
            this.txtX.ReadOnly = true;
            this.txtX.Size = new System.Drawing.Size(100, 20);
            this.txtX.TabIndex = 3;

            // lblY
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(0, 54);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(14, 13);
            this.lblY.TabIndex = 4;
            this.lblY.Text = "&Y";
            // txtY
            this.txtY.Location = new System.Drawing.Point(55, 50);
            this.txtY.Name = "txtY";
            this.txtY.ReadOnly = true;
            this.txtY.Size = new System.Drawing.Size(100, 20);
            this.txtY.TabIndex = 5;

            // lblZ
            this.lblZ.AutoSize = true;
            this.lblZ.Location = new System.Drawing.Point(0, 80);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(14, 13);
            this.lblZ.TabIndex = 6;
            this.lblZ.Text = "&Z";
            // txtZ
            this.txtZ.Location = new System.Drawing.Point(55, 76);
            this.txtZ.Name = "txtZ";
            this.txtZ.ReadOnly = true;
            this.txtZ.Size = new System.Drawing.Size(100, 20);
            this.txtZ.TabIndex = 7;

            this.Controls.Add(this.btnPick);
            this.Controls.Add(this.lblPick);

            this.Controls.Add(this.lblX);
            this.Controls.Add(this.txtX);

            this.Controls.Add(this.lblY);
            this.Controls.Add(this.txtY);

            this.Controls.Add(this.lblZ);
            this.Controls.Add(this.txtZ);

            this.Width = 200;

            this.ResumeLayout(false);
        }

        private void BtnPick_Click(object sender, EventArgs e)
        {
            Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            using (EditorUserInteraction UI = ed.StartUserInteraction(FindForm()))
            {
                string message = string.IsNullOrEmpty(PickPointPrompt) ? "\nBaz noktası: " : PickPointPrompt;
                PromptPointResult ptRes = ed.GetPoint(message);
                if (ptRes.Status == PromptStatus.OK) PickPoint = ptRes.Value;
            }
        }
    }
}
