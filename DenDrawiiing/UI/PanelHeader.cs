using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DenDrawiiing.UI
{
    public class PanelHeader : UserControl
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(BackColor), 0, 0, Width, Height);
            g.DrawLine(new Pen((Color)GlobalSettings.GetColor(2)), Width - 1, 0, Width - 1, Height - 1);
            g.DrawString(Text, SystemFonts.CaptionFont, new SolidBrush(ForeColor), 0, 0);
        }
    }
}