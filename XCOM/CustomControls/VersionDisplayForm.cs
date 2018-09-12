using System.Reflection;
using System.Windows.Forms;

namespace XCOM.CustomControls
{
    public class VersionDisplayForm : Form
    {
        private string text = "";

        public override string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                base.Text = text + " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            }
        }
    }
}
