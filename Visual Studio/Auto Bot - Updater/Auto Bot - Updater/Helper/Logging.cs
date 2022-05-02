using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Drawing;

namespace Logging
{
    internal class Logging
    {
        private static void chck_dir()
        {
            if (Directory.Exists(@"C:\logs") == false)
            {
                Directory.CreateDirectory(@"C:\logs");
            }
        }

        public static void log_error(string form, string description, string error)
        {
            chck_dir();

            File.AppendAllText(@"C:\logs\Error.log", "[" + DateTime.Now + "] - [" + form + "] -> Description: " + description + " Error: " + error + Environment.NewLine);
        }
    }
}
