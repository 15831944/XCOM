using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace AcadUtility.WinForms
{
    public class LayerComboBox : DatabaseComboBox
    {
        [Category("Data"), DefaultValue(false)]
        public bool IncludeXRef { get; set; }

        public LayerComboBox()
        {
            IncludeXRef = false;
            HasIcons = true;
            IconSize = new Size(16, 16);

            try
            {
                var layers = AcadSymbolTable.GetLayerTableRecords(this.Database, p => (IncludeXRef || !p.IsDependent), p => new { p.Name, p.Color }).OrderBy(p => p.Name);
                var colors = new List<Color>();
                foreach (var layer in layers)
                {
                    this.Items.Add(layer.Name);
                    colors.Add(layer.Color.ColorValue);
                }

                this.DrawIcon += (o, e) =>
                {
                    using (var brush = new SolidBrush(colors[e.Index]))
                    using (var pen = new Pen(e.ForeColor))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                        e.Graphics.DrawRectangle(pen, e.Bounds);
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
