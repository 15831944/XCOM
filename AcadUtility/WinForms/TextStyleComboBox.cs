using System.ComponentModel;
using System.Linq;

namespace AcadUtility.WinForms
{
    public class TextStyleComboBox : DatabaseComboBox
    {
        [Category("Data"), DefaultValue(false)]
        public bool IncludeXRef { get; set; }

        public TextStyleComboBox()
        {
            IncludeXRef = false;

            try
            {
                var textStyleNames = AcadSymbolTable.GetTextStyleTableRecords(this.Database,
                    p => (IncludeXRef || !p.IsDependent) && !p.IsShapeFile,
                    p => p.Name).OrderBy(p => p);
                this.Items.AddRange(textStyleNames.ToArray());
            }
            catch
            {
                ;
            }
        }
    }
}