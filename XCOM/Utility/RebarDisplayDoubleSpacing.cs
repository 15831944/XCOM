using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XCOM.Utility
{
    [Designer(typeof(RebarDisplayDesigner))]
    public class RebarDisplayDoubleSpacing : Panel
    {
        private Label lblDiameterSign;
        private Label lblSpacingSign1;
        private Label lblSpacingSign2;
        private Label lblSpacingUnit;
        private TextBox txtDiameter;
        private TextBox txtSpacing1;
        private TextBox txtSpacing2;
        private FlowLayoutPanel layoutPanel;

        public int Diameter { get { return int.Parse(txtDiameter.Text); } set { txtDiameter.Text = value.ToString(); } }
        public int Spacing1 { get { return int.Parse(txtSpacing1.Text); } set { txtSpacing1.Text = value.ToString(); } }
        public int Spacing2 { get { return int.Parse(txtSpacing2.Text); } set { txtSpacing2.Text = value.ToString(); } }

        private void InitializeComponent()
        {
            layoutPanel = new FlowLayoutPanel();

            lblDiameterSign = new Label();
            lblSpacingSign1 = new Label();
            lblSpacingSign2 = new Label();
            lblSpacingUnit = new Label();
            txtDiameter = new TextBox();
            txtSpacing1 = new TextBox();
            txtSpacing2 = new TextBox();

            SuspendLayout();
            Controls.Add(layoutPanel);
            layoutPanel.SuspendLayout();

            layoutPanel.FlowDirection = FlowDirection.LeftToRight;
            layoutPanel.WrapContents = false;
            layoutPanel.Dock = DockStyle.Fill;

            layoutPanel.Controls.Add(lblDiameterSign);
            layoutPanel.Controls.Add(txtDiameter);
            layoutPanel.Controls.Add(lblSpacingSign1);
            layoutPanel.Controls.Add(txtSpacing1);
            layoutPanel.Controls.Add(lblSpacingSign2);
            layoutPanel.Controls.Add(txtSpacing2);
            layoutPanel.Controls.Add(lblSpacingUnit);

            lblDiameterSign.Text = "Φ";
            txtDiameter.Text = "16";
            lblSpacingSign1.Text = "/";
            txtSpacing1.Text = "150";
            lblSpacingSign2.Text = "/";
            txtSpacing2.Text = "150";
            lblSpacingUnit.Text = "mm";

            foreach (Control ctrl in layoutPanel.Controls)
            {
                ctrl.BackColor = SystemColors.Window;
                ctrl.Margin = new Padding(0, 3, 0, 3);
                if (ctrl is Label)
                {
                    Label lbl = ctrl as Label;
                    lbl.AutoSize = true;
                }
                if (ctrl is TextBox)
                {
                    TextBox txt = ctrl as TextBox;
                    txt.BorderStyle = BorderStyle.None;
                    txt.Size = new Size(38, 13);
                    txt.TextAlign = HorizontalAlignment.Center;
                }
            }

            Size = new Size(180, 20);
            layoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        public RebarDisplayDoubleSpacing()
        {
            InitializeComponent();

            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = SystemColors.Window;
        }
    }
}
