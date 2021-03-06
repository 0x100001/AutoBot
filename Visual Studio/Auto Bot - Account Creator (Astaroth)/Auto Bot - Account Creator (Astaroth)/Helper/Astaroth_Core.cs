using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;
using Auto_Bot___Account_Creator__Astaroth_;

namespace Astaroth_Core
{
    class Astaroth_Core
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public static Rectangle FindImageOnScreen(Bitmap bmpMatch, bool ExactMatch)
        {
            Rectangle rct = Rectangle.Empty;

            try
            {
                var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

                Bitmap ScreenBmp = new Bitmap(@"C:\autobot\ss\" + Main_Form_Init.ss + ".png");

                BitmapData ImgBmd = bmpMatch.LockBits(new Rectangle(0, 0, bmpMatch.Width, bmpMatch.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                BitmapData ScreenBmd = ScreenBmp.LockBits(new Rectangle(0, 0, ScreenBmp.Width, ScreenBmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                byte[] ImgByts = new byte[(Math.Abs(ImgBmd.Stride) * bmpMatch.Height) - 1 + 1];
                byte[] ScreenByts = new byte[(Math.Abs(ScreenBmd.Stride) * ScreenBmp.Height) - 1 + 1];

                Marshal.Copy(ImgBmd.Scan0, ImgByts, 0, ImgByts.Length);
                Marshal.Copy(ScreenBmd.Scan0, ScreenByts, 0, ScreenByts.Length);

                bool FoundMatch = false;

                int sindx, iindx;
                int spc, ipc;

                int skpx = Convert.ToInt32((bmpMatch.Width - 1) / (double)10);
                if (skpx < 1 | ExactMatch)
                    skpx = 1;
                int skpy = Convert.ToInt32((bmpMatch.Height - 1) / (double)10);
                if (skpy < 1 | ExactMatch)
                    skpy = 1;

                for (int si = 0; si <= ScreenByts.Length - 1; si += 3)
                {
                    FoundMatch = true;
                    for (int iy = 0; iy <= ImgBmd.Height - 1; iy += skpy)
                    {
                        for (int ix = 0; ix <= ImgBmd.Width - 1; ix += skpx)
                        {
                            sindx = (iy * ScreenBmd.Stride) + (ix * 3) + si;
                            iindx = (iy * ImgBmd.Stride) + (ix * 3);
                            spc = Color.FromArgb(ScreenByts[sindx + 2], ScreenByts[sindx + 1], ScreenByts[sindx]).ToArgb();
                            ipc = Color.FromArgb(ImgByts[iindx + 2], ImgByts[iindx + 1], ImgByts[iindx]).ToArgb();
                            if (spc != ipc)
                            {
                                FoundMatch = false;
                                iy = ImgBmd.Height - 1;
                                ix = ImgBmd.Width - 1;
                            }
                        }
                    }
                    if (FoundMatch)
                    {
                        double r = si / (double)(ScreenBmp.Width * 3);
                        double c = ScreenBmp.Width * (r % 1);
                        if (r % 1 >= 0.5)
                            r -= 1;
                        rct.X = Convert.ToInt32(c);
                        rct.Y = Convert.ToInt32(r);
                        rct.Width = bmpMatch.Width;
                        rct.Height = bmpMatch.Height;
                        break;
                    }
                }

                bmpMatch.UnlockBits(ImgBmd);
                ScreenBmp.UnlockBits(ScreenBmd);
                ScreenBmp.Dispose();
            }
            catch(Exception ex)
            {
                Logging.Logging.log_error("Astaroth Google Recaptcha", "FindImageOnScreen", ex.Message);
                return rct;
            }

            return rct;
        }

        public static void SimulateMove(int target_X, int target_Y, int steps)
        {
            try
            {
                //Set Random Mouse Cursor Position
                Random random_cur = new Random();
                SetCursorPos(random_cur.Next(50, 1920), random_cur.Next(50, 1920));

                //Get current Mouse Cursor Position
                PointF iterPoint = Cursor.Position;

                //Find the slope of the line segment defined by start and newPosition
                PointF slope = new PointF(target_X - iterPoint.X, target_Y - iterPoint.Y);

                //Divide by the number of steps
                slope.X = slope.X / steps;
                slope.Y = slope.Y / steps;

                //Move the mouse to each iterative point.
                for (int i = 0; i < steps; i++)
                {
                    iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                    SetCursorPos(Convert.ToInt32(iterPoint.X), Convert.ToInt32(iterPoint.Y));
                    Random random_ms = new Random();
                    Thread.Sleep(random_ms.Next(1, 30));
                }

                //Move the mouse to the final destination.
                SetCursorPos(target_X, target_Y);
            }
            catch (Exception ex)
            {
                Logging.Logging.log_error("Astaroth Google Recaptcha", "SimulateMove", ex.Message);
            }
        }

        public static void take_ss()
        {
            try
            {
                var Main_Form_Init = Application.OpenForms.OfType<Main_Form>().FirstOrDefault();

                Main_Form_Init.ss_count++;
                Main_Form_Init.ss = "ss_" + Main_Form_Init.ss_count.ToString();

                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;

                Rectangle rect1 = new Rectangle(0, 0, screenWidth, screenHeight);
                Bitmap bmp = new Bitmap(rect1.Width, rect1.Height, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect1.Left, rect1.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
                bmp.Save(@"C:\autobot\ss\" + Main_Form_Init.ss + ".png", ImageFormat.Png);
                bmp.Dispose();
            }
            catch (Exception ex)
            {
                Logging.Logging.log_error("Astaroth Google Recaptcha", "take_ss", ex.Message);
            }
        }
    }
}
