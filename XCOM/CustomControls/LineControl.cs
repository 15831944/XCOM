using System.Windows.Forms;

namespace XCOM.CustomControls
{
    public class LineControl : ContainerControl
    {
        private Label label1;

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(0, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 2);
            this.label1.TabIndex = 0;
            // 
            // LineControl
            // 
            this.MaximumSize = new System.Drawing.Size(0, 4);
            this.MinimumSize = new System.Drawing.Size(0, 4);
            this.Size = new System.Drawing.Size(0, 4);
            this.ResumeLayout(false);

        }

        public LineControl()
        {
            InitializeComponent();

            Controls.Add(label1);
        }
    }
}
