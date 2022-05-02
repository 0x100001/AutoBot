using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using Auto_Bot___Account_Creator__Astaroth_;

namespace Telegram
{
    internal class Telegram
    {
        public static void telegram_alert_send(string content)
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

            WebRequest req = WebRequest.Create("https://api.telegram.org/bot" + Main_Form_Init.telegram_api_token_textbox.Text + "/sendMessage?chat_id=" + Main_Form_Init.telegram_alert_chat_id_textbox.Text + "&text=" + "Auto Bot - Account Creator (Astaroth)\n\n" + content);
            req.UseDefaultCredentials = true;
            var result = req.GetResponse();
            req.Abort();
        }
    }
}
