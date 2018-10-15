using System.ComponentModel;
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

            try
            {
                var layerNames = AcadSymbolTable.GetLayerTableRecords(this.Database,
                p => (IncludeXRef || !p.IsDependent),
                p => p.Name).OrderBy(p => p);
                this.SetItems(layerNames);
            }
            catch
            {
                ;
            }
        }
    }
}
