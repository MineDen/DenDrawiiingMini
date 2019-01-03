using DenDrawiiing.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DenDrawiiing.UI
{
    public partial class MainForm
    {
        private PanelHeader generalHeader;
        private ColorPicker colorPicker;
        private ColorDisplay colorDisplay;
        private TrackBar brushSize;
        private Button filters;
        private ContextMenuStrip filtersStrip;
        private Button tools;
        private ContextMenuStrip toolsStrip;

        public void InitializePanel()
        {
            generalHeader = new PanelHeader
            {
                Location = new Point(0, 0),
                Size = new Size(240, 16),
                Text = "Загальні"
            };

            colorPicker = new ColorPicker
            {
                Location = new Point(6, 22),
                ClientSize = new Size(228, 72)
            };
            colorPicker.OnColorChange += OnColorChange;

            colorDisplay = new ColorDisplay
            {
                Location = new Point(6, 100),
                ClientSize = new Size(228, 48)
            };
            colorDisplay.MouseDown += OnColorSwap;

            brushSize = new TrackBar
            {
                AutoSize = false,
                Minimum = 2,
                Value = 4,
                Maximum = 100,
                Location = new Point(6, 154),
                ClientSize = new Size(228, 32),
                TickStyle = TickStyle.None
            };
            brushSize.ValueChanged += OnBrushSizeChanged;

            filters = new Button
            {
                Location = new Point(6, 192),
                Size = new Size(110, 28),
                Text = "Фільтри"
            };
            filters.Click += OnFiltersClick;

            filtersStrip = new ContextMenuStrip();
            filtersStrip.Items.Add("Інвертувати кольори", null, (object sender, EventArgs e) => Execute(Filters.Invert));
            filtersStrip.Items.Add("Градації сірого", null, (object sender, EventArgs e) => Execute(Filters.Greyscale));
            filtersStrip.Items.Add("Шум", null, (object sender, EventArgs e) => Execute(Filters.Drugs));
            filtersStrip.Items.Add("Перефарбувати", null, (object sender, EventArgs e) => Execute(Filters.Recolor));
            filtersStrip.Items.Add("Стиснення кольорів", null, (object sender, EventArgs e) => Execute(Filters.ColorPress));

            tools = new Button
            {
                Location = new Point(124, 192),
                Size = new Size(110, 28),
                Text = "Інструменти"
            };
            tools.Click += OnToolsClick;

            toolsStrip = new ContextMenuStrip();
            toolsStrip.Items.Add("Пензель", null, (object sender, EventArgs e) => SetTool(Tool.Brush));
            toolsStrip.Items.Add("Ґумка", null, (object sender, EventArgs e) => SetTool(Tool.Eraser));
            toolsStrip.Items.Add("Лінія", null, (object sender, EventArgs e) => SetTool(Tool.Line));
            // toolsStrip.Items.Add("Заповнення", null, (object sender, EventArgs e) => SetTool(Tool.FloodFill)); // floodfill is incomplete
            toolsStrip.Items.Add("Прямокутник", null, (object sender, EventArgs e) => SetTool(Tool.Rectangle));
            toolsStrip.Items.Add("Еліпс", null, (object sender, EventArgs e) => SetTool(Tool.Ellipse));
            toolsStrip.Items.Add("Трикутник", null, (object sender, EventArgs e) => SetTool(Tool.Triangle));
            toolsStrip.Items.Add("Довільна фігура", null, (object sender, EventArgs e) => SetTool(Tool.FreeShape));

            mainPanel.Controls.Add(generalHeader);
            mainPanel.Controls.Add(colorPicker);
            mainPanel.Controls.Add(colorDisplay);
            mainPanel.Controls.Add(brushSize);
            mainPanel.Controls.Add(filters);
            mainPanel.Controls.Add(tools);
        }

        private void OnColorSwap(object sender, MouseEventArgs e)
        {
            if (e.X >= 50 && e.X < 72 && e.Y >= 2 && e.Y < 46)
            {
                Color c2 = Color2;
                Color2 = Color;
                Color = c2;
                colorPicker.UpdateColorNoSave(Color.R, Color.G, Color.B);
            }
        }

        private void OnToolsClick(object sender, EventArgs e)
        {
            toolsStrip.Show(tools.PointToScreen(new Point(0, tools.Height)));
        }

        private void SetTool(Tool tool)
        {
            Tool.Current = tool;
        }

        private void Execute(Filters.Filter filter)
        {
            History.Action((Bitmap)bmp.Clone());
            filter(bmp);
            BufferDrawStart();
            BufferDrawEnd();
        }

        private void OnFiltersClick(object sender, EventArgs e)
        {
            filtersStrip.Show(filters.PointToScreen(new Point(0, filters.Height)));
        }

        private void OnBrushSizeChanged(object sender, EventArgs e)
        {
            Tool.Brush.size = brushSize.Value / 2f;
            Tool.Eraser.size = brushSize.Value / 2f;
            Tool.Line.size = brushSize.Value / 2f;
            Tool.Rectangle.size = brushSize.Value / 2f;
            Tool.Ellipse.size = brushSize.Value / 2f;
            Tool.Triangle.size = brushSize.Value / 2f;
            Tool.FreeShape.size = brushSize.Value / 2f;
        }

        private void OnColorChange(object sender, EventArgs e)
        {
            Color = Color.FromArgb(colorPicker.R, colorPicker.G, colorPicker.B);
        }

        private void ShowColorPicker()
        {
            Form cp = new Form
            {
                Visible = false,
                Location = new Point(0, 0),
                FormBorderStyle = FormBorderStyle.None,
                Size = Screen.PrimaryScreen.Bounds.Size,
                TopMost = true
            };
            bool hint = true;
            bool igf = true;
            Bitmap bmp = new Bitmap(cp.Size.Width, cp.Size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, cp.Size);
            }
            cp.Paint += (object sender, PaintEventArgs e) =>
            {
                Graphics g = e.Graphics;
                g.DrawImage(bmp, 0, 0);
                Font font = new Font("Segoe UI", 16);
                SizeF str = g.MeasureString("Режим піпетки", font);
                g.FillRectangle(new SolidBrush(Color.FromArgb(160, Color.Black)), 10, 10, str.Width, str.Height);
                g.DrawString("Режим піпетки", font, Brushes.White, 10, 10);
                font.Dispose();
            };
            cp.KeyDown += (object sender, KeyEventArgs e) =>
            {
                cp.Close();
            };
            cp.MouseMove += (object sender, MouseEventArgs e) =>
            {
                if (igf)
                {
                    igf = false;
                    return;
                }
                if (hint == true)
                {
                    hint = false;
                    cp.Invalidate();
                }
            };
            cp.MouseClick += (object sender, MouseEventArgs e) =>
            {
                Color c = bmp.GetPixel(e.X, e.Y);
                colorPicker.R = c.R;
                colorPicker.G = c.G;
                colorPicker.B = c.B;
                cp.Close();
            };
            cp.Show();
        }
    }
}
