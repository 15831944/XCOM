using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcadUtility
{
    public class CurrentDB : IDisposable
    {
        private bool contextSwitch;
        private Database oldDB;
        private Database currDB;

        public CurrentDB(Database db)
        {
            currDB = db;
            oldDB = HostApplicationServices.WorkingDatabase;
            contextSwitch = (currDB != oldDB);

            if (contextSwitch)
            {
                HostApplicationServices.WorkingDatabase = currDB;
            }
        }

        public void Dispose()
        {
            if (contextSwitch)
            {
                HostApplicationServices.WorkingDatabase = oldDB;
            }
        }

        public static implicit operator Database(CurrentDB current)
        {
            return current.currDB;
        }
    }
}
