using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Security.AccessControl;

namespace Auto_Bot___Client
{
    public partial class Login_Form : Form
    {
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

        public Login_Form()
        {
            InitializeComponent();
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
                RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_host_regkey.SetValue("MySQL_Host", mysql_host_decoded_str, RegistryValueKind.String);
                    mysql_host_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Save MySQL Host Information to Registry", ex.Message);

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
                RegistryKey mysql_database_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_database_regkey.SetValue("MySQL_Database", mysql_database_decoded_str, RegistryValueKind.String);
                    mysql_database_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Save MySQL Database Information to Registry", ex.Message);

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
                RegistryKey mysql_username_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_username_regkey.SetValue("MySQL_Username", mysql_username_request_decoded_str, RegistryValueKind.String);
                    mysql_username_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Save MySQL Username Information to Registry", ex.Message);

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
                RegistryKey mysql_password_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_password_regkey.SetValue("MySQL_Password", mysql_password_request_decoded_str, RegistryValueKind.String);
                    mysql_password_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Save MySQL Password Information to Registry", ex.Message);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }

            //Get Mysql Port Information
            database_request.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection
            mysql_port_request = database_request.DownloadString(Settings.Database_Provider + "?username=" + username_textbox.Text + "&password=" + password_textbox.Text + "&mpp=1");

            byte[] mysql_port_request_decoded = Convert.FromBase64String(mysql_port_request);
            string mysql_port_request_decoded_str = Encoding.UTF8.GetString(mysql_port_request_decoded);

            try
            {
                RegistryKey mysql_port_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                {
                    mysql_port_regkey.SetValue("MySQL_Port", mysql_port_request_decoded_str, RegistryValueKind.String);
                    mysql_port_regkey.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Save MySQL Port Information to Registry", ex.Message);

                Process.Start(Application.StartupPath + @"\logs\error.log");
            }
        }

        private void Login_Form_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Auto_Login == true)
            {
                //Read Username Regkey
                try
                {
                    RegistryKey Username_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                    {
                        username_textbox.Text = Encryption.StringCipher.Decrypt(Username_Regkey.GetValue("Username").ToString(), Settings.Encryption_Key);
                        Username_Regkey.Close();
                    }
                }
                catch (Exception ex)
                {
                    Logging.log_error(this.Name, "Read Username Information from Registry", ex.Message);
                }

                //Read Password Regkey
                try
                {
                    RegistryKey Password_Regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client");
                    {
                        password_textbox.Text = Encryption.StringCipher.Decrypt(Password_Regkey.GetValue("Password").ToString(), Settings.Encryption_Key);
                        Password_Regkey.Close();
                    }
                }
                catch (Exception ex)
                {
                    Logging.log_error(this.Name, "Read Password Information from Registry", ex.Message);
                }

                login_button.PerformClick();
            }
        }

        private void form_loader()
        {
            RegistryKey winLogonKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
            {
                if (winLogonKey.GetValueNames().Contains("First_Start"))
                {
                    var show_main_form = new Auto_Bot___Client.Main_Form();
                    show_main_form.Closed += (s, args) => this.Close();
                    show_main_form.Show();
                    this.Hide();
                }
                else
                {
                    var show_setup_assistant_database = new Setup.Setup_Assistant();
                    show_setup_assistant_database.Closed += (s, args) => this.Close();
                    show_setup_assistant_database.Show();
                    this.Hide();
                }
            }
        }

        private void login_button_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient login_request = new WebClient(); // creats a new webclient called wb
                string loginstring; // declares a string called loginstring so we can assign a value later
                login_request.Headers.Add("User-Agent", Settings.Useragent); // adds Useragent for leak protection

                loginstring = login_request.DownloadString(Settings.Auth + "?username=" + username_textbox.Text + "&password=" + password_textbox.Text); // makes a webrequest using wb for authentication, other parameters are in Settings.cs
                //MessageBox.Show(loginstring);
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
                            RegistryKey mysql_host_regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Auto Bot Client", true);
                            {
                                mysql_host_regkey.SetValue("Username", Encryption.StringCipher.Encrypt(username_textbox.Text, Settings.Encryption_Key), RegistryValueKind.String);
                                mysql_host_regkey.SetValue("Password", Encryption.StringCipher.Encrypt(password_textbox.Text, Settings.Encryption_Key), RegistryValueKind.String);
                                mysql_host_regkey.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(Application.StartupPath + @"\logs\error.log", DateTime.Now + " - Save Licence Information to Registry: " + ex.Message + Environment.NewLine);

                            Process.Start(Application.StartupPath + @"\logs\error.log");
                        }

                        get_rental_hostet();

                        Properties.Settings.Default.Auto_Login = true;
                        Properties.Settings.Default.Save();

                        //Check for first run
                        try
                        {
                            this.BeginInvoke(new MethodInvoker(form_loader));
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
                    MessageBox.Show("Wrong username or password.", "Login failed.", MessageBoxButtons.OK, MessageBoxIcon.Error); //messagebox that says password is wrong
                }
            }
            catch (Exception ex)
            {
                Logging.log_error(this.Name, "Login Unknown Error", ex.Message);

                Misc.kill_everything_perform_restart();
            }
        }
    }
}
