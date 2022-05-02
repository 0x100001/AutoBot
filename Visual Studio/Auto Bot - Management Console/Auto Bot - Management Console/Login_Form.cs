using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Auto_Bot___Management_Console;
using System.Security.AccessControl;

namespace Auto_Bot_Management_Console
{
    public partial class Login_Form : Form
    {
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

        private void background_picturebox_MouseDown(object sender, MouseEventArgs e)
        {
            move_window_last_point = new Point(e.X, e.Y);
        }

        private void background_picturebox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - move_window_last_point.X;
                this.Top += e.Y - move_window_last_point.Y;
            }
        }

        private void gui_misc_loader()
        {

        }

        public void Alert(string msg, Helper.Form_Alert.enmType type)
        {
            Helper.Form_Alert frm = new Helper.Form_Alert();
            frm.showAlert(msg, type);
        }

        //######################################GUI END###############################################

        public Login_Form()
        {
            InitializeComponent();
            gui_misc_loader();
        }

        private void get_rental_hostet()
        {
            //Get Mysql Host Information
            WebClient database_request = new WebClient();

            string mysql_host_request;
            string mysql_database_request;
            string mysql_username_request;
            string mysql_password_request;
            string mysql_port_request;

            database_request.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection
            mysql_host_request = database_request.DownloadString(Settings.Database_Provider + "?username=" + username_textbox.Text + "&password=" + password_textbox.Text + "&mh=1");

            byte[] mysql_host_decoded = Convert.FromBase64String(mysql_host_request);
            string mysql_host_decoded_str = Encoding.UTF8.GetString(mysql_host_decoded);

            try
            {
                RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console", true);
                {
                    mysql_host_regkey.SetValue("MySQL_Host", mysql_host_decoded_str, RegistryValueKind.String);
                    mysql_host_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save MySQL Host Information to Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Properties.Settings.Default.MySQL_Host = mysql_host_decoded_str;

            //Get Mysql Database Information
            database_request.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection
            mysql_database_request = database_request.DownloadString(Settings.Database_Provider + "?username=" + username_textbox.Text + "&password=" + password_textbox.Text + "&md=1");

            byte[] mysql_database_decoded = Convert.FromBase64String(mysql_database_request);
            string mysql_database_decoded_str = Encoding.UTF8.GetString(mysql_database_decoded);

            try
            {
                RegistryKey mysql_database_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console", true);
                {
                    mysql_database_regkey.SetValue("MySQL_Database", mysql_database_decoded_str, RegistryValueKind.String);
                    mysql_database_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save MySQL Database Information to Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
            //Properties.Settings.Default.MySQL_Database = mysql_database_decoded_str;

            //Get Mysql Username Information
            database_request.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection
            mysql_username_request = database_request.DownloadString(Settings.Database_Provider + "?username=" + username_textbox.Text + "&password=" + password_textbox.Text + "&mu=1");

            byte[] mysql_username_request_decoded = Convert.FromBase64String(mysql_username_request);
            string mysql_username_request_decoded_str = Encoding.UTF8.GetString(mysql_username_request_decoded);

            try
            {
                RegistryKey mysql_username_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console", true);
                {
                    mysql_username_regkey.SetValue("MySQL_Username", mysql_username_request_decoded_str, RegistryValueKind.String);
                    mysql_username_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save MySQL Username Information to Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
            //Properties.Settings.Default.MySQL_Username = mysql_username_request_decoded_str;

            //Get Mysql Password Information
            database_request.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection
            mysql_password_request = database_request.DownloadString(Settings.Database_Provider + "?username=" + username_textbox.Text + "&password=" + password_textbox.Text + "&mp=1");

            byte[] mysql_password_request_decoded = Convert.FromBase64String(mysql_password_request);
            string mysql_password_request_decoded_str = Encoding.UTF8.GetString(mysql_password_request_decoded);

            try
            {
                RegistryKey mysql_password_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console", true);
                {
                    mysql_password_regkey.SetValue("MySQL_Password", mysql_password_request_decoded_str, RegistryValueKind.String);
                    mysql_password_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save MySQL Password Information to Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Get Mysql Port Information
            database_request.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection
            mysql_port_request = database_request.DownloadString(Settings.Database_Provider + "?username=" + username_textbox.Text + "&password=" + password_textbox.Text + "&mpp=1");

            byte[] mysql_port_request_decoded = Convert.FromBase64String(mysql_port_request);
            string mysql_port_request_decoded_str = Encoding.UTF8.GetString(mysql_port_request_decoded);

            try
            {
                RegistryKey mysql_port_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console", true);
                {
                    mysql_port_regkey.SetValue("MySQL_Port", mysql_port_request_decoded_str, RegistryValueKind.String);
                    mysql_port_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save MySQL Port Information to Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void Login_Form_Load(object sender, EventArgs e)
        {
            //Login Username
            try
            {
                RegistryKey Username_RegKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    username_textbox.Text = StringCipher.Decrypt(Username_RegKey.GetValue("Username").ToString(), Settings.Encryption_Key);
                    Username_RegKey.Close();
                }

                RegistryKey Password_RegKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    password_textbox.Text = StringCipher.Decrypt(Password_RegKey.GetValue("Password").ToString(), Settings.Encryption_Key);
                    Password_RegKey.Close();
                }
            }
            catch (Exception ex)
            {


            }

            this.Hide();

            this.Show();

            username_textbox.Focus();
        }

        private void username_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                login_button.PerformClick();
        }

        private void password_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                login_button.PerformClick();
        }

        private void login_button_Click_1(object sender, EventArgs e)
        {
            try
            {
                WebClient login_request = new WebClient(); // creats a new webclient called wb
                string loginstring; // declares a string called loginstring so we can assign a value later
                login_request.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection

                loginstring = login_request.DownloadString(Settings.Auth + "?username=" + username_textbox.Text + "&password=" + password_textbox.Text); // makes a webrequest using wb for authentication, other parameters are in Settings.cs
                //MessageBox.Show(loginstring);
                //Clipboard.SetText(loginstring);
                if (loginstring.Contains("x8457n84x5n784x5")) //if the password is correct
                {
                    if (loginstring.Contains("x79347xm37489xm3")) //if user is banned
                    {
                        MessageBox.Show("Account banned.", "Login Failed.", MessageBoxButtons.OK, MessageBoxIcon.Error); //banned
                        Application.Exit();
                    }
                    else if (loginstring.Contains("489c7n495784c75n84")) //if the access license expired
                    {
                        MessageBox.Show("Your license expired. Please renew.", "License expired.", MessageBoxButtons.OK, MessageBoxIcon.Error); //banned
                        Application.Exit();
                    }
                    else if (loginstring.Contains("4x67493n6x47836nx47")) //Check if the user has a rental hosting license
                    {
                        try
                        {
                            RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console", true);
                            {
                                mysql_host_regkey.SetValue("Username", StringCipher.Encrypt(username_textbox.Text, Settings.Encryption_Key), RegistryValueKind.String);
                                mysql_host_regkey.SetValue("Password", StringCipher.Encrypt(password_textbox.Text, Settings.Encryption_Key), RegistryValueKind.String);
                                mysql_host_regkey.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Licence Information to Registry: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");
                        }

                        get_rental_hostet();

                        //Check for first run
                        try
                        {
                            RegistryKey winLogonKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console", true);
                            {
                                if (winLogonKey.GetValueNames().Contains("First_Start"))
                                {
                                    var show_main_form = new Main_Form();
                                    show_main_form.Closed += (s, args) => this.Close();
                                    show_main_form.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    var show_setup_assistant_database = new Setup.Setup_Assistant_Database();
                                    show_setup_assistant_database.Closed += (s, args) => this.Close();
                                    show_setup_assistant_database.Show();
                                    this.Hide();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Host Information from Registry: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");
                        }
                    }
                }
                else // if the password is wrong
                {
                    this.Alert("Wrong username or password.", Helper.Form_Alert.enmType.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Authentification Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void refresh_logo_timer_Tick(object sender, EventArgs e)
        {
            logo_picturebox.Refresh();
        }
    }
}
