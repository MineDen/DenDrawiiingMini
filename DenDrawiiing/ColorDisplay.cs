using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace DenDrawiiing
{
    public class ColorDisplay : UserControl
    {
        private Color color = Color.Black;
        private Color color2 = Color.White;
        private bool isDark = true;
        private bool isDark2 = false;
        private float size = 2.0f;
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                if ((color.R + color.G + color.B) / 3d < 127)
                    isDark = true;
                else isDark = false;
                Invalidate();
            }
        }
        public Color Color2
        {
            get => color2;
            set
            {
                color2 = value;
                if ((color2.R + color2.G + color2.B) / 3d < 127)
                    isDark2 = true;
                else isDark2 = false;
                Invalidate();
            }
        }

        public float ToolSize {
            get => size;
            set
            {
                size = value;
                Invalidate();
            }
        }

        public ColorDisplay()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color), 2, 2, 44, 44);
            g.DrawRectangle(new Pen(Color.FromArgb(60, isDark ? Color.White : Color.Black)), 2, 2, 43, 43);
            g.FillRectangle(
                new LinearGradientBrush(
                    new Point(50, 0),
                    new Point(72, 0),
                    Color, Color2
                    ), 50, 2, 22, 44);
            g.DrawRectangle(new Pen(
                Color.FromArgb(60,
                    (Color.R + Color.G + Color.B + Color2.R + Color2.G + Color2.B) / 6d < 127 ? Color.White : Color.Black)
                ), 50, 2, 21, 43);
            g.FillRectangle(new SolidBrush(Color2), 76, 2, 44, 44);
            g.DrawRectangle(new Pen(Color.FromArgb(60, isDark2 ? Color.White : Color.Black)), 76, 2, 43, 43);
            GraphicsState gState = g.Save();
            g.SetClip(new Rectangle(124, 2, 44, 44));
            g.TranslateTransform(124, 2);
            g.Clear(isDark ? Color.White : Color.Black);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillEllipse((Brush)(ColorAPI.Color)Color, 22 - ToolSize / 2, 22 - ToolSize / 2, ToolSize, ToolSize);
            g.SmoothingMode = SmoothingMode.Default;
            g.DrawRectangle(new Pen(Color.FromArgb(60, isDark ? Color.Black : Color.White)), 0, 0, 43, 43);
            g.Restore(gState);
        }
    }
}
