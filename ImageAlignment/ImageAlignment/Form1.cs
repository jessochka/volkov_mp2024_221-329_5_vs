using System;
using System.Drawing;
using System.Windows.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ImageAlignment.ibrary;

namespace ImageAlignment
{
    public partial class Form1 : Form
    {
        private ImageProcessor imageProcessor;

        public Form1()
        {
            InitializeComponent();
            btnRotate90.Click += btnRotate90_Click;
            btnRotateMinus90.Click += btnRotateMinus90_Click;
            tbAngle.Scroll += tbAngle_Scroll;
            btnLoadImage.Click += BtnLoadImage_Click;
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.webp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // ��������� ����������� � ������� ImageSharp
                var loadedImage = Image.Load<Rgba32>(openFileDialog.FileName);

                // ����������� ImageSharp Image � Bitmap
                Bitmap bitmap = ImageToBitmap(loadedImage);

                // ������������� ����������� � PictureBox
                pictureBox1.Image = bitmap;

                // ������� � �������������� ImageProcessor
                imageProcessor = new ImageProcessor(loadedImage);
            }
        }

        private void btnRotate90_Click(object sender, EventArgs e)
        {
            if (imageProcessor != null)
            {
                pictureBox1.Image = imageProcessor.RotateImage(imageProcessor.OriginalImage, 90);
            }
        }

        private void btnRotateMinus90_Click(object sender, EventArgs e)
        {
            if (imageProcessor != null)
            {
                pictureBox1.Image = imageProcessor.RotateImage(imageProcessor.OriginalImage, -90);
            }
        }

        private void tbAngle_Scroll(object sender, EventArgs e)
        {
            if (imageProcessor != null)
            {
                float angle = tbAngle.Value;
                txtAngle.Text = angle.ToString();
                pictureBox1.Image = imageProcessor.RotateImage(imageProcessor.OriginalImage, angle);
            }
        }

        // ����� ��� �������������� ImageSharp Image � Bitmap
        private Bitmap ImageToBitmap(Image<Rgba32> image)
        {
            var memoryStream = new System.IO.MemoryStream();
            image.SaveAsBmp(memoryStream); // ��������� � ����� � ������� BMP
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            return new Bitmap(memoryStream); // ��������� �� ������ ��� Bitmap
        }
    }
}
