using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;

namespace Auto_Bot___Client
{
    internal class Autobot_Helper
    {
        //Mark Credentials as outdated
        public static void outdated_credentials()
        {
            var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();
            WebClient request = new WebClient();
            request.Headers.Add("User-Agent", Settings.Useragent);
            request.DownloadString(Settings.Helper + "?username=" + Main_Form_Init.username + "&password=" + Main_Form_Init.password + "&mysql_database=" + Main_Form_Init.mysql_database + "&client_name=" + Main_Form_Init.client_name + "&hive_name=" + Main_Form_Init.hive_name + "&out_creds=" + Main_Form_Init.spotify_player_username);
        }
    }
}
