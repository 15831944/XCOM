using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;

namespace XCOM
{
    public class Command_XPURGE
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("XPURGE")]
        public static void XPurge()
        {
            // Process active document
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            // Read settings
            var deploy = new Deploy();

            // Add purge actions
            deploy.AddAction(new PurgeDGNLS());
            deploy.AddAction(new PurgeAll());

            // Start processing
            deploy.Run(db);
        }
    }
}
