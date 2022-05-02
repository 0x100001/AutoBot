using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Net;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using System.ServiceProcess;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing.Imaging;
using System.Management;

namespace Auto_Bot___Client.Player
{
    public partial class Spotify_Form : Form
    {
        string connStr;

        public int Clicks = 0;

        public int ss_count = 0;
        public string ss = "ss_";

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        private void callbackfunction(IAsyncResult res)
        {

        }

        public Spotify_Form()
        {
            InitializeComponent();
            database_information_loader();
        }

        //Load Database Information from Registry
        public void database_information_loader()
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            //Init Connection String
            connStr = "server=" + Main_Form_Init.mysql_host + ";user=" + Main_Form_Init.mysql_username + ";database=" + Main_Form_Init.mysql_database + ";port=" + Main_Form_Init.mysql_port + ";password=" + Main_Form_Init.mysql_password + ";Pooling=false;Connection Timeout=30;";
        }

        private void openvpn_connect()
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            Misc.kill_existing_process("openvpn");
            Misc.kill_existing_process("openvpn-gui");
            Misc.kill_existing_process("openvpnserv");

            try
            {
                SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
                ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
                foreach (ManagementObject item in searchProcedure.Get())
                {
                    item.InvokeMethod("Disable", null);
                }

                Thread.Sleep(5000);

                SelectQuery wmiQuery_Enable_Network_Adapter = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
                ManagementObjectSearcher search_network_adapter_enable = new ManagementObjectSearcher(wmiQuery_Enable_Network_Adapter);
                foreach (ManagementObject item_enable_adapter in search_network_adapter_enable.Get())
                {
                    item_enable_adapter.InvokeMethod("Enable", null);
                }
            }
            catch
            {

            }

            Thread.Sleep(10000);

            if (Main_Form_Init.openvpn_enabled == 1)
            {
                try
                {
                    //Start Open VPN Service
                    try
                    {
                        ServiceController sc = new ServiceController("OpenVPNServiceInteractive");
                        if (sc.Status.Equals(ServiceControllerStatus.Stopped))
                        {
                            {
                                sc.Start();
                                sc.WaitForStatus(ServiceControllerStatus.Running);
                            }
                        }
                        else if (sc.Status.Equals(ServiceControllerStatus.Running))
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped);
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running);
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Restart Service (OpenVPNServiceInteractive): " + ex.Message + Environment.NewLine);
                        Misc.kill_everything_perform_restart();
                    }

                    Thread.Sleep(1000);

                    try
                    {
                        ProcessStartInfo proc = new ProcessStartInfo();
                        proc.FileName = @"C:\Program Files\OpenVPN\bin\openvpn-gui.exe";
                        proc.Arguments = "--connect " + Main_Form_Init.openvpn_profile_combobox.SelectedItem.ToString();
                        Process.Start(proc);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Connect with OpenVPN (" + Main_Form_Init.openvpn_profile_combobox.SelectedItem.ToString() + "): " + ex.Message + Environment.NewLine);
                        Misc.kill_everything_perform_restart();
                    }

