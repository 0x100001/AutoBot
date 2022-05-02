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
using System.Threading;
using Auto_Bot___Client.Player;
using System.ServiceProcess;
using System.Timers;

namespace Auto_Bot___Client
{
    public partial class Main_Form : Form
    {
        public string connStr;

        //Bot Information
        public string username;
        public string password;
        public string client_name;
        public string hive_name;

        //MySQL Information
        public string mysql_host;
        public string mysql_database;
        public string mysql_username;
        public string mysql_password;
        public string mysql_port;

        //Main Bot Settings
        int auto_restart_bot;
        int auto_restart_bot_timer;
        public int custom_useragent_enabled; //Enabled or not
        public string custom_useragent; //Name of the Useragent
        public string custom_useragent_string; //Useragent String
        public int emulate_device_enabled; //Enabled or not
        public string emulate_device_type; //Device Type selected
        public string automation_mode; //Selenium or Astaroth
        public string player_type; //Album or Playlist
        Byte[] cookies_file_bytes; //Album or Playlist
        string cookies_file_encoded; //Album or Playlist
        int cookies_sync_enabled;

        //OpenVPN Configuration
        public int openvpn_enabled;
        int openvpn_randomize_enabled;
        public int openvpn_health_check_enabled;
        int openvpn_bound_credentials_enabled;
        public string openvpn_profile;
        public Byte[] openvpn_login_login;
        public Byte[] openvpn_login_profile_bytes;

        //Time Sheduler Settings
        int bot_active = 0;

        int monday;
        int tuesday;
        int wednesday;
        int thursday;
        int friday;
        int saturday;
        int sunday;

        //Misc
        int reinstall_chrome;
        int reinstall_firefox;
        int renew_windows_trial_licence;
        public int custom_profile_enabled; //Custom Profile (Chrome)

        //Monitoring
        public string bot_runtime;
        public int outdated_credentials_flag = 0;

        //Telegram Monitoring
        public int Telegram_Monitoring;
        public string Telegram_API_Token;
        public string Telegram_Chat_Id;
        public string Telegram_Alert_Chat_Id;
        public string Telegram_Alert_Creds_Chat_Id;

        //Spotify Player Configuration
        int spotify_enabled = 0;

        public string spotify_player_username;
        public string spotify_player_password;
        public int chrome_persistent_profile_enabled;
        public string spotify_playlist_url;

        public int spotify_autoskip_enabled;
        public int spotify_autoskip_forward_from;
        public int spotify_autoskip_forward_to;

        public int spotify_rotate_credentials_enabled;
        public string spotify_next_password;

        //Deezer Player Configuration
        int deezer_enabled = 0;

        public string deezer_player_username;
        public string deezer_player_password;
        public string deezer_playlist_url;

        public int deezer_autoskip_enabled;
        public int deezer_autoskip_forward_from;
        public int deezer_autoskip_forward_to;

        //Astaroth
        public int ss_count = 0;
        public string ss = "ss_";
        public int debug_ss_count = 0;
        public string debug_ss = "ss_";

        private void callbackfunction(IAsyncResult res)
        {

        }

        public Main_Form()
        {
            InitializeComponent();
            database_information_loader();
        }

        private void StopWatchMeth()
        {
            MethodInvoker mk = delegate
            {
                bool loop = true;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                // Get the elapsed time as a TimeSpan value.

                while (loop == true)
                {
                    TimeSpan ts = stopWatch.Elapsed;

                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
                        ts.Hours, ts.Minutes, ts.Seconds / 10);

                    bot_runtime = elapsedTime;
                    runtime_label.Text = bot_runtime;
                    Thread.Sleep(10000);
                }
            };
            mk.BeginInvoke(callbackfunction, null);
        }

        //Update last access function
        public void no_creds_update_last_access()
        {
            //monitoring_timer.Stop();
            outdated_credentials_flag = 1;

            int run_counter = 0;
            int run_max_counter = 24;

            while (run_counter != run_max_counter)
            {
                try
                {
                    MySqlConnection conn = new MySqlConnection(connStr);

                    conn.Open();

                    try
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `last_access`='" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + "' WHERE client_name = '" + client_name + "' AND hive_name = '" + hive_name + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Logging.log_error(this.Name, "Update last access (No Credentials Mode)", ex.Message);
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    Logging.log_error(this.Name, "Update last access (Open Connection)", ex.Message);
                }

                Thread.Sleep(600000);
                run_counter++;
            }

            if (Telegram_Monitoring == 1)
            {
                Telegram.telegram_critical_alert_send("Restarting bot and checking for availiable credentials.");
            }

            Misc.kill_everything_perform_restart();
        }

        //Extend Windows Trial Licence
        private void renew_windows_licence()
        {
            var process = Process.Start("slmgr.vbs", "-rearm //B /ato");
            process.WaitForExit();

            Telegram.telegram_alert_send("Successfully renewed windows trial licence. Restarting.");

            Misc.restart_pc();
        }

