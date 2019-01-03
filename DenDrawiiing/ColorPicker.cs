using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DenDrawiiing
{
    public class ColorPicker : UserControl
    {
        private byte r = 0;
        private byte g = 0;
        private byte b = 0;
        public byte R
        {
            get => r;
            set
            {
                r = value;
                Invalidate();
                OnColorChange(this, EventArgs.Empty);
            }
        }
        public byte G
        {
            get => g;
            set
            {
                g = value;
                Invalidate();
                OnColorChange(this, EventArgs.Empty);
            }
        }
        public byte B {
            get => b;
            set {
                b = value;
                Invalidate();
                OnColorChange(this, EventArgs.Empty);
            }
        }

        private bool mPressed = false;
        private int cChange = 1;

        public event EventHandler OnColorChange;

        public ColorPicker()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        public void UpdateColorNoSave(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mPressed)
            {
                int sliderHeight = (int)Math.Round((Height - 8) / 3d);

                if (cChange == 0)
                {
                    R = (byte)Math.Round(Math.Max(Math.Min((double)(e.X - 2) / (Width - 4) * 255d, 255), 0));
                }
                if (cChange == 1)
                {
                    G = (byte)Math.Round(Math.Max(Math.Min((double)(e.X - 2) / (Width - 4) * 255d, 255), 0));
                }
                if (cChange == 2)
                {
                    B = (byte)Math.Round(Math.Max(Math.Min((double)(e.X - 2) / (Width - 4) * 255d, 255), 0));
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mPressed = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mPressed = true;
            int sliderHeight = (int)Math.Round((Height - 8) / 3d);
            if (e.Y >= 3 && e.Y <= sliderHeight)
                cChange = 0;
            else if (e.Y >= sliderHeight + 6 && e.Y <= sliderHeight * 2 + 3)
                cChange = 1;
            else if (e.Y >= sliderHeight * 2 + 8 && e.Y <= sliderHeight * 3 + 5)
                cChange = 2;
            else cChange = 3;

            if (e.X < 2 || e.X > Width - 3) cChange = 3;

            if (cChange == 0)
            {
                R = (byte)Math.Round((double)(e.X - 2) / (Width - 4) * 255d);
            }
            if (cChange == 1)
            {
                G = (byte)Math.Round((double)(e.X - 2) / (Width - 4) * 255d);
            }
            if (cChange == 2)
            {
                B = (byte)Math.Round((double)(e.X - 2) / (Width - 4) * 255d);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            int sliderHeight = (int)Math.Round((Height - 8) / 3d);
            for (int x = 0; x < Width - 4; x++)
            {
                int val = (int)Math.Round(x / (double)(Width - 4) * 255);
                g.FillRectangle(new SolidBrush(Color.FromArgb(val, G, B)), 2 + x, 3, 1, sliderHeight - 2);
                g.FillRectangle(new SolidBrush(Color.FromArgb(R, val, B)), 2 + x, 6 + sliderHeight, 1, sliderHeight - 2);
                g.FillRectangle(new SolidBrush(Color.FromArgb(R, G, val)), 2 + x, 8 + sliderHeight * 2, 1, sliderHeight - 2);
            }
            g.DrawRectangle(Pens.Black, 1 + Math.Min((float)(R / 255d * (Width - 4)), Width - 5), 2, 2, sliderHeight - 1);
            g.DrawRectangle(Pens.Black, 1 + Math.Min((float)(G / 255d * (Width - 4)), Width - 5), 5 + sliderHeight, 2, sliderHeight - 1);
            g.DrawRectangle(Pens.Black, 1 + Math.Min((float)(B / 255d * (Width - 4)), Width - 5), 7 + sliderHeight * 2, 2, sliderHeight - 1);
        }
    }
}
