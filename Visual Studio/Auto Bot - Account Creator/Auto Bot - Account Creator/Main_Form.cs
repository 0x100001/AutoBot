using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using OpenQA.Selenium.Firefox;
using System.Drawing.Imaging;

namespace Auto_Bot___Account_Creator
{
    public partial class Main_Form : Form
    {
        public int ss_count = 0;
        public string ss = "ss_";

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public Main_Form()
        {
            InitializeComponent();
        }

        //Hides Chromedriver
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

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


        //######################################GUI START###############################################

        //GUI Handling
        Point move_window_last_point;

        //Move From if left mouse button clicked on it.
        private void form_header_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - move_window_last_point.X;
                this.Top += e.Y - move_window_last_point.Y;
            }
        }

        //Move Form if header is selected
        private void form_header_MouseDown(object sender, MouseEventArgs e)
        {
            move_window_last_point = new Point(e.X, e.Y);
        }

        private void header_title_label_MouseDown(object sender, MouseEventArgs e)
        {
            move_window_last_point = new Point(e.X, e.Y);
        }

        private void header_title_label_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - move_window_last_point.X;
                this.Top += e.Y - move_window_last_point.Y;
            }
        }

        public void Alert(string msg, Helper.Form_Alert.enmType type)
        {
            Helper.Form_Alert frm = new Helper.Form_Alert();
            frm.showAlert(msg, type);
        }

        private void callbackfunction(IAsyncResult res)
        {

        }

        //######################################GUI END###############################################

        private void account_generator_spotify_start_instance_button_Click(object sender, EventArgs e)
        {
         


            if (account_generator_use_imported_content_checkbox.Checked)
            {
                if (account_generator_spotify_mail_domain_selector_textbox.Text == "" || account_generator_acc_password_textbox.Text == "" || account_generator_use_imported_content_combobox.Items.Count == 0)
                {
                    this.Alert("Please fill all details.", Helper.Form_Alert.enmType.Error);

                    return;
                }
            }

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var nicknameChars = new char[15];
            var randomnickname = new Random();

            for (int i = 0; i < nicknameChars.Length; i++)
            {
                nicknameChars[i] = chars[randomnickname.Next(chars.Length)];
            }

            var finalnickname = new String(nicknameChars);

            var passwordChars = new char[15];
            var randompassword = new Random();

            for (int i = 0; i < passwordChars.Length; i++)
            {
                passwordChars[i] = chars[randompassword.Next(chars.Length)];
            }

            var finalpassword = new String(passwordChars);

            var int_chars = "11";
            var intChars = new char[2];
            var randomint = new Random();

            for (int i = 0; i < intChars.Length; i++)
            {
                intChars[i] = int_chars[randomint.Next(int_chars.Length)];
            }

            var finalint = new String(intChars);

            //Generate Random Displayname int
            var int_chars_display_name = "362468253478236478236576527946376";
            var intChars_display_name = new char[5];
            var randomint_display_name = new Random();

            for (int i = 0; i < intChars_display_name.Length; i++)
            {
                intChars_display_name[i] = int_chars_display_name[randomint.Next(int_chars_display_name.Length)];
            }

            var final_random_display_name_int = new String(intChars_display_name);

            account_generator_spotify_credentials_mail_textbox.Text = finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text;
            account_generator_spotify_credentials_username_textbox.Text = finalnickname;
            account_generator_spotify_credentials_password_textbox.Text = finalpassword;


            int distributor_type = account_generator_distributor_selection_combobox.SelectedIndex;

            ChromeOptions chrome_options = new ChromeOptions();

            chrome_options.AddArguments("--start-maximized");
            chrome_options.AddArguments("--disable-notifications");
            chrome_options.AddArguments("--disable-gpu");
            chrome_options.AddArguments("--incognito");
            chrome_options.AddArguments("--disable-infobars");
            chrome_options.AddArguments("--disable-blink-features=AutomationControlled");
            chrome_options.AddAdditionalCapability("useAutomationExtension", false);
            chrome_options.AddExcludedArgument("enable-automation");

            FirefoxOptions firefox_options = new FirefoxOptions();

            if (browser_mode_combobox.SelectedItem.ToString() == "Chrome")
            {
                IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", chrome_options);

                hide_chromedriver();

                if (account_generator_use_imported_content_checkbox.Checked)
                {
                    if (account_generator_spotify_mail_domain_selector_textbox.Text == "" || account_generator_acc_password_textbox.Text == "")
                    {
                        this.Alert("Please fill all details.", Helper.Form_Alert.enmType.Error);

                        return;
                    }

                    if (distributor_type == 0)
                    {
                        driver.Navigate().GoToUrl("https://www.spotify.com/de");

                        Thread.Sleep(2500);

                        driver.FindElement(By.XPath("//a[contains(@class,'mh-header-secondary mh-tier-2')]")).Click();

                        Thread.Sleep(5000);

                        try
                        {
                            driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
                        }
                        catch
                        {

                        }

                        driver.FindElement(By.Id("email")).SendKeys(account_generator_use_imported_content_combobox.SelectedItem.ToString() + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);

                        driver.FindElement(By.Id("confirm")).SendKeys(account_generator_use_imported_content_combobox.SelectedItem.ToString() + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);
                        driver.FindElement(By.Id("password")).SendKeys(account_generator_acc_password_textbox.Text);

                        driver.FindElement(By.Id("displayname")).SendKeys(account_generator_use_imported_content_combobox.SelectedItem.ToString() + final_random_display_name_int);

                        //Generate Random Year
                        Random random_day_r = new Random();
                        int random_day_int = random_day_r.Next(1, 30);

                        driver.FindElement(By.Id("day")).SendKeys(random_day_int.ToString());
                        driver.FindElement(By.Id("month")).Click();

                        Thread.Sleep(500);

                        //Generate Random Month
                        var random_month_chars = "jfmajasond";
                        var random_month_char_length = new char[1];
                        var random_month_char_rand = new Random();

                        for (int i = 0; i < random_month_char_length.Length; i++)
                        {
                            random_month_char_length[i] = random_month_chars[random_month_char_rand.Next(random_month_chars.Length)];
                        }

                        var random_month_final = new String(random_month_char_length);

                        driver.FindElement(By.Id("month")).SendKeys(random_month_final);

                        //Generate Random Year
                        Random random_year_r = new Random();
                        int random_year_int = random_year_r.Next(1970, 2005);

                        driver.FindElement(By.Id("year")).SendKeys(random_year_int.ToString());

                        if (account_generator_spotify_male_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[1]/label[1]/span[1]")).Click();
                        }
                        else if (account_generator_spotify_female_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[2]/label[1]/span[1]")).Click();
                        }
                        else if (account_generator_spotify_homo_radiobutton.Checked == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[3]/label[1]/span[1]")).Click();
                            }
                            catch
                            {

                            }
                        }

                        try
                        {
                            driver.FindElement(By.XPath("//label[@for='terms-conditions-checkbox']")).Click();
                        }
                        catch
                        {

                        }
                    }

                    this.Alert("Finished filling.", Helper.Form_Alert.enmType.Success);

                    account_generator_use_imported_content_combobox.SelectedIndex++;

                    Properties.Settings.Default.imported_content_index = account_generator_use_imported_content_combobox.SelectedIndex;
                    Properties.Settings.Default.Save();

                    this.Alert("Next Account selected.", Helper.Form_Alert.enmType.Success);
                }
                else
                {
                    if (distributor_type == 0)
                    {
                        driver.Navigate().GoToUrl("https://www.spotify.com/de");

                        driver.FindElement(By.XPath("//a[contains(@class,'mh-header-secondary mh-tier-2')]")).Click();

                        Thread.Sleep(5000);

                        try
                        {
                            driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
                        }
                        catch
                        {

                        }

                        driver.FindElement(By.Id("email")).SendKeys(finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);

                        driver.FindElement(By.Id("confirm")).SendKeys(finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);
                        driver.FindElement(By.Id("password")).SendKeys(account_generator_acc_password_textbox.Text);

                        driver.FindElement(By.Id("displayname")).SendKeys(finalnickname);

                        //Generate Random Year
                        Random random_day_r = new Random();
                        int random_day_int = random_day_r.Next(1, 30);

                        driver.FindElement(By.Id("day")).SendKeys(random_day_int.ToString());
                        driver.FindElement(By.Id("month")).Click();

                        Thread.Sleep(500);

                        //Generate Random Month
                        var random_month_chars = "jfmajasond";
                        var random_month_char_length = new char[1];
                        var random_month_char_rand = new Random();

                        for (int i = 0; i < random_month_char_length.Length; i++)
                        {
                            random_month_char_length[i] = random_month_chars[random_month_char_rand.Next(random_month_chars.Length)];
                        }

                        var random_month_final = new String(random_month_char_length);

                        driver.FindElement(By.Id("month")).SendKeys(random_month_final);

                        //Generate Random Year
                        Random random_year_r = new Random();
                        int random_year_int = random_year_r.Next(1970, 2005);

                        driver.FindElement(By.Id("year")).SendKeys(random_year_int.ToString());

                        if (account_generator_spotify_male_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[1]/label[1]/span[1]")).Click();
                        }
                        else if (account_generator_spotify_female_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[2]/label[1]/span[1]")).Click();
                        }
                        else if (account_generator_spotify_homo_radiobutton.Checked == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[3]/label[1]/span[1]")).Click();
                            }
                            catch
                            {

                            }
                        }

                        try
                        {
                            driver.FindElement(By.XPath("//label[@for='terms-conditions-checkbox']")).Click();
                        }
                        catch
                        {

                        }
                    }
                    else if (distributor_type == 1)
                    {
                        driver.Navigate().GoToUrl("https://www.deezer.com/de/register");

                        Thread.Sleep(2000);

                        try
                        {
                            driver.FindElement(By.ClassName("cookie-btn")).Click();
                        }
                        catch
                        {

                        }

                        driver.FindElement(By.Id("register_form_mail_input")).SendKeys(finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);

                        driver.FindElement(By.Id("register_form_username_input")).SendKeys(finalnickname);
                        driver.FindElement(By.Id("register_form_password_input")).SendKeys(finalpassword);

                        driver.FindElement(By.Id("register_form_gender_input")).Click();

                        driver.FindElement(By.Id("register_form_age_input")).SendKeys("19");

                        if (account_generator_spotify_male_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.Id("register_form_gender_input")).SendKeys("m");
                        }
                        else if (account_generator_spotify_female_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.Id("register_form_gender_input")).SendKeys("w");
                        }
                    }

                }
            }
            else if (browser_mode_combobox.SelectedItem.ToString() == "Firefox")
            {
                IWebDriver driver = new FirefoxDriver(@"C:\Windows\System32\", firefox_options);

                hide_chromedriver();

                if (account_generator_use_imported_content_checkbox.Checked)
                {
                    if (account_generator_spotify_mail_domain_selector_textbox.Text == "" || account_generator_acc_password_textbox.Text == "")
                    {
                        this.Alert("Please fill all details.", Helper.Form_Alert.enmType.Error);

                        return;
                    }

                    if (distributor_type == 0)
                    {
                        driver.Navigate().GoToUrl("https://www.spotify.com/");

                        Thread.Sleep(2500);

                        driver.FindElement(By.XPath("//a[contains(@class,'mh-header-secondary mh-tier-2')]")).Click();

                        Thread.Sleep(5000);

                        try
                        {
                            driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
                        }
                        catch
                        {

                        }

                        driver.FindElement(By.Id("email")).SendKeys(account_generator_use_imported_content_combobox.SelectedItem.ToString() + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);

                        driver.FindElement(By.Id("confirm")).SendKeys(account_generator_use_imported_content_combobox.SelectedItem.ToString() + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);
                        driver.FindElement(By.Id("password")).SendKeys(account_generator_acc_password_textbox.Text);

                        driver.FindElement(By.Id("displayname")).SendKeys(account_generator_use_imported_content_combobox.SelectedItem.ToString() + final_random_display_name_int);

                        //Generate Random Year
                        Random random_day_r = new Random();
                        int random_day_int = random_day_r.Next(1, 30);

                        driver.FindElement(By.Id("day")).SendKeys(random_day_int.ToString());
                        driver.FindElement(By.Id("month")).Click();

                        Thread.Sleep(500);

                        //Generate Random Month
                        var random_month_chars = "jfmajasond";
                        var random_month_char_length = new char[1];
                        var random_month_char_rand = new Random();

                        for (int i = 0; i < random_month_char_length.Length; i++)
                        {
                            random_month_char_length[i] = random_month_chars[random_month_char_rand.Next(random_month_chars.Length)];
                        }

                        var random_month_final = new String(random_month_char_length);

                        driver.FindElement(By.Id("month")).SendKeys(random_month_final);

                        //Generate Random Year
                        Random random_year_r = new Random();
                        int random_year_int = random_year_r.Next(1970, 2005);

                        driver.FindElement(By.Id("year")).SendKeys(random_year_int.ToString());

                        if (account_generator_spotify_male_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[1]/label[1]/span[1]")).Click();
                        }
                        else if (account_generator_spotify_female_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[2]/label[1]/span[1]")).Click();
                        }
                        else if (account_generator_spotify_homo_radiobutton.Checked == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[3]/label[1]/span[1]")).Click();
                            }
                            catch
                            {

                            }
                        }

                        try
                        {
                            driver.FindElement(By.XPath("//label[@for='terms-conditions-checkbox']")).Click();
                        }
                        catch
                        {

                        }
                    }

                    this.Alert("Finished filling.", Helper.Form_Alert.enmType.Success);

                    account_generator_use_imported_content_combobox.SelectedIndex++;

                    Properties.Settings.Default.imported_content_index = account_generator_use_imported_content_combobox.SelectedIndex;
                    Properties.Settings.Default.Save();

                    this.Alert("Next Account selected.", Helper.Form_Alert.enmType.Success);
                }
                else
                {
                    if (distributor_type == 0)
                    {
                        driver.Navigate().GoToUrl("https://www.spotify.com/");

                        Thread.Sleep(2500);

                        driver.FindElement(By.XPath("//a[contains(@class,'mh-header-secondary mh-tier-2')]")).Click();

                        Thread.Sleep(5000);

                        try
                        {
                            driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
                        }
                        catch
                        {

                        }

                        driver.FindElement(By.Id("email")).SendKeys(finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);

                        driver.FindElement(By.Id("confirm")).SendKeys(finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);
                        driver.FindElement(By.Id("password")).SendKeys(finalpassword);

                        driver.FindElement(By.Id("displayname")).SendKeys(finalnickname);

                        //Generate Random Year
                        Random random_day_r = new Random();
                        int random_day_int = random_day_r.Next(1, 30);

                        driver.FindElement(By.Id("day")).SendKeys(random_day_int.ToString());
                        driver.FindElement(By.Id("month")).Click();

                        Thread.Sleep(500);

                        //Generate Random Month
                        var random_month_chars = "jfmajasond";
                        var random_month_char_length = new char[1];
                        var random_month_char_rand = new Random();

                        for (int i = 0; i < random_month_char_length.Length; i++)
                        {
                            random_month_char_length[i] = random_month_chars[random_month_char_rand.Next(random_month_chars.Length)];
                        }

                        var random_month_final = new String(random_month_char_length);

                        driver.FindElement(By.Id("month")).SendKeys(random_month_final);

                        //Generate Random Year
                        Random random_year_r = new Random();
                        int random_year_int = random_year_r.Next(1970, 2005);

                        driver.FindElement(By.Id("year")).SendKeys(random_year_int.ToString());

                        if (account_generator_spotify_male_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[1]/label[1]/span[1]")).Click();
                        }
                        else if (account_generator_spotify_female_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[2]/label[1]/span[1]")).Click();
                        }
                        else if (account_generator_spotify_homo_radiobutton.Checked == true)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[3]/label[1]/span[1]")).Click();
                            }
                            catch
                            {

                            }
                        }

                        try
                        {
                            driver.FindElement(By.XPath("//label[@for='terms-conditions-checkbox']")).Click();
                        }
                        catch
                        {

                        }
                    }
                    else if (distributor_type == 1)
                    {
                        driver.Navigate().GoToUrl("https://www.deezer.com/de/register");

                        Thread.Sleep(2000);

                        try
                        {
                            driver.FindElement(By.ClassName("cookie-btn")).Click();
                        }
                        catch
                        {

                        }

                        driver.FindElement(By.Id("register_form_mail_input")).SendKeys(finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);

                        driver.FindElement(By.Id("register_form_username_input")).SendKeys(finalnickname);
                        driver.FindElement(By.Id("register_form_password_input")).SendKeys(finalpassword);

                        driver.FindElement(By.Id("register_form_gender_input")).Click();

                        driver.FindElement(By.Id("register_form_age_input")).SendKeys("19");

                        if (account_generator_spotify_male_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.Id("register_form_gender_input")).SendKeys("m");
                        }
                        else if (account_generator_spotify_female_radiobutton.Checked == true)
                        {
                            driver.FindElement(By.Id("register_form_gender_input")).SendKeys("w");
                        }
                    }

                }
            }


            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }

        private void account_generator_spotify_close_instance_button_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcessesByName("firefox"))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcessesByName("geckodriver"))
            {
                process.Kill();
            }
        }

        private void account_generator_spotify_credentials_save_button_Click(object sender, EventArgs e)
        {
            if (account_generator_distributor_selection_combobox.SelectedIndex == 0)
            {
                if (account_generator_use_imported_content_checkbox.Checked)
                {
                    File.AppendAllText(Application.StartupPath + @"\accounts\spotify_accounts.log", account_generator_use_imported_content_combobox.SelectedItem.ToString() + "@" + account_generator_spotify_mail_domain_selector_textbox .Text + Environment.NewLine);
                    this.Alert("Saved credentials.", Helper.Form_Alert.enmType.Success);
                }
                else
                {
                    File.AppendAllText(Application.StartupPath + @"\accounts\spotify_accounts.log", account_generator_spotify_credentials_mail_textbox.Text + Environment.NewLine);
                    this.Alert("Saved credentials.", Helper.Form_Alert.enmType.Success);
                }
            }
            else
            {
                File.AppendAllText(Application.StartupPath + @"\accounts\deezer_accounts.log", account_generator_spotify_credentials_mail_textbox.Text + Environment.NewLine);
                this.Alert("Saved credentials.", Helper.Form_Alert.enmType.Success);
            }
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(Application.StartupPath + @"\accounts") == false)
            {
                Directory.CreateDirectory(Application.StartupPath + @"\accounts");
            }

            account_generator_acc_password_textbox.Text = Properties.Settings.Default.account_password;
            account_generator_spotify_mail_domain_selector_textbox.Text = Properties.Settings.Default.mail_domain;
        }

        private void account_generator_import_content_button_Click(object sender, EventArgs e)
        {
            try
            {
                string file_path;

                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Downloads";
                openFileDialog.Filter = "Text File with Words (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file_path = openFileDialog.FileName;

                    int lineCount = File.ReadLines(file_path).Count();

                    foreach (string line in File.ReadLines(file_path))
                    {
                        if (account_generator_use_imported_content_combobox.Items.Contains(line) == false)
                        {
                            account_generator_use_imported_content_combobox.Items.Add(line);
                        }
                        else
                        {
                            File.AppendAllText(Application.StartupPath + @"\account_generator\cpanel_mail_accounts_not_imported.log", line + Environment.NewLine);
                        }
                    }

                    account_generator_use_imported_content_combobox.SelectedIndex = 0;

                    int imported_count = account_generator_use_imported_content_combobox.Items.Count;

                    this.Alert("Imported " + imported_count + " of " + lineCount + ".", Helper.Form_Alert.enmType.Success);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Import File Content (cPanel): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void account_generator_acc_password_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.account_password = account_generator_acc_password_textbox.Text;
            Properties.Settings.Default.Save();
        }

        private void account_generator_import_content_load_index_button_Click(object sender, EventArgs e)
        {
            if (account_generator_use_imported_content_combobox.Items.Count != 0)
            {
                account_generator_use_imported_content_combobox.SelectedIndex = Properties.Settings.Default.imported_content_index;
                this.Alert("Index loaded.", Helper.Form_Alert.enmType.Success);
            }
            else
            {
                this.Alert("Please import content.", Helper.Form_Alert.enmType.Error);
            }
        }

        private void account_generator_spotify_mail_domain_selector_textbox_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.mail_domain = account_generator_spotify_mail_domain_selector_textbox.Text;
            Properties.Settings.Default.Save();
        }

        private void account_generator_credentials_open_log_folder_button_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath + @"\accounts");
        }

        private void start_auto_mode_button_Click(object sender, EventArgs e)
        {
            if (account_generator_use_imported_content_checkbox.Checked)
            {
                if (account_generator_spotify_mail_domain_selector_textbox.Text == "" || account_generator_acc_password_textbox.Text == "" || account_generator_use_imported_content_combobox.Items.Count == 0)
                {
                    this.Alert("Please fill all details.", Helper.Form_Alert.enmType.Error);

                    return;
                }
            }

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var nicknameChars = new char[15];
            var randomnickname = new Random();

            for (int i = 0; i < nicknameChars.Length; i++)
            {
                nicknameChars[i] = chars[randomnickname.Next(chars.Length)];
            }

            var finalnickname = new String(nicknameChars);

            var passwordChars = new char[15];
            var randompassword = new Random();

            for (int i = 0; i < passwordChars.Length; i++)
            {
                passwordChars[i] = chars[randompassword.Next(chars.Length)];
            }

            var finalpassword = new String(passwordChars);

            var int_chars = "11";
            var intChars = new char[2];
            var randomint = new Random();

            for (int i = 0; i < intChars.Length; i++)
            {
                intChars[i] = int_chars[randomint.Next(int_chars.Length)];
            }

            var finalint = new String(intChars);

            //Generate Random Displayname int
            var int_chars_display_name = "362468253478236478236576527946376";
            var intChars_display_name = new char[5];
            var randomint_display_name = new Random();

            for (int i = 0; i < intChars_display_name.Length; i++)
            {
                intChars_display_name[i] = int_chars_display_name[randomint.Next(int_chars_display_name.Length)];
            }

            var final_random_display_name_int = new String(intChars_display_name);

            account_generator_spotify_credentials_mail_textbox.Text = finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text;
            account_generator_spotify_credentials_username_textbox.Text = finalnickname;
            account_generator_spotify_credentials_password_textbox.Text = finalpassword;

            ChromeOptions chrome_options = new ChromeOptions();

            chrome_options.AddArguments("--start-maximized");
            chrome_options.AddArguments("--disable-notifications");
            chrome_options.AddArguments("--disable-gpu");
            chrome_options.AddArguments("--incognito");
            chrome_options.AddArguments("--disable-infobars");
            chrome_options.AddArguments("--disable-blink-features=AutomationControlled");
            chrome_options.AddAdditionalCapability("useAutomationExtension", false);
            chrome_options.AddArguments("user-data-dir=C:\\Chrome_Profiles\\autobot");

            chrome_options.AddExcludedArgument("enable-automation");

            IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", chrome_options);

            hide_chromedriver();
            
            driver.Navigate().GoToUrl("https://www.spotify.com/de");

            driver.FindElement(By.XPath("//a[contains(@class,'mh-header-secondary mh-tier-2')]")).Click();

            Thread.Sleep(5000);

            try
            {
                driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
            }
            catch
            {

            }

            driver.FindElement(By.Id("email")).SendKeys(finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);

            driver.FindElement(By.Id("confirm")).SendKeys(finalnickname + "@" + account_generator_spotify_mail_domain_selector_textbox.Text);
            driver.FindElement(By.Id("password")).SendKeys(account_generator_acc_password_textbox.Text);

            driver.FindElement(By.Id("displayname")).SendKeys(finalnickname);

            //Generate Random Year
            Random random_day_r = new Random();
            int random_day_int = random_day_r.Next(1, 30);

            driver.FindElement(By.Id("day")).SendKeys(random_day_int.ToString());
            driver.FindElement(By.Id("month")).Click();

            Thread.Sleep(500);

            //Generate Random Month
            var random_month_chars = "jfmajasond";
            var random_month_char_length = new char[1];
            var random_month_char_rand = new Random();

            for (int i = 0; i < random_month_char_length.Length; i++)
            {
                random_month_char_length[i] = random_month_chars[random_month_char_rand.Next(random_month_chars.Length)];
            }

            var random_month_final = new String(random_month_char_length);

            driver.FindElement(By.Id("month")).SendKeys(random_month_final);

            //Generate Random Year
            Random random_year_r = new Random();
            int random_year_int = random_year_r.Next(1970, 2005);

            driver.FindElement(By.Id("year")).SendKeys(random_year_int.ToString());

            if (account_generator_spotify_male_radiobutton.Checked == true)
            {
                driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[1]/label[1]/span[1]")).Click();
            }
            else if (account_generator_spotify_female_radiobutton.Checked == true)
            {
                driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[2]/label[1]/span[1]")).Click();
            }
            else if (account_generator_spotify_homo_radiobutton.Checked == true)
            {
                try
                {
                    driver.FindElement(By.XPath("//div[@id='__next']/main[1]/div[2]/div[1]/form[1]/fieldset[1]/div[1]/div[3]/label[1]/span[1]")).Click();
                }
                catch
                {

                }
            }

            try
            {
                driver.FindElement(By.XPath("//label[@for='terms-conditions-checkbox']")).Click();
            }
            catch
            {

            }

            //Click Recaptcha Login
            Bitmap google_recaptcha = new Bitmap(Application.StartupPath + @"\ss_patterns\google_recaptcha.png");
            bool google_recaptcha_flag = true;
            int google_recaptcha_count = 0;
            int google_recaptcha_maxTries = 10;
            while (google_recaptcha_flag == true)
            {
                try
                {
                    //Take Screenshot
                    take_ss();

                    Rectangle pp_rect = FindImageOnScreen(google_recaptcha, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

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

                        Thread.Sleep(5000);
                    }
                }
                catch
                {

                }
            }

            //Click Buster Logo
            Bitmap buster_item = new Bitmap(Application.StartupPath + @"\ss_patterns\buster_pattern.png");
            bool buster_item_flag = true;
            int buster_item_count = 0;
            int buster_item_maxTries = 10;
            while (buster_item_flag == true)
            {
                try
                {
                    //Take Screenshot
                    take_ss();

                    Rectangle pp_rect = FindImageOnScreen(buster_item, false);

                    if (pp_rect != Rectangle.Empty)
                    {
                        SimulateMove(pp_rect.X, pp_rect.Y, 250); //Simulate Moving

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
                catch
                {

                }
            }

            Thread.Sleep(5000);
            
            try
            {
                driver.FindElement(By.XPath("//div[text()='Registrieren']")).Click();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Thread.Sleep(10000);

            try
            {
                driver.FindElement(By.XPath("//h1[text()='Spotify herunterladen']")).Click();
                MessageBox.Show("done");
            }
            catch
            {

            }
        }

        private Rectangle FindImageOnScreen(Bitmap bmpMatch, bool ExactMatch)
        {
            Bitmap ScreenBmp = new Bitmap(Application.StartupPath + @"\ss\" + ss + ".png");

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
                bmp.Save(Application.StartupPath + @"\ss\" + ss + ".png", ImageFormat.Png);
                bmp.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ss.ToString() + " take SS");
            }
        }
    }
}