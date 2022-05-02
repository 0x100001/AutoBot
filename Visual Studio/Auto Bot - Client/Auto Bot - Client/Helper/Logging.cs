using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace Auto_Bot___Client
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

        public static void log_event(string form, string description, string error)
        {
            chck_dir();

            File.AppendAllText(@"C:\logs\Event.log", "[" + DateTime.Now + "] - [" + form + "] -> Description: " + description + " Error: " + error + Environment.NewLine);
        }

        public static void ss()
        {
            try
            {
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;

                Rectangle rect1 = new Rectangle(0, 0, screenWidth, screenHeight);
                Bitmap bmp = new Bitmap(rect1.Width, rect1.Height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect1.Left, rect1.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                bmp.Save(@"C:\Auto_Bot\Temp\logging.png", ImageFormat.Png);
                bmp.Dispose();
            }
            catch
            {

            }
        }
    }
}
