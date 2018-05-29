using System;
using System.Windows.Forms;

namespace XCOM.Commands.XCommand
{
    public partial class RenameXREFsForm : Form
    {
        private bool textChanged = false;

        public bool RenamePaths { get { return cbRenamePaths.Checked; } set { cbRenamePaths.Checked = value; } }

        public RenameXREFsForm()
        {
            InitializeComponent();
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (textChanged)
            {
                DialogResult result = MessageBox.Show("Bul/Değiştir kutularında yapılan değişikler uygulansın mı?", "XCOM", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                else if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    btnAddFindReplace_Click(null, null);
                    return;
                }
                else
                {
                    textChanged = false;
                }
            }

            if (lvFindReplace.Items.Count == 0)
            {
                MessageBox.Show("Lütfen yapılacak değişiklikleri seçin.", "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void btnAddFindReplace_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFind.Text)) return;
            ListViewItem item = new ListViewItem(txtFind.Text);
            item.SubItems.Add(txtReplace.Text);
            lvFindReplace.Items.Add(item);
            textChanged = false;
        }

        private void btnClearFindReplace_Click(object sender, EventArgs e)
        {
            lvFindReplace.Items.Clear();
        }

        public void SetFindReplaceStrings(string[] find, string[] replace)
        {
            lvFindReplace.Items.Clear();
            for (int i = 0; i < find.Length; i++)
            {
                ListViewItem item = new ListViewItem(find[i]);
                item.SubItems.Add(replace[i]);
                lvFindReplace.Items.Add(item);
            }
        }

        public void GetFindReplaceStrings(out string[] find, out string[] replace)
        {
            find = new string[lvFindReplace.Items.Count];
            replace = new string[lvFindReplace.Items.Count];

            for (int i = 0; i < lvFindReplace.Items.Count; i++)
            {
                ListViewItem item = lvFindReplace.Items[i];
                find[i] = item.SubItems[0].Text;
                replace[i] = item.SubItems[1].Text;
            }
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
        }

        private void txtReplace_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
        }
    }
}
