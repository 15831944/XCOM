using System;
using System.Windows.Forms;

namespace XCOM.Commands.XCommand
{
    public partial class DeleteBlockForm : Form
    {
        private bool textChanged = false;

        public DeleteBlockForm()
        {
            InitializeComponent();

            cbWhere.SelectedIndex = 0;

            Application.Idle += new EventHandler(Application_Idle);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            cbWhere.Enabled = !cbBlock.Checked;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            if (textChanged)
            {
                DialogResult result = MessageBox.Show("Blok adı kutusundaa yapılan değişiklik uygulansın mı?", "XCOM", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                else if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    btnAddBlockName_Click(null, null);
                    return;
                }
                else
                {
                    textChanged = false;
                }
            }

            if (lvBlockNames.Items.Count == 0)
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

        private void btnAddBlockName_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBlockName.Text)) return;
            ListViewItem item = new ListViewItem(txtBlockName.Text);
            lvBlockNames.Items.Add(item);
            textChanged = false;
        }

        private void btnClearBlockNames_Click(object sender, EventArgs e)
        {
            lvBlockNames.Items.Clear();
        }

        public void SetBlockNames(string[] names)
        {
            lvBlockNames.Items.Clear();
            for (int i = 0; i < names.Length; i++)
            {
                ListViewItem item = new ListViewItem(names[i]);
                lvBlockNames.Items.Add(item);
            }
        }

        public void GetBlockNames(out string[] names)
        {
            names = new string[lvBlockNames.Items.Count];

            for (int i = 0; i < lvBlockNames.Items.Count; i++)
            {
                ListViewItem item = lvBlockNames.Items[i];
                names[i] = item.SubItems[0].Text;
            }
        }

        public void SetSearchScope(bool applyModel, bool applyLayouts, bool applyAllBlocks)
        {
            if (applyModel && applyLayouts)
                cbWhere.SelectedIndex = 0;
            else if (applyModel)
                cbWhere.SelectedIndex = 1;
            else if (applyLayouts)
                cbWhere.SelectedIndex = 2;
            else
                cbWhere.SelectedIndex = 0;

            cbBlock.Checked = applyAllBlocks;
        }

        public void GetSearchScope(out bool applyModel, out bool applyLayouts, out bool applyAllBlocks)
        {
            applyModel = (cbWhere.SelectedIndex == 0) || (cbWhere.SelectedIndex == 1);
            applyLayouts = (cbWhere.SelectedIndex == 0) || (cbWhere.SelectedIndex == 2);
            applyAllBlocks = cbBlock.Checked;
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
