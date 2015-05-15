using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCOM.Utility
{
    public static class FileLogger
    {
        private static string LogFilename
        {
            get
            {
                string path = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                return System.IO.Path.Combine(path, "log.txt");
            }
        }

        public static void Log(string message)
        {
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(LogFilename, true))
            {
                w.WriteLine(message);
            }
        }

        public static void Log(Exception ex)
        {
            using (System.IO.StreamWriter w = new System.IO.StreamWriter(LogFilename, true))
            {
                w.WriteLine(ex.ToString());
            }
        }
    }
}
