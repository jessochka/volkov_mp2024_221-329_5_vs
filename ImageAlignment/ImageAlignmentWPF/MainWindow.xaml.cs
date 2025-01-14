using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageAlignmentLibrary;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media;
using System.Drawing;

namespace ImageAlignmentWPF
{
    public partial class MainWindow : Window
    {
        private ImageProcessor? imageProcessor;
        private Image<Rgba32>? originalImage;

        private bool showContours = true;
        private bool isUpdating = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnToggleContour_Click(object sender, RoutedEventArgs e)
        {
            showContours = !showContours;
            btnToggleContour.Content = showContours ? "Скрыть контур" : "Показать контур";
            DrawDetectedRectangle();
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files|*.jpg;*.png;*.bmp" };
            if (ofd.ShowDialog() == true)
            {
                HideAllContours();
                imageProcessor?.Dispose();
                imageProcessor = new ImageProcessor();
                imageProcessor.LoadImage(ofd.FileName);

                originalImage = imageProcessor.OriginalImage?.Clone();

                imageProcessor.ReDetectDominantRectangleOnAligned();
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
            imageProcessor.ReDetectDominantRectangleOnAligned();

            isUpdating = true;
            sliderAngle.Value = 0;
            txtAngle.Text = "0";
            isUpdating = false;

            UpdateImageDisplay();
            MessageBox.Show("Сброшено к исходному.");
        }

        private void sliderAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (imageProcessor == null) return;
            if (isUpdating) return;

            isUpdating = true;
            int angle = (int)Math.Round(e.NewValue);
            txtAngle.Text = angle.ToString();

            imageProcessor.RotateImage(angle);
            imageProcessor.ReDetectDominantRectangleOnAligned();
            UpdateImageDisplay();

            isUpdating = false;
        }

        private void txtAngle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(txtAngle.Text, out int val))
                {
                    if (val < (int)sliderAngle.Minimum) val = (int)sliderAngle.Minimum;
                    if (val > (int)sliderAngle.Maximum) val = (int)sliderAngle.Maximum;

                    if (imageProcessor != null)
                    {
                        isUpdating = true;
                        val = NormalizeAngle(val);
                        sliderAngle.Value = val;
                        txtAngle.Text = val.ToString();
                        isUpdating = false;

                        imageProcessor.RotateImage(val);
                        imageProcessor.ReDetectDominantRectangleOnAligned();
                        UpdateImageDisplay();
                    }
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
            double newAngle = imageProcessor.CurrentAngle + 90;
            int angle = NormalizeAngle(newAngle);

            isUpdating = true;
            sliderAngle.Value = angle;
            txtAngle.Text = angle.ToString();
            isUpdating = false;

            imageProcessor.RotateImage(angle);
            imageProcessor.ReDetectDominantRectangleOnAligned();
            UpdateImageDisplay();
        }

