using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.ImageSharp;
using System.Windows.Forms;
using ImageAlignmentLibrary;

namespace ImageAlignmentWFA
{
    public partial class Form1 : Form
    {
        private ImageProcessor imageProcessor;

        public Form1()
        {
            InitializeComponent();
            btnLoadImage.Click += BtnLoadImage_Click;
            btnRotate90.Click += BtnRotate90_Click;
            btnRotateMinus90.Click += BtnRotateMinus90_Click;
        }

        private void BtnLoadImage_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.webp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                imageProcessor = new ImageProcessor();
                imageProcessor.LoadImage(openFileDialog.FileName);

                // Преобразуем SixLabors.ImageSharp.Image в Bitmap для отображения
                pictureBox1.Image = ImageToBitmap(imageProcessor.AlignedImage);
            }
        }

        private void BtnRotate90_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null)
            {
                MessageBox.Show("Сначала загрузите изображение.");
                return;
            }

            imageProcessor.RotateImage(90);
            pictureBox1.Image = ImageToBitmap(imageProcessor.AlignedImage);
        }

        private void BtnRotateMinus90_Click(object sender, EventArgs e)
        {
            if (imageProcessor == null)
            {
                MessageBox.Show("Сначала загрузите изображение.");
                return;
            }

            imageProcessor.RotateImage(-90);
            pictureBox1.Image = ImageToBitmap(imageProcessor.AlignedImage);
        }

        private Bitmap ImageToBitmap(Image<Rgba32> image)
        {
            using var memoryStream = new MemoryStream();
            image.SaveAsBmp(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new Bitmap(memoryStream);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
