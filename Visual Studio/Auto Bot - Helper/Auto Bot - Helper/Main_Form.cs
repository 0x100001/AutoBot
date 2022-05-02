using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Net.Mail;
using System.Net;
using System.Runtime.InteropServices;

namespace Auto_Bot___Helper
{
    public partial class Main_Form : Form
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        public Main_Form()
        {
            InitializeComponent();

            if (Directory.Exists(@"C:\Auto_Bot_Helper\Screenshots") == false)
                Directory.CreateDirectory(@"C:\Auto_Bot_Helper\Screenshots");
        }

        private void screenshot_take_ss_button_Click(object sender, EventArgs e)
        {
            this.Opacity = 0;

            try
            {
                Rectangle rect1 = new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Bitmap bmp = new Bitmap(rect1.Width, rect1.Height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect1.Left, rect1.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                bmp.Save(@"C:\Auto_Bot_Helper\Screenshots\" + screenshot_ss_name_textbox.Text + ".png", ImageFormat.Png);
                bmp.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Opacity = 100;

            screenshot_ss_name_textbox.Clear();
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {

        }

        private void screenshot_send_via_mail_button_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Auto_Bot_Helper\ss.zip"))
                File.Delete(@"C:\Auto_Bot_Helper\ss.zip");

            string startPath = @"C:\Auto_Bot_Helper\Screenshots";
            string zipPath = @"C:\Auto_Bot_Helper\ss.zip";

            ZipFile.CreateFromDirectory(startPath, zipPath);

            using (SmtpClient smtpclient = new SmtpClient())
            {
                var basicCredential = new NetworkCredential("", "");
                using (MailMessage message = new MailMessage())
                {
                    MailAddress smtpaccount = new MailAddress("");

                    smtpclient.EnableSsl = true;
                    smtpclient.Host = "";
                    smtpclient.UseDefaultCredentials = false;
                    smtpclient.Credentials = basicCredential;
                    message.From = smtpaccount;
                    message.Subject = "Auto Bot Helper - Screenshots";
                    message.IsBodyHtml = true;
                    message.Body = "Attached.";
                    message.To.Add("");
                    message.Attachments.Add(new Attachment(@"C:\Auto_Bot_Helper\ss.zip"));

                    try
                    {
                        smtpclient.Send(message);
                        MessageBox.Show("Successfully send.", "Information.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Information.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void screenshot_delete_all_button_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\Auto_Bot_Helper\Screenshots");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            if (File.Exists(@"C:\Auto_Bot_Helper\ss.zip"))
                File.Delete(@"C:\Auto_Bot_Helper\ss.zip");
        }

        private void show_cursor_xyz_form_button_Click(object sender, EventArgs e)
        {
            var show_form = new Cursor_Position.Cursor_XYZ();
            show_form.Closed += (s, args) => this.Close();
            show_form.Show();
        }

        private void start_cursor_recorder_timer_Click(object sender, EventArgs e)
        {
            cursor_recorder_timer.Start();
        }

        private void stop_cursor_recorder_timer_Click(object sender, EventArgs e)
        {
            cursor_recorder_timer.Stop();
        }

        private void cursor_recorder_timer_Tick(object sender, EventArgs e)
        {
            File.AppendAllText(Application.StartupPath + @"\logs\recorder.log", Cursor.Position + Environment.NewLine);
        }
    }
}
