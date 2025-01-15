using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using SdColor = System.Drawing.Color;
using ImageAlignmentLibrary;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Reflection;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;

namespace ImageAlignmentWFA
{
    public partial class Form1 : Form
    {
        private ImageProcessor? imageProcessor;
        private Image<Rgba32>? originalImage;
        private bool showContours = true;
        private bool isUpdating = false;
        private string? currentImagePath;
        private Image<Rgba32>? lowResImage;
        private const int MaxDimension = 1000;

        public Form1()
        {
            InitializeComponent();

            btnLoadImage.Click += BtnLoadImage_Click;
            btnReset.Click += BtnReset_Click;
            tbAngle.Scroll += TbAngle_Scroll;
            btnRotate90.Click += BtnRotate90_Click;
            btnRotateMinus90.Click += BtnRotateMinus90_Click;
            btnSaveImage.Click += BtnSaveImage_Click;
            btnAutoAlign.Click += BtnAutoAlign_Click;
            btnToggleContour.Click += BtnToggleContour_Click;
            btnSaveAs.Click += BtnSaveAs_Click;
            txtAngle.KeyPress += TxtAngle_KeyPress;
            this.Resize += Form1_Resize;

            pictureBox1.Paint += PictureBox1_Paint;
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                currentImagePath = ofd.FileName;
                lblImagePath.Text = $"���� � �����������: {currentImagePath}";
                lblImagePath.Refresh(); // ���������� ����������� �����

                imageProcessor?.Dispose();
                imageProcessor = new ImageProcessor();
                imageProcessor.LoadImage(currentImagePath);

                originalImage = imageProcessor.OriginalImage?.Clone();
                lowResImage = CreateLowResCopy(originalImage, MaxDimension);

                if (lowResImage != null)
                {
                    imageProcessor.ResetImage();
                    imageProcessor.AlignedImage?.Dispose();
                    imageProcessor.AlignedImage = lowResImage.Clone();
                }

                imageProcessor.ReDetectDominantRectangleOnAligned();
                UpdatePicture();
                ResetControls();
            }
        }

