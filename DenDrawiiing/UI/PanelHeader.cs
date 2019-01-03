using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DenDrawiiing.UI
{
    public class PanelHeader : UserControl
    {
        public PanelHeader()
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            LinearGradientBrush b = new LinearGradientBrush(
                new Point(0, 0),
                new Point(Width, 0),
                Color.SeaGreen,
                BackColor
                );
            g.FillRectangle(b, 0, 0, Width, Height);
            g.DrawString(Text, new Font(SystemFonts.CaptionFont, FontStyle.Bold), Brushes.White, 0, 0);
        }
    }
}