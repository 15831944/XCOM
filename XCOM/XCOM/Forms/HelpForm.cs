using System;
using System.Drawing;
using System.Windows.Forms;

namespace XCOM.Commands.XCommand
{
    public partial class HelpForm : XCOM.CustomControls.VersionDisplayForm
    {
        public HelpForm()
        {
            InitializeComponent();

            pbIcon.Image = SystemIcons.Information.ToBitmap();
        }

        public static void ShowDialog(string caption, string text)
        {
            using (HelpForm form = new HelpForm())
            {
                form.txtCaption.Text = caption;
                form.txtDialog.Text = text;

                int minWidth = TextRenderer.MeasureText(caption, form.txtCaption.Font).Width;

                Size size = TextRenderer.MeasureText(text, form.txtDialog.Font, new Size(Math.Max(minWidth, form.txtDialog.ClientSize.Width), int.MaxValue), TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak);
                form.Size = new Size(size.Width + 87, size.Height + 123);
                form.ShowDialog();
            }
        }
    }
}
