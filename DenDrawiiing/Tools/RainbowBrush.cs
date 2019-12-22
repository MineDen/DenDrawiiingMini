using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class RainbowBrush : Tool
    {
        public List<Point> path;
        public Point lastPoint;

        public float size = 2.0f;

        public override void DrawEnd(Point p, Graphics canvas, Color color, Bitmap img)
        {
            //path.Dispose();
        }

        public override void DrawStart(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p;
            lastPoint = startPoint;
            path = new List<Point>();
            path.Add(startPoint);
            DrawPoints(canvas);
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                if (((p.X - lastPoint.X) * (p.X - lastPoint.X) + (p.Y - lastPoint.Y) * (p.Y - lastPoint.Y)) > size)
                {
                    path.Add(p);
                    lastPoint = p;
                }
                DrawPoints(canvas);
            }
        }

        public override void Serialize(Graphics bmp, Color color, Bitmap img)
        {
            DrawPoints(bmp);
        }

        private void DrawPoints(Graphics g)
        {
            Pen pen = new Pen(Color.Red, size);
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            for (int i = 0; i < path.Count - 1; i++)
            {
                pen.Color = (Color)ColorAPI.Color.FromHSL((double)i / path.Count, 1, 0.5d);
                g.DrawLine(pen, path[i], path[i + 1]);
            }
            pen.Dispose();
        }
    }
}
