using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenDrawiiing.Tools
{
    public abstract class Tool
    {
        public static readonly Brush Brush = new Brush();
        public static readonly Eraser Eraser = new Eraser();
        public static readonly Line Line = new Line();
        public static readonly RainbowBrush RainbowBrush = new RainbowBrush();
        public static readonly Rectangle Rectangle = new Rectangle();
        public static readonly Ellipse Ellipse = new Ellipse();
        public static readonly Triangle Triangle = new Triangle();
        public static readonly FreeShape FreeShape = new FreeShape();
        public static readonly Stamp Stamp = new Stamp();
        public static readonly FreeSelection FreeSelection = new FreeSelection();

        public static Tool Current = Brush;

        public Point startPoint;

        public abstract void DrawStart(Point p, Graphics canvas, Color color, Bitmap img);
        public abstract void MouseMove(Point p, Graphics canvas, Color color, Bitmap img, bool pressed);
        public abstract void DrawEnd(Point p, Graphics canvas, Color color, Bitmap img);
        public abstract void Serialize(Graphics bmp, Color color, Bitmap img);
    }
}
