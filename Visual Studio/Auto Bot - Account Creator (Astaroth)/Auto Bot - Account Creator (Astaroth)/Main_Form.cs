using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.IO;
using Encryption;
using System.Net;
using System.Threading;
using System.Net.Http;
using System.Collections.Specialized;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support;
using Newtonsoft.Json;

namespace Auto_Bot___Account_Creator__Astaroth_
{
    public partial class Main_Form : Form
    {
        public int ss_count = 0;
        public string ss = "ss_";

        public string last_gen_timestamp;

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //Hides Chromedriver
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        public static HttpClient http_client = new HttpClient();

        private void hide_chromedriver()
        {
            int hWnd;
            Process[] processRunning = Process.GetProcesses();
            foreach (Process pr in processRunning)
            {
                if (pr.ProcessName == "chromedriver")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
            }
        }
        
        private void callbackfunction(IAsyncResult res)
        {

        }

        public Main_Form()
        {
            InitializeComponent();

            if (Directory.Exists(@"C:\autobot\ss\") == false)
                Directory.CreateDirectory(@"C:\autobot\ss\");
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    if (Properties.Settings.Default.telegram_notifications_enabled == true)
                        telegram_notifications_checkbox.Checked = true;
                    else
                        telegram_notifications_checkbox.Checked = false;
                }
                catch
                {
         
                }

                try
                {
                    if (Properties.Settings.Default.adb_mobile_data_enabled == true)
                        adb_mobile_data_checkbox.Checked = true;
                    else
                        adb_mobile_data_checkbox.Checked = false;
                }
                catch
                {

                }

                try
                {
                    if (Properties.Settings.Default.gigacube_mobile_data_enabled == true)
                        giga_cube_mobile_data_checkbox.Checked = true;
                    else
                        giga_cube_mobile_data_checkbox.Checked = false;
                }
                catch
                {

                }

                try
                {
                    if (Properties.Settings.Default.mail_domain_randomize == true)
                        randomize_mail_domain_checkbox.Checked = true;
                    else
                        randomize_mail_domain_checkbox.Checked = false;
                }
                catch
                {

                }

                try
                {
                    if (Properties.Settings.Default.gender_randomize_enabled == true)
                        gender_randomize_checkbox.Checked = true;
                    else
                        gender_randomize_checkbox.Checked = false;
                }
                catch
                {

                }

                try
                {
                    mail_domain_textbox.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.mail_domain, Settings.Encryption_Key);
                }
                catch
                {

                }

                try
                {
                    telegram_alert_chat_id_textbox.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.telegram_alert_id, Settings.Encryption_Key);

                }
                catch
                {

                }

                try
                {
                    telegram_api_token_textbox.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.telegram_api_key, Settings.Encryption_Key);

                }
                catch
                {

                }

                try
                {
                    max_accounts_count_textbox.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.count, Settings.Encryption_Key);

                }
                catch
                {

                }

