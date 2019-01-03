using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class Line : Tool
    {
        public Point lastPoint;

        public float size = 2.0f;

        public override void DrawEnd(Point p, Graphics canvas, Color color, Bitmap img)
        {
            lastPoint = p; // refine before serializing
        }

        public override void DrawStart(Point p, Graphics canvas, Color color, Bitmap img)
        {
            startPoint = p;
            lastPoint = startPoint;
            Pen pen = new Pen(color, size);
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            canvas.DrawLine(pen, startPoint, lastPoint);
            pen.Dispose();
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                lastPoint = p;
                Pen pen = new Pen(color, size);
                pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
                canvas.DrawLine(pen, startPoint, lastPoint);
                pen.Dispose();
            }
        }

        public override void Serialize(Graphics bmp, Color color, Bitmap img)
        {
            Pen pen = new Pen(color, size);
            pen.SetLineCap(LineCap.Round, LineCap.Round, DashCap.Round);
            bmp.DrawLine(pen, startPoint, lastPoint);
            pen.Dispose();
        }
    }
}
