using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using System.Resources;
using System.Globalization;
using System.Collections;

namespace RebarPosCommands
{
    public partial class DrawBOQForm : VersionDisplayForm
    {
        public class BOQLanguage
        {
            public string Code { get; private set; }
            public string Name { get; private set; }
            public string[] Lines { get; private set; }

            public BOQLanguage(string resourceKey, string resourceVal)
            {
                string[] items = resourceVal.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Name = items[0];
                Lines = new string[items.Length - 1];
                Array.Copy(items, 1, Lines, 0, Lines.Length);
                Code = resourceKey.Substring(13);
            }

            public override string ToString()
            {
                return Code + " (" + Name + ")";
            }
        }

        public List<string> SelectedLanguages
        {
            get
            {
                List<string> sel = new List<string>();
                foreach (object item in lbLanguage.CheckedItems)
                {
                    sel.Add(((BOQLanguage)item).Code);
                }
                return sel;
            }
        }

        public int Multiplier { get { return (int)udMultiplier.Value; } }

        public double TextHeight { get { return double.Parse(txtTextHeight.Text); } }
        public int Precision { get { return cbPrecision.SelectedIndex; } }

        public bool DrawShapes { get { return chkDrawShapes.Checked; } }
        public bool HideMissing { get { return chkHideMissing.Checked; } }
        public bool HideUnusedDiameters { get { return chkHideUnusedDiameters.Checked; } }

        public string TableNote { get { return txtNote.Text; } }
        public string TableHeader { get { return txtHeader.Text; } }
        public string TableFooter { get { return txtFooter.Text; } }

        public DrawBOQForm()
        {
            InitializeComponent();
        }

        public bool Init()
        {
            lbLanguage.Items.Clear();
            Dictionary<string, int> languageList = new Dictionary<string, int>();
            int i = 0;
            foreach (string lang in Properties.Settings.Default.DrawBOQ_LanguageOrder.Split(' '))
            {
                languageList.Add(lang, i);
                i++;
            }
            ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, false, false);
            SortedList<int, BOQLanguage> lbItems = new SortedList<int, BOQLanguage>();
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = (string)entry.Key;
                if (resourceKey.StartsWith("BOQ_Language_"))
                {
                    BOQLanguage lang = new BOQLanguage(resourceKey, (string)entry.Value);
                    int n = int.MaxValue;
                    languageList.TryGetValue(lang.Code, out n);
                    lbItems.Add(n, lang);
                }
            }

            foreach (BOQLanguage lang in lbItems.Values)
            {
                lbLanguage.Items.Add(lang);
            }

            // Read from settings
            txtTextHeight.Text = Properties.Settings.Default.DrawBOQ_TextHeight.ToString();
            cbPrecision.SelectedIndex = Properties.Settings.Default.DrawBOQ_Precision;

            chkDrawShapes.Checked = Properties.Settings.Default.DrawBOQ_DrawShapes;
            chkHideMissing.Checked = Properties.Settings.Default.DrawBOQ_HideMissing;
            chkHideUnusedDiameters.Checked = Properties.Settings.Default.DrawBOQ_HideUnusedDiameters;

            List<string> selLang = new List<string>(Properties.Settings.Default.DrawBOQ_SelectedLanguages.Split(' '));
            foreach (BOQLanguage lang in lbItems.Values)
            {
                if (selLang.Contains(lang.Code)) lbLanguage.SetItemChecked(lbItems.IndexOfValue(lang), true);
            }
            if (lbLanguage.CheckedItems.Count == 0) lbLanguage.SetItemChecked(0, true);
            lbLanguage.SelectedIndex = 0;

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Save to settings
            Properties.Settings.Default.DrawBOQ_TextHeight = double.Parse(txtTextHeight.Text);
            Properties.Settings.Default.DrawBOQ_Precision = cbPrecision.SelectedIndex;

            Properties.Settings.Default.DrawBOQ_DrawShapes = chkDrawShapes.Checked;
            Properties.Settings.Default.DrawBOQ_HideMissing = chkHideMissing.Checked;
            Properties.Settings.Default.DrawBOQ_HideUnusedDiameters = chkHideUnusedDiameters.Checked;

            Properties.Settings.Default.DrawBOQ_SelectedLanguages = string.Join(" ", SelectedLanguages);

            Properties.Settings.Default.Save();

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtTextHeight_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TextBox txt = sender as TextBox;
            double val = 0;
            if (string.IsNullOrEmpty(txt.Text) || double.TryParse(txt.Text, out val))
            {
                if (val < 0.0001)
                {
                    errorProvider.SetError(txt, "Yazı yüksekliği sıfırdan büyük olmalı.");
                    errorProvider.SetIconAlignment(txt, ErrorIconAlignment.MiddleLeft);
                    e.Cancel = true;
                }
                else
                {
                    errorProvider.SetError(txt, "");
                }
            }
            else
            {
                errorProvider.SetError(txt, "Lütfen bir reel sayı girin.");
                errorProvider.SetIconAlignment(txt, ErrorIconAlignment.MiddleLeft);
                e.Cancel = true;
            }
        }



    }
}
