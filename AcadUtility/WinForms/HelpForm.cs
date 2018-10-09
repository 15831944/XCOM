using System.Drawing;
using System.Windows.Forms;

namespace AcadUtility.WinForms
{
    public partial class HelpForm : VersionDisplayForm
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
                if (AcadText.IsRtfText(text))
                    form.txtDialog.Rtf = text;
                else
                    form.txtDialog.Text = text;

                int minWidth = TextRenderer.MeasureText(caption, form.txtCaption.Font).Width;

                Size size = form.txtDialog.GetPreferredSize(form.txtDialog.Size);
                form.Size = new Size(size.Width + 97, size.Height + 133);
                form.ShowDialog();
            }
        }
    }
}