                    Thread.Sleep(30000);
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- OpenVPN Methode: " + ex.Message + Environment.NewLine);
                    Misc.kill_everything_perform_restart();
                }
            }
        }

        private void Spotify_Form_Load(object sender, EventArgs e)
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            if (Main_Form_Init.automation_mode == "Selenium")
                this.BeginInvoke(new MethodInvoker(player_instance_selenium));
            else if (Main_Form_Init.automation_mode == "Astaroth")
                this.BeginInvoke(new MethodInvoker(player_instance_astaroth));
        }

        private void player_instance_astaroth()
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            MethodInvoker mk = delegate
            {
                //Declare Astaroth Class
                Astaroth_Perfect_Privacy.Astaroth_Perfect_Privacy Astaroth_Perfect_Privacy_Firefox = new Astaroth_Perfect_Privacy.Astaroth_Perfect_Privacy();
                Astaroth_Spotify_Firefox.Astaroth_Spotify_Firefox Astaroth_Spotify_Firefox = new Astaroth_Spotify_Firefox.Astaroth_Spotify_Firefox();
                Astaroth_Firefox.Astaroth_Firefox Astaroth_Firefox = new Astaroth_Firefox.Astaroth_Firefox();

                Misc.kill_existing_process("cmd");
                Misc.kill_existing_process("explorer");
                Misc.kill_existing_process("firefox");
                Misc.kill_existing_process("firefox");
                Misc.kill_existing_process("firefox");
                Misc.process_setforeground("firefox");

                openvpn_connect();

                //Check PP Login IP
                /*bool perfect_privacy_login_ip = false;
                perfect_privacy_login_ip = Astaroth_Perfect_Privacy_Firefox.pp_login_ip(perfect_privacy_login_ip);

                if (perfect_privacy_login_ip == false)
                {
                    Telegram.telegram_alert_send("Player: Spotify\n\nLogin VPN not connected correctly, or configuration is wrong. Restarting the bot now.");

                    Misc.kill_everything_perform_restart();
                }*/

                //Process.Start(@"C:\Program Files\Mozilla Firefox\firefox.exe", Main_Form_Init.spotify_playlist_url + " -no-remote");
                //cookie_login = Astaroth_Spotify_Firefox.check_spotify_cookie_login(cookie_login);

                Misc.kill_existing_process("firefox");
                Misc.kill_existing_process("firefox");
                Misc.kill_existing_process("firefox");
                Misc.kill_existing_process("firefox");

                bool pp_status = false;
                pp_status = Astaroth_Perfect_Privacy_Firefox.pp_status(pp_status);

                if (pp_status == false)
                {
                    Telegram.telegram_alert_send("Player: Spotify\n\nPerfect Privacy VPN not connected correctly, or configuration is wrong. Restarting the bot now.\n\nProfile: " + Main_Form_Init.openvpn_profile);

                    Misc.kill_everything_perform_restart();
                }

                Process.Start(@"C:\Program Files\Mozilla Firefox\firefox.exe", Main_Form_Init.spotify_playlist_url + " -no-remote");

                Astaroth_Spotify_Firefox.accept_cookies_normal_button();
                Astaroth_Spotify_Firefox.accept_cookies_abnormal_button();

                bool cookie_login = false;
                cookie_login = Astaroth_Spotify_Firefox.check_spotify_cookie_login(cookie_login, 30);

                if (cookie_login == false)
                {
                    //Check Device Bar
                    bool other_device_bar = false;
                    other_device_bar = Astaroth_Spotify_Firefox.check_other_device_playing_bar(other_device_bar);

                    if (other_device_bar == true)
                    {
                        Astaroth_Spotify_Firefox.other_device_playing_icon_button();

                        Astaroth_Spotify_Firefox.other_device_playing_use_this_button();
                    }

                    Astaroth_Spotify_Firefox.play_big_button(15);

                    //Skip Methode
                    if (Main_Form_Init.player_type == "Playlist")
                    {
                        if (Main_Form_Init.spotify_autoskip_enabled == 1)
                        {
                            while (Main_Form_Init.spotify_autoskip_enabled == 1)
                            {
                                cookie_login = false;

                                cookie_login = Astaroth_Spotify_Firefox.check_spotify_cookie_login(cookie_login, 1);
                                
                                if (cookie_login == true)
                                {
                                    Autobot_Helper.outdated_credentials();

                                    Telegram.telegram_critical_alert_send("Player: Spotify\n\nOutdated Credentials:\n\n" + Main_Form_Init.spotify_player_username);

                                    Misc.kill_everything_perform_restart();
                                }

                                Astaroth_Spotify_Firefox.play_big_button(1);

                                //Init the Skip
                                Random r = new Random();
                                int rInt = r.Next(Main_Form_Init.spotify_autoskip_forward_from, Main_Form_Init.spotify_autoskip_forward_to);
                                Thread.Sleep(rInt);

                                bool click_confirmed = false;
                                click_confirmed = Astaroth_Spotify_Firefox.skip_forward_button(click_confirmed);

                                if (click_confirmed == true)
                                {
                                    Clicks = Clicks + 1;
                                }
                            }
                        }
                    }
                    else if (Main_Form_Init.player_type == "Album")
                    {
                        int skip_counter = 0;
                        int skip_counter_max = 20;

                        if (Main_Form_Init.spotify_autoskip_enabled == 1)
                        {
                            while (Main_Form_Init.spotify_autoskip_enabled == 1)
                            {
                                try
                                {
                                    //if skip counter same as max then do...
                                    if (skip_counter == skip_counter_max)
                                    {
                                        //reset counter
                                        skip_counter = 0;

                                        if (Main_Form_Init.spotify_album_listbox.Items.Count - 1 > Main_Form_Init.spotify_album_listbox.SelectedIndex)
                                        {
                                            Main_Form_Init.spotify_album_listbox.SelectedIndex++;
                                            Misc.kill_existing_process("firefox");
                                            Process.Start(@"C:\Program Files\Mozilla Firefox\firefox.exe", Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                                        }
                                        else
                                        {
                                            Main_Form_Init.spotify_album_listbox.SelectedIndex = 0;
                                            Misc.kill_existing_process("firefox");
                                            Process.Start(@"C:\Program Files\Mozilla Firefox\firefox.exe", Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }


                                //Init the Skip
                                Random r = new Random();
                                int rInt = r.Next(Main_Form_Init.spotify_autoskip_forward_from, Main_Form_Init.spotify_autoskip_forward_to);
                                Thread.Sleep(rInt);

                                bool click_confirmed = false;
                                Astaroth_Spotify_Firefox.skip_forward_button(click_confirmed);

                                if (click_confirmed == true)
                                {
                                    Clicks = Clicks + 1;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Autobot_Helper.outdated_credentials();

                    Telegram.telegram_critical_alert_send("Player: Spotify\n\nOutdated Credentials:\n\n" + Main_Form_Init.spotify_player_username);

                    Misc.kill_everything_perform_restart();
                    //not logged in 
                }
            };
            mk.BeginInvoke(callbackfunction, null);
        }

        private void player_instance_selenium()
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            int windows_size_width;
            int windows_size_height;

            MethodInvoker mk = delegate
            {
                ChromeOptions options = new ChromeOptions();

                options.AddArguments("--start-maximized");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-gpu");

                if (Main_Form_Init.chrome_persistent_profile_enabled == 0)
                {
                    options.AddArguments("--incognito");
                }

                options.AddArguments("--disable-infobars");
                options.AddArguments("--disable-blink-features=AutomationControlled");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddExcludedArgument("enable-automation");

                //Apply Custom Useragent if enabled
                if (Main_Form_Init.custom_useragent_enabled == 1)
                {
                    options.AddArgument("--user-agent=" + Main_Form_Init.custom_useragent_string + "\"");
                }

                //Check if persitent profile is enabled
                if (Main_Form_Init.chrome_persistent_profile_enabled == 1)
                {
                    if (Directory.Exists(@"C:\Chrome_Profiles") == false)
                    {
                        Directory.CreateDirectory(@"C:\Chrome_Profiles");
                    }

                    options.AddArguments("user-data-dir=C:\\Chrome_Profiles\\" + Main_Form_Init.spotify_player_username);
                }
                else if (Main_Form_Init.custom_profile_enabled == 1) //Check if custom plugins should be loaded
                {
                    options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + @"\\AppData\\Local\\Google\\Chrome\\User Data");
                }

                if (Main_Form_Init.emulate_device_enabled == 1)
                {
                    options.EnableMobileEmulation(Main_Form_Init.emulate_device_type);
                }

                IWebDriver driver = new ChromeDriver(@"C:\Auto_Bot\driver", options);

                Misc.hide_process("chromedriver");

                //Get Window Size
                if (Main_Form_Init.emulate_device_enabled == 0)
                {
                    windows_size_width = driver.Manage().Window.Size.Width;
                    windows_size_height = driver.Manage().Window.Size.Height;

                    //Change Window Size on the flight
                    if (windows_size_width > 1000)
                    {
                        driver.Manage().Window.Size = new Size(1000, 740);
                    }
                    else
                    {
                        driver.Manage().Window.Size = new Size(1040, 784);
                    }
                }

                if (Main_Form_Init.openvpn_health_check_enabled == 1)
                {
                    driver.Navigate().GoToUrl("https://checkip.perfect-privacy.com/json");

                    bool check_pp_status_bool = true;
                    int check_pp_status_count = 0;
                    int check_pp_status_maxTries = 20;
                    while (check_pp_status_bool == true)
                    {
                        try
                        {
                            string vpn_status = driver.FindElement(By.TagName("pre")).Text;

                            if (vpn_status.Contains("92.119.159.151"))
                                check_pp_status_bool = false;
                            else
                                check_pp_status_count++;

                            if (++check_pp_status_count == check_pp_status_maxTries)
                            {
                                if (Main_Form_Init.Telegram_Monitoring == 1)
                                {
                                    Telegram.telegram_alert_send("Player: Spotify\n\nLogin VPN not connected correctly, or configuration is wrong. Restarting the bot now.");
                                }

                                Misc.kill_everything_perform_restart();
                            }
                        }
                        catch
                        {
                            // handle exception
                            if (++check_pp_status_count == check_pp_status_maxTries)
                            {
                                if (Main_Form_Init.Telegram_Monitoring == 1)
                                {
                                    Telegram.telegram_alert_send("Player: Spotify\n\nLogin VPN not connected correctly, or configuration is wrong. Restarting the bot now.");
                                }

                                Misc.kill_everything_perform_restart();
                            }
                        }

                        Thread.Sleep(1000);
                    }
                }

                if (Main_Form_Init.chrome_persistent_profile_enabled == 1)
                {
                    driver.Navigate().GoToUrl("https://open.spotify.com/");

                    Thread.Sleep(10000);
                    
                    //Check if account already logged in
                    try
                    {
                        driver.FindElement(By.XPath("//p[text()='Registriere dich, um unbegrenzt Songs und Podcasts mit gelegentlichen Werbeunterbrechungen zu hören. Ganz ohne Kreditkarte']"));

                        driver.Navigate().GoToUrl("https://accounts.spotify.com/login");

                        bool start_login_persistent_flag = true;
                        int start_login_persistent_registercount = 0;
                        int start_login_persistent_registermaxTries = 15;
                        while (start_login_persistent_flag == true)
                        {
                            Thread.Sleep(5000);

                            try
                            {
                                driver.FindElement(By.Id("login-username")).Clear();
                                driver.FindElement(By.Id("login-password")).Clear();

                                driver.FindElement(By.Id("login-username")).SendKeys(Main_Form_Init.spotify_player_username);
                                driver.FindElement(By.Id("login-password")).SendKeys(Main_Form_Init.spotify_player_password);

                                driver.FindElement(By.Id("login-button")).Click();
                                start_login_persistent_flag = false;
                            }
                            catch
                            {
                                // handle exception
                                if (++start_login_persistent_registercount == start_login_persistent_registermaxTries)
                                {
                                    if (Main_Form_Init.Telegram_Monitoring == 1)
                                    {
                                        Telegram.telegram_alert_send("Player: Spotify\n\nLogin could not be started. Restarting the Bot now.");
                                    }

                                    Misc.kill_everything_perform_restart();
                                }

                                Thread.Sleep(1000);
                            }
                        }

                        Thread.Sleep(10000);

                        bool start_login_password_flag = true;
                        int start_login_password_registercount = 0;
                        int start_login_password_registermaxTries = 15;
                        while (start_login_password_flag == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//span[text()='Der Benutzername oder das Passwort ist falsch.']"));

                                //Change creds to outdated
                                try
                                {
                                    Telegram.telegram_critical_alert_send("Player: Spotify\n\nOutdated credentials. Please check." + "\nSpotify Account Mail: " + Main_Form_Init.spotify_player_username + "\nSpotify Account Password: " + Main_Form_Init.spotify_player_password);

                                    Autobot_Helper.outdated_credentials();
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Mark Spotify Creds as Outdated: " + ex.Message + Environment.NewLine);
                                }

                                Misc.kill_everything_perform_restart();
                            }
                            catch
                            {
                                // handle exception
                                if (++start_login_password_registercount == start_login_password_registermaxTries)
                                {
                                    start_login_password_flag = false;
                                }
                                
                                Thread.Sleep(1000);
                            }
                        }

                        //Try to catch weird error
                        bool denied_login_flag = true;
                        int denied_login_registercount = 0;
                        int denied_login_registermaxTries = 15;
                        int max_denied_counter = 0;
                        while (denied_login_flag == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//span[text()='Da ist wohl etwas schiefgegangen. Versuche es noch einmal oder besuche unseren ']"));

                                // handle exception
                                if (++denied_login_registercount == denied_login_registermaxTries)
                                {
                                    Telegram.telegram_alert_send("Player: Spotify\n\nLogin denied max reached. Restarting now.");
                                    Misc.kill_everything_perform_restart();
                                }

                                Thread.Sleep(1000);

                                driver.Navigate().GoToUrl("https://accounts.spotify.com/login");

                                driver.FindElement(By.Id("login-username")).Clear();
                                driver.FindElement(By.Id("login-password")).Clear();

                                driver.FindElement(By.Id("login-username")).SendKeys(Main_Form_Init.spotify_player_username);
                                driver.FindElement(By.Id("login-password")).SendKeys(Main_Form_Init.spotify_player_password);

                                driver.FindElement(By.Id("login-button")).Click();

                                Thread.Sleep(5000);

                                try
                                {
                                    driver.FindElement(By.XPath("//span[text()='Der Benutzername oder das Passwort ist falsch.']"));

                                    //Change creds to outdated
                                    try
                                    {
                                        Telegram.telegram_critical_alert_send("Player: Spotify\n\nOutdated credentials. Please check." + "\nSpotify Account Mail: " + Main_Form_Init.spotify_player_username + "\nSpotify Account Password: " + Main_Form_Init.spotify_player_password);

                                        Autobot_Helper.outdated_credentials();
                                    }
                                    catch (Exception ex)
                                    {
                                        File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Mark Spotify Creds as Outdated: " + ex.Message + Environment.NewLine);
                                    }

                                    Misc.kill_everything_perform_restart();
                                }
                                catch
                                {

                                }
                            }
                            catch
                            {
                                // handle exception
                                if (++max_denied_counter == denied_login_registermaxTries)
                                {
                                    denied_login_flag = false;
                                }

                                Thread.Sleep(1000);
                            }
                        }

                        bool check_login_flag = true;
                        int check_login_registercount = 0;
                        int check_login_registermaxTries = 10;
                        while (check_login_flag == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//a[@analytics-event='Webplayer Button']"));
                                check_login_flag = false;
                            }
                            catch
                            {
                                // handle exception
                                if (++check_login_registercount == check_login_registermaxTries)
                                {
                                    if (Main_Form_Init.Telegram_Monitoring == 1)
                                    {
                                        Telegram.telegram_alert_send("Player: Spotify\n\nThe login check failed. Restarting now.");
                                    }

                                    Logging.ss();

                                    check_login_flag = false;

                                    Misc.kill_everything_perform_restart();
                                }
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    catch //Already logged in continue to the new one
                    {

                    }
                }
                else if (Main_Form_Init.chrome_persistent_profile_enabled == 0)
                {
                    driver.Navigate().GoToUrl("https://accounts.spotify.com/login");

                    bool start_login_flag = true;
                    int start_login_registercount = 0;
                    int start_login_registermaxTries = 90;
                    while (start_login_flag == true)
                    {
                        Thread.Sleep(5000);

                        try
                        {
                            driver.FindElement(By.Id("login-username")).SendKeys(Main_Form_Init.spotify_player_username);
                            driver.FindElement(By.Id("login-password")).SendKeys(Main_Form_Init.spotify_player_password);

                            driver.FindElement(By.Id("login-button")).Click();
                            start_login_flag = false;
                        }
                        catch
                        {
                            // handle exception
                            if (++start_login_registercount == start_login_registermaxTries)
                            {
                                if (Main_Form_Init.Telegram_Monitoring == 1)
                                {
                                    Telegram.telegram_alert_send("Player: Spotify\n\nLogin could not be started. Restarting the Bot now.");
                                }

                                Misc.kill_everything_perform_restart();
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    bool start_login_password_flag = true;
                    int start_login_password_registercount = 0;
                    int start_login_password_registermaxTries = 15;
                    while (start_login_password_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//span[text()='Der Benutzername oder das Passwort ist falsch.']"));

                            //Change creds to outdated
                            try
                            {
                                Telegram.telegram_critical_alert_send("Player: Spotify\n\nOutdated credentials. Please check." + "\nSpotify Account Mail: " + Main_Form_Init.spotify_player_username + "\nSpotify Account Password: " + Main_Form_Init.spotify_player_password);

                                Autobot_Helper.outdated_credentials();
                            }
                            catch (Exception ex)
                            {
                                File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Mark Spotify Creds as Outdated: " + ex.Message + Environment.NewLine);
                            }

                            Misc.kill_everything_perform_restart();
                        }
                        catch
                        {
                            // handle exception
                            if (++start_login_password_registercount == start_login_password_registermaxTries)
                            {
                                start_login_password_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Try to catch weird error
                    bool denied_login_flag = true;
                    int denied_login_registercount = 0;
                    int denied_login_registermaxTries = 15;
                    int max_denied_counter = 0;
                    while (denied_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//span[text()='Da ist wohl etwas schiefgegangen. Versuche es noch einmal oder besuche unseren ']"));

                            max_denied_counter++;

                            driver.Navigate().GoToUrl("https://accounts.spotify.com/login");

                            driver.FindElement(By.Id("login-username")).Clear();
                            driver.FindElement(By.Id("login-password")).Clear();

                            driver.FindElement(By.Id("login-username")).SendKeys(Main_Form_Init.spotify_player_username);
                            driver.FindElement(By.Id("login-password")).SendKeys(Main_Form_Init.spotify_player_password);

                            driver.FindElement(By.Id("login-button")).Click();

                            Thread.Sleep(5000);

                            try
                            {
                                driver.FindElement(By.XPath("//span[text()='Der Benutzername oder das Passwort ist falsch.']"));

                                //Change creds to outdated
                                try
                                {
                                    Telegram.telegram_critical_alert_send("Player: Spotify\n\nOutdated credentials. Please check." + "\nSpotify Account Mail: " + Main_Form_Init.spotify_player_username + "\nSpotify Account Password: " + Main_Form_Init.spotify_player_password);

                                    Autobot_Helper.outdated_credentials();
                                }
                                catch (Exception ex)
                                {
                                    File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Mark Spotify Creds as Outdated: " + ex.Message + Environment.NewLine);
                                }

                                Misc.kill_everything_perform_restart();
                            }
                            catch
                            {

                            }
                        }
                        catch
                        {
                            // handle exception
                            if (++denied_login_registercount == denied_login_registermaxTries)
                            {
                                if (max_denied_counter == denied_login_registermaxTries)
                                {
                                    Telegram.telegram_alert_send("Player: Spotify\n\nLogin denied max reached. Restarting now.");
                                    Misc.kill_everything_perform_restart();
                                }

                                denied_login_flag = false;
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    bool check_login_flag = true;
                    int check_login_registercount = 0;
                    int check_login_registermaxTries = 10;
                    while (check_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//a[@analytics-event='Webplayer Button']"));
                            check_login_flag = false;
                        }
                        catch
                        {
                            // handle exception
                            if (++check_login_registercount == check_login_registermaxTries)
                            {
                                if (Main_Form_Init.Telegram_Monitoring == 1)
                                {
                                    Telegram.telegram_alert_send("Player: Spotify\n\nThe login check failed. Restarting now.");
                                }

                                Logging.ss();

                                check_login_flag = false;

                                Misc.kill_everything_perform_restart();
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    //Misc.suspend_process("chrome");

                    int process_id;

                    foreach (var process in Process.GetProcessesByName("chrome"))
                    {
                        try
                        {
                            process_id = process.Id;
                            SuspendProcess(process_id);
                        }
                        catch
                        {

                        }
                    }

                    openvpn_connect();
                    
                    foreach (var process in Process.GetProcessesByName("chrome"))
                    {
                        try
                        {
                            process_id = process.Id;
                            ResumeProcess(process_id);
                        }
                        catch
                        {

                        }
                    }

                    if (Main_Form_Init.openvpn_health_check_enabled == 1)
                    {
                        driver.Navigate().GoToUrl("https://checkip.perfect-privacy.com/json");

                        bool check_pp_status_bool = true;
                        int check_pp_status_count = 0;
                        int check_pp_status_maxTries = 20;
                        while (check_pp_status_bool == true)
                        {
                            try
                            {
                                string vpn_status = driver.FindElement(By.TagName("pre")).Text;

                                if (vpn_status.Contains("VPN\":true"))
                                {
                                    check_pp_status_bool = false;
                                }
                                else if (vpn_status.Contains("VPN\":false"))
                                {
                                    if (Main_Form_Init.Telegram_Monitoring == 1)
                                    {
                                        Telegram.telegram_alert_send("Player: Spotify\n\nPerfect Privacy VPN not connected correctly (" + Main_Form_Init.openvpn_profile + "), or configuration is wrong. Restarting the bot now.");
                                    }

                                    Misc.kill_everything_perform_restart();
                                }
                            }
                            catch // handle exception
                            {
                                
                                if (++check_pp_status_count == check_pp_status_maxTries)
                                {
                                    if (Main_Form_Init.Telegram_Monitoring == 1)
                                    {
                                        Telegram.telegram_alert_send("Player: Spotify\n\nPerfect Privacy VPN Test failed. Profile:"+ Main_Form_Init.openvpn_profile + "Restarting the bot now.");
                                    }

                                    Misc.kill_everything_perform_restart();
                                }
                            }
                        }
                    }

                    Thread.Sleep(5000);

                    if (Main_Form_Init.player_type == "Playlist")
                    {
                        if (Main_Form_Init.emulate_device_enabled == 0)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl(Main_Form_Init.spotify_playlist_url);
                            }
                            catch (Exception ex)
                            {
                                Logging.log_error(this.Name, "Driver Navigate Playlist", ex.Message);
                            }
                        }
                        else //Emu
                        {
                            driver.FindElement(By.XPath("//a[@href='https://open.spotify.com']")).Click();

                            Thread.Sleep(10000);

                            driver.Navigate().GoToUrl(Main_Form_Init.spotify_playlist_url);
                        }
                    }
                    else if (Main_Form_Init.player_type == "Album")
                    {
                        if (Main_Form_Init.emulate_device_enabled == 0)
                        {
                            try
                            {
                                Main_Form_Init.spotify_album_listbox.SelectedIndex = 0;

                                driver.Navigate().GoToUrl(Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                            }
                            catch (Exception ex)
                            {
                                Logging.log_error(this.Name, "Driver Navigate Album", ex.Message);
                            }
                        }
                        else //Emu
                        {
                            driver.FindElement(By.XPath("//a[@href='https://open.spotify.com']")).Click();

                            Thread.Sleep(10000);

                            Main_Form_Init.spotify_album_listbox.SelectedIndex = 0;
                            driver.Navigate().GoToUrl(Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                        }
                    }

                    //Get Window Size
                    if (Main_Form_Init.emulate_device_enabled == 0)
                    {
                        windows_size_width = driver.Manage().Window.Size.Width;
                        windows_size_height = driver.Manage().Window.Size.Height;

                        //Change Window Size on the flight
                        if (windows_size_width > 1000)
                        {
                            driver.Manage().Window.Size = new Size(1000, 740);
                        }
                        else
                        {
                            driver.Manage().Window.Size = new Size(1040, 784);
                        }
                    }

                    //Check if advertising appeared
                    bool check_advertisement_flag = true;
                    int check_advertisement_count = 0;
                    int check_advertisement_tries = 10;
                    while (check_advertisement_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//h1[text()='Musik ohne Limits']"));
                            driver.FindElement(By.XPath("//button[text()='Schließen']")).Click();
                        }
                        catch
                        {
                            // handle exception
                            if (++check_advertisement_count == check_advertisement_tries)
                            {
                                check_advertisement_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Check if normal Cookie Popups appeared
                    bool check_cookie_normal_flag = true;
                    int check_cookie_normal_count = 0;
                    int check_cookie_normal_tries = 10;
                    while (check_cookie_normal_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
                        }
                        catch
                        {
                            // handle exception
                            if (++check_cookie_normal_count == check_cookie_normal_tries)
                            {
                                check_cookie_normal_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Check if ababnormal Cookie Popups appeared one
                    bool check_cookie_abnormal_flag = true;
                    int check_cookie_abnormal_count = 0;
                    int check_cookie_abnormal_tries = 10;
                    while (check_cookie_abnormal_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("(//button[contains(@class,'onetrust-close-btn-handler onetrust-close-btn-ui')])[2]")).Click();

                            Telegram.telegram_mon_send("Player: Spotify\n\nCatched abnormal Onetrust Cookie Popup and removed it.");
                        }
                        catch
                        {
                            // handle exception
                            if (++check_cookie_abnormal_count == check_cookie_abnormal_tries)
                            {
                                check_cookie_abnormal_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Check if ababnormal Cookie Popups appeared two
                    bool check_cookie_abnormal_two_flag = true;
                    int check_cookie_abnormal_two_count = 0;
                    int check_cookie_abnormal_two_tries = 10;
                    while (check_cookie_abnormal_two_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//*[@id='onetrust-close-btn-container']/button")).Click();

                            Telegram.telegram_mon_send("Player: Spotify\n\nCatched abnormal Onetrust Cookie Popup (two) and removed it.");
                        }
                        catch
                        {
                            // handle exception
                            if (++check_cookie_abnormal_two_count == check_cookie_abnormal_two_tries)
                            {
                                check_cookie_abnormal_two_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Check if device is already playing and select the current one.
                    if (Main_Form_Init.emulate_device_enabled == 0)
                    {
                        bool check_player_used_flag = true;
                        int check_player_used_count = 0;
                        int check_player_max_tries = 10;
                        while (check_player_used_flag == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//div[@class='now-playing-bar']/following-sibling::div[1]"));
                                driver.FindElement(By.XPath("//span[@class='connect-device-picker']//button")).Click();
                                Thread.Sleep(2500);
                                driver.FindElement(By.XPath("//ul[@class='connect-device-list']//button[1]")).Click();

                                Telegram.telegram_mon_send("Player: Spotify\n\nPlayer already in use. Switched to the current one.");

                                check_player_used_flag = false;
                            }
                            catch
                            {
                                // handle exception
                                if (++check_player_used_count == check_player_max_tries)
                                {
                                    check_player_used_flag = false;
                                }

                                Thread.Sleep(1000);
                            }
                        }
                    }
                    else //Emu
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//span[text()='AUF DIESEM SMARTPHONE HÖREN']")).Click();
                        }
                        catch
                        {

                        }
                    }

                    //Check for account flagged as suspicious
                    try
                    {
                        driver.FindElement(By.XPath("//div[@class='ErrorPage__inner']//img[1]"));

                        Telegram.telegram_critical_alert_send("Player: Spotify\n\nThe Player Account has been flagged for suspicious behavior. The System will be shutdown for safety reasons.");

                        Misc.shutdown_pc();
                    }
                    catch
                    {

                    }

                    if (Main_Form_Init.emulate_device_enabled == 0)
                    {
                        //Start Player (Big Play Button)
                        try
                        {
                            driver.FindElement(By.XPath("(//button[@data-testid='play-button'])[2]")).Click();
                        }
                        catch
                        {

                        }

                        Thread.Sleep(5000);

                        //Stop Player (Small Stop Button)
                        try
                        {
                            driver.FindElement(By.XPath("//button[@data-testid='control-button-pause']")).Click();
                        }
                        catch
                        {

                        }


                        Thread.Sleep(5000);

                        //Start Player (Small Play Button)
                        try
                        {
                            driver.FindElement(By.XPath("//button[@data-testid='control-button-play']")).Click();
                        }
                        catch
                        {

                        }
                    }
                    else //Emu
                    {
                        try
                        {
                            if (Main_Form_Init.player_type == "Playlist")
                            {
                                driver.FindElement(By.XPath("//div[@data-testid='track-row']//button")).Click();
                            }
                            else if (Main_Form_Init.player_type == "Album")
                            {
                                driver.FindElement(By.XPath("//div[@id='main']/div[1]/div[1]/div[1]/div[3]/div[2]/button[1]")).Click();
                            }

                            Thread.Sleep(5000);

                            driver.FindElement(By.XPath("//div[@data-test-id='click-target']")).Click();
                        }
                        catch
                        {
                            Misc.kill_everything_perform_restart();
                        }
                    }

                    //Check if player is already in use and switch to the current one if needed.
                    try
                    {
                        if (Main_Form_Init.emulate_device_enabled == 0)
                        {
                            driver.FindElement(By.XPath("//div[text()='Du hörst auf diesem Gerät:']"));

                            driver.FindElement(By.XPath("//span[@class='connect-device-picker']//button")).Click();
                            Thread.Sleep(2500);
                            driver.FindElement(By.XPath("//ul[@class='connect-device-list']//button[1]")).Click();
                        }
                        else //Emu
                        {
                            driver.FindElement(By.XPath("//span[text()='AUF DIESEM SMARTPHONE HÖREN']")).Click();
                        }
                    }
                    catch
                    {

                    }

                    //Skip Methode
                    if (Main_Form_Init.player_type == "Playlist")
                    {
                        if (Main_Form_Init.spotify_autoskip_enabled == 1)
                        {
                            while (Main_Form_Init.spotify_autoskip_enabled == 1)
                            {
                                try
                                {
                                    if (Main_Form_Init.emulate_device_enabled == 0)
                                    {
                                        try
                                        {
                                            driver.FindElement(By.XPath("//button[@data-testid='control-button-play']")).Click();
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    //Init the Skip
                                    try
                                    {
                                        Random r = new Random();
                                        int rInt = r.Next(Main_Form_Init.spotify_autoskip_forward_from, Main_Form_Init.spotify_autoskip_forward_to);
                                        Thread.Sleep(rInt);

                                        if (Main_Form_Init.emulate_device_enabled == 0)
                                        {
                                            driver.FindElement(By.XPath("//button[@data-testid='control-button-skip-forward']")).Click();
                                        }
                                        else //Skip Emu
                                        {
                                            try
                                            {
                                                driver.FindElement(By.XPath("//div[@data-test-id='click-target']")).Click();
                                            }
                                            catch
                                            {

                                            }

                                            driver.FindElement(By.XPath("//div[@id='main']/div[1]/div[3]/div[1]/div[2]/div[2]/div[4]/button[1]")).Click();
                                        }

                                        Clicks = Clicks + 1;
                                    }
                                    catch
                                    {

                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    else if (Main_Form_Init.player_type == "Album")
                    {
                        int skip_counter = 0;
                        int skip_counter_max = 20;

                        if (Main_Form_Init.spotify_autoskip_enabled == 1)
                        {
                            while (Main_Form_Init.spotify_autoskip_enabled == 1)
                            {
                                try
                                {
                                    try
                                    {
                                        //if skip counter same as max then do...
                                        if (skip_counter == skip_counter_max)
                                        {
                                            //reset counter
                                            skip_counter = 0;

                                            if (Main_Form_Init.spotify_album_listbox.Items.Count - 1 > Main_Form_Init.spotify_album_listbox.SelectedIndex)
                                            {
                                                Main_Form_Init.spotify_album_listbox.SelectedIndex++;
                                                driver.Navigate().GoToUrl(Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                                            }
                                            else
                                            {
                                                Main_Form_Init.spotify_album_listbox.SelectedIndex = 0;
                                                driver.Navigate().GoToUrl(Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                                            }

                                            Thread.Sleep(5000);

                                            if (Main_Form_Init.emulate_device_enabled == 0)
                                            {
                                                try
                                                {
                                                    driver.FindElement(By.XPath("(//button[@data-testid='play-button'])[2]")).Click();
                                                }
                                                catch
                                                {

                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    if (Main_Form_Init.emulate_device_enabled == 0)
                                    {
                                        try
                                        {
                                            driver.FindElement(By.XPath("//button[@data-testid='control-button-play']")).Click();
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    //Init the Skip
                                    try
                                    {
                                        Random r = new Random();
                                        int rInt = r.Next(Main_Form_Init.spotify_autoskip_forward_from, Main_Form_Init.spotify_autoskip_forward_to);
                                        Thread.Sleep(rInt);

                                        if (Main_Form_Init.emulate_device_enabled == 0)
                                        {
                                            driver.FindElement(By.XPath("//button[@data-testid='control-button-skip-forward']")).Click();
                                        }
                                        else //Skip Emu
                                        {
                                            try
                                            {
                                                driver.FindElement(By.XPath("//div[@data-test-id='click-target']")).Click();
                                            }
                                            catch
                                            {

                                            }

                                            driver.FindElement(By.XPath("//div[@id='main']/div[1]/div[3]/div[1]/div[2]/div[2]/div[4]/button[1]")).Click();
                                        }
                                        skip_counter++;
                                        Clicks = Clicks + 1;
                                    }
                                    catch
                                    {

                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
                
                //If persistent profile enabled, directly jump to the new one
                if (Main_Form_Init.chrome_persistent_profile_enabled == 1)
                {
                    //Navigate to PP to prevent IP Leaks
                    driver.Navigate().GoToUrl("https://checkip.perfect-privacy.com/json");

                    Thread.Sleep(5000);
                    
                    int process_id;

                    foreach (var process in Process.GetProcessesByName("chrome"))
                    {
                        try
                        {
                            process_id = process.Id;
                            SuspendProcess(process_id);
                        }
                        catch
                        {

                        }
                    }

                    openvpn_connect();

                    foreach (var process in Process.GetProcessesByName("chrome"))
                    {
                        try
                        {
                            process_id = process.Id;
                            ResumeProcess(process_id);
                        }
                        catch
                        {

                        }
                    }

                    if (Main_Form_Init.openvpn_health_check_enabled == 1)
                    {
                        driver.Navigate().GoToUrl("https://checkip.perfect-privacy.com/json");

                        bool check_pp_status_bool = true;
                        int check_pp_status_count = 0;
                        int check_pp_status_maxTries = 20;
                        while (check_pp_status_bool == true)
                        {
                            try
                            {
                                string vpn_status = driver.FindElement(By.TagName("pre")).Text;

                                if (vpn_status.Contains("VPN\":true"))
                                {
                                    check_pp_status_bool = false;
                                }
                                else if (vpn_status.Contains("VPN\":false"))
                                {
                                    if (Main_Form_Init.Telegram_Monitoring == 1)
                                    {
                                        Telegram.telegram_alert_send("Player: Spotify\n\nPerfect Privacy VPN not connected correctly (" + Main_Form_Init.openvpn_profile + "), or configuration is wrong. Restarting the bot now.");
                                    }

                                    Misc.kill_everything_perform_restart();
                                }
                            }
                            catch // handle exception
                            {

                                if (++check_pp_status_count == check_pp_status_maxTries)
                                {
                                    if (Main_Form_Init.Telegram_Monitoring == 1)
                                    {
                                        Telegram.telegram_alert_send("Player: Spotify\n\nPerfect Privacy VPN Test failed. Profile:" + Main_Form_Init.openvpn_profile + "Restarting the bot now.");
                                    }

                                    Misc.kill_everything_perform_restart();
                                }
                            }
                        }
                    }

                    Thread.Sleep(5000);

                    if (Main_Form_Init.player_type == "Playlist")
                    {
                        if (Main_Form_Init.emulate_device_enabled == 0)
                        {
                            try
                            {
                                driver.Navigate().GoToUrl(Main_Form_Init.spotify_playlist_url);
                            }
                            catch (Exception ex)
                            {
                                Logging.log_error(this.Name, "Driver Navigate Playlist", ex.Message);
                            }
                        }
                        else //Emu
                        {
                            driver.FindElement(By.XPath("//a[@href='https://open.spotify.com']")).Click();

                            Thread.Sleep(10000);

                            driver.Navigate().GoToUrl(Main_Form_Init.spotify_playlist_url);
                        }
                    }
                    else if (Main_Form_Init.player_type == "Album")
                    {
                        if (Main_Form_Init.emulate_device_enabled == 0)
                        {
                            try
                            {
                                Main_Form_Init.spotify_album_listbox.SelectedIndex = 0;

                                driver.Navigate().GoToUrl(Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                            }
                            catch (Exception ex)
                            {
                                Logging.log_error(this.Name, "Driver Navigate Album", ex.Message);
                            }
                        }
                        else //Emu
                        {
                            driver.FindElement(By.XPath("//a[@href='https://open.spotify.com']")).Click();

                            Thread.Sleep(10000);

                            Main_Form_Init.spotify_album_listbox.SelectedIndex = 0;
                            driver.Navigate().GoToUrl(Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                        }
                    }

                    //Get Window Size
                    if (Main_Form_Init.emulate_device_enabled == 0)
                    {
                        windows_size_width = driver.Manage().Window.Size.Width;
                        windows_size_height = driver.Manage().Window.Size.Height;

                        //Change Window Size on the flight
                        if (windows_size_width > 1000)
                        {
                            driver.Manage().Window.Size = new Size(1000, 740);
                        }
                        else
                        {
                            driver.Manage().Window.Size = new Size(1040, 784);
                        }
                    }

                    //Check if advertising appeared
                    bool check_advertisement_flag = true;
                    int check_advertisement_count = 0;
                    int check_advertisement_tries = 10;
                    while (check_advertisement_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//h1[text()='Musik ohne Limits']"));
                            driver.FindElement(By.XPath("//button[text()='Schließen']")).Click();
                        }
                        catch
                        {
                            // handle exception
                            if (++check_advertisement_count == check_advertisement_tries)
                            {
                                check_advertisement_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Check if normal Cookie Popups appeared
                    bool check_cookie_normal_flag = true;
                    int check_cookie_normal_count = 0;
                    int check_cookie_normal_tries = 10;
                    while (check_cookie_normal_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
                        }
                        catch
                        {
                            // handle exception
                            if (++check_cookie_normal_count == check_cookie_normal_tries)
                            {
                                check_cookie_normal_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Check if ababnormal Cookie Popups appeared one
                    bool check_cookie_abnormal_flag = true;
                    int check_cookie_abnormal_count = 0;
                    int check_cookie_abnormal_tries = 10;
                    while (check_cookie_abnormal_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("(//button[contains(@class,'onetrust-close-btn-handler onetrust-close-btn-ui')])[2]")).Click();

                            Telegram.telegram_mon_send("Player: Spotify\n\nCatched abnormal Onetrust Cookie Popup and removed it.");
                        }
                        catch
                        {
                            // handle exception
                            if (++check_cookie_abnormal_count == check_cookie_abnormal_tries)
                            {
                                check_cookie_abnormal_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Check if ababnormal Cookie Popups appeared two
                    bool check_cookie_abnormal_two_flag = true;
                    int check_cookie_abnormal_two_count = 0;
                    int check_cookie_abnormal_two_tries = 10;
                    while (check_cookie_abnormal_two_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//*[@id='onetrust-close-btn-container']/button")).Click();

                            Telegram.telegram_mon_send("Player: Spotify\n\nCatched abnormal Onetrust Cookie Popup (two) and removed it.");
                        }
                        catch
                        {
                            // handle exception
                            if (++check_cookie_abnormal_two_count == check_cookie_abnormal_two_tries)
                            {
                                check_cookie_abnormal_two_flag = false;
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    //Check if device is already playing and select the current one.
                    if (Main_Form_Init.emulate_device_enabled == 0)
                    {
                        bool check_player_used_flag = true;
                        int check_player_used_count = 0;
                        int check_player_max_tries = 10;
                        while (check_player_used_flag == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//div[@class='now-playing-bar']/following-sibling::div[1]"));
                                driver.FindElement(By.XPath("//span[@class='connect-device-picker']//button")).Click();
                                Thread.Sleep(2500);
                                driver.FindElement(By.XPath("//ul[@class='connect-device-list']//button[1]")).Click();

                                Telegram.telegram_mon_send("Player: Spotify\n\nPlayer already in use. Switched to the current one.");

                                check_player_used_flag = false;
                            }
                            catch
                            {
                                // handle exception
                                if (++check_player_used_count == check_player_max_tries)
                                {
                                    check_player_used_flag = false;
                                }

                                Thread.Sleep(1000);
                            }
                        }
                    }
                    else //Emu
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//span[text()='AUF DIESEM SMARTPHONE HÖREN']")).Click();
                        }
                        catch
                        {

                        }
                    }

                    //Check for account flagged as suspicious
                    try
                    {
                        driver.FindElement(By.XPath("//div[@class='ErrorPage__inner']//img[1]"));

                        Telegram.telegram_critical_alert_send("Player: Spotify\n\nThe Player Account has been flagged for suspicious behavior. The System will be shutdown for safety reasons.");

                        Misc.shutdown_pc();
                    }
                    catch
                    {

                    }

                    if (Main_Form_Init.emulate_device_enabled == 0)
                    {
                        //Start Player (Big Play Button)
                        try
                        {
                            driver.FindElement(By.XPath("(//button[@data-testid='play-button'])[2]")).Click();
                        }
                        catch
                        {

                        }

                        Thread.Sleep(5000);

                        //Stop Player (Small Stop Button)
                        try
                        {
                            driver.FindElement(By.XPath("//button[@data-testid='control-button-pause']")).Click();
                        }
                        catch
                        {

                        }


                        Thread.Sleep(5000);

                        //Start Player (Small Play Button)
                        try
                        {
                            driver.FindElement(By.XPath("//button[@data-testid='control-button-play']")).Click();
                        }
                        catch
                        {

                        }
                    }
                    else //Emu
                    {
                        try
                        {
                            if (Main_Form_Init.player_type == "Playlist")
                            {
                                driver.FindElement(By.XPath("//div[@data-testid='track-row']//button")).Click();
                            }
                            else if (Main_Form_Init.player_type == "Album")
                            {
                                driver.FindElement(By.XPath("//div[@id='main']/div[1]/div[1]/div[1]/div[3]/div[2]/button[1]")).Click();
                            }

                            Thread.Sleep(5000);

                            driver.FindElement(By.XPath("//div[@data-test-id='click-target']")).Click();
                        }
                        catch
                        {
                            Misc.kill_everything_perform_restart();
                        }
                    }

                    //Check if player is already in use and switch to the current one if needed.
                    try
                    {
                        if (Main_Form_Init.emulate_device_enabled == 0)
                        {
                            driver.FindElement(By.XPath("//div[text()='Du hörst auf diesem Gerät:']"));

                            driver.FindElement(By.XPath("//span[@class='connect-device-picker']//button")).Click();
                            Thread.Sleep(2500);
                            driver.FindElement(By.XPath("//ul[@class='connect-device-list']//button[1]")).Click();
                        }
                        else //Emu
                        {
                            driver.FindElement(By.XPath("//span[text()='AUF DIESEM SMARTPHONE HÖREN']")).Click();
                        }
                    }
                    catch
                    {

                    }

                    //Skip Methode
                    if (Main_Form_Init.player_type == "Playlist")
                    {
                        if (Main_Form_Init.spotify_autoskip_enabled == 1)
                        {
                            while (Main_Form_Init.spotify_autoskip_enabled == 1)
                            {
                                try
                                {
                                    if (Main_Form_Init.emulate_device_enabled == 0)
                                    {
                                        try
                                        {
                                            driver.FindElement(By.XPath("//button[@data-testid='control-button-play']")).Click();
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    //Init the Skip
                                    try
                                    {
                                        Random r = new Random();
                                        int rInt = r.Next(Main_Form_Init.spotify_autoskip_forward_from, Main_Form_Init.spotify_autoskip_forward_to);
                                        Thread.Sleep(rInt);

                                        if (Main_Form_Init.emulate_device_enabled == 0)
                                        {
                                            driver.FindElement(By.XPath("//button[@data-testid='control-button-skip-forward']")).Click();
                                        }
                                        else //Skip Emu
                                        {
                                            try
                                            {
                                                driver.FindElement(By.XPath("//div[@data-test-id='click-target']")).Click();
                                            }
                                            catch
                                            {

                                            }

                                            driver.FindElement(By.XPath("//div[@id='main']/div[1]/div[3]/div[1]/div[2]/div[2]/div[4]/button[1]")).Click();
                                        }

                                        Clicks = Clicks + 1;
                                    }
                                    catch
                                    {

                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    else if (Main_Form_Init.player_type == "Album")
                    {
                        int skip_counter = 0;
                        int skip_counter_max = 20;

                        if (Main_Form_Init.spotify_autoskip_enabled == 1)
                        {
                            while (Main_Form_Init.spotify_autoskip_enabled == 1)
                            {
                                try
                                {
                                    try
                                    {
                                        //if skip counter same as max then do...
                                        if (skip_counter == skip_counter_max)
                                        {
                                            //reset counter
                                            skip_counter = 0;

                                            if (Main_Form_Init.spotify_album_listbox.Items.Count - 1 > Main_Form_Init.spotify_album_listbox.SelectedIndex)
                                            {
                                                Main_Form_Init.spotify_album_listbox.SelectedIndex++;
                                                driver.Navigate().GoToUrl(Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                                            }
                                            else
                                            {
                                                Main_Form_Init.spotify_album_listbox.SelectedIndex = 0;
                                                driver.Navigate().GoToUrl(Main_Form_Init.spotify_album_listbox.SelectedItem.ToString());
                                            }

                                            Thread.Sleep(5000);

                                            if (Main_Form_Init.emulate_device_enabled == 0)
                                            {
                                                try
                                                {
                                                    driver.FindElement(By.XPath("(//button[@data-testid='play-button'])[2]")).Click();
                                                }
                                                catch
                                                {

                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    if (Main_Form_Init.emulate_device_enabled == 0)
                                    {
                                        try
                                        {
                                            driver.FindElement(By.XPath("//button[@data-testid='control-button-play']")).Click();
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    //Init the Skip
                                    try
                                    {
                                        Random r = new Random();
                                        int rInt = r.Next(Main_Form_Init.spotify_autoskip_forward_from, Main_Form_Init.spotify_autoskip_forward_to);
                                        Thread.Sleep(rInt);

                                        if (Main_Form_Init.emulate_device_enabled == 0)
                                        {
                                            driver.FindElement(By.XPath("//button[@data-testid='control-button-skip-forward']")).Click();
                                        }
                                        else //Skip Emu
                                        {
                                            try
                                            {
                                                driver.FindElement(By.XPath("//div[@data-test-id='click-target']")).Click();
                                            }
                                            catch
                                            {

                                            }

                                            driver.FindElement(By.XPath("//div[@id='main']/div[1]/div[3]/div[1]/div[2]/div[2]/div[4]/button[1]")).Click();
                                        }
                                        skip_counter++;
                                        Clicks = Clicks + 1;
                                    }
                                    catch
                                    {

                                    }
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
            };
            mk.BeginInvoke(callbackfunction, null);
        }

        private Rectangle FindImageOnScreen(Bitmap bmpMatch, bool ExactMatch)
        {
            Bitmap ScreenBmp = new Bitmap(@"C:\Auto_Bot\Temp\" + ss + ".png");

            BitmapData ImgBmd = bmpMatch.LockBits(new Rectangle(0, 0, bmpMatch.Width, bmpMatch.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData ScreenBmd = ScreenBmp.LockBits(new Rectangle(0, 0, ScreenBmp.Width, ScreenBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            byte[] ImgByts = new byte[(Math.Abs(ImgBmd.Stride) * bmpMatch.Height) - 1 + 1];
            byte[] ScreenByts = new byte[(Math.Abs(ScreenBmd.Stride) * ScreenBmp.Height) - 1 + 1];

            Marshal.Copy(ImgBmd.Scan0, ImgByts, 0, ImgByts.Length);
            Marshal.Copy(ScreenBmd.Scan0, ScreenByts, 0, ScreenByts.Length);

            bool FoundMatch = false;
            Rectangle rct = Rectangle.Empty;
            int sindx, iindx;
            int spc, ipc;

            int skpx = Convert.ToInt32((bmpMatch.Width - 1) / (double)10);
            if (skpx < 1 | ExactMatch)
                skpx = 1;
            int skpy = Convert.ToInt32((bmpMatch.Height - 1) / (double)10);
            if (skpy < 1 | ExactMatch)
                skpy = 1;

            for (int si = 0; si <= ScreenByts.Length - 1; si += 3)
            {
                FoundMatch = true;
                for (int iy = 0; iy <= ImgBmd.Height - 1; iy += skpy)
                {
                    for (int ix = 0; ix <= ImgBmd.Width - 1; ix += skpx)
                    {
                        sindx = (iy * ScreenBmd.Stride) + (ix * 3) + si;
                        iindx = (iy * ImgBmd.Stride) + (ix * 3);
                        spc = Color.FromArgb(ScreenByts[sindx + 2], ScreenByts[sindx + 1], ScreenByts[sindx]).ToArgb();
                        ipc = Color.FromArgb(ImgByts[iindx + 2], ImgByts[iindx + 1], ImgByts[iindx]).ToArgb();
                        if (spc != ipc)
                        {
                            FoundMatch = false;
                            iy = ImgBmd.Height - 1;
                            ix = ImgBmd.Width - 1;
                        }
                    }
                }
                if (FoundMatch)
                {
                    double r = si / (double)(ScreenBmp.Width * 3);
                    double c = ScreenBmp.Width * (r % 1);
                    if (r % 1 >= 0.5)
                        r -= 1;
                    rct.X = Convert.ToInt32(c);
                    rct.Y = Convert.ToInt32(r);
                    rct.Width = bmpMatch.Width;
                    rct.Height = bmpMatch.Height;
                    break;
                }
            }

            bmpMatch.UnlockBits(ImgBmd);
            ScreenBmp.UnlockBits(ScreenBmd);
            ScreenBmp.Dispose();
            return rct;
        }

        public void SimulateMove(int target_X, int target_Y, int steps)
        {
            //Set Random Mouse Cursor Position
            Random random_cur = new Random();
            SetCursorPos(random_cur.Next(50, 1920), random_cur.Next(50, 1920));

            //Get current Mouse Cursor Position
            PointF iterPoint = Cursor.Position;

            //Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(target_X - iterPoint.X, target_Y - iterPoint.Y);

            //Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;

            //Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                SetCursorPos(Convert.ToInt32(iterPoint.X), Convert.ToInt32(iterPoint.Y));
                Random random_ms = new Random();
                Thread.Sleep(random_ms.Next(1, 30));
            }

            //Move the mouse to the final destination.
            SetCursorPos(target_X, target_Y);
        }

        private void take_ss()
        {
            try
            {
                ss_count++;
                ss = "ss_" + ss_count.ToString();

                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;

                Rectangle rect1 = new Rectangle(0, 0, screenWidth, screenHeight);
                Bitmap bmp = new Bitmap(rect1.Width, rect1.Height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect1.Left, rect1.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                bmp.Save(@"C:\Auto_Bot\Temp\" + ss + ".png", ImageFormat.Png);
                bmp.Dispose();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + ss.ToString());
            }
        }

        private static void SuspendProcess(int pid)
        {
            var process = Process.GetProcessById(pid); // throws exception if process does not exist

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                SuspendThread(pOpenThread);

                CloseHandle(pOpenThread);
            }
        }

        public static void ResumeProcess(int pid)
        {
            var process = Process.GetProcessById(pid);

            if (process.ProcessName == string.Empty)
                return;

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                CloseHandle(pOpenThread);
            }
        }
    }
}
