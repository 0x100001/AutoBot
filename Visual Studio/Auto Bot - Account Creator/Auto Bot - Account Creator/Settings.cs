using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Bot___Account_Creator

{
    internal class Settings
    //Open brackets
    {
        public static string Useragent = "1337"; 

        //Version of current loader, increment this number here and in the version.txt file each time you wish to push out a update
        public static string Version = "1";

        //The link to where you store your version.txt on your site
        public static string Check_Version = "https://auth./autobot/server/version";

        public static string Update_Url = "https://auth./autobot/updates/server";

        //The link to where you store your version.txt on your site
        //public static string Changelog = "https://auth./loader/management_console/management_console_spotify_changelog";

        //The link to check.php so you can authenticate
        public static string Auth = "https://auth./autobot/server/auth.php";

        public static string Ban = "https://auth./autobot/ban.php";

        public static string Database_Provider = "https://auth./autobot/database_provider.php";

        public static string Driver_Provider = "https://auth./autobot/driver_provider.php";

        public static string Website = "https:///";

        public static string Encryption_Key = "eKz2hfsznvQsji08syRK2Su9Y00WcDEKPiGew3vcYlnzTaUNPLZPYsvcxnVCPgTT";
    }
}
