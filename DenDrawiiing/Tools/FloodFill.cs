using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class FloodFill : Tool
    {
        private int w;
        private int h;
        private BitmapData dat;
        private byte[] data;
        public override void DrawEnd(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p; // refine before serializing
        }

        public override void DrawStart(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p; // we use startPoint as last point, because we don't need to store start point
            w = img.Width;
            h = img.Height;
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                SaveData(img);
                startPoint = p;
                ColorAPI.Color targetColor = GetPixel(p.X, p.Y);
                Stack<Point> pixels = new Stack<Point>();
                pixels.Push(p);

                while (pixels.Count > 0)
                {
                    Point a = pixels.Pop();
                    if (a.X <= w && a.X >= 0 &&
                            a.Y <= h && a.Y >= 0)//make sure we stay within bounds
                    {

                        if (GetPixel(a.X, a.Y) == targetColor)
                        {
                            SetPixel(a.X, a.Y, color);
                            pixels.Push(new Point(a.X - 1, a.Y));
                            pixels.Push(new Point(a.X + 1, a.Y));
                            pixels.Push(new Point(a.X, a.Y - 1));
                            pixels.Push(new Point(a.X, a.Y + 1));
                        }
                    }
                }
                RestoreData(img);
            }
        }

        public override void Serialize(Graphics bmp, Color color, Bitmap img)
        {
            throw new NotImplementedException();
        }

        private void SaveData(Bitmap bmp)
        {
            dat = bmp.LockBits(new System.Drawing.Rectangle(new Point(0, 0), bmp.Size), ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr fl = dat.Scan0;
            int bytes = Math.Abs(dat.Stride) * bmp.Height;
            data = new byte[bytes];
            Marshal.Copy(fl, data, 0, bytes);
        }

        private void RestoreData(Bitmap to)
        {
            Marshal.Copy(data, 0, dat.Scan0, data.Length);
            to.UnlockBits(dat);
            dat = null;
            data = null;
        }

        private ColorAPI.Color GetPixel(int x, int y)
        {
            int pos = y * w * 4 + x * 4;
            return new ColorAPI.Color(data[pos + 3], data[pos + 2], data[pos + 1], data[pos]);
        }

        private void SetPixel(int x, int y, ColorAPI.Color c)
        {
            int pos = (y * w + x) * 4;
            data[pos] = c.B;
            data[pos + 1] = c.G;
            data[pos + 2] = c.R;
            data[pos + 3] = c.A;
        }
    }
}
