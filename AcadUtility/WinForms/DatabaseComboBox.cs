using Autodesk.AutoCAD.DatabaseServices;

namespace AcadUtility.WinForms
{
    public class DatabaseComboBox : StringListComboBox
    {
        public Database Database { get; set; }

        public DatabaseComboBox()
        {
            try
            {
                this.Database = HostApplicationServices.WorkingDatabase;
            }
            catch
            {
                this.Database = null;
            }
        }
    }
}
