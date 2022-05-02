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
using System.Resources;
using System.Reflection;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using Microsoft.Win32;
using Auto_Bot_Management_Console;

namespace Auto_Bot_Management_Console.Setup
{
    public partial class Setup_Assistant_Database : Form
    {
        string username = "";
        string password = "";

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

        public void Alert(string msg, Helper.Form_Alert.enmType type)
        {
            Helper.Form_Alert frm = new Helper.Form_Alert();
            frm.showAlert(msg, type);
        }

        //######################################GUI END###############################################

        public Setup_Assistant_Database()
        {
            InitializeComponent();
        }

        private void Setup_Assistant_Database_Load(object sender, EventArgs e)
        {
            //Read MySQL Host Regkey
            try
            {
                RegistryKey MySQL_Host_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    database_configuration_mysql_host_textbox.Text = StringCipher.Decrypt(MySQL_Host_Regkey.GetValue("MySQL_Host").ToString(), Settings.Encryption_Key);
                    MySQL_Host_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Host Information from Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Read MySQL Host Regkey
            try
            {
                RegistryKey MySQL_Host_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    database_configuration_mysql_host_textbox.Text = StringCipher.Decrypt(MySQL_Host_Regkey.GetValue("MySQL_Host").ToString(), Settings.Encryption_Key);
                    MySQL_Host_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Host Information from Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Read MySQL Database Regkey
            try
            {
                RegistryKey MySQL_Database_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    database_configuration_mysql_database_textbox.Text = StringCipher.Decrypt(MySQL_Database_Regkey.GetValue("MySQL_Database").ToString(), Settings.Encryption_Key);
                    MySQL_Database_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Database Information from Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

            }

            //Read MySQL Username Regkey
            try
            {
                RegistryKey MySQL_Username_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    database_configuration_mysql_username_textbox.Text = StringCipher.Decrypt(MySQL_Username_Regkey.GetValue("MySQL_Username").ToString(), Settings.Encryption_Key);
                    MySQL_Username_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Username Information from Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Read MySQL Password Regkey
            try
            {
                RegistryKey MySQL_Password_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    database_configuration_mysql_password_textbox.Text = StringCipher.Decrypt(MySQL_Password_Regkey.GetValue("MySQL_Password").ToString(), Settings.Encryption_Key);
                    MySQL_Password_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Password Information from Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Read MySQL Port  Regkey
            try
            {
                RegistryKey MySQL_Port_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    database_configuration_mysql_port_textbox.Text = StringCipher.Decrypt(MySQL_Port_Regkey.GetValue("MySQL_Port").ToString(), Settings.Encryption_Key);
                    MySQL_Port_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read MySQL Port Information from Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Read Username
            try
            {
                RegistryKey Username_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    username = StringCipher.Decrypt(Username_Regkey.GetValue("Username").ToString(), Settings.Encryption_Key);
                    Username_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Username Information from Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Read Password
            try
            {
                RegistryKey Password_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
                {
                    password = StringCipher.Decrypt(Password_Regkey.GetValue("Password").ToString(), Settings.Encryption_Key);
                    Password_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Username Information from Registry: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            database_configuration_mysql_host_textbox.Enabled = false;
            database_configuration_mysql_database_textbox.Enabled = false;
            database_configuration_mysql_username_textbox.Enabled = false;
            database_configuration_mysql_password_textbox.Enabled = false;
            database_configuration_mysql_port_textbox.Enabled = false;
        }

        //Check Database Connection
        private void database_configuration_check_connection_button_Click(object sender, EventArgs e)
        {
            string connStr = "server=" + database_configuration_mysql_host_textbox.Text + ";user=" + database_configuration_mysql_username_textbox.Text + ";database=" + database_configuration_mysql_database_textbox.Text + ";port=" + database_configuration_mysql_port_textbox.Text + ";password=" + database_configuration_mysql_password_textbox.Text + ";Pooling=false;default command timeout=360";
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string store_mail_recipient_sql = "SELECT version();";
                MySqlCommand store_mail_recipient_cmd = new MySqlCommand(store_mail_recipient_sql, conn);
                store_mail_recipient_cmd.ExecuteNonQuery();

                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Test MySQL Connection: " + "Connection successfully tested." + Environment.NewLine);

                this.Alert("Connection test passed.", Helper.Form_Alert.enmType.Success);

                database_configuration_continue_button.Enabled = true;
                database_configuration_continue_button.FillColor = Color.FromArgb(0, 215, 0);
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Test MySQL Connection: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void database_configuration_continue_button_Click(object sender, EventArgs e)
        {
            //Download Chrome Driver
            try
            {
                WebClient wb = new WebClient(); // creats a new webclient called wb
                string download_chromedriver;

                wb.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection
                download_chromedriver = wb.DownloadString(Settings.Driver_Provider + "?username=" + username + "&password=" + password + "&driver=zfg73x783x7n36n746x3"); // makes a webrequest using wb for authentication, other parameters are in Settings.cs

                Byte[] chromedriver_decoded = Convert.FromBase64String(download_chromedriver);
                File.WriteAllBytes(@"C:\Windows\System32\chromedriver.exe", chromedriver_decoded);

                this.Alert("Driver installed successfully.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Install Driver: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Disable First Start
            try
            {
                RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console", true);
                {
                    mysql_host_regkey.SetValue("First_Start", 0, RegistryValueKind.DWord);
                    mysql_host_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Disable First Start: " + ex.Message + Environment.NewLine);
            }

            var show_notification_settings_form = new Main_Form();
            show_notification_settings_form.Closed += (s, args) => this.Close();
            show_notification_settings_form.Show();
            this.Hide();
        }
    }
}
