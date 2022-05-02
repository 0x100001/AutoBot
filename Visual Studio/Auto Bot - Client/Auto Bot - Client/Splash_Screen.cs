using System;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.Win32;

namespace Auto_Bot___Client
{
    public partial class Splash_Screen : Form
    {
        //Other
        public int error = 0;

        public Splash_Screen()
        {
            InitializeComponent();
        }

        private void Splash_Screen_Load(object sender, EventArgs e)
        {
            /*//Save Hive Name
            try
            {
                RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_host_regkey.SetValue("Hive_Name", "Hive1", RegistryValueKind.String);
                    mysql_host_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Save Hive Name", ex.Message);
            }*/
            
            //Kill Chrome
            Misc.kill_existing_process("chrome");

            //Kill Chromedriver
            Misc.kill_existing_process("chromedriver");

            //Delete Astaroth Screenshots
            try
            {
                foreach (var file in Directory.GetFiles(@"C:\Auto_Bot\Temp"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {

                    }
                }
            }
            catch(Exception ex)
            {
                Logging.log_error(this.Name, "Delete Astaroth Screenshots", ex.Message);
            }

            try
            {
                if (Process.GetProcessesByName("firefox").Length != 0)
                {
                    foreach (var process in Process.GetProcessesByName("firefox"))
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            //Delete Chrome Temp Files
            try
            {
                DirectoryInfo chrome_directorys = new DirectoryInfo(@"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp");
                foreach (DirectoryInfo chrome_folders in chrome_directorys.GetDirectories("chrome*"))
                {
                    try
                    {
                        chrome_folders.Delete(true);
                    }
                    catch (IOException)
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Delete Astaroth Screenshots", ex.Message);
            }


            //Delete Appdata Scope Temp Files
            try
            {
                DirectoryInfo scope_directorys = new DirectoryInfo(@"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp");
                foreach (DirectoryInfo scope_files in scope_directorys.GetDirectories("scope*"))
                {
                    try
                    {
                        scope_files.Delete(true);
                    }
                    catch (IOException)
                    {

                    }
                }
            }
            catch(Exception ex)
            {
                Logging.log_error(this.Name, "Delete Appdata Scope Temp Files", ex.Message);
            }

            //Check if Backup folder exists and create if not
            try
            {
                try
                {
                    if (Directory.Exists(@"C:\Backup") == false)
                    {
                        Directory.CreateDirectory(@"C:\Backup");
                    }
                }
                catch
                {

                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Check/Create Backup Folder", ex.Message);
            }

            //Copy Starter and Updater to C:\
            try
            {
                try
                {
                    //Copy to C:
                    File.Copy(@"C:\Auto_Bot\Auto_Bot_Starter.exe", @"C:\Auto_Bot_Starter.exe", true);
                    File.Copy(@"C:\Auto_Bot\Auto_Bot_Updater.exe", @"C:\Auto_Bot_Updater.exe", true);
                    File.Copy(@"C:\Auto_Bot\Auto_Bot_exe", @"C:\Auto_Bot_exe", true);

                    //Copy to C:\Backup
                    File.Copy(@"C:\Auto_Bot\Auto_Bot_Starter.exe", @"C:\Backup\Auto_Bot_Starter.exe", true);
                    File.Copy(@"C:\Auto_Bot\Auto_Bot_Updater.exe", @"C:\Backup\Auto_Bot_Updater.exe", true);
                    File.Copy(@"C:\Auto_Bot\Auto_Bot_exe", @"C:\Backup\Auto_Bot_exe", true);
                }
                catch (IOException)
                {

                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Copy Starter and Updater to C:\\", ex.Message);
            }

            //Check Version and Update if needed.
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", Settings.Useragent);
                string versioncheck = webClient.DownloadString(Settings.Check_Version);

                if (versioncheck != Settings.Version)
                {
                    Process.Start(@"C:\Auto_Bot_Updater.exe");
                    Application.Exit();
                }

                loading_timer.Start();
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Check Client Version", ex.Message);
                Misc.kill_everything_perform_restart();
            }
        }

        private void loading_timer_Tick(object sender, EventArgs e)
        {
            loading_timer.Stop();

            try
            {
                var form = new Login_Form();
                form.Closed += (s, args) => this.Close();
                form.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Show Login Form", ex.Message);
                Misc.kill_everything_perform_restart();
            }
        }
    }
}
