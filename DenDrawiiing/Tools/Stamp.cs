using DenDrawiiing.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class Stamp : Tool
    {
        public string[] stamps;
        public string selectedStamp = string.Empty;
        public Bitmap stampBmp;
        public bool tintStamp = true;

        public override void DrawEnd(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p;
        }

        public override void DrawStart(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p;
            if (selectedStamp != string.Empty && File.Exists("stamps\\" + selectedStamp + ".png"))
                stampBmp = new Bitmap("stamps\\" + selectedStamp + ".png");
            else
            {
                stampBmp = new Bitmap(64, 64);
                using (Graphics g = Graphics.FromImage(stampBmp))
                using (Pen pen = new Pen(Color.Red, 10))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.DrawEllipse(pen, 5, 5, 53, 53);
                    float sin = (float)Math.Sin(Math.PI * 0.25);
                    float cos = (float)Math.Cos(Math.PI * 0.25);
                    g.DrawLine(pen, 32 - sin * 27, 32 + cos * 27, 32 + sin * 27, 32 - cos * 27);
                }
            }
            if (tintStamp)
                ImageTint.Tint(stampBmp, color);
            canvas.DrawImage(stampBmp, startPoint.X - stampBmp.Width / 2f, startPoint.Y - stampBmp.Height / 2f, stampBmp.Width, stampBmp.Height);
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                startPoint = p;
                canvas.DrawImage(stampBmp, startPoint.X - stampBmp.Width / 2f, startPoint.Y - stampBmp.Height / 2f, stampBmp.Width, stampBmp.Height);
            }
        }

        public override void Serialize(Graphics bmp, Color color, Bitmap img)
        {
            bmp.DrawImage(stampBmp, startPoint.X - stampBmp.Width / 2f, startPoint.Y - stampBmp.Height / 2f, stampBmp.Width, stampBmp.Height);
            stampBmp.Dispose();
        }
    }
}
