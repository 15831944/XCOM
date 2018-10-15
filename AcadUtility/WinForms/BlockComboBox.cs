using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AcadUtility.WinForms
{
    public class BlockComboBox : DatabaseComboBox
    {
        [Category("Data"), DefaultValue(false)]
        public bool IncludeAnonymous { get; set; }

        [Category("Data"), DefaultValue(false)]
        public bool IncludeXRef { get; set; }

        [Category("Data"), DefaultValue(false)]
        public bool IncludeLayout { get; set; }

        [Category("Appearance"), DefaultValue(36)]
        public int MinimumItemHeight = 36;

        public BlockComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable;

            IncludeAnonymous = false;
            IncludeXRef = false;
            IncludeLayout = false;

            try
            {
                var blocks = AcadSymbolTable.GetBlockTableRecords(this.Database,
                    p => (IncludeXRef || !p.IsFromExternalReference) && (IncludeXRef || !p.IsFromOverlayReference) && (IncludeLayout || !p.IsLayout) && (IncludeAnonymous || !p.IsAnonymous),
                    p => new { p.Name, p.HasPreviewIcon, p.PreviewIcon }).OrderBy(p => p.Name);
                var icons = new List<Bitmap>();
                var blockNames = new List<string>();
                foreach (var block in blocks)
                {
                    blockNames.Add(block.Name);
                    icons.Add(block.HasPreviewIcon ? block.PreviewIcon : null);
                }
                this.SetItems(blockNames);

                int iconMargin = 2;
                int textMargin = 2;
                var iconWidth = MinimumItemHeight - 2 * textMargin;

                int dropDownWidth = 0;
                foreach (var obj in base.Items)
                {
                    var width = iconMargin + iconWidth + textMargin + TextRenderer.MeasureText((string)obj, this.Font).Width + textMargin;
                    dropDownWidth = Math.Max(dropDownWidth, width);
                }
                if (base.Items.Count > base.MaxDropDownItems)
                {
                    dropDownWidth += SystemInformation.VerticalScrollBarWidth;
                }
                dropDownWidth = Math.Min(dropDownWidth, 240);
                this.DropDownWidth = dropDownWidth;

                this.MeasureItem += (o, e) =>
                {
                    var size = TextRenderer.MeasureText(e.Graphics, blockNames[e.Index], this.Font);
                    e.ItemHeight = Math.Max(MinimumItemHeight, size.Height + 2 * textMargin);
                    e.ItemWidth = iconMargin + iconWidth + textMargin + size.Width + textMargin;
                };

                this.DrawItem += (o, e) =>
                {
                    e.DrawBackground();
                    Bitmap icon = icons[e.Index];
                    if (icon != null)
                    {
                        var iconSize = icon.Size;
                        var iconHeight = e.Bounds.Height - 2 * textMargin;
                        var iconRect = new Rectangle(e.Bounds.Left + iconMargin, e.Bounds.Top + (e.Bounds.Height - iconHeight) / 2, iconWidth, iconHeight);

                        float xScale = iconRect.Width / (float)iconSize.Width;
                        float yScale = iconRect.Height / (float)iconSize.Height;
                        float scale = Math.Min(xScale, yScale);
                        var targetRect = new Rectangle(iconRect.Left + (int)((iconRect.Width - scale * iconSize.Width) / 2),
                            iconRect.Top + (int)((iconRect.Height - scale * iconSize.Height) / 2),
                            (int)(scale * iconSize.Width), (int)(scale * iconSize.Height));
                        e.Graphics.DrawImage(icon, targetRect);
                    }

                    var textRect = new Rectangle(e.Bounds.Left + iconMargin + iconWidth + textMargin, e.Bounds.Top, e.Bounds.Width - (iconMargin + iconWidth + textMargin), e.Bounds.Height);
                    TextRenderer.DrawText(e.Graphics, blockNames[e.Index], e.Font, textRect, e.ForeColor, e.BackColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

                    e.DrawFocusRectangle();
                };
            }
            catch
            {
                ;
            }
        }
    }
}
