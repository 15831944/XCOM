using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.AutoCAD.DatabaseServices;
using System.Resources;
using System.Globalization;
using System.Collections;
using System.Linq;

namespace RebarPosCommands
{
    public partial class DrawBOQForm : VersionDisplayForm
    {
        public BOQLanguage[] SelectedLanguages
        {
            get
            {
                List<BOQLanguage> sel = new List<BOQLanguage>();
                foreach (object item in lbLanguage.CheckedItems)
                {
                    sel.Add((BOQLanguage)item);
                }
                return sel.ToArray();
            }
        }

        public BOQLanguage EffectiveLanguage
        {
            get
            {
                string displayUnit = "";
                switch (PosSettings.Current.DisplayUnit)
                {
                    case PosSettings.DrawingUnits.Millimeter:
                        displayUnit = "(mm)";
                        break;
                    case PosSettings.DrawingUnits.Centimeter:
                        displayUnit = "(cm)";
                        break;
                    case PosSettings.DrawingUnits.Decimeter:
                        displayUnit = "(dm)";
                        break;
                    case PosSettings.DrawingUnits.Meter:
                        displayUnit = "(m)";
                        break;
                }

                return BOQLanguage.GetEffectiveLanguage(SelectedLanguages, Multiplier, displayUnit);
            }
        }

        public int Multiplier { get { return (int)udMultiplier.Value; } }

        public double TextHeight { get { return double.Parse(txtTextHeight.Text); } }
        public int Precision { get { return cbPrecision.SelectedIndex; } }

        public bool DrawShapes { get { return chkDrawShapes.Checked; } }
        public bool HideMissing { get { return chkHideMissing.Checked; } }
        public bool HideUnusedDiameters { get { return chkHideUnusedDiameters.Checked; } }

        public string TableHeader { get { return txtHeader.Text; } }
        public string TableFooter { get { return txtFooter.Text; } }

        public DrawBOQForm()
        {
            InitializeComponent();
        }

        public bool Init(double scale)
        {
            lbLanguage.Items.Clear();
            BOQLanguage[] languages = BOQLanguage.FromResource("BOQ_Language_", Properties.Settings.Default.DrawBOQ_LanguageOrder.Split(' '));
            foreach (BOQLanguage lang in languages)
            {
                lbLanguage.Items.Add(lang);
            }

            // Read from settings
            txtTextHeight.Text = scale.ToString();
            cbPrecision.SelectedIndex = Properties.Settings.Default.DrawBOQ_Precision;

            chkDrawShapes.Checked = Properties.Settings.Default.DrawBOQ_DrawShapes;
            chkHideMissing.Checked = Properties.Settings.Default.DrawBOQ_HideMissing;
            chkHideUnusedDiameters.Checked = Properties.Settings.Default.DrawBOQ_HideUnusedDiameters;

            List<string> selLang = new List<string>(Properties.Settings.Default.DrawBOQ_SelectedLanguages.Split(' '));
            int i = 0;
            foreach (BOQLanguage lang in languages)
            {
                if (selLang.Contains(lang.Code)) lbLanguage.SetItemChecked(i, true);
                i++;
            }
            if (lbLanguage.CheckedItems.Count == 0) lbLanguage.SetItemChecked(0, true);
            lbLanguage.SelectedIndex = 0;

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Save to settings
            Properties.Settings.Default.DrawBOQ_Precision = cbPrecision.SelectedIndex;

            Properties.Settings.Default.DrawBOQ_DrawShapes = chkDrawShapes.Checked;
            Properties.Settings.Default.DrawBOQ_HideMissing = chkHideMissing.Checked;
            Properties.Settings.Default.DrawBOQ_HideUnusedDiameters = chkHideUnusedDiameters.Checked;

            Properties.Settings.Default.DrawBOQ_SelectedLanguages = string.Join(" ", SelectedLanguages.Select(lang => lang.Code));

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

        private void lbLanguage_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lbLanguage.CheckedItems.Count == 0)
            {
                errorProvider.SetError(lbLanguage, "Lütfen en az bir dil seçin.");
                errorProvider.SetIconAlignment(lbLanguage, ErrorIconAlignment.MiddleLeft);
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(lbLanguage, "");
            }
        }
    }
}