                try
                {
                    auto_bot_username_textbox.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.username, Settings.Encryption_Key);

                }
                catch
                {

                }

                try
                {
                    auto_bot_password_textbox.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.password, Settings.Encryption_Key);

                }
                catch
                {

                }

                try
                {
                    settings_gigacube_username_textbox.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.gigacube_username, Settings.Encryption_Key);

                }
                catch
                {

                }

                try
                {
                    settings_gigacube_password_textbox.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.gigacube_password, Settings.Encryption_Key);

                }
                catch
                {

                }

                try
                {
                    firefox_profile_path.Text = Encryption.String_Encryption.Decrypt(Properties.Settings.Default.firefox_profile_path, Settings.Encryption_Key);
                }
                catch
                {

                }
            }
            catch
            {

            }
        }

        private void spotify_register_button()
        {
             //Click Buster Logo
            Bitmap spotify_register_button = new Bitmap(Application.StartupPath + @"\ss_patterns\spotify_register_button.png");
            bool spotify_register_button_flag = true;
            int spotify_register_button_count = 0;
            int spotify_register_button_maxTries = 10;
            while (spotify_register_button_flag == true)
            {
                try
                {
                    //Take Screenshot
                    Astaroth_Core.Astaroth_Core.take_ss();

                    Rectangle pp_rect = Astaroth_Core.Astaroth_Core.FindImageOnScreen(spotify_register_button, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        Astaroth_Core.Astaroth_Core.SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

                        mouse_event(MOUSEEVENTF_LEFTDOWN, pp_rect.X, pp_rect.Y, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, pp_rect.X, pp_rect.Y, 0, 0);

                        spotify_register_button_flag = false;
                    }
                    else
                    {
                        // handle exception
                        if (++spotify_register_button_count == spotify_register_button_maxTries)
                        {
                            spotify_register_button_flag = false;

                            Thread.Sleep(20000);
                        }

                        Thread.Sleep(5000);
                    }
                }
                catch
                {

                }
            }
        }

        //Start Instance
        private void start_instance_button_Click(object sender, EventArgs e)
        {
            //Autobot Creds
            string autobot_username = auto_bot_username_textbox.Text;
            string autobot_password = auto_bot_password_textbox.Text;

            //Gigacube Credentials
            string gigacube_username = settings_gigacube_username_textbox.Text;
            string gigacube_password = settings_gigacube_password_textbox.Text;

            int count = 0;
            int max_count = Convert.ToInt32(max_accounts_count_textbox.Text);

            string mail_domain = mail_domain_textbox.Text;

            List<string> mail_domain_list = new List<string>(new string[] { });

            if (randomize_mail_domain_checkbox.Checked)
            {
                foreach (var domain in mail_domain_combobox.Items)
                    mail_domain_list.Add(domain.ToString());
            }

            string gender = "";

            List<string> gender_list = new List<string>(new string[] { });
            gender_list.Add("male");
            gender_list.Add("female");
            gender_list.Add("homo");

            if (male_checkbox.Checked)
            {
                gender = "male";
            }
            else if (female_checkbox.Checked)
            {
                gender = "female";
            }
            else if (homo_checkbox.Checked)
            {
                gender = "homo";
            }

            bool gigacube = false;

            if (giga_cube_mobile_data_checkbox.Checked)
                gigacube = true;

            string cookie_encoded = "";

            //Declare Astaroth Class
            Astaroth_Google_Recaptcha.Astaroth_Google_Recaptcha Astaroth_Google_Recaptcha = new Astaroth_Google_Recaptcha.Astaroth_Google_Recaptcha();
            Astaroth_Spotify.Astaroth_Spotify Astaroth_Spotify = new Astaroth_Spotify.Astaroth_Spotify();

            MethodInvoker mk = delegate
            {
                while (count != max_count)
                {
                    try
                    {
                        count++;

                        check_status.Start();

                        foreach (var process in Process.GetProcessesByName("geckodriver"))
                        {
                            try
                            {
                                process.Kill();
                                process.WaitForExit();
                            }
                            catch
                            {

                            }
                        }

                        foreach (var process in Process.GetProcessesByName("firefox"))
                        {
                            try
                            {
                                process.Kill();
                                process.WaitForExit();
                            }
                            catch
                            {

                            }
                        }

                        Thread.Sleep(2500);

                        //Delete Firefox Cookies
                        try
                        {
                            if (File.Exists(firefox_profile_path.Text + @"\cookies.sqlite"))
                                File.Delete(firefox_profile_path.Text + @"\cookies.sqlite");
                        }
                        catch (Exception ex)
                        {
                            Logging.Logging.log_error(this.Name, "Delete Firefox Cookies", ex.Message);
                        }

                        Random random = new Random();
                        try
                        {
                            int rInt_mail_domain = random.Next(0, mail_domain_list.Count);
                            mail_domain = mail_domain_list[rInt_mail_domain];
                        }
                        catch (Exception ex)
                        {
                            Logging.Logging.log_error(this.Name, "Randomize Mail Domain", ex.Message);
                        }

                        try
                        {
                            int rInt_gender = random.Next(0, gender_list.Count - 1);
                            gender = gender_list[rInt_gender];
                        }
                        catch (Exception ex)
                        {
                            Logging.Logging.log_error(this.Name, "Randomize Gender", ex.Message);
                        }

                        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        string username = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
                        string password = new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());

                        //Generate Random Year
                        Random random_day_r = new Random();
                        int random_day_int = random_day_r.Next(1, 30);

                        //Generate Random Month
                        var random_month_chars = "jfmajasond";
                        var random_month_char_length = new char[1];
                        var random_month_char_rand = new Random();

                        for (int i = 0; i < random_month_char_length.Length; i++)
                        {
                            random_month_char_length[i] = random_month_chars[random_month_char_rand.Next(random_month_chars.Length)];
                        }

                        var random_month_final = new String(random_month_char_length);

                        //Generate Random Year
                        Random random_year_r = new Random();
                        int random_year_int = random_year_r.Next(1970, 2005);

                        try
                        {
                            if (gigacube)
                            {
                                foreach (var process in Process.GetProcessesByName("geckodriver"))
                                {
                                    try
                                    {
                                        process.Kill();
                                        process.WaitForExit();
                                    }
                                    catch
                                    {

                                    }
                                }

                                foreach (var process in Process.GetProcessesByName("firefox"))
                                {
                                    try
                                    {
                                        process.Kill();
                                        process.WaitForExit();
                                    }
                                    catch
                                    {

                                    }
                                }
                                
                                //IWebDriver driver = new ChromeDriver(Application.StartupPath +  @"\driver", chrome_options);
                                IWebDriver driver = new FirefoxDriver(Application.StartupPath + @"\driver");

                                driver.Navigate().GoToUrl("http://192.168.8.1");

                                Thread.Sleep(2500);

                                driver.FindElement(By.XPath("//span[text()='Anmelden']")).Click();

                                Thread.Sleep(2500);

                                driver.FindElement(By.XPath("//div[@class='QR_login_password']//input[1]")).Click();
                                driver.FindElement(By.XPath("//div[@class='QR_login_password']//input[1]")).Clear();
                                driver.FindElement(By.XPath("//div[@class='QR_login_password']//input[1]")).SendKeys(gigacube_password);
                                driver.FindElement(By.XPath("//span[contains(@class,'button_wrapper pop_login')]//input[1]")).Click();

                                Thread.Sleep(2500);

                                driver.Navigate().GoToUrl("http://192.168.8.1/html/reboot.html");
                                Thread.Sleep(2500);
                                driver.FindElement(By.Id("reboot_apply_button")).Click();
                                driver.FindElement(By.Id("pop_confirm")).Click();

                                Thread.Sleep(90000);

                                foreach (var process in Process.GetProcessesByName("geckodriver"))
                                {
                                    try
                                    {
                                        process.Kill();
                                        process.WaitForExit();
                                    }
                                    catch
                                    {

                                    }
                                }

                                foreach (var process in Process.GetProcessesByName("firefox"))
                                {
                                    try
                                    {
                                        process.Kill();
                                        process.WaitForExit();
                                    }
                                    catch
                                    {

                                    }
                                }
                            }

                            Process.Start(@"C:\Program Files\Mozilla Firefox\firefox.exe", "-new-instance -P autobot https://www.spotify.com/de/signup/");
                            
                            Astaroth_Spotify.cookie_button();
                            Astaroth_Spotify.fill_mail_textbox(username + "@" + mail_domain);
                            Astaroth_Spotify.cookie_button();
                            Astaroth_Spotify.fill_password_textbox(password);
                            Astaroth_Spotify.cookie_button();
                            Astaroth_Spotify.fill_username_textbox(username);
                            Astaroth_Spotify.cookie_button();
                            Astaroth_Spotify.mail_header();

                            Astaroth_Spotify.page_down(50);

                            Thread.Sleep(5000);

                            Astaroth_Spotify.cookie_button();
                            Astaroth_Spotify.fill_birtday_day_textbox(random_day_int.ToString());
                            Astaroth_Spotify.fill_birtday_month_textbox(random_month_final);
                            Astaroth_Spotify.fill_birtday_year_textbox(random_year_int.ToString());

                            if (gender == "male")
                            {
                                try
                                {
                                    Astaroth_Spotify.check_male_checkbox();
                                }
                                catch
                                {
                                    Telegram.Telegram.telegram_alert_send("Can't check male. Restarting now.");
                                }
                            }
                            else if (gender == "female")
                            {
                                try
                                {
                                    Astaroth_Spotify.check_female_checkbox();
                                }
                                catch
                                {
                                    Telegram.Telegram.telegram_alert_send("Can't check female. Restarting now.");
                                }
                            }
                            else if (gender == "homo")
                            {
                                try
                                {
                                    Astaroth_Spotify.check_homo_checkbox();
                                }
                                catch
                                {
                                    Telegram.Telegram.telegram_alert_send("Can't check homo. Restarting now.");
                                }
                            }

                            //Accept TOS
                            Astaroth_Spotify.check_tos_checkbox();

                            Astaroth_Spotify.cookie_button();

                            Astaroth_Google_Recaptcha.click_recaptcha();

                            bool recaptcha_status = false;

                            recaptcha_status = Astaroth_Google_Recaptcha.check_recaptcha_confirmed(recaptcha_status);

                            //MessageBox.Show(recaptcha_status.ToString());

                            if (recaptcha_status == false)
                            {
                                Astaroth_Google_Recaptcha.click_buster();

                                Astaroth_Google_Recaptcha.recaptcha_multiple_solutions_check();

                                Astaroth_Google_Recaptcha.check_recaptcha_confirmed(recaptcha_status);
                            }

                            bool recaptcha_detected_result = false;

                            Astaroth_Google_Recaptcha.recaptcha_detected_check(recaptcha_detected_result);

                            try
                            {
                                spotify_register_button();
                            }
                            catch (Exception ex)
                            {
                                Logging.Logging.log_error(this.Name, "Click register button.", ex.Message);
                            }

                            bool account_creation_finished = false;

                            account_creation_finished = Astaroth_Spotify.account_creation_finished(account_creation_finished);

                            if (account_creation_finished == true)
                            {
                                last_gen_timestamp = DateTime.Now.ToString();

                                foreach (var process in Process.GetProcessesByName("geckodriver"))
                                {
                                    try
                                    {
                                        process.Kill();
                                        process.WaitForExit();
                                    }
                                    catch
                                    {

                                    }
                                }

                                foreach (var process in Process.GetProcessesByName("firefox"))
                                {
                                    try
                                    {
                                        process.Kill();
                                        process.WaitForExit();
                                    }
                                    catch
                                    {

                                    }
                                }

                                Thread.Sleep(5000);

                                //Firefox Cookie Encoding
                                try
                                {
                                    byte[] byte_array = File.ReadAllBytes(firefox_profile_path.Text + @"\cookies.sqlite");
                                    cookie_encoded = Convert.ToBase64String(byte_array);
                                }
                                catch (Exception ex)
                                {
                                    Logging.Logging.log_error(this.Name, "Encode Cookies File to Base64.", ex.Message);
                                }

                                try
                                {
                                    http_client.DefaultRequestHeaders.Add("User-Agent", Settings.Useragent);

                                    var values = new Dictionary<string, string>
                                    {
                                        { "username", autobot_username },
                                        { "password", autobot_password },
                                        { "mysql_database", "admin_autobot_dev" },
                                        { "add_credentials_username", username + "@" + mail_domain },
                                        { "add_credentials_password", Encryption.String_Encryption.Encrypt(password, Settings.Encryption_Key) },
                                        { "cookies_file", cookie_encoded }
                                    };

                                    var items = values.Select(i => WebUtility.UrlEncode(i.Key) + "=" + WebUtility.UrlEncode(i.Value));
                                    var content = new StringContent(String.Join("&", items), null, "application/x-www-form-urlencoded");

                                    var response = http_client.PostAsync(Settings.Helper, content);

                                    string response_string = response.Result.Content.ReadAsStringAsync().Result;
                                }
                                catch (Exception ex)
                                {
                                    Logging.Logging.log_error(this.Name, "Send credentials to server.", ex.Message);
                                }

                                Telegram.Telegram.telegram_alert_send("Account created successfully.");
                            }
                            else
                            {
                                Telegram.Telegram.telegram_alert_send("Creating Account failed.");
                            }

                            check_status.Stop();
                        }
                        catch(Exception ex)
                        {
                            Logging.Logging.log_error(this.Name, "Start driver.", ex.Message);
                        }
                    }
                    catch
                    {
                        check_status.Stop();
                        this.BeginInvoke(new MethodInvoker(restart_instance));
                        return;
                    }
                }
            };
            mk.BeginInvoke(callbackfunction, null);
        }

        private void restart_instance()
        {
            close_instance_button.PerformClick();
            start_instance_button.PerformClick();
        }

        private void telegram_notifications_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (telegram_notifications_checkbox.Checked)
            {
                Properties.Settings.Default.telegram_notifications_enabled = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.telegram_notifications_enabled = false;
                Properties.Settings.Default.Save();
            }
        }

        private void telegram_alert_chat_id_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.telegram_alert_id = Encryption.String_Encryption.Encrypt(telegram_alert_chat_id_textbox.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }

        private void telegram_api_token_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.telegram_api_key = Encryption.String_Encryption.Encrypt(telegram_api_token_textbox.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }

        //Send Test Notificat
        private void telegram_send_test_notification_button_Click(object sender, EventArgs e)
        {
            try
            {
                WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + telegram_api_token_textbox.Text + "/sendMessage?chat_id=" + telegram_alert_chat_id_textbox.Text + "&text=" + "Test");
                req.UseDefaultCredentials = true;
                var result = req.GetResponse();
                req.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void close_instance_button_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch
                {

                }
            }

            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                }
                catch
                {

                }
            }
        }

        private void max_accounts_count_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.count = Encryption.String_Encryption.Encrypt(max_accounts_count_textbox.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }

        private void auto_bot_username_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.username = Encryption.String_Encryption.Encrypt(auto_bot_username_textbox.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }

        private void auto_bot_password_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.password = Encryption.String_Encryption.Encrypt(auto_bot_password_textbox.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }

        private void adb_mobile_data_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (adb_mobile_data_checkbox.Checked)
            {
                Properties.Settings.Default.adb_mobile_data_enabled = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.adb_mobile_data_enabled = false;
                Properties.Settings.Default.Save();
            }
        }

        private void check_status_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime old_datetime = DateTime.Parse(last_gen_timestamp);

                if ((DateTime.UtcNow - old_datetime).TotalMinutes > 5)
                {
                    try
                    {
                        WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + telegram_api_token_textbox.Text + "/sendMessage?chat_id=" + telegram_alert_chat_id_textbox.Text + "&text=" + "Auto Bot - Account Creator (Astaroth)\n\nAccount creation failed for unknown reasons.");
                        req.UseDefaultCredentials = true;
                        var result = req.GetResponse();
                        req.Abort();
                    }
                    catch (Exception ex)
                    {
                        Logging.Logging.log_error(this.Name, "Send Last Gen outdated.", ex.Message);
                    }

                    close_instance_button.PerformClick();
                    start_instance_button.PerformClick();
                }
            }
            catch (Exception ex)
            {
                Logging.Logging.log_error(this.Name, "Health Check", ex.Message);
            }
        }

        private void settings_gigacube_username_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gigacube_username= Encryption.String_Encryption.Encrypt(settings_gigacube_username_textbox.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }

        private void settings_gigacube_password_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gigacube_password= Encryption.String_Encryption.Encrypt(settings_gigacube_password_textbox.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }

        private void giga_cube_mobile_data_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (giga_cube_mobile_data_checkbox.Checked)
            {
                Properties.Settings.Default.gigacube_mobile_data_enabled = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.gigacube_mobile_data_enabled = false;
                Properties.Settings.Default.Save();
            }
        }

        private void randomize_mail_domain_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (randomize_mail_domain_checkbox.Checked)
            {
                Properties.Settings.Default.mail_domain_randomize = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.mail_domain_randomize = false;
                Properties.Settings.Default.Save();
            }
        }

        private void gender_randomize_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (gender_randomize_checkbox.Checked)
            {
                Properties.Settings.Default.gender_randomize_enabled = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.gender_randomize_enabled = false;
                Properties.Settings.Default.Save();
            }
        }

        private void mail_domain_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.mail_domain = Encryption.String_Encryption.Encrypt(mail_domain_textbox.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }

        private void firefox_profile_path_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.firefox_profile_path = Encryption.String_Encryption.Encrypt(firefox_profile_path.Text, Settings.Encryption_Key);
            Properties.Settings.Default.Save();
        }
    }
}
