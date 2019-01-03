using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class Rectangle : Tool
    {
        public Point lastPoint;

        public float size = 2.0f;

        public override void DrawEnd(Point p, Graphics canvas, Color color, Bitmap img)
        {
            
        }

        public override void DrawStart(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p;
            lastPoint = startPoint;
            Pen pen = new Pen(color, size);
            int rsx = Math.Min(startPoint.X, lastPoint.X);
            int rsy = Math.Min(startPoint.Y, lastPoint.Y);
            Size rsize = new Size(Math.Abs(startPoint.X - lastPoint.X), Math.Abs(startPoint.Y - lastPoint.Y));
            if (rsize.Width <= size || rsize.Height <= size)
                using (SolidBrush brush = new SolidBrush(color))
                    canvas.FillRectangle(brush, rsx - size / 2, rsy - size / 2, rsize.Width + size, rsize.Height + size);
            else
                canvas.DrawRectangle(pen, rsx, rsy, rsize.Width, rsize.Height);
            pen.Dispose();
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                lastPoint = p;
                Pen pen = new Pen(color, size);
                int rsx = Math.Min(startPoint.X, lastPoint.X);
                int rsy = Math.Min(startPoint.Y, lastPoint.Y);
                Size rsize = new Size(Math.Abs(startPoint.X - lastPoint.X), Math.Abs(startPoint.Y - lastPoint.Y));
                if (rsize.Width <= size || rsize.Height <= size)
                    using (SolidBrush brush = new SolidBrush(color))
                        canvas.FillRectangle(brush, rsx - size / 2, rsy - size / 2, rsize.Width + size, rsize.Height + size);
                else
                    canvas.DrawRectangle(pen, rsx, rsy, rsize.Width, rsize.Height);
                pen.Dispose();
            }
        }

        public override void Serialize(Graphics bmp, Color color, Bitmap img)
        {
            Pen pen = new Pen(color, size);
            int rsx = Math.Min(startPoint.X, lastPoint.X);
            int rsy = Math.Min(startPoint.Y, lastPoint.Y);
            Size rsize = new Size(Math.Abs(startPoint.X - lastPoint.X), Math.Abs(startPoint.Y - lastPoint.Y));
            if (rsize.Width <= size || rsize.Height <= size)
                using (SolidBrush brush = new SolidBrush(color))
                    bmp.FillRectangle(brush, rsx - size / 2, rsy - size / 2, rsize.Width + size, rsize.Height + size);
            else
                bmp.DrawRectangle(pen, rsx, rsy, rsize.Width, rsize.Height);
            pen.Dispose();
        }
    }
}
