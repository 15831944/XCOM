using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading;

// This line is not mandatory, but improves loading performances
[assembly: Autodesk.AutoCAD.Runtime.CommandClass(typeof(XCOM2014.Commands))]

namespace XCOM2014
{
    public class Commands
    {
        [Autodesk.AutoCAD.Runtime.CommandMethod("XCOM")]
        public static void XCOMBatch()
        {
            XCOMCore.CommandImplementation.RunBatch();
        }

        [Autodesk.AutoCAD.Runtime.CommandMethod("XPURGE")]
        public static void XPurge()
        {
            // Process active document
            Autodesk.AutoCAD.ApplicationServices.Document doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Autodesk.AutoCAD.DatabaseServices.Database db = doc.Database;

            // Run
            XCOMCore.CommandImplementation.PurgeCurrent(db);
        }
   }
}