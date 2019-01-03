using DenDrawiiing.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DenDrawiiing.UI
{
    public partial class MainForm : Form
    {
        private Panel canvasPanel;
        private PictureBox canvas;
        private Panel mainPanel;

        private Bitmap bmp = new Bitmap(640, 480);
        private Graphics bmpGraphics;
        private Bitmap buffer = new Bitmap(640, 480);
        private Graphics bufferGraphics;

        private bool mPressed = false;
        private Color color = Color.Black;
        private Color color2 = Color.White;

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                colorDisplay.Color = color;
            }
        }
        public Color Color2 {
            get => color2;
            set
            {
                color2 = value;
                colorDisplay.Color2 = color2;
            }
           }
        public MainForm()
        {
            GlobalSettings.Load();

            InitializeComponent();
            InitializePanel();
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            bmp.MakeTransparent();
            buffer.MakeTransparent();
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    bmp.SetPixel(x, y, Color.White);
                }
            }
            bmpGraphics = Graphics.FromImage(bmp);
            bmpGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            bufferGraphics = Graphics.FromImage(buffer);
            bufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            canvas.MouseDown += PMouseDown;
            canvas.MouseMove += PMouseMove;
            canvas.MouseUp += PMouseUp;
            canvas.Paint += PPaint;
            //canvas.Image = buffer;
            canvas.Size = new Size(bmp.Width, bmp.Height);
            BufferDrawStart();
            BufferDrawEnd();
        }

        private void PPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new HatchBrush(HatchStyle.LargeCheckerBoard, Color.White, Color.LightGray),
                0, 0, canvas.Width, canvas.Height);
            e.Graphics.DrawImage(buffer, 0, 0);
        }

        private void BufferDrawStart()
        {
            bufferGraphics.Clear(Color.Transparent);
            bufferGraphics.DrawImage(bmp, 0, 0);
        }

        private void BufferDrawEnd()
        {
            canvas.Invalidate();
        }

        private void PMouseUp(object sender, MouseEventArgs e)
        {
            if (mPressed == true)
            {
                mPressed = false;
                Tool.Current.DrawEnd(e.Location, bufferGraphics, Color, buffer);
                History.Action((Bitmap)bmp.Clone());
                Tool.Current.Serialize(bmpGraphics, Color, bmp);
                BufferDrawStart();
                BufferDrawEnd();
            }
        }

        private void PMouseMove(object sender, MouseEventArgs e)
        {
            BufferDrawStart();
            Tool.Current.MouseMove(e.Location, bufferGraphics, Color, buffer, mPressed);
            BufferDrawEnd();
        }

        private void PMouseDown(object sender, MouseEventArgs e)
        {
            if (mPressed == false)
            {
                mPressed = true;
                BufferDrawStart();
                Tool.Current.DrawStart(e.Location, bufferGraphics, Color, buffer);
                BufferDrawEnd();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.Z)
            {
                if (History.redoHistory.Count >= 1)
                    History.Redo(bmpGraphics, (Bitmap)bmp.Clone());
                BufferDrawStart();
                BufferDrawEnd();
            }
            else if (e.Control && e.KeyCode == Keys.Z)
            {
                if (History.undoHistory.Count >= 1)
                    History.Undo(bmpGraphics, (Bitmap)bmp.Clone());
                BufferDrawStart();
                BufferDrawEnd();
            }
            else if (e.Control && e.KeyCode == Keys.P)
            {
                ShowColorPicker();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    AddExtension = true,
                    AutoUpgradeEnabled = true,
                    DefaultExt = "png",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    Title = "Зберегти як...",
                    FileName = "Без імені.png",
                    Filter =
                    "Portable Network Graphics (*.png)|*.png|Зображення JPEG (*.jpg, *.jpeg)|*.jpg;*.jpeg|Точковий рисунок (*.bmp)|*.bmp|Зображення TIFF (*.tiff, *.tif)|*.tiff;*.tif"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string ext = Path.GetExtension(sfd.FileName);
                    ImageFormat format = ImageFormat.Png;
                    switch (ext)
                    {
                        case ".png":
                            format = ImageFormat.Png;
                            break;
                        case ".jpg":
                        case ".jpeg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                        case ".tiff":
                        case ".tif":
                            format = ImageFormat.Tiff;
                            break;
                    }
                    bmp.Save(sfd.FileName, format);
                }
            }
        }

        #region designer code

        private void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.Panel();
            this.canvasPanel = new System.Windows.Forms.Panel();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.canvasPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(240, 540);
            this.mainPanel.TabIndex = 0;
            this.mainPanel.BackColor = (Color)GlobalSettings.GetColor(1);
            // 
            // canvasPanel
            // 
            this.canvasPanel.AutoScroll = true;
            this.canvasPanel.AutoScrollMargin = new Size(4, 4);
            this.canvasPanel.BackColor = (Color)GlobalSettings.GetColor(0);
            this.canvasPanel.Controls.Add(this.canvas);
            this.canvasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasPanel.Location = new System.Drawing.Point(240, 0);
            this.canvasPanel.Margin = new System.Windows.Forms.Padding(0);
            this.canvasPanel.Name = "canvasPanel";
            this.canvasPanel.Padding = new System.Windows.Forms.Padding(4);
            this.canvasPanel.Size = new System.Drawing.Size(720, 540);
            this.canvasPanel.TabIndex = 1;
            // 
            // canvas
            // 
            this.canvas.Location = new System.Drawing.Point(4, 4);
            this.canvas.Margin = new System.Windows.Forms.Padding(0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(0, 0);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(960, 540);
            this.Controls.Add(this.canvasPanel);
            this.Controls.Add(this.mainPanel);
            this.KeyPreview = true;
            this.KeyDown += OnKeyDown;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Den Drawiiing Mini [DEBUG]";
            this.canvasPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