        private void btnRotateMinus90_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor == null) return;
            double newAngle = imageProcessor.CurrentAngle - 90;
            int angle = NormalizeAngle(newAngle);

            isUpdating = true;
            sliderAngle.Value = angle;
            txtAngle.Text = angle.ToString();
            isUpdating = false;

            imageProcessor.RotateImage(angle);
            imageProcessor.ReDetectDominantRectangleOnAligned();
            UpdateImageDisplay();
        }

        private int NormalizeAngle(double angle)
        {
            angle %= 360;
            if (angle > 180) angle -= 360;
            if (angle < -180) angle += 360;
            return (int)Math.Round(angle);
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

        private void btnAutoAlign_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor == null)
            {
                MessageBox.Show("Сначала загрузите изображение!");
                return;
            }

            try
            {
                imageProcessor.AutoAlignImage();

                isUpdating = true;
                int newAngle = (int)Math.Round(imageProcessor.CurrentAngle);
                newAngle = NormalizeAngle(newAngle);
                sliderAngle.Value = newAngle;
                txtAngle.Text = newAngle.ToString();
                isUpdating = false;

                UpdateImageDisplay();
                MessageBox.Show("Автовыравнивание выполнено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при автовыравнивании: " + ex.Message);
            }
        }

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

            DrawDetectedRectangle(); // отрисовываем контур
            if (chkGuidelines.IsChecked == true || chkDiagonals.IsChecked == true)
            {
                UpdateGuidelines();
            }
        }

        private void ResetControls()
        {
            isUpdating = true;
            sliderAngle.Value = 0;
            txtAngle.Text = "0";
            isUpdating = false;
            chkGuidelines.IsChecked = false;
            chkDiagonals.IsChecked = false;
            HideAllContours();
        }

        private void UpdateGuidelines()
        {
            if (imgDisplay.Source == null) return;

            double w = imgDisplay.ActualWidth;
            double h = imgDisplay.ActualHeight;
            double cx = w / 2;
            double cy = h / 2;

            lineHorizontal.X1 = cx; lineHorizontal.Y1 = 0;
            lineHorizontal.X2 = cx; lineHorizontal.Y2 = h;

            lineVertical.X1 = 0; lineVertical.Y1 = cy;
            lineVertical.X2 = w; lineVertical.Y2 = cy;

            lineDiagonal1.X1 = 0; lineDiagonal1.Y1 = 0;
            lineDiagonal1.X2 = w; lineDiagonal1.Y2 = h;

            lineDiagonal2.X1 = w; lineDiagonal2.Y1 = 0;
            lineDiagonal2.X2 = 0; lineDiagonal2.Y2 = h;
        }

        private void DrawDetectedRectangle()
        {
            if (!showContours)
            {
                HideAllContours();
                return;
            }

            if (imageProcessor?.DetectedRectanglePoints == null || imageProcessor.DetectedRectanglePoints.Count != 4)
            {
                HideAllContours();
                return;
            }

            double imgWidth = imageProcessor.AlignedImage.Width;
            double imgHeight = imageProcessor.AlignedImage.Height;
            double displayWidth = imgDisplay.ActualWidth;
            double displayHeight = imgDisplay.ActualHeight;
            if (displayWidth == 0 || displayHeight == 0) return;

            double ratioX = displayWidth / imgWidth;
            double ratioY = displayHeight / imgHeight;
            double ratio = Math.Min(ratioX, ratioY);

            double offsetX = (displayWidth - (imgWidth * ratio)) / 2;
            double offsetY = (displayHeight - (imgHeight * ratio)) / 2;

            var points = new List<System.Windows.Point>();
            foreach (var pt in imageProcessor.DetectedRectanglePoints)
            {
                double x = pt.X * ratio + offsetX;
                double y = pt.Y * ratio + offsetY;
                points.Add(new System.Windows.Point(x, y));
            }

            double extension = 10.0;
            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;
                var p1 = points[i];
                var p2 = points[next];

                double dx = p2.X - p1.X;
                double dy = p2.Y - p1.Y;
                double length = Math.Sqrt(dx * dx + dy * dy);
                if (length == 0) continue;

                double ux = dx / length;
                double uy = dy / length;

                var extendedP1 = new System.Windows.Point(p1.X - ux * extension, p1.Y - uy * extension);
                var extendedP2 = new System.Windows.Point(p2.X + ux * extension, p2.Y + uy * extension);

                switch (i)
                {
                    case 0:
                        lineTop.X1 = extendedP1.X;
                        lineTop.Y1 = extendedP1.Y;
                        lineTop.X2 = extendedP2.X;
                        lineTop.Y2 = extendedP2.Y;
                        lineTop.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        lineRight.X1 = extendedP1.X;
                        lineRight.Y1 = extendedP1.Y;
                        lineRight.X2 = extendedP2.X;
                        lineRight.Y2 = extendedP2.Y;
                        lineRight.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        lineBottom.X1 = extendedP1.X;
                        lineBottom.Y1 = extendedP1.Y;
                        lineBottom.X2 = extendedP2.X;
                        lineBottom.Y2 = extendedP2.Y;
                        lineBottom.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        lineLeft.X1 = extendedP1.X;
                        lineLeft.Y1 = extendedP1.Y;
                        lineLeft.X2 = extendedP2.X;
                        lineLeft.Y2 = extendedP2.Y;
                        lineLeft.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void HideAllContours()
        {
            lineTop.Visibility = Visibility.Collapsed;
            lineRight.Visibility = Visibility.Collapsed;
            lineBottom.Visibility = Visibility.Collapsed;
            lineLeft.Visibility = Visibility.Collapsed;
        }

        private void canvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void chkGuidelines_Checked(object sender, RoutedEventArgs e)
        {
            lineHorizontal.Visibility = Visibility.Visible;
            lineVertical.Visibility = Visibility.Visible;
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
        }

        private void chkDiagonals_Unchecked(object sender, RoutedEventArgs e)
        {
            lineDiagonal1.Visibility = Visibility.Collapsed;
            lineDiagonal2.Visibility = Visibility.Collapsed;
        }
    }
}
