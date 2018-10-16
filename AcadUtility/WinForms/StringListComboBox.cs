using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AcadUtility.WinForms
{
    public class StringListComboBox : ComboBox
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ComboBoxStyle DropDownStyle
        {
            get => ComboBoxStyle.DropDownList;
            set => base.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        [Category("Appearance"), DefaultValue(false)]
        public bool HasIcons { get; set; }

        [Category("Appearance"), DefaultValue(typeof(Size), "24, 24")]
        public Size IconSize { get; set; }

        [Category("Appearance"), DefaultValue(2)]
        public int TextMargin { get; set; }

        [Category("Appearance"), DefaultValue(2)]
        public int IconMargin { get; set; }

        [Category("Appearance"), DefaultValue(240)]
        public int MaxDropDownWidth { get; set; }

        public event DrawItemEventHandler DrawIcon;

        public override string Text
        {
            get => (string)base.SelectedItem;
            set => this.SetSelectedItemFromText(value);
        }

        public StringListComboBox()
        {
            base.DropDownStyle = ComboBoxStyle.DropDownList;
            this.MaxDropDownWidth = 240;

            this.DrawMode = DrawMode.OwnerDrawVariable;

            this.HasIcons = false;
            this.IconSize = new Size(24, 24);
            this.TextMargin = 2;
            this.IconMargin = 2;

            this.MeasureItem += StringListComboBox_MeasureItem;
            this.DrawItem += StringListComboBox_DrawItem;
        }

        private void StringListComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            if (e.Index > base.Items.Count - 1)
                return;

            Size textSize = TextRenderer.MeasureText(e.Graphics, (string)base.Items[e.Index], this.Font);
            if (HasIcons)
            {
                e.ItemWidth = this.IconSize.Width + IconMargin + textSize.Width + 2 * this.TextMargin;
                e.ItemHeight = Math.Max(this.IconSize.Height, textSize.Height) + 2 * this.TextMargin;
            }
            else
            {
                e.ItemWidth = textSize.Width + 2 * this.TextMargin;
                e.ItemHeight = textSize.Height + 2 * this.TextMargin;
            }

            int itemDropDownWidth = e.ItemWidth;
            if (base.Items.Count > base.MaxDropDownItems)
            {
                itemDropDownWidth += SystemInformation.VerticalScrollBarWidth;
            }
            itemDropDownWidth = Math.Min(itemDropDownWidth, MaxDropDownWidth);
            this.DropDownWidth = Math.Max(this.DropDownWidth, itemDropDownWidth);
        }

        private void StringListComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index > base.Items.Count - 1)
                return;

            e.DrawBackground();

            if (HasIcons)
            {
                var iconRect = new Rectangle(e.Bounds.Left + IconMargin, e.Bounds.Top + (e.Bounds.Height - IconSize.Height) / 2, IconSize.Width, IconSize.Height);
                OnDrawIcon(new DrawItemEventArgs(e.Graphics, e.Font, iconRect, e.Index, e.State, e.ForeColor, e.BackColor));
                var textRect = new Rectangle(e.Bounds.Left + IconMargin + IconSize.Width + TextMargin, e.Bounds.Top + TextMargin, e.Bounds.Width - (IconMargin + IconSize.Width + 2 * TextMargin), e.Bounds.Height - 2 * TextMargin);
                TextRenderer.DrawText(e.Graphics, (string)base.Items[e.Index], e.Font, textRect, e.ForeColor, e.BackColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
            else
            {
                var textRect = new Rectangle(e.Bounds.Left + TextMargin, e.Bounds.Top + TextMargin, e.Bounds.Width - 2 * TextMargin, e.Bounds.Height - 2 * TextMargin);
                TextRenderer.DrawText(e.Graphics, (string)base.Items[e.Index], e.Font, textRect, e.ForeColor, e.BackColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }

            e.DrawFocusRectangle();
        }

        protected void SetSelectedItemFromText(string value)
        {
            for (int i = 0; i < base.Items.Count; i++)
            {
                if (string.Compare((string)base.Items[i], value, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    base.SelectedIndex = i;
                    return;
                }
            }

            if (base.SelectedIndex == -1 && base.Items.Count > 0) base.SelectedIndex = 0;
        }

        protected void OnDrawIcon(DrawItemEventArgs args)
        {
            DrawIcon?.Invoke(this, args);
        }
    }
}
