using System.ComponentModel;
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

            try
            {
                var blockNames = AcadSymbolTable.GetBlockTableRecords(this.Database,
                    p => (IncludeXRef || !p.IsFromExternalReference) && (IncludeXRef || !p.IsFromOverlayReference) && (IncludeLayout || !p.IsLayout) && (IncludeAnonymous || !p.IsAnonymous),
                    p => p.Name).OrderBy(p => p);
                this.SetItems(blockNames);
            }
            catch
            {
                ;
            }
        }
    }
}
