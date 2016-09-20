using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XCOM.Commands.XCommand
{
    public partial class ProgressForm : XCOM.Utility.VersionDisplayForm
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void Start(string[] files, int actionCount)
        {
            lvProgress.Items.Clear();
            foreach (string file in files)
            {
                ListViewItem item = new ListViewItem(file);
                item.UseItemStyleForSubItems = false;
                item.SubItems.Add("");
                item.Tag = new string[0];
                lvProgress.Items.Add(item);
            }

            txErrors.Text = "Hataları görüntülemek için listeden bir satır seçin.";

            lblItem.Text = "";
            pgTotal.Minimum = 0;
            pgTotal.Maximum = files.Length * actionCount;

            lblAction.Text = "";
            pgFile.Minimum = 0;
            pgFile.Maximum = actionCount;

            txErrors.Visible = false;
            pnProgress.Visible = true;

            cmdClose.Enabled = false;
        }

        public void Complete()
        {
            txErrors.Visible = true;
            pnProgress.Visible = false;

            cmdClose.Enabled = true;
        }

        public void StartFile(string filename)
        {
            ListViewItem item = FindItem(filename);
            item.SubItems[1].Text = "........";

            lblItem.Text = filename;
            pgFile.Value = 0;
        }

        public void ActionError(string filename, string error)
        {
            SetItemError(filename, error);
        }

        public void ActionComplete(string filename, string actionName)
        {
            lblAction.Text = actionName;
            pgFile.Increment(1);
            pgTotal.Increment(1);
        }

        public void FileComplete(string filename)
        {
            lblItem.Text = "";

            lblAction.Text = "";
            pgFile.Value = 0;

            ListViewItem item = FindItem(filename);
            ListViewItem.ListViewSubItem sub = item.SubItems[1];
            string[] errors = (string[])item.Tag;
            if (errors == null || errors.Length == 0)
            {
                sub.Text = "OK";
            }
            else
            {
                sub.Text = "Hata";
                sub.ForeColor = Color.Red;
            }
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lvProgress_SelectedIndexChanged(object sender, EventArgs e)
        {
            txErrors.Text = "Hataları görüntülemek için listeden bir satır seçin.";
            if (lvProgress.SelectedItems.Count > 0)
            {
                ListViewItem item = lvProgress.SelectedItems[0];
                string[] errors = (string[])item.Tag;
                if (item.SubItems[1].Text == "OK" && errors == null || errors.Length == 0)
                {
                    txErrors.Text = "Hatasız tamamlandı.";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string err in errors)
                    {
                        sb.AppendLine(err);
                    }
                    txErrors.Text = sb.ToString();
                }
            }
        }

        private ListViewItem FindItem(string filename)
        {
            foreach (ListViewItem item in lvProgress.Items)
            {
                if (item.Text == filename) return item;
            }
            throw new ArgumentException("Item not found for: " + filename);
        }

        private void SetItemError(string filename, string[] errors)
        {
            ListViewItem item = FindItem(filename);
            List<string> newerrors = new List<string>();
            newerrors.AddRange((string[])item.Tag);
            foreach (string error in errors)
            {
                if (!string.IsNullOrEmpty(error))
                {
                    newerrors.Add(error);
                }
            }
            item.Tag = newerrors.ToArray();
        }

        private void SetItemError(string filename, string error)
        {
            SetItemError(filename, new string[] { error });
        }
    }
}
