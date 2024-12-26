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
using System.Windows.Shapes;
using System.Collections.Generic;

// Добавляем псевдонимы для пространств имен
using WpfPoint = System.Windows.Point;
using ImageSharpPoint = SixLabors.ImageSharp.Point;

namespace ImageAlignmentWPF
{
    public partial class MainWindow : Window
    {
        private ImageProcessor? imageProcessor;
        private Image<Rgba32>? originalImage;

        private bool showContours = true; // Переменная для отслеживания видимости контура
        private bool isUpdating = false; // Переменная для предотвращения рекурсии при обновлении Slider и TextBox

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
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.png;*.bmp"
            };
            if (ofd.ShowDialog() == true)
            {
                // Сбрасываем старые контуры перед загрузкой нового изображения
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
            imageProcessor.ReDetectDominantRectangleOnAligned(); // Обновляем прямоугольник
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
            if (isUpdating) return; // предотвращаем рекурсию

            isUpdating = true;
            int targetAngle = (int)Math.Round(e.NewValue, MidpointRounding.AwayFromZero);
            txtAngle.Text = targetAngle.ToString();

            imageProcessor.RotateImage(targetAngle);
            imageProcessor.ReDetectDominantRectangleOnAligned(); // Обновляем прямоугольник
            UpdateImageDisplay();

            isUpdating = false;
        }

        private void txtAngle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(txtAngle.Text, out int targetAngle))
                {
                    if (targetAngle < (int)sliderAngle.Minimum) targetAngle = (int)sliderAngle.Minimum;
                    if (targetAngle > (int)sliderAngle.Maximum) targetAngle = (int)sliderAngle.Maximum;

                    if (imageProcessor != null)
                    {
                        isUpdating = true;
                        targetAngle = NormalizeAngle(targetAngle);
                        sliderAngle.Value = targetAngle;
                        txtAngle.Text = targetAngle.ToString();
                        isUpdating = false;

                        imageProcessor.RotateImage(targetAngle);
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
            double newAngleDouble = imageProcessor.CurrentAngle + 90;
            int angle = NormalizeAngle(newAngleDouble);

            // Обновляем Slider и TextBox без вызова событий
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
            double newAngleDouble = imageProcessor.CurrentAngle - 90;
            int angle = NormalizeAngle(newAngleDouble);

            // Обновляем Slider и TextBox без вызова событий
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
            return (int)Math.Round(angle, MidpointRounding.AwayFromZero);
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

        // Кнопка Автовыравнивания
        private void btnAutoAlign_Click(object sender, RoutedEventArgs e)
        {
            if (imageProcessor == null)
            {
                MessageBox.Show("Сначала загрузите изображение!");
                return;
            }
            try
            {
                // Выполняем автовыравнивание
                imageProcessor.AutoAlignImage();
                // AutoAlignImage внутри сам ещё раз вызовет 
                // ReDetectDominantRectangleOnAligned для повернутого

                // Обновляем Slider и TextBox
                isUpdating = true;
                int newAngle = (int)Math.Round(imageProcessor.CurrentAngle, MidpointRounding.AwayFromZero);
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

            // Обновляем прямоугольник
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

            // Получаем размеры изображения
            double imgWidth = imageProcessor.AlignedImage.Width;
            double imgHeight = imageProcessor.AlignedImage.Height;

            // Получаем размер отображаемого изображения
            double displayWidth = imgDisplay.ActualWidth;
            double displayHeight = imgDisplay.ActualHeight;

            if (displayWidth == 0 || displayHeight == 0) return; // предотвращаем деление на ноль

            // Вычисляем коэффициент масштабирования
            double ratioX = displayWidth / imgWidth;
            double ratioY = displayHeight / imgHeight;
            double ratio = Math.Min(ratioX, ratioY);

            // Вычисляем отступы
            double offsetX = (displayWidth - (imgWidth * ratio)) / 2;
            double offsetY = (displayHeight - (imgHeight * ratio)) / 2;

            // Создаём список точек
            var points = new List<WpfPoint>();
            foreach (var pt in imageProcessor.DetectedRectanglePoints)
            {
                double x = pt.X * ratio + offsetX;
                double y = pt.Y * ratio + offsetY;
                points.Add(new WpfPoint(x, y));
            }

            // Определяем линию и её расширение
            double extension = 10.0; // длина расширения линий

            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;
                WpfPoint p1 = points[i];
                WpfPoint p2 = points[next];

                // Вычисляем вектор направления
                double dx = p2.X - p1.X;
                double dy = p2.Y - p1.Y;
                double length = Math.Sqrt(dx * dx + dy * dy);

                if (length == 0) continue;

                // Нормализуем вектор
                double ux = dx / length;
                double uy = dy / length;

                // Расширяем линию вперед и назад
                WpfPoint extendedP1 = new WpfPoint(p1.X - ux * extension, p1.Y - uy * extension);
                WpfPoint extendedP2 = new WpfPoint(p2.X + ux * extension, p2.Y + uy * extension);

                // Назначаем координаты соответствующим линиям
                switch (i)
                {
                    case 0:
                        // Верхняя линия
                        lineTop.X1 = extendedP1.X;
                        lineTop.Y1 = extendedP1.Y;
                        lineTop.X2 = extendedP2.X;
                        lineTop.Y2 = extendedP2.Y;
                        lineTop.Visibility = Visibility.Visible;
                        break;
                    case 1:
                        // Правая линия
                        lineRight.X1 = extendedP1.X;
                        lineRight.Y1 = extendedP1.Y;
                        lineRight.X2 = extendedP2.X;
                        lineRight.Y2 = extendedP2.Y;
                        lineRight.Visibility = Visibility.Visible;
                        break;
                    case 2:
                        // Нижняя линия
                        lineBottom.X1 = extendedP1.X;
                        lineBottom.Y1 = extendedP1.Y;
                        lineBottom.X2 = extendedP2.X;
                        lineBottom.Y2 = extendedP2.Y;
                        lineBottom.Visibility = Visibility.Visible;
                        break;
                    case 3:
                        // Левая линия
                        lineLeft.X1 = extendedP1.X;
                        lineLeft.Y1 = extendedP1.Y;
                        lineLeft.X2 = extendedP2.X;
                        lineLeft.Y2 = extendedP2.Y;
                        lineLeft.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        /// <summary>
        /// Метод для скрытия всех линий контура
        /// </summary>
        private void HideAllContours()
        {
            lineTop.Visibility = Visibility.Collapsed;
            lineRight.Visibility = Visibility.Collapsed;
            lineBottom.Visibility = Visibility.Collapsed;
            lineLeft.Visibility = Visibility.Collapsed;
        }

        // Обработчик события MouseDown на Canvas (реализуйте при необходимости)
        private void canvasImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Реализуйте необходимую логику при нажатии мыши на Canvas
            // Например, можно добавить выбор области или отображение информации
        }

        // Обработчики событий для CheckBoxes (chkGuidelines и chkDiagonals)

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
