using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace XCOM.CustomControls
{
    [Designer(typeof(RebarDisplayDesigner))]
    public class RebarDisplaySpacing : Panel
    {
        private Label lblDiameterSign;
        private Label lblSpacingSign;
        private Label lblSpacingUnit;
        private TextBox txtDiameter;
        private TextBox txtSpacing;
        private FlowLayoutPanel layoutPanel;

        public int Diameter { get { return int.Parse(txtDiameter.Text); } set { txtDiameter.Text = value.ToString(); } }
        public int Spacing { get { return int.Parse(txtSpacing.Text); } set { txtSpacing.Text = value.ToString(); } }

        private void InitializeComponent()
        {
            layoutPanel = new FlowLayoutPanel();
            lblDiameterSign = new Label();
            txtDiameter = new TextBox();
            lblSpacingSign = new Label();
            txtSpacing = new TextBox();
            lblSpacingUnit = new Label();

            SuspendLayout();
            Controls.Add(this.layoutPanel);
            layoutPanel.SuspendLayout();

            layoutPanel.FlowDirection = FlowDirection.LeftToRight;
            layoutPanel.WrapContents = false;
            layoutPanel.Dock = DockStyle.Fill;

            layoutPanel.Controls.Add(this.lblDiameterSign);
            layoutPanel.Controls.Add(this.txtDiameter);
            layoutPanel.Controls.Add(this.lblSpacingSign);
            layoutPanel.Controls.Add(this.txtSpacing);
            layoutPanel.Controls.Add(this.lblSpacingUnit);

            lblDiameterSign.Text = "Φ";
            txtDiameter.Text = "16";
            lblSpacingSign.Text = "/";
            txtSpacing.Text = "150";
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

            Size = new Size(130, 20);
            layoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        public RebarDisplaySpacing()
        {
            InitializeComponent();

            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = SystemColors.Window;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            using (Graphics g = CreateGraphics())
            {
                // Height = (FontSize * LineSpacing / EmHeight) + 7
                float fontLineSpacing = Font.FontFamily.GetLineSpacing(Font.Style);
                float fontEmHeight = Font.FontFamily.GetEmHeight(Font.Style);
                float fontSize = Font.SizeInPoints * g.DpiX / 72;
                height = (int)Math.Round(fontSize * fontLineSpacing / fontEmHeight) + 8;
                height = Math.Max(8, height);
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                base.Font = value;
                SetBoundsCore(Left, Top, Width, Height, BoundsSpecified.All);
            }
        }
    }

    internal class RebarDisplayDesigner : ControlDesigner
    {
        RebarDisplayDesigner()
        {
            base.AutoResizeHandles = true;
        }

        public override SelectionRules SelectionRules
        {
            get
            {
                return SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Moveable | SelectionRules.Visible;
            }
        }

        public override bool ParticipatesWithSnapLines
        {
            get
            {
                return true;
            }
        }

        public override IList SnapLines
        {
            get
            {
                Control parent = base.Control;
                List<SnapLine> lines = new List<SnapLine>();
                // bounds
                lines.Add(new SnapLine(SnapLineType.Top, 0));
                lines.Add(new SnapLine(SnapLineType.Bottom, parent.Height));
                lines.Add(new SnapLine(SnapLineType.Left, 0));
                lines.Add(new SnapLine(SnapLineType.Right, parent.Width));
                // text baseline
                using (Graphics g = parent.CreateGraphics())
                {
                    // Baseline = (FontSize * LineSpacing / EmHeight) + TopPadding
                    float fontLineSpacing = parent.Font.FontFamily.GetLineSpacing(parent.Font.Style);
                    float fontEmHeight = parent.Font.FontFamily.GetEmHeight(parent.Font.Style);
                    float fontSize = parent.Font.SizeInPoints * g.DpiX / 72;
                    int baseline = (int)Math.Round(fontSize * fontLineSpacing / fontEmHeight) + 3;
                    baseline = Math.Max(4, baseline);
                    lines.Add(new SnapLine(SnapLineType.Baseline, baseline));
                }
                return lines;
            }
        }
    }
}
