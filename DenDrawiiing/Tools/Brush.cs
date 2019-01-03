using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class Brush : Tool
    {
        public GraphicsPath path;
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
            path = new GraphicsPath();
            Pen pen = new Pen(color, size) { LineJoin = LineJoin.Round };
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            path.AddLine(startPoint, startPoint);
            canvas.DrawPath(pen, path);
            pen.Dispose();
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                Pen pen = new Pen(color, size) { LineJoin = LineJoin.Round };
                pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
                path.AddLine(lastPoint, p);
                lastPoint = p;
                canvas.DrawPath(pen, path);
                pen.Dispose();
            }
        }

        public override void Serialize(Graphics bmp, Color color, Bitmap img)
        {
            Pen pen = new Pen(color, size) { LineJoin = LineJoin.Round };
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            bmp.DrawPath(pen, path);
            pen.Dispose();
            path.Dispose();
        }
    }
}
