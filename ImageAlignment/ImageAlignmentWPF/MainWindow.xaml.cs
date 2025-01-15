using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ImageAlignmentLibrary;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Drawing;
using MediaColor = System.Windows.Media.Color;
using MediaBrushes = System.Windows.Media.Brushes;
using SixLabors.ImageSharp.Processing;

namespace ImageAlignmentWPF
{
    public partial class MainWindow : Window
    {
        private ImageProcessor? imageProcessor;
        private Image<Rgba32>? originalImage;
        private bool showContours = true;
        private bool isUpdating = false;
        private bool isDarkTheme = false; // Флаг текущей темы
        private string? currentImagePath;
        private Image<Rgba32>? lowResImage; // Копия для обработки
        private const int MaxDimension = 1000;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Переключение темы
        private void btnToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            if (isDarkTheme)
            {
                // Темная тема
                Resources["WindowBackgroundBrush"] = new SolidColorBrush(MediaColor.FromRgb(40, 40, 40));
                Resources["WindowForegroundBrush"] = new SolidColorBrush(MediaBrushes.White.Color);
                Resources["PanelBackgroundBrush"] = new SolidColorBrush(MediaColor.FromRgb(60, 60, 60));
                Resources["PanelForegroundBrush"] = new SolidColorBrush(MediaBrushes.White.Color);

                btnToggleTheme.Content = "Светлая тема";
            }
            else
            {
                // Светлая тема
                Resources["WindowBackgroundBrush"] = new SolidColorBrush(MediaBrushes.White.Color);
                Resources["WindowForegroundBrush"] = new SolidColorBrush(MediaBrushes.Black.Color);
                Resources["PanelBackgroundBrush"] = new SolidColorBrush(MediaColor.FromRgb(238, 238, 238));
                Resources["PanelForegroundBrush"] = new SolidColorBrush(MediaBrushes.Black.Color);

                btnToggleTheme.Content = "Тёмная тема";
            }
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff"
            };
            if (ofd.ShowDialog() == true)
            {
                currentImagePath = ofd.FileName;
                txtImagePath.Text = $"Путь к изображению: {currentImagePath}";

                HideAllContours();
                imageProcessor?.Dispose();
                imageProcessor = new ImageProcessor();
                imageProcessor.LoadImage(currentImagePath);

                // Создание уменьшенной копии
                originalImage = imageProcessor.OriginalImage?.Clone();
                lowResImage = CreateLowResCopy(originalImage, MaxDimension);

                // Использование уменьшенной копии для обработки
                if (lowResImage != null)
                {
                    imageProcessor.ResetImage();
                    // Замена оригинала на уменьшенную копию для отображения и выравнивания
                    imageProcessor.AlignedImage?.Dispose();
                    imageProcessor.AlignedImage = lowResImage.Clone();
                }

                imageProcessor.ReDetectDominantRectangleOnAligned();
                UpdateImageDisplay();
                ResetControls();
            }
        }

        // Метод создания уменьшенной копии
        private Image<Rgba32>? CreateLowResCopy(Image<Rgba32>? image, int maxDim)
        {
            if (image == null) return null;
            if (image.Width <= maxDim && image.Height <= maxDim) return image.Clone();
            double scale = Math.Min((double)maxDim / image.Width, (double)maxDim / image.Height);
            int newWidth = (int)(image.Width * scale);
            int newHeight = (int)(image.Height * scale);
            var clone = image.Clone((IImageProcessingContext ctx) => ctx.Resize(newWidth, newHeight));
            return clone;
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
            double angle = e.NewValue;
            txtAngle.Text = angle.ToString("F2"); // отображаем с 2 знаками после запятой

            imageProcessor.RotateImage(angle);
            imageProcessor.ReDetectDominantRectangleOnAligned();
            UpdateImageDisplay();

            isUpdating = false;
        }

        private void txtAngle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (double.TryParse(txtAngle.Text, out double val))
                {
                    if (val < sliderAngle.Minimum) val = sliderAngle.Minimum;
                    if (val > sliderAngle.Maximum) val = sliderAngle.Maximum;

                    if (imageProcessor != null)
                    {
                        isUpdating = true;
                        val = NormalizeAngle(val);
                        sliderAngle.Value = val;
                        txtAngle.Text = val.ToString("F2");
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

            if (File.Exists(currentImagePath))
            {
                var result = MessageBox.Show("Файл будет перезаписан. Продолжить?",
                                             "Подтверждение", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes) return;
                try
                {
                    imageProcessor.SaveAlignedImage(currentImagePath);
                    MessageBox.Show("Сохранено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
            else
            {
                // Если файла нет
                var sfd = new SaveFileDialog
                {
                    Filter = "JPEG|*.jpg;*.jpeg|PNG|*.png|Bitmap|*.bmp|TIFF|*.tif;*.tiff",
                    Title = "Сохранить",
                    FileName = currentImagePath != null ? System.IO.Path.GetFileName(currentImagePath) : "AlignedImage"
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
        }

        private void btnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor?.AlignedImage == null)
            {
                MessageBox.Show("Нет изображения для сохранения.");
                return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "JPEG|*.jpg;*.jpeg|PNG|*.png|Bitmap|*.bmp|TIFF|*.tif;*.tiff",
                Title = "Сохранить как",
                FileName = currentImagePath != null ? System.IO.Path.GetFileName(currentImagePath) : "AlignedImage"
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

        private async void btnAutoAlign_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor == null)
            {
                MessageBox.Show("Сначала загрузите изображение!");
                return;
            }

            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                imageProcessor.AutoAlignImage();

                if (chkAutoCrop.IsChecked == true)
                {
                    imageProcessor.AutoCrop();
                }

                stopwatch.Stop();

                txtOperationTime.Text = $"Время выполнения: {stopwatch.ElapsedMilliseconds} мс";

                isUpdating = true;
                double newAngle = imageProcessor.CurrentAngle;
                newAngle = NormalizeAngle(newAngle);
                sliderAngle.Value = newAngle;
                txtAngle.Text = newAngle.ToString("F2");
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

            canvasImage.UpdateLayout();

            DrawDetectedRectangle();
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

            var ptsWpf = new List<System.Windows.Point>();
            foreach (var pt in imageProcessor.DetectedRectanglePoints)
            {
                double x = pt.X * ratio + offsetX;
                double y = pt.Y * ratio + offsetY;
                ptsWpf.Add(new System.Windows.Point(x, y));
            }

            double extension = 10.0;
            for (int i = 0; i < 4; i++)
            {
                int j = (i + 1) % 4;
                var pA = ptsWpf[i];
                var pB = ptsWpf[j];

                double dx = pB.X - pA.X;
                double dy = pB.Y - pA.Y;
                double length = Math.Sqrt(dx * dx + dy * dy);
                if (length == 0) continue;

                double ux = dx / length;
                double uy = dy / length;

                var pAext = new System.Windows.Point(pA.X - ux * extension, pA.Y - uy * extension);
                var pBext = new System.Windows.Point(pB.X + ux * extension, pB.Y + uy * extension);

                switch (i)
                {
                    case 0:
                        lineTop.X1 = pAext.X; lineTop.Y1 = pAext.Y;
                        lineTop.X2 = pBext.X; lineTop.Y2 = pBext.Y;
                        lineTop.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        lineRight.X1 = pAext.X; lineRight.Y1 = pAext.Y;
                        lineRight.X2 = pBext.X; lineRight.Y2 = pBext.Y;
                        lineRight.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        lineBottom.X1 = pAext.X; lineBottom.Y1 = pAext.Y;
                        lineBottom.X2 = pBext.X; lineBottom.Y2 = pBext.Y;
                        lineBottom.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        lineLeft.X1 = pAext.X; lineLeft.Y1 = pAext.Y;
                        lineLeft.X2 = pBext.X; lineLeft.Y2 = pBext.Y;
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


        private void btnToggleContour_Click(object sender, RoutedEventArgs e)
        {
            showContours = !showContours;
            btnToggleContour.Content = showContours ? "Скрыть контур" : "Показать контур";
            DrawDetectedRectangle();
        }
    }
}
