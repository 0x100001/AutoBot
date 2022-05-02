using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Auto_Bot___Client
{
    internal class Misc
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        [DllImport("User32")]
        static extern long SetForegroundWindow(int hwnd);

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow); //For hiding process

        public static void kill_everything_perform_restart()
        {
            var Splash_Form_Init = Application.OpenForms.OfType<Splash_Screen>().FirstOrDefault();

            if (Splash_Form_Init.error == 0)
            {
                if (Process.GetProcessesByName("Auto_Bot_Starter").Length != 0)
                {
                    Process.Start(@"C:\Auto_Bot_Starter.exe");
                }
                else
                {
                    foreach (var process in Process.GetProcessesByName("Auto_Bot_Starter"))
                    {
                        process.Kill();
                        process.WaitForExit();
                    }

                    Process.Start(@"C:\Auto_Bot_Starter.exe");
                }

                Application.Exit();

                Splash_Form_Init.error = 1;
            }
        }

        //Set Application
        public static void process_setforeground(string process)
        {
            try
            {
                Process[] local = Process.GetProcessesByName(process);
                if (local.Length > 0)
                {
                    Process p = local[0];
                    IntPtr h = p.MainWindowHandle;
                    SetForegroundWindow(h.ToInt32());
                }
            }
            catch(Exception ex)
            {
                Logging.log_error("Misc", "Set Process Foreground", ex.Message);
            }
        }

        //Shutdown PC
        public static void shutdown_pc()
        {
            var proc = new ProcessStartInfo("shutdown", "/s /t 0");
            proc.CreateNoWindow = true;
            proc.UseShellExecute = false;
            Process.Start(proc);
        }

        //Restart PC
        public static void restart_pc()
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.FileName = "cmd";
            proc.Arguments = "/C shutdown -f -r -t 0";
            Process.Start(proc);
        }

        //Kill Specific Process
        public static void kill_existing_process(string proc)
        {
            if (Process.GetProcessesByName(proc).Length != 0)
            {
                foreach (var process in Process.GetProcessesByName(proc))
                {
                    try
                    {
                        process.Kill();

                        Thread.Sleep(1000);
                    }
                    catch
                    {

                    }
                }
            }
        }

        //Hide Window
        public static void hide_process(string process)
        {
            int hWnd;
            Process[] processRunning = Process.GetProcesses();
            foreach (Process pr in processRunning)
            {
                if (pr.ProcessName == process)
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
            }
        }
    }
}