        private Image<Rgba32>? CreateLowResCopy(Image<Rgba32>? image, int maxDim)
        {
            if (image == null) return null;
            if (image.Width <= maxDim && image.Height <= maxDim) return image.Clone();
            double scale = Math.Min((double)maxDim / image.Width, (double)maxDim / image.Height);
            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);
            var clone = image.Clone(ctx => ctx.Resize(newWidth, newHeight));
            return clone;
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null || originalImage == null)
            {
                MessageBox.Show("������� ��������� �����������!");
                return;
            }
            imageProcessor.ResetImage();
            imageProcessor.ReDetectDominantRectangleOnAligned();

            isUpdating = true;
            tbAngle.Value = 0;
            txtAngle.Text = "0";
            isUpdating = false;

            UpdatePicture();
            MessageBox.Show("�������� � ���������.");
        }

        private void TbAngle_Scroll(object sender, EventArgs e)
        {
            if (imageProcessor == null || isUpdating) return;
            isUpdating = true;

            int angle = tbAngle.Value;
            txtAngle.Text = angle.ToString();

            imageProcessor.RotateImage(angle);
            imageProcessor.ReDetectDominantRectangleOnAligned();

            UpdatePicture();

            isUpdating = false;
        }

        private void BtnRotate90_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null) return;
            double newAngle = imageProcessor.CurrentAngle + 90;
            int angle = NormalizeAngle(newAngle);

            isUpdating = true;
            tbAngle.Value = angle;
            txtAngle.Text = angle.ToString();
            isUpdating = false;

            imageProcessor.RotateImage(angle);
            imageProcessor.ReDetectDominantRectangleOnAligned();

            UpdatePicture();
        }

        private void BtnRotateMinus90_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null) return;
            double newAngle = imageProcessor.CurrentAngle - 90;
            int angle = NormalizeAngle(newAngle);

            isUpdating = true;
            tbAngle.Value = angle;
            txtAngle.Text = angle.ToString();
            isUpdating = false;

            imageProcessor.RotateImage(angle);
            imageProcessor.ReDetectDominantRectangleOnAligned();

            UpdatePicture();
        }

        private int NormalizeAngle(double angle)
        {
            angle %= 360;
            if (angle > 180) angle -= 360;
            if (angle < -180) angle += 360;
            return (int)Math.Round(angle);
        }

        private void BtnSaveImage_Click(object sender, EventArgs e)
        {
            if (imageProcessor?.AlignedImage == null)
            {
                MessageBox.Show("��� ����������� ��� ����������!");
                return;
            }
            using var sfd = new SaveFileDialog
            {
                Filter = "JPEG|*.jpg;*.jpeg|PNG|*.png|Bitmap|*.bmp|TIFF|*.tif;*.tiff",
                Title = "��������� �����������",
                FileName = "AlignedImage"
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imageProcessor.SaveAlignedImage(sfd.FileName);
                    MessageBox.Show("���������!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("������: " + ex.Message);
                }
            }
        }

        private void BtnSaveAs_Click(object? sender, EventArgs e)
        {
            if (imageProcessor?.AlignedImage == null)
            {
                MessageBox.Show("��� ����������� ��� ����������.");
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "JPEG|*.jpg;*.jpeg|PNG|*.png|Bitmap|*.bmp|TIFF|*.tif;*.tiff",
                Title = "��������� ���",
                FileName = currentImagePath != null ? Path.GetFileName(currentImagePath) : "AlignedImage"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imageProcessor.SaveAlignedImage(sfd.FileName);
                    MessageBox.Show("���������!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("������: " + ex.Message);
                }
            }
        }

        private void BtnAutoAlign_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null)
            {
                MessageBox.Show("������� ��������� �����������!");
                return;
            }

            try
            {
                var stopwatch = Stopwatch.StartNew();
                imageProcessor.AutoAlignImage();

                if (chkAutoCrop.Checked)
                {
                    imageProcessor.AutoCrop();
                }

                stopwatch.Stop();

                lblOperationTime.Text = $"����� ����������: {stopwatch.ElapsedMilliseconds} ��";

                isUpdating = true;
                int newAngle = NormalizeAngle(imageProcessor.CurrentAngle);
                tbAngle.Value = newAngle;
                txtAngle.Text = newAngle.ToString("F2");
                isUpdating = false;

                UpdatePicture();
                MessageBox.Show("���������������� ���������.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("������ ��� ����������������: " + ex.Message);
            }
        }

        private void BtnToggleContour_Click(object sender, EventArgs e)
        {
            showContours = !showContours;
            btnToggleContour.Text = showContours ? "������ ������" : "�������� ������";
            pictureBox1.Invalidate();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (chkGuidelines.Checked) DrawGuidelines(e.Graphics);
            if (chkDiagonals.Checked) DrawDiagonals(e.Graphics);

            if (imageProcessor?.DetectedRectanglePoints != null && showContours)
            {
                var pts = imageProcessor.DetectedRectanglePoints;
                if (pts.Count == 4 && pictureBox1.Image != null)
                {
                    int imgW = pictureBox1.Image.Width;
                    int imgH = pictureBox1.Image.Height;
                    var pbRect = pictureBox1.ClientRectangle;

                    float ratioX = (float)pbRect.Width / imgW;
                    float ratioY = (float)pbRect.Height / imgH;
                    float ratio = Math.Min(ratioX, ratioY);

                    float offsetX = (pbRect.Width - (imgW * ratio)) / 2;
                    float offsetY = (pbRect.Height - (imgH * ratio)) / 2;

                    using var penRect = new Pen(SdColor.Blue, 2);
                    float extension = 10.0f;

                    for (int i = 0; i < 4; i++)
                    {
                        int j = (i + 1) % 4;
                        var pA = pts[i];
                        var pB = pts[j];

                        float pxA = pA.X * ratio + offsetX;
                        float pyA = pA.Y * ratio + offsetY;
                        float pxB = pB.X * ratio + offsetX;
                        float pyB = pB.Y * ratio + offsetY;

                        float dx = pxB - pxA;
                        float dy = pyB - pyA;
                        float length = (float)Math.Sqrt(dx * dx + dy * dy);
                        if (length == 0) continue;

                        float ux = dx / length;
                        float uy = dy / length;

                        float extendedPxA = pxA - ux * extension;
                        float extendedPyA = pyA - uy * extension;
                        float extendedPxB = pxB + ux * extension;
                        float extendedPyB = pyB + uy * extension;

                        e.Graphics.DrawLine(penRect, extendedPxA, extendedPyA, extendedPxB, extendedPyB);
                    }
                }
            }
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (double.TryParse(txtAngle.Text, out double val))
                {
                    if (val < tbAngle.Minimum) val = tbAngle.Minimum;
                    if (val > tbAngle.Maximum) val = tbAngle.Maximum;

                    isUpdating = true;
                    int intVal = NormalizeAngle(val);
                    tbAngle.Value = intVal;
                    txtAngle.Text = val.ToString("F2");
                    isUpdating = false;

                    imageProcessor?.RotateImage(val);
                    imageProcessor?.ReDetectDominantRectangleOnAligned();

                    UpdatePicture();
                }
                else
                {
                    MessageBox.Show("������������ ����!");
                }
                e.Handled = true;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width = this.ClientSize.Width - pictureBox1.Left - 10;
            pictureBox1.Height = this.ClientSize.Height - pictureBox1.Top - 10;
        }

        private void ResetControls()
        {
            isUpdating = true;
            tbAngle.Value = 0;
            txtAngle.Text = "0";
            isUpdating = false;
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

            pictureBox1.Invalidate();
        }
    }
}