        //Load Database Information from Registry
        public void database_information_loader()
        {
            //Read MySQL Host Regkey
            try
            {
                RegistryKey MySQL_Host_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_host = Encryption.StringCipher.Decrypt(MySQL_Host_Regkey.GetValue("MySQL_Host").ToString(), Settings.Encryption_Key);
                    MySQL_Host_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read MySQL Host Information from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            //Read MySQL Database Regkey
            try
            {
                RegistryKey MySQL_Database_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_database = Encryption.StringCipher.Decrypt(MySQL_Database_Regkey.GetValue("MySQL_Database").ToString(), Settings.Encryption_Key);
                    MySQL_Database_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read MySQL Database Information from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            //Read MySQL Username Regkey
            try
            {
                RegistryKey MySQL_Username_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_username = Encryption.StringCipher.Decrypt(MySQL_Username_Regkey.GetValue("MySQL_Username").ToString(), Settings.Encryption_Key);
                    MySQL_Username_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read MySQL Username Information from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            //Read MySQL Password Regkey
            try
            {
                RegistryKey MySQL_Password_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_password = Encryption.StringCipher.Decrypt(MySQL_Password_Regkey.GetValue("MySQL_Password").ToString(), Settings.Encryption_Key);
                    MySQL_Password_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read MySQL Password Information from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            //Read MySQL Port  Regkey
            try
            {
                RegistryKey MySQL_Port_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    mysql_port = Encryption.StringCipher.Decrypt(MySQL_Port_Regkey.GetValue("MySQL_Port").ToString(), Settings.Encryption_Key);
                    MySQL_Port_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read MySQL Port Information from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            //Read Username Regkey
            try
            {
                RegistryKey Username_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    username = Encryption.StringCipher.Decrypt(Username_Regkey.GetValue("Username").ToString(), Settings.Encryption_Key);
                    Username_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read Username from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            //Read Password Regkey
            try
            {
                RegistryKey Password_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    password = Encryption.StringCipher.Decrypt(Password_Regkey.GetValue("Password").ToString(), Settings.Encryption_Key);
                    Password_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read Password from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            //Read Client Name Regkey
            try
            {
                RegistryKey Client_Name_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    client_name = Client_Name_Regkey.GetValue("Client_Name").ToString();
                    Client_Name_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read Client Name from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            //Read Hive Name Regkey
            try
            {
                RegistryKey Hive_Name_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                {
                    hive_name = Hive_Name_Regkey.GetValue("Hive_Name").ToString();
                    Hive_Name_Regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Read Hive Name from Registry", ex.Message);

                Misc.kill_everything_perform_restart();
            }

            client_name_label.Text = "Client Name: " + client_name + " (" + hive_name + ")";

            //Init Connection String
            connStr = "server=" + mysql_host + ";user=" + mysql_username + ";database=" + mysql_database + ";port=" + mysql_port + ";password=" + mysql_password + ";Pooling=false;default command timeout=360;Connection Timeout=60;";
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(StopWatchMeth));

            today_day_label.Text = "Today: " + DateTime.Today.DayOfWeek.ToString();

            //28
            string sql_clients = "SELECT spotify_enabled, deezer_enabled, auto_restart_bot, auto_restart_bot_timer, openvpn_enabled, openvpn_randomize, openvpn_health_check, openvpn_profile, custom_profile_enabled, player_type, play_time_sheduler_monday_enabled, play_time_sheduler_tuesday_enabled, play_time_sheduler_wednesday_enabled, play_time_sheduler_thursday_enabled, play_time_sheduler_friday_enabled, play_time_sheduler_saturday_enabled, play_time_sheduler_sunday_enabled, reinstall_chrome, custom_useragent_enabled, custom_useragent, emulate_device_enabled, emulate_device_type, renew_windows_trial_licence, automation_mode, openvpn_bound_credentials_enabled, openvpn_login_profile, reinstall_firefox, cookies_file, cookies_sync_enabled FROM `" + mysql_database + "`.`clients` WHERE client_name = '" + client_name + "' AND hive_name = '" + hive_name + "';";

            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlCommand settings_cmd = new MySqlCommand(sql_clients, conn);

            //Load Client Settings from Database
            try
            {
                conn.Open();

                //General Settings from selected Client
                MySqlDataReader settings_rdr = settings_cmd.ExecuteReader();
                if (settings_rdr.HasRows)
                {
                    while (settings_rdr.Read())
                    {
                        spotify_enabled = (int)settings_rdr[0];
                        deezer_enabled = (int)settings_rdr[1];
                        auto_restart_bot = (int)settings_rdr[2];
                        auto_restart_bot_timer = (int)settings_rdr[3];
                        custom_useragent_enabled = (int)settings_rdr[18];
                        emulate_device_enabled = (int)settings_rdr[20];
                        automation_mode = settings_rdr[23].ToString();

                        if (emulate_device_enabled == 1)
                        {
                            emulate_device_type = settings_rdr[21].ToString();
                        }

                        if (custom_useragent_enabled == 1)
                        {
                            custom_useragent = settings_rdr[19].ToString();
                            useragent_name_label.Text = "Useragent Name: " + custom_useragent;
                        }

                        openvpn_enabled = (int)settings_rdr[4];
                        openvpn_randomize_enabled = (int)settings_rdr[5];
                        openvpn_health_check_enabled = (int)settings_rdr[6];
                        openvpn_profile = settings_rdr[7].ToString();
                        openvpn_bound_credentials_enabled = (int)settings_rdr[24];
                        openvpn_login_profile_bytes = Convert.FromBase64String(settings_rdr[25].ToString());

                        cookies_sync_enabled = (int)settings_rdr[28];

                        cookies_file_bytes = Convert.FromBase64String(settings_rdr[27].ToString());

                        //Custom Profile
                        custom_profile_enabled = (int)settings_rdr[8];

                        //Get Player Type
                        player_type = settings_rdr[9].ToString();

                        //Get Time Sheduler Settings
                        monday = (int)settings_rdr[10];
                        tuesday = (int)settings_rdr[11];
                        wednesday = (int)settings_rdr[12];
                        thursday = (int)settings_rdr[13];
                        friday = (int)settings_rdr[14];
                        saturday = (int)settings_rdr[15];
                        sunday = (int)settings_rdr[16];

                        //Misc
                        reinstall_chrome = (int)settings_rdr[17];
                        reinstall_firefox = (int)settings_rdr[26];
                        renew_windows_trial_licence = (int)settings_rdr[22];
                    }
                }
                settings_rdr.Close();

                //Read Album URLs if player type is album
                if (player_type == "Album")
                {
                    try
                    {
                        string sql = "SELECT url FROM spotify_playlists WHERE type = 'Album' AND hive_name = '" + hive_name + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                spotify_album_listbox.Items.Add(rdr[0].ToString());
                            }
                        }

                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Album URLs: " + ex.Message + Environment.NewLine);
                    }
                }

                if (custom_useragent_enabled == 1)
                {
                    try
                    {
                        string sql = "SELECT useragent_string FROM useragents WHERE useragent_name = '" + custom_useragent + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();

                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                custom_useragent_string = rdr[0].ToString();
                                useragent_string_label.Text = "Useragent String: " + custom_useragent_string;
                            }
                        }

                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Custom Useragent String: " + ex.Message + Environment.NewLine);
                    }
                }

                //OpenVPN Config Loader
                string sql_openvpn_profile_loader = "SELECT * FROM openvpn_profiles;";
                MySqlCommand spotify_openvpn_profile_loader = new MySqlCommand(sql_openvpn_profile_loader, conn);
                MySqlDataReader openvpn_profile_loader_rdr = spotify_openvpn_profile_loader.ExecuteReader();
                if (openvpn_profile_loader_rdr.HasRows)
                {
                    while (openvpn_profile_loader_rdr.Read())
                    {
                        if (openvpn_profile_loader_rdr[1].ToString() != "login.ovpn")
                            openvpn_profile_combobox.Items.Add(openvpn_profile_loader_rdr[1].ToString());

                        string openvpn_profile_name = openvpn_profile_loader_rdr[1].ToString();
                        string openvpn_profile_data = openvpn_profile_loader_rdr[2].ToString();
                        string openvpn_profile_login_conf_data = openvpn_profile_loader_rdr[3].ToString();

                        Byte[] openvpn_profile_base64_decoded = Convert.FromBase64String(openvpn_profile_data);
                        Byte[] openvpn_login_conf_base64_decoded = Convert.FromBase64String(openvpn_profile_login_conf_data);

                        File.WriteAllBytes(@"C:\Program Files\OpenVPN\config\" + openvpn_profile_name, openvpn_profile_base64_decoded);
                        File.WriteAllBytes(@"C:\Program Files\OpenVPN\config\login.conf", openvpn_login_conf_base64_decoded);
                    }
                    openvpn_profile_loader_rdr.Close();
                }

                //Monitoring Settings Loader
                string monitoring_settings_sql = "SELECT telegram_active, telegram_api_token, telegram_chat_id, telegram_alert_chat_id, telegram_alert_creds_chat_id FROM settings;";
                MySqlCommand monitoring_settings_cmd = new MySqlCommand(monitoring_settings_sql, conn);
                MySqlDataReader monitoring_settings_rdr = monitoring_settings_cmd.ExecuteReader();
                if (monitoring_settings_rdr.HasRows)
                {
                    while (monitoring_settings_rdr.Read())
                    {
                        Telegram_Monitoring = (int)monitoring_settings_rdr[0];
                        Telegram_API_Token = Encryption.StringCipher.Decrypt(monitoring_settings_rdr[1].ToString(), Settings.Encryption_Key);
                        Telegram_Chat_Id = Encryption.StringCipher.Decrypt(monitoring_settings_rdr[2].ToString(), Settings.Encryption_Key);
                        Telegram_Alert_Chat_Id = Encryption.StringCipher.Decrypt(monitoring_settings_rdr[3].ToString(), Settings.Encryption_Key);
                        Telegram_Alert_Creds_Chat_Id = Encryption.StringCipher.Decrypt(monitoring_settings_rdr[4].ToString(), Settings.Encryption_Key);
                    }
                }
                monitoring_settings_rdr.Close();

                //Check if Firefox should be reinstalled
                if (reinstall_firefox == 1)
                {
                    MessageBox.Show("firefox");
                    //Remove old chrome version and reinstall
                    try
                    {
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

                        var uninstall_process = Process.Start(@"C:\Program Files\Mozilla Firefox\uninstall\helper.exe", "/s");
                        uninstall_process.WaitForExit();

                        var install_process = Process.Start(Application.StartupPath + @"\software\firefox_setup.exe", "-ms");
                        install_process.WaitForExit();

                        //Disable reinstall flag
                        try
                        {
                            string sql = "UPDATE `" + mysql_database + "`.`clients` SET `reinstall_firefox`='0' WHERE `client_name`='" + client_name + "' AND hive_name = '" + hive_name + "';";
                            MySqlCommand cmd = new MySqlCommand(sql, conn);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            Logging.log_error(this.Name, "Disable Reinstall Flag", ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Reinstall Firefox: " + ex.Message + Environment.NewLine);
                    }
                }

                //Check if Chrome should be reinstalled
                if (reinstall_chrome == 1)
                {
                    //Remove old chrome version and reinstall
                    try
                    {
                        using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\Wow6432node\Google", true))
                        {
                            if (regkey.OpenSubKey("Update") != null)
                            {
                                regkey.DeleteSubKeyTree("Update");
                            }
                        }

                        Thread.Sleep(5000);

                        Process.Start(@"C:\Auto_Bot\chrome_setup.exe");

                        Thread.Sleep(60000);

                        foreach (var process in Process.GetProcessesByName("GoogleUpdate"))
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

                        string chrome_update_path = @"C:\Program Files (x86)\Google\Update\GoogleUpdate.exe";

                        if (File.Exists(chrome_update_path))
                        {
                            try
                            {
                                File.Copy(chrome_update_path, @"C:\Backup\GoogleUpdate.exe", true);

                                File.Delete(chrome_update_path);
                            }
                            catch
                            {

                            }
                        }

                        //Disable reinstall flag
                        try
                        {
                            string disable_chrome_reinstall_sql = "UPDATE `" + mysql_database + "`.`clients` SET `reinstall_chrome`='0' WHERE `client_name`='" + client_name + "' AND hive_name = '" + hive_name + "';";
                            MySqlCommand disable_chrome_reinstall_cmd = new MySqlCommand(disable_chrome_reinstall_sql, conn);
                            disable_chrome_reinstall_cmd.ExecuteNonQuery();
                        }
                        catch
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Reinstall Chrome: " + ex.Message + Environment.NewLine);
                    }
                }

                if (renew_windows_trial_licence == 1)
                {
                    //Disable Renew Windows Trial Licence Flag
                    try
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `renew_windows_trial_licence`='0' WHERE `client_name`='" + client_name + "' AND hive_name = '" + hive_name + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {

                    }

                    renew_windows_licence();
                }

                conn.Close();

                //Delete Firefox Cookies
                foreach (var directory in Directory.GetDirectories(@"C:\Users\Administrator\AppData\Roaming\Mozilla\Firefox\Profiles"))
                {
                    try
                    {
                        if (File.Exists(directory + @"\cookies.sqlite"))
                            File.Delete(directory + @"\cookies.sqlite");
                    }
                    catch (Exception ex)
                    {
                        Logging.log_error(this.Name, "Delete Firefox Cookies (cookies.sqlite)", ex.Message);
                    }

                    try
                    {
                        if (File.Exists(directory + @"\cookies.sqlite.bak"))
                            File.Delete(directory + @"\cookies.sqlite.bak");
                    }
                    catch (Exception ex)
                    {
                        Logging.log_error(this.Name, "Delete Firefox Cookies (cookies.sqlite.bak)", ex.Message);
                    }

                    try
                    {
                        if (File.Exists(directory + @"\cookies.sqlite-shm"))
                            File.Delete(directory + @"\cookies.sqlite-shm");
                    }
                    catch (Exception ex)
                    {
                        Logging.log_error(this.Name, "Delete Firefox Cookies (sqlite-shm)", ex.Message);
                    }

                    try
                    {
                        if (File.Exists(directory + @"\cookies.sqlite-wal"))
                            File.Delete(directory + @"\cookies.sqlite-wal");
                    }
                    catch (Exception ex)
                    {
                        Logging.log_error(this.Name, "Delete Firefox Cookies (sqlite-wal)", ex.Message);
                    }
                }

                //Thread.Sleep(15000);

                //Check if cookies sync is enabled
                if (cookies_sync_enabled == 1)
                {
                    foreach (var directory in Directory.GetDirectories(@"C:\Users\Administrator\AppData\Roaming\Mozilla\Firefox\Profiles"))
                    {
                        try
                        {
                            File.WriteAllBytes(directory + @"\cookies.sqlite", cookies_file_bytes);
                        }
                        catch (Exception ex)
                        {
                            Logging.log_error(this.Name, "Write Firefox Cookies", ex.Message);
                        }
                    }
                }

                if (auto_restart_bot == 1)
                {
                    restart_bot_auto.Interval = auto_restart_bot_timer;
                    restart_bot_auto.Start();
                }

                if (spotify_enabled == 1)
                {
                    spotify_player_settings_loader();
                    time_sheduler();
                }

                if (deezer_enabled == 1)
                {
                    deezer_player_settings_loader();
                    time_sheduler();
                }
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Read Pre Player Configuration: " + ex.Message + Environment.NewLine);

                Misc.kill_everything_perform_restart();
            }
        }

        private void time_sheduler()
        {
            try
            {
                //Check if the player should be active today
                if (monday == 1 && DateTime.Today.DayOfWeek.ToString() == "Monday")
                {
                    sheduled_monday_label.Text = "Is Monday: yes";
                    bot_active = 1;
                    this.BeginInvoke(new MethodInvoker(player_init));
                }
                else
                {
                    sheduled_monday_label.Text = "Is Monday: no";
                }

                if (tuesday == 1 && DateTime.Today.DayOfWeek.ToString() == "Tuesday")
                {
                    sheduled_tuesday_label.Text = "Is Tuesday: yes";
                    bot_active = 1;
                    this.BeginInvoke(new MethodInvoker(player_init));
                }
                else
                {
                    sheduled_tuesday_label.Text = "Is Tuesday: no";
                }

                if (wednesday == 1 && DateTime.Today.DayOfWeek.ToString() == "Wednesday")
                {
                    sheduled_wednesday_label.Text = "Is Wednesday: yes";
                    bot_active = 1;
                    this.BeginInvoke(new MethodInvoker(player_init));
                }
                else
                {
                    sheduled_wednesday_label.Text = "Is Wednesday: no";
                }

                if (thursday == 1 && DateTime.Today.DayOfWeek.ToString() == "Thursday")
                {
                    sheduled_thursday_label.Text = "Is Thursday: yes";
                    bot_active = 1;
                    this.BeginInvoke(new MethodInvoker(player_init));
                }
                else
                {
                    sheduled_thursday_label.Text = "Is Thursday: no";
                }

                if (friday == 1 && DateTime.Today.DayOfWeek.ToString() == "Friday")
                {
                    sheduled_friday_label.Text = "Is Friday: yes";
                    bot_active = 1;
                    this.BeginInvoke(new MethodInvoker(player_init));
                }
                else
                {
                    sheduled_friday_label.Text = "Is Friday: no";
                }

                if (saturday == 1 && DateTime.Today.DayOfWeek.ToString() == "Saturday")
                {
                    sheduled_saturday_label.Text = "Is Saturday: yes";
                    bot_active = 1;
                    this.BeginInvoke(new MethodInvoker(player_init));
                }
                else
                {
                    sheduled_saturday_label.Text = "Is Saturday: no";
                }

                if (sunday == 1 && DateTime.Today.DayOfWeek.ToString() == "Sunday")
                {
                    sheduled_sunday_label.Text = "Is Sunday: yes";
                    bot_active = 1;
                    this.BeginInvoke(new MethodInvoker(player_init));
                }
                else
                {
                    sheduled_sunday_label.Text = "Is Sunday: no";
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Time Sheduler: " + ex.Message + Environment.NewLine);

                Misc.kill_everything_perform_restart();
            }
        }

        //Init Spotify Settings Loader
        private void spotify_player_settings_loader()
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                //Select all Credentials assigned to the client
                try
                {
                    string sql = "SELECT username, openvpn_profile FROM `" + mysql_database + "`.`spotify_credentials` WHERE client_name = '" + client_name + "' AND outdated = '0' AND hive_name = '" + hive_name + "';";

                    MySqlDataAdapter mysql_data_adapter = new MySqlDataAdapter();
                    mysql_data_adapter.SelectCommand = new MySqlCommand(sql, conn);

                    DataTable mysql_table = new DataTable();
                    mysql_data_adapter.Fill(mysql_table);

                    spotify_credentials_datagridview.DataSource = mysql_table;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Spotify Credentials from Database: " + ex.Message + Environment.NewLine);

                    Misc.kill_everything_perform_restart();
                }

                //Load current client settings for spotify
                try
                {
                    //7
                    string client_settings_sql = "SELECT spotify_playlist_url, spotify_player_account, spotify_player_password, spotify_autoskip_enabled, spotify_autoskip_forward_from, spotify_autoskip_forward_to, rotate_credentials_enabled, chrome_persistent_profiles_enabled FROM `" + mysql_database + "`.`clients` WHERE client_name = '" + client_name + "' AND hive_name = '" + hive_name + "';";
                    MySqlCommand client_settings_cmd = new MySqlCommand(client_settings_sql, conn);
                    MySqlDataReader client_settings_rdr = client_settings_cmd.ExecuteReader();
                    if (client_settings_rdr.HasRows)
                    {
                        while (client_settings_rdr.Read())
                        {
                            spotify_playlist_url = client_settings_rdr[0].ToString();
                            spotify_player_username = client_settings_rdr[1].ToString();
                            chrome_persistent_profile_enabled = (int)client_settings_rdr[7];

                            if (client_settings_rdr[2].ToString() != "")
                                spotify_player_password = Encryption.StringCipher.Decrypt(client_settings_rdr[2].ToString(), Settings.Encryption_Key);

                            spotify_autoskip_enabled = (int)client_settings_rdr[3];
                            spotify_autoskip_forward_from = (int)client_settings_rdr[4];
                            spotify_autoskip_forward_to = (int)client_settings_rdr[5];

                            spotify_rotate_credentials_enabled = (int)client_settings_rdr[6];
                        }
                    }
                    client_settings_rdr.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();

                    File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load Spotify Client Settings from Database: " + ex.Message + Environment.NewLine);

                    Misc.kill_everything_perform_restart();
                }

                if (spotify_rotate_credentials_enabled == 1)
                {
                    //Check if creds are loaded
                    if (spotify_credentials_datagridview.Rows.Count == 0)
                    {
                        int no_vpn_profile_assigned = 0;
                        string spotify_username = "";
                        string spotify_password = "";
                        string new_openvpn_profile = "";
                        int not_assigned_availiable = 1;

                        //Try to receive a fresh credential and prepare for assigning
                        try
                        {
                            string sql1 = "SELECT username, password, client_name, openvpn_profile, cookies_file FROM spotify_credentials WHERE client_name = '-' AND outdated = '0' LIMIT 1;";
                            MySqlCommand cmd1 = new MySqlCommand(sql1, conn);
                            MySqlDataReader rdr = cmd1.ExecuteReader();

                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    if (rdr[3].ToString() == "-") //If VPN Profile is not set
                                    {
                                        spotify_username = rdr[0].ToString();
                                        spotify_password = rdr[1].ToString();

                                        Random random = new Random();
                                        int randomNumber = random.Next(0, openvpn_profile_combobox.Items.Count);

                                        cookies_file_encoded = rdr[4].ToString();

                                        openvpn_profile_combobox.SelectedIndex = randomNumber;
                                        new_openvpn_profile = openvpn_profile_combobox.SelectedItem.ToString();
                                        no_vpn_profile_assigned = 1;
                                    }
                                    else //If VPN Profile is already set
                                    {
                                        spotify_username = rdr[0].ToString();
                                        spotify_password = rdr[1].ToString();
                                    }
                                }
                            }
                            else
                            {
                                not_assigned_availiable = 0;
                            }
                            rdr.Close();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();

                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load fresh Credentials from Database (1): " + ex.Message + Environment.NewLine);

                            Misc.kill_everything_perform_restart();
                        }

                        if (not_assigned_availiable == 1)
                        {
                            //Assign Creds to the Client
                            try
                            {
                                if (no_vpn_profile_assigned == 1) //Set new Openvpn Profile
                                {
                                    string sql1 = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `client_name`='" + client_name + "', `hive_name`='" + hive_name + "', `openvpn_profile`='" + new_openvpn_profile + "', `openvpn_profile`='" + new_openvpn_profile + "' WHERE `username`='" + spotify_username + "';";
                                    MySqlCommand cmd1 = new MySqlCommand(sql1, conn);
                                    cmd1.ExecuteNonQuery();
                                }
                                else if (no_vpn_profile_assigned == 0) //Set new client for credentials, no new vpn profile, because it is already set
                                {
                                    string sql = "UPDATE `" + mysql_database + "`.`spotify_credentials` SET `client_name`='" + client_name + "', `hive_name`='" + hive_name + "' WHERE `username`='" + spotify_username + "';";
                                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                                    cmd.ExecuteNonQuery();
                                }

                                //Update Client Settings with newly loaded Creds
                                string sql2 = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_account`='" + spotify_username + "', `spotify_player_password`='" + spotify_password + "', `cookies_file`='" + cookies_file_encoded + "' WHERE `client_name`='" + client_name + "' AND hive_name = '" + hive_name + "';";
                                MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                                cmd2.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                conn.Close();

                                File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Update Credentials, assign it to the client: " + ex.Message + Environment.NewLine);

                                Misc.kill_everything_perform_restart();
                            }

                            Misc.kill_everything_perform_restart();
                        }
                        else
                        {
                            //Count again if new creds could be loaded. If not, pause and retry in four hours
                            if (spotify_credentials_datagridview.Rows.Count == 0)
                            {
                                if (Telegram_Monitoring == 1)
                                {
                                    Telegram.telegram_critical_alert_send("No credentials availiable to receive. Please add new ones or update the outdated. Retrying in four hours.");
                                }

                                runtime_label.Text = "Paused. No credentials.";
                                runtime_label.Refresh();

                                conn.Close();

                                no_creds_update_last_access(); //Init last access updater ->
                            }
                            else //Continue
                            {
                                if (Telegram_Monitoring == 1)
                                {
                                    Telegram.telegram_critical_alert_send("No assigned credentials are availiable. Loaded a new one that is not assigned to another client.");
                                }
                            }
                        }
                    }

                    //Select current used Credentials and then go to next one
                    try
                    {
                        spotify_credentials_datagridview.ClearSelection();

                        int selected_index = 0;

                        //Select Index where Username is located
                        foreach (DataGridViewRow row in spotify_credentials_datagridview.Rows)
                        {
                            // 0 is the column index
                            if (row.Cells[0].Value.ToString().Equals(spotify_player_username))
                            {
                                row.Selected = true;
                                selected_index = row.Index;
                                break;
                            }
                        }

                        //Set vpn profile of credentials to ensure connecting to the vpn works -> will be loaded from vpn loader
                        if (openvpn_bound_credentials_enabled == 1)
                        {
                            openvpn_profile = spotify_credentials_datagridview.SelectedCells[1].Value.ToString();
                        }

                        //Get selected index
                        selected_index = selected_index + 1;

                        if (selected_index != spotify_credentials_datagridview.RowCount)
                        {
                            spotify_credentials_datagridview.CurrentCell = spotify_credentials_datagridview.Rows[selected_index].Cells[0];
                        }
                        else //If credentials count == one go to zero again
                        {
                            spotify_credentials_datagridview.Rows[0].Selected = true;
                        }
                    }
                    catch
                    {

                    }

                    //Load next selected usernames password
                    try
                    {
                        string sql = "SELECT password, cookies_file FROM `" + mysql_database + "`.`spotify_credentials` WHERE username = '" + spotify_credentials_datagridview.SelectedCells[0].Value.ToString() + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                spotify_next_password = rdr[0].ToString();

                                cookies_file_encoded = rdr[1].ToString();
                            }
                        }
                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Load next selected usernames password: " + ex.Message + Environment.NewLine);

                        Misc.kill_everything_perform_restart();
                    }

                    //Save next selected credentials to the client settings
                    try
                    {
                        string sql = "UPDATE `" + mysql_database + "`.`clients` SET `spotify_player_account`='" + spotify_credentials_datagridview.SelectedCells[0].Value.ToString() + "', `spotify_player_password`='" + spotify_next_password + "', `cookies_file`='" + cookies_file_encoded + "' WHERE `client_name`='" + client_name + "';";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        conn.Close();

                        File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save new Credential to Database: " + ex.Message + Environment.NewLine);

                        Misc.kill_everything_perform_restart();
                    }
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Client Load: " + ex.Message + Environment.NewLine);

                Misc.kill_everything_perform_restart();
            }
        }

        //Init Deezer Settings Loader
        private void deezer_player_settings_loader()
        {
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                //Client Settings SQL Loader
                string client_settings_sql = "SELECT deezer_playlist_url, deezer_player_account, deezer_player_password, deezer_autoskip_enabled, deezer_autoskip_forward_from, deezer_autoskip_forward_to, openvpn_enabled, openvpn_randomize, openvpn_health_check, openvpn_profile FROM `" + mysql_database + "`.`clients` WHERE client_name = '" + client_name + "' AND hive_name = '" + hive_name + "';";
                MySqlCommand client_settings_cmd = new MySqlCommand(client_settings_sql, conn);
                MySqlDataReader client_settings_rdr = client_settings_cmd.ExecuteReader();
                if (client_settings_rdr.HasRows)
                {
                    while (client_settings_rdr.Read())
                    {
                        deezer_playlist_url = client_settings_rdr[0].ToString();
                        deezer_player_username = client_settings_rdr[1].ToString();
                        deezer_player_password = Encryption.StringCipher.Decrypt(client_settings_rdr[2].ToString(), Settings.Encryption_Key);

                        deezer_autoskip_enabled = (int)client_settings_rdr[3];
                        deezer_autoskip_forward_from = (int)client_settings_rdr[4];
                        deezer_autoskip_forward_to = (int)client_settings_rdr[5];
                    }
                }
                client_settings_rdr.Close();

                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();

                File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Client Load: " + ex.Message + Environment.NewLine);

                Misc.kill_everything_perform_restart();
            }
        }

        private void player_init()
        {
            if (openvpn_enabled == 1)
            {
                try
                {
                    if (openvpn_bound_credentials_enabled == 1) //Select Openvpn Profile from selected Credential
                    {
                        try
                        {
                            openvpn_profile_combobox.SelectedIndex = openvpn_profile_combobox.FindStringExact(openvpn_profile);
                        }
                        catch(Exception ex)
                        {
                            Telegram.telegram_critical_alert_send("Cannot select OpenVPN Profile: " + openvpn_profile + " Error: " + ex.Message);
                            Misc.kill_everything_perform_restart();
                        }
                    }
                    else if (openvpn_randomize_enabled == 1)
                    {
                        Random random = new Random();
                        int randomNumber = random.Next(0, openvpn_profile_combobox.Items.Count);
                        openvpn_profile_combobox.SelectedIndex = randomNumber;
                    }
                    else if (openvpn_randomize_enabled == 0)
                    {
                        openvpn_profile_combobox.SelectedIndex = openvpn_profile_combobox.FindStringExact(openvpn_profile);
                    }

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

                    if (spotify_enabled == 1)
                    {
                        /*try
                        {
                            //Delete OpenVPN Login Profile
                            File.Delete(@"C:\Program Files\OpenVPN\config\login.ovpn");

                            //Write OpenVPN Login Profile
                            File.WriteAllBytes(@"C:\Program Files\OpenVPN\config\login.ovpn", openvpn_login_profile_bytes);

                            ProcessStartInfo proc = new ProcessStartInfo();
                            proc.FileName = @"C:\Program Files\OpenVPN\bin\openvpn-gui.exe";
                            proc.Arguments = "--connect login.ovpn";
                            Process.Start(proc);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Connect with OpenVPN (" + openvpn_profile_combobox.SelectedItem.ToString() + "): " + ex.Message + Environment.NewLine);
                            Misc.kill_everything_perform_restart();
                        }*/
                    }
                    else
                    {
                        try
                        {
                            ProcessStartInfo proc = new ProcessStartInfo();
                            proc.FileName = @"C:\Program Files\OpenVPN\bin\openvpn-gui.exe";
                            proc.Arguments = "--connect " + openvpn_profile_combobox.SelectedItem.ToString();
                            Process.Start(proc);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Connect with OpenVPN (" + openvpn_profile_combobox.SelectedItem.ToString() + "): " + ex.Message + Environment.NewLine);
                            Misc.kill_everything_perform_restart();
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- OpenVPN Methode: " + ex.Message + Environment.NewLine);
                    Misc.kill_everything_perform_restart();
                }
            }

            Thread.Sleep(20000);

            if (spotify_enabled == 1)
            {
                var spotify_form = new Player.Spotify_Form();
                spotify_form.Closed += (s, args) => this.Close();
                spotify_form.Show();
            }

            if (deezer_enabled == 1)
            {
                var deezer_form = new Player.Deezer_Form();
                deezer_form.Closed += (s, args) => this.Close();
                deezer_form.Show();
            }
        }

        private void monitoring_methode()
        {
            MethodInvoker mk = delegate
            {
                //Update Last Access
                try
                {
                    WebClient request = new WebClient();
                    request.Headers.Add("User-Agent", Settings.Useragent);
                    request.DownloadString(Settings.Helper + "?username=" + username + "&password=" + password + "&mysql_database=" + mysql_database + "&client_name=" + client_name + "&hive_name=" + hive_name + "&update_last_access=1");
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Monitoring(Update Last Access): " + ex.Message + Environment.NewLine);
                    Misc.kill_everything_perform_restart();
                }

                try
                {
                    var Spotify_Form_Init = Application.OpenForms.OfType<Spotify_Form>().FirstOrDefault();
                    var Deezer_Form_Init = Application.OpenForms.OfType<Deezer_Form>().FirstOrDefault();

                    //Check for Click Health Spotify
                    try
                    {
                        if (spotify_enabled == 1 && bot_active == 1 && outdated_credentials_flag == 0)
                        {
                            if (Spotify_Form_Init.Clicks == 0)
                            {
                                if (Telegram_Monitoring == 1)
                                {
                                    Telegram.telegram_alert_send("Player: Spotify\nAfter ten minutes no clicks were registered. The Player will now be restarted.");
                                }

                                Misc.kill_everything_perform_restart();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Monitoring(Check Spotify Health Status): " + ex.Message + Environment.NewLine);
                        Misc.kill_everything_perform_restart();
                    }

                    //Check for Click Health deezer
                    try
                    {
                        if (deezer_enabled == 1 && bot_active == 1 && outdated_credentials_flag == 0)
                        {
                            if (Deezer_Form_Init.Clicks == 0)
                            {
                                if (Telegram_Monitoring == 1)
                                {
                                    Telegram.telegram_alert_send("Player: Deezer\nAfter ten minutes no clicks were registered. The Player will now be restarted.");
                                }

                                Misc.kill_everything_perform_restart();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Monitoring(Check Deezer Health Status): " + ex.Message + Environment.NewLine);
                        Misc.kill_everything_perform_restart();
                    }

                    //Check if telegram monitoring is enabled
                    if (Telegram_Monitoring == 1)
                    {
                        string bot_status = "-";

                        string spotify_status = "-";
                        string spotify_clicks = "-";

                        string deezer_status = "-";
                        string deezer_clicks = "-";

                        try
                        {
                            if (bot_active == 1)
                            {
                                bot_status = "Active.";

                                if (spotify_enabled == 1 && outdated_credentials_flag == 0)
                                {
                                    spotify_status = "Active.";
                                    int spotify_clicks_int = Spotify_Form_Init.Clicks;
                                    spotify_clicks = spotify_clicks_int.ToString();
                                }
                                else
                                {
                                    spotify_status = "Paused.";
                                }

                                if (deezer_enabled == 1 && outdated_credentials_flag == 0)
                                {
                                    deezer_status = "Active.";
                                    int deezer_clicks_int = Deezer_Form_Init.Clicks;
                                    deezer_clicks = deezer_clicks_int.ToString();
                                }
                                else
                                {
                                    deezer_status = "Paused.";
                                }
                            }
                            else
                            {
                                bot_status = "Paused.";
                            }
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + "- Monitoring(Check and collect Status Information): " + ex.Message + Environment.NewLine);
                        }

                        //Send Report via Telegram
                        try
                        {
                            Telegram.telegram_mon_send("Bot Status: " + bot_status + "\n" + "Spotify Status: " + spotify_status + "\n" + "Deezer Status: " + deezer_status + "\n" + "Runtime: " + bot_runtime + "\n" + "Spotify Clicks: " + spotify_clicks + "\n" + "Deezer Clicks: " + deezer_clicks);
                        }
                        catch (Exception ex)
                        {

                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + " - Monitoring(Send Report via bot_status): " + bot_status + Environment.NewLine);
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + " - Monitoring(Send Report via spotify_status): " + spotify_status + Environment.NewLine);
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + " - Monitoring(Send Report via deezer_status): " + deezer_status + Environment.NewLine);
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + " - Monitoring(Send Report via bot_runtime): " + bot_runtime + Environment.NewLine);
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + " - Monitoring(Send Report via spotify_clicks): " + spotify_clicks + Environment.NewLine);
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + " - Monitoring(Send Report via deezer_clicks): " + deezer_clicks + Environment.NewLine);
                            File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + " - Monitoring(Send Report via Telegram): " + ex.Message + Environment.NewLine);
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Application.StartupPath + "\\logs\\Error.log", DateTime.Now + " - Monitoring(Alles am Arsch): " + ex.Message + Environment.NewLine);
                    Misc.kill_everything_perform_restart();
                }
            };
            mk.BeginInvoke(callbackfunction, null);
        }

        private void open_menu_button_Click(object sender, EventArgs e)
        {
            if (menu_panel.Visible == false)
            {
                menu_panel.Visible = true;
            }
            else if (menu_panel.Visible == true)
            {
                menu_panel.Visible = false;
            }
        }

        private void restart_bot_button_Click(object sender, EventArgs e)
        {
            Misc.kill_everything_perform_restart();
        }

        private void task_manager_button_Click(object sender, EventArgs e)
        {
            Process.Start("taskmgr");
        }

        private void reset_bot_settings_button_Click(object sender, EventArgs e)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true))
            {
                key.DeleteValue("First_Start");
            }

            Misc.kill_everything_perform_restart();
        }

        private void cmd_button_Click(object sender, EventArgs e)
        {
            Process.Start("cmd");
        }

        private void file_explorer_button_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\totalcmd\TOTALCMD64.EXE");
        }

        private void restart_bot_auto_Tick(object sender, EventArgs e)
        {
            Autobot_Helper.outdated_credentials();
            Misc.kill_everything_perform_restart();
        }

        private void monitoring_timer_Tick(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(monitoring_methode));
        }

        private void open_chrome_button_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe");
        }

        private void update_client_button_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Auto_Bot_Updater.exe");
        }

        private void send_telegram_message_button_Click(object sender, EventArgs e)
        {
            Telegram.telegram_critical_alert_send(send_telegram_message_textbox.Text);
        }

        private void open_logfile_button_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Auto_Bot\logs\error.log");
        }
    }
}
