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
using Microsoft.Win32.TaskScheduler;

namespace Auto_Bot___Client.Setup
{
    public partial class Setup_Assistant : Form
    {
        string connStr;

        public string mysql_host;
        public string mysql_database;
        public string mysql_username;
        public string mysql_password;
        public string mysql_port;

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

        //######################################GUI END###############################################

        public Setup_Assistant()
        {
            InitializeComponent();
            database_information_loader();
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

                Process.Start(Application.StartupPath + @"\logs\error.log");
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

                Process.Start(Application.StartupPath + @"\logs\error.log");
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

                Process.Start(Application.StartupPath + @"\logs\error.log");
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

                Process.Start(Application.StartupPath + @"\logs\error.log");
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

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Init Connection String
            connStr = "server=" + mysql_host + ";user=" + mysql_username + ";database=" + mysql_database + ";port=" + mysql_port + ";password=" + mysql_password + ";Pooling=false;default command timeout=360;";
        }

        private void database_configuration_continue_button_Click(object sender, EventArgs e)
        {
            //Add Client to Database
            if (client_already_existing_checkbox.Checked == false)
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();

                    string sql = "INSERT INTO `" + mysql_database + "`.`clients` (`client_name`, `hive_name`) VALUES ('" + client_name_textbox.Text + "', '" + hive_name_combobox.SelectedItem.ToString() + "');";
                    MySqlCommand cmd= new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Client to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }
            }

            //Save Client Name
            try
            {
                RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_host_regkey.SetValue("Client_Name", client_name_textbox.Text, RegistryValueKind.String);
                    mysql_host_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Client Name: " + ex.Message + Environment.NewLine);
            }

            //Save Hive Name
            try
            {
                RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_host_regkey.SetValue("Hive_Name", hive_name_combobox.SelectedItem.ToString(), RegistryValueKind.String);
                    mysql_host_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Hive Name: " + ex.Message + Environment.NewLine);
            }

            //Enable Windows AutoLogOn
            try
            {
                using (var enable_windows_autologon_64view = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                        RegistryView.Registry64))
                {
                    using (var set_windows_autologon_keys = enable_windows_autologon_64view.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", true))
                    {
                        set_windows_autologon_keys.SetValue("AutoAdminLogon", "1", RegistryValueKind.String);
                        set_windows_autologon_keys.SetValue("DefaultUserName", Environment.UserName, RegistryValueKind.String);
                        set_windows_autologon_keys.SetValue("DefaultPassword", "Hurensohn123!", RegistryValueKind.String);
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Enable Windows Auto Logon: " + ex.Message + Environment.NewLine);
            }

            //Add Task to Auto Start
            try
            {
                using (TaskService service = new TaskService())
                {
                    if (!service.RootFolder.AllTasks.Any(t => t.Name == "Auto_Bot_Client_Starter"))
                    {
                        var task = service.NewTask();
                        task.RegistrationInfo.Description = "Starts the Auto Bot Client.";
                        task.RegistrationInfo.Author = "Auto Bot";

                        var LogonTrigger = new LogonTrigger();
                        task.Triggers.Add(LogonTrigger);

                        var taskExecutablePath = @"C:\Auto_Bot_Starter.exe";
                        task.Actions.Add(new ExecAction(taskExecutablePath));

                        service.RootFolder.RegisterTaskDefinition("Auto_Bot_Client_Starter", task);
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Enable Windows Auto Start: " + ex.Message + Environment.NewLine);
            }

            //Disable First Start
            try
            {
                RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_host_regkey.SetValue("First_Start", 0, RegistryValueKind.DWord);
                    mysql_host_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Disable First Start: " + ex.Message + Environment.NewLine);
            }

            MessageBox.Show("Setup finished. Press OK to continue.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var show_notification_settings_form = new Auto_Bot___Client.Main_Form();
            show_notification_settings_form.Closed += (s, args) => this.Close();
            show_notification_settings_form.Show();
            this.Hide();
        }

        private void Setup_Assistant_Load(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            string sql = "SELECT hive_name FROM `" + mysql_database + "`.`hives`;";

            try
            {
                conn.Open();

                //Load Hives
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            hive_name_combobox.Items.Add(rdr[0].ToString());
                        }
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Clients from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");
                }

                conn.Close();

                try
                {
                    hive_name_combobox.SelectedIndex = 0;
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Open MySQL Connection: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }
    }
}
