using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Diagnostics;
using Auto_Bot_Management_Console;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Html5;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Management;
using NAudio.Wave;
using Microsoft.VisualBasic;
using System.Globalization;
using Renci.SshNet;
using Renci.SshNet.Common;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support;


namespace Auto_Bot_Management_Console
{
    public partial class Main_Form : Form
    {
        string connStr;

        public string mysql_host;
        public string mysql_database;
        public string mysql_username;
        public string mysql_password;
        public string mysql_port;

        int artist_index;

        string cookies_file_encoded;

        public int ss_count = 0;
        public string ss = "ss_";
        public int debug_ss_count = 0;
        public string debug_ss = "ss_";

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

        private void background_panel_MouseDown(object sender, MouseEventArgs e)
        {
            move_window_last_point = new Point(e.X, e.Y);
        }

        private void background_panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - move_window_last_point.X;
                this.Top += e.Y - move_window_last_point.Y;
            }
        }

        private void header_form_always_top_button_Click(object sender, EventArgs e)
        {
            if (this.TopMost == false)
            {
                this.TopMost = true;

                this.Alert("Top Most enabled.", Helper.Form_Alert.enmType.Success);
            }
            else
            {
                this.TopMost = false;

                this.Alert("Top Most disabled.", Helper.Form_Alert.enmType.Success);
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
        //######################################MAC Spoofer Start#############################################

        public class Adapter
        {
            public ManagementObject adapter;
            public string adaptername;
            public string customname;
            public int devnum;

            public Adapter(ManagementObject a, string aname, string cname, int n)
            {
                this.adapter = a;
                this.adaptername = aname;
                this.customname = cname;
                this.devnum = n;
            }

            public Adapter(NetworkInterface i) : this(i.Description) { }

            public Adapter(string aname)
            {
                this.adaptername = aname;

                var searcher = new ManagementObjectSearcher("select * from win32_networkadapter where Name='" + adaptername + "'");
                var found = searcher.Get();
                this.adapter = found.Cast<ManagementObject>().FirstOrDefault();

                // Extract adapter number; this should correspond to the keys under
                // HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}
                try
                {
                    var match = Regex.Match(adapter.Path.RelativePath, "\\\"(\\d+)\\\"$");
                    this.devnum = int.Parse(match.Groups[1].Value);
                }
                catch
                {
                    return;
                }

                // Find the name the user gave to it in "Network Adapters"
                this.customname = NetworkInterface.GetAllNetworkInterfaces().Where(
                    i => i.Description == adaptername
                ).Select(
                    i => " (" + i.Name + ")"
                ).FirstOrDefault();
            }

            /// <summary>
            /// Get the .NET managed adapter.
            /// </summary>
            public NetworkInterface ManagedAdapter
            {
                get
                {
                    return NetworkInterface.GetAllNetworkInterfaces().Where(
                        nic => nic.Description == this.adaptername
                    ).FirstOrDefault();
                }
            }

            /// <summary>
            /// Get the MAC address as reported by the adapter.
            /// </summary>
            public string Mac
            {
                get
                {
                    try
                    {
                        return BitConverter.ToString(this.ManagedAdapter.GetPhysicalAddress().GetAddressBytes()).Replace("-", "").ToUpper();
                    }
                    catch { return null; }
                }
            }

            /// <summary>
            /// Get the registry key associated to this adapter.
            /// </summary>
            public string RegistryKey
            {
                get
                {
                    return String.Format(@"SYSTEM\ControlSet001\Control\Class\{{4D36E972-E325-11CE-BFC1-08002BE10318}}\{0:D4}", this.devnum);
                }
            }

            /// <summary>
            /// Get the NetworkAddress registry value of this adapter.
            /// </summary>
            public string RegistryMac
            {
                get
                {
                    try
                    {
                        using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(this.RegistryKey, false))
                        {
                            return regkey.GetValue("NetworkAddress").ToString();
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            /// Sets the NetworkAddress registry value of this adapter.
            /// </summary>
            /// <param name="value">The value. Should be EITHER a string of 12 hexadecimal digits, uppercase, without dashes, dots or anything else, OR an empty string (clears the registry value).</param>
            /// <returns>true if successful, false otherwise</returns>
            public bool SetRegistryMac(string value)
            {
                bool shouldReenable = false;

                try
                {
                    // If the value is not the empty string, we want to set NetworkAddress to it,
                    // so it had better be valid
                    if (value.Length > 0 && !Adapter.IsValidMac(value, false))
                        throw new Exception(value + " ist keine gültige MAC Adresse.");

                    using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(this.RegistryKey, true))
                    {
                        if (regkey == null)
                            throw new Exception("Zugriff auf Registry nicht möglich.");

                        // Sanity check
                        if (regkey.GetValue("AdapterModel") as string != this.adaptername
                            && regkey.GetValue("DriverDesc") as string != this.adaptername)
                            throw new Exception("Adapter konnte nicht gefunden werden.");

                        // Attempt to disable the adepter
                        var result = (uint)adapter.InvokeMethod("Disable", null);
                        if (result != 0)
                            throw new Exception("Netzwerk Adapter konnte nicht gestoppt werden.");

                        // If we're here the adapter has been disabled, so we set the flag that will re-enable it in the finally block
                        shouldReenable = true;

                        // If we're here everything is OK; update or clear the registry value
                        if (value.Length > 0)
                            regkey.SetValue("NetworkAddress", value, RegistryValueKind.String);
                        else
                            regkey.DeleteValue("NetworkAddress");

                        return true;
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Failed spoofing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                finally
                {
                    if (shouldReenable)
                    {
                        uint result = (uint)adapter.InvokeMethod("Enable", null);
                        if (result != 0)
                            MessageBox.Show("Das Neustarten des Adapters ist fehlgeschlagen. Bitte starte deinen Rechner neu.");
                    }
                }
            }

            public override string ToString()
            {
                return this.adaptername + this.customname;
            }

            /// <summary>
            /// Get a random (locally administered) MAC address.
            /// </summary>
            /// <returns>A MAC address having 01 as the least significant bits of the first byte, but otherwise random.</returns>
            public static string GetNewMac()
            {
                System.Random r = new System.Random();

                byte[] bytes = new byte[6];
                r.NextBytes(bytes);

                // Set second bit to 1
                bytes[0] = (byte)(bytes[0] | 0x02);
                // Set first bit to 0
                bytes[0] = (byte)(bytes[0] & 0xfe);

                return MacToString(bytes);
            }

            /// <summary>
            /// Verifies that a given string is a valid MAC address.
            /// </summary>
            /// <param name="mac">The string.</param>
            /// <param name="actual">false if the address is a locally administered address, true otherwise.</param>
            /// <returns>true if the string is a valid MAC address, false otherwise.</returns>
            public static bool IsValidMac(string mac, bool actual)
            {
                // 6 bytes == 12 hex characters (without dashes/dots/anything else)
                if (mac.Length != 12)
                    return false;

                // Should be uppercase
                if (mac != mac.ToUpper())
                    return false;

                // Should not contain anything other than hexadecimal digits
                if (!Regex.IsMatch(mac, "^[0-9A-F]*$"))
                    return false;

                if (actual)
                    return true;

                // If we're here, then the second character should be a 2, 6, A or E
                char c = mac[1];
                return (c == '2' || c == '6' || c == 'A' || c == 'E');
            }

            /// <summary>
            /// Verifies that a given MAC address is valid.
            /// </summary>
            /// <param name="mac">The address.</param>
            /// <param name="actual">false if the address is a locally administered address, true otherwise.</param>
            /// <returns>true if valid, false otherwise.</returns>
            public static bool IsValidMac(byte[] bytes, bool actual)
            {
                return IsValidMac(Adapter.MacToString(bytes), actual);
            }

            /// <summary>
            /// Converts a byte array of length 6 to a MAC address (i.e. string of hexadecimal digits).
            /// </summary>
            /// <param name="bytes">The bytes to convert.</param>
            /// <returns>The MAC address.</returns>
            public static string MacToString(byte[] bytes)
            {
                return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
            }
        }

        //######################################MAC Spoofer End###############################################

        public Main_Form()
        {
            InitializeComponent();
            load_network_adapters();
            database_information_loader();
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {
            client_bomb_loader();
        }

        public void client_bomb_loader()
        {
            clients_listbox.Items.Clear();
            credentials_spotify_edit_account_combobox.Items.Clear();
            credentials_spotify_account_combobox.Items.Clear();
            credentials_spotify_remove_playlist_combobox.Items.Clear();
            credentials_deezer_edit_account_combobox.Items.Clear();
            credentials_deezer_remove_playlist_combobox.Items.Clear();
            credentials_deezer_account_combobox.Items.Clear();
            openvpn_profiles_listbox.Items.Clear();
            playlist_creator_spotify_song_url_listbox.Items.Clear();
            playlist_creator_deezer_song_url_listbox.Items.Clear();
            playlist_creator_spotify_credentials_combobox.Items.Clear();
            artist_manager_artists_listbox.Items.Clear();
            playlist_creator_deezer_credentials_combobox.Items.Clear();
            playlist_creator_add_song_name_textbox.Clear();
            settings_hive_combobox.Items.Clear();
            clients_hive_filter_combobox.Items.Clear();
            clients_general_settings_hive_combobox.Items.Clear();
            playlist_poly_spotify_selected_hive_combobox.Items.Clear();
            artist_manager_hive_name_combobox.Items.Clear();
            artist_manager_selected_hive_filter_combobox.Items.Clear();
            playlist_creator_spotify_selected_hive_combobox.Items.Clear();
            credentials_spotify_add_playlist_hive_filter_combobox.Items.Clear();
            misc_random_time_sheduler_hive_filter_combobox.Items.Clear();
            openvpn_clients_profiles_listbox.Items.Clear();

            client_overview_loader();

            this.Alert("DB Configuration loaded.", Helper.Form_Alert.enmType.Success);
        }

        private void load_network_adapters()
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces().Where(
                    a => Adapter.IsValidMac(a.GetPhysicalAddress().GetAddressBytes(), true)
                ).OrderByDescending(a => a.Speed))
            {
                AdaptersComboBox.Items.Add(new Adapter(adapter));
            }

            AdaptersComboBox.SelectedIndex = 0;
        }

        //Load Database Information from Registry
        public void database_information_loader()
        {
            //Read MySQL Host Regkey
            try
            {
                RegistryKey MySQL_Host_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
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
                RegistryKey MySQL_Database_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
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
                RegistryKey MySQL_Username_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
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
                RegistryKey MySQL_Password_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
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
                RegistryKey MySQL_Port_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Management Console");
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
            connStr = "server=" + mysql_host + ";user=" + mysql_username + ";database=" + mysql_database + ";port=" + mysql_port + ";password=" + mysql_password + ";Pooling=false;default command timeout=30;Connection Timeout=30;";
        }

        //Load the client information to a overview
        private void client_overview_loader()
        {
            string sql_clients = "SELECT client_name FROM `" + mysql_database + "`.`clients`;";

            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand clients_cmd = new MySqlCommand(sql_clients, conn);

            //Client Credentials Loader
            try
            {
                conn.Open();

                //Load Client Overview
                try
                {
                    string sql = "SELECT client_name, hive_name, last_access, spotify_enabled, spotify_player_account, deezer_enabled, auto_restart_bot, openvpn_enabled, custom_useragent FROM `" + mysql_database + "`.`clients`;";

                    MySqlDataAdapter mysql_data_adapter = new MySqlDataAdapter();
                    mysql_data_adapter.SelectCommand = new MySqlCommand(sql, conn);

                    DataTable mysql_table = new DataTable();
                    mysql_data_adapter.Fill(mysql_table);

                    client_overview_gridview.DataSource = mysql_table;


                    //Change Colum Titles to look more user friendly
                    client_overview_gridview.Columns[0].HeaderText = "Client Name";
                    client_overview_gridview.Columns[1].HeaderText = "Hive Name";
                    client_overview_gridview.Columns[2].HeaderText = "Last Access";
                    client_overview_gridview.Columns[3].HeaderText = "Spotify Enabled";
                    client_overview_gridview.Columns[4].HeaderText = "Spotify Account";
                    client_overview_gridview.Columns[5].HeaderText = "Deezer Enabled";
                    client_overview_gridview.Columns[6].HeaderText = "Auto Restart";
                    client_overview_gridview.Columns[7].HeaderText = "OpenVPN";
                    client_overview_gridview.Columns[8].HeaderText = "Useragent Name";
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Clients Overview from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Fill Playlist Creator Song overview
                try
                {
                    string sql = "";

                    if (playlist_creator_spotify_selected_hive_combobox.SelectedIndex == -1)
                    {
                        sql = "SELECT artist_name, song_name, spotify_url, hive_name FROM songs WHERE spotify_url;";
                    }
                    else
                    {
                        sql = "SELECT artist_name, song_name, spotify_url, hive_name FROM songs WHERE spotify_url IS NOT NULL AND hive_name = '" + playlist_creator_spotify_selected_hive_combobox.SelectedItem.ToString() + "';";
                    }

                    MySqlDataAdapter mysql_data_adapter = new MySqlDataAdapter();
                    mysql_data_adapter.SelectCommand = new MySqlCommand(sql, conn);

                    DataTable mysql_table = new DataTable();
                    mysql_data_adapter.Fill(mysql_table);

                    playlist_generator_spotify_song_datagridview.DataSource = mysql_table;

                    //Change Colum Titles to look more user friendly
                    playlist_generator_spotify_song_datagridview.Columns[0].HeaderText = "Artist";
                    playlist_generator_spotify_song_datagridview.Columns[1].HeaderText = "Song";
                    playlist_generator_spotify_song_datagridview.Columns[2].HeaderText = "Spotify URL";
                    playlist_generator_spotify_song_datagridview.Columns[3].HeaderText = "Hive Name";
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Playlist Creator Overview from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Load Clients connected to the selected group
                try
                {
                    MySqlDataReader clients_rdr = clients_cmd.ExecuteReader();
                    if (clients_rdr.HasRows)
                    {
                        while (clients_rdr.Read())
                        {
                            clients_listbox.Items.Add(clients_rdr[0].ToString());
                            openvpn_clients_profiles_listbox.Items.Add(clients_rdr[0].ToString());
                        }
                    }
                    clients_rdr.Close();

                    clients_counter_label.Text = clients_listbox.Items.Count.ToString();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Clients from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Spotify Playlist Loader
                try
                {
                    string sql1 = "SELECT url FROM spotify_playlists";
                    MySqlCommand cmd1 = new MySqlCommand(sql1, conn);
                    MySqlDataReader rdr1 = cmd1.ExecuteReader();

                    if (rdr1.HasRows)
                    {
                        while (rdr1.Read())
                        {
                            credentials_spotify_remove_playlist_combobox.Items.Add(rdr1[0]).ToString();
                        }
                    }
                    rdr1.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Spotify Playlists from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Spotify Credentials Loader
                try
                {
                    string sql2 = "SELECT username FROM spotify_credentials;";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                    MySqlDataReader rdr2 = cmd2.ExecuteReader();

                    if (rdr2.HasRows)
                    {
                        while (rdr2.Read())
                        {
                            credentials_spotify_edit_account_combobox.Items.Add(rdr2[0]);
                            credentials_spotify_account_combobox.Items.Add(rdr2[0]);
                        }
                    }

                    rdr2.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Spotify Credentials from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Load outdated spotify credentials
                try
                {
                    string sql = "SELECT username, client_name, hive_name FROM spotify_credentials WHERE outdated = '1';";

                    MySqlDataAdapter mysql_data_adapter = new MySqlDataAdapter();
                    mysql_data_adapter.SelectCommand = new MySqlCommand(sql, conn);

                    DataTable mysql_table = new DataTable();
                    mysql_data_adapter.Fill(mysql_table);

                    credentials_spotify_outdated_datagridview.DataSource = mysql_table;

                    //Change Colum Titles to look more user friendly
                    credentials_spotify_outdated_datagridview.Columns[0].HeaderText = "Username";
                    credentials_spotify_outdated_datagridview.Columns[1].HeaderText = "Client Name";
                    credentials_spotify_outdated_datagridview.Columns[2].HeaderText = "Hive Name";
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Spotify Credentials (Outdated) from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Deezer Playlist Loader
                try
                {
                    string sql3 = "SELECT url FROM deezer_playlists";
                    MySqlCommand cmd3 = new MySqlCommand(sql3, conn);
                    MySqlDataReader rdr3 = cmd3.ExecuteReader();

                    if (rdr3.HasRows)
                    {
                        while (rdr3.Read())
                        {
                            credentials_deezer_remove_playlist_combobox.Items.Add(rdr3[0]).ToString();
                            playlist_creator_deezer_playlist_url_listbox.Items.Add(rdr3[0]).ToString();
                        }
                    }
                    rdr3.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Deezer Playlists from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Deezer Credentials Loader
                try
                {
                    string sql4 = "SELECT username FROM deezer_credentials;";
                    MySqlCommand cmd4 = new MySqlCommand(sql4, conn);
                    MySqlDataReader rdr4 = cmd4.ExecuteReader();

                    if (rdr4.HasRows)
                    {
                        while (rdr4.Read())
                        {
                            credentials_deezer_edit_account_combobox.Items.Add(rdr4[0]);
                            credentials_deezer_account_combobox.Items.Add(rdr4[0]);
                        }
                    }

                    rdr4.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Deezer Credentials from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //OpenVPN Loader
                try
                {
                    string openvpn_profiles_sql = "SELECT * FROM openvpn_profiles";
                    MySqlCommand openvpn_profiles_cmd = new MySqlCommand(openvpn_profiles_sql, conn);
                    MySqlDataReader openvpn_profiles_rdr = openvpn_profiles_cmd.ExecuteReader();

                    if (openvpn_profiles_rdr.HasRows)
                    {
                        while (openvpn_profiles_rdr.Read())
                        {
                            client_openvpn_selected_profile_combobox.Items.Add(openvpn_profiles_rdr[1]);
                            openvpn_profiles_listbox.Items.Add(openvpn_profiles_rdr[1]);
                        }
                    }

                    openvpn_profiles_rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load OpenVPN Profiles from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Load Spotify Song Urls to listbox
                try
                {
                    string songs_sql = "";

                    if (playlist_creator_spotify_selected_hive_combobox.SelectedIndex == -1)
                    {
                        songs_sql = "SELECT spotify_url FROM songs WHERE spotify_url;";
                    }
                    else
                    {
                        songs_sql = "SELECT spotify_url FROM songs WHERE spotify_url IS NOT NULL AND hive_name = '" + playlist_creator_spotify_selected_hive_combobox.SelectedItem.ToString() + "';";
                    }

                    MySqlCommand songs_cmd = new MySqlCommand(songs_sql, conn);
                    MySqlDataReader songs_rdr = songs_cmd.ExecuteReader();

                    if (songs_rdr.HasRows)
                    {
                        while (songs_rdr.Read())
                        {
                            playlist_creator_spotify_song_url_listbox.Items.Add(songs_rdr[0]);
                        }
                    }
                    songs_rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Spotify Playlist Creator Song URLs from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Load Credentials Spotify Playlist Accs
                try
                {
                    string spotify_playlist_accounts_sql = "SELECT username FROM spotify_playlist_accounts;";
                    MySqlCommand spotify_playlist_accounts_cmd = new MySqlCommand(spotify_playlist_accounts_sql, conn);
                    MySqlDataReader spotify_playlist_accounts_rdr = spotify_playlist_accounts_cmd.ExecuteReader();
                    if (spotify_playlist_accounts_rdr.HasRows)
                    {
                        while (spotify_playlist_accounts_rdr.Read())
                        {
                            playlist_creator_spotify_credentials_combobox.Items.Add(spotify_playlist_accounts_rdr[0]);
                        }
                    }
                    spotify_playlist_accounts_rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Spotify Playlist Creator Accounts from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Load Song Urls to listbox
                try
                {
                    string deezer_song_urls_sql = "SELECT url FROM deezer_song_urls;";
                    MySqlCommand deezer_song_urls_cmd = new MySqlCommand(deezer_song_urls_sql, conn);
                    MySqlDataReader deezer_song_urls_rdr = deezer_song_urls_cmd.ExecuteReader();

                    if (deezer_song_urls_rdr.HasRows)
                    {
                        while (deezer_song_urls_rdr.Read())
                        {
                            playlist_creator_deezer_song_url_listbox.Items.Add(deezer_song_urls_rdr[0]);
                        }
                    }
                    deezer_song_urls_rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Deezer Playlist Creator Song URLs from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Load Credentials Deezer
                try
                {
                    string deezer_playlist_accounts_sql = "SELECT username FROM deezer_playlist_accounts;";
                    MySqlCommand deezer_playlist_accounts_cmd = new MySqlCommand(deezer_playlist_accounts_sql, conn);
                    MySqlDataReader deezer_playlist_accounts_rdr = deezer_playlist_accounts_cmd.ExecuteReader();
                    if (deezer_playlist_accounts_rdr.HasRows)
                    {
                        while (deezer_playlist_accounts_rdr.Read())
                        {
                            playlist_creator_deezer_credentials_combobox.Items.Add(deezer_playlist_accounts_rdr[0]);
                        }
                    }
                    deezer_playlist_accounts_rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Deezer Playlist Creator Accounts from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Account Creator Settings Loader
                try
                {
                    string account_creator_settings_sql = "SELECT * FROM account_generator;";
                    MySqlCommand account_creator_settings_cmd = new MySqlCommand(account_creator_settings_sql, conn);
                    MySqlDataReader account_creator_settings_rdr = account_creator_settings_cmd.ExecuteReader();

                    if (account_creator_settings_rdr.HasRows)
                    {
                        while (account_creator_settings_rdr.Read())
                        {
                            account_generator_distributor_first_name_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[3].ToString(), Settings.Encryption_Key);
                            account_generator_distributor_last_name_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[4].ToString(), Settings.Encryption_Key);
                            account_generator_distributor_country_combobox.SelectedIndex = account_generator_distributor_country_combobox.Items.IndexOf(StringCipher.Decrypt(account_creator_settings_rdr[5].ToString(), Settings.Encryption_Key));
                            account_generator_distributor_phone_number_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[6].ToString(), Settings.Encryption_Key);
                            account_generator_distributor_hdykau_combobox.Text = StringCipher.Decrypt(account_creator_settings_rdr[7].ToString(), Settings.Encryption_Key);
                            account_generator_distributor_street_number_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[8].ToString(), Settings.Encryption_Key);
                            account_generator_distributor_plz_city_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[9].ToString(), Settings.Encryption_Key);
                            account_generator_distributor_artist_name_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[10].ToString(), Settings.Encryption_Key);
                            account_generator_distributor_mail_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[11].ToString(), Settings.Encryption_Key);
                            account_generator_distributor_password_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[12].ToString(), Settings.Encryption_Key);
                            misc_cpanel_mail_account_password_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[13].ToString(), Settings.Encryption_Key);
                            misc_cpanel_mail_account_amount_textbox.Text = account_creator_settings_rdr[14].ToString();
                            misc_spotify_mail_registration_confirmer_roundcube_url_textbox.Text = StringCipher.Decrypt(account_creator_settings_rdr[15].ToString(), Settings.Encryption_Key);
                        }
                    }
                    account_creator_settings_rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Account Creator Info from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                int telegram_monitoring = 0;

                //Settings SQL Loader
                try
                {
                    //18
                    string settings_sql1 = "SELECT telegram_active, telegram_api_token, telegram_chat_id, telegram_alert_chat_id, namescheap_username, namescheap_password, artist_manager_roundcube_mail_password, artist_manager_roundcube_mail_domain, playlist_poly_index, telegram_alert_creds_chat_id, cpanel_url, cpanel_username, cpanel_password, client_mail_domain, client_mail_password, ssh_remote_hostname, ssh_username, ssh_password, ssh_port FROM settings;";
                    MySqlCommand settings_cmd1 = new MySqlCommand(settings_sql1, conn);
                    MySqlDataReader settings_rdr1 = settings_cmd1.ExecuteReader();

                    if (settings_rdr1.HasRows)
                    {
                        while (settings_rdr1.Read())
                        {
                            telegram_monitoring = Convert.ToInt32(settings_rdr1[0]);
                            telegram_api_token_textbox.Text = StringCipher.Decrypt(settings_rdr1[1].ToString(), Settings.Encryption_Key);
                            telegram_channelid_textbox.Text = StringCipher.Decrypt(settings_rdr1[2].ToString(), Settings.Encryption_Key);
                            telegram_alert_chat_id_textbox.Text = StringCipher.Decrypt(settings_rdr1[3].ToString(), Settings.Encryption_Key);
                            settings_namescheap_username_textbox.Text = StringCipher.Decrypt(settings_rdr1[4].ToString(), Settings.Encryption_Key);
                            settings_namescheap_password_textbox.Text = StringCipher.Decrypt(settings_rdr1[5].ToString(), Settings.Encryption_Key);
                            settings_artist_manager_mail_password_textbox.Text = StringCipher.Decrypt(settings_rdr1[6].ToString(), Settings.Encryption_Key);
                            settings_artist_manager_mail_domain_textbox.Text = StringCipher.Decrypt(settings_rdr1[7].ToString(), Settings.Encryption_Key);
                            //playlist_poly_spotify_playlist_name_listbox.SelectedIndex = (int)settings_rdr1[8];
                            telegram_alert_creds_chat_id_textbox.Text = StringCipher.Decrypt(settings_rdr1[9].ToString(), Settings.Encryption_Key);
                            settings_cpanel_url_textbox.Text = StringCipher.Decrypt(settings_rdr1[10].ToString(), Settings.Encryption_Key);
                            settings_cpanel_username_textbox.Text = StringCipher.Decrypt(settings_rdr1[11].ToString(), Settings.Encryption_Key);
                            settings_cpanel_password_textbox.Text = StringCipher.Decrypt(settings_rdr1[12].ToString(), Settings.Encryption_Key);
                            settings_client_mail_domain_textbox.Text = StringCipher.Decrypt(settings_rdr1[13].ToString(), Settings.Encryption_Key);
                            settings_client_mail_password_textbox.Text = StringCipher.Decrypt(settings_rdr1[14].ToString(), Settings.Encryption_Key);
                            settings_ssh_remote_host_textbox.Text = StringCipher.Decrypt(settings_rdr1[15].ToString(), Settings.Encryption_Key);
                            settings_ssh_username_textbox.Text = StringCipher.Decrypt(settings_rdr1[16].ToString(), Settings.Encryption_Key);
                            settings_ssh_password_textbox.Text = StringCipher.Decrypt(settings_rdr1[17].ToString(), Settings.Encryption_Key);
                            settings_ssh_port_textbox.Text = StringCipher.Decrypt(settings_rdr1[18].ToString(), Settings.Encryption_Key);
                        }
                    }
                    settings_rdr1.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Overall Settings from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Artists Manager
                try
                {
                    string sql1 = "SELECT artist_name FROM artist_manager;";
                    MySqlCommand cmd = new MySqlCommand(sql1, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            artist_manager_artists_listbox.Items.Add(rdr[0].ToString());
                        }
                    }
                    rdr.Close();

                    artist_manager_artists_counter_label.Text = artist_manager_artists_listbox.Items.Count.ToString();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Artists from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Hive Loader
                try
                {
                    string sql = "SELECT hive_name FROM hives;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            settings_hive_combobox.Items.Add(rdr[0]).ToString();
                            clients_hive_filter_combobox.Items.Add(rdr[0]).ToString();
                            clients_general_settings_hive_combobox.Items.Add(rdr[0]).ToString();
                            playlist_poly_spotify_selected_hive_combobox.Items.Add(rdr[0]).ToString();
                            artist_manager_hive_name_combobox.Items.Add(rdr[0]).ToString();
                            artist_manager_selected_hive_filter_combobox.Items.Add(rdr[0]).ToString();
                            playlist_creator_spotify_selected_hive_combobox.Items.Add(rdr[0]).ToString();
                            credentials_spotify_add_playlist_hive_filter_combobox.Items.Add(rdr[0]).ToString();
                            misc_random_time_sheduler_hive_filter_combobox.Items.Add(rdr[0]).ToString();
                        }
                    }
                    rdr.Close();

                    try
                    {
                        credentials_spotify_add_playlist_hive_filter_combobox.SelectedIndex = 0;
                        clients_hive_filter_combobox.SelectedIndex = 0;
                        artist_manager_selected_hive_filter_combobox.SelectedIndex = 0;
                    }
                    catch
                    {

                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Hives from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");
                }

                //Useragent Loader
                try
                {
                    string sql = "SELECT useragent_name FROM useragents;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            useragent_manager_useragents_combobox.Items.Add(rdr[0]);
                        }
                    }

                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Useragents from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                conn.Close();

                if (telegram_monitoring == 1)
                {
                    telegram_monitoring_checkbox.Checked = true;
                }
                else
                {
                    telegram_monitoring_checkbox.Checked = false;
                }

                playlist_creator_deezer_song_urls_counter_label.Text = "(" + playlist_creator_deezer_song_url_listbox.Items.Count.ToString() + ")";
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Overall Error!: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        //Listbox Drawing
        private void clients_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < clients_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(102, 102, 102) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = clients_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, clients_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        //Load a clients configuration
        private void clients_listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*Client General Information Loader*/

            if (clients_listbox.SelectedItems.Count == 0)
            {
                player_menu_tabcontrol.Enabled = false;
                metroTabControl3.Enabled = false;
                return;
            }
            else
            {
                player_menu_tabcontrol.Enabled = true;
                metroTabControl3.Enabled = true;
            }

            metroTabControl3.Enabled = false;

            client_openvpn_selected_profile_combobox.Items.Clear();
            client_spotify_playlist_combobox.Items.Clear();
            client_spotify_selected_credentials_combobox.Items.Clear();
            client_deezer_playlist_combobox.Items.Clear();
            client_deezer_selected_credentials_combobox.Items.Clear();
            client_useragent_string_textbox.Clear();
            client_spotify_assigned_credentials_listbox.Items.Clear();
            client_spotify_availiable_credentials_listbox.Items.Clear();

            //General Ints
            int auto_restart_bot = 0;
            int auto_restart_bot_timer = 0;
            int openvpn_enabled = 0;
            int openvpn_randomize = 0;
            int openvpn_health_check = 0;
            int custom_profile_enabled = 0;
            int custom_useragent_enabled = 0;
            int openvpn_bound_credentials_enabled = 0;
            int cookies_sync_enabled = 0;

            //Time Sheduler Settings
            int monday = 0;
            int tuesday = 0;
            int wednesday = 0;
            int thursday = 0;
            int friday = 0;
            int saturday = 0;
            int sunday = 0;

            //Spotify Ints
            int spotify_enabled = 0;
            int spotify_autoskip_enabled = 0;
            int spotify_autoskip_from = 0;
            int spotify_autoskip_to = 0;
            int spotify_rotate_credentials = 0;
            int emulate_device_enabled = 0;
            int chrome_persistent_profiles_enabled = 0;

            //Deezer Ints
            int deezer_enabled = 0;
            int deezer_autoskip_enabled = 0;
            int deezer_autoskip_from = 0;
            int deezer_autoskip_to = 0;

            //Misc
            int reinstall_chrome = 0;
            int reinstall_firefox = 0;
            int renew_windows_trial_licence = 0;

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                //Spotify Playlist Loader
                try
                {
                    string sql = "SELECT url FROM spotify_playlists WHERE hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            client_spotify_playlist_combobox.Items.Add(rdr[0]).ToString();
                        }
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Spotify Playlists from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Spotify Credentials Loader
                try
                {
                    string sql = "SELECT username FROM spotify_credentials WHERE client_name = '" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            client_spotify_selected_credentials_combobox.Items.Add(rdr[0]);
                        }
                    }

                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Spotify Credentials from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Spotify Credentials Loader (Available)
                try
                {
                    string sql = "SELECT username FROM spotify_credentials WHERE client_name = '-';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            client_spotify_availiable_credentials_listbox.Items.Add(rdr[0]);
                        }
                    }

                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Spotify Credentials Loader (Available): " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Spotify Credentials Loader (Assigned)
                try
                {
                    string sql = "SELECT username FROM spotify_credentials WHERE client_name = '" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            client_spotify_assigned_credentials_listbox.Items.Add(rdr[0]);
                        }
                    }

                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Spotify Credentials Loader (Assigned): " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Deezer Playlist Loader
                try
                {
                    string sql = "SELECT url FROM deezer_playlists;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            client_deezer_playlist_combobox.Items.Add(rdr[0]).ToString();
                        }
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Deezer Playlists from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Deezer Credentials Loader
                try
                {
                    string sql = "SELECT username FROM deezer_credentials;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            client_deezer_selected_credentials_combobox.Items.Add(rdr[0]);
                        }
                    }

                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Delete Group from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //OpenVPN Loader
                try
                {
                    string sql = "SELECT * FROM openvpn_profiles";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            client_openvpn_selected_profile_combobox.Items.Add(rdr[1]);
                        }
                    }

                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Delete Group from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Useragent Loader
                try
                {
                    client_useragent_name_combobox.Items.Clear();

                    string sql = "SELECT useragent_name FROM useragents;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            client_useragent_name_combobox.Items.Add(rdr[0]);
                        }
                    }

                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Useragents from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Client Settings Loader
                try
                {
                    //40
                    string general_information_sql = "SELECT auto_restart_bot, auto_restart_bot_timer, openvpn_enabled, openvpn_randomize, openvpn_health_check, openvpn_profile, spotify_enabled, spotify_playlist_url, spotify_player_account, spotify_player_password, spotify_autoskip_enabled, spotify_autoskip_forward_from, spotify_autoskip_forward_to, deezer_enabled, deezer_playlist_url, deezer_player_account, deezer_autoskip_enabled, deezer_autoskip_forward_from, deezer_autoskip_forward_to, custom_profile_enabled, player_type, play_time_sheduler_monday_enabled, play_time_sheduler_tuesday_enabled, play_time_sheduler_wednesday_enabled, play_time_sheduler_thursday_enabled, play_time_sheduler_friday_enabled, play_time_sheduler_saturday_enabled, play_time_sheduler_sunday_enabled, reinstall_chrome, custom_useragent_enabled, custom_useragent, rotate_credentials_enabled, emulate_device_enabled, emulate_device_type, renew_windows_trial_licence, hive_name, automation_mode, openvpn_bound_credentials_enabled, chrome_persistent_profiles_enabled, reinstall_firefox, cookies_sync_enabled FROM clients WHERE client_name = '" + clients_listbox.SelectedItem.ToString() + "';";

                    MySqlCommand cmd = new MySqlCommand(general_information_sql, conn);
                    MySqlDataReader sql_reader = cmd.ExecuteReader();

                    if (sql_reader.HasRows)
                    {
                        while (sql_reader.Read())
                        {
                            //General Settings
                            auto_restart_bot = (int)sql_reader[0];
                            auto_restart_bot_timer = (int)sql_reader[1] / 3600000;
                            client_auto_restart_time_textbox.Text = auto_restart_bot_timer.ToString();
                            openvpn_enabled = (int)sql_reader[2];
                            openvpn_randomize = (int)sql_reader[3];
                            openvpn_health_check = (int)sql_reader[4];
                            client_openvpn_selected_profile_combobox.SelectedIndex = client_openvpn_selected_profile_combobox.FindStringExact(sql_reader[5].ToString());
                            custom_profile_enabled = (int)sql_reader[19];
                            openvpn_bound_credentials_enabled = (int)sql_reader[37];
                            client_player_type_combobox.SelectedIndex = client_player_type_combobox.FindStringExact(sql_reader[20].ToString());
                            custom_useragent_enabled = (int)sql_reader[29];
                            client_useragent_name_combobox.SelectedIndex = client_useragent_name_combobox.FindStringExact(sql_reader[30].ToString());
                            clients_general_settings_automation_mode_combobox.SelectedIndex = clients_general_settings_automation_mode_combobox.FindStringExact(sql_reader[36].ToString());
                            cookies_sync_enabled = (int)sql_reader[40];

                            //Get Time Sheduler Settings
                            monday = (int)sql_reader[21];
                            tuesday = (int)sql_reader[22];
                            wednesday = (int)sql_reader[23];
                            thursday = (int)sql_reader[24];
                            friday = (int)sql_reader[25];
                            saturday = (int)sql_reader[26];
                            sunday = (int)sql_reader[27];

                            //Spotify Settings
                            spotify_enabled = (int)sql_reader[6];
                            client_spotify_playlist_combobox.SelectedIndex = client_spotify_playlist_combobox.FindStringExact(sql_reader[7].ToString());
                            client_spotify_selected_credentials_combobox.SelectedIndex = client_spotify_selected_credentials_combobox.FindStringExact(sql_reader[8].ToString());
                            spotify_autoskip_enabled = (int)sql_reader[10];
                            spotify_autoskip_from = (int)sql_reader[11] / 1000;
                            client_spotify_autoskip_forward_from_textbox.Text = spotify_autoskip_from.ToString();
                            spotify_autoskip_to = (int)sql_reader[12] / 1000; ;
                            client_spotify_autoskip_to_textbox.Text = spotify_autoskip_to.ToString();
                            spotify_rotate_credentials = (int)sql_reader[31];
                            emulate_device_enabled = (int)sql_reader[32];
                            client_emulate_device_combobox.SelectedIndex = client_emulate_device_combobox.FindStringExact(sql_reader[33].ToString());
                            chrome_persistent_profiles_enabled = (int)sql_reader[38];

                            //Deezer Settings
                            deezer_enabled = (int)sql_reader[13];
                            client_deezer_playlist_combobox.SelectedIndex = client_deezer_playlist_combobox.FindStringExact(sql_reader[14].ToString());
                            client_deezer_selected_credentials_combobox.SelectedIndex = client_deezer_selected_credentials_combobox.FindStringExact(sql_reader[15].ToString());
                            deezer_autoskip_enabled = (int)sql_reader[16];
                            deezer_autoskip_from = (int)sql_reader[17] / 1000;
                            client_deezer_autoskip_forward_from_textbox.Text = deezer_autoskip_from.ToString();
                            deezer_autoskip_to = (int)sql_reader[18] / 1000;
                            client_deezer_autoskip_to_textbox.Text = deezer_autoskip_to.ToString();

                            //Misc
                            reinstall_chrome = (int)sql_reader[28];
                            reinstall_firefox = (int)sql_reader[39];
                            renew_windows_trial_licence = (int)sql_reader[34];
                            clients_general_settings_hive_combobox.SelectedIndex = clients_general_settings_hive_combobox.FindStringExact(sql_reader[35].ToString());
                        }
                    }
                    sql_reader.Close();

                    if (auto_restart_bot == 1)
                    {
                        client_auto_restart_enabled_checkbox.Checked = true;
                    }
                    else
                    {
                        client_auto_restart_enabled_checkbox.Checked = false;
                    }

                    if (openvpn_enabled == 1)
                    {
                        client_openvpn_enabled_checkbox.Checked = true;
                    }
                    else
                    {
                        client_openvpn_enabled_checkbox.Checked = false;
                    }

                    if (openvpn_randomize == 1)
                    {
                        client_openvpn_randomize_checkbox.Checked = true;
                    }
                    else
                    {
                        client_openvpn_randomize_checkbox.Checked = false;
                    }

                    if (openvpn_health_check == 1)
                    {
                        client_openvpn_health_status_checkbox.Checked = true;
                    }
                    else
                    {
                        client_openvpn_health_status_checkbox.Checked = false;
                    }

                    if (openvpn_bound_credentials_enabled == 1)
                    {
                        client_general_settings_bind_to_credential_checkbox.Checked = true;
                    }
                    else
                    {
                        client_general_settings_bind_to_credential_checkbox.Checked = false;
                    }

                    if (custom_useragent_enabled == 1)
                    {
                        client_useragent_enabled_checkbox.Checked = true;
                    }
                    else
                    {
                        client_useragent_enabled_checkbox.Checked = false;
                    }

                    //Check Custom Profile Checkbox
                    if (custom_profile_enabled == 1)
                    {
                        clients_general_settings_custom_profile_checkbox.Checked = true;
                    }
                    else
                    {
                        clients_general_settings_custom_profile_checkbox.Checked = false;
                    }

                    //Spotify Checkboxes
                    if (spotify_enabled == 1)
                    {
                        client_spotify_enabled_toggle.Checked = true;
                    }
                    else
                    {
                        client_spotify_enabled_toggle.Checked = false;
                    }

                    if (spotify_autoskip_enabled == 1)
                    {
                        client_spotify_autoskip_forward_enabled_checkbox.Checked = true;
                    }
                    else
                    {
                        client_spotify_autoskip_forward_enabled_checkbox.Checked = false;
                    }

                    if (spotify_rotate_credentials == 1)
                    {
                        client_spotify_rotate_credentials_checkbox.Checked = true;
                    }
                    else
                    {
                        client_spotify_rotate_credentials_checkbox.Checked = false;
                    }

                    if (chrome_persistent_profiles_enabled == 1)
                    {
                        client_persistent_chrome_profile_checkbox.Checked = true;
                    }
                    else
                    {
                        client_persistent_chrome_profile_checkbox.Checked = false;
                    }

                    //Deezer Checkboxes
                    if (deezer_enabled == 1)
                    {
                        client_deezer_enabled_toggle.Checked = true;
                    }
                    else
                    {
                        client_deezer_enabled_toggle.Checked = false;
                    }

                    if (deezer_autoskip_enabled == 1)
                    {
                        client_deezer_autoskip_forward_enabled_checkbox.Checked = true;
                    }
                    else
                    {
                        client_deezer_autoskip_forward_enabled_checkbox.Checked = false;
                    }

                    //Time Sheduler Checkboxes
                    if (monday == 1)
                    {
                        client_play_time_sheduler_monday_checkbox.Checked = true;
                    }
                    else
                    {
                        client_play_time_sheduler_monday_checkbox.Checked = false;
                    }

                    if (tuesday == 1)
                    {
                        client_play_time_sheduler_tuesday_checkbox.Checked = true;
                    }
                    else
                    {
                        client_play_time_sheduler_tuesday_checkbox.Checked = false;
                    }

                    if (wednesday == 1)
                    {
                        client_play_time_sheduler_wednesday_checkbox.Checked = true;
                    }
                    else
                    {
                        client_play_time_sheduler_wednesday_checkbox.Checked = false;
                    }

                    if (thursday == 1)
                    {
                        client_play_time_sheduler_thursday_checkbox.Checked = true;
                    }
                    else
                    {
                        client_play_time_sheduler_thursday_checkbox.Checked = false;
                    }


                    if (friday == 1)
                    {
                        client_play_time_sheduler_friday_checkbox.Checked = true;
                    }
                    else
                    {
                        client_play_time_sheduler_friday_checkbox.Checked = false;
                    }

                    if (saturday == 1)
                    {
                        client_play_time_sheduler_saturday_checkbox.Checked = true;
                    }
                    else
                    {
                        client_play_time_sheduler_saturday_checkbox.Checked = false;
                    }

                    if (sunday == 1)
                    {
                        client_play_time_sheduler_sunday_checkbox.Checked = true;
                    }
                    else
                    {
                        client_play_time_sheduler_sunday_checkbox.Checked = false;
                    }

                    //Misc
                    if (reinstall_chrome == 1)
                    {
                        client_reinstall_chrome_checkbox.Checked = true;
                    }
                    else
                    {
                        client_reinstall_chrome_checkbox.Checked = false;
                    }

                    if (reinstall_firefox == 1)
                    {
                        client_reinstall_firefox_checkbox.Checked = true;
                    }
                    else
                    {
                        client_reinstall_firefox_checkbox.Checked = false;
                    }

                    //Emulate Device
                    if (emulate_device_enabled == 1)
                    {
                        client_emulate_device_checkbox.Checked = true;
                    }
                    else
                    {
                        client_emulate_device_checkbox.Checked = false;
                    }

                    //Renew Windows Trial Licence
                    if (renew_windows_trial_licence == 1)
                    {
                        clients_general_settings_renew_windows_trial_licence_checkbox.Checked = true;
                    }
                    else
                    {
                        clients_general_settings_renew_windows_trial_licence_checkbox.Checked = false;
                    }

                    //Cookies Sync
                    if (cookies_sync_enabled == 1)
                    {
                        cookies_sync_checkbox.Checked = true;
                    }
                    else
                    {
                        cookies_sync_checkbox.Checked = false;
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Client Loader: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Delete Group from Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;

                return;
            }

            player_menu_tabcontrol.Enabled = true;
            metroTabControl3.Enabled = true;

            this.Alert("Successfully loaded.", Helper.Form_Alert.enmType.Success);
        }

        //Save Settings from Client to Database
        private void client_save_settings_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                //Save Hive Name
                try
                {
                    string statement_autorestart_time_sql = "UPDATE `" + mysql_database + "`.`clients` SET `hive_name`='" + clients_general_settings_hive_combobox.SelectedItem.ToString() + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "';";
                    MySqlCommand statement_autorestart_time_cmd = new MySqlCommand(statement_autorestart_time_sql, conn);
                    statement_autorestart_time_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Auto Restart State
                try
                {
                    if (client_auto_restart_enabled_checkbox.Checked)
                    {
                        string statement_autorestart_sql = "UPDATE `" + mysql_database + "`.`clients` SET `auto_restart_bot`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand statement_autorestart_cmd = new MySqlCommand(statement_autorestart_sql, conn);
                        statement_autorestart_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string statement_autorestart_sql = "UPDATE `" + mysql_database + "`.`clients` SET `auto_restart_bot`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand statement_autorestart_cmd = new MySqlCommand(statement_autorestart_sql, conn);
                        statement_autorestart_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Firefox Cookies Sync
                try
                {
                    if (cookies_sync_checkbox.Checked)
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `cookies_sync_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `cookies_sync_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Enable Persistant Chrome Profiles
                try
                {
                    if (client_persistent_chrome_profile_checkbox.Checked)
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `chrome_persistent_profiles_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `chrome_persistent_profiles_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Credentials Bound to Openvpn Profile
                try
                {
                    if (client_general_settings_bind_to_credential_checkbox.Checked)
                    {
                        string statement_autorestart_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_bound_credentials_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand statement_autorestart_cmd = new MySqlCommand(statement_autorestart_sql, conn);
                        statement_autorestart_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string statement_autorestart_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_bound_credentials_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand statement_autorestart_cmd = new MySqlCommand(statement_autorestart_sql, conn);
                        statement_autorestart_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Auto Restart Timer
                try
                {
                    string statement_autorestart_time_sql = "UPDATE `" + mysql_database + "`.`clients` SET `auto_restart_bot_timer`='" + Convert.ToInt32(client_auto_restart_time_textbox.Text) * 3600000 + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand statement_autorestart_time_cmd = new MySqlCommand(statement_autorestart_time_sql, conn);
                    statement_autorestart_time_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save OpenVPN State
                try
                {
                    if (client_openvpn_enabled_checkbox.Checked)
                    {
                        string statement_openvpn_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand statement_openvpn_enabled_cmd = new MySqlCommand(statement_openvpn_enabled_sql, conn);
                        statement_openvpn_enabled_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string statement_openvpn_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand statement_openvpn_enabled_cmd = new MySqlCommand(statement_openvpn_enabled_sql, conn);
                        statement_openvpn_enabled_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save OpenVPN Randomize State
                try
                {
                    if (client_openvpn_randomize_checkbox.Checked)
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_randomize`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_randomize`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save OpenVPN Healthcheck State
                try
                {
                    if (client_openvpn_health_status_checkbox.Checked)
                    {
                        string openvpn_health_status_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_health_check`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand openvpn_health_status_cmd = new MySqlCommand(openvpn_health_status_sql, conn);
                        openvpn_health_status_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string openvpn_health_status_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_health_check`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand openvpn_health_status_cmd = new MySqlCommand(openvpn_health_status_sql, conn);
                        openvpn_health_status_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save OpenVPN Profile
                try
                {
                    string statement_sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_profile`='" + client_openvpn_selected_profile_combobox.Text + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand statement_cmd = new MySqlCommand(statement_sql, conn);
                    statement_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Spotify Toogle State
                try
                {
                    if (client_spotify_enabled_toggle.Checked)
                    {
                        string client_spotify_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_spotify_enabled_cmd = new MySqlCommand(client_spotify_enabled_sql, conn);
                        client_spotify_enabled_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Spotify Playlist 
                try
                {
                    string client_spotify_playlist_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_playlist_url`='" + client_spotify_playlist_combobox.Text + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_spotify_playlist_cmd = new MySqlCommand(client_spotify_playlist_sql, conn);
                    client_spotify_playlist_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Spotify Credential Username
                try
                {
                    string client_spotify_credential_username_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_account`='" + client_spotify_selected_credentials_combobox.Text + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_spotify_credential_username_cmd = new MySqlCommand(client_spotify_credential_username_sql, conn);
                    client_spotify_credential_username_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Cookies to Client
                try
                {
                    string _sql = "UPDATE `" + mysql_database + "`.`clients` SET `cookies_file`='" + cookies_file_encoded + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(_sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Cookies File to Client: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Spotify Credential used by Client Name
                try
                {
                    string sql = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `client_name`='" + clients_listbox.SelectedItem.ToString() + "' WHERE `username`='" + client_spotify_selected_credentials_combobox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Spotify Credential Password
                try
                {
                    string client_spotify_credential_password_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_password`='" + StringCipher.Encrypt(client_spotify_selected_credentials_password_textbox.Text, Settings.Encryption_Key) + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_spotify_credential_password_cmd = new MySqlCommand(client_spotify_credential_password_sql, conn);
                    client_spotify_credential_password_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }


                //Save Spotify Auto Skip State
                try
                {
                    if (client_spotify_autoskip_forward_enabled_checkbox.Checked)
                    {
                        string client_spotify_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_autoskip_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_spotify_enabled_cmd = new MySqlCommand(client_spotify_enabled_sql, conn);
                        client_spotify_enabled_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_autoskip_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Spotify Auto Skip From Int.
                try
                {
                    string client_spotify_credential_password_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_autoskip_forward_from`='" + Convert.ToInt32(client_spotify_autoskip_forward_from_textbox.Text) * 1000 + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_spotify_credential_password_cmd = new MySqlCommand(client_spotify_credential_password_sql, conn);
                    client_spotify_credential_password_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Spotify Auto Skip To Int.
                try
                {
                    string client_spotify_credential_password_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_autoskip_forward_to`='" + Convert.ToInt32(client_spotify_autoskip_to_textbox.Text) * 1000 + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_spotify_credential_password_cmd = new MySqlCommand(client_spotify_credential_password_sql, conn);
                    client_spotify_credential_password_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save deezer Toogle State
                try
                {
                    if (client_deezer_enabled_toggle.Checked)
                    {
                        string client_deezer_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_deezer_enabled_cmd = new MySqlCommand(client_deezer_enabled_sql, conn);
                        client_deezer_enabled_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save deezer Playlist 
                try
                {
                    string client_deezer_playlist_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_playlist_url`='" + client_deezer_playlist_combobox.Text + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_deezer_playlist_cmd = new MySqlCommand(client_deezer_playlist_sql, conn);
                    client_deezer_playlist_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save deezer Credential Username
                try
                {
                    string client_deezer_credential_username_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_player_account`='" + client_deezer_selected_credentials_combobox.Text + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_deezer_credential_username_cmd = new MySqlCommand(client_deezer_credential_username_sql, conn);
                    client_deezer_credential_username_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save deezer Credential Password
                try
                {
                    string client_deezer_credential_password_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_player_password`='" + StringCipher.Encrypt(client_deezer_selected_credentials_password_textbox.Text, Settings.Encryption_Key) + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_deezer_credential_password_cmd = new MySqlCommand(client_deezer_credential_password_sql, conn);
                    client_deezer_credential_password_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save deezer Auto Skip State
                try
                {
                    if (client_deezer_autoskip_forward_enabled_checkbox.Checked)
                    {
                        string client_deezer_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_autoskip_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_deezer_enabled_cmd = new MySqlCommand(client_deezer_enabled_sql, conn);
                        client_deezer_enabled_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_autoskip_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save deezer Auto Skip From Int.
                try
                {
                    string client_deezer_credential_password_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_autoskip_forward_from`='" + Convert.ToInt32(client_deezer_autoskip_forward_from_textbox.Text) * 1000 + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_deezer_credential_password_cmd = new MySqlCommand(client_deezer_credential_password_sql, conn);
                    client_deezer_credential_password_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save deezer Auto Skip To Int.
                try
                {
                    string client_deezer_credential_password_sql = "UPDATE `" + mysql_database + "`.`clients` SET `deezer_autoskip_forward_to`='" + Convert.ToInt32(client_deezer_autoskip_to_textbox.Text) * 1000 + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_deezer_credential_password_cmd = new MySqlCommand(client_deezer_credential_password_sql, conn);
                    client_deezer_credential_password_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Check if custom profile checkbox is checked
                try
                {
                    if (clients_general_settings_custom_profile_checkbox.Checked)
                    {
                        string client_deezer_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `custom_profile_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_deezer_enabled_cmd = new MySqlCommand(client_deezer_enabled_sql, conn);
                        client_deezer_enabled_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `custom_profile_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Custom Profile to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Player Type
                try
                {
                    string client_player_type_sql = "UPDATE `" + mysql_database + "`.`clients` SET `player_type`='" + client_player_type_combobox.Text + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_player_type_cmd = new MySqlCommand(client_player_type_sql, conn);
                    client_player_type_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Player Type to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Check if Reinstall Chrome enabled
                try
                {
                    if (client_reinstall_chrome_checkbox.Checked)
                    {
                        string client_deezer_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `reinstall_chrome`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_deezer_enabled_cmd = new MySqlCommand(client_deezer_enabled_sql, conn);
                        client_deezer_enabled_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `reinstall_chrome`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Reinstall Chrome to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Check if Reinstall Firefox enabled
                try
                {
                    if (client_reinstall_chrome_checkbox.Checked)
                    {
                        string client_deezer_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `reinstall_firefox`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_deezer_enabled_cmd = new MySqlCommand(client_deezer_enabled_sql, conn);
                        client_deezer_enabled_cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `reinstall_firefox`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                        client_openvpn_randomize_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Reinstall Firefox to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Spotify Rotate Credentials
                try
                {
                    if (client_spotify_rotate_credentials_checkbox.Checked)
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `rotate_credentials_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `rotate_credentials_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Spotify Rotate Credentials: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Emulated Device
                try
                {
                    if (client_emulate_device_checkbox.Checked)
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `emulate_device_enabled`='1', `emulate_device_type`='" + client_emulate_device_combobox.SelectedItem.ToString() + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `emulate_device_enabled`='0', `emulate_device_type`='" + client_emulate_device_combobox.SelectedItem.ToString() + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Emulate Device Type: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save renew windows trial licence
                try
                {
                    if (clients_general_settings_renew_windows_trial_licence_checkbox.Checked)
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `renew_windows_trial_licence`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `renew_windows_trial_licence`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Renew Windows Trial Licence: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Save Automation Mode
                try
                {
                    string client_player_type_sql = "UPDATE `" + mysql_database + "`.`clients` SET `automation_mode`='" + clients_general_settings_automation_mode_combobox.SelectedItem.ToString() + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_player_type_cmd = new MySqlCommand(client_player_type_sql, conn);
                    client_player_type_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Automation Mode to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Safe Time Sheduler Settings
                try
                {
                    int monday = 0;
                    int tuesday = 0;
                    int wednesday = 0;
                    int thursday = 0;
                    int friday = 0;
                    int saturday = 0;
                    int sunday = 0;

                    if (client_play_time_sheduler_monday_checkbox.Checked)
                        monday = 1;

                    if (client_play_time_sheduler_tuesday_checkbox.Checked)
                        tuesday = 1;

                    if (client_play_time_sheduler_wednesday_checkbox.Checked)
                        wednesday = 1;

                    if (client_play_time_sheduler_thursday_checkbox.Checked)
                        thursday = 1;

                    if (client_play_time_sheduler_friday_checkbox.Checked)
                        friday = 1;

                    if (client_play_time_sheduler_saturday_checkbox.Checked)
                        saturday = 1;

                    if (client_play_time_sheduler_sunday_checkbox.Checked)
                        sunday = 1;

                    string client_player_type_sql = "UPDATE `" + mysql_database + "`.`clients` SET `play_time_sheduler_monday_enabled`='" + monday + "', `play_time_sheduler_tuesday_enabled`='" + tuesday + "', `play_time_sheduler_wednesday_enabled`='" + wednesday + "', `play_time_sheduler_thursday_enabled`='" + thursday + "', `play_time_sheduler_friday_enabled`='" + friday + "', `play_time_sheduler_saturday_enabled`='" + saturday + "', `play_time_sheduler_sunday_enabled`='" + sunday + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand client_player_type_cmd = new MySqlCommand(client_player_type_sql, conn);
                    client_player_type_cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Player Type to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                //Check if custom Useragent should be used and set the selected one
                try
                {
                    if (client_useragent_enabled_checkbox.Checked && client_useragent_name_combobox.SelectedIndex == -1)
                    {
                        this.Alert("No useragent selected. Skipped.", Helper.Form_Alert.enmType.Error);
                        client_useragent_enabled_checkbox.Checked = false;
                        return;
                    }
                    else if (client_useragent_name_combobox.SelectedIndex > -1)
                    {
                        if (client_useragent_enabled_checkbox.Checked)
                        {
                            string client_deezer_enabled_sql = "UPDATE `" + mysql_database + "`.`clients` SET `custom_useragent_enabled`='1' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                            MySqlCommand client_deezer_enabled_cmd = new MySqlCommand(client_deezer_enabled_sql, conn);
                            client_deezer_enabled_cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            string client_openvpn_randomize_sql = "UPDATE `" + mysql_database + "`.`clients` SET `custom_useragent_enabled`='0' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                            MySqlCommand client_openvpn_randomize_cmd = new MySqlCommand(client_openvpn_randomize_sql, conn);
                            client_openvpn_randomize_cmd.ExecuteNonQuery();
                        }

                        string client_useragent_string_sql = "UPDATE `" + mysql_database + "`.`clients` SET `custom_useragent`='" + client_useragent_name_combobox.SelectedItem.ToString() + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                        MySqlCommand client_useragent_string_cmd = new MySqlCommand(client_useragent_string_sql, conn);
                        client_useragent_string_cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Custom Useragent Information to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;

                    return;
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;

                return;
            }

            this.Alert("Settings saved.", Helper.Form_Alert.enmType.Success);
        }

        //Select Spotify Credentials
        private void client_spotify_selected_credentials_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            //Load selected credentials password
            try
            {
                string sql = "SELECT password, client_name, premium, cookies_file FROM spotify_credentials WHERE username = '" + client_spotify_selected_credentials_combobox.SelectedItem.ToString() + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        client_spotify_selected_credentials_password_textbox.Text = StringCipher.Decrypt((rdr[0]).ToString(), Settings.Encryption_Key);

                        client_spotify_credentials_in_use_by_client_label.Text = rdr[1].ToString();
                        client_spotify_credentials_premium_label.Text = rdr[2].ToString();

                        cookies_file_encoded = rdr[3].ToString();
                    }
                }
                rdr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        //Load selected deezer credentials
        private void client_deezer_selected_credentials_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            //Load selected credentials password
            try
            {
                string sql = "SELECT password, client_name FROM deezer_credentials WHERE username = '" + client_deezer_selected_credentials_combobox.SelectedItem.ToString() + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        client_deezer_selected_credentials_password_textbox.Text = StringCipher.Decrypt((rdr[0]).ToString(), Settings.Encryption_Key);

                        client_deezer_credentials_in_use_by_client_label.Text = rdr[1].ToString();
                    }
                }
                rdr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void client_spotify_view_credentials_password_button_Click(object sender, EventArgs e)
        {
            if (client_spotify_selected_credentials_password_textbox.UseSystemPasswordChar == true)
            {
                client_spotify_selected_credentials_password_textbox.UseSystemPasswordChar = false;
            }
            else
            {
                client_spotify_selected_credentials_password_textbox.UseSystemPasswordChar = true;
            }
        }

        private void client_deezer_view_credentials_password_button_Click(object sender, EventArgs e)
        {
            if (client_deezer_selected_credentials_password_textbox.UseSystemPasswordChar == true)
            {
                client_deezer_selected_credentials_password_textbox.UseSystemPasswordChar = false;
            }
            else
            {
                client_deezer_selected_credentials_password_textbox.UseSystemPasswordChar = true;
            }
        }

        private void organizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client_bomb_loader();
        }

        private void credentials_spotify_add_credentials_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
                string sql = "INSERT INTO `" + mysql_database + "`.`spotify_credentials` (`username`, `password`) VALUES ('" + credentials_spotify_new_username_textbox.Text + "', '" + StringCipher.Encrypt(credentials_spotify_new_password_textbox.Text, Settings.Encryption_Key) + "')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Credentials added.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }

            /*try
            {
                conn.Open();
                string sql = "INSERT INTO `" + mysql_database + "`.`spotify_credentials` (`username`, `password`, `client_name`) VALUES ('" + credentials_spotify_new_username_textbox.Text + "', '" + StringCipher.Encrypt(credentials_spotify_new_password_textbox.Text, Settings.Encryption_Key) + "', 'None.')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Credentials added.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }*/
        }

        private void credentials_spotify_edit_account_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "SELECT password FROM spotify_credentials WHERE username = '" + credentials_spotify_edit_account_combobox.SelectedItem.ToString() + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        credentials_spotify_edit_password_textbox.Text = StringCipher.Decrypt((rdr[0]).ToString(), Settings.Encryption_Key);
                    }
                }
                rdr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_spotify_edit_credentials_button_Click(object sender, EventArgs e)
        {
            string current_account = credentials_spotify_edit_account_combobox.SelectedItem.ToString();

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql_client_selected_credentials_client_name1 = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `password`='" + StringCipher.Encrypt(credentials_spotify_edit_password_textbox.Text, Settings.Encryption_Key) + "' WHERE username = '" + credentials_spotify_edit_account_combobox.Text + "'";
                MySqlCommand cmd_sql_client_selected_credentials_client_name1 = new MySqlCommand(sql_client_selected_credentials_client_name1, conn);
                cmd_sql_client_selected_credentials_client_name1.ExecuteNonQuery();

                string sql_client_selected_credentials_client_name_where_client = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_password`='" + StringCipher.Encrypt(credentials_spotify_edit_password_textbox.Text, Settings.Encryption_Key) + "' WHERE spotify_player_account = '" + credentials_spotify_edit_account_combobox.Text + "'";
                MySqlCommand cmd_sql_client_selected_credentials_client_name_where_client = new MySqlCommand(sql_client_selected_credentials_client_name_where_client, conn);
                cmd_sql_client_selected_credentials_client_name_where_client.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                credentials_spotify_edit_account_combobox.SelectedIndex = credentials_spotify_edit_account_combobox.FindStringExact(current_account);

                this.Alert("Credentials edited.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_spotify_remove_credentials_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `" + mysql_database + "`.`spotify_credentials` WHERE `username`= '" + credentials_spotify_account_combobox.Text + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                credentials_spotify_account_combobox.Items.Remove(credentials_spotify_account_combobox.Text);
                credentials_spotify_edit_account_combobox.Items.Remove(credentials_spotify_account_combobox.Text);

                this.Alert("Credentials deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_spotify_add_playlist_button_Click(object sender, EventArgs e)
        {
            int index = credentials_spotify_add_playlist_hive_filter_combobox.SelectedIndex;

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                if (credentials_spotify_add_album_checkbox.Checked)
                {
                    string sql = "INSERT INTO `" + mysql_database + "`.`spotify_playlists` (`url`, `type`, `hive_name`) VALUES ('" + credentials_spotify_new_playlist_textbox.Text + "', 'Album', '" + credentials_spotify_add_playlist_hive_filter_combobox.SelectedItem.ToString() + "')";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string sql = "INSERT INTO `" + mysql_database + "`.`spotify_playlists` (`url`, `type`, `hive_name`) VALUES ('" + credentials_spotify_new_playlist_textbox.Text + "', 'Playlist', '" + credentials_spotify_add_playlist_hive_filter_combobox.SelectedItem.ToString() + "')";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }

                client_bomb_loader();

                conn.Close();

                credentials_spotify_add_playlist_hive_filter_combobox.SelectedIndex = index;

                this.Alert("Playlist added.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_spotify_remove_playlist_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `" + mysql_database + "`.`spotify_playlists` WHERE `url`= '" + credentials_spotify_remove_playlist_combobox.Text + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                credentials_spotify_remove_playlist_combobox.Items.Remove(credentials_spotify_remove_playlist_combobox.Text);

                this.Alert("Playlist deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_deezer_add_credentials_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
                string sql = "INSERT INTO `" + mysql_database + "`.`deezer_credentials` (`username`, `password`, `client_name`) VALUES ('" + credentials_deezer_new_username_textbox.Text + "', '" + StringCipher.Encrypt(credentials_deezer_new_password_textbox.Text, Settings.Encryption_Key) + "', 'None.')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Credentials added.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_deezer_edit_credentials_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql_client_selected_credentials_client_name1 = "UPDATE `" + mysql_database + "`.`deezer_credentials` SET `password`='" + StringCipher.Encrypt(credentials_deezer_edit_password_textbox.Text, Settings.Encryption_Key) + "' WHERE username = '" + credentials_deezer_edit_account_combobox.Text + "'";
                MySqlCommand cmd_sql_client_selected_credentials_client_name1 = new MySqlCommand(sql_client_selected_credentials_client_name1, conn);
                cmd_sql_client_selected_credentials_client_name1.ExecuteNonQuery();

                string sql_client_selected_credentials_client_name_where_client = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_password`='" + StringCipher.Encrypt(credentials_deezer_edit_password_textbox.Text, Settings.Encryption_Key) + "' WHERE spotify_player_account = '" + credentials_deezer_edit_account_combobox.Text + "'";
                MySqlCommand cmd_sql_client_selected_credentials_client_name_where_client = new MySqlCommand(sql_client_selected_credentials_client_name_where_client, conn);
                cmd_sql_client_selected_credentials_client_name_where_client.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Credentials edited.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_deezer_remove_credentials_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `" + mysql_database + "`.`deezer_credentials` WHERE `username`= '" + credentials_deezer_account_combobox.Text + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                credentials_deezer_account_combobox.Items.Remove(credentials_deezer_account_combobox.Text);
                credentials_deezer_edit_account_combobox.Items.Remove(credentials_deezer_account_combobox.Text);

                this.Alert("Credentials deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_deezer_add_playlist_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "INSERT INTO `" + mysql_database + "`.`deezer_playlists` (`url`) VALUES ('" + credentials_deezer_new_playlist_textbox.Text + "')";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Playlist added.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void credentials_deezer_remove_playlist_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `" + mysql_database + "`.`deezer_playlists` WHERE `url`= '" + credentials_deezer_remove_playlist_combobox.Text + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                credentials_deezer_remove_playlist_combobox.Items.Remove(credentials_deezer_remove_playlist_combobox.Text);

                this.Alert("Playlist deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void client_search_client_listview_textbox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                clients_listbox.Items.Clear();

                string sql;

                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();

                //Settings SQL Loader
                if (clients_hive_filter_combobox.SelectedIndex == -1)
                {
                    sql = "SELECT client_name FROM clients WHERE client_name LIKE '%" + client_search_client_listview_textbox.Text + "%';";
                }
                else
                {
                    sql = "SELECT client_name FROM clients WHERE client_name LIKE '%" + client_search_client_listview_textbox.Text + "%' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                }

                //Search Client via Name
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            clients_listbox.Items.Add(rdr[0].ToString());
                        }
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Search Client: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Open MySQL Connection: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void client_delete_client_button_Click(object sender, EventArgs e)
        {

            if (clients_listbox.SelectedItems.Count == 0)
            {
                this.Alert("No client selected.", Helper.Form_Alert.enmType.Error);
                return;
            }

            try
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure to delete the client?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    MySqlConnection conn = new MySqlConnection(connStr);
                    conn.Open();

                    string sql = "DELETE FROM `clients` WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "' AND hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();

                    clients_listbox.Items.Clear();
                    client_overview_loader();
                    player_menu_tabcontrol.Enabled = false;

                    this.Alert("Client deleted successfully.", Helper.Form_Alert.enmType.Success);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Delete Client: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void openvpn_profiles_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < openvpn_profiles_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(187, 1, 18) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = openvpn_profiles_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, openvpn_profiles_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void openvpn_to_mysql_button_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + @"\OpenVPN_Profiles\login.conf") == false)
            {
                this.Alert("Please add login.conf", Helper.Form_Alert.enmType.Error);
                return;
            }

            try
            {
                foreach (var profile in Directory.GetFiles(Application.StartupPath + @"\OpenVPN_Profiles"))
                {
                    string file_name = Path.GetFileName(Application.StartupPath + @"\OpenVPN_Profiles\" + profile);

                    if (file_name != "login.conf" && file_name != "loginipvanish.conf" && file_name != "ca.ipvanish.com.crt" && file_name != "login.ovpn")
                    {
                        File.AppendAllText(profile, "block-outside-dns" + Environment.NewLine + "auth-user-pass login.conf");
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Append VPN Profile Settings: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure to overwrite all existing profiles?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    string openvpn_profile_path = Application.StartupPath + @"\OpenVPN_Profiles\";

                    conn.Open();

                    string truncate_table = "TRUNCATE openvpn_profiles";
                    MySqlCommand truncate_cmd = new MySqlCommand(truncate_table, conn);
                    truncate_cmd.ExecuteNonQuery();

                    Byte[] bytes_login_conf = File.ReadAllBytes(openvpn_profile_path + "login.conf");
                    string converted_login_conf = Convert.ToBase64String(bytes_login_conf);

                    Byte[] bytes_login_conf_ipvanish = File.ReadAllBytes(openvpn_profile_path + "loginipvanish.conf");
                    string converted_login_conf_ipvanish = Convert.ToBase64String(bytes_login_conf_ipvanish);

                    Byte[] bytes_certificate = File.ReadAllBytes(openvpn_profile_path + "ca.ipvanish.com.crt");
                    string converted_certificate = Convert.ToBase64String(bytes_certificate);

                    foreach (string openvpn_profile_file in Directory.EnumerateFiles(openvpn_profile_path, "*.ovpn"))
                    {
                        {
                            try
                            {
                                var openvpn_profile_file_onlyFileName = System.IO.Path.GetFileName(openvpn_profile_file);

                                Byte[] bytes_openvpn_profile_data = File.ReadAllBytes(openvpn_profile_file);
                                string converted_profile = Convert.ToBase64String(bytes_openvpn_profile_data);

                                string sql = "INSERT INTO `" + mysql_database + "`.`openvpn_profiles` (`profile_name`, `profile_data`, `login_conf`, `certificate`, `login_conf_ipvanish`) VALUES ('" + openvpn_profile_file_onlyFileName + "', '" + converted_profile + "', '" + converted_login_conf + "', '" + converted_certificate + "', '" + converted_login_conf_ipvanish + "');";
                                MySqlCommand cmd = new MySqlCommand(sql, conn);
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                conn.Close();
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }
                    conn.Close();

                    client_bomb_loader();

                    this.Alert("OpenVPN Profiles added.", Helper.Form_Alert.enmType.Success);
                }
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void openvpn_remove_selected_profile_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                string profile_to_remove = openvpn_profiles_listbox.SelectedItem.ToString();

                conn.Open();

                string sql = "DELETE FROM `" + mysql_database + "`.`openvpn_profiles` WHERE  `profile_name`='" + profile_to_remove + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                credentials_deezer_remove_playlist_combobox.Items.Remove(credentials_deezer_remove_playlist_combobox.Text);

                this.Alert("OpenVPN Profile deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void openvpn_remove_all_profiles_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                string profile_to_remove = openvpn_profiles_listbox.SelectedItem.ToString();

                conn.Open();

                string truncate_table = "TRUNCATE openvpn_profiles";
                MySqlCommand truncate_cmd = new MySqlCommand(truncate_table, conn);
                truncate_cmd.ExecuteNonQuery();

                conn.Close();

                credentials_deezer_remove_playlist_combobox.Items.Remove(credentials_deezer_remove_playlist_combobox.Text);

                this.Alert("OpenVPN Profiles deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void account_generator_spotify_start_instance_button_Click(object sender, EventArgs e)
        {
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

            string namescheap_username = settings_namescheap_username_textbox.Text;
            string namescheap_password = settings_namescheap_password_textbox.Text;
            string mail_password = settings_artist_manager_mail_password_textbox.Text;

            int distributor_type = account_generator_distributor_selection_combobox.SelectedIndex;

            //Set Distributor Details
            string artist_name = account_generator_distributor_artist_name_textbox.Text;
            string dis_mail = account_generator_distributor_mail_textbox.Text;
            string dis_password = account_generator_distributor_password_textbox.Text;
            string first_name = account_generator_distributor_first_name_textbox.Text;
            string last_name = account_generator_distributor_last_name_textbox.Text;
            string country = account_generator_distributor_country_combobox.Text;
            string phone_number = account_generator_distributor_phone_number_textbox.Text;
            string hdykau = account_generator_distributor_hdykau_combobox.Text;
            string street_number = account_generator_distributor_street_number_textbox.Text;
            string city = account_generator_distributor_plz_city_textbox.Text;

            if (account_generator_distributor_selection_combobox.SelectedItem.ToString() == "Recordjet")
            {
                MethodInvoker mk = delegate
                {
                    ChromeOptions options = new ChromeOptions();

                    options.AddArguments("--start-maximized");
                    options.AddArguments("--disable-notifications");
                    options.AddArguments("--disable-gpu");
                    options.AddArguments("--disable-infobars");
                    options.AddArguments("--disable-blink-features=AutomationControlled");
                    options.AddAdditionalCapability("useAutomationExtension", false);
                    options.AddExcludedArgument("enable-automation");

                    //Check if custom plugins should be loaded
                    if (settings_chrome_default_profile_checkbox.Checked)
                    {
                        options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                    }
                    else
                    {
                        options.AddArguments("--incognito");
                    }

                    IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                    hide_chromedriver();

                    if (account_generator_distributor_selection_combobox.SelectedItem.ToString() == "Recordjet")
                    {
                        driver.Navigate().GoToUrl("https://recordjet.com");

                        Thread.Sleep(5000);

                        try
                        {
                            driver.FindElement(By.Id("CookieBoxSaveButton")).Click();
                        }
                        catch
                        {

                        }

                        Thread.Sleep(5000);

                        driver.FindElement(By.XPath("//span[text()='Passagier werden']")).Click();

                        Thread.Sleep(2000);

                        driver.FindElement(By.Name("artistName")).SendKeys(artist_name);
                        driver.FindElement(By.Name("email")).SendKeys(dis_mail);
                        driver.FindElement(By.Name("password1")).SendKeys(dis_password);
                        driver.FindElement(By.Name("firstname")).SendKeys(first_name);
                        driver.FindElement(By.Name("lastname")).SendKeys(last_name);
                        driver.FindElement(By.Name("idCountry")).SendKeys(country);
                        driver.FindElement(By.Name("phoneNumber")).SendKeys(phone_number);
                        driver.FindElement(By.Name("know_about_us")).SendKeys(hdykau);
                        driver.FindElement(By.Name("accept_terms")).Click();
                        driver.FindElement(By.Name("accept_privacy")).Click();

                        Thread.Sleep(2000);

                        driver.FindElement(By.XPath("//a[contains(@class,'btn_green login-submit')]")).Click();

                        Thread.Sleep(5000);

                        driver.Navigate().GoToUrl("https://cockpit.recordjet.com/my-profile");

                        MessageBox.Show("Continue", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        driver.FindElement(By.XPath("//a[@href='#my_profile']")).Click();
                        driver.FindElement(By.Name("streetNumber")).SendKeys(street_number);
                        driver.FindElement(By.Name("postalCodeCity")).SendKeys(city);
                        driver.FindElement(By.LinkText("Save")).Click();
                    }

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            else if (account_generator_distributor_selection_combobox.SelectedItem.ToString() == "RecordUnion")
            {
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

                    hide_chromedriver();

                    driver.Navigate().GoToUrl("https://recordunion.com/signup");

                    Thread.Sleep(2000);

                    driver.FindElement(By.Id("signupArtistName")).SendKeys(artist_name);
                    driver.FindElement(By.XPath("//span[text()='Choose a country...']")).Click();
                    driver.FindElement(By.XPath("//input[@placeholder='Type country...']")).SendKeys("Germany");

                    Thread.Sleep(2000);
                    driver.FindElement(By.XPath("//li[@data-option-array-index='84']")).Click();

                    driver.FindElement(By.Id("signupEmail")).SendKeys(dis_mail);
                    driver.FindElement(By.Id("signupPassword")).SendKeys(dis_password);
                    driver.FindElement(By.XPath("//label[@class='check-label unchecked']")).Click();

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            else if (account_generator_distributor_selection_combobox.SelectedItem.ToString() == "CD Baby")
            {
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

                    hide_chromedriver();

                    driver.Navigate().GoToUrl("https://members.cdbaby.com/sign-up");

                    Thread.Sleep(5000);

                    driver.FindElement(By.Id("ctl00_MainContent_txtFirstName")).SendKeys(first_name);
                    driver.FindElement(By.Id("ctl00_MainContent_txtLastName")).SendKeys(last_name);
                    driver.FindElement(By.Id("ctl00_MainContent_txtEmail")).SendKeys(dis_mail);
                    driver.FindElement(By.Id("ctl00_MainContent_txtEmailConfirm")).SendKeys(dis_mail);
                    driver.FindElement(By.Id("ctl00_MainContent_txtAddressLine1")).SendKeys(street_number);
                    driver.FindElement(By.Id("ctl00_MainContent_txtCity")).SendKeys(city);
                    //driver.FindElement(By.Id("ctl00_MainContent_ddlCountry")).SendKeys("Germany");
                    //driver.FindElement(By.Id("ctl00_MainContent_ddlState")).SendKeys("Nordrhein-Westfalen");
                    driver.FindElement(By.Id("ctl00_MainContent_txtPhone")).SendKeys(phone_number);
                    driver.FindElement(By.Id("ctl00_MainContent_txtUsername")).SendKeys(artist_name);
                    driver.FindElement(By.Id("ctl00_MainContent_txtPassword")).SendKeys(dis_password);
                    driver.FindElement(By.Id("ctl00_MainContent_txtPasswordConf")).SendKeys(dis_password);

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);
            }
        }

        private void settings_namescheap_save_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string encrypted_telegram_api_token = StringCipher.Encrypt(settings_namescheap_username_textbox.Text, Settings.Encryption_Key);

                string telegram_store_api_token_sql = "UPDATE `" + mysql_database + "`.`settings` SET `namescheap_username`='" + encrypted_telegram_api_token + "', `namescheap_password`='" + StringCipher.Encrypt(settings_namescheap_password_textbox.Text, Settings.Encryption_Key) + "';";
                MySqlCommand telegram_store_api_token_sql_cmd = new MySqlCommand(telegram_store_api_token_sql, conn);
                telegram_store_api_token_sql_cmd.ExecuteNonQuery();

                conn.Close();

                this.Alert("Saved.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
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
        }

        private void playlist_creator_song_url_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < playlist_creator_spotify_song_url_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(187, 1, 18) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = playlist_creator_spotify_song_url_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, playlist_creator_spotify_song_url_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void playlist_creator_start_instance_button_Click(object sender, EventArgs e)
        {
            foreach (var account in playlist_creator_spotify_credentials_combobox.Items)
            {
                playlist_creator_spotify_shuffle_songs_button.PerformClick();

                Thread.Sleep(2000);

                string mail_username = playlist_creator_spotify_credentials_combobox.SelectedItem.ToString();
                string mail_password = playlist_creator_spotify_selected_credentials_password_textbox.Text;
                string playlist_name = playlist_creator_spotify_playlist_name_combobox.Text;

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

                hide_chromedriver();

                try
                {
                    driver.Navigate().GoToUrl("https://accounts.spotify.com/login");
                }
                catch
                {

                }

                bool start_login_flag = true;
                int start_login_registercount = 0;
                int start_login_registermaxTries = 90;
                while (start_login_flag == true)
                {
                    try
                    {
                        driver.FindElement(By.Id("login-username")).SendKeys(mail_username);
                        driver.FindElement(By.Id("login-password")).SendKeys(mail_password);
                        start_login_flag = false;
                    }
                    catch
                    {
                        // handle exception
                        if (++start_login_registercount == start_login_registermaxTries)
                        {
                            MessageBox.Show("Creating playlist failed.", "Playlist Creator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        Thread.Sleep(1000);
                    }
                }

                bool finish_login_flag = true;
                int finish_login_registercount = 0;
                int finish_loginr_registermaxTries = 90;
                while (finish_login_flag == true)
                {
                    try
                    {
                        driver.FindElement(By.Id("login-button")).Click();
                        finish_login_flag = false;
                    }
                    catch
                    {
                        // handle exception
                        if (++finish_login_registercount == finish_loginr_registermaxTries)
                        {
                            MessageBox.Show("Creating playlist failed.", "Playlist Creator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            driver.Dispose();
                        }
                        Thread.Sleep(1000);
                    }
                }

                bool check_login_flag = true;
                int check_login_registercount = 0;
                int check_login_registermaxTries = 60;
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
                            MessageBox.Show("Creating playlist failed.", "Playlist Creator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            driver.Dispose();
                        }
                        Thread.Sleep(1000);
                    }
                }

                try
                {
                    driver.Navigate().GoToUrl("https://open.spotify.com/search");
                }
                catch
                {

                }

                Thread.Sleep(5000);

                try
                {
                    driver.FindElement(By.Id("onetrust-accept-btn-handler")).Click();
                }
                catch
                {

                }

                Thread.Sleep(2500);

                try
                {
                    driver.FindElement(By.XPath("//div[contains(@class,'GlueDropTarget GlueDropTarget--albums')]//button[1]")).Click();
                }
                catch
                {

                }

                Thread.Sleep(5000);

                try
                {
                    driver.FindElement(By.XPath("//button[@type='button']//h1[1]")).Click();
                }
                catch
                {

                }

                Thread.Sleep(2000);

                try
                {
                    driver.FindElement(By.XPath("//input[@data-testid='playlist-edit-details-name-input']")).Click();
                    driver.FindElement(By.XPath("//input[@data-testid='playlist-edit-details-name-input']")).Clear();
                    driver.FindElement(By.XPath("//input[@data-testid='playlist-edit-details-name-input']")).SendKeys(playlist_creator_spotify_playlist_name_combobox.Text);
                    driver.FindElement(By.XPath("//button[@data-testid='playlist-edit-details-save-button']")).Click();
                }
                catch
                {

                }

                Thread.Sleep(1000);

                foreach (var item in playlist_creator_spotify_song_url_listbox.Items)
                {
                    try
                    {
                        driver.Navigate().GoToUrl(item.ToString());

                        Thread.Sleep(5000);

                        try
                        {
                            driver.FindElement(By.XPath("(//button[@title='Play'])[2]")).Click();
                        }
                        catch
                        {

                        }

                        try
                        {
                            driver.FindElement(By.XPath("(//button[@title='Play'])[3]")).Click();
                        }
                        catch
                        {

                        }

                        Thread.Sleep(2000);

                        try
                        {
                            //var song_name = driver.FindElement(By.XPath("//a[@data-testid='nowplaying-track-link']"));
                            //MessageBox.Show(song_name.ToString());

                            var context_click = driver.FindElement(By.XPath("//a[@data-testid='nowplaying-track-link']"));
                            Actions actions = new Actions(driver);
                            actions.ContextClick(context_click).Perform();
                            Thread.Sleep(1500);
                            driver.FindElement(By.XPath("//span[text()='Zur Playlist hinzufügen']")).Click();
                            Thread.Sleep(1500);
                            driver.FindElement(By.XPath("(//span[text()='" + playlist_creator_spotify_playlist_name_combobox.Text + "'])[2]")).Click();
                        }
                        catch
                        {

                        }
                    }
                    catch
                    {

                    }
                    Thread.Sleep(2000);
                }

                try
                {
                    driver.FindElement(By.XPath("//span[text()='" + playlist_name + "']")).Click();
                }
                catch
                {

                }

                Thread.Sleep(10000);

                string playlist_url = driver.Url;

                MySqlConnection conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();

                    string sql = "INSERT INTO `" + mysql_database + "`.`spotify_playlists` (`url`, `type`, `hive_name`) VALUES ('" + playlist_url + "', 'Playlist', '" + playlist_creator_spotify_selected_hive_combobox.SelectedItem.ToString() + "');";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Insert Playlist to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }

                try
                {
                    foreach (var process in Process.GetProcessesByName("chrome"))
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {

                        }
                    }

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }

                playlist_creator_spotify_credentials_combobox.SelectedIndex++;
                playlist_creator_spotify_playlist_name_combobox.SelectedIndex++;

                Thread.Sleep(5000);
            }
        }

        private void playlist_creator_stop_instance_button_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }

        private void playlist_creator_copy_mail_password_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playlist_creator_spotify_credentials_combobox.Text) == false)
                {
                    Clipboard.SetText(playlist_creator_spotify_credentials_combobox.Text + "\nPassword: " + playlist_creator_spotify_selected_credentials_password_textbox.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void playlist_creator_login_spotify_button_Click(object sender, EventArgs e)
        {
            string mail_username = playlist_creator_spotify_credentials_combobox.SelectedItem.ToString();
            string mail_password = playlist_creator_spotify_selected_credentials_password_textbox.Text;

            // MethodInvoker mk = delegate
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

                hide_chromedriver();

                driver.Navigate().GoToUrl("https://accounts.spotify.com/login");

                driver.FindElement(By.Id("login-username")).SendKeys(mail_username);
                driver.FindElement(By.Id("login-password")).SendKeys(mail_password);
            }
        }

        private void playlist_creator_add_url_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(playlist_creator_spotify_add_url_textbox.Text) == false)
            {
                MySqlConnection conn = new MySqlConnection(connStr);

                try
                {
                    conn.Open();

                    string sql = "INSERT INTO `" + mysql_database + "`.`songs` (`url`, `artist_name`, `song_name`) VALUES ('" + playlist_creator_spotify_add_url_textbox.Text + "', '" + playlist_creator_add_artist_name_textbox.Text + "', '" + playlist_creator_add_song_name_textbox.Text + "');";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();

                    client_bomb_loader();

                    this.Alert("Song URL added.", Helper.Form_Alert.enmType.Success);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }

                playlist_creator_spotify_add_url_textbox.Clear();
            }
            else
            {
                this.Alert("No URL provided.", Helper.Form_Alert.enmType.Error);
            }
        }

        private void playlist_creator_delete_url_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `songs` WHERE `url`='" + playlist_creator_spotify_song_url_listbox.SelectedItem.ToString() + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Song URL deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void playlist_creator_shuffle_songs_button_Click(object sender, EventArgs e)
        {
            playlist_creator_loader();

            ListBox.ObjectCollection list = playlist_creator_spotify_song_url_listbox.Items;
            Random rng = new Random();
            int n = list.Count;
            //begin updating
            playlist_creator_spotify_song_url_listbox.BeginUpdate();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                object value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            playlist_creator_spotify_song_url_listbox.EndUpdate();
            playlist_creator_spotify_song_url_listbox.Invalidate();

            if (playlist_creator_spotify_randomly_remove_songs_checkbox.Checked)
            {
                if (playlist_creator_spotify_randomly_remove_songs_checkbox.Checked)
                {
                    Random random_number = new Random();
                    int random_number_int = random_number.Next(Convert.ToInt32(playlist_creator_spotify_song_removal_randomizer_from_index_textbox.Text), Convert.ToInt32(playlist_creator_spotify_song_removal_randomizer_to_index_textbox.Text));
                    //MessageBox.Show(random_number_int.ToString());

                    int song_max_count = random_number_int;
                    int song_count = 0;

                    while (song_max_count != song_count)
                    {
                        Random random_index = new Random();
                        int random_index_int = random_index.Next(0, playlist_creator_spotify_song_url_listbox.Items.Count);

                        playlist_creator_spotify_song_url_listbox.SelectedIndex = random_index_int;

                        string random_selected_item = playlist_creator_spotify_song_url_listbox.SelectedItem.ToString();

                        playlist_creator_spotify_song_url_listbox.Items.Remove(random_selected_item);

                        song_count++;
                    }
                }
            }

            this.Alert("Shuffled.", Helper.Form_Alert.enmType.Success);
        }

        private void playlist_creator_credentials_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "SELECT password FROM spotify_playlist_accounts WHERE username = '" + playlist_creator_spotify_credentials_combobox.SelectedItem.ToString() + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        string playlist_items = StringCipher.Decrypt((rdr[0]).ToString(), Settings.Encryption_Key);
                        playlist_creator_spotify_selected_credentials_password_textbox.Text = playlist_items;
                    }
                }
                rdr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void playlist_creator_spotify_password_visible_picturebox_Click(object sender, EventArgs e)
        {
            if (playlist_creator_spotify_selected_credentials_password_textbox.UseSystemPasswordChar == true)
            {
                playlist_creator_spotify_selected_credentials_password_textbox.UseSystemPasswordChar = false;
            }
            else
            {
                playlist_creator_spotify_selected_credentials_password_textbox.UseSystemPasswordChar = true;
            }
        }

        private void settings_telegram_send_test_notification_button_Click(object sender, EventArgs e)
        {
            try
            {
                WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + telegram_api_token_textbox.Text + "/sendMessage?chat_id=" + telegram_channelid_textbox.Text + "&text=" + "Test");
                req.UseDefaultCredentials = true;
                var result = req.GetResponse();
                req.Abort();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + telegram_api_token_textbox.Text + "/sendMessage?chat_id=" + telegram_alert_chat_id_textbox.Text + "&text=" + "Test");
                req.UseDefaultCredentials = true;
                var result = req.GetResponse();
                req.Abort();

                this.Alert("Successfully send.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + telegram_api_token_textbox.Text + "/sendMessage?chat_id=" + telegram_alert_creds_chat_id_textbox.Text + "&text=" + "Test");
                req.UseDefaultCredentials = true;
                var result = req.GetResponse();
                req.Abort();

                this.Alert("Successfully send.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void settings_telegram_save_credentials_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                if (telegram_monitoring_checkbox.Checked == true)
                {
                    {
                        try
                        {
                            string sql = "UPDATE `" + mysql_database + "`.`settings` SET `telegram_active`='1'";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");

                            return;
                        }
                    }
                }
                else if (telegram_monitoring_checkbox.Checked == false)
                {
                    {
                        try
                        {
                            string sql = "UPDATE `" + mysql_database + "`.`settings` SET `telegram_active`='0'";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");

                            return;
                        }
                    }
                }

                string encrypted_telegram_api_token = StringCipher.Encrypt(telegram_api_token_textbox.Text, Settings.Encryption_Key);

                string telegram_store_api_token_sql = "UPDATE `" + mysql_database + "`.`settings` SET `telegram_api_token`='" + encrypted_telegram_api_token + "'";
                MySqlCommand telegram_store_api_token_sql_cmd = new MySqlCommand(telegram_store_api_token_sql, conn);
                telegram_store_api_token_sql_cmd.ExecuteNonQuery();

                string encrypted_telegram_chat_id = StringCipher.Encrypt(telegram_channelid_textbox.Text, Settings.Encryption_Key);

                string telegram_channel_id_sql = "UPDATE `" + mysql_database + "`.`settings` SET `telegram_chat_id`='" + encrypted_telegram_chat_id + "'";
                MySqlCommand store_channel_id_cmd = new MySqlCommand(telegram_channel_id_sql, conn);
                store_channel_id_cmd.ExecuteNonQuery();

                string encrypted_telegram_alert_chat_id = StringCipher.Encrypt(telegram_alert_chat_id_textbox.Text, Settings.Encryption_Key);

                string telegram_alert_chat_id_sql = "UPDATE `" + mysql_database + "`.`settings` SET `telegram_alert_chat_id`='" + encrypted_telegram_alert_chat_id + "'";
                MySqlCommand store_telegram_alert_chat_id_sql = new MySqlCommand(telegram_alert_chat_id_sql, conn);
                store_telegram_alert_chat_id_sql.ExecuteNonQuery();

                string encrypted_telegram_alert_creds_chat_id = StringCipher.Encrypt(telegram_alert_creds_chat_id_textbox.Text, Settings.Encryption_Key);

                string telegram_alert_creds_chat_id_sql = "UPDATE `" + mysql_database + "`.`settings` SET `telegram_alert_creds_chat_id`='" + encrypted_telegram_alert_creds_chat_id + "'";
                MySqlCommand store_telegram_alert_creds_chat_id_sql = new MySqlCommand(telegram_alert_creds_chat_id_sql, conn);
                store_telegram_alert_creds_chat_id_sql.ExecuteNonQuery();

                conn.Close();

                this.Alert("Credentials saved.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void logOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var show_login_form = new Login_Form();
            show_login_form.Closed += (s, args) => this.Close();
            show_login_form.Show();
            this.Hide();
        }

        private void AdaptersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAddresses();
        }

        private void UpdateAddresses()
        {
            Adapter a = AdaptersComboBox.SelectedItem as Adapter;
            this.mac_spoofer_real_mac_adress_label.Text = a.Mac;
        }

        private void SetRegistryMac(string mac)
        {
            Adapter a = AdaptersComboBox.SelectedItem as Adapter;

            try
            {
                if (a.SetRegistryMac(mac))
                {
                    System.Threading.Thread.Sleep(100);
                    UpdateAddresses();

                    this.Alert("Successfully spoofshitted.", Helper.Form_Alert.enmType.Success);
                }
            }
            catch
            {
                this.Alert("Failed spoofshit.", Helper.Form_Alert.enmType.Error);
            }
        }

        private void mac_spoofer_spoof_now_button_Click(object sender, EventArgs e)
        {
            string spoofed_mac_adress = Adapter.GetNewMac();

            if (!Adapter.IsValidMac(spoofed_mac_adress, false))
            {
                MessageBox.Show("Entered MAC-address is not valid and will not update.", "Invalid MAC-address specified", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetRegistryMac(spoofed_mac_adress);
        }

        private void mac_spoofer_reset_button_Click(object sender, EventArgs e)
        {
            SetRegistryMac("");
        }

        private void client_spotify_login_mail_button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(settings_client_mail_domain_textbox.Text))
            {
                string mail_account = client_spotify_selected_credentials_combobox.SelectedItem.ToString();
                string mail_password = settings_client_mail_password_textbox.Text;
                string mail_domain = settings_client_mail_domain_textbox.Text;

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

                    hide_chromedriver();

                    driver.Navigate().GoToUrl(mail_domain);

                    bool mail_login_flag = true;
                    int mail_login_tries_count = 0;
                    int mail_login_register_max_tries = 30;
                    while (mail_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.Id("rcmloginuser")).SendKeys(mail_account);
                            driver.FindElement(By.Id("rcmloginpwd")).SendKeys(mail_password);
                            driver.FindElement(By.Id("rcmloginsubmit")).Click();

                            mail_login_flag = false;
                        }
                        catch
                        {
                            if (++mail_login_tries_count == mail_login_register_max_tries)
                            {
                                MessageBox.Show("Mail login could not be completed. Failed after several tries.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                driver.Dispose();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);

                DialogResult dialogResult2 = MessageBox.Show("Do you like to update the credentials of your spotify account?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult2 == DialogResult.Yes)
                {
                    //Generate new random password
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();

                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }

                    var new_password = new String(stringChars);

                    //Save new password to the database
                    MySqlConnection conn = new MySqlConnection(connStr);

                    try
                    {
                        conn.Open();

                        string update_credentials_password_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_account`='" + client_spotify_selected_credentials_combobox.SelectedItem.ToString() + "', `spotify_player_password`='" + StringCipher.Encrypt(new_password, Settings.Encryption_Key) + "' WHERE client_name = '" + clients_listbox.SelectedItem.ToString() + "'";
                        MySqlCommand cmd_update_credentials_password = new MySqlCommand(update_credentials_password_sql, conn);
                        cmd_update_credentials_password.ExecuteNonQuery();

                        string update_credentials_password_sql2 = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `password`='" + StringCipher.Encrypt(new_password, Settings.Encryption_Key) + "' WHERE username = '" + client_spotify_selected_credentials_combobox.Text + "'";
                        MySqlCommand cmd_update_credentials_password2 = new MySqlCommand(update_credentials_password_sql2, conn);
                        cmd_update_credentials_password2.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Player Password Update Routine: " + ex.Message + Environment.NewLine);
                    }

                    client_spotify_selected_credentials_password_textbox.Text = new_password;

                    Clipboard.SetText(new_password);
                    MessageBox.Show("A new password has been copied to your clipboard and saved to your database. Please use the new password.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No Mail Server URL Provided. Please add it.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void client_deezer_login_mail_button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(settings_artist_manager_mail_password_textbox.Text))
            {
                string mail_account = client_deezer_selected_credentials_combobox.Text;
                string mail_password = client_deezer_selected_credentials_password_textbox.Text;

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

                    hide_chromedriver();

                    bool mail_login_flag = true;
                    int mail_login_tries_count = 0;
                    int mail_login_register_max_tries = 30;
                    while (mail_login_flag == true)
                    {
                        try
                        {
                            driver.Navigate().GoToUrl(settings_artist_manager_mail_password_textbox.Text);

                            driver.FindElement(By.Id("rcmloginuser")).SendKeys(mail_account);
                            driver.FindElement(By.Id("rcmloginpwd")).SendKeys(mail_password);
                            driver.FindElement(By.Id("rcmloginsubmit")).Click();

                            mail_login_flag = false;
                        }
                        catch
                        {
                            if (++mail_login_tries_count == mail_login_register_max_tries)
                            {
                                MessageBox.Show("Mail login could not be completed. Failed after several tries.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                driver.Dispose();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);

                DialogResult dialogResult2 = MessageBox.Show("Do you like to update the credentials of your spotify account?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult2 == DialogResult.Yes)
                {
                    //Generate new random password
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();

                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }

                    var new_password = new String(stringChars);

                    //Save new password to the database
                    MySqlConnection conn = new MySqlConnection(connStr);

                    try
                    {
                        conn.Open();

                        string update_credentials_password_sql = "UPDATE `" + mysql_database + "`.`autobot_clients` SET `player_password`='" + StringCipher.Encrypt(new_password, Settings.Encryption_Key) + "' WHERE player_account = '" + clients_listbox.SelectedItem.ToString() + "'";
                        MySqlCommand cmd_update_credentials_password = new MySqlCommand(update_credentials_password_sql, conn);
                        cmd_update_credentials_password.ExecuteNonQuery();

                        string update_credentials_password_sql2 = "UPDATE `" + mysql_database + "`.`autobot_spotify_credentials` SET `password`='" + StringCipher.Encrypt(new_password, Settings.Encryption_Key) + "' WHERE username = '" + clients_listbox.SelectedItem.ToString() + "'";
                        MySqlCommand cmd_update_credentials_password2 = new MySqlCommand(update_credentials_password_sql2, conn);
                        cmd_update_credentials_password2.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Player Password Update Routine: " + ex.Message + Environment.NewLine);
                    }

                    Clipboard.SetText(new_password);
                    MessageBox.Show("A new password has been copied to your clipboard and saved to your database. Please use the new password.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No Mail Server URL Provided. Please add it.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void artist_manager_artists_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < artist_manager_artists_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(102, 102, 102) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = artist_manager_artists_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, artist_manager_artists_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void artist_manager_artists_listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (artist_manager_artists_listbox.SelectedItems.Count == 0)
                {
                    head_tabcontrol.Enabled = true;
                    return;
                }
                else
                {
                    head_tabcontrol.Enabled = false;
                }

                artist_manager_songs_listbox.Items.Clear();

                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();

                //Bot Type Loader
                try
                {
                    //16
                    string sql = "SELECT distributor, release_status, first_name, last_name, mail, password, copyright, country, street_number, plz_city, phone_number, notes, paypal_mail, bank_iban, bank_bic, username, hive_name FROM artist_manager WHERE artist_name = '" + artist_manager_artists_listbox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            artist_manager_distributor_type_combobox.SelectedIndex = artist_manager_distributor_type_combobox.FindStringExact(rdr[0].ToString());
                            artist_manager_release_status_combobox.SelectedIndex = artist_manager_release_status_combobox.FindStringExact(rdr[1].ToString());
                            artists_manager_distributor_first_name_textbox.Text = StringCipher.Decrypt((rdr[2]).ToString(), Settings.Encryption_Key);
                            artists_manager_distributor_last_name_textbox.Text = StringCipher.Decrypt((rdr[3]).ToString(), Settings.Encryption_Key);
                            artists_manager_distributor_mail_textbox.Text = StringCipher.Decrypt((rdr[4]).ToString(), Settings.Encryption_Key);
                            artists_manager_distributor_password_textbox.Text = StringCipher.Decrypt((rdr[5]).ToString(), Settings.Encryption_Key);
                            artist_manager_distributor_copyright_textbox.Text = StringCipher.Decrypt((rdr[6]).ToString(), Settings.Encryption_Key);
                            artist_manager_distributor_country_textbox.Text = StringCipher.Decrypt((rdr[7]).ToString(), Settings.Encryption_Key);
                            artist_manager_distributor_street_a_number_textbox.Text = StringCipher.Decrypt((rdr[8]).ToString(), Settings.Encryption_Key);
                            artist_manager_distributor_plz_city_textbox.Text = StringCipher.Decrypt((rdr[9]).ToString(), Settings.Encryption_Key);
                            artist_manager_distributor_phone_number_textbox.Text = StringCipher.Decrypt((rdr[10]).ToString(), Settings.Encryption_Key);
                            artist_manager_notes_textbox.Text = StringCipher.Decrypt((rdr[11]).ToString(), Settings.Encryption_Key);
                            artists_manager_distributor_paypal_mail_textbox.Text = StringCipher.Decrypt((rdr[12]).ToString(), Settings.Encryption_Key);
                            artist_manager_distributor_bank_iban_textbox.Text = StringCipher.Decrypt((rdr[13]).ToString(), Settings.Encryption_Key);
                            artist_manager_distributor_bank_bic_textbox.Text = StringCipher.Decrypt((rdr[14]).ToString(), Settings.Encryption_Key);
                            artists_manager_distributor_username_textbox.Text = StringCipher.Decrypt((rdr[15]).ToString(), Settings.Encryption_Key);
                            artist_manager_hive_name_combobox.SelectedIndex = artist_manager_hive_name_combobox.FindStringExact(rdr[16].ToString());
                        }
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //Artist Song Loader
                try
                {
                    string sql = "SELECT song_name FROM songs WHERE artist_name = '" + artist_manager_artists_listbox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            string song_items = rdr[0].ToString();
                            artist_manager_songs_listbox.Items.Add(song_items);
                        }
                    }
                    rdr.Close();

                    artist_manager_song_count_label.Text = artist_manager_songs_listbox.Items.Count.ToString();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Artists Information: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }

                //Load Song Overview

                //Fill Playlist Creator Song overview
                try
                {
                    string sql = "SELECT artist_name, song_name, spotify_url FROM songs WHERE artist_name = '" + artist_manager_artists_listbox.SelectedItem.ToString() + "';";

                    MySqlDataAdapter mysql_data_adapter = new MySqlDataAdapter();
                    mysql_data_adapter.SelectCommand = new MySqlCommand(sql, conn);

                    DataTable mysql_table = new DataTable();
                    mysql_data_adapter.Fill(mysql_table);

                    artist_manager_song_overview_datagridview.DataSource = mysql_table;

                    //Change Colum Titles to look more user friendly
                    artist_manager_song_overview_datagridview.Columns[0].HeaderText = "Artist";
                    artist_manager_song_overview_datagridview.Columns[1].HeaderText = "Song";
                    artist_manager_song_overview_datagridview.Columns[2].HeaderText = "Spotify URL";
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Song Overview from Artist: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Fill Payout History in Datagridview
                try
                {
                    string sql = "SELECT datetime, type, value FROM artist_manager_payout_history WHERE artist_name = '" + artist_manager_artists_listbox.SelectedItem.ToString() + "' ORDER BY datetime ASC;";

                    MySqlDataAdapter mysql_data_adapter = new MySqlDataAdapter();
                    mysql_data_adapter.SelectCommand = new MySqlCommand(sql, conn);

                    DataTable mysql_table = new DataTable();
                    mysql_data_adapter.Fill(mysql_table);

                    artist_manager_payout_history_datagridview.DataSource = mysql_table;

                    //Change Colum Titles to look more user friendly
                    artist_manager_payout_history_datagridview.Columns[0].HeaderText = "Datetime";
                    artist_manager_payout_history_datagridview.Columns[1].HeaderText = "Type";
                    artist_manager_payout_history_datagridview.Columns[2].HeaderText = "Value";
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Artist Manager Payout History from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    player_menu_tabcontrol.Enabled = true;
                }

                //Visualize Payout History
                artist_manager_payout_visualisation_chart.Series["Money"].Points.Clear();
                artist_manager_payout_visualisation_chart.ChartAreas[0].AxisX.ScrollBar.Enabled = true;

                //Artist Song Loader
                try
                {
                    string timestamp = null;
                    string value = null;

                    string sql = "SELECT datetime, value FROM artist_manager_payout_history WHERE artist_name = '" + artist_manager_artists_listbox.SelectedItem.ToString() + "' ORDER BY datetime ASC;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            //Initialize Axis Titles
                            artist_manager_payout_visualisation_chart.ChartAreas[0].AxisX.Title = "Date & Time";
                            artist_manager_payout_visualisation_chart.ChartAreas[0].AxisY.Title = "Money €";
                            artist_manager_payout_visualisation_chart.ChartAreas[0].AxisY.LabelStyle.Format = "{##.##}€";

                            timestamp = rdr[0].ToString();
                            value = rdr[1].ToString();

                            //Parse String to Date
                            DateTime Date = DateTime.ParseExact(timestamp, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                            //Convert Date to correct string format
                            string converted_timestamp = "Date: " + Date.ToString("dd.MM.yyyy") + " (Time: " + Date.ToString("HH:mm)");

                            artist_manager_payout_visualisation_chart.Series["Money"].Points.AddXY(converted_timestamp, value);
                        }
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Artists Information: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }

                conn.Close();

                head_tabcontrol.Enabled = true;
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Artists Information: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void artists_manager_add_artist_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(artists_manager_add_artist_textbox.Text))
                {
                    this.Alert("Please define a artist name.", Helper.Form_Alert.enmType.Error);
                }
                else
                {
                    MySqlConnection conn = new MySqlConnection(connStr);
                    {
                        try
                        {
                            conn.Open();

                            string sql = "INSERT INTO `artist_manager` (`artist_name`, `notes`) VALUES ('" + artists_manager_add_artist_textbox.Text + "', 'MbSDPO8YftdqPbgmev786QP+UNUqX90SWbf6TBDZCb0y7HZLLRPeRVoYaLpoUBgD6Y5c5DtUxHWK2ir5+WDkta+FPgGAAk+hERbKM4JOea1/UqWB/f7w0hRvM98jyiT1');";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();

                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Artist: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");

                            return;
                        }
                    }

                    if (Directory.Exists(Application.StartupPath + @"\Artist Manager\" + artists_manager_add_artist_textbox.Text + "\\Backup") == false)
                    {
                        Directory.CreateDirectory(Application.StartupPath + @"\Artist Manager\" + artists_manager_add_artist_textbox.Text + "\\Backup");
                    }

                    this.Alert("Artist successfully added.", Helper.Form_Alert.enmType.Success);

                    client_bomb_loader();

                    artist_manager_artists_listbox.SelectedIndex = artist_manager_artists_listbox.Items.IndexOf(artists_manager_add_artist_textbox.Text);
                }
            }
            catch
            {
                MessageBox.Show("Setup failed. Please contact the support.", "Contact Support.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void artists_manager_delete_artist_button_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure to delete this artist?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                MySqlConnection conn = new MySqlConnection(connStr);
                {
                    try
                    {
                        conn.Open();

                        string sql = "DELETE FROM `artist_manager` WHERE artist_name = '" + artist_manager_artists_listbox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        conn.Close();

                        artist_manager_artists_listbox.Items.Remove(artist_manager_artists_listbox.SelectedItem.ToString());

                        this.Alert("Artist deleted successfully.", Helper.Form_Alert.enmType.Success);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Artist: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        return;
                    }
                }
            }
        }

        private void artists_manager_add_song_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(artists_manager_add_song_textbox.Text))
                {
                    this.Alert("Please define a song name.", Helper.Form_Alert.enmType.Error);
                }
                else if (artist_manager_songs_listbox.Items.Contains(artists_manager_add_song_textbox.Text))
                {
                    this.Alert("Song already exists.", Helper.Form_Alert.enmType.Error);
                }
                else
                {
                    int index = artist_manager_artists_listbox.SelectedIndex;

                    MySqlConnection conn = new MySqlConnection(connStr);
                    {
                        try
                        {
                            conn.Open();

                            string sql = "INSERT INTO `songs` (`artist_name`, `song_name`) VALUES ('" + artist_manager_artists_listbox.SelectedItem.ToString() + "', '" + artists_manager_add_song_textbox.Text + "');";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();

                            conn.Close();

                            artist_manager_songs_listbox.Items.Add(artists_manager_add_song_textbox.Text);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Artist: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");

                            return;
                        }

                        artist_manager_artists_listbox.SelectedIndex = -1;
                        artist_manager_artists_listbox.SelectedIndex = index;
                    }

                    artist_manager_song_count_label.Text = artist_manager_songs_listbox.Items.Count.ToString();

                    this.Alert("Song Name added successfully.", Helper.Form_Alert.enmType.Success);
                }
            }
            catch
            {
                MessageBox.Show("Setup failed. Please contact the support.", "Contact Support.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void artist_manager_songs_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < artist_manager_songs_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(102, 102, 102) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = artist_manager_songs_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, artist_manager_songs_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void artist_manager_distributor_login_button_Click(object sender, EventArgs e)
        {
            string distributor_username = artists_manager_distributor_mail_textbox.Text;
            string distributor_password = artists_manager_distributor_password_textbox.Text;

            if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "Recordjet")
            {
                ChromeOptions options = new ChromeOptions();

                options.AddArguments("--start-maximized");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--disable-infobars");
                options.AddArguments("--disable-blink-features=AutomationControlled");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddExcludedArgument("enable-automation");
                //options.EnableMobileEmulation("iPhone 6");

                //Check if custom plugins should be loaded
                if (settings_chrome_default_profile_checkbox.Checked)
                {
                    options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                }
                else
                {
                    options.AddArguments("--incognito");
                }

                IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                driver.Navigate().GoToUrl("https://cockpit.recordjet.com/login");

                driver.FindElement(By.Name("email")).SendKeys(distributor_username);
                driver.FindElement(By.Name("password")).SendKeys(distributor_password);
                driver.FindElement(By.ClassName("submit")).Click();
            }
            else if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "RecordUnion")
            {
                MethodInvoker mk = delegate
                {
                    ChromeOptions options = new ChromeOptions();

                    options.AddArguments("--start-maximized");
                    options.AddArguments("--disable-notifications");
                    options.AddArguments("--disable-gpu");
                    options.AddArguments("--disable-infobars");
                    options.AddArguments("--disable-blink-features=AutomationControlled");
                    options.AddAdditionalCapability("useAutomationExtension", false);
                    options.AddExcludedArgument("enable-automation");

                    //Check if custom plugins should be loaded
                    if (settings_chrome_default_profile_checkbox.Checked)
                    {
                        options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                    }
                    else
                    {
                        options.AddArguments("--incognito");
                    }

                    IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                    driver.Navigate().GoToUrl("https://recordunion.com/login");

                    driver.FindElement(By.Id("loginEmail")).SendKeys(distributor_username);
                    driver.FindElement(By.Id("loginPassword")).SendKeys(distributor_password);
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            else if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "CD Baby")
            {
                MethodInvoker mk = delegate
                {
                    ChromeOptions options = new ChromeOptions();

                    options.AddArguments("--start-maximized");
                    options.AddArguments("--disable-notifications");
                    options.AddArguments("--disable-gpu");
                    options.AddArguments("--disable-infobars");
                    options.AddArguments("--disable-blink-features=AutomationControlled");
                    options.AddAdditionalCapability("useAutomationExtension", false);
                    options.AddExcludedArgument("enable-automation");

                    //Check if custom plugins should be loaded
                    if (settings_chrome_default_profile_checkbox.Checked)
                    {
                        options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                    }
                    else
                    {
                        options.AddArguments("--incognito");
                    }

                    IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                    driver.Navigate().GoToUrl("https://cdbaby.com/");

                    driver.FindElement(By.Id("AccountLink")).Click();

                    Thread.Sleep(5000);

                    driver.FindElement(By.Id("Username")).SendKeys(distributor_username);
                    driver.FindElement(By.Id("Password")).SendKeys(distributor_password);
                    driver.FindElement(By.XPath("//button[@name='button']")).Click();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            else if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "Boomy")
            {
                string artist_name = artist_manager_artists_listbox.SelectedItem.ToString();

                MethodInvoker mk = delegate
                {
                    ChromeOptions options = new ChromeOptions();

                    options.AddArguments("--start-maximized");
                    options.AddArguments("--disable-notifications");
                    options.AddArguments("--disable-gpu");
                    options.AddArguments("--disable-infobars");
                    options.AddArguments("--disable-blink-features=AutomationControlled");
                    options.AddAdditionalCapability("useAutomationExtension", false);
                    options.AddExcludedArgument("enable-automation");

                    //Check if custom plugins should be loaded
                    if (settings_chrome_default_profile_checkbox.Checked)
                    {
                        options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                    }
                    else
                    {
                        options.AddArguments("--incognito");
                    }

                    IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                    driver.Navigate().GoToUrl("https://boomy.com/sign-in");

                    driver.FindElement(By.Name("email")).SendKeys(distributor_username);
                    driver.FindElement(By.Name("password")).SendKeys(distributor_password);
                    driver.FindElement(By.XPath("//button[@type='submit']")).Click();

                    Thread.Sleep(2000);

                    driver.Navigate().GoToUrl("https://boomy.com/library");

                    driver.FindElement(By.XPath("//div[text()=' Releases ']")).Click();
                    Thread.Sleep(1000);
                    driver.FindElement(By.XPath("//span[text()='" + artist_name + "']")).Click();
                };
                mk.BeginInvoke(callbackfunction, null);
            }

            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }

        private void artists_manager_save_distributor_information_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                string sql_player_type = "UPDATE `" + mysql_database + "`.`artist_manager` SET `first_name`='" + StringCipher.Encrypt(artists_manager_distributor_first_name_textbox.Text, Settings.Encryption_Key) + "', `username`='" + StringCipher.Encrypt(artists_manager_distributor_username_textbox.Text, Settings.Encryption_Key) + "', `last_name`='" + StringCipher.Encrypt(artists_manager_distributor_last_name_textbox.Text, Settings.Encryption_Key) + "', `mail`='" + StringCipher.Encrypt(artists_manager_distributor_mail_textbox.Text, Settings.Encryption_Key) + "', `password`='" + StringCipher.Encrypt(artists_manager_distributor_password_textbox.Text, Settings.Encryption_Key) + "', `paypal_mail`='" + StringCipher.Encrypt(artists_manager_distributor_paypal_mail_textbox.Text, Settings.Encryption_Key) + "', `copyright`='" + StringCipher.Encrypt(artist_manager_distributor_copyright_textbox.Text, Settings.Encryption_Key) + "', `bank_bic`='" + StringCipher.Encrypt(artist_manager_distributor_bank_bic_textbox.Text, Settings.Encryption_Key) + "', `bank_iban`='" + StringCipher.Encrypt(artist_manager_distributor_bank_iban_textbox.Text, Settings.Encryption_Key) + "', `release_status`='" + artist_manager_release_status_combobox.SelectedItem.ToString() + "', `distributor`='" + artist_manager_distributor_type_combobox.SelectedItem.ToString() + "', `country`='" + StringCipher.Encrypt(artist_manager_distributor_country_textbox.Text, Settings.Encryption_Key) + "', `street_number`='" + StringCipher.Encrypt(artist_manager_distributor_street_a_number_textbox.Text, Settings.Encryption_Key) + "', `plz_city`='" + StringCipher.Encrypt(artist_manager_distributor_plz_city_textbox.Text, Settings.Encryption_Key) + "', `phone_number`='" + StringCipher.Encrypt(artist_manager_distributor_phone_number_textbox.Text, Settings.Encryption_Key) + "', `notes`='" + StringCipher.Encrypt(artist_manager_notes_textbox.Text, Settings.Encryption_Key) + "', `hive_name`='" + artist_manager_hive_name_combobox.SelectedItem.ToString() + "' WHERE `artist_name`='" + artist_manager_artists_listbox.SelectedItem.ToString() + "';";
                MySqlCommand cmd_player_type = new MySqlCommand(sql_player_type, conn);
                cmd_player_type.ExecuteNonQuery();

                conn.Close();

                this.Alert("Distributor info saved.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings (Artist Manager): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }
        private void artist_manager_mail_login_button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(settings_artist_manager_mail_domain_textbox.Text))
            {
                string mail_account = artists_manager_distributor_mail_textbox.Text;
                string mail_password = settings_artist_manager_mail_password_textbox.Text;
                string mail_domain = settings_artist_manager_mail_domain_textbox.Text;

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

                    hide_chromedriver();

                    driver.Navigate().GoToUrl(mail_domain);

                    bool mail_login_flag = true;
                    int mail_login_tries_count = 0;
                    int mail_login_register_max_tries = 30;
                    while (mail_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.Id("rcmloginuser")).SendKeys(mail_account);
                            driver.FindElement(By.Id("rcmloginpwd")).SendKeys(mail_password);
                            driver.FindElement(By.Id("rcmloginsubmit")).Click();

                            mail_login_flag = false;
                        }
                        catch
                        {
                            if (++mail_login_tries_count == mail_login_register_max_tries)
                            {
                                MessageBox.Show("Mail login could not be completed. Failed after several tries.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                driver.Dispose();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);
            }
        }

        private void artist_manager_notes_textbox_TextChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                string artist_name = artist_manager_artists_listbox.SelectedItem.ToString();
                string note = StringCipher.Encrypt(artist_manager_notes_textbox.Text, Settings.Encryption_Key);

                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string sql_player_type = "UPDATE `" + mysql_database + "`.`artist_manager` SET `notes`='" + note + "' WHERE `artist_name`='" + artist_name + "';";
                    MySqlCommand cmd_player_type = new MySqlCommand(sql_player_type, conn);
                    cmd_player_type.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void playlist_creator_deezer_start_instance_button_Click(object sender, EventArgs e)
        {
            foreach (var item in playlist_creator_deezer_playlist_name_combobox.Items)
            {
                MySqlConnection conn = new MySqlConnection(connStr);

                string mail_username = playlist_creator_deezer_credentials_combobox.SelectedItem.ToString();
                string mail_password = playlist_creator_deezer_selected_credentials_password_textbox.Text;
                string playlist_name = playlist_creator_deezer_playlist_name_combobox.Text;

                //MethodInvoker mk = delegate
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

                    hide_chromedriver();

                    driver.Navigate().GoToUrl("https://www.deezer.com/de/login");

                    bool start_login_flag = true;
                    int start_login_registercount = 0;
                    int start_login_registermaxTries = 90;
                    while (start_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.ClassName("cookie-btn")).Click();

                            driver.FindElement(By.Id("login_mail")).SendKeys(mail_username);
                            driver.FindElement(By.Id("login_password")).SendKeys(mail_password);
                            driver.FindElement(By.Id("login_form_submit")).Click();
                            start_login_flag = false;
                        }
                        catch
                        {
                            // handle exception
                            if (++start_login_registercount == start_login_registermaxTries)
                            {
                                MessageBox.Show("Creating playlist failed.", "Playlist Creator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            Thread.Sleep(1000);
                        }
                    }

                    Thread.Sleep(60000);

                    driver.FindElement(By.XPath("//span[text()='Playlists']")).Click();

                    Thread.Sleep(5000);

                    driver.FindElement(By.XPath("//div[text()='Eine Playlist erstellen']")).Click();

                    Thread.Sleep(2500);

                    driver.FindElement(By.XPath("//h2[@id='playlist-assistant-name']/following-sibling::input[1]")).SendKeys(playlist_creator_deezer_playlist_name_combobox.Text);

                    //driver.FindElement(By.XPath("//label[@class='checkbox-label']")).Click();

                    driver.FindElement(By.XPath("(//div[@class='modal-footer']//button)[2]")).Click();

                    Thread.Sleep(5000);

                    foreach (var listBoxItem in playlist_creator_deezer_song_url_listbox.Items)
                    {
                        try
                        {
                            driver.Navigate().GoToUrl(listBoxItem.ToString());

                            Thread.Sleep(10000);

                            driver.FindElement(By.XPath("(//button[@class='root-0-3-1 containedSecondary-0-3-10'])[3]")).Click();

                            Thread.Sleep(500);

                            driver.FindElement(By.XPath("//div[@id='popper-portal']/div[1]/div[1]/div[1]/div[1]/ul[1]/li[6]/button[1]")).Click();

                            Thread.Sleep(500);

                            driver.FindElement(By.XPath("//div[@id='popper-portal']/div[1]/div[1]/div[1]/div[2]/div[2]/div[3]/ul[1]/li[1]/button[1]/span[1]")).Click();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                        Thread.Sleep(1000);
                    }

                    driver.FindElement(By.XPath("//span[text()='Playlists']")).Click();

                    Thread.Sleep(5000);

                    driver.FindElement(By.XPath("My Life Is Life")).Click();

                    Thread.Sleep(5000);

                    //Save Playlist to Database
                    string playlist_url = driver.Url;

                    try
                    {
                        conn.Open();

                        string sql = "INSERT INTO `" + mysql_database + "`.`deezer_playlists` (`url`) VALUES ('" + playlist_url + "')";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        return;
                    }

                    playlist_creator_deezer_credentials_combobox.SelectedIndex++;
                    playlist_creator_deezer_playlist_name_combobox.SelectedIndex++;
                    playlist_creator_deezer_shuffle_songs_button.PerformClick();
                }//;

                MessageBox.Show("Continue...");
            }
        }

        private void playlist_creator_deezer_playlist_url_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < playlist_creator_deezer_playlist_url_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(187, 1, 18) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = playlist_creator_deezer_playlist_url_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, playlist_creator_deezer_playlist_url_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void playlist_creator_deezer_song_url_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < playlist_creator_deezer_song_url_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(187, 1, 18) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = playlist_creator_deezer_song_url_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, playlist_creator_deezer_song_url_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void playlist_creator_deezer_stop_instance_button_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }

        private void playlist_creator_deezer_credentials_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "SELECT password FROM deezer_playlist_accounts WHERE username = '" + playlist_creator_deezer_credentials_combobox.SelectedItem.ToString() + "'";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        string playlist_items = StringCipher.Decrypt((rdr[0]).ToString(), Settings.Encryption_Key);
                        playlist_creator_deezer_selected_credentials_password_textbox.Text = playlist_items;
                    }
                }
                rdr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void playlist_creator_deezer_copy_mail_password_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(playlist_creator_deezer_credentials_combobox.Text) == false)
                {
                    Clipboard.SetText(playlist_creator_deezer_credentials_combobox.Text + "\nPassword: " + playlist_creator_deezer_selected_credentials_password_textbox.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void playlist_creator_deezer_add_url_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(playlist_creator_deezer_add_url_textbox.Text) == false)
            {
                MySqlConnection conn = new MySqlConnection(connStr);

                try
                {
                    conn.Open();

                    string sql = "INSERT INTO `deezer_song_urls` (`url`) VALUES ('" + playlist_creator_deezer_add_url_textbox.Text + "');";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    client_bomb_loader();

                    this.Alert("Song URL added.", Helper.Form_Alert.enmType.Success);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }

                playlist_creator_deezer_add_url_textbox.Clear();
            }
            else
            {
                this.Alert("No URL provided.", Helper.Form_Alert.enmType.Error);
            }
        }

        private void playlist_creator_deezer_copy_song_url_button_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(playlist_creator_deezer_song_url_listbox.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void playlist_creator_deezer_delete_url_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `deezer_song_urls` WHERE `url`='" + playlist_creator_deezer_song_url_listbox.SelectedItem.ToString() + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                client_bomb_loader();

                this.Alert("Song URL deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void playlist_creator_deezer_shuffle_songs_button_Click(object sender, EventArgs e)
        {
            ListBox.ObjectCollection list = playlist_creator_deezer_song_url_listbox.Items;
            Random rng = new Random();
            int n = list.Count;
            //begin updating
            playlist_creator_deezer_song_url_listbox.BeginUpdate();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                object value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            playlist_creator_deezer_song_url_listbox.EndUpdate();
            playlist_creator_deezer_song_url_listbox.Invalidate();
        }

        private void playlist_creator_deezer_password_visible_picturebox_Click(object sender, EventArgs e)
        {
            if (playlist_creator_deezer_selected_credentials_password_textbox.UseSystemPasswordChar == true)
            {
                playlist_creator_deezer_selected_credentials_password_textbox.UseSystemPasswordChar = false;
            }
            else
            {
                playlist_creator_deezer_selected_credentials_password_textbox.UseSystemPasswordChar = true;
            }
        }

        private void playlist_creator_deezer_add_url_textbox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(playlist_creator_deezer_add_url_textbox.Text) == false)
            {
                MySqlConnection conn = new MySqlConnection(connStr);

                try
                {
                    conn.Open();

                    string sql = "INSERT INTO `deezer_song_urls` (`url`) VALUES ('" + playlist_creator_deezer_add_url_textbox.Text + "');";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    client_bomb_loader();

                    this.Alert("Song URL added.", Helper.Form_Alert.enmType.Success);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }

                playlist_creator_deezer_add_url_textbox.Clear();
            }
            else
            {
                this.Alert("No URL provided.", Helper.Form_Alert.enmType.Error);
            }
        }

        private void playlist_poly_spotify_playlist_name_combobox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < playlist_poly_spotify_playlist_name_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(187, 1, 18) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = playlist_poly_spotify_playlist_name_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, playlist_poly_spotify_playlist_name_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void playlist_poly_timer_Tick(object sender, EventArgs e)
        {
            int playlists = playlist_poly_spotify_playlist_name_listbox.Items.Count - 1;

            if (playlists > playlist_poly_spotify_playlist_name_listbox.SelectedIndex)
            {
                playlist_poly_spotify_playlist_name_listbox.SelectedIndex++;

                MySqlConnection conn = new MySqlConnection(connStr);

                try
                {
                    conn.Open();

                    string playlist_poly_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_playlist_url`='" + playlist_poly_spotify_playlist_name_listbox.SelectedItem.ToString() + "';";
                    MySqlCommand playlist_poly_cmd = new MySqlCommand(playlist_poly_sql, conn);
                    playlist_poly_cmd.ExecuteNonQuery();

                    string playlist_poly_index_sql = "UPDATE `" + mysql_database + "`.`settings` SET `playlist_poly_index`='" + playlist_poly_spotify_playlist_name_listbox.SelectedIndex + "';";
                    MySqlCommand playlist_poly_index_cmd = new MySqlCommand(playlist_poly_index_sql, conn);
                    playlist_poly_index_cmd.ExecuteNonQuery();

                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\playlist_poly.log", DateTime.Now + " - Updated playlists on all clients.");

                    this.Alert("Playlists updated.", Helper.Form_Alert.enmType.Success);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Playlist Poly: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }
            }
            else
            {
                WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + telegram_api_token_textbox.Text + "/sendMessage?chat_id=" + telegram_alert_chat_id_textbox.Text + "&text=" + "Playlist Poly: No playlists left!");
                req.UseDefaultCredentials = true;
                var result = req.GetResponse();
                req.Abort();

                playlist_poly_spotify_playlist_name_listbox.SelectedIndex = 0;
            }
        }

        private void spotify_playlist_poly_manual_button_Click(object sender, EventArgs e)
        {
            int playlists = playlist_poly_spotify_playlist_name_listbox.Items.Count - 1;

            if (playlists > playlist_poly_spotify_playlist_name_listbox.SelectedIndex)
            {
                playlist_poly_spotify_playlist_name_listbox.SelectedIndex++;

                MySqlConnection conn = new MySqlConnection(connStr);

                try
                {
                    conn.Open();

                    string playlist_poly_sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_playlist_url`='" + playlist_poly_spotify_playlist_name_listbox.SelectedItem.ToString() + "' WHERE hive_name = '" + playlist_poly_spotify_selected_hive_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand playlist_poly_cmd = new MySqlCommand(playlist_poly_sql, conn);
                    playlist_poly_cmd.ExecuteNonQuery();

                    string playlist_poly_index_sql = "UPDATE `" + mysql_database + "`.`settings` SET `playlist_poly_index`='" + playlist_poly_spotify_playlist_name_listbox.SelectedIndex + "';";
                    MySqlCommand playlist_poly_index_cmd = new MySqlCommand(playlist_poly_index_sql, conn);
                    playlist_poly_index_cmd.ExecuteNonQuery();

                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\playlist_poly.log", DateTime.Now + " - Updated playlists on all clients.");

                    this.Alert("Playlists updated.", Helper.Form_Alert.enmType.Success);
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Playlist Poly: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }
            }
            else
            {
                WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + telegram_api_token_textbox.Text + "/sendMessage?chat_id=" + telegram_alert_chat_id_textbox.Text + "&text=" + "Playlist Poly: No playlists left!");
                req.UseDefaultCredentials = true;
                var result = req.GetResponse();
                req.Abort();

                playlist_poly_spotify_playlist_name_listbox.SelectedIndex = 0;
            }
        }

        private void client_spotify_credentials_button_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(client_spotify_selected_credentials_combobox.Text);
        }

        private void account_generator_distributor_first_name_textbox_TextChanged(object sender, EventArgs e)
        {
            string first_name = account_generator_distributor_first_name_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_first_name`='" + StringCipher.Encrypt(first_name, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_last_name_textbox_TextChanged(object sender, EventArgs e)
        {
            string last_name = account_generator_distributor_last_name_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_last_name`='" + StringCipher.Encrypt(last_name, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_country_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string country = account_generator_distributor_country_combobox.SelectedItem.ToString();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_country`='" + StringCipher.Encrypt(country, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_phone_number_textbox_TextChanged(object sender, EventArgs e)
        {
            string phone_number = account_generator_distributor_phone_number_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_phone_number`='" + StringCipher.Encrypt(phone_number, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_hdykau_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string hdykau = account_generator_distributor_hdykau_combobox.SelectedItem.ToString();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_hdykau`='" + StringCipher.Encrypt(hdykau, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_street_number_textbox_TextChanged(object sender, EventArgs e)
        {
            string street_number = account_generator_distributor_street_number_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_street_number`='" + StringCipher.Encrypt(street_number, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_plz_citry_textbox_TextChanged(object sender, EventArgs e)
        {
            string plz_city = account_generator_distributor_plz_city_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_plz_city`='" + StringCipher.Encrypt(plz_city, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_mail_textbox_TextChanged(object sender, EventArgs e)
        {
            string mail = account_generator_distributor_mail_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_mail`='" + StringCipher.Encrypt(mail, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_artist_name_textbox_TextChanged(object sender, EventArgs e)
        {
            string artist_name = account_generator_distributor_artist_name_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_artist_name`='" + StringCipher.Encrypt(artist_name, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_password_textbox_TextChanged(object sender, EventArgs e)
        {
            string password = account_generator_distributor_password_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string background_sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `distributor_password`='" + StringCipher.Encrypt(password, Settings.Encryption_Key) + "';";
                    MySqlCommand background_cmd = new MySqlCommand(background_sql, conn);
                    background_cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void account_generator_distributor_selection_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (account_generator_distributor_selection_combobox.SelectedItem.ToString() == "Recordjet")
            {
                account_generator_distributor_hdykau_combobox.Enabled = true;
            }

            if (account_generator_distributor_selection_combobox.SelectedItem.ToString() == "RecordUnion" || account_generator_distributor_selection_combobox.SelectedItem.ToString() == "CD Baby")
            {
                account_generator_distributor_hdykau_combobox.Enabled = false;
            }
        }

        private void account_generator_distributor_save_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            {
                try
                {
                    conn.Open();

                    string sql = "INSERT INTO `artist_manager` (`artist_name`, `first_name`, `last_name`, `mail`, `password`, `distributor`, `username`, `notes`) VALUES ('" + account_generator_distributor_artist_name_textbox.Text + "', '" + StringCipher.Encrypt(account_generator_distributor_first_name_textbox.Text, Settings.Encryption_Key) + "', '" + StringCipher.Encrypt(account_generator_distributor_last_name_textbox.Text, Settings.Encryption_Key) + "', '" + StringCipher.Encrypt(account_generator_distributor_mail_textbox.Text, Settings.Encryption_Key) + "', '" + StringCipher.Encrypt(account_generator_distributor_password_textbox.Text, Settings.Encryption_Key) + "', '" + account_generator_distributor_selection_combobox.SelectedItem.ToString() + "', '" + StringCipher.Encrypt(account_generator_distributor_artist_name_textbox.Text, Settings.Encryption_Key) + "', 'MbSDPO8YftdqPbgmev786QP+UNUqX90SWbf6TBDZCb0y7HZLLRPeRVoYaLpoUBgD6Y5c5DtUxHWK2ir5+WDkta+FPgGAAk+hERbKM4JOea1/UqWB/f7w0hRvM98jyiT1');";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Artist: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");

                    return;
                }
            }

            if (Directory.Exists(Application.StartupPath + @"\Artist Manager\" + account_generator_distributor_artist_name_textbox.Text + "\\Backup") == false)
            {
                Directory.CreateDirectory(Application.StartupPath + @"\Artist Manager\" + account_generator_distributor_artist_name_textbox.Text + "\\Backup");
            }

            this.Alert("Artist successfully added.", Helper.Form_Alert.enmType.Success);

            client_bomb_loader();
        }

        private void playlist_creator_deezer_login_deezer_button_Click(object sender, EventArgs e)
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

            hide_chromedriver();

            driver.Navigate().GoToUrl("https://www.deezer.com/de/login");

            bool start_login_flag = true;
            int start_login_registercount = 0;
            int start_login_registermaxTries = 90;
            while (start_login_flag == true)
            {
                try
                {
                    driver.FindElement(By.ClassName("cookie-btn")).Click();

                    driver.FindElement(By.Id("login_mail")).SendKeys(playlist_creator_deezer_credentials_combobox.Text);
                    driver.FindElement(By.Id("login_password")).SendKeys(playlist_creator_deezer_selected_credentials_password_textbox.Text);
                    driver.FindElement(By.Id("login_form_submit")).Click();
                    start_login_flag = false;
                }
                catch
                {
                    // handle exception
                    if (++start_login_registercount == start_login_registermaxTries)
                    {
                        MessageBox.Show("Creating playlist failed.", "Playlist Creator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    Thread.Sleep(1000);
                }
            }
        }

        private void audio_converter_mp3_to_wav_button_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to convert a whole folder?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    string audio_path;

                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    openFileDialog.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Desktop";
                    openFileDialog.Filter = "Mp3 Audio (*.mp3)|*.mp3|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 0;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        audio_path = Path.GetDirectoryName(openFileDialog.FileName);

                        foreach (var file in Directory.GetFiles(audio_path))
                        {
                            try
                            {
                                using (Mp3FileReader mp3 = new Mp3FileReader(file))
                                {
                                    using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                                    {
                                        WaveFileWriter.CreateWaveFile(file + ".wav", pcm);

                                        this.Alert("Audio converted.", Helper.Form_Alert.enmType.Success);
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                else
                {
                    string audio_path;

                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    openFileDialog.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Downloads";
                    openFileDialog.Filter = "Mp3 Audio (*.mp3)|*.mp3|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 0;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        audio_path = openFileDialog.FileName;

                        using (Mp3FileReader mp3 = new Mp3FileReader(audio_path))
                        {
                            using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                            {
                                WaveFileWriter.CreateWaveFile(audio_path + ".wav", pcm);

                                this.Alert("Audio converted.", Helper.Form_Alert.enmType.Success);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Convert Audio (mp3 to wav): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void account_generator_spotify_import_accounts_button_Click(object sender, EventArgs e)
        {
            try
            {
                string audio_path;
                string password;

                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Downloads";
                openFileDialog.Filter = "Account Generator File (*.log)|*.log|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    audio_path = openFileDialog.FileName;

                    MySqlConnection conn = new MySqlConnection(connStr);
                    conn.Open();

                    foreach (var line in File.ReadLines(audio_path))
                    {
                        try
                        {
                            //password = line.Remove(15);

                            string sql = "INSERT INTO `spotify_playlist_accounts` (`username`, `password`) VALUES ('" + line + "', '" + StringCipher.Encrypt(playlist_creator_spotify_selected_credentials_password_textbox.Text, Settings.Encryption_Key) + "');";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Import Usernames into Database: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");

                            return;
                        }
                    }

                    conn.Close();

                    client_bomb_loader();

                    this.Alert("Accounts successfully imported.", Helper.Form_Alert.enmType.Success);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Account Generator, import Accounts (Spotify): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void account_generator_spotify_delete_accounts_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `spotify_playlist_accounts`;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Playlist Accounts deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        //Payout Artist Methode
        private void artist_manager_distributor_payout_artist_button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(settings_artist_manager_mail_domain_textbox.Text))
            {
                string mail_account = artists_manager_distributor_mail_textbox.Text;
                string mail_password = settings_artist_manager_mail_password_textbox.Text;
                string mail_domain = settings_artist_manager_mail_domain_textbox.Text;
                string dis_password = artists_manager_distributor_password_textbox.Text;
                string paypal_mail = artists_manager_distributor_paypal_mail_textbox.Text;

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

                    hide_chromedriver();

                    driver.Navigate().GoToUrl("https://cockpit.recordjet.com/login");

                    driver.FindElement(By.Id("email")).SendKeys(mail_account);
                    driver.FindElement(By.Id("password1")).SendKeys(dis_password);
                    driver.FindElement(By.ClassName("submit")).Click();

                    Thread.Sleep(1000);

                    driver.FindElement(By.LinkText("Mein Konto")).Click();

                    Thread.Sleep(1000);

                    driver.FindElement(By.LinkText("Verfügbares Guthaben auszahlen")).Click();

                    Thread.Sleep(1000);

                    driver.FindElement(By.LinkText("PayPal")).Click();
                    string money = driver.FindElement(By.XPath("//h3[@class='price_title']//span[1]")).Text;

                    driver.FindElement(By.Name("amount2_eur")).Clear();
                    driver.FindElement(By.Name("amount2_eur")).SendKeys(money.Substring(3));
                    driver.FindElement(By.Id("email")).Clear();
                    driver.FindElement(By.Id("email")).SendKeys(paypal_mail);
                    driver.FindElement(By.Name("confirmemail")).Click();
                    driver.FindElement(By.Name("confirmemail")).Clear();
                    driver.FindElement(By.Name("confirmemail")).SendKeys(paypal_mail);
                    driver.FindElement(By.Name("password2")).SendKeys(dis_password);
                    driver.FindElement(By.Id("submitPaypalPayout")).Click();
                    Thread.Sleep(1500);
                    driver.FindElement(By.ClassName("submit")).Click();

                    Thread.Sleep(60000);

                    driver.Navigate().GoToUrl(mail_domain);

                    bool mail_login_flag = true;
                    int mail_login_tries_count = 0;
                    int mail_login_register_max_tries = 30;
                    while (mail_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.Id("rcmloginuser")).SendKeys(mail_account);
                            driver.FindElement(By.Id("rcmloginpwd")).SendKeys(mail_password);
                            driver.FindElement(By.Id("rcmloginsubmit")).Click();

                            mail_login_flag = false;
                        }
                        catch
                        {
                            if (++mail_login_tries_count == mail_login_register_max_tries)
                            {
                                MessageBox.Show("Mail login could not be completed. Failed after several tries.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                driver.Dispose();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    try
                    {
                        driver.FindElement(By.XPath("//span[text()='Auszahlungsbestätigung']")).Click();
                        driver.FindElement(By.XPath("//span[text()='Auszahlungsbestätigung']")).Click();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    try
                    {
                        driver.FindElement(By.XPath("(//img[@alt='image'])[3]")).Click();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    Thread.Sleep(5000);

                    driver.Close();

                    Thread.Sleep(1000);

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);
            }
        }

        //Create Release
        private void artist_manager_create_release_button_Click(object sender, EventArgs e)
        {
            string distributor_username = artists_manager_distributor_mail_textbox.Text;
            string distributor_password = artists_manager_distributor_password_textbox.Text;

            //Release Info
            string artist_name = artist_manager_artists_listbox.SelectedItem.ToString();
            string copyright_name = artist_manager_distributor_copyright_textbox.Text;

            var album_name = Microsoft.VisualBasic.Interaction.InputBox("What should your album be called?", "Question", "");

            var number_of_discs = "";
            var number_of_tracks = "";

            if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "CDBaby")
            {
                number_of_discs = Microsoft.VisualBasic.Interaction.InputBox("Number of Discs ", "Question", "1");
                number_of_tracks = Microsoft.VisualBasic.Interaction.InputBox("Number of Tracks ", "Question", "20");
            }

            int song_index = 0;

            //Select the Album Artwork/Cover
            bool upload_artwork = false;
            string artwork_path = "";

            if (artist_manager_distributor_type_combobox.SelectedItem.ToString() != "Boomy")
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to select the album artwork/picture?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    openFileDialog.InitialDirectory = Application.StartupPath + @"\Artist Manager\" + artist_name;
                    openFileDialog.Filter = "Your Album Artwork/Picture (*.jpg)|*.jpg|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 0;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        artwork_path = openFileDialog.FileName;
                        upload_artwork = true;
                    }
                }
            }

            if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "Recordjet")
            {
                //Decide if mp3 files should be auto converted to wav
                bool auto_convert = false;

                DialogResult dialogResult_auto_convert = MessageBox.Show("Auto convert mp3 files to wav?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult_auto_convert == DialogResult.Yes)
                {
                    auto_convert = true;
                }

                ChromeOptions options = new ChromeOptions();

                options.AddArguments("--start-maximized");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--disable-infobars");
                options.AddArguments("--disable-blink-features=AutomationControlled");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddExcludedArgument("enable-automation");

                //Check if custom plugins should be loaded
                if (settings_chrome_default_profile_checkbox.Checked)
                {
                    options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                }
                else
                {
                    options.AddArguments("--incognito");
                }

                IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                driver.Navigate().GoToUrl("https://cockpit.recordjet.com/login");

                driver.FindElement(By.Name("email")).SendKeys(distributor_username);
                driver.FindElement(By.Name("password")).SendKeys(distributor_password);
                driver.FindElement(By.ClassName("submit")).Click();

                Thread.Sleep(10000);

                try
                {
                    driver.FindElement(By.Id("popup-authentication-challenge-content"));
                    MessageBox.Show("Confirm Captcha.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {

                }

                Thread.Sleep(2500);

                driver.Navigate().GoToUrl("https://cockpit.recordjet.com/release-intro");

                //Select the Release Type
                driver.FindElement(By.Id("select-release-deal")).Click();

                //driver.FindElement(By.Name("artist[0]")).SendKeys(artist_name); //Fill Artist Name

                //Fill Song Title
                Thread.Sleep(2500);

                driver.FindElement(By.Name("title")).SendKeys(album_name); //Fill Song Title

                //Select Bundle Price
                driver.FindElement(By.Name("idBundlePrice")).SendKeys("E"); //Select Bundle Price
                driver.FindElement(By.Name("idBundlePrice")).SendKeys("E"); //Select Bundle Price
                driver.FindElement(By.Name("idBundlePrice")).SendKeys("E"); //Select Bundle Price
                driver.FindElement(By.Name("idBundlePrice")).SendKeys("E"); //Select Bundle Price
                driver.FindElement(By.Name("idBundlePrice")).SendKeys("E"); //Select Bundle Price

                //Select Track Price
                driver.FindElement(By.Name("idSongPrice")).SendKeys("E"); //Select Euro Price

                //Select the Lofi Hip Hop Genre
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");
                driver.FindElement(By.Name("idGenre[0]")).SendKeys("H");

                //Fill Copyright Name
                driver.FindElement(By.Name("c_name")).SendKeys(copyright_name);

                //Select date of today
                driver.FindElement(By.Name("date_release")).SendKeys(OpenQA.Selenium.Keys.Enter); //Fill Copyright Name

                //Fill Artist Name (Creates Popup))
                driver.FindElement(By.Name("artist[0]")).SendKeys(artist_name);

                //Upload Picture
                driver.FindElement(By.Id("cover-upload-picture")).SendKeys(artwork_path);

                MessageBox.Show("Please select the Tracklist and then continue.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                foreach (var song in artist_manager_songs_listbox.Items)
                {
                    string song_path = Application.StartupPath + @"\Artist Manager\" + artist_name + "\\";

                    if (auto_convert)
                    {
                        foreach (var file in Directory.GetFiles(song_path))
                        {
                            try
                            {
                                using (Mp3FileReader mp3 = new Mp3FileReader(file))
                                {
                                    using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                                    {
                                        WaveFileWriter.CreateWaveFile(file + ".wav", pcm);
                                    }
                                }

                                if (Directory.Exists(Application.StartupPath + @"\Artist Manager\" + artist_name + "\\Backup") == false)
                                {
                                    Directory.CreateDirectory(Application.StartupPath + @"\Artist Manager\" + artist_name + "\\Backup");
                                }

                                File.Move(Application.StartupPath + @"\Artist Manager\" + artist_name + "\\" + song + ".mp3", Application.StartupPath + @"\Artist Manager\" + artist_name + "\\Backup\\" + song + ".mp3");
                            }
                            catch
                            {

                            }
                        }
                    }

                    //Continue with creating the song
                    driver.FindElement(By.ClassName("create_song")).Click();

                    Thread.Sleep(2000);

                    if (song_index == 0)
                    {
                        //driver.FindElement(By.XPath("//input[@class='upload-wav-audio show-audio-tip']")).SendKeys(song_path);

                        driver.FindElement(By.XPath("//a[contains(@class,'btn btn_edit')]")).Click();
                    }
                    else if (song_index == 1)
                    {
                        //driver.FindElement(By.XPath("(//input[@class='upload-wav-audio show-audio-tip'])[2]")).SendKeys(song_path);

                        driver.FindElement(By.XPath("(//a[contains(@class,'btn btn_edit')])[2]")).Click();
                    }
                    else if (song_index == 2)
                    {
                        //driver.FindElement(By.XPath("(//input[@class='upload-wav-audio show-audio-tip'])[3]")).SendKeys(song_path);

                        driver.FindElement(By.XPath("(//a[contains(@class,'btn btn_edit')])[3]")).Click();
                    }
                    else if (song_index == 3)
                    {
                        //driver.FindElement(By.XPath("//ul[@id='tracklist-sortable']/li[4]/div[5]/a[1]/input[1]")).SendKeys(song_path);

                        driver.FindElement(By.XPath("//ul[@id='tracklist-sortable']/li[4]/div[5]/a[2]")).Click();

                        song_index++;
                    }
                    else
                    {
                        //driver.FindElement(By.XPath("//ul[@id='tracklist-sortable']/li[" + song_index + "]/div[5]/a[1]/input[1]")).SendKeys(song_path);

                        driver.FindElement(By.XPath("//ul[@id='tracklist-sortable']/li[" + song_index + "]/div[5]/a[2]")).Click();
                    }

                    Thread.Sleep(1000);

                    driver.FindElement(By.XPath("(//input[@name='title'])[2]")).SendKeys(song.ToString());

                    driver.FindElement(By.Name("composer[0]")).SendKeys(artist_name);

                    driver.FindElement(By.Name("lyrist[0]")).SendKeys(artist_name);

                    driver.FindElement(By.Id("submitTrack")).Click();

                    song_index++;

                    Thread.Sleep(1500);
                }
            }
            else if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "Boomy")
            {
                ChromeOptions options = new ChromeOptions();

                options.AddArguments("--start-maximized");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--disable-infobars");
                options.AddArguments("--disable-blink-features=AutomationControlled");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddExcludedArgument("enable-automation");

                //Check if custom plugins should be loaded
                if (settings_chrome_default_profile_checkbox.Checked)
                {
                    options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                }
                else
                {
                    options.AddArguments("--incognito");
                }

                IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                driver.Navigate().GoToUrl("https://boomy.com/sign-in");

                driver.FindElement(By.Name("email")).SendKeys(distributor_username);
                driver.FindElement(By.Name("password")).SendKeys(distributor_password);
                driver.FindElement(By.XPath("//button[@type='submit']")).Click();

                Thread.Sleep(5000);

                //Create Songs
                foreach (var song in artist_manager_songs_listbox.Items)
                {
                    driver.FindElement(By.XPath("//span[text()='Lo-Fi']")).Click();

                    Thread.Sleep(2500);

                    Random random_max_count = new Random();
                    int random_int = random_max_count.Next(0, 3);

                    if (random_int == 0)
                    {
                        driver.FindElement(By.XPath("//span[text()='Morning Sun ']")).Click();
                        driver.FindElement(By.XPath("//button[text()='Create song']")).Click();

                        Thread.Sleep(5000);

                        bool recording = true;

                        while (recording)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//span[text()='Recording...']")).Click();
                            }
                            catch
                            {
                                recording = false;
                            }

                            Thread.Sleep(2500);
                        }
                    }
                    else if (random_int == 1)
                    {
                        driver.FindElement(By.XPath("//span[text()='Afternoon Nap ']")).Click();
                        driver.FindElement(By.XPath("//button[text()='Create song']")).Click();

                        Thread.Sleep(5000);

                        bool recording = true;

                        while (recording)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//span[text()='Recording...']")).Click();
                            }
                            catch
                            {
                                recording = false;
                            }

                            Thread.Sleep(2500);
                        }
                    }
                    else if (random_int == 2)
                    {
                        driver.FindElement(By.XPath("//span[text()='Rainy Nights ']")).Click();
                        driver.FindElement(By.XPath("//button[text()='Create song']")).Click();

                        Thread.Sleep(5000);

                        bool recording = true;

                        while (recording)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//span[text()='Recording...']")).Click();
                            }
                            catch
                            {
                                recording = false;
                            }

                            Thread.Sleep(2500);
                        }
                    }
                    else if (random_int == 3)
                    {
                        driver.FindElement(By.XPath("//span[text()='Rainy Nights ']")).Click();
                        driver.FindElement(By.XPath("//button[text()='Create song']")).Click();

                        Thread.Sleep(5000);

                        bool recording = true;

                        while (recording)
                        {
                            try
                            {
                                driver.FindElement(By.XPath("//span[text()='Recording...']")).Click();
                            }
                            catch
                            {
                                recording = false;
                            }

                            Thread.Sleep(2500);
                        }
                    }

                    Thread.Sleep(5000);

                    try
                    {
                        driver.FindElement(By.XPath("//img[@class='icon']")).Click();

                        Thread.Sleep(1000);

                        driver.FindElement(By.XPath("//input[@placeholder='New Song']")).SendKeys(song.ToString());
                        driver.FindElement(By.XPath("//input[@placeholder='New Artist']")).SendKeys(artist_name);

                        driver.FindElement(By.ClassName("dropdown-title")).Click();

                        Thread.Sleep(1000);

                        driver.FindElement(By.XPath("//span[text()='Only Me']")).Click();

                        driver.FindElement(By.XPath("//button[text()='Save Changes']")).Click();
                    }
                    catch
                    {

                    }

                    Thread.Sleep(5000);

                    driver.Navigate().GoToUrl("https://boomy.com/style");

                    Thread.Sleep(5000);
                }

                driver.Navigate().GoToUrl("https://boomy.com/release/create");

                Thread.Sleep(2500);
                
                driver.FindElement(By.XPath("//button[text()=' Generate Art']")).Click();

                bool generating = true;

                while (generating)
                {
                    try
                    {
                        driver.FindElement(By.XPath("//i[contains(@class,'fa fa-spin')]"));
                    }
                    catch
                    {
                        generating = false;
                    }

                    Thread.Sleep(2500);
                }

                driver.FindElement(By.XPath("//input[@placeholder='New Album']")).SendKeys(album_name);


                driver.FindElement(By.XPath("//input[@placeholder='New Artist']")).SendKeys(artist_name);

                driver.FindElement(By.XPath("//span[text()='Alternative']")).Click();

                Thread.Sleep(1000);

                driver.FindElement(By.XPath("//span[text()='Hip Hop / Rap']")).Click();

                driver.FindElement(By.XPath("//button[text()=' Continue ']")).Click();

                Thread.Sleep(5000);

                foreach (var song in artist_manager_songs_listbox.Items)
                {
                    driver.FindElement(By.XPath("//span[text()='" + song.ToString() + "']")).Click();
                }

                //driver.FindElement(By.XPath("//button[text()=' Continue ']")).Click();
            }
            else if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "RecordUnion")
            {
                DialogResult dialogResult_auto_convert = MessageBox.Show("Convert mp3 files to wav?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult_auto_convert == DialogResult.Yes)
                {
                    string audio_path;

                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    openFileDialog.InitialDirectory = Application.StartupPath + @"\Artist Manager\" + artist_name;
                    openFileDialog.Filter = "Mp3 Audio (*.mp3)|*.mp3|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 0;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        audio_path = Path.GetDirectoryName(openFileDialog.FileName);

                        foreach (var file in Directory.GetFiles(audio_path))
                        {
                            try
                            {
                                using (Mp3FileReader mp3 = new Mp3FileReader(file))
                                {
                                    using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                                    {
                                        WaveFileWriter.CreateWaveFile(file + ".wav", pcm);

                                        this.Alert("Audio converted.", Helper.Form_Alert.enmType.Success);
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }

                ChromeOptions options = new ChromeOptions();

                options.AddArguments("--start-maximized");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--disable-infobars");
                options.AddArguments("--disable-blink-features=AutomationControlled");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddExcludedArgument("enable-automation");

                //Check if custom plugins should be loaded
                if (settings_chrome_default_profile_checkbox.Checked)
                {
                    options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                }
                else
                {
                    options.AddArguments("--incognito");
                }

                IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                driver.Navigate().GoToUrl("https://recordunion.com/login");

                driver.FindElement(By.Id("loginEmail")).SendKeys(distributor_username);
                driver.FindElement(By.Id("loginPassword")).SendKeys(distributor_password);

                MessageBox.Show("Please login and continue afterwards.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                driver.Navigate().GoToUrl("https://app.recordunion.com/SubmitContent/ChoosePlan");

                driver.FindElement(By.XPath("//button[text()='Continue with this plan']")).Click();

                //Fill Title Name
                driver.FindElement(By.Id("title")).SendKeys(album_name);

                //Select Genre
                driver.FindElement(By.XPath("//span[text()='Select Genre']")).Click();

                Thread.Sleep(1000);

                driver.FindElement(By.XPath("//div[@class='chosen-search']//input")).SendKeys("HipHop");
                driver.FindElement(By.XPath("//em[text()='HipHop']")).Click();

                //Check EAN Checkbox
                driver.FindElement(By.XPath("//label[@for='buyUpcCode']")).Click();

                //Fill and create Artist Name
                driver.FindElement(By.XPath("//a[@data-type='primaryArtist']")).Click();
                Thread.Sleep(1500);
                driver.FindElement(By.Name("Artist name")).SendKeys(artist_name);

                driver.FindElement(By.XPath("//button[text()='Next']")).Click();
                Thread.Sleep(1500);
                driver.FindElement(By.XPath("//button[text()='Assign new Spotify Artist URI']")).Click();

                //Fill Copyright
                driver.FindElement(By.Id("copyrightText")).SendKeys(copyright_name);

                driver.FindElement(By.Id("publishName")).SendKeys(copyright_name);

                //Continue
                Thread.Sleep(2500);
                driver.FindElement(By.LinkText("Continue")).Click();

                //Upload Artwork
                Thread.Sleep(5000);
                driver.FindElement(By.XPath("//a[@class='icon mpStartsAddingArtwork']")).Click();

                MessageBox.Show("Please upload your Artwork and then open the Add Tracks field.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //Add Tracks
                foreach (var song_name in artist_manager_songs_listbox.Items)
                {
                    driver.FindElement(By.Id("trackTitle")).Clear();
                    driver.FindElement(By.Id("trackTitle")).SendKeys(song_name.ToString());

                    driver.FindElement(By.Id("audioLanguageType1")).Click();

                    //Fill Language
                    driver.FindElement(By.XPath("(//span[text()='Select an Option'])[2]")).Click();
                    Thread.Sleep(1000);
                    driver.FindElement(By.XPath("(//span[text()='Select an Option'])[2]/following::input")).SendKeys("German");
                    Thread.Sleep(1000);
                    driver.FindElement(By.XPath("//em[text()='German']")).Click();

                    //Fill the Lyricist Name
                    driver.FindElement(By.XPath("//input[@placeholder='Full name of the lyricist(s) of this track']")).SendKeys(artists_manager_distributor_first_name_textbox.Text + " " + artists_manager_distributor_last_name_textbox.Text);
                    driver.FindElement(By.XPath("//input[@placeholder='Full name of the composer(s) of this track']")).SendKeys(artists_manager_distributor_first_name_textbox.Text + " " + artists_manager_distributor_last_name_textbox.Text);

                    //Save Track
                    driver.FindElement(By.LinkText("Save this track")).Click();

                    MessageBox.Show("Please upload the next Track and then repeat.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                MessageBox.Show("All songs added. Please continue now.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                driver.FindElement(By.LinkText("Continue")).Click();

                Thread.Sleep(1000);

                //Continue to the Store Selection Page
                driver.FindElement(By.LinkText("//a[@class='icon mpOpensChooseStoresPackage']")).Click();
                driver.FindElement(By.XPath("//li[@data-name='World domination']//span")).Click();
                driver.FindElement(By.LinkText("Continue")).Click();
            }
            else if (artist_manager_distributor_type_combobox.SelectedItem.ToString() == "CD Baby")
            {
                ChromeOptions options = new ChromeOptions();

                options.AddArguments("--start-  ");
                options.AddArguments("--disable-notifications");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--disable-infobars");
                options.AddArguments("--disable-blink-features=AutomationControlled");
                options.AddAdditionalCapability("useAutomationExtension", false);
                options.AddExcludedArgument("enable-automation");

                //Check if custom plugins should be loaded
                if (settings_chrome_default_profile_checkbox.Checked)
                {
                    options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                }
                else
                {
                    options.AddArguments("--incognito");
                }

                IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                try
                {
                    driver.Navigate().GoToUrl("https://cdbaby.com/");
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Navigate CDBaby", "Click on Account Link: " + ex.Message);
                }

                try
                {
                    driver.FindElement(By.Id("AccountLink")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Click on Account Link: " + ex.Message);
                }

                Thread.Sleep(5000);

                try
                {
                    driver.FindElement(By.Id("Username")).SendKeys(distributor_username);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill Username: " + ex.Message);
                }

                try
                {
                    driver.FindElement(By.Id("Password")).SendKeys(distributor_password);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill Password: " + ex.Message);
                }

                try
                {
                    driver.FindElement(By.XPath("//button[@name='button']")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Confirm Login: " + ex.Message);
                }

                MessageBox.Show("Confirm login.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //Skip Hello Again Message
                try
                {
                    driver.FindElement(By.Id("confirm-contact-info")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Skip Hello Again Message: " + ex.Message);
                }

                //Click on add new title
                try
                {
                    driver.FindElement(By.XPath("//span[text()='Add New Title']")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Click on add new title: " + ex.Message);
                    return;
                }

                //Select create album
                try
                {
                    driver.FindElement(By.XPath("//div[contains(@class,'button green')]")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Click on Create Album: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Select Standard Album
                try
                {
                    driver.FindElement(By.XPath("(//div[@class='button go-standard-button'])[2]")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Click on Standard Album: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Fill Artist Name
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_txtArtistName")).SendKeys(artist_name);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill Artist Name: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Fill Album Title
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_txtAlbumName")).SendKeys(album_name);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill Album Title: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Fill Release Date
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_txtReleaseDate")).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill Release Date: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Check Digital Release
                try
                {
                    driver.FindElement(By.XPath("//label[text()='Digital']")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Check Digital Release: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Select Bar Code
                try
                {
                    driver.FindElement(By.XPath("/html/body/form/div[8]/div/div/div[2]/div[1]/div[4]/div/div[2]/div[2]/div[3]/table/tbody/tr[2]/td/div/label")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Select Bar Code: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Save and Continue
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_btnSaveAndContinue")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and Continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(5000);

                //Accept TOS
                try
                {
                    driver.FindElement(By.XPath("//label[text()='I have read, understood, and agree to the terms above, and I am at least 13 years old.']")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Accept Tos: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Fill First & Last Name
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_txtSignature")).SendKeys(artists_manager_distributor_first_name_textbox.Text + " " + artists_manager_distributor_last_name_textbox.Text);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill First & Last Name: " + ex.Message);
                    return;
                }

                //I agree
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_btnIAgree")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "I agree: " + ex.Message);
                    return;
                }

                //Fill Number of Disks
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_txtDiscCount")).Clear();
                    driver.FindElement(By.Id("ctl00_rightColumn_txtDiscCount")).SendKeys(number_of_discs);

                    driver.FindElement(By.Id("ctl00_rightColumn_btnDiscCountNext")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill Number of Disks: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Fill Number of Tracks
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_rptTracksPerDisc_ctl00_txtTracksPerDisc")).Clear();
                    driver.FindElement(By.Id("ctl00_rightColumn_rptTracksPerDisc_ctl00_txtTracksPerDisc")).SendKeys(number_of_tracks);

                    driver.FindElement(By.Id("ctl00_rightColumn_ImageButton1")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill Number of Tracks: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Fill Song Names to Track Overview
                song_index = 1;

                try
                {
                    foreach (var song in artist_manager_songs_listbox.Items)
                    {
                        if (song_index < 10)
                        {
                            driver.FindElement(By.Id("ctl00_rightColumn_rptTracks_ctl0" + song_index + "_txtTrackName")).SendKeys(song.ToString());
                        }
                        else
                        {
                            driver.FindElement(By.Id("ctl00_rightColumn_rptTracks_ctl" + song_index + "_txtTrackName")).SendKeys(song.ToString());
                        }

                        song_index++;

                        Thread.Sleep(1000);
                    }

                    song_index = 1;
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Fill Song Names to Track Overview: " + ex.Message);
                    return;
                }

                //Save and Continue
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_btnSaveAndContinue")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and Continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(7500);

                bool add_song_writer = true;

                foreach (var song in artist_manager_songs_listbox.Items)
                {
                    try
                    {
                        if (song_index < 10)
                        {
                            driver.FindElement(By.Id("ctl00_rightColumn_rptDiscs_ctl00_rptTracks_ctl0" + song_index + "_lbActionButton")).Click();
                        }
                        else
                        {
                            driver.FindElement(By.Id("ctl00_rightColumn_rptDiscs_ctl00_rptTracks_ctl" + song_index + "_lbActionButton")).Click();
                        }

                        Thread.Sleep(5000);

                        driver.FindElement(By.Id("ctl00_rightColumn_rblLyricCleanliness_0")).Click();

                        driver.FindElement(By.Id("ctl00_rightColumn_ucTrackLanguage_ddlAlbumLanguage")).Click();
                        driver.FindElement(By.Id("ctl00_rightColumn_ucTrackLanguage_ddlAlbumLanguage")).SendKeys("N");
                        driver.FindElement(By.Id("ctl00_rightColumn_ucTrackLanguage_ddlAlbumLanguage")).SendKeys(OpenQA.Selenium.Keys.Enter);
                        driver.FindElement(By.Id("ctl00_rightColumn_rblTrackLiveliness_0")).Click();
                        driver.FindElement(By.Id("ctl00_rightColumn_rblCompositionType_0")).Click();

                        if (add_song_writer == true)
                        {
                            try
                            {
                                driver.FindElement(By.Id("ctl00_rightColumn_SongWriterBank_lbAddNewSongWriter")).Click();

                                Thread.Sleep(3500);

                                driver.FindElement(By.XPath("(//input[@id='first_Name'])[2]")).SendKeys(artists_manager_distributor_first_name_textbox.Text);
                                driver.FindElement(By.XPath("(//input[@id='last_Name'])[2]")).SendKeys(artists_manager_distributor_last_name_textbox.Text);

                                Thread.Sleep(2500);

                                driver.FindElement(By.XPath("(//a[@id='ctl00_rightColumn_SongWriterBank_lbSaveSongWriter'])[2]")).Click();

                                Thread.Sleep(5000);

                                //Add to this Track
                                try
                                {
                                    driver.FindElement(By.Id("ctl00_rightColumn_SongWriterBank_rptSongwriters_ctl01_lbActionButton")).Click();
                                }
                                catch (Exception ex)
                                {
                                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Add to this Track: " + ex.Message);
                                    return;
                                }

                                Thread.Sleep(5000);

                                //Check Music Checkbox
                                try
                                {
                                    driver.FindElement(By.Id("ctl00_rightColumn_SongWriterBank_rptSongwriterDetails_ctl00_chklContribuiton_0")).Click();
                                }
                                catch (Exception ex)
                                {
                                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Check Music Checkbox: " + ex.Message);
                                    return;
                                }

                                Thread.Sleep(2500);

                                //Check Lyrics Checkbox
                                try
                                {
                                    driver.FindElement(By.Id("ctl00_rightColumn_SongWriterBank_rptSongwriterDetails_ctl00_chklContribuiton_1")).Click();
                                }
                                catch (Exception ex)
                                {
                                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Check Lyrics Checkbox: " + ex.Message);
                                    return;
                                }

                                Thread.Sleep(2500);

                                //Check Publisher No Checkbox
                                try
                                {
                                    driver.FindElement(By.Id("ctl00_rightColumn_SongWriterBank_rptSongwriterDetails_ctl00_rdlPublisher_0")).Click();
                                }
                                catch (Exception ex)
                                {
                                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Check Publisher No Checkbox: " + ex.Message);
                                    return;
                                }

                                Thread.Sleep(2500);

                                //Apply this songwriter information to all tracks (this will overwrite any existing information you have on other tracks)
                                try
                                {
                                    driver.FindElement(By.Id("ctl00_rightColumn_chkAppyAll")).Click();
                                }
                                catch (Exception ex)
                                {
                                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Check Apply this songwriter information to all tracks: " + ex.Message);
                                    return;
                                }

                                add_song_writer = false;
                            }
                            catch
                            {

                            }
                        }

                        Thread.Sleep(2500);

                        //Save Song Info
                        try
                        {
                            driver.FindElement(By.Id("ctl00_rightColumn_lbSave")).Click();
                        }
                        catch (Exception ex)
                        {
                            artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save Song Info: " + ex.Message);
                            return;
                        }

                        song_index++;

                        Thread.Sleep(7500);
                    }
                    catch (Exception ex)
                    {
                        artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Open Track and add info to it (Song: " + song.ToString() + "): " + ex.Message);
                        return;
                    }
                }

                Thread.Sleep(7500);

                //Continue
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_lbContinueButton")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Select Genre #1
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlGenresFirst")).Click();
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlGenresFirst")).SendKeys("H");
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlGenresFirst")).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Select Genre #1: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Select Sub Genre #1
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlSubGenresFirst")).Click();
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlSubGenresFirst")).SendKeys("I");
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlSubGenresFirst")).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Select Sub Genre #1: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Select Genre #2
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlGenresSecond")).Click();
                    Thread.Sleep(500);
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlGenresSecond")).SendKeys("H");
                    Thread.Sleep(500);
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlGenresSecond")).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Select Genre #2: " + ex.Message);
                    return;
                }

                Thread.Sleep(5000);

                //Select Sub Genre #2
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlSubGenresSecond")).Click();
                    Thread.Sleep(500);
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlSubGenresSecond")).SendKeys("I");
                    Thread.Sleep(500);
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlSubGenresSecond")).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Select Sub Genre #2: " + ex.Message);
                    return;

                }

                Thread.Sleep(5000);

                //Select Genre #3
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlMoods")).Click();
                    Thread.Sleep(500);
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlMoods")).SendKeys("I");
                    Thread.Sleep(500);
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlMoods")).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Select Genre #3: " + ex.Message);
                    return;
                }

                Thread.Sleep(5000);

                //Select Artist Location
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlLocations")).Click();
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlLocations")).SendKeys("G");
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlLocations")).SendKeys("G");
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlLocations")).SendKeys("G");
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlLocations")).SendKeys("G");
                    driver.FindElement(By.Id("ctl00_rightColumn_ddlLocations")).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Select Genre #2: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Save and continue
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_btnSaveAndContinue")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(5000);

                //Save and continue
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_btnSaveAndContinue")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(5000);

                //Save and continue
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_btnSaveAndContinue")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(5000);

                //Edit Partner IDs Button
                try
                {
                    driver.FindElement(By.XPath("//button[contains(@class,'editBtn button')]")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Edit Partner IDs Button: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Check Create New Artist ID (Apple Music / iTunes)
                try
                {
                    driver.FindElement(By.XPath("//input[@class='createModeOption']")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Check Create New Artist ID (Apple Music / iTunes): " + ex.Message);
                }

                Thread.Sleep(2500);

                //Check Create New Artist ID (Spotify)
                try
                {
                    driver.FindElement(By.XPath("(//input[@class='createModeOption'])[2]")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Check Create New Artist ID (Spotify): " + ex.Message);
                }

                Thread.Sleep(2500);

                //Save and Continue
                try
                {
                    driver.FindElement(By.XPath("//a[@class='button green']")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and Continue: " + ex.Message);
                }

                Thread.Sleep(7500);

                //Continue
                try
                {
                    driver.FindElement(By.XPath("(//a[@class='button green'])[2]")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(7500);

                //Save and Continue
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_btnSaveAndContinue")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and Continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(7500);

                //Save and Continue
                try
                {
                    driver.FindElement(By.XPath("//a[@class='button green']")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and Continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(7500);

                //DON'T COLLECT ADDITIONAL REVENUE
                try
                {
                    driver.FindElement(By.ClassName("optOutOption")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "DON'T COLLECT ADDITIONAL REVENUE: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Save and Continue
                try
                {
                    driver.FindElement(By.XPath("//input[contains(@class,'button text-right')]")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and Continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //I agree, continue
                try
                {
                    driver.FindElement(By.XPath("(//input[contains(@class,'button text-right')])[3]")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "I agree, continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(7500);

                //Add Artwork
                try
                {
                    driver.FindElement(By.XPath("//a[contains(@class,'file-browse button')]")).SendKeys(artwork_path);
                    driver.FindElement(By.XPath("//a[contains(@class,'file-browse button')]")).SendKeys(OpenQA.Selenium.Keys.Enter);
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Add Artwork: " + ex.Message);
                    return;
                }

                MessageBox.Show("Confirm Artwork selection.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //Save and Continue
                try
                {
                    driver.FindElement(By.Id("ctl00_rightColumn_btnSaveAndContinue")).Click();
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Save and Continue: " + ex.Message);
                    return;
                }

                Thread.Sleep(2500);

                //Upload every Song
                song_index = 1;

                try
                {
                    foreach (var song in artist_manager_songs_listbox.Items)
                    {
                        string song_name = Application.StartupPath + @"\Artist Manager\" + artist_name + "\\" + song.ToString() + ".wav";

                        if (song_index < 10)
                        {
                            driver.FindElement(By.Id("ctl00_rightColumn_albumUpload_rptTracks_ctl0" + song_index + "_fileUpload")).SendKeys(song_name);
                        }
                        else
                        {
                            driver.FindElement(By.Id("ctl00_rightColumn_albumUpload_rptTracks_ctl" + song_index + "_fileUpload")).SendKeys(song_name);
                        }

                        song_index++;

                        Thread.Sleep(1000);
                    }

                    song_index = 1;
                }
                catch (Exception ex)
                {
                    artist_manager_logs_datagridview.Rows.Add(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), "Create Album", "Upload every Song: " + ex.Message);
                    return;
                }
            }

            MessageBox.Show("Now you can continue on your own.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }

        //Filter existing artists
        private void artist_manager_filter_release_status_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            artist_manager_artists_listbox.Items.Clear();

            string sql = null;

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                if (artist_manager_filter_release_status_combobox.SelectedItem.ToString() == "Any")
                {
                    if (artist_manager_selected_hive_filter_combobox.SelectedIndex == -1)
                    {
                        sql = "SELECT artist_name FROM artist_manager;";
                    }
                    else
                    {
                        sql = "SELECT artist_name FROM artist_manager WHERE hive_name = '" + artist_manager_selected_hive_filter_combobox.SelectedItem.ToString() + "';";
                    }

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                artist_manager_artists_listbox.Items.Add(rdr[0].ToString());
                            }
                        }
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Search Artist: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");
                    }
                }
                else if (artist_manager_filter_release_status_combobox.SelectedItem.ToString() == "Pending")
                {
                    if (artist_manager_selected_hive_filter_combobox.SelectedIndex == -1)
                    {
                        sql = "SELECT artist_name FROM artist_manager WHERE release_status = 'Pending';";
                    }
                    else
                    {
                        sql = "SELECT artist_name FROM artist_manager WHERE hive_name = '" + artist_manager_selected_hive_filter_combobox.SelectedItem.ToString() + "' AND release_status = 'Pending';";
                    }

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                artist_manager_artists_listbox.Items.Add(rdr[0].ToString());
                            }
                        }
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Search Artist: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");
                    }
                }
                else if (artist_manager_filter_release_status_combobox.SelectedItem.ToString() == "Released")
                {
                    if (artist_manager_selected_hive_filter_combobox.SelectedIndex == -1)
                    {
                        sql = "SELECT artist_name FROM artist_manager WHERE release_status = 'Released';";
                    }
                    else
                    {
                        sql = "SELECT artist_name FROM artist_manager WHERE hive_name = '" + artist_manager_selected_hive_filter_combobox.SelectedItem.ToString() + "' AND release_status = 'Released';";
                    }

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                artist_manager_artists_listbox.Items.Add(rdr[0].ToString());
                            }
                        }
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Search Artist: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");
                    }
                }
                else if (artist_manager_filter_release_status_combobox.SelectedItem.ToString() == "Prepared")
                {
                    if (artist_manager_selected_hive_filter_combobox.SelectedIndex == -1)
                    {
                        sql = "SELECT artist_name FROM artist_manager WHERE release_status = 'Prepared';";
                    }
                    else
                    {
                        sql = "SELECT artist_name FROM artist_manager WHERE hive_name = '" + artist_manager_selected_hive_filter_combobox.SelectedItem.ToString() + "' AND release_status = 'Prepared';";
                    }

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                artist_manager_artists_listbox.Items.Add(rdr[0].ToString());
                            }
                        }
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Search Artist: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");
                    }
                }

                conn.Close();

                artist_manager_artists_counter_label.Text = artist_manager_artists_listbox.Items.Count.ToString();
            }
            catch(Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Open MySQL Connection: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void artist_manager_song_urls_integrity_check_button_Click(object sender, EventArgs e)
        {
            foreach (var song in artist_manager_songs_listbox.Items)
            {
                if (File.Exists(Application.StartupPath + @"\Artist Manager\" + artist_manager_artists_listbox.SelectedItem.ToString() + "\\" + song + ".mp3") == false)
                {
                    MessageBox.Show("Song not found: " + Application.StartupPath + @"\Artist Manager\" + artist_manager_artists_listbox.SelectedItem.ToString() + "\\" + song + ".mp3", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            MessageBox.Show("Integrity check completed.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Add Song
        private void artists_manager_add_song_textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                artists_manager_add_song_button.PerformClick();
        }

        private void artist_manager_songs_listbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == System.Windows.Forms.Keys.C)
            {
                int index = artist_manager_songs_listbox.SelectedIndex;
                Clipboard.SetText(artist_manager_songs_listbox.SelectedItem.ToString());
                artist_manager_songs_listbox.SelectedIndex = index;
            }
        }

        private void artist_manager_distributor_payout_artist_paypal_button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(settings_artist_manager_mail_domain_textbox.Text))
            {
                string artist_name = artist_manager_artists_listbox.SelectedItem.ToString();
                string mail_account = artists_manager_distributor_mail_textbox.Text;
                string mail_password = settings_artist_manager_mail_password_textbox.Text;
                string mail_domain = settings_artist_manager_mail_domain_textbox.Text;
                string dis_password = artists_manager_distributor_password_textbox.Text;
                string paypal_mail = artists_manager_distributor_paypal_mail_textbox.Text;
                string money = "";

                int index = artist_manager_artists_listbox.SelectedIndex;

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

                    //Check if custom plugins should be loaded
                    if (settings_chrome_default_profile_checkbox.Checked)
                    {
                        options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                    }
                    else
                    {
                        options.AddArguments("--incognito");
                    }

                    IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                    hide_chromedriver();

                    driver.Navigate().GoToUrl("https://cockpit.recordjet.com/login");

                    driver.FindElement(By.Name("email")).SendKeys(mail_account);
                    driver.FindElement(By.Name("password")).SendKeys(dis_password);
                    driver.FindElement(By.ClassName("submit")).Click();

                    MessageBox.Show("Confirm Captcha.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    try
                    {
//                        driver.FindElement(By.Id("popup-authentication-challenge-content"));
                    }
                    catch
                    {

                    }

                    driver.FindElement(By.LinkText("Mein Konto")).Click();

                    Thread.Sleep(1000);

                    driver.FindElement(By.LinkText("Verfügbares Guthaben auszahlen")).Click();

                    Thread.Sleep(1000);

                    driver.FindElement(By.LinkText("PayPal")).Click();
                    money = driver.FindElement(By.XPath("//h3[@class='price_title']//span[1]")).Text;

                    driver.FindElement(By.Name("amount2_eur")).Clear();
                    driver.FindElement(By.Name("amount2_eur")).SendKeys(money.Substring(3));
                    driver.FindElement(By.Id("email")).Clear();
                    driver.FindElement(By.Id("email")).SendKeys(paypal_mail);
                    driver.FindElement(By.Name("confirmemail")).Click();
                    driver.FindElement(By.Name("confirmemail")).Clear();
                    driver.FindElement(By.Name("confirmemail")).SendKeys(paypal_mail);
                    driver.FindElement(By.Name("password2")).SendKeys(dis_password);
                    driver.FindElement(By.Id("submitPaypalPayout")).Click();
                    Thread.Sleep(1500);
                    driver.FindElement(By.ClassName("submit")).Click();

                    Thread.Sleep(60000);

                    driver.Navigate().GoToUrl(mail_domain);

                    Thread.Sleep(5000);

                    bool mail_login_flag = true;
                    int mail_login_tries_count = 0;
                    int mail_login_register_max_tries = 30;
                    while (mail_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.Id("rcmloginuser")).SendKeys(mail_account);
                            driver.FindElement(By.Id("rcmloginpwd")).SendKeys(mail_password);
                            driver.FindElement(By.Id("rcmloginsubmit")).Click();

                            mail_login_flag = false;
                        }
                        catch
                        {
                            if (++mail_login_tries_count == mail_login_register_max_tries)
                            {
                                MessageBox.Show("Mail login could not be completed. Failed after several tries.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                driver.Dispose();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    try
                    {
                        driver.FindElement(By.XPath("//span[text()='Auszahlungsbestätigung']")).Click();
                        driver.FindElement(By.XPath("//span[text()='Auszahlungsbestätigung']")).Click();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    try
                    {
                        driver.FindElement(By.XPath("(//img[@alt='image'])[3]")).Click();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    Thread.Sleep(1000);

                    driver.Close();

                    //Insert payout history into database
                    MySqlConnection conn = new MySqlConnection(connStr);
                    try
                    {
                        money = money.Substring(4);

                        conn.Open();

                        string sql = "INSERT INTO `" + mysql_database + "`.`artist_manager_payout_history` (`artist_name`, `datetime`, `type`, `detail`, `value`) VALUES ('" + artist_name + "', '" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + "', 'PayPal', '" + paypal_mail + "', '" + money + "');";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Payout Information to Database: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        return;
                    }

                    this.Alert("Payout History updated.", Helper.Form_Alert.enmType.Success);

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);
            }
        }

        private void artist_manager_distributor_payout_artist_bank_button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(settings_artist_manager_mail_domain_textbox.Text))
            {
                string mail_account = artists_manager_distributor_mail_textbox.Text;
                string mail_password = settings_artist_manager_mail_password_textbox.Text;
                string mail_domain = settings_artist_manager_mail_domain_textbox.Text;
                string dis_password = artists_manager_distributor_password_textbox.Text;

                string iban = artist_manager_distributor_bank_iban_textbox.Text;
                string bic = artist_manager_distributor_bank_bic_textbox.Text;
                string account_owner = artists_manager_distributor_first_name_textbox.Text + " " + artists_manager_distributor_last_name_textbox.Text;
                string street_a_number = account_generator_distributor_street_number_textbox.Text;
                string plz_a_city = account_generator_distributor_plz_city_textbox.Text;
                string country = account_generator_distributor_country_combobox.Text;

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

                    //Check if custom plugins should be loaded
                    if (settings_chrome_default_profile_checkbox.Checked)
                    {
                        options.AddArguments("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                    }
                    else
                    {
                        options.AddArguments("--incognito");
                    }

                    IWebDriver driver = new ChromeDriver(@"C:\Windows\System32\", options);

                    hide_chromedriver();

                    driver.Navigate().GoToUrl("https://cockpit.recordjet.com/login");

                    driver.FindElement(By.Name("email")).SendKeys(mail_account);
                    driver.FindElement(By.Name("password")).SendKeys(dis_password);
                    driver.FindElement(By.ClassName("submit")).Click();

                    MessageBox.Show("Confirm Captcha.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    driver.FindElement(By.LinkText("Mein Konto")).Click();

                    Thread.Sleep(1000);

                    driver.FindElement(By.LinkText("Verfügbares Guthaben auszahlen")).Click();

                    Thread.Sleep(1000);

                    string money = driver.FindElement(By.XPath("//h3[@class='price_title']//span[1]")).Text;

                    driver.FindElement(By.Name("amount1_eur")).Clear();
                    driver.FindElement(By.Name("amount1_eur")).SendKeys(money.Substring(3));

                    driver.FindElement(By.Name("iban")).SendKeys(iban);
                    driver.FindElement(By.Name("swift")).SendKeys(bic);

                    driver.FindElement(By.Name("bank_name")).SendKeys(account_owner);

                    driver.FindElement(By.Name("streetNumber")).SendKeys(street_a_number);
                    driver.FindElement(By.Name("postalCodeCity")).SendKeys(plz_a_city);

                    driver.FindElement(By.Name("country")).SendKeys(country);

                    driver.FindElement(By.Name("password1")).SendKeys(dis_password);
                    driver.FindElement(By.Id("submitBankPayout")).Click();
                    Thread.Sleep(1500);
                    driver.FindElement(By.ClassName("submit")).Click();

                    Thread.Sleep(60000);

                    driver.Navigate().GoToUrl(mail_domain);

                    bool mail_login_flag = true;
                    int mail_login_tries_count = 0;
                    int mail_login_register_max_tries = 30;
                    while (mail_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.Id("rcmloginuser")).SendKeys(mail_account);
                            driver.FindElement(By.Id("rcmloginpwd")).SendKeys(mail_password);
                            driver.FindElement(By.Id("rcmloginsubmit")).Click();

                            mail_login_flag = false;
                        }
                        catch
                        {
                            if (++mail_login_tries_count == mail_login_register_max_tries)
                            {
                                MessageBox.Show("Mail login could not be completed. Failed after several tries.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                driver.Dispose();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }

                    try
                    {
                        driver.FindElement(By.XPath("//span[text()='Auszahlungsbestätigung']")).Click();
                        driver.FindElement(By.XPath("//span[text()='Auszahlungsbestätigung']")).Click();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    try
                    {
                        driver.FindElement(By.XPath("(//img[@alt='image'])[3]")).Click();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    Thread.Sleep(5000);

                    driver.Close();

                    Thread.Sleep(1000);

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                    }
                };
                mk.BeginInvoke(callbackfunction, null);
            }
        }

        private void artist_manager_songs_delete_song_button_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure to delete this song?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int index = artist_manager_artists_listbox.SelectedIndex;

                MySqlConnection conn = new MySqlConnection(connStr);
                {
                    try
                    {
                        conn.Open();

                        string sql = "DELETE FROM `songs` WHERE artist_name = '" + artist_manager_artists_listbox.SelectedItem.ToString() + "' AND song_name = '" + artist_manager_songs_listbox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        conn.Close();

                        artist_manager_artists_listbox.SelectedIndex = -1;
                        artist_manager_artists_listbox.SelectedIndex = index;

                        this.Alert("Song deleted successfully.", Helper.Form_Alert.enmType.Success);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Artist: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        return;
                    }
                }
            }
        }

        private void playlist_creator_spotify_clear_playlistname_textbox_button_Click(object sender, EventArgs e)
        {
            playlist_creator_spotify_playlist_name_combobox.Items.Clear();

            this.Alert("Playlist Names cleared.", Helper.Form_Alert.enmType.Success);
        }

        private void playlist_creator_spotify_load_textfile_button_Click(object sender, EventArgs e)
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

                    MySqlConnection conn = new MySqlConnection(connStr);
                    conn.Open();

                    foreach (var line in File.ReadLines(file_path))
                    {
                        playlist_creator_spotify_playlist_name_combobox.Items.Add(line);
                    }

                    this.Alert("Playlist Names imported.", Helper.Form_Alert.enmType.Success);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Import Playlist Names Words (Spotify): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void artist_manager_add_song_button_Click(object sender, EventArgs e)
        {
            int index = artist_manager_artists_listbox.SelectedIndex;

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "INSERT INTO `" + mysql_database + "`.`songs` (`spotify_url`, `artist_name`, `song_name`) VALUES ('" + artist_manager_add_song_url_textbox.Text + "', '" + artist_manager_artists_listbox.SelectedItem.ToString() + "', '" + artist_manager_add_song_name_textbox.Text + "');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                artist_manager_artists_listbox.SelectedIndex = -1;
                artist_manager_artists_listbox.SelectedIndex = index;

                artist_manager_add_song_name_textbox.Clear();
                artist_manager_add_song_url_textbox.Clear();

                this.Alert("Song URL added.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void artist_manager_delete_song_button_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure to delete this song?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                //int index = artist_manager_artists_listbox.SelectedIndex;

                MySqlConnection conn = new MySqlConnection(connStr);
                {
                    try
                    {
                        conn.Open();

                        string sql = "DELETE FROM `songs` WHERE artist_name = '" + artist_manager_artists_listbox.SelectedItem.ToString() + "' AND song_name = '" + artist_manager_song_overview_datagridview.SelectedCells[1].Value.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        conn.Close();

                        artist_manager_song_overview_datagridview.Rows.RemoveAt(this.artist_manager_song_overview_datagridview.SelectedRows[0].Index);

                        //artist_manager_artists_listbox.SelectedIndex = -1;
                        //artist_manager_artists_listbox.SelectedIndex = index;

                        this.Alert("Song deleted successfully.", Helper.Form_Alert.enmType.Success);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Artist: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        return;
                    }
                }
            }
        }

        //Update Song Info
        private void artist_manager_update_song_button_Click(object sender, EventArgs e)
        {
            int index = artist_manager_artists_listbox.SelectedIndex;

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "UPDATE `" + mysql_database + "`.`songs` SET `spotify_url`='" + artist_manager_add_song_url_textbox.Text + "', `song_name`='" + artist_manager_add_song_name_textbox.Text + "' WHERE `artist_name`='" + artist_manager_song_overview_datagridview.SelectedCells[0].Value.ToString() + "' AND song_name = '" + artist_manager_song_overview_datagridview.SelectedCells[1].Value.ToString() + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                artist_manager_artists_listbox.SelectedIndex = -1;
                artist_manager_artists_listbox.SelectedIndex = index;

                artist_manager_add_song_name_textbox.Clear();
                artist_manager_add_song_url_textbox.Clear();

                this.Alert("Song information updated.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        //Load Current Song Details to Editing Area
        private void artist_manager_song_overview_datagridview_MouseClick(object sender, MouseEventArgs e)
        {
            artist_manager_add_song_name_textbox.Text = artist_manager_song_overview_datagridview.SelectedCells[1].Value.ToString();
            artist_manager_add_song_url_textbox.Text = artist_manager_song_overview_datagridview.SelectedCells[2].Value.ToString();

        }

        private void header_menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void artist_manager_import_songs_list_button_Click(object sender, EventArgs e)
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

                MySqlConnection conn = new MySqlConnection(connStr);
                {
                    try
                    {
                        conn.Open();

                        foreach (var line in File.ReadLines(file_path))
                        {
                            string sql = "INSERT INTO `songs` (`artist_name`, `song_name`) VALUES ('" + artist_manager_artists_listbox.SelectedItem.ToString() + "', '" + line.ToString() + "');";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();

                            artist_manager_songs_listbox.Items.Add(line.ToString());
                        }

                        conn.Close();

                        this.Alert("Song list imported.", Helper.Form_Alert.enmType.Success);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Import Songs List: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        return;
                    }
                }
            }
        }

        private void playlist_creator_loader()
        {
            playlist_creator_spotify_song_url_listbox.Items.Clear();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                //Load Song Urls to listbox
                try
                {
                    string songs_sql = "";

                    if (playlist_creator_spotify_selected_hive_combobox.SelectedIndex == -1)
                    {
                        songs_sql = "SELECT spotify_url FROM songs WHERE spotify_url;";
                    }
                    else
                    {
                        songs_sql = "SELECT spotify_url FROM songs WHERE spotify_url IS NOT NULL AND hive_name = '" + playlist_creator_spotify_selected_hive_combobox.SelectedItem.ToString() + "';";
                    }

                    MySqlCommand songs_cmd = new MySqlCommand(songs_sql, conn);
                    MySqlDataReader songs_rdr = songs_cmd.ExecuteReader();

                    if (songs_rdr.HasRows)
                    {
                        while (songs_rdr.Read())
                        {
                            playlist_creator_spotify_song_url_listbox.Items.Add(songs_rdr[0]);
                        }
                    }
                    songs_rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Spotify Playlist Creator Song URLs from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");
                }

                //Fill Playlist Creator Song overview
                try
                {
                    string sql = "";

                    if (playlist_creator_spotify_selected_hive_combobox.SelectedIndex == -1)
                    {
                        sql = "SELECT artist_name, song_name, spotify_url, hive_name FROM songs WHERE spotify_url;";
                    }
                    else
                    {
                        sql = "SELECT artist_name, song_name, spotify_url, hive_name FROM songs WHERE spotify_url IS NOT NULL AND hive_name = '" + playlist_creator_spotify_selected_hive_combobox.SelectedItem.ToString() + "';";
                    }

                    MySqlDataAdapter mysql_data_adapter = new MySqlDataAdapter();
                    mysql_data_adapter.SelectCommand = new MySqlCommand(sql, conn);

                    DataTable mysql_table = new DataTable();
                    mysql_data_adapter.Fill(mysql_table);

                    playlist_generator_spotify_song_datagridview.DataSource = mysql_table;

                    //Change Colum Titles to look more user friendly
                    playlist_generator_spotify_song_datagridview.Columns[0].HeaderText = "Artist";
                    playlist_generator_spotify_song_datagridview.Columns[1].HeaderText = "Song";
                    playlist_generator_spotify_song_datagridview.Columns[2].HeaderText = "Spotify URL";
                    playlist_generator_spotify_song_datagridview.Columns[3].HeaderText = "Hive Name";
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Playlist Creator Overview from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Spotify Playlist Creator Song URLs from Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;
            }
        }

        private void client_useragent_name_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            //Useragent Loader
            try
            {
                conn.Open();

                string sql = "SELECT useragent_string FROM useragents WHERE useragent_name = '" + client_useragent_name_combobox.SelectedItem.ToString() + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        client_useragent_string_textbox.Text = rdr[0].ToString();
                    }
                }

                rdr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load selected Useragent String: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;

                return;
            }
        }

        //Add Useragent to Database
        private void useragent_manager_add_useragent_button_Click(object sender, EventArgs e)
        {
            if (useragent_manager_useragents_combobox.Items.Contains(useragent_manager_add_useragent_name_textbox.Text))
            {
                this.Alert("Useragent already existing.", Helper.Form_Alert.enmType.Error);
                return;
            }

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string statement_autorestart_time_sql = "INSERT INTO `" + mysql_database + "`.`useragents` (`useragent_name`, `useragent_string`) VALUES ('" + useragent_manager_add_useragent_name_textbox.Text + "', '" + useragent_manager_add_useragent_string_textbox.Text + "');";
                MySqlCommand statement_autorestart_time_cmd = new MySqlCommand(statement_autorestart_time_sql, conn);
                statement_autorestart_time_cmd.ExecuteNonQuery();

                conn.Close();

                useragent_manager_useragents_combobox.Items.Add(useragent_manager_add_useragent_name_textbox.Text);

                this.Alert("Useragent added.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Useragent to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;

                return;
            }

        }

        //Select Useragent
        private void useragent_manager_useragents_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "SELECT useragent_string FROM useragents WHERE useragent_name = '" + useragent_manager_useragents_combobox.SelectedItem.ToString() + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        useragent_manager_useragent_string_textbox.Text = rdr[0].ToString();
                    }
                }

                rdr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load selected Useragent String: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;

                return;
            }
        }

        //Update Useragent 
        private void useragent_manager_useragent_update_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "UPDATE `" + mysql_database + "`.`useragents` SET `useragent_string`= '" + useragent_manager_useragent_string_textbox.Text + "' WHERE useragent_name = '" + useragent_manager_useragents_combobox.SelectedItem.ToString() + "';";
                MySqlCommand sql_cmd = new MySqlCommand(sql, conn);
                sql_cmd.ExecuteNonQuery();

                conn.Close();

                this.Alert("Useragent updated.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Update Useragent String: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;

                return;
            }
        }

        //Delete Useragent 
        private void useragent_manager_useragent_delete_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `" + mysql_database + "`.`useragents` WHERE useragent_name = '" + useragent_manager_useragents_combobox.SelectedItem.ToString() + "';";
                MySqlCommand sql_cmd = new MySqlCommand(sql, conn);
                sql_cmd.ExecuteNonQuery();

                conn.Close();

                useragent_manager_useragents_combobox.Items.Remove(useragent_manager_useragents_combobox.SelectedItem.ToString());

                this.Alert("Useragent deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Update Useragent String: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;

                return;
            }
        }

        private void client_play_time_sheduler_randomize_button_Click(object sender, EventArgs e)
        {
            Random random_max_count = new Random();
            int random_max_count_int = random_max_count.Next(5000, 20000);

            int number_count = 0;
            int number_max_count = random_max_count_int;

            while (number_count != number_max_count)
            {
                Random random_number = new Random();
                int random_number_int = random_number.Next(0, 6);

                if (random_number_int == 0)
                {
                    if (client_play_time_sheduler_monday_checkbox.Checked)
                    {
                        client_play_time_sheduler_monday_checkbox.Checked = false;
                    }
                    else
                    {
                        client_play_time_sheduler_monday_checkbox.Checked = true;
                    }
                }

                if (random_number_int == 1)
                {
                    if (client_play_time_sheduler_tuesday_checkbox.Checked)
                    {
                        client_play_time_sheduler_tuesday_checkbox.Checked = false;
                    }
                    else
                    {
                        client_play_time_sheduler_tuesday_checkbox.Checked = true;
                    }
                }

                if (random_number_int == 2)
                {
                    if (client_play_time_sheduler_wednesday_checkbox.Checked)
                    {
                        client_play_time_sheduler_wednesday_checkbox.Checked = false;
                    }
                    else
                    {
                        client_play_time_sheduler_wednesday_checkbox.Checked = true;
                    }
                }

                if (random_number_int == 3)
                {
                    if (client_play_time_sheduler_thursday_checkbox.Checked)
                    {
                        client_play_time_sheduler_thursday_checkbox.Checked = false;
                    }
                    else
                    {
                        client_play_time_sheduler_thursday_checkbox.Checked = true;
                    }
                }

                if (random_number_int == 4)
                {
                    if (client_play_time_sheduler_friday_checkbox.Checked)
                    {
                        client_play_time_sheduler_friday_checkbox.Checked = false;
                    }
                    else
                    {
                        client_play_time_sheduler_friday_checkbox.Checked = true;
                    }
                }

                if (random_number_int == 5)
                {
                    if (client_play_time_sheduler_saturday_checkbox.Checked)
                    {
                        client_play_time_sheduler_saturday_checkbox.Checked = false;
                    }
                    else
                    {
                        client_play_time_sheduler_saturday_checkbox.Checked = true;
                    }
                }

                if (random_number_int == 6)
                {
                    if (client_play_time_sheduler_sunday_checkbox.Checked)
                    {
                        client_play_time_sheduler_sunday_checkbox.Checked = false;
                    }
                    else
                    {
                        client_play_time_sheduler_sunday_checkbox.Checked = true;
                    }
                }

                number_count++;
            }
        }

        private void client_spotify_assigned_credentials_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < client_spotify_assigned_credentials_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(187, 1, 18) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = client_spotify_assigned_credentials_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, client_spotify_assigned_credentials_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void client_spotify_availiable_credentials_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < client_spotify_availiable_credentials_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(187, 1, 18) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = client_spotify_availiable_credentials_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, client_spotify_availiable_credentials_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void client_spotify_credentials_move_to_client_button_Click(object sender, EventArgs e)
        {
            if (client_spotify_availiable_credentials_listbox.SelectedItems.Count > 0)
            {
                string selected_item = client_spotify_availiable_credentials_listbox.SelectedItem.ToString();

                Random random = new Random();
                int randomNumber = random.Next(0, openvpn_profiles_listbox.Items.Count);

                string openvpn_profile = openvpn_profiles_listbox.Items[randomNumber].ToString();

                string credentials_openvpn_profile = "";
                string credentials_cookies_file = "";
                string credentials_password = "";

                //Collect all running processes, delete all processes from database and insert all running processes, ignoring duplicates
                MySqlConnection conn = new MySqlConnection(connStr);
                {
                    try
                    {
                        conn.Open();

                        string sql1 = "SELECT openvpn_profile, cookies_file, password FROM spotify_credentials WHERE username = '" + client_spotify_availiable_credentials_listbox.SelectedItem.ToString() + "';";
                        MySqlCommand cmd1 = new MySqlCommand(sql1, conn);
                        MySqlDataReader rdr = cmd1.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                credentials_openvpn_profile = rdr[0].ToString();
                                credentials_cookies_file = rdr[1].ToString();
                                credentials_password = rdr[2].ToString();
                            }
                        }
                        rdr.Close();

                        if (credentials_openvpn_profile == "-")
                        {
                            string sql = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `client_name`='" + clients_listbox.SelectedItem.ToString() + "', `hive_name`='" + clients_general_settings_hive_combobox.SelectedItem.ToString() + "', `openvpn_profile`='" + openvpn_profile + "' WHERE `username`='" + client_spotify_availiable_credentials_listbox.SelectedItem.ToString() + "';";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            string sql = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `client_name`='" + clients_listbox.SelectedItem.ToString() + "', `hive_name`='" + clients_general_settings_hive_combobox.SelectedItem.ToString() + "' WHERE `username`='" + client_spotify_availiable_credentials_listbox.SelectedItem.ToString() + "';";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }

                        try
                        {
                            string sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_account`='" + client_spotify_availiable_credentials_listbox.SelectedItem.ToString() + "', `spotify_player_password`='" + credentials_password + "', `cookies_file`='" + credentials_cookies_file + "' WHERE `client_name`='" + clients_listbox.SelectedItem.ToString() + "';";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch(Exception ex)
                        {
                            Logging.Logging.log_error(this.Name, "Add Credentials to Client", ex.Message);
                        }

                        //Move Item after selection
                        client_spotify_availiable_credentials_listbox.Items.Remove(selected_item);
                        client_spotify_assigned_credentials_listbox.Items.Add(selected_item);
                        client_spotify_selected_credentials_combobox.Items.Add(selected_item);

                        conn.Close();

                        client_spotify_assigned_credentials_listbox.SelectedIndex = client_spotify_assigned_credentials_listbox.FindStringExact(selected_item);

                        this.Alert("Credentials assigned.", Helper.Form_Alert.enmType.Success);
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Move availiable Credential to Assigned: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        player_menu_tabcontrol.Enabled = true;

                        return;
                    }
                }
            }
        }

        private void client_spotify_credentials_move_to_availiable_credentials_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (client_spotify_assigned_credentials_listbox.SelectedItems.Count > 0)
                {
                    string selected_item = client_spotify_assigned_credentials_listbox.SelectedItem.ToString();

                    client_spotify_selected_credentials_combobox.SelectedIndex = 0;

                    //Collect all running processes, delete all processes from database and insert all running processes, ignoring duplicates
                    MySqlConnection conn = new MySqlConnection(connStr);
                    {
                        try
                        {
                            conn.Open();

                            string sql = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `client_name`='-', `hive_name`='-' WHERE `username`='" + client_spotify_assigned_credentials_listbox.SelectedItem.ToString() + "';";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();

                            //Move Item after selection
                            client_spotify_assigned_credentials_listbox.Items.Remove(selected_item);
                            client_spotify_selected_credentials_combobox.Items.Remove(selected_item);
                            client_spotify_availiable_credentials_listbox.Items.Add(selected_item);

                            conn.Close();

                            client_spotify_availiable_credentials_listbox.SelectedIndex = client_spotify_availiable_credentials_listbox.FindStringExact(selected_item);

                            this.Alert("Credentials removed.", Helper.Form_Alert.enmType.Success);
                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Move availiable Credential to Assigned: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");

                            player_menu_tabcontrol.Enabled = true;

                            return;
                        }
                    }
                }
            }
            catch
            {

            }
            
        }

        private void misc_cpanel_mail_gen_stop_instance_button_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("chrome"))
            {
                process.Kill();
            }

            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
        }

        private void misc_cpanel_mail_gen_start_instance_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (misc_mail_account_generator_cpanel_use_imported_content_checkbox.Checked)
                {
                    if (misc_account_generator_cpanel_mail_accounts_combobox.Items.Count == 0)
                    {
                        this.Alert("Please import content.", Helper.Form_Alert.enmType.Error);

                        return;
                    }
                }
                else if (settings_cpanel_url_textbox.Text == "" || settings_cpanel_username_textbox.Text == "" || settings_cpanel_password_textbox.Text == "" || misc_cpanel_mail_account_password_textbox.Text == "" || misc_cpanel_mail_account_amount_textbox.Text == "")
                {
                    this.Alert("Please fill all details.", Helper.Form_Alert.enmType.Error);

                    return;
                }

                if (File.Exists(Application.StartupPath + @"\account_generator\cpanel_mail_accounts.txt"))
                {
                    File.Delete(Application.StartupPath + @"\account_generator\cpanel_mail_accounts.txt");
                }

                bool use_imported_content = false;

                string cpanel_url = settings_cpanel_url_textbox.Text;
                string cpanel_username = settings_cpanel_username_textbox.Text;
                string cpanel_password = settings_cpanel_password_textbox.Text;

                string mail_password = misc_cpanel_mail_account_password_textbox.Text;

                int count = 0;
                int count_max = Convert.ToInt32(misc_cpanel_mail_account_amount_textbox.Text);

                if (misc_mail_account_generator_cpanel_use_imported_content_checkbox.Checked)
                    use_imported_content = true;
                else
                    use_imported_content = false;

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

                hide_chromedriver();

                driver.Navigate().GoToUrl(cpanel_url);

                driver.FindElement(By.Id("user")).SendKeys(cpanel_username);
                driver.FindElement(By.Id("pass")).SendKeys(cpanel_password);
                driver.FindElement(By.Id("login_submit")).Click();

                Thread.Sleep(5000);

                driver.FindElement(By.Id("item_email_accounts")).Click();

                Thread.Sleep(1500);

                driver.FindElement(By.Id("btnCreateEmailAccount")).Click();

                Thread.Sleep(1500);

                //driver.FindElement(By.Id("unlimitedQuota")).Click();

                driver.FindElement(By.Id("stay")).Click();

                if (use_imported_content)
                {
                    foreach (var item in misc_account_generator_cpanel_mail_accounts_combobox.Items)
                    {
                        driver.FindElement(By.Id("txtUserName")).Clear();
                        driver.FindElement(By.Id("txtEmailPassword")).Clear();

                        Thread.Sleep(1000);

                        driver.FindElement(By.Id("txtUserName")).SendKeys(item.ToString());
                        driver.FindElement(By.Id("txtEmailPassword")).SendKeys(mail_password);

                        Thread.Sleep(500);

                        driver.FindElement(By.Id("btnCreateEmailAccount")).Click();

                        File.AppendAllText(Application.StartupPath + @"\account_generator\cpanel_mail_accounts.txt", item.ToString() + Environment.NewLine);

                        Thread.Sleep(5000);
                        count++;
                    }
                }
                else
                {
                    while (count != count_max)
                    {
                        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

                        var nicknameChars = new char[15];
                        var randomnickname = new Random();

                        for (int i = 0; i < nicknameChars.Length; i++)
                        {
                            nicknameChars[i] = chars[randomnickname.Next(chars.Length)];
                        }

                        var finalnickname = new String(nicknameChars);

                        driver.FindElement(By.Id("txtUserName")).Clear();
                        driver.FindElement(By.Id("txtEmailPassword")).Clear();

                        Thread.Sleep(1000);

                        driver.FindElement(By.Id("txtUserName")).SendKeys(finalnickname);
                        driver.FindElement(By.Id("txtEmailPassword")).SendKeys(mail_password);

                        Thread.Sleep(500);

                        driver.FindElement(By.Id("btnCreateEmailAccount")).Click();

                        File.AppendAllText(Application.StartupPath + @"\account_generator\cpanel_mail_accounts.txt", finalnickname + Environment.NewLine);

                        Thread.Sleep(5000);
                        count++;
                    }
                }

                this.BeginInvoke(new MethodInvoker(cpanel_acc_gen_complete_alert));

                foreach (var process in Process.GetProcessesByName("chromedriver"))
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Mail Account Generator (cPanel): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void cpanel_acc_gen_complete_alert()
        {
            this.Alert("Successfully finished.", Helper.Form_Alert.enmType.Success);
        }

        private void misc_cpanel_mail_account_password_textbox_TextChanged(object sender, EventArgs e)
        {
            string cpanel_mail_password = misc_cpanel_mail_account_password_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `cpanel_mail_password`='" + StringCipher.Encrypt(cpanel_mail_password, Settings.Encryption_Key) + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save cPanel Mail Account Password: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void misc_cpanel_mail_account_amount_textbox_TextChanged(object sender, EventArgs e)
        {
            string cpanel_mail_amount = misc_cpanel_mail_account_amount_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `cpanel_mail_amount`='" + cpanel_mail_amount + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save cPanel Mail Acc Amount : " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void misc_cpanel_import_mail_button_Click(object sender, EventArgs e)
        {
            try
            {
                string file_path;

                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Downloads";
                openFileDialog.Filter = "Autobot File (*.autobot)|*.autobot|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file_path = openFileDialog.FileName;

                    int lineCount = File.ReadLines(file_path).Count();

                    foreach (string line in File.ReadLines(file_path))
                    {
                        if (misc_account_generator_cpanel_mail_accounts_combobox.Items.Contains(line) == false)
                        {
                            misc_account_generator_cpanel_mail_accounts_combobox.Items.Add(line);
                        }
                        else
                        {
                            File.AppendAllText(Application.StartupPath + @"\account_generator\cpanel_mail_accounts_not_imported.log", line + Environment.NewLine);
                        }
                    }

                    misc_account_generator_cpanel_mail_accounts_combobox.SelectedIndex = 0;

                    int imported_count = misc_account_generator_cpanel_mail_accounts_combobox.Items.Count;

                    this.Alert("Imported " + imported_count + " of " + lineCount + ".", Helper.Form_Alert.enmType.Success);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Import File Content (cPanel): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void misc_cpanel_mail_clear_accounts_button_Click(object sender, EventArgs e)
        {
            misc_account_generator_cpanel_mail_accounts_combobox.Items.Clear();
        }

        private void misc_cpanel_mail_gen_use_imported_content_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (misc_mail_account_generator_cpanel_use_imported_content_checkbox.Checked)
                misc_cpanel_mail_account_amount_textbox.Enabled = false;
            else
                misc_cpanel_mail_account_amount_textbox.Enabled = true;
        }

        private void credentials_spotify_import_credentials_button_Click(object sender, EventArgs e)
        {
            try
            {
                string file_path;

                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = @"C:\Users\" + Environment.UserName + @"\Downloads";
                openFileDialog.Filter = "Autobot File (*.autobot)|*.autobot|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file_path = openFileDialog.FileName;

                    MySqlConnection conn = new MySqlConnection(connStr);
                    conn.Open();

                    foreach (var line in File.ReadLines(file_path))
                    {
                        try
                        {
                            //string sql = "INSERT INTO `spotify_credentials` (`username`, `password`) VALUES ('" + line + "', '" + StringCipher.Encrypt(credentials_spotify_import_credentials_password_textbox.Text, Settings.Encryption_Key) + "') WHERE username NOT ;";
                            string sql = "INSERT INTO spotify_credentials (`username`, `password`) SELECT * FROM (SELECT '" + line + "', '" + StringCipher.Encrypt(credentials_spotify_import_credentials_password_textbox.Text, Settings.Encryption_Key) + "') AS tmp WHERE NOT EXISTS (SELECT username FROM spotify_credentials WHERE username = '" + line + "') LIMIT 1;";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Import Usernames into Database: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");

                            return;
                        }
                    }

                    conn.Close();

                    client_bomb_loader();

                    this.Alert("Accounts successfully imported.", Helper.Form_Alert.enmType.Success);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Account Generator, import Accounts (Spotify): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void misc_spotify_mail_registration_confirmer_accounts_import_button_Click(object sender, EventArgs e)
        {
            try
            {
                string file_path;

                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = Application.StartupPath + @"\account_generator";
                openFileDialog.Filter = "Autobot File(*.autobot)|*.autobot|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 0;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    file_path = openFileDialog.FileName;

                    int lineCount = File.ReadLines(file_path).Count();

                    foreach (string line in File.ReadLines(file_path))
                    {
                        if (misc_spotify_mail_registration_confirmer_accounts_combobox.Items.Contains(line) == false)
                        {
                            misc_spotify_mail_registration_confirmer_accounts_combobox.Items.Add(line);
                        }
                        else
                        {
                            File.AppendAllText(Application.StartupPath + @"\account_generator\roundcube_mail_accounts_not_imported.log", line + Environment.NewLine);
                        }
                    }

                    misc_spotify_mail_registration_confirmer_accounts_combobox.SelectedIndex = 0;

                    int imported_count = misc_spotify_mail_registration_confirmer_accounts_combobox.Items.Count;

                    this.Alert("Imported " + imported_count + " of " + lineCount + ".", Helper.Form_Alert.enmType.Success);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Import File Content (Roundcube): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void misc_spotify_mail_registration_confirmer_accounts_clear_button_Click(object sender, EventArgs e)
        {
            misc_spotify_mail_registration_confirmer_accounts_combobox.Items.Clear();
        }

        private void misc_spotify_mail_registration_confirmer_start_instance_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (misc_mail_account_generator_cpanel_use_imported_content_checkbox.Checked)
                {
                    if (misc_account_generator_cpanel_mail_accounts_combobox.Items.Count == 0)
                    {
                        this.Alert("Please import content.", Helper.Form_Alert.enmType.Error);

                        return;
                    }
                }
                else if (settings_cpanel_url_textbox.Text == "" || settings_cpanel_username_textbox.Text == "" || settings_cpanel_password_textbox.Text == "" || misc_cpanel_mail_account_password_textbox.Text == "" || misc_cpanel_mail_account_amount_textbox.Text == "")
                {
                    this.Alert("Please fill all details.", Helper.Form_Alert.enmType.Error);

                    return;
                }

                string cpanel_url = settings_cpanel_url_textbox.Text;
                string cpanel_username = settings_cpanel_username_textbox.Text;
                string cpanel_password = settings_cpanel_password_textbox.Text;

                string mail_password = misc_spotify_mail_registration_confirmer_password_textbox.Text;

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

                hide_chromedriver();

                driver.Navigate().GoToUrl(cpanel_url);

                driver.FindElement(By.Id("user")).SendKeys(cpanel_username);
                driver.FindElement(By.Id("pass")).SendKeys(cpanel_password);
                driver.FindElement(By.Id("login_submit")).Click();

                Thread.Sleep(5000);

                driver.FindElement(By.Id("item_email_accounts")).Click();

                Thread.Sleep(1500);

                driver.FindElement(By.Id("btnCreateEmailAccount")).Click();

                Thread.Sleep(1500);

                driver.FindElement(By.Id("unlimitedQuota")).Click();

                driver.FindElement(By.Id("stay")).Click();

                foreach (var item in misc_spotify_mail_registration_confirmer_accounts_combobox.Items)
                {
                    driver.FindElement(By.Id("txtUserName")).Clear();
                    driver.FindElement(By.Id("txtEmailPassword")).Clear();

                    Thread.Sleep(1000);

                    driver.FindElement(By.Id("txtUserName")).SendKeys(item.ToString());
                    driver.FindElement(By.Id("txtEmailPassword")).SendKeys(mail_password);

                    Thread.Sleep(500);

                    driver.FindElement(By.Id("btnCreateEmailAccount")).Click();

                    File.AppendAllText(Application.StartupPath + @"\account_generator\cpanel_mail_accounts.autobot", item.ToString() + Environment.NewLine);

                    Thread.Sleep(2500);

                    misc_spotify_mail_registration_confirmer_accounts_combobox.SelectedIndex++;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Confirm Spotify Registration Mails (Roundcube): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void misc_spotify_mail_registration_confirmer_roundcube_url_textbox_TextChanged(object sender, EventArgs e)
        {
            string roundcube_url = misc_spotify_mail_registration_confirmer_roundcube_url_textbox.Text;

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                MethodInvoker mk = delegate
                {
                    conn.Open();

                    string sql = "UPDATE `" + mysql_database + "`.`account_generator` SET `roundcube_url`='" + StringCipher.Encrypt(roundcube_url, Settings.Encryption_Key) + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                };
                mk.BeginInvoke(callbackfunction, null);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Roundcube URL: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void settings_artist_manager_save_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string mail_password_sql = "UPDATE `" + mysql_database + "`.`settings` SET `artist_manager_roundcube_mail_domain`='" + StringCipher.Encrypt(settings_artist_manager_mail_domain_textbox.Text, Settings.Encryption_Key) + "', `artist_manager_roundcube_mail_password`='" + StringCipher.Encrypt(settings_artist_manager_mail_password_textbox.Text, Settings.Encryption_Key) + "';";
                MySqlCommand mail_password_cmd = new MySqlCommand(mail_password_sql, conn);
                mail_password_cmd.ExecuteNonQuery();

                conn.Close();

                this.Alert("Saved.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings (Artist Manager): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void settings_cpanel_save_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string mail_password_sql = "UPDATE `" + mysql_database + "`.`settings` SET `cpanel_url`='" + StringCipher.Encrypt(settings_cpanel_url_textbox.Text, Settings.Encryption_Key) + "', `cpanel_username`='" + StringCipher.Encrypt(settings_cpanel_username_textbox.Text, Settings.Encryption_Key) + "', `cpanel_password`='" + StringCipher.Encrypt(settings_cpanel_password_textbox.Text, Settings.Encryption_Key) + "';";
                MySqlCommand mail_password_cmd = new MySqlCommand(mail_password_sql, conn);
                mail_password_cmd.ExecuteNonQuery();

                conn.Close();

                this.Alert("Saved.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings (cPanel): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void settings_client_mail_save_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string mail_password_sql = "UPDATE `" + mysql_database + "`.`settings` SET `client_mail_domain`='" + StringCipher.Encrypt(settings_client_mail_domain_textbox.Text, Settings.Encryption_Key) + "', `client_mail_password`='" + StringCipher.Encrypt(settings_client_mail_password_textbox.Text, Settings.Encryption_Key) + "';";
                MySqlCommand mail_password_cmd = new MySqlCommand(mail_password_sql, conn);
                mail_password_cmd.ExecuteNonQuery();

                conn.Close();

                this.Alert("Saved.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings (Client Mail Domain): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void client_spotify_copy_password_button_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(client_spotify_selected_credentials_password_textbox.Text);
        }

        private void reload_button_Click(object sender, EventArgs e)
        {
            client_bomb_loader();
        }

        private void credentials_spotify_outdated_reset_button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(settings_client_mail_domain_textbox.Text))
            {
                string mail_account = credentials_spotify_outdated_datagridview.SelectedCells[0].Value.ToString();
                string mail_password = settings_client_mail_password_textbox.Text;
                string mail_domain = settings_client_mail_domain_textbox.Text;

                DialogResult dialogResult = MessageBox.Show("Press OK to log into the connected Mail Account. A new password will be copied to your clipboard. Just select the spotify reset mail and insert the new password with ctrl + v. The credentials in your database will be updated automatically. Press 'No' if you want to abort this operation.", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    //Generate new random password
                    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    var stringChars = new char[8];
                    var random = new Random();

                    for (int i = 0; i < stringChars.Length; i++)
                    {
                        stringChars[i] = chars[random.Next(chars.Length)];
                    }

                    var new_password = new String(stringChars);

                    credentials_spotify_outdated_new_password_textbox.Text = new_password;

                    //Save new password to spotify credentials
                    MySqlConnection conn = new MySqlConnection(connStr);
                    try
                    {
                        conn.Open();

                        string sql = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `password`='" + StringCipher.Encrypt(new_password, Settings.Encryption_Key) + "', `outdated`='0' WHERE username = '" + mail_account + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Update Spotify Credentials (Outdated Flag): " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        return;
                    }

                    //Save new password to the client that currently selected those
                    try
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_password`='" + StringCipher.Encrypt(new_password, Settings.Encryption_Key) + "' WHERE spotify_player_account = '" + mail_account + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Update Spotify Credentials (Client): " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");

                        return;
                    }

                    client_spotify_selected_credentials_password_textbox.Text = new_password;

                    Clipboard.SetText(new_password);

                    client_bomb_loader();

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

                    hide_chromedriver();

                    //Navigate and Login to Mail
                    driver.Navigate().GoToUrl(mail_domain);

                    bool mail_login_flag = true;
                    int mail_login_tries_count = 0;
                    int mail_login_register_max_tries = 30;
                    while (mail_login_flag == true)
                    {
                        try
                        {
                            driver.FindElement(By.Id("rcmloginuser")).SendKeys(mail_account);
                            driver.FindElement(By.Id("rcmloginpwd")).SendKeys(mail_password);
                            driver.FindElement(By.Id("rcmloginsubmit")).Click();

                            mail_login_flag = false;
                        }
                        catch
                        {
                            if (++mail_login_tries_count == mail_login_register_max_tries)
                            {
                                MessageBox.Show("Mail login could not be completed. Failed after several tries.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                driver.Dispose();
                                return;
                            }
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Mail Server URL Provided. Please add it.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void client_emulate_device_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (client_emulate_device_checkbox.Checked)
            {
                client_useragent_enabled_checkbox.Checked = false;
            }
        }

        private void client_useragent_enabled_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (client_useragent_enabled_checkbox.Checked)
            {
                client_emulate_device_checkbox.Checked = false;
            }
        }

        private void artist_manager_open_songs_folder_button_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath + @"\Artist Manager\" + artist_manager_artists_listbox.SelectedItem.ToString());
        }

        private void settings_add_hive_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
                string sql = "INSERT INTO `" + mysql_database + "`.`hives` (`hive_name`) VALUES ('" + settings_add_hive_textbox.Text + "');";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Hive added.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Add Hive to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void settings_remove_hive_button_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "DELETE FROM `" + mysql_database + "`.`hives` WHERE `hive_name`= '" + settings_hive_combobox.SelectedItem.ToString() + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                client_bomb_loader();

                this.Alert("Credentials deleted.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Remove Hive from Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void clients_hive_filter_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            player_menu_tabcontrol.Enabled = false;
            clients_listbox.Items.Clear();
            
            try
            {
                MySqlConnection conn = new MySqlConnection(connStr);

                conn.Open();

                try
                {
                    string sql1 = "SELECT client_name FROM clients WHERE hive_name = '" + clients_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql1, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            clients_listbox.Items.Add(rdr[0].ToString());
                        }
                    }
                    rdr.Close();

                    conn.Close();
                }
                catch
                {
                    conn.Close();

                    player_menu_tabcontrol.Enabled = true;
                }

                clients_counter_label.Text = clients_listbox.Items.Count.ToString();
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Clients from Database (Hive Selection): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                player_menu_tabcontrol.Enabled = true;
            }

            player_menu_tabcontrol.Enabled = true;
        }

        private void playlist_poly_spotify_selected_hive_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            playlist_poly_spotify_playlist_name_listbox.Items.Clear();

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                //Load selected Hives Playlists
                try
                {
                    string sql = "SELECT url FROM spotify_playlists WHERE type = 'Playlist' AND hive_name = '" + playlist_poly_spotify_selected_hive_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            playlist_poly_spotify_playlist_name_listbox.Items.Add(rdr[0]).ToString();
                        }
                    }
                    rdr.Close();

                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Spotify Playlists from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");
                }
                
                //Load selected hive playlist poly index
                try
                {
                    string sql = "SELECT playlist_poly_index FROM settings;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            playlist_poly_spotify_playlist_name_listbox.SelectedIndex = (int)rdr[0];
                        }
                    }
                    rdr.Close();
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Overall Settings from Database: " + ex.Message + Environment.NewLine);

                    Process.Start(Application.StartupPath + @"\logs\error.log");
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Open SQL Connection (Playlist Poly Index Loader): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void metroTabPage14_Click(object sender, EventArgs e)
        {

        }

        private void artist_manager_selected_hive_filter_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = artist_manager_filter_release_status_combobox.SelectedIndex;

            if (artist_manager_filter_release_status_combobox.SelectedIndex == 0)
                artist_manager_filter_release_status_combobox.SelectedIndex = 1;
            else
                artist_manager_filter_release_status_combobox.SelectedIndex = 0;

            artist_manager_filter_release_status_combobox.SelectedIndex = index;
        }

        private void playlist_creator_spotify_selected_hive_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            playlist_creator_loader();
        }

        private void misc_random_time_sheduler_exception_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < misc_random_time_sheduler_client_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(102, 102, 102) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = misc_random_time_sheduler_client_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, misc_random_time_sheduler_client_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void misc_random_time_sheduler_exception_add_button_Click(object sender, EventArgs e)
        {
            misc_random_time_sheduler_client_listbox.Items.Remove(misc_random_time_sheduler_client_listbox.SelectedItem.ToString());

            misc_random_time_sheduler_client_listbox.SelectedIndex = 0;
        }

        private void misc_random_time_sheduler_randomize_button_Click(object sender, EventArgs e)
        {
            if (misc_random_time_sheduler_hive_filter_combobox.Text == "")
            {
                this.Alert("No Hive selected.", Helper.Form_Alert.enmType.Error);

                return;
            }

            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();

                foreach (var client in misc_random_time_sheduler_client_listbox.Items)
                {
                    Random random_max_count = new Random();
                    int random_max_count_int = random_max_count.Next(5000, 20000);

                    int number_count = 0;
                    int number_max_count = random_max_count_int;

                    while (number_count != number_max_count)
                    {
                        Random random_number = new Random();
                        int random_number_int = random_number.Next(0, 7);

                        if (random_number_int == 0)
                        {
                            if (misc_random_time_sheduler_monday_checkbox.Checked)
                            {
                                misc_random_time_sheduler_monday_checkbox.Checked = false;
                            }
                            else
                            {
                                misc_random_time_sheduler_monday_checkbox.Checked = true;
                            }
                        }

                        if (random_number_int == 1)
                        {
                            if (misc_random_time_sheduler_tuesday_checkbox.Checked)
                            {
                                misc_random_time_sheduler_tuesday_checkbox.Checked = false;
                            }
                            else
                            {
                                misc_random_time_sheduler_tuesday_checkbox.Checked = true;
                            }
                        }

                        if (random_number_int == 2)
                        {
                            if (misc_random_time_sheduler_wednesday_checkbox.Checked)
                            {
                                misc_random_time_sheduler_wednesday_checkbox.Checked = false;
                            }
                            else
                            {
                                misc_random_time_sheduler_wednesday_checkbox.Checked = true;
                            }
                        }

                        if (random_number_int == 3)
                        {
                            if (misc_random_time_sheduler_thursday_checkbox.Checked)
                            {
                                misc_random_time_sheduler_thursday_checkbox.Checked = false;
                            }
                            else
                            {
                                misc_random_time_sheduler_thursday_checkbox.Checked = true;
                            }
                        }

                        if (random_number_int == 4)
                        {
                            if (misc_random_time_sheduler_friday_checkbox.Checked)
                            {
                                misc_random_time_sheduler_friday_checkbox.Checked = false;
                            }
                            else
                            {
                                misc_random_time_sheduler_friday_checkbox.Checked = true;
                            }
                        }

                        if (random_number_int == 5)
                        {
                            if (misc_random_time_sheduler_saturday_checkbox.Checked)
                            {
                                misc_random_time_sheduler_saturday_checkbox.Checked = false;
                            }
                            else
                            {
                                misc_random_time_sheduler_saturday_checkbox.Checked = true;
                            }
                        }

                        if (random_number_int == 6)
                        {
                            if (misc_random_time_sheduler_sunday_checkbox.Checked)
                            {
                                misc_random_time_sheduler_sunday_checkbox.Checked = false;
                            }
                            else
                            {
                                misc_random_time_sheduler_sunday_checkbox.Checked = true;
                            }
                        }

                        number_count++;
                    }

                    //Safe Time Sheduler Settings
                    try
                    {
                        int monday = 0;
                        int tuesday = 0;
                        int wednesday = 0;
                        int thursday = 0;
                        int friday = 0;
                        int saturday = 0;
                        int sunday = 0;

                        if (misc_random_time_sheduler_monday_checkbox.Checked)
                            monday = 1;

                        if (misc_random_time_sheduler_tuesday_checkbox.Checked)
                            tuesday = 1;

                        if (misc_random_time_sheduler_wednesday_checkbox.Checked)
                            wednesday = 1;

                        if (misc_random_time_sheduler_thursday_checkbox.Checked)
                            thursday = 1;

                        if (misc_random_time_sheduler_friday_checkbox.Checked)
                            friday = 1;

                        if (misc_random_time_sheduler_saturday_checkbox.Checked)
                            saturday = 1;

                        if (misc_random_time_sheduler_sunday_checkbox.Checked)
                            sunday = 1;

                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `play_time_sheduler_monday_enabled`='" + monday + "', `play_time_sheduler_tuesday_enabled`='" + tuesday + "', `play_time_sheduler_wednesday_enabled`='" + wednesday + "', `play_time_sheduler_thursday_enabled`='" + thursday + "', `play_time_sheduler_friday_enabled`='" + friday + "', `play_time_sheduler_saturday_enabled`='" + saturday + "', `play_time_sheduler_sunday_enabled`='" + sunday + "' WHERE `client_name`='" + client.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Player Type to Database: " + ex.Message + Environment.NewLine);

                        Process.Start(Application.StartupPath + @"\logs\error.log");
                    }

                    File.AppendAllText(Application.StartupPath + @"\logs\event.log", DateTime.Now + " - Mass Randomize Time Sheduler: " + client.ToString() + " finished." + Environment.NewLine);
                }

                conn.Close();

                MessageBox.Show("Successfully finished mass randomizing time sheduler.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Process.Start(Application.StartupPath + @"\logs\event.log");
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Open MySQL Connection (Misc Randomize Time Sheduler): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void misc_random_time_sheduler_hive_filter_combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                misc_random_time_sheduler_client_listbox.Items.Clear();

                MySqlConnection conn = new MySqlConnection(connStr);

                conn.Open();

                try
                {
                    string sql1 = "SELECT client_name FROM clients WHERE hive_name = '" + misc_random_time_sheduler_hive_filter_combobox.SelectedItem.ToString() + "';";
                    MySqlCommand cmd = new MySqlCommand(sql1, conn);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            misc_random_time_sheduler_client_listbox.Items.Add(rdr[0].ToString());
                        }
                    }
                    rdr.Close();

                    conn.Close();
                }
                catch
                {
                    conn.Close();

                    player_menu_tabcontrol.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Clients from Database for Misc Random(Hive Selection): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void openvpn_generate_login_profiles_button_Click(object sender, EventArgs e)
        {
            SshClient sshclient = new SshClient(settings_ssh_remote_host_textbox.Text, Convert.ToInt32(settings_ssh_port_textbox.Text), settings_ssh_username_textbox.Text, settings_ssh_password_textbox.Text);
            sshclient.Connect();

            foreach (var client in openvpn_clients_profiles_listbox.Items)
            {
                SshCommand sc = sshclient.CreateCommand("cd /home/docker/Profiles/ | echo \"zowmr6KbT0vvlqFmcLSb\" | docker run -v ovpn-data:/etc/openvpn --rm -i kylemanna/openvpn easyrsa build-client-full " + client + " nopass");
                sc.Execute();
                string answer = sc.Result;
                //MessageBox.Show(answer.ToString() + client);

                File.AppendAllText(Application.StartupPath + @"\logs\ssh_ovpn_clients.txt", answer + Environment.NewLine);
            }
        }

        private void openvpn_clients_profiles_listbox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            bool isItemSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
            int itemIndex = e.Index;
            if (itemIndex >= 0 && itemIndex < openvpn_clients_profiles_listbox.Items.Count)
            {
                Graphics g = e.Graphics;

                // Background Color
                SolidBrush backgroundColorBrush = new SolidBrush((isItemSelected) ? Color.FromArgb(187, 1, 18) : Color.White);
                g.FillRectangle(backgroundColorBrush, e.Bounds);

                // Set text color
                string itemText = openvpn_clients_profiles_listbox.Items[itemIndex].ToString();

                SolidBrush itemTextColorBrush = (isItemSelected) ? new SolidBrush(Color.White) : new SolidBrush(Color.Black);
                g.DrawString(itemText, e.Font, itemTextColorBrush, openvpn_clients_profiles_listbox.GetItemRectangle(itemIndex).Location);

                // Clean up
                backgroundColorBrush.Dispose();
                itemTextColorBrush.Dispose();
            }

            e.DrawFocusRectangle();
        }

        private void openvpn_delete_all_login_profiles_button_Click(object sender, EventArgs e)
        {
            SshClient sshclient = new SshClient(settings_ssh_remote_host_textbox.Text, Convert.ToInt32(settings_ssh_port_textbox.Text), settings_ssh_username_textbox.Text, settings_ssh_password_textbox.Text);
            sshclient.Connect();

            foreach (var client in openvpn_clients_profiles_listbox.Items)
            {
                SshCommand sc = sshclient.CreateCommand("cd /home/docker/Profiles/ | echo \"zowmr6KbT0vvlqFmcLSb\" | docker run -v openvpn_data:/etc/openvpn --rm -it martin/openvpn revokeclient " + client);
                sc.Execute();
                string answer = sc.Result;
                //MessageBox.Show(answer.ToString() + client);
            }
        }

        private void openvpn_assign_login_profiles_to_clients_button_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var profile in Directory.GetFiles(Application.StartupPath + @"\OpenVPN_Login_Profiles"))
                {
                    if (File.ReadAllText(profile).Contains("block-outside-dns") == false)
                    {
                        File.AppendAllText(profile, Environment.NewLine + "block-outside-dns");
                    }
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Append block-outside-dns to Login OpenVPN Profiles: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                string openvpn_profile_path = Application.StartupPath + @"\OpenVPN_Login_Profiles\";

                conn.Open();

                foreach (var client in openvpn_clients_profiles_listbox.Items)
                {
                    try
                    {
                        Byte[] bytes_openvpn_profile_data = File.ReadAllBytes(Application.StartupPath + @"\OpenVPN_Login_Profiles\" + client + ".ovpn");
                        string converted_profile = Convert.ToBase64String(bytes_openvpn_profile_data);

                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `openvpn_login_profile`='" + converted_profile + "' WHERE client_name = '" + client + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Assign Login OpenVPN Profiles to Clients: " + ex.Message + Environment.NewLine);
                    }
                }
                
                conn.Close();

                client_bomb_loader();

                this.Alert("OpenVPN Login Profiles assigned to Clients.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings to Database: " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");

                return;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                string sql = "UPDATE `" + mysql_database + "`.`settings` SET `ssh_remote_hostname`='" + StringCipher.Encrypt(settings_ssh_remote_host_textbox.Text, Settings.Encryption_Key) + "', `ssh_username`='" + StringCipher.Encrypt(settings_ssh_username_textbox.Text, Settings.Encryption_Key) + "', `ssh_password`='" + StringCipher.Encrypt(settings_ssh_password_textbox.Text, Settings.Encryption_Key) + "', `ssh_port`='" + StringCipher.Encrypt(settings_ssh_port_textbox.Text, Settings.Encryption_Key) + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                conn.Close();

                this.Alert("Saved.", Helper.Form_Alert.enmType.Success);
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Settings (Client Mail Domain): " + ex.Message + Environment.NewLine);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void openvpn_get_all_configs_button_Click(object sender, EventArgs e)
        {
            SshClient sshclient = new SshClient(settings_ssh_remote_host_textbox.Text, Convert.ToInt32(settings_ssh_port_textbox.Text), settings_ssh_username_textbox.Text, settings_ssh_password_textbox.Text);
            sshclient.Connect();

            foreach (var client in openvpn_clients_profiles_listbox.Items)
            {
                SshCommand sc = sshclient.CreateCommand("docker run -v ovpn-data:/etc/openvpn --rm kylemanna/openvpn ovpn_getclient " + client + " > /home/docker/Profiles/" + client + ".ovpn");
                sc.Execute();
                string answer = sc.Result;
                //MessageBox.Show(answer.ToString() + client);
            }
        }

        private void client_persistent_chrome_profile_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (client_persistent_chrome_profile_checkbox.Checked)
                clients_general_settings_custom_profile_checkbox.Checked = false;
        }

        private void clients_general_settings_custom_profile_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (clients_general_settings_custom_profile_checkbox.Checked)
                client_persistent_chrome_profile_checkbox.Checked = false;
        }

        private void credentials_spotify_outdated_new_password_textbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(credentials_spotify_outdated_new_password_textbox.Text);
            this.Alert("Copied.", Helper.Form_Alert.enmType.Success);
        }

        private void misc_spotify_test_astaroth_button_Click(object sender, EventArgs e)
        {
            var proc = new ProcessStartInfo(@"C:\Program Files\Mozilla Firefox\firefox.exe", "\"https://checkip.perfect-privacy.com/json\"");
            proc.UseShellExecute = false;
            Process.Start(proc);

            Astaroth_Perfect_Privacy.Astaroth_Perfect_Privacy Astaroth_Perfect_Privacy = new Astaroth_Perfect_Privacy.Astaroth_Perfect_Privacy();

            bool firefox_pp_status = false;

            firefox_pp_status = Astaroth_Perfect_Privacy.firefox_pp_status(firefox_pp_status);

            MessageBox.Show(firefox_pp_status.ToString());
            MessageBox.Show("final");

            //firefox_pp_status = Astaroth_Perfect_Privacy.Astaroth_Perfect_Privacy.

            //Declare Astaroth Class

            //Astaroth_Core.Astaroth_Core.SearchPixel("#15883e");

            Astaroth_Spotify_Firefox.Astaroth_Spotify_Firefox Astaroth_Spotify_Firefox = new Astaroth_Spotify_Firefox.Astaroth_Spotify_Firefox();


            //Astaroth_Spotify_Firefox.spotify_login_button_textbox();

            bool login_status = false;

           // login_status = Astaroth_Spotify_Firefox.check_spotify_login_confirmed(login_status);

            if (login_status == true)
            {
                //Astaroth_Spotify_Firefox.login_confirm_url("https://open.spotify.com/playlist/37i9dQZF1DX36edUJpD76c");
                Astaroth_Spotify_Firefox.accept_cookies_normal_button();
            }

            //Astaroth_Spotify_Firefox.play_big_button();

            //Astaroth_Spotify_Firefox.skip_forward_button();

            bool bar_status = false;

            //bar_status = Astaroth_Spotify_Firefox.check_other_device_playing_bar(bar_status);

            //MessageBox.Show(bar_status.ToString());

            //Astaroth_Spotify_Firefox.other_device_playing_icon_button();

           // Astaroth_Spotify_Firefox.other_device_playing_use_this_button();

            bool register_button_status = false;

            register_button_status = Astaroth_Spotify_Firefox.check_register_button(register_button_status);

            MessageBox.Show(register_button_status.ToString());
        }

        private void form_header_Paint(object sender, PaintEventArgs e)
        {

        }
    }           
}