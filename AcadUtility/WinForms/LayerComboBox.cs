using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AcadUtility.WinForms
{
    public class LayerComboBox : DatabaseComboBox
    {
        [Category("Data"), DefaultValue(false)]
        public bool IncludeXRef { get; set; }

        public LayerComboBox()
        {
            this.DrawMode = DrawMode.OwnerDrawVariable;

            IncludeXRef = false;

            try
            {
                var layers = AcadSymbolTable.GetLayerTableRecords(this.Database, p => (IncludeXRef || !p.IsDependent), p => new { p.Name, p.Color }).OrderBy(p => p.Name);
                var colors = new List<Color>();
                var layerNames = new List<string>();
                foreach (var layer in layers)
                {
                    layerNames.Add(layer.Name);
                    colors.Add(layer.Color.ColorValue);
                }
                this.SetItems(layerNames);

                int iconMargin = 2;
                int textMargin = 2;
                var iconWidth = 24;

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
                    var size = TextRenderer.MeasureText(e.Graphics, layerNames[e.Index], this.Font);
                    e.ItemHeight = size.Height + 2 * textMargin;
                    e.ItemWidth = iconMargin + iconWidth + textMargin + size.Width + textMargin;
                };

                this.DrawItem += (o, e) =>
                {
                    e.DrawBackground();
                    using (var brush = new SolidBrush(colors[e.Index]))
                    {
                        var iconHeight = e.Bounds.Height - 2 * textMargin;
                        var iconRect = new Rectangle(e.Bounds.Left + iconMargin, e.Bounds.Top + (e.Bounds.Height - iconHeight) / 2, iconWidth, iconHeight);
                        var textRect = new Rectangle(e.Bounds.Left + iconMargin + iconWidth + textMargin, e.Bounds.Top, e.Bounds.Width - (iconMargin + iconWidth + textMargin), e.Bounds.Height);
                        e.Graphics.FillRectangle(brush, iconRect);
                        e.Graphics.DrawRectangle(SystemPens.WindowText, iconRect);
                        TextRenderer.DrawText(e.Graphics, layerNames[e.Index], e.Font, textRect, e.ForeColor, e.BackColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
                    }
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
