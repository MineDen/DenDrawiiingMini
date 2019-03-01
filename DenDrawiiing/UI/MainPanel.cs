using DenDrawiiing.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private Button fillAll;
        private Button clearAll;
        private PanelHeader stampHeader;
        private ComboBox stamp;
        private CheckBox tintStamp;
        private PanelHeader selectionHeader;
        private CheckBox showSelection;
        private CheckBox previewSelection;

        public void InitializePanel()
        {
            generalHeader = new PanelHeader
            {
                Location = new Point(0, 0),
                Size = new Size(240, 16),
                Text = "Загальні",
                BackColor = (Color)GlobalSettings.GetColor(3),
                ForeColor = (Color)GlobalSettings.GetColor(5)
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

            fillAll = new Button()
            {
                Location = new Point(6, 192),
                Size = new Size(110, 28),
                Text = "Заповнити все"
            };
            fillAll.Click += (object sender, EventArgs e) =>
            {
                History.Action((Bitmap)bmp.Clone());
                bmpGraphics.Clear(Color);
                BufferDrawStart();
                BufferDrawEnd();
            };

            clearAll = new Button()
            {
                Location = new Point(124, 192),
                Size = new Size(110, 28),
                Text = "Очистити все"
            };
            clearAll.Click += (object sender, EventArgs e) =>
            {
                History.Action((Bitmap)bmp.Clone());
                bmpGraphics.Clear(Color.Transparent);
                BufferDrawStart();
                BufferDrawEnd();
            };

            filters = new Button
            {
                Location = new Point(6, 226),
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
            filtersStrip.Items.Add("Заміна кольору", null, (object sender, EventArgs e) => Execute(Filters.Tint));

            tools = new Button
            {
                Location = new Point(124, 226),
                Size = new Size(110, 28),
                Text = "Інструменти"
            };
            tools.Click += OnToolsClick;

            toolsStrip = new ContextMenuStrip();
            toolsStrip.Items.Add("Пензель", null, (object sender, EventArgs e) => SetTool(Tool.Brush));
            toolsStrip.Items.Add("Ґумка", null, (object sender, EventArgs e) => SetTool(Tool.Eraser));
            toolsStrip.Items.Add("Лінія", null, (object sender, EventArgs e) => SetTool(Tool.Line));
            toolsStrip.Items.Add("Заповнення", null, (object sender, EventArgs e) => SetTool(Tool.FloodFill)); // floodfill is incomplete
            toolsStrip.Items.Add("Веселковий пензель", null, (object sender, EventArgs e) => SetTool(Tool.RainbowBrush));
            toolsStrip.Items.Add("Прямокутник", null, (object sender, EventArgs e) => SetTool(Tool.Rectangle));
            toolsStrip.Items.Add("Еліпс", null, (object sender, EventArgs e) => SetTool(Tool.Ellipse));
            toolsStrip.Items.Add("Трикутник", null, (object sender, EventArgs e) => SetTool(Tool.Triangle));
            toolsStrip.Items.Add("Довільна фігура", null, (object sender, EventArgs e) => SetTool(Tool.FreeShape));
            toolsStrip.Items.Add("Штамп", null, (object sender, EventArgs e) => SetTool(Tool.Stamp));
            toolsStrip.Items.Add(new ToolStripSeparator());
            toolsStrip.Items.Add("Довільне виділення", null, (object sender, EventArgs e) => SetTool(Tool.FreeSelection));

            stampHeader = new PanelHeader
            {
                Location = new Point(0, 260),
                Size = new Size(240, 16),
                Text = "Штамп",
                BackColor = (Color)GlobalSettings.GetColor(3),
                ForeColor = (Color)GlobalSettings.GetColor(5)
            };

            stamp = new ComboBox
            {
                Location = new Point(6, 282),
                Size = new Size(228, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            Tool.Stamp.stamps = Directory.GetFiles("stamps", "*.png", SearchOption.AllDirectories);
            for (int i = 0; i < Tool.Stamp.stamps.Length; i++)
            {
                stamp.Items.Add(Path.GetFileNameWithoutExtension(Tool.Stamp.stamps[i]));
            }
            if (Tool.Stamp.stamps.Length > 0)
            {
                stamp.Enabled = true;
                stamp.SelectedIndex = 0;
                Tool.Stamp.selectedStamp = Path.GetFileNameWithoutExtension(Tool.Stamp.stamps[0]);
            }
            stamp.SelectedIndexChanged += (object sender, EventArgs e) =>
            {
                Tool.Stamp.selectedStamp = Path.GetFileNameWithoutExtension(Tool.Stamp.stamps[stamp.SelectedIndex]);
            };

            tintStamp = new CheckBox
            {
                AutoSize = false,
                Location = new Point(6, 312),
                Size = new Size(228, 24),
                Text = "Зафарбовувати штамп",
                Checked = true
            };
            tintStamp.CheckedChanged += (object sender, EventArgs e) =>
            {
                Tool.Stamp.tintStamp = tintStamp.Checked;
            };

            selectionHeader = new PanelHeader
            {
                Location = new Point(0, 342),
                Size = new Size(240, 16),
                Text = "Виділення",
                BackColor = (Color)GlobalSettings.GetColor(3),
                ForeColor = (Color)GlobalSettings.GetColor(5)
            };

            showSelection = new CheckBox
            {
                AutoSize = false,
                Location = new Point(6, 364),
                Size = new Size(228, 24),
                Text = "Відображати виділення",
                Checked = true
            };
            showSelection.CheckedChanged += (object sender, EventArgs e) =>
            {
                canvas.Invalidate();
            };

            previewSelection = new CheckBox
            {
                AutoSize = false,
                Location = new Point(6, 394),
                Size = new Size(228, 24),
                Text = "Перегляд результату (повільно)",
                Checked = false
            };

            mainPanel.Controls.Add(generalHeader);
            mainPanel.Controls.Add(colorPicker);
            mainPanel.Controls.Add(colorDisplay);
            mainPanel.Controls.Add(brushSize);
            mainPanel.Controls.Add(fillAll);
            mainPanel.Controls.Add(clearAll);
            mainPanel.Controls.Add(filters);
            mainPanel.Controls.Add(tools);
            mainPanel.Controls.Add(stampHeader);
            mainPanel.Controls.Add(stamp);
            mainPanel.Controls.Add(tintStamp);
            mainPanel.Controls.Add(selectionHeader);
            mainPanel.Controls.Add(showSelection);
            mainPanel.Controls.Add(previewSelection);

            mainPanel.Paint += (object sender, PaintEventArgs e) =>
            {
                e.Graphics.DrawLine(new Pen((Color)GlobalSettings.GetColor(2)), 239, 0, 239, Height);
            };
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
            filter(bmp, Color);
            BufferDrawStart();
            BufferDrawEnd();
        }

        private void OnFiltersClick(object sender, EventArgs e)
        {
            filtersStrip.Show(filters.PointToScreen(new Point(0, filters.Height)));
        }

        private void OnBrushSizeChanged(object sender, EventArgs e)
        {
            colorDisplay.ToolSize = brushSize.Value / 2f;
            Tool.Brush.size = brushSize.Value / 2f;
            Tool.Eraser.size = brushSize.Value / 2f;
            Tool.Line.size = brushSize.Value / 2f;
            Tool.RainbowBrush.size = brushSize.Value / 2f;
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
