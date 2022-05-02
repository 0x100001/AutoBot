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

namespace Astaroth_Spotify
{
    public class Astaroth_Spotify
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public void fill_mail_textbox(string mail)
        {
            Bitmap fill_mail_textbox = new Bitmap(Application.StartupPath + @"\ss_patterns\mail_adress_textbox.png");
            Bitmap fill_mail_confirm_textbox = new Bitmap(Application.StartupPath + @"\ss_patterns\mail_adress_confirm_textbox.png");

            bool fill_mail_textbox_flag = true;
            int fill_mail_textbox_count = 0;
            int fill_mail_textbox_maxTries = 20;
            while (fill_mail_textbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(fill_mail_textbox, false);
                    Rectangle pp_confirm_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(fill_mail_confirm_textbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(mail);
                    }
                    
                    if (pp_confirm_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_confirm_rect.X, pp_confirm_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_confirm_rect.X, pp_confirm_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_confirm_rect.X, pp_confirm_rect.Y, 0, 0);

                        SendKeys.SendWait(mail);

                        fill_mail_textbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++fill_mail_textbox_count == fill_mail_textbox_maxTries)
                        {
                            fill_mail_textbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void fill_password_textbox(string password)
        {
            Bitmap fill_mail_textbox = new Bitmap(Application.StartupPath + @"\ss_patterns\password_textbox.png");
            bool fill_mail_textbox_flag = true;
            int fill_mail_textbox_count = 0;
            int fill_mail_textbox_maxTries = 20;
            while (fill_mail_textbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(fill_mail_textbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(password);

                        fill_mail_textbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++fill_mail_textbox_count == fill_mail_textbox_maxTries)
                        {
                            fill_mail_textbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void fill_username_textbox(string username)
        {
            Bitmap fill_username_textbox = new Bitmap(Application.StartupPath + @"\ss_patterns\username_textbox.png");
            bool fill_username_textbox_flag = true;
            int fill_username_textbox_count = 0;
            int fill_username_textbox_maxTries = 20;
            while (fill_username_textbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(fill_username_textbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(username);

                        fill_username_textbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++fill_username_textbox_count == fill_username_textbox_maxTries)
                        {
                            fill_username_textbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void fill_birtday_day_textbox(string birthday_day)
        {
            Bitmap fill_birtday_day_textbox = new Bitmap(Application.StartupPath + @"\ss_patterns\birtday_day.png");
            bool fill_birtday_day_textbox_flag = true;
            int fill_birtday_day_textbox_count = 0;
            int fill_birtday_day_textbox_maxTries = 10;
            while (fill_birtday_day_textbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(fill_birtday_day_textbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(birthday_day);

                        fill_birtday_day_textbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++fill_birtday_day_textbox_count == fill_birtday_day_textbox_maxTries)
                        {
                            fill_birtday_day_textbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void fill_birtday_month_textbox(string birthday_month)
        {
            Bitmap fill_birthday_month_textbox = new Bitmap(Application.StartupPath + @"\ss_patterns\birthday_month.png");
            bool fill_birthday_month_textbox_flag = true;
            int fill_birthday_month_textbox_count = 0;
            int fill_birthday_month_textbox_maxTries = 10;
            while (fill_birthday_month_textbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(fill_birthday_month_textbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(birthday_month);

                        fill_birthday_month_textbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++fill_birthday_month_textbox_count == fill_birthday_month_textbox_maxTries)
                        {
                            fill_birthday_month_textbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void fill_birtday_year_textbox(string birthday_year)
        {
            Bitmap fill_birthday_year_textbox = new Bitmap(Application.StartupPath + @"\ss_patterns\birthday_year.png");
            bool fill_birthday_year_textbox_flag = true;
            int fill_birthday_year_textbox_count = 0;
            int fill_birthday_year_textbox_maxTries = 10;
            while (fill_birthday_year_textbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(fill_birthday_year_textbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(birthday_year);

                        fill_birthday_year_textbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++fill_birthday_year_textbox_count == fill_birthday_year_textbox_maxTries)
                        {
                            fill_birthday_year_textbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void check_male_checkbox()
        {
            Bitmap check_male_checkbox = new Bitmap(Application.StartupPath + @"\ss_patterns\male_checkbox.png");
            bool check_male_checkbox_flag = true;
            int check_male_checkbox_count = 0;
            int check_male_checkbox_maxTries = 10;
            while (check_male_checkbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(check_male_checkbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        check_male_checkbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++check_male_checkbox_count == check_male_checkbox_maxTries)
                        {
                            check_male_checkbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void check_female_checkbox()
        {
            Bitmap check_female_checkbox = new Bitmap(Application.StartupPath + @"\ss_patterns\female_checkbox.png");
            bool check_female_checkbox_flag = true;
            int check_female_checkbox_count = 0;
            int check_female_checkbox_maxTries = 10;
            while (check_female_checkbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(check_female_checkbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        check_female_checkbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++check_female_checkbox_count == check_female_checkbox_maxTries)
                        {
                            check_female_checkbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void check_homo_checkbox()
        {
            Bitmap check_homo_checkbox = new Bitmap(Application.StartupPath + @"\ss_patterns\homo_checkbox.png");
            bool check_homo_checkbox_flag = true;
            int check_homo_checkbox_count = 0;
            int check_homo_checkbox_maxTries = 10;
            while (check_homo_checkbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(check_homo_checkbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        check_homo_checkbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++check_homo_checkbox_count == check_homo_checkbox_maxTries)
                        {
                            check_homo_checkbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void check_tos_checkbox()
        {
            Bitmap check_tos_checkbox = new Bitmap(Application.StartupPath + @"\ss_patterns\tos_checkbox.png");
            bool check_tos_checkbox_flag = true;
            int check_tos_checkbox_count = 0;
            int check_tos_checkbox_maxTries = 10;
            while (check_tos_checkbox_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(check_tos_checkbox, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        check_tos_checkbox_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++check_tos_checkbox_count == check_tos_checkbox_maxTries)
                        {
                            check_tos_checkbox_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void cookie_button()
        {
            Bitmap cookie_button = new Bitmap(Application.StartupPath + @"\ss_patterns\cookie_button.png");
            bool cookie_button_flag = true;
            int cookie_button_count = 0;
            int cookie_button_maxTries = 1;
            while (cookie_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(cookie_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        cookie_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++cookie_button_count == cookie_button_maxTries)
                        {
                            cookie_button_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }

        public void page_down(int times)
        {
            int times_count = 0;
            int times_max = times;

            while (times_count != times_max)
            {
                SendKeys.SendWait("{DOWN}");

                times_count++;
            }
        }

        public bool account_creation_finished(bool account_creation_finished_status)
        {
            Bitmap account_creation_finished_confirmed = new Bitmap(Application.StartupPath + @"\ss_patterns\account_creation_finished.png");
            bool account_creation_finished_confirmed_flag = true;
            int account_creation_finished_confirmed_count = 0;
            int account_creation_finished_confirmed_maxTries = 15;
            bool account_creation_finished_confirmed_status = false;

            while (account_creation_finished_confirmed_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(account_creation_finished_confirmed, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        account_creation_finished_confirmed_status = true;
                        account_creation_finished_confirmed_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++account_creation_finished_confirmed_count == account_creation_finished_confirmed_maxTries)
                        {
                            account_creation_finished_confirmed_status = false;
                            account_creation_finished_confirmed_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google account_creation_finished", "Check account_creation_finished Confirmed", ex.Message);
                }
            }

            return account_creation_finished_confirmed_status;
        } 

        public void mail_header()
        {
            Bitmap mail_header = new Bitmap(Application.StartupPath + @"\ss_patterns\mail_header.png");
            bool mail_header_flag = true;
            int mail_header_count = 0;
            int mail_header_maxTries = 20;
            while (mail_header_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(mail_header, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 50); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        Thread.Sleep(5000);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        mail_header_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++mail_header_count == mail_header_maxTries)
                        {
                            mail_header_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Astaroth Google Recaptcha", "Fill Spotify E-Mail", ex.Message);
                }
            }
        }
    }
}
