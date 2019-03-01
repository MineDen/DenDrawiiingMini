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
    public static class ImageMask
    {
        public static void Mask(Bitmap src, Bitmap mask, Bitmap dst)
        {
            Rectangle rect = new Rectangle(new Point(0, 0), src.Size);
            BitmapData srcData = src.LockBits(rect, ImageLockMode.ReadOnly, src.PixelFormat);
            IntPtr sfl = srcData.Scan0;
            int sbytes = Math.Abs(srcData.Stride) * src.Height;
            byte[] srgb = new byte[sbytes];
            Marshal.Copy(sfl, srgb, 0, sbytes);

            BitmapData maskData = mask.LockBits(rect, ImageLockMode.ReadOnly, mask.PixelFormat);
            IntPtr mfl = maskData.Scan0;
            int mbytes = Math.Abs(maskData.Stride) * mask.Height;
            byte[] mrgb = new byte[mbytes];
            Marshal.Copy(mfl, mrgb, 0, mbytes);

            BitmapData dstData = dst.LockBits(rect, ImageLockMode.ReadWrite, dst.PixelFormat);
            IntPtr dfl = dstData.Scan0;
            int dbytes = Math.Abs(dstData.Stride) * dst.Height;
            byte[] drgb = new byte[dbytes];
            Marshal.Copy(dfl, drgb, 0, dbytes);
            if (dbytes != sbytes || dbytes != mbytes || sbytes != mbytes)
                throw new ArgumentException("The sizes of bitmaps must be equal.");

            for (int i = 0; i < sbytes; i += 4)
            {
                double alpha = (mrgb[i + 2] + mrgb[i + 1] + mrgb[i]) / 3d / 255d;
                drgb[i] = (byte)(alpha * srgb[i] + (1 - alpha) * drgb[i]);
                drgb[i + 1] = (byte)(alpha * srgb[i + 1] + (1 - alpha) * drgb[i + 1]);
                drgb[i + 2] = (byte)(alpha * srgb[i + 2] + (1 - alpha) * drgb[i + 2]);
                drgb[i + 3] = (byte)(alpha * srgb[i + 3] + (1 - alpha) * drgb[i + 3]);
            }

            Marshal.Copy(drgb, 0, dfl, dbytes);
            src.UnlockBits(srcData);
            mask.UnlockBits(maskData);
            dst.UnlockBits(dstData);
        }

        public static Bitmap GetMaskVisual(Bitmap mask, Color color)
        {
            Bitmap bmp = new Bitmap(mask.Width, mask.Height);

            Rectangle rect = new Rectangle(new Point(0, 0), mask.Size);
            BitmapData mData = mask.LockBits(rect, ImageLockMode.ReadOnly, mask.PixelFormat);
            IntPtr mfl = mData.Scan0;
            int mbytes = Math.Abs(mData.Stride) * mask.Height;
            byte[] mrgb = new byte[mbytes];
            Marshal.Copy(mfl, mrgb, 0, mbytes);

            BitmapData bData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr bfl = bData.Scan0;
            int bbytes = Math.Abs(bData.Stride) * bmp.Height;
            byte[] brgb = new byte[bbytes];
            Marshal.Copy(bfl, brgb, 0, bbytes);

            for (int i = 0; i < bbytes; i += 4)
            {
                double alpha = (mrgb[i + 2] + mrgb[i + 1] + mrgb[i]) / 3d / 255d;
                brgb[i] = (byte)(alpha * color.B + (1 - alpha) * brgb[i]);
                brgb[i + 1] = (byte)(alpha * color.G + (1 - alpha) * brgb[i + 1]);
                brgb[i + 2] = (byte)(alpha * color.R + (1 - alpha) * brgb[i + 2]);
                brgb[i + 3] = (byte)(alpha * color.A + (1 - alpha) * brgb[i + 3]);
            }

            Marshal.Copy(brgb, 0, bfl, bbytes);
            mask.UnlockBits(mData);
            bmp.UnlockBits(bData);
            return bmp;
        }
    }
}