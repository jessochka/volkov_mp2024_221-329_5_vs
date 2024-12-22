using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageAlignmentLibrary;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageAlignmentWPF
{
    public partial class MainWindow : Window
    {
        private ImageProcessor? imageProcessor;
        private Image<Rgba32>? originalImage;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.png;*.bmp"
            };
            if (ofd.ShowDialog() == true)
            {
                imageProcessor?.Dispose();
                imageProcessor = new ImageProcessor();
                imageProcessor.LoadImage(ofd.FileName);
                originalImage = imageProcessor.OriginalImage?.Clone();
                UpdateImageDisplay();
                ResetControls();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor == null || originalImage == null)
            {
                MessageBox.Show("Сначала загрузите изображение!");
                return;
            }
            imageProcessor.ResetImage();
            sliderAngle.Value = 0;
            txtAngle.Text = "0";
            UpdateImageDisplay();
            MessageBox.Show("Сброшено к исходному.");
        }

        private void sliderAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (imageProcessor == null) return;
            int angle = (int)e.NewValue;
            txtAngle.Text = angle.ToString();
            imageProcessor.RotateImage(angle);
            UpdateImageDisplay();
        }

        private void txtAngle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(txtAngle.Text, out int a))
                {
                    if (a < sliderAngle.Minimum) a = (int)sliderAngle.Minimum;
                    if (a > sliderAngle.Maximum) a = (int)sliderAngle.Maximum;
                    sliderAngle.Value = a;
                    imageProcessor?.RotateImage(a);
                    UpdateImageDisplay();
                }
                else
                {
                    MessageBox.Show("Некорректный угол!");
                }
            }
        }

        private void btnRotate90_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor == null) return;
            int angle = (int)(sliderAngle.Value + 90);
            if (angle > 180) angle -= 360;
            sliderAngle.Value = angle;
        }

        private void btnRotateMinus90_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor == null) return;
            int angle = (int)(sliderAngle.Value - 90);
            if (angle < -180) angle += 360;
            sliderAngle.Value = angle;
        }

        private void chkGuidelines_Checked(object sender, RoutedEventArgs e)
        {
            lineHorizontal.Visibility = Visibility.Visible;
            lineVertical.Visibility = Visibility.Visible;
            UpdateGuidelines();
        }
        private void chkGuidelines_Unchecked(object sender, RoutedEventArgs e)
        {
            lineHorizontal.Visibility = Visibility.Collapsed;
            lineVertical.Visibility = Visibility.Collapsed;
        }
        private void chkDiagonals_Checked(object sender, RoutedEventArgs e)
        {
            lineDiagonal1.Visibility = Visibility.Visible;
            lineDiagonal2.Visibility = Visibility.Visible;
            UpdateGuidelines();
        }
        private void chkDiagonals_Unchecked(object sender, RoutedEventArgs e)
        {
            lineDiagonal1.Visibility = Visibility.Collapsed;
            lineDiagonal2.Visibility = Visibility.Collapsed;
        }

        private void UpdateGuidelines()
        {
            if (imgDisplay.Source == null) return;

            double w = imgDisplay.ActualWidth;
            double h = imgDisplay.ActualHeight;

            double centerX = w / 2;
            double centerY = h / 2;

            // Горизонтальная
            lineHorizontal.X1 = centerX;
            lineHorizontal.Y1 = 0;
            lineHorizontal.X2 = centerX;
            lineHorizontal.Y2 = h;
            // Вертикальная
            lineVertical.X1 = 0;
            lineVertical.Y1 = centerY;
            lineVertical.X2 = w;
            lineVertical.Y2 = centerY;
            // Диагонали
            lineDiagonal1.X1 = 0;
            lineDiagonal1.Y1 = 0;
            lineDiagonal1.X2 = w;
            lineDiagonal1.Y2 = h;

            lineDiagonal2.X1 = w;
            lineDiagonal2.Y1 = 0;
            lineDiagonal2.X2 = 0;
            lineDiagonal2.Y2 = h;
        }

        private void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor?.AlignedImage == null)
            {
                MessageBox.Show("Нет изображения для сохранения.");
                return;
            }
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JPEG|*.jpg|PNG|*.png|Bitmap|*.bmp",
                Title = "Сохранить",
                FileName = "AlignedImage"
            };
            if (sfd.ShowDialog() == true)
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

        private void canvasImage_MouseDown(object sender, MouseButtonEventArgs e) { }

        private void UpdateImageDisplay()
        {
            if (imageProcessor?.AlignedImage == null) return;
            using var ms = new MemoryStream();
            imageProcessor.AlignedImage.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);

            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = ms;
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();
            imgDisplay.Source = bmp;

            if (chkGuidelines.IsChecked == true || chkDiagonals.IsChecked == true)
            {
                UpdateGuidelines();
            }
        }

        private void ResetControls()
        {
            sliderAngle.Value = 0;
            txtAngle.Text = "0";
            chkGuidelines.IsChecked = false;
            chkDiagonals.IsChecked = false;
        }
    }
}
