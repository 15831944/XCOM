using System.Windows.Forms;

namespace XCOM.CustomControls
{
    public class RichTextLabel : RichTextBox
    {
        private string rtfResource;

        public string RtfResource
        {
            get
            {
                return rtfResource;
            }
            set
            {
                try
                {
                    rtfResource = value;
                    Rtf = Properties.Resources.ResourceManager.GetString(value);
                }
                catch
                {
                    rtfResource = "";
                    Rtf = "";
                }
            }
        }

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
            if (m.Msg == 0x204) return; // WM_RBUTTONDOWN
            if (m.Msg == 0x205) return; // WM_RBUTTONUP
            base.WndProc(ref m);
        }
    }
}
