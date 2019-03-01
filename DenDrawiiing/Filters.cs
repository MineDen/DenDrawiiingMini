using DenDrawiiing.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DenDrawiiing
{
    public static class Filters
    {
        public delegate void Filter(Bitmap bmp, Color c);

        public static void Invert(Bitmap bmp, Color c)
        {
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(new Point(0, 0), bmp.Size), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr fl = data.Scan0;
            int bytes = Math.Abs(data.Stride) * bmp.Height;
            byte[] rgb = new byte[bytes];
            Marshal.Copy(fl, rgb, 0, bytes);
            for (int i = 0; i < bytes; i += 4)
            {
                // it's fucking BGRA!!!
                rgb[i] = (byte)(255 - rgb[i]);
                rgb[i + 1] = (byte)(255 - rgb[i + 1]);
                rgb[i + 2] = (byte)(255 - rgb[i + 2]);
            }
            Marshal.Copy(rgb, 0, fl, bytes);
            bmp.UnlockBits(data);
        }

        public static void Greyscale(Bitmap bmp, Color c)
        {
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(new Point(0, 0), bmp.Size), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr fl = data.Scan0;
            int bytes = Math.Abs(data.Stride) * bmp.Height;
            byte[] rgb = new byte[bytes];
            Marshal.Copy(fl, rgb, 0, bytes);
            for (int i = 0; i < bytes; i += 4)
            {
                // it's fucking BGRA!!!
                byte avg = (byte)Math.Round((rgb[i] + rgb[i + 1] + rgb[i + 2]) / 3d);
                rgb[i] = avg;
                rgb[i + 1] = avg;
                rgb[i + 2] = avg;
            }
            Marshal.Copy(rgb, 0, fl, bytes);
            bmp.UnlockBits(data);
        }

        public static void Drugs(Bitmap bmp, Color c)
        {
            Random rnd = new Random();
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(new Point(0, 0), bmp.Size), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr fl = data.Scan0;
            int bytes = Math.Abs(data.Stride) * bmp.Height;
            byte[] rgb = new byte[bytes];
            Marshal.Copy(fl, rgb, 0, bytes);
            for (int i = 0; i < bytes; i += 4)
            {
                // it's fucking BGRA!!!
                rgb[i] = (byte)Math.Max(Math.Min(rgb[i] + (rnd.NextDouble() * 32 - 16), 255), 0);
                rgb[i + 1] = (byte)Math.Max(Math.Min(rgb[i + 1] + (rnd.NextDouble() * 32 - 16), 255), 0);
                rgb[i + 2] = (byte)Math.Max(Math.Min(rgb[i + 2] + (rnd.NextDouble() * 32 - 16), 255), 0);
            }
            Marshal.Copy(rgb, 0, fl, bytes);
            bmp.UnlockBits(data);
        }

        public static void Recolor(Bitmap bmp, Color c)
        {
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(new Point(0, 0), bmp.Size), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr fl = data.Scan0;
            int bytes = Math.Abs(data.Stride) * bmp.Height;
            byte[] rgb = new byte[bytes];
            Marshal.Copy(fl, rgb, 0, bytes);
            for (int i = 0; i < bytes; i += 4)
            {
                // it's fucking BGRA!!!
                byte oldB = rgb[i];
                rgb[i] = rgb[i + 2];
                rgb[i + 1] = (byte)Math.Round((rgb[i] + oldB) / 2d);
                rgb[i + 2] = oldB;
            }
            Marshal.Copy(rgb, 0, fl, bytes);
            bmp.UnlockBits(data);
        }

        public static void ColorPress(Bitmap bmp, Color c)
        {
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(new Point(0, 0), bmp.Size), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr fl = data.Scan0;
            int bytes = Math.Abs(data.Stride) * bmp.Height;
            byte[] rgb = new byte[bytes];
            Marshal.Copy(fl, rgb, 0, bytes);
            for (int i = 0; i < bytes; i += 4)
            {
                // it's fucking BGRA!!!
                rgb[i] = IsDark(rgb[i]) ? (byte)(255 - rgb[i]) : rgb[i + 1]; // B
                //rgb[i + 1] = ; // G
                rgb[i + 2] = IsDark(rgb[i + 2]) ? rgb[i + 2] : (byte)(255 - rgb[i + 2]); // R
            }
            Marshal.Copy(rgb, 0, fl, bytes);
            bmp.UnlockBits(data);
        }

        public static void Tint(Bitmap bmp, Color c)
        {
            ImageTint.Tint(bmp, c);
        }

        private static bool IsDark(byte channel)
        {
            return channel < 127;
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static bool IsDark(byte r, byte g, byte b)
#pragma warning restore IDE0051 // Remove unused private members
        {
            return ((r + g + b) / 3d) < 127;
        }
    }
}
