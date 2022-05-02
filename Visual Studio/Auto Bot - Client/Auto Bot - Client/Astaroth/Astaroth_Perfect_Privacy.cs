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

namespace Astaroth_Perfect_Privacy
{
    public class Astaroth_Perfect_Privacy
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public bool firefox_pp_status(bool pp_status)
        {
            //Check PP VPN Status
            Bitmap perfect_privacy_confirmed = new Bitmap(Application.StartupPath + @"\astaroth\pp\perfect_privacy_status_active.png");
            Bitmap perfect_privacy_negative = new Bitmap(Application.StartupPath + @"\astaroth\pp\perfect_privacy_status_deactivated.png");

            bool perfect_privacy_status_flag = true;
            int perfect_privacy_status_count = 0;
            int perfect_privacy_status_maxTries = 15;

            while (perfect_privacy_status_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_confirmed_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(perfect_privacy_confirmed, false);
                    Rectangle pp_negative_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(perfect_privacy_negative, false);

                    if (pp_confirmed_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("ena");
                        pp_status = true;
                        perfect_privacy_status_flag = false;
                    }
                    else if (pp_negative_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("dis");
                        pp_status = false;
                        perfect_privacy_status_flag = false;

                        /*if (Main_Form_Init.Telegram_Monitoring == 1)
                        {
                            Telegram.telegram_alert_send("Perfect Privacy VPN not connected correctly(" + Main_Form_Init.openvpn_profile + "), or configuration is wrong.Restarting the bot now.");
                        }*/

                        //Misc.kill_everything_perform_restart();
                    }
                    else
                    {
                        MessageBox.Show("no");

                        // handle exception
                        if (++perfect_privacy_status_count == perfect_privacy_status_maxTries)
                        {
                            pp_status = false;
                            perfect_privacy_status_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.log_error("Perfect Privacy (Astaroth)", "Check spotify_login Confirmed", ex.Message);
                }
            }

            return pp_status;
        }

        public bool pp_status(bool pp_status)
        {
            var webclient = new WebClient();
            string data = webclient.DownloadString("https://checkip.perfect-privacy.com/json");
            if (data.Contains("VPN\":true"))
                pp_status = true;
            else
                pp_status = false;

            return pp_status;
        }

        public bool pp_login_ip(bool pp_login_ip)
        {
            var webclient = new WebClient();
            string data = webclient.DownloadString("https://checkip.perfect-privacy.com/json");
            if (data.Contains("92.119.159.151"))
                pp_login_ip = true;
            else
                pp_login_ip = false;

            return pp_login_ip;
        }
    }
}
