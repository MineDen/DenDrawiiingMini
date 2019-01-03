using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class FloodFill : Tool
    {
        public override void DrawEnd(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p; // refine before serializing
        }

        public override void DrawStart(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p;
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                startPoint = p;
                Color targetColor = img.GetPixel(p.X, p.Y);
                Stack<Point> pixels = new Stack<Point>();
                pixels.Push(p);

                while (pixels.Count > 0)
                {
                    Point a = pixels.Pop();
                    if (a.X <= img.Width && a.X >= 0 &&
                            a.Y <= img.Height && a.Y >= 0)//make sure we stay within bounds
                    {

                        if (img.GetPixel(a.X, a.Y) == targetColor)
                        {
                            img.SetPixel(a.X, a.Y, color);
                            pixels.Push(new Point(a.X - 1, a.Y));
                            pixels.Push(new Point(a.X + 1, a.Y));
                            pixels.Push(new Point(a.X, a.Y - 1));
                            pixels.Push(new Point(a.X, a.Y + 1));
                        }
                    }
                }
            }
        }

        public override void Serialize(Graphics bmp, Color color, Bitmap img)
        {
            throw new NotImplementedException();
        }
    }
}
