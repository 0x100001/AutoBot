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
using Auto_Bot___Management_Console;

namespace Astaroth_Spotify_Firefox
{
    class Astaroth_Spotify_Firefox
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public void spotify_login_username(string username)
        {
            Bitmap spotify_login_username = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\login_username.png");
            bool spotify_login_username_flag = true;
            int spotify_login_username_count = 0;
            int spotify_login_username_maxTries = 10;
            while (spotify_login_username_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(spotify_login_username, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(username);

                        spotify_login_username_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++spotify_login_username_count == spotify_login_username_maxTries)
                        {
                            spotify_login_username_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void spotify_login_password(string password)
        {
            Bitmap spotify_login_password = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\login_password.png");
            bool spotify_login_password_flag = true;
            int spotify_login_password_count = 0;
            int spotify_login_password_maxTries = 10;
            while (spotify_login_password_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(spotify_login_password, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(password);

                        spotify_login_password_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++spotify_login_password_count == spotify_login_password_maxTries)
                        {
                            spotify_login_password_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void spotify_login_button()
        {
            Bitmap spotify_login_button= new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\login_button.png");
            bool spotify_login_button_flag = true;
            int spotify_login_button_count = 0;
            int spotify_login_button_maxTries = 10;
            while (spotify_login_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(spotify_login_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        spotify_login_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++spotify_login_button_count == spotify_login_button_maxTries)
                        {
                            spotify_login_button_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public bool check_spotify_login_confirmed(bool login_status)
        {
            Bitmap spotify_login_confirmed = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\confirm_login_button.png");
            bool spotify_login_confirmed_flag = true;
            int spotify_login_confirmed_count = 0;
            int spotify_login_confirmed_maxTries = 10;

            while (spotify_login_confirmed_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(spotify_login_confirmed, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");
                        login_status = true;
                        spotify_login_confirmed_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++spotify_login_confirmed_count == spotify_login_confirmed_maxTries)
                        {
                            login_status = false;
                            spotify_login_confirmed_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Check spotify_login Confirmed", ex.Message);
                }
            }

            return login_status;
        }

        public void login_confirm_url(string url)
        {
            Bitmap login_confirm_url = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\confirm_login_url.png");
            bool login_confirm_url_flag = true;
            int login_confirm_url_count = 0;
            int login_confirm_url_maxTries = 10;
            while (login_confirm_url_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(login_confirm_url, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        SendKeys.SendWait(url);
                        SendKeys.SendWait("{ENTER}");


                        login_confirm_url_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++login_confirm_url_count == login_confirm_url_maxTries)
                        {
                            login_confirm_url_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void accept_cookies_normal_button()
        {
            Bitmap accept_cookies_normal_button = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\accept_cookies_normal_button.png");
            bool accept_cookies_normal_button_flag = true;
            int accept_cookies_normal_button_count = 0;
            int accept_cookies_normal_button_maxTries = 10;
            while (accept_cookies_normal_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(accept_cookies_normal_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        accept_cookies_normal_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++accept_cookies_normal_button_count == accept_cookies_normal_button_maxTries)
                        {
                            accept_cookies_normal_button_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void play_big_button()
        {
            Bitmap play_big_button = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\play_big_button.png");
            bool play_big_button_flag = true;
            int play_big_button_count = 0;
            int play_big_button_maxTries = 10;
            while (play_big_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(play_big_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        play_big_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++play_big_button_count == play_big_button_maxTries)
                        {
                            play_big_button_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void skip_forward_button()
        {
            Bitmap skip_forward_button = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\skip_forward_button.png");
            bool skip_forward_button_flag = true;
            int skip_forward_button_count = 0;
            int skip_forward_button_maxTries = 10;
            while (skip_forward_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(skip_forward_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        skip_forward_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++skip_forward_button_count == skip_forward_button_maxTries)
                        {
                            skip_forward_button_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public bool check_other_device_playing_bar(bool bar_status)
        {
            Bitmap other_device_playing_bar = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\other_device_playing_bar.png");
            bool other_device_playing_bar_flag = true;
            int other_device_playing_bar_count = 0;
            int other_device_playing_bar_maxTries = 10;

            while (other_device_playing_bar_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(other_device_playing_bar, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");
                        bar_status = true;
                        other_device_playing_bar_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++other_device_playing_bar_count == other_device_playing_bar_maxTries)
                        {
                            bar_status = false;
                            other_device_playing_bar_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Check spotify_login Confirmed", ex.Message);
                }
            }

            return bar_status;
        }

        public void other_device_playing_icon_button()
        {
            Bitmap other_device_playing_icon_button = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\other_device_playing_icon_button.png");
            bool other_device_playing_icon_button_flag = true;
            int other_device_playing_icon_button_count = 0;
            int other_device_playing_icon_button_maxTries = 10;
            while (other_device_playing_icon_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(other_device_playing_icon_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        other_device_playing_icon_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++other_device_playing_icon_button_count == other_device_playing_icon_button_maxTries)
                        {
                            other_device_playing_icon_button_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void other_device_playing_use_this_button()
        {
            Bitmap other_device_playing_use_this_button = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\other_device_playing_use_this_button.png");
            bool other_device_playing_use_this_button_flag = true;
            int other_device_playing_use_this_button_count = 0;
            int other_device_playing_use_this_button_maxTries = 10;
            while (other_device_playing_use_this_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(other_device_playing_use_this_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");

                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        other_device_playing_use_this_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++other_device_playing_use_this_button_count == other_device_playing_use_this_button_maxTries)
                        {
                            other_device_playing_use_this_button_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Click Recaptcha", ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public bool check_register_button(bool register_button_status)
        {
            Bitmap register_button = new Bitmap(Application.StartupPath + @"\tests\spotify_astaroth\ss_patterns\register_button.png");
            bool register_button_flag = true;
            int register_button_count = 0;
            int register_button_maxTries = 10;

            while (register_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(register_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        MessageBox.Show("yes");
                        register_button_status = true;
                        register_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++register_button_count == register_button_maxTries)
                        {
                            register_button_status = false;
                            register_button_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logging.log_error("Spotify (Astaroth)", "Check spotify_login Confirmed", ex.Message);
                }
            }

            return register_button_status;
        }
    }
}
