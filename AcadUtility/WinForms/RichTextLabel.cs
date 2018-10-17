using System.Windows.Forms;

namespace AcadUtility.WinForms
{
    public class RichTextLabel : RichTextBox
    {
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_RBUTTONUP = 0x205;

        public RichTextLabel()
        {
            ReadOnly = true;
            BorderStyle = BorderStyle.None;
            TabStop = false;
            SetStyle(ControlStyles.Selectable, false);
            SetStyle(ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            MouseEnter += (sender, e) => Cursor = Cursors.Default;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_RBUTTONDOWN || m.Msg == WM_RBUTTONUP)
                return;
            else
                base.WndProc(ref m);
        }
    }
}
