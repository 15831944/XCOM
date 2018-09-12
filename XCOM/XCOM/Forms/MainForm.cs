using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace XCOM.Commands.XCommand
{
    public partial class MainForm : XCOM.CustomControls.VersionDisplayForm
    {
        public MainForm()
        {
            InitializeComponent();

            Application.Idle += new EventHandler(Application_Idle);

            // Find and add actions
            Assembly assembly = Assembly.GetExecutingAssembly();
            List<IXCOMAction> actions = new List<IXCOMAction>();
            foreach (Type type in assembly.GetTypes().Where(p => p.BaseType == typeof(XCOMActionBase)))
            {
                IXCOMAction instance = (IXCOMAction)Activator.CreateInstance(type);
                actions.Add(instance);
            }
            actions.Sort((a, b) => { return (a.Order < b.Order ? -1 : 1); });
            foreach (IXCOMAction action in actions)
            {
                bool canRunWithoutDialog = ((action.Interface & ActionInterface.Command) != ActionInterface.None);
                lbActions.Items.Add(action, action.Recommended && canRunWithoutDialog);
                // Dialog button
                if ((action.Interface & ActionInterface.Dialog) != ActionInterface.None)
                {
                    lbActions.AddButton(lbActions.Items.Count - 1);
                }
                // Help button
                if (!string.IsNullOrEmpty(action.HelpText))
                {
                    lbActions.AddButton(lbActions.Items.Count - 1, "?");
                }
            }
        }

        public string[] Filenames
        {
            get
            {
                List<string> filenames = new List<string>();
                foreach (ListViewItem item in lvSourceFiles.Items)
                {
                    filenames.Add((string)item.Tag);
                }
                return filenames.ToArray();
            }
        }

        public IXCOMAction[] SelectedActions
        {
            get
            {
                List<IXCOMAction> actions = new List<IXCOMAction>();
                // Actions
                foreach (object obj in lbActions.CheckedItems)
                {
                    actions.Add((IXCOMAction)obj);
                }
                return actions.ToArray();
            }
        }

        void Application_Idle(object sender, EventArgs e)
        {
            cmdRemoveFile.Enabled = (lvSourceFiles.SelectedItems.Count > 0);
            cmdClearFiles.Enabled = (lvSourceFiles.Items.Count > 0);
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            if (lvSourceFiles.Items.Count == 0)
            {
                MessageBox.Show("Lütfen DWG dosyalarını ekleyin.", "Deploy DWG", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmdAddFile_Click(object sender, EventArgs e)
        {
            if (browseDWG.ShowDialog() == DialogResult.OK)
            {
                AddFile(browseDWG.FileNames);
            }
        }

        private void AddFile(string file)
        {
            string name = Path.GetFileName(file);
            string path = Path.GetDirectoryName(file);
            ListViewItem item = new ListViewItem(name);
            item.SubItems.Add(path);
            item.Tag = file;
            lvSourceFiles.Items.Add(item);
        }

        private void AddFile(string[] filenames)
        {
            foreach (string file in filenames)
            {
                string ext = Path.GetExtension(file);
                if (string.Compare(ext, ".dwg", true) == 0)
                {
                    AddFile(file);
                }
            }
        }

        private void cmdRemoveFile_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvSourceFiles.SelectedItems)
            {
                lvSourceFiles.Items.Remove(item);
            }
        }

        private void cmdClearFiles_Click(object sender, EventArgs e)
        {
            lvSourceFiles.Items.Clear();
        }

        void lbActions_ButtonClick(object sender, CheckedListBoxWithButtons.ButtonClickEventArgs e)
        {
            IXCOMAction action = (IXCOMAction)lbActions.Items[e.ItemIndex];
            if (string.IsNullOrEmpty(e.ButtonText) && (action.Interface & ActionInterface.Dialog) != ActionInterface.None)
            {
                action.ShowDialog();
            }
            else if (e.ButtonText == "?" && !string.IsNullOrEmpty(action.HelpText))
            {
                HelpForm.ShowDialog(action.Name, action.HelpText);
            }
        }

        private void lvSourceFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Move;
        }

        private void lvSourceFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                AddFile((string[])e.Data.GetData(DataFormats.FileDrop));
            }
        }

        private void cmdAddFolder_Click(object sender, EventArgs e)
        {
            if (browseFolder.ShowDialog() == DialogResult.OK)
            {
                AddFile(Directory.GetFiles(browseFolder.SelectedPath, "*.dwg", SearchOption.TopDirectoryOnly));
            }
        }
    }
}
