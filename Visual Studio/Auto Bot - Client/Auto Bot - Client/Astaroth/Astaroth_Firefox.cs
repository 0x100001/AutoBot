using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Auto_Bot___Client;
using System.Diagnostics;
using System.Net;

namespace Astaroth_Firefox
{
    public class Astaroth_Firefox
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public void firefox_url_textbox_https(string url)
        {
            Bitmap firefox_url_textbox = new Bitmap(Application.StartupPath + @"\astaroth\spotify\firefox_url_textbox.png");
            bool firefox_url_textbox_flag = true;
            int firefox_url_textbox_count = 0;
            int firefox_url_textbox_maxTries = 10;
            while (firefox_url_textbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(firefox_url_textbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(url);
                        SendKeys.SendWait("{ENTER}");


                        firefox_url_textbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++firefox_url_textbox_count == firefox_url_textbox_maxTries)
                        {
                            firefox_url_textbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.log_error("Spotify (Astaroth)", "Click Firefox URL (https)", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
