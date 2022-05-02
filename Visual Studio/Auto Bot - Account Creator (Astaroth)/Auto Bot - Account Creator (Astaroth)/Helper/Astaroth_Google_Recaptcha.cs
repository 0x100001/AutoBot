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
using Auto_Bot___Account_Creator__Astaroth_;

namespace Astaroth_Google_Recaptcha
{
    class Astaroth_Google_Recaptcha
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public void click_recaptcha()
        {
            Bitmap google_recaptcha = new Bitmap(Application.StartupPath + @"\ss_patterns\google_recaptcha.png");
            bool google_recaptcha_flag = true;
            int google_recaptcha_count = 0;
            int google_recaptcha_maxTries = 10;
            while (google_recaptcha_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(google_recaptcha, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        google_recaptcha_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++google_recaptcha_count == google_recaptcha_maxTries)
                        {
                            google_recaptcha_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Click Recaptcha", ex.Message);
                }
            }
        }

        public void click_buster()
        {
            Bitmap buster_item = new Bitmap(Application.StartupPath + @"\ss_patterns\buster_pattern.png");
            bool buster_item_flag = true;
            int buster_item_count = 0;
            int buster_item_maxTries = 5;
            while (buster_item_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(buster_item, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        buster_item_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++buster_item_count == buster_item_maxTries)
                        {
                            buster_item_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Click Buster", ex.Message);
                }
            }
        }

        public bool check_recaptcha_confirmed(bool recaptcha_status)
        {
            Bitmap recaptcha_confirmed = new Bitmap(Application.StartupPath + @"\ss_patterns\recaptcha_confirmed.png");
            bool recaptcha_confirmed_flag = true;
            int recaptcha_confirmed_count = 0;
            int recaptcha_confirmed_maxTries = 10;
            bool recaptcha_confirmed_status = false;

            while (recaptcha_confirmed_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(recaptcha_confirmed, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        recaptcha_confirmed_status = true;
                        recaptcha_confirmed_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++recaptcha_confirmed_count == recaptcha_confirmed_maxTries)
                        {
                            recaptcha_confirmed_status = false;
                            recaptcha_confirmed_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Check Recaptcha Confirmed", ex.Message);
                }
            }

            return recaptcha_confirmed_status;
        }

        public bool check_recaptcha_connection(bool recaptcha_connection)
        {
            Bitmap recaptcha_confirmed = new Bitmap(Application.StartupPath + @"\ss_patterns\recaptcha_couldnt_connect.png");
            bool recaptcha_confirmed_flag = true;
            int recaptcha_confirmed_count = 0;
            int recaptcha_confirmed_maxTries = 10;
            bool recaptcha_confirmed_status = false;

            while (recaptcha_confirmed_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(recaptcha_confirmed, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        recaptcha_confirmed_status = true;
                        recaptcha_confirmed_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++recaptcha_confirmed_count == recaptcha_confirmed_maxTries)
                        {
                            recaptcha_confirmed_status = false;
                            recaptcha_confirmed_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Check Recaptcha Connection Status", ex.Message);
                }
            }

            return recaptcha_confirmed_status;
        }

        public bool recaptcha_detected_check(bool recaptcha_detected_result)
        {
            Bitmap recaptcha_detected = new Bitmap(Application.StartupPath + @"\ss_patterns\recaptcha_detected.png");
            bool recaptcha_detected_flag = true;
            int recaptcha_detected_count = 0;
            int recaptcha_detected_maxTries = 1;
            bool recaptcha_detected_status = false;

            while (recaptcha_detected_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(recaptcha_detected, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        recaptcha_detected_status = true;
                        recaptcha_detected_flag = false;

                        Telegram.Telegram.telegram_alert_send("Account Generator detected.");
                    }
                    else
                    {
                        // handle exception
                        if (++recaptcha_detected_count == recaptcha_detected_maxTries)
                        {
                            recaptcha_detected_status = false;
                            recaptcha_detected_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Recaptcha Detected Check", ex.Message);
                }
            }

            return recaptcha_detected_status;
        }

        public void recaptcha_multiple_solutions_check()
        {
            Bitmap recaptcha_multiple_solutions = new Bitmap(Application.StartupPath + @"\ss_patterns\recaptcha_multiple_solutions.png");
            bool recaptcha_multiple_solutions_flag = true;
            int recaptcha_multiple_solutions_count = 0;
            int recaptcha_multiple_solutions_maxTries = 5;
            while (recaptcha_multiple_solutions_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(recaptcha_multiple_solutions, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        click_buster();

                        recaptcha_multiple_solutions_flag = false;
                    }
                    else
                    {

                        // handle exception
                        if (++recaptcha_multiple_solutions_count == recaptcha_multiple_solutions_maxTries)
                        {
                            recaptcha_multiple_solutions_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Recaptcha Multiple Solutions Check", ex.Message);
                }
            }
        }
    }
}
