using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Management;
using System.Diagnostics;
namespace Auto_Bot_Starter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\nInitializing.");

            try
            {
                if (Process.GetProcessesByName("Auto_Bot_Client").Length != 0)
                {
                    Console.WriteLine("\nAubotbot Client wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("Auto_Bot_Client"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nAuto_Bot_Client: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("GoogleUpdate").Length != 0)
                {
                    Console.WriteLine("\nGoogle Update wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("GoogleUpdate"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nGoogle Update: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("GoogleCrashHandler").Length != 0)
                {
                    Console.WriteLine("\nGoogleCrashHandler wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("GoogleCrashHandler"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nGoogleCrashHandler: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("GoogleCrashHandler64").Length != 0)
                {
                    Console.WriteLine("\nGoogleCrashHandler64 wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("GoogleCrashHandler64"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nGoogleCrashHandler64: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("chrome").Length != 0)
                {
                    Console.WriteLine("\nChrome wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("chrome"))
                    {
                        process.Kill();

                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nChrome: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("firefox").Length != 0)
                {
                    Console.WriteLine("\nFirefox wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("firefox"))
                    {
                        process.Kill();

                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nFirefox: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("chromedriver").Length != 0)
                {
                    Console.WriteLine("\nChromedriver wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("chromedriver"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nChromedriver: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("openvpn").Length != 0)
                {
                    Console.WriteLine("\nOpenvpn wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("openvpn"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nOpenvpn: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("openvpn-gui").Length != 0)
                {
                    Console.WriteLine("\nOpenvpn-Gui wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("openvpn-gui"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nOpenvpn-Gui: " + ex);
            }

            try
            {
                if (Process.GetProcessesByName("openvpnserv").Length != 0)
                {
                    Console.WriteLine("\nOpenvpnserv wurde beendet.");

                    foreach (var process in Process.GetProcessesByName("openvpnserv"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nOpenvpnserver: " + ex);
            }

            try
            {
                Process.Start(@"C:\Auto_Bot\Skripte\pagefile.bat");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nPagefile: " + ex);
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
                Console.WriteLine(ex);
            }

            try
            {
                Process.Start(@"C:\Auto_Bot\Auto_Bot_Client.exe");
            }
            catch(Exception ex)
            {
                Console.WriteLine("\nAuto Bot start process failed: " + ex);
            }
        }
    }
}
