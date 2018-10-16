using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

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

        public BlockComboBox()
        {
            IncludeAnonymous = false;
            IncludeXRef = false;
            IncludeLayout = false;

            HasIcons = true;
            IconSize = new Size(24, 24);

            try
            {
                var blocks = AcadSymbolTable.GetBlockTableRecords(this.Database,
                    p => (IncludeXRef || !p.IsFromExternalReference) && (IncludeXRef || !p.IsFromOverlayReference) && (IncludeLayout || !p.IsLayout) && (IncludeAnonymous || !p.IsAnonymous),
                    p => new { p.Name, p.HasPreviewIcon, p.PreviewIcon }).OrderBy(p => p.Name);
                var icons = new List<Bitmap>();
                foreach (var block in blocks)
                {
                    this.Items.Add(block.Name);
                    icons.Add(block.HasPreviewIcon ? block.PreviewIcon : null);
                }

                this.DrawIcon += (o, e) =>
                {
                    Bitmap icon = icons[e.Index];
                    if (icon != null)
                    {
                        float xScale = e.Bounds.Width / (float)IconSize.Width;
                        float yScale = e.Bounds.Height / (float)IconSize.Height;
                        float scale = Math.Min(xScale, yScale);
                        var targetRect = new Rectangle(e.Bounds.Left + (int)((e.Bounds.Width - scale * IconSize.Width) / 2),
                            e.Bounds.Top + (int)((e.Bounds.Height - scale * IconSize.Height) / 2),
                            (int)(scale * IconSize.Width), (int)(scale * IconSize.Height));
                        e.Graphics.DrawImage(icon, targetRect);
                    }
                };
            }
            catch
            {
                ;
            }
        }
    }
}
