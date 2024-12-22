using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using SdColor = System.Drawing.Color;
using ImageAlignmentLibrary;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageAlignmentWFA
{
    public partial class Form1 : Form
    {
        private ImageProcessor? imageProcessor;
        private Image<Rgba32>? originalImage;

        public Form1()
        {
            InitializeComponent();

            btnLoadImage.Click += BtnLoadImage_Click;
            btnReset.Click += BtnReset_Click;
            tbAngle.Scroll += TbAngle_Scroll;
            btnRotate90.Click += BtnRotate90_Click;
            btnRotateMinus90.Click += BtnRotateMinus90_Click;
            chkGuidelines.CheckedChanged += (s, e) => pictureBox1.Invalidate();
            chkDiagonals.CheckedChanged += (s, e) => pictureBox1.Invalidate();
            pictureBox1.Paint += PictureBox1_Paint;
            txtAngle.KeyPress += TxtAngle_KeyPress;
            btnSaveImage.Click += BtnSaveImage_Click;
            this.Resize += Form1_Resize;
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.png;*.bmp" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                imageProcessor?.Dispose();
                imageProcessor = new ImageProcessor();
                imageProcessor.LoadImage(ofd.FileName);
                originalImage = imageProcessor.OriginalImage?.Clone();
                UpdatePicture();
                ResetControls();
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null || originalImage == null)
            {
                MessageBox.Show("Сначала загрузите изображение!");
                return;
            }
            imageProcessor.ResetImage();
            tbAngle.Value = 0;
            txtAngle.Text = "0";
            UpdatePicture();
            MessageBox.Show("Сброшено к исходному.");
        }

        private void TbAngle_Scroll(object sender, EventArgs e)
        {
            if (imageProcessor == null) return;
            int angle = tbAngle.Value;
            txtAngle.Text = angle.ToString();
            imageProcessor.RotateImage(angle);
            UpdatePicture();
        }

        private void BtnRotate90_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null) return;
            int angle = tbAngle.Value + 90;
            if (angle > 180) angle -= 360;
            tbAngle.Value = angle;
            txtAngle.Text = angle.ToString();
            imageProcessor.RotateImage(angle);
            UpdatePicture();
        }

        private void BtnRotateMinus90_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null) return;
            int angle = tbAngle.Value - 90;
            if (angle < -180) angle += 360;
            tbAngle.Value = angle;
            txtAngle.Text = angle.ToString();
            imageProcessor.RotateImage(angle);
            UpdatePicture();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (chkGuidelines.Checked) DrawGuidelines(e.Graphics);
            if (chkDiagonals.Checked) DrawDiagonals(e.Graphics);
        }

        private void DrawGuidelines(Graphics g)
        {
            using var pen = new Pen(SdColor.Red, 1);
            int cx = pictureBox1.Width / 2;
            int cy = pictureBox1.Height / 2;
            g.DrawLine(pen, cx, 0, cx, pictureBox1.Height);
            g.DrawLine(pen, 0, cy, pictureBox1.Width, cy);
        }

        private void DrawDiagonals(Graphics g)
        {
            using var pen = new Pen(SdColor.Blue, 1);
            g.DrawLine(pen, 0, 0, pictureBox1.Width, pictureBox1.Height);
            g.DrawLine(pen, pictureBox1.Width, 0, 0, pictureBox1.Height);
        }

        private void TxtAngle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)
               && e.KeyChar != '-' && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (int.TryParse(txtAngle.Text, out int val))
                {
                    if (val < tbAngle.Minimum) val = tbAngle.Minimum;
                    if (val > tbAngle.Maximum) val = tbAngle.Maximum;
                    tbAngle.Value = val;
                    imageProcessor?.RotateImage(val);
                    UpdatePicture();
                }
                else
                {
                    MessageBox.Show("Некорректный угол!");
                }
                e.Handled = true;
            }
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            if (imageProcessor?.AlignedImage == null)
            {
                MessageBox.Show("Нет изображения для сохранения!");
                return;
            }
            using var sfd = new SaveFileDialog
            {
                Filter = "JPEG|*.jpg|PNG|*.png|Bitmap|*.bmp",
                Title = "Сохранить изображение",
                FileName = "AlignedImage"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imageProcessor.SaveAlignedImage(sfd.FileName);
                    MessageBox.Show("Сохранено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width = this.ClientSize.Width - pictureBox1.Left - 10;
            pictureBox1.Height = this.ClientSize.Height - pictureBox1.Top - 10;
        }

        private void ResetControls()
        {
            tbAngle.Value = 0;
            txtAngle.Text = "0";
            chkGuidelines.Checked = false;
            chkDiagonals.Checked = false;
        }

        private void UpdatePicture()
        {
            if (imageProcessor?.AlignedImage == null) return;
            pictureBox1.Image?.Dispose();
            using var ms = new MemoryStream();
            imageProcessor.AlignedImage.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            pictureBox1.Image = new Bitmap(ms);
        }
    }
}
