using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public class FreeSelection : SelectionTool
    {
        public GraphicsPath path;
        public Point lastPoint;

        public override void DrawEnd(Point p, Graphics canvas, Color color, Bitmap img)
        {
            
        }

        public override void DrawStart(Point p, Graphics canvas, Color color, Bitmap img)
        {
            SelectionTool.Selected = false;
            startPoint = p;
            lastPoint = startPoint;
            path = new GraphicsPath();
            SolidBrush brush = new SolidBrush(color);
            path.AddLine(startPoint, startPoint);
            canvas.FillPath(brush, path);
            brush.Dispose();
        }

        public override void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed)
        {
            if (pressed)
            {
                SolidBrush brush = new SolidBrush(color);
                path.AddLine(lastPoint, p);
                lastPoint = p;
                canvas.FillPath(brush, path);
                brush.Dispose();
                SelectionTool.Selected = true;
            }
        }

        public override void Serialize(Graphics mask, Color color, Bitmap img)
        {
            path.AddLine(lastPoint, startPoint);
            mask.FillPath(Brushes.White, path);
            path.Dispose();
        }
    }
}
