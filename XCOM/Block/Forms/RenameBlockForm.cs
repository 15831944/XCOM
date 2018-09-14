using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace XCOM.Commands.Block
{
    public partial class RenameBlockForm : XCOM.CustomControls.VersionDisplayForm
    {
        private class BlockRename
        {
            public string NewName { get; set; }
            public bool IsAnon { get; }
            public System.Drawing.Bitmap Icon { get; }

            public BlockRename(string newName, bool isAnon, System.Drawing.Bitmap icon)
            {
                NewName = newName;
                IsAnon = isAnon;
                Icon = icon;
            }
        }
        Dictionary<string, BlockRename> allNames = new Dictionary<string, BlockRename>();

        public Dictionary<string, string> BlockNames
        {
            get
            {
                Dictionary<string, string> names = new Dictionary<string, string>();
                foreach (ListViewItem item in lvBlocks.Items)
                {
                    if (string.Compare(item.Text, item.SubItems[1].Text, true) == 0 || string.IsNullOrEmpty(item.SubItems[1].Text))
                        continue;
                    names.Add(item.Text, item.SubItems[1].Text);
                }
                return names;
            }
        }

        public RenameBlockForm()
        {
            InitializeComponent();
        }

        public void AddBlockName(string name, bool isAnon, System.Drawing.Bitmap preview)
        {
            ListViewItem item = new ListViewItem(name);
            item.BackColor = (isAnon ? System.Drawing.Color.LightGray : System.Drawing.SystemColors.Window);
            item.SubItems.Add("");
            allNames.Add(name, new BlockRename("", isAnon, preview));
            if (!isAnon || cbShowAnon.Checked)
                lvBlocks.Items.Add(item);
        }

        public void SelectBlock(string name)
        {
            var item = lvBlocks.FindItemWithText(name);
            if (item != null)
            {
                lvBlocks.SelectedItems.Clear();
                item.Selected = true;
                item.Focused = true;
                lvBlocks.EnsureVisible(item.Index);
            }
        }

        private void CheckNames()
        {
            bool foundDuplicates = false;

            foreach (ListViewItem item in lvBlocks.Items)
            {
                if (string.Compare(item.Text, item.SubItems[1].Text, true) == 0 || string.IsNullOrEmpty(item.SubItems[1].Text))
                {
                    item.SubItems[1].Text = "";
                    item.ImageKey = "";
                }
            }

            foreach (ListViewItem item1 in lvBlocks.Items)
            {
                string name1 = (string.IsNullOrEmpty(item1.SubItems[1].Text) ? item1.Text : item1.SubItems[1].Text);

                bool hasDuplicates = false;
                foreach (ListViewItem item2 in lvBlocks.Items)
                {
                    if (ReferenceEquals(item1, item2))
                        continue;
                    string name2 = (string.IsNullOrEmpty(item2.SubItems[1].Text) ? item2.Text : item2.SubItems[1].Text);
                    if (string.Compare(name1, name2, true) == 0)
                    {
                        foundDuplicates = true;
                        hasDuplicates = true;
                    }
                }

                item1.ImageKey = (hasDuplicates ? "error" : (string.IsNullOrEmpty(item1.SubItems[1].Text) ? "" : "tick"));
            }

            cmdOK.Enabled = !foundDuplicates;
        }

        private void CheckSelection()
        {
            if (lvBlocks.SelectedItems.Count > 0)
            {
                txtOldName.Text = lvBlocks.SelectedItems[0].Text;
                txtBlockName.Text = lvBlocks.SelectedItems[0].SubItems[1].Text;
                txtBlockName.Enabled = true;
                btnRename.Enabled = true;
                pbPreview.Image = allNames[lvBlocks.SelectedItems[0].Text].Icon;
            }
            else
            {
                txtOldName.Text = "";
                txtBlockName.Text = "";
                txtBlockName.Enabled = false;
                btnRename.Enabled = false;
                pbPreview.Image = null;
            }
        }

        private void lvBlocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSelection();
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (lvBlocks.SelectedItems.Count > 0)
            {
                try
                {
                    SymbolUtilityServices.ValidateSymbolName(txtBlockName.Text, false);
                    lvBlocks.SelectedItems[0].SubItems[1].Text = txtBlockName.Text;
                    allNames[lvBlocks.SelectedItems[0].Text].NewName = txtBlockName.Text;
                    CheckNames();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Error: " + ex.ToString(), "XCOM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cbShowAnon_CheckedChanged(object sender, EventArgs e)
        {
            lvBlocks.Items.Clear();
            pbPreview.Image = null;
            foreach (var pair in allNames)
            {
                ListViewItem item = new ListViewItem(pair.Key);
                item.BackColor = (pair.Value.IsAnon ? System.Drawing.Color.LightGray : System.Drawing.SystemColors.Window);
                item.SubItems.Add(pair.Value.NewName);
                if (!pair.Value.IsAnon || cbShowAnon.Checked)
                    lvBlocks.Items.Add(item);
            }
            CheckNames();
            CheckSelection();
        }
    }
}
