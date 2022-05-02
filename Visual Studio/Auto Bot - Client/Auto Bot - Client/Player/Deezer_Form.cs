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

namespace Auto_Bot___Client.Player
{
    public partial class Deezer_Form : Form
    {
        string connStr;

        string mysql_host;
        string mysql_database;
        string mysql_username;
        string mysql_password;
        string mysql_port;

        public int Clicks;

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

        public static class StringCipher
        {
            // This constant is used to determine the keysize of the encryption algorithm in bits.
            // We divide this by 8 within the code below to get the equivalent number of bytes.
            private const int Keysize = 256;

            // This constant determines the number of iterations for the password bytes generation function.
            private const int DerivationIterations = 1000;

            public static string Encrypt(string plainText, string passPhrase)
            {
                // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
                // so that the same Salt and IV values can be used when decrypting.  
                var saltStringBytes = Generate256BitsOfRandomEntropy();
                var ivStringBytes = Generate256BitsOfRandomEntropy();
                var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                    cryptoStream.FlushFinalBlock();
                                    // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                    var cipherTextBytes = saltStringBytes;
                                    cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                    cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Convert.ToBase64String(cipherTextBytes);
                                }
                            }
                        }
                    }
                }
            }

            public static string Decrypt(string cipherText, string passPhrase)
            {
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
                {
                    var keyBytes = password.GetBytes(Keysize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }

            private static byte[] Generate256BitsOfRandomEntropy()
            {
                var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
                using (var rngCsp = new RNGCryptoServiceProvider())
                {
                    // Fill the array with cryptographically secure random bytes.
                    rngCsp.GetBytes(randomBytes);
                }
                return randomBytes;
            }
        }

        private void callbackfunction(IAsyncResult res)
        {

        }

        public Deezer_Form()
        {
            InitializeComponent();
            database_information_loader();
        }

        private void kill_everything_perform_restart()
        {
            if (Process.GetProcessesByName("Auto_Bot_Starter").Length != 0)
            {
                Process.Start(@"C:\Auto_Bot_Starter.exe");
            }
            else
            {
                foreach (var process in Process.GetProcessesByName("Auto_Bot_Starter"))
                {
                    process.Kill();
                    process.WaitForExit();
                }

                Process.Start(@"C:\Auto_Bot_Starter.exe");
            }

            Application.Exit();
        }

        private void shutdown_system()
        {
            var shutdown_action = new ProcessStartInfo("shutdown", "/s /t 0");
            shutdown_action.CreateNoWindow = true;
            shutdown_action.UseShellExecute = false;
            Process.Start(shutdown_action);
        }

        //Load Database Information from Registry
        public void database_information_loader()
        {
            //Read MySQL Host Regkey
            try
            {
                RegistryKey MySQL_Host_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_host = StringCipher.Decrypt(MySQL_Host_Regkey.GetValue("MySQL_Host").ToString(), Settings.Encryption_Key);
                    MySQL_Host_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Host Information from Registry: " + ex.Message + Environment.NewLine);
            }

            //Read MySQL Database Regkey
            try
            {
                RegistryKey MySQL_Database_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_database = StringCipher.Decrypt(MySQL_Database_Regkey.GetValue("MySQL_Database").ToString(), Settings.Encryption_Key);
                    MySQL_Database_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Database Information from Registry: " + ex.Message + Environment.NewLine);
            }

            //Read MySQL Username Regkey
            try
            {
                RegistryKey MySQL_Username_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_username = StringCipher.Decrypt(MySQL_Username_Regkey.GetValue("MySQL_Username").ToString(), Settings.Encryption_Key);
                    MySQL_Username_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Username Information from Registry: " + ex.Message + Environment.NewLine);
            }

            //Read MySQL Password Regkey
            try
            {
                RegistryKey MySQL_Password_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_password = StringCipher.Decrypt(MySQL_Password_Regkey.GetValue("MySQL_Password").ToString(), Settings.Encryption_Key);
                    MySQL_Password_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Password Information from Registry: " + ex.Message + Environment.NewLine);
            }

            //Read MySQL Port  Regkey
            try
            {
                RegistryKey MySQL_Port_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_port = StringCipher.Decrypt(MySQL_Port_Regkey.GetValue("MySQL_Port").ToString(), Settings.Encryption_Key);
                    MySQL_Port_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Port Information from Registry: " + ex.Message + Environment.NewLine);
            }

            //Init Connection String
            connStr = "server=" + mysql_host + ";user=" + mysql_username + ";database=" + mysql_database + ";port=" + mysql_port + ";password=" + mysql_password + ";Pooling=false;default command timeout=360;";
        }

        private void Deezer_Form_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(player_instance));
        }

        private void player_instance()
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            MethodInvoker mk = delegate
            {
                ChromeOptions options = new ChromeOptions();

                options.AddArguments("--start-maximized");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--incognito");
                options.AddArguments("--disable-infobars");
                options.AddArguments("--disable-blink-features=AutomationControlled");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddExcludedArgument("enable-automation");

                IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                //hide_chromedriver();

                if (Main_Form_Init.openvpn_health_check_enabled == 1)
                {
                    driver.Navigate().GoToUrl("https://checkip.perfect-privacy.com/");

                    bool check_pp_status_bool = true;
                    int check_pp_status_count = 0;
                    int check_pp_status_maxTries = 25;
                    while (check_pp_status_bool == true)
                    {
                        try
                        {
                            driver.FindElement(By.XPath("//img[@alt='VPN enabled']"));

                            check_pp_status_bool = false;
                        }
                        catch
                        {
                            // handle exception
                            if (++check_pp_status_count == check_pp_status_maxTries)
                            {
                                if (Main_Form_Init.Telegram_Monitoring == 1)
                                {
                                    WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + Main_Form_Init.Telegram_API_Token + "/sendMessage?chat_id=" + Main_Form_Init.Telegram_Alert_Chat_Id + "&text=" + "Client Name: " + Main_Form_Init.client_name + "\nPerfect Privacy VPN not connected correctly, or configuration is wrong. Restarting the bot now.");
                                    req.UseDefaultCredentials = true;
                                    var result = req.GetResponse();
                                    Thread.Sleep(1000);
                                    req.Abort();
                                }

                                Misc.kill_everything_perform_restart();
                            }

                            Thread.Sleep(1000);
                        }
                    }

                }

                driver.Navigate().GoToUrl("https://www.deezer.com/de/login");

                bool cookie_login_flag = true;
                int cookie_login_registercount = 0;
                int cookie_login_registermaxTries = 20;
                while (cookie_login_flag == true)
                {
                    try
                    {
                        driver.FindElement(By.ClassName("cookie-btn")).Click();

                        cookie_login_flag = false;
                    }
                    catch
                    {
                        // handle exception
                        if (++cookie_login_registercount == cookie_login_registermaxTries)
                        {
                            cookie_login_flag = false;
                        }

                        Thread.Sleep(1000);
                    }
                }

                Thread.Sleep(5000);

                /*bool start_login_flag = true;
                int start_login_registercount = 0;
                int start_login_registermaxTries = 90;
                while (start_login_flag == true)
                {
                    try
                    {
                        driver.FindElement(By.Id("login_mail")).SendKeys(Main_Form_Init.deezer_player_username);
                        driver.FindElement(By.Id("login_password")).SendKeys(Main_Form_Init.deezer_player_password);

                        driver.FindElement(By.Id("login_form_submit")).Click();
                        start_login_flag = false;
                    }
                    catch
                    {
                        // handle exception
                        if (++start_login_registercount == start_login_registermaxTries)
                        {
                            if (Main_Form_Init.Telegram_Monitoring == 1)
                            {
                                WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + Main_Form_Init.Telegram_API_Token + "/sendMessage?chat_id=" + Main_Form_Init.Telegram_Alert_Chat_Id + "&text=" + "Player: Deezer\nClient Name: " + Main_Form_Init.client_name + "\nLogin could not be started. Restarting the Bot now.");
                                req.UseDefaultCredentials = true;
                                var result = req.GetResponse();
                                Thread.Sleep(1000);
                                req.Abort();
                            }

                            Misc.kill_everything_perform_restart();
                        }

                        Thread.Sleep(1000);
                    }
                }*/

                //Thread.Sleep(60000);

                driver.Navigate().GoToUrl(Main_Form_Init.deezer_playlist_url);

                Thread.Sleep(2500);

                try
                {
                    driver.FindElement(By.XPath("//span[text()='Anhören']")).Click();
                }
                catch
                {

                }

                if (Main_Form_Init.deezer_autoskip_enabled == 1)
                {
                    while (Main_Form_Init.deezer_autoskip_enabled == 1)
                    {
                        int index = 1;

                        try
                        {
                            driver.FindElement(By.XPath("//span[text()='EMPFEHLUNG']"));

                            driver.Navigate().GoToUrl(Main_Form_Init.deezer_playlist_url);

                            Thread.Sleep(5000);

                            try
                            {
                                driver.FindElement(By.XPath("//span[text()='Anhören']")).Click();
                            }
                            catch
                            {

                            }
                        }
                        catch
                        {

                        }

                        try
                        {
                            driver.FindElement(By.Id("modal-close")).Click();
                        }
                        catch
                        {

                        }

                        try
                        {
                            driver.FindElement(By.XPath("//span[text()='Anhören']")).Click();
                        }
                        catch
                        {

                        }

                        try
                        {
                            driver.FindElement(By.XPath("//span[text()='Fortsetzen']")).Click();
                        }
                        catch
                        {

                        }

                        try
                        {
                            driver.FindElement(By.XPath("//div[text()='00:00']")).Click();
                            driver.FindElement(By.XPath("(//span[@itemprop='name'])[" + index + "]']")).Click();
                            driver.FindElement(By.XPath("(//span[@itemprop='name'])[" + index + "]']")).Click();

                        }
                        catch
                        {

                        }

                        try
                        {
                            TimeSpan timeSpan2 = new TimeSpan(2, 14, 18);
                            Random r = new Random();
                            int rInt = r.Next(Main_Form_Init.deezer_autoskip_forward_from, Main_Form_Init.deezer_autoskip_forward_to);
                            Thread.Sleep(rInt);
                            //driver.FindElement(By.XPath("//div[@id='page_player']/div[1]/div[1]/ul[1]/li[5]/div[1]/button[1]")).Click();

                            Clicks = Clicks + 1;
                            index = index + 1;
                        }
                        catch
                        {

                        }
                    }
                }
            };
            mk.BeginInvoke(callbackfunction, null);
        }
    }
}
