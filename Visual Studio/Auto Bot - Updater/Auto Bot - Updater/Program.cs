using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.IO.Compression;
using Microsoft.Win32;
using System.Management;

namespace Auto_Bot_Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Directory.Exists(@"C:\logs") == false)
                Directory.CreateDirectory(@"C:\logs");

            Thread.Sleep(5000);

            try
            {
                if (Process.GetProcessesByName("Auto_Bot_Starter").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: Auto Bot Starter terminated.");

                    foreach (var process in Process.GetProcessesByName("Auto_Bot_Starter"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("Auto_Bot_Client").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: Auto Bot Client terminated.");

                    foreach (var process in Process.GetProcessesByName("Auto_Bot_Client"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("GoogleUpdate").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: Google Update terminated.");

                    foreach (var process in Process.GetProcessesByName("GoogleUpdate"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("GoogleCrashHandler").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: GoogleCrashHandler terminated.");

                    foreach (var process in Process.GetProcessesByName("GoogleCrashHandler"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("GoogleCrashHandler64").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: GoogleCrashHandler64 terminated.");

                    foreach (var process in Process.GetProcessesByName("GoogleCrashHandler64"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("chromedriver").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: Chromedriver terminated.");

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("chrome").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: Chrome terminated.");

                    foreach (var process in Process.GetProcessesByName("chrome"))
                    {
                        process.Kill();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("openvpn").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: OpenVPN terminated.");

                    foreach (var process in Process.GetProcessesByName("openvpn"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("openvpn-gui").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: Openvpn-Gui terminated.");

                    foreach (var process in Process.GetProcessesByName("openvpn-gui"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            try
            {
                if (Process.GetProcessesByName("openvpnserv").Length != 0)
                {
                    Console.WriteLine("Process Cleanup: OpenVPN Server terminated.");

                    foreach (var process in Process.GetProcessesByName("openvpnserv"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch
            {

            }

            Thread.Sleep(5000);

            try
            {
                if (Directory.Exists(@"C:\Auto_Bot") == true)
                {
                    Directory.Delete(@"C:\Auto_Bot", true);
                    File.Delete(@"C:\update_files.zip");
                }
                else
                {
                    File.Delete(@"C:\update_files.zip");
                }

                Console.WriteLine("Old version removed.");
            }
            catch (Exception ex)
            {
                Logging.Logging.log_error("Auto Bot Updater","Remove old version.", ex.Message);
            }

            try
            {
                SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
                ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
                foreach (ManagementObject item in searchProcedure.Get())
                {
                    item.InvokeMethod("Disable", null);
                    Console.WriteLine("\n" + item + " deactivated.");
                }

                Thread.Sleep(5000);

                SelectQuery wmiQuery_Enable_Network_Adapter = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
                ManagementObjectSearcher search_network_adapter_enable = new ManagementObjectSearcher(wmiQuery_Enable_Network_Adapter);
                foreach (ManagementObject item_enable_adapter in search_network_adapter_enable.Get())
                {
                    item_enable_adapter.InvokeMethod("Enable", null);
                    Console.WriteLine("\n" + item_enable_adapter + " activated.");

                }

                Console.WriteLine("\nSuccessfully cleaned.");
                Console.WriteLine("\nStarting Auto Bot Client now.");

                //Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Logging.Logging.log_error("Auto Bot Updater", "Restart network adapters.", ex.Message);
            }

            Console.WriteLine("Downloading Client.");

            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("User-Agent", "1337"); // adds useragent for leak protection
                webClient.DownloadFile("https:///autobot/updates/update.zip", @"C:\update_files.zip");

                Console.WriteLine("Downloaded successfully.");

                string zipPath = @"C:\update_files.zip";
                string extractPath = @"C:\Auto_Bot";

                ZipFile.ExtractToDirectory(zipPath, extractPath);

                Console.WriteLine("Finished.");

                //Restart System
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.WindowStyle = ProcessWindowStyle.Hidden;
                proc.FileName = "cmd";
                proc.Arguments = "/C shutdown -f -r -t 0";
                Process.Start(proc);

                Environment.Exit(0);
            }
            catch(Exception ex)
            {
                Logging.Logging.log_error("Auto Bot Updater", "Download and Update Client.", ex.Message);

                Process.Start(@"C:\Auto_Bot_Updater.exe");

                Environment.Exit(0);
            }
        }
    }
}
