using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class Triangle : Tool
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
            Pen pen = new Pen(color, size) { LineJoin = LineJoin.Round };
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            int rsx = Math.Min(startPoint.X, lastPoint.X);
            int rsy = Math.Min(startPoint.Y, lastPoint.Y);
            bool flipvert = startPoint.Y - lastPoint.Y > 0;
            Size rsize = new Size(Math.Abs(startPoint.X - lastPoint.X), Math.Abs(startPoint.Y - lastPoint.Y));
            GraphicsPath path = new GraphicsPath();
            path.AddLine(rsx + rsize.Width, flipvert ? rsy : rsy + rsize.Height,
                rsx, flipvert ? rsy : rsy + rsize.Height);
            path.AddLine(rsx, flipvert ? rsy : rsy + rsize.Height,
                startPoint.X + (lastPoint.X - startPoint.X) / 2, startPoint.Y);
            path.CloseFigure();
            canvas.DrawPath(pen, path);
            if (rsize.Width <= size || rsize.Height <= size)
                using (SolidBrush brush = new SolidBrush(color))
                {
                    canvas.FillEllipse(brush, rsx + rsize.Width / 2 - size / 2, rsy + rsize.Height / 2 - size / 2,
                        size, size);
                }
            pen.Dispose();
            path.Dispose();
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                lastPoint = p;
                Pen pen = new Pen(color, size) { LineJoin = LineJoin.Round };
                pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
                int rsx = Math.Min(startPoint.X, lastPoint.X);
                int rsy = Math.Min(startPoint.Y, lastPoint.Y);
                bool flipvert = startPoint.Y - lastPoint.Y > 0;
                Size rsize = new Size(Math.Abs(startPoint.X - lastPoint.X), Math.Abs(startPoint.Y - lastPoint.Y));
                GraphicsPath path = new GraphicsPath();
                path.AddLine(rsx + rsize.Width, flipvert ? rsy : rsy + rsize.Height,
                    rsx, flipvert ? rsy : rsy + rsize.Height);
                path.AddLine(rsx, flipvert ? rsy : rsy + rsize.Height,
                    startPoint.X + (lastPoint.X - startPoint.X) / 2, startPoint.Y);
                path.CloseFigure();
                canvas.DrawPath(pen, path);
                if (rsize.Width <= size || rsize.Height <= size)
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        canvas.FillEllipse(brush, rsx + rsize.Width / 2 - size / 2, rsy + rsize.Height / 2 - size / 2,
                            size, size);
                    }
                pen.Dispose();
                path.Dispose();
            }
        }

        public override void Serialize(Graphics bmp, Color color, Bitmap img)
        {
            Pen pen = new Pen(color, size) { LineJoin = LineJoin.Round };
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            int rsx = Math.Min(startPoint.X, lastPoint.X);
            int rsy = Math.Min(startPoint.Y, lastPoint.Y);
            bool flipvert = startPoint.Y - lastPoint.Y > 0;
            Size rsize = new Size(Math.Abs(startPoint.X - lastPoint.X), Math.Abs(startPoint.Y - lastPoint.Y));
            GraphicsPath path = new GraphicsPath();
            path.AddLine(rsx + rsize.Width, flipvert ? rsy : rsy + rsize.Height,
                rsx, flipvert ? rsy : rsy + rsize.Height);
            path.AddLine(rsx, flipvert ? rsy : rsy + rsize.Height,
                startPoint.X + (lastPoint.X - startPoint.X) / 2, startPoint.Y);
            path.CloseFigure();
            bmp.DrawPath(pen, path);
            if (rsize.Width <= size || rsize.Height <= size)
                using (SolidBrush brush = new SolidBrush(color))
                {
                    bmp.FillEllipse(brush, rsx + rsize.Width / 2 - size / 2, rsy + rsize.Height / 2 - size / 2,
                        size, size);
                }
            pen.Dispose();
            path.Dispose();
        }
    }
}
