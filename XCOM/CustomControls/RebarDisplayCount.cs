using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace XCOM.CustomControls
{
    [Designer(typeof(RebarDisplayDesigner))]
    public class RebarDisplayCount : Panel
    {
        private Label lblDiameterSign;
        private TextBox txtCount;
        private TextBox txtDiameter;
        private FlowLayoutPanel layoutPanel;

        public int Count { get { return int.Parse(txtCount.Text); } set { txtCount.Text = value.ToString(); } }
        public int Diameter { get { return int.Parse(txtDiameter.Text); } set { txtDiameter.Text = value.ToString(); } }

        private void InitializeComponent()
        {
            layoutPanel = new FlowLayoutPanel();

            lblDiameterSign = new Label();
            txtCount = new TextBox();
            txtDiameter = new TextBox();

            SuspendLayout();
            Controls.Add(layoutPanel);
            layoutPanel.SuspendLayout();

            layoutPanel.FlowDirection = FlowDirection.LeftToRight;
            layoutPanel.WrapContents = false;
            layoutPanel.Dock = DockStyle.Fill;

            layoutPanel.Controls.Add(txtCount);
            layoutPanel.Controls.Add(lblDiameterSign);
            layoutPanel.Controls.Add(txtDiameter);

            txtCount.Text = "1";
            lblDiameterSign.Text = "Φ";
            txtDiameter.Text = "16";

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

            Size = new Size(100, 20);
            layoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        public RebarDisplayCount()
        {
            InitializeComponent();

            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = SystemColors.Window;
        }
    }
}
