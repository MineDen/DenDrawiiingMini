using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Utils
{
    public static class ImageTint
    {
        public static void Tint(Bitmap src, Color c)
        {
            Rectangle rect = new Rectangle(new Point(0, 0), src.Size);
            Bitmap dst = src.Clone(rect, PixelFormat.Format32bppArgb);
            BitmapData data = dst.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            IntPtr fl = data.Scan0;
            int bytes = Math.Abs(data.Stride) * dst.Height;
            byte[] rgb = new byte[bytes];
            Marshal.Copy(fl, rgb, 0, bytes);

            for (int i = 0; i < bytes; i += 4)
            {
                rgb[i] = Limit(rgb[i] * c.B / 255d);
                rgb[i + 1] = Limit(rgb[i + 1] * c.G / 255d);
                rgb[i + 2] = Limit(rgb[i + 2] * c.R / 255d);
                //rgb[i + 3] = Limit((c.A - rgb[i + 3]) * 0.75m + rgb[i + 3]);
            }

            Marshal.Copy(rgb, 0, fl, bytes);
            dst.UnlockBits(data);
            using (Graphics g = Graphics.FromImage(src))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(dst, 0, 0);
            }
            dst.Dispose();
        }

        public static byte[] TintColor(byte r1, byte g1, byte b1, byte a, byte r2, byte g2, byte b2)
        {
            byte[] bgra = new byte[4];
            bgra[0] = Limit(b1 * b2 / 255d);
            bgra[1] = Limit(g1 * g2 / 255d);
            bgra[2] = Limit(r1 * r2 / 255d);
            bgra[3] = a;
            return bgra;
        }

        private static byte Limit(double val)
        {
            return (byte)Math.Max(Math.Min(Math.Round(val), byte.MaxValue), byte.MinValue);
        }
    }
}
