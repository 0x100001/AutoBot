using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;

namespace Auto_Bot___Client
{
    internal class Telegram
    {
        public static void telegram_mon_send(string content)
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + Main_Form_Init.Telegram_API_Token + "/sendMessage?chat_id=" + Main_Form_Init.Telegram_Chat_Id + "&text=" + "Hive Name: " + Main_Form_Init.hive_name + "\nClient Name: " + Main_Form_Init.client_name + "\n\n" + content);
            req.UseDefaultCredentials = true;
            var result = req.GetResponse();
            req.Abort();
        }

        public static void telegram_alert_send(string content)
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + Main_Form_Init.Telegram_API_Token + "/sendMessage?chat_id=" + Main_Form_Init.Telegram_Alert_Chat_Id + "&text=" + "Hive Name: " + Main_Form_Init.hive_name + "\nClient Name: " + Main_Form_Init.client_name + "\n\n" + content);
            req.UseDefaultCredentials = true;
            var result = req.GetResponse();
            req.Abort();
        }

        public static void telegram_critical_alert_send(string content)
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + Main_Form_Init.Telegram_API_Token + "/sendMessage?chat_id=" + Main_Form_Init.Telegram_Alert_Creds_Chat_Id + "&text=" + "Hive Name: " + Main_Form_Init.hive_name + "\nClient Name: " + Main_Form_Init.client_name + "\n\n" + content);
            req.UseDefaultCredentials = true;
            var result = req.GetResponse();
            req.Abort();
        }
    }
}
