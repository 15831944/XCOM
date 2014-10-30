using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XCOM.ActionForms
{
    public partial class FindReplaceTextForm : Form
    {
        private bool textChanged = false;

        public FindReplaceTextForm()
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

        public bool SearchText { get { return cbText.Checked; } set { cbText.Checked = value; } }
        public bool SearchAttribute { get { return cbAttribute.Checked; } set { cbAttribute.Checked = value; } }
        public bool SearchDimension { get { return cbDimension.Checked; } set { cbDimension.Checked = value; } }
        public bool SearchLeader { get { return cbLeader.Checked; } set { cbLeader.Checked = value; } }
        public bool SearchTable { get { return cbTable.Checked; } set { cbTable.Checked = value; } }

        public bool CaseSensitive { get { return cbCaseSensitive.Checked; } set { cbCaseSensitive.Checked = value; } }
        public bool MatchWholeWords { get { return cbWholeWords.Checked; } set { cbWholeWords.Checked = value; } }
        public bool UseWildcards { get { return cbWildcards.Checked; } set { cbWildcards.Checked = value; } }

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
