using DenDrawiiing.Tools;
using DenDrawiiing.Utils;
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
        private Bitmap mask = new Bitmap(640, 480);
        private Graphics maskGraphics;
        private Bitmap rawBuffer = new Bitmap(640, 480);
        private Graphics rawBufferGraphics;

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
        public Color Color2
        {
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
            maskGraphics = Graphics.FromImage(mask);
            maskGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            rawBufferGraphics = Graphics.FromImage(rawBuffer);
            rawBufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            maskGraphics.Clear(Color.White);

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
            if (showSelection.Checked && SelectionTool.Selected)
                e.Graphics.DrawImage(ImageMask.GetMaskVisual(mask, Color.FromArgb(140, Color.LightBlue)), 0, 0);
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
                if (previewSelection.Checked && !(Tool.Current is SelectionTool))
                {
                    rawBufferGraphics.Clear(Color.Transparent);
                    rawBufferGraphics.DrawImage(bmp, 0, 0);
                    Tool.Current.DrawEnd(e.Location, rawBufferGraphics, Color, rawBuffer);
                    ImageMask.Mask(rawBuffer, mask, buffer);
                }
                else
                {
                    Tool.Current.DrawEnd(e.Location, bufferGraphics, Color, buffer);
                }
                if (!(Tool.Current is SelectionTool))
                {
                    History.Action((Bitmap)bmp.Clone());
                    rawBufferGraphics.Clear(Color.Transparent);
                    rawBufferGraphics.DrawImage(bmp, 0, 0);
                    Tool.Current.Serialize(rawBufferGraphics, Color, rawBuffer);
                    ImageMask.Mask(rawBuffer, mask, bmp);
                    BufferDrawStart();
                }
                else
                {
                    Tool.Current.Serialize(maskGraphics, Color.White, mask);
                    if (!SelectionTool.Selected)
                        maskGraphics.Clear(Color.White);
                }
                BufferDrawEnd();
            }
        }

        private void PMouseMove(object sender, MouseEventArgs e)
        {
            BufferDrawStart();
            if (previewSelection.Checked && !(Tool.Current is SelectionTool))
            {
                rawBufferGraphics.Clear(Color.Transparent);
                rawBufferGraphics.DrawImage(bmp, 0, 0);
                Tool.Current.MouseMove(e.Location, rawBufferGraphics, Color, rawBuffer, mPressed);
                ImageMask.Mask(rawBuffer, mask, buffer);
            }
            else
            {
                Tool.Current.MouseMove(e.Location, bufferGraphics, Color, buffer, mPressed);
            }
            BufferDrawEnd();
        }

        private void PMouseDown(object sender, MouseEventArgs e)
        {
            if (mPressed == false)
            {
                mPressed = true;
                BufferDrawStart();
                if (previewSelection.Checked && !(Tool.Current is SelectionTool))
                {
                    rawBufferGraphics.Clear(Color.Transparent);
                    rawBufferGraphics.DrawImage(bmp, 0, 0);
                    Tool.Current.DrawStart(e.Location, rawBufferGraphics, Color, rawBuffer);
                    ImageMask.Mask(rawBuffer, mask, buffer);
                }
                else
                {
                    Tool.Current.DrawStart(e.Location, bufferGraphics, Color, buffer);
                }
                if (Tool.Current is SelectionTool)
                    maskGraphics.Clear(Color.Black);
                BufferDrawEnd();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && !e.Alt)
            {
                if (e.KeyCode == Keys.Z)
                {
                    if (History.redoHistory.Count >= 1)
                        History.Redo(bmpGraphics, (Bitmap)bmp.Clone());
                    BufferDrawStart();
                    BufferDrawEnd();
                }
            }
            else if (e.Control && !e.Shift && !e.Alt)
            {
                if (e.KeyCode == Keys.Z)
                {
                    if (History.undoHistory.Count >= 1)
                        History.Undo(bmpGraphics, (Bitmap)bmp.Clone());
                    BufferDrawStart();
                    BufferDrawEnd();
                }
                else if (e.KeyCode == Keys.P)
                {
                    ShowColorPicker();
                }
                else if (e.KeyCode == Keys.I)
                {
                    if (SelectionTool.Selected)
                    {
                        Filters.Invert(mask, Color);
                        BufferDrawStart();
                        BufferDrawEnd();
                    }
                }
                else if (e.KeyCode == Keys.O)
                {
                    OpenFileDialog ofd = new OpenFileDialog
                    {
                        AddExtension = true,
                        AutoUpgradeEnabled = true,
                        CheckFileExists = true,
                        CheckPathExists = true,
                        DereferenceLinks = true,
                        Filter = "Усі підтримувані формати (*.png, *.jpg, *.jpeg, *.bmp, *.tiff, *.tif)|*.png;*.jpg;*.jpeg;*.bmp;*.tiff;*.tif" +
                        "|Portable Network Graphics (*.png)|*.png|Зображення JPEG (*.jpg, *.jpeg)|*.jpg;*.jpeg|Точковий рисунок (*.bmp)|*.bmp|Зображення TIFF (*.tiff, *.tif)|*.tiff;*.tif",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                        Multiselect = false,
                        Title = "Відкрити...",
                        ValidateNames = true
                    };

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            Bitmap b = new Bitmap(ofd.FileName);
                            bmp.Dispose();
                            bmp = b.Clone(new System.Drawing.Rectangle(0, 0, b.Width, b.Height),
                                PixelFormat.Format32bppArgb);
                            b.Dispose();

                            buffer.Dispose();
                            mask.Dispose();
                            rawBuffer.Dispose();

                            buffer = new Bitmap(bmp.Width, bmp.Height);
                            mask = new Bitmap(bmp.Width, bmp.Height);
                            rawBuffer = new Bitmap(bmp.Width, bmp.Height);

                            bmpGraphics.Dispose();
                            bufferGraphics.Dispose();
                            maskGraphics.Dispose();
                            rawBufferGraphics.Dispose();

                            bmpGraphics = Graphics.FromImage(bmp);
                            bmpGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                            bufferGraphics = Graphics.FromImage(buffer);
                            bufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                            maskGraphics = Graphics.FromImage(mask);
                            maskGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                            rawBufferGraphics = Graphics.FromImage(rawBuffer);
                            rawBufferGraphics.SmoothingMode = SmoothingMode.AntiAlias;

                            maskGraphics.Clear(Color.White);
                            SelectionTool.Selected = false;

                            canvas.Size = bmp.Size;

                            BufferDrawStart();
                            BufferDrawEnd();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Помилка відкриття файлу: " + ex.Message, "Помилка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (e.KeyCode == Keys.S)
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
            this.ForeColor = (Color)GlobalSettings.GetColor(4);
            this.canvasPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
