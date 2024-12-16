using System;
using System.IO;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageAlignmentLibrary
{
    public enum AutoAlignMethod
    {
        ByContour,
        ByContent
    }

    public class ImageProcessor : IDisposable
    {
        public Image<Rgba32>? OriginalImage { get; private set; }
        public Image<Rgba32>? AlignedImage { get; private set; }

        public bool AutoCrop { get; set; } = false; // Включение автообрезания

        /// <summary>
        /// Загружает изображение из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу изображения.</param>
        public void LoadImage(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            OriginalImage = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
            AlignedImage = OriginalImage.Clone();
        }

        /// <summary>
        /// Сбрасывает изображение к оригиналу
        /// </summary>
        /// <param name="original">Оригинальное изображение</param>
        public void ResetImage(Image<Rgba32> original)
        {
            AlignedImage?.Dispose();
            AlignedImage = original.Clone();
        }

        /// <summary>
        /// Поворачивает изображение на заданный угол
        /// Вращение происходит внутри фиксированных границ, предотвращая выход за пределы PictureBox
        /// </summary>
        /// <param name="angleDegrees">Угол поворота в градусах</param>
        public void RotateImage(float angleDegrees)
        {
            if (OriginalImage == null)
                throw new InvalidOperationException("Изображение не загружено.");

            AlignedImage?.Dispose();

            // Проверка, кратен ли угол 90 градусам и не равен 0°
            if (angleDegrees % 90 == 0 && angleDegrees != 0)
            {
                // Используем RotateMode для точного поворота без артефактов
                RotateMode mode;
                switch (angleDegrees % 360)
                {
                    case 90:
                        mode = RotateMode.Rotate90;
                        break;
                    case 180:
                        mode = RotateMode.Rotate180;
                        break;
                    case 270:
                    case -90:
                        mode = RotateMode.Rotate270;
                        break;
                    default:
                        // Если угол не соответствует, пропускаем поворот
                        AlignedImage = OriginalImage.Clone();
                        return;
                }

                AlignedImage = OriginalImage.Clone(ctx =>
                {
                    ctx.Rotate(mode);
                });
            }
            else if (angleDegrees != 0)
            {
                // Для произвольных углов используем поворот внутри фиксированных границ

                // Шаг 1: Вращаем изображение
                var rotated = OriginalImage.Clone(ctx =>
                {
                    ctx.Rotate(angleDegrees, KnownResamplers.Bicubic);
                });

                // Шаг 2: Создаём новое изображение с теми же размерами и белым фоном
                var background = new Image<Rgba32>(OriginalImage.Width, OriginalImage.Height, Color.White);

                // Шаг 3: Вычисляем позицию для центрирования повернутого изображения
                int x = (background.Width - rotated.Width) / 2;
                int y = (background.Height - rotated.Height) / 2;

                // Шаг 4: Накладываем повернутое изображение на белый фон
                background.Mutate(ctx =>
                {
                    ctx.DrawImage(rotated, new SixLabors.ImageSharp.Point(x, y), 1f);
                });

                // Шаг 5: Назначаем новое изображение как AlignedImage
                AlignedImage = background;
            }
            else
            {
                // Угол 0°, копируем оригинал
                AlignedImage = OriginalImage.Clone();
            }

            if (AutoCrop)
            {
                AutoCropImage();
            }
        }

        /// <summary>
        /// Автоматически выравнивает изображение по выбранному методу
        /// </summary>
        /// <param name="method">Метод выравнивания</param>
        public void AutoAlignImage(AutoAlignMethod method)
        {
            if (OriginalImage == null)
                throw new InvalidOperationException("Изображение не загружено.");

            using var mat = ImageSharpToMat(OriginalImage);
            double angle = 0.0;

            switch (method)
            {
                case AutoAlignMethod.ByContour:
                    angle = GetSkewAngleByContour(mat);
                    break;
                case AutoAlignMethod.ByContent:
                    angle = GetSkewAngleByContent(mat);
                    break;
                default:
                    throw new ArgumentException("Неизвестный метод выравнивания.");
            }

            RotateImage((float)angle);
        }

        /// <summary>
        /// Сохраняет выровненное изображение в файл
        /// </summary>
        /// <param name="filePath">Путь к файлу сохранения</param>
        public void SaveAlignedImage(string filePath)
        {
            AlignedImage?.Save(filePath);
        }

        /// <summary>
        /// Преобразует ImageSharp Image в Mat OpenCV
        /// </summary>
        /// <param name="image">ImageSharp Image</param>
        /// <returns>Mat OpenCV.</returns>
        private Mat ImageSharpToMat(Image<Rgba32> image)
        {
            using var ms = new MemoryStream();
            image.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
        }

        /// <summary>
        /// Вычисляет угол наклона по внешнему контуру
        /// </summary>
        private double GetSkewAngleByContour(Mat src)
        {
            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(5, 5), 0);
            Cv2.Canny(gray, gray, 50, 150, 3);
            Cv2.Dilate(gray, gray, null);
            Cv2.Erode(gray, gray, null);

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(gray, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            double maxArea = 0;
            RotatedRect maxRect = new RotatedRect();

            foreach (var contour in contours)
            {
                double area = Cv2.ContourArea(contour);
                if (area > maxArea)
                {
                    maxArea = area;
                    maxRect = Cv2.MinAreaRect(contour);
                }
            }

            double angle;
            if (maxRect.Angle < -45)
            {
                angle = maxRect.Angle + 90;
            }
            else
            {
                angle = maxRect.Angle;
            }

            return angle;
        }

        /// <summary>
        /// Вычисляет угол наклона по содержимому
        /// </summary>
        private double GetSkewAngleByContent(Mat src)
        {
            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(5, 5), 0);
            Cv2.Threshold(gray, gray, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            var lines = Cv2.HoughLinesP(
                gray,
                1,
                Math.PI / 180,
                100,
                minLineLength: gray.Cols / 2,
                maxLineGap: 20
            );

            double angleSum = 0.0;
            int count = 0;

            foreach (var line in lines)
            {
                double currentAngle = Math.Atan2(line.P2.Y - line.P1.Y, line.P2.X - line.P1.X) * 180.0 / Math.PI;
                angleSum += currentAngle;
                count++;
            }

            if (count == 0)
                return 0.0;

            double avgAngle = angleSum / count;
            return avgAngle;
        }

        /// <summary>
        /// Автоматически обрезает изображение
        /// </summary>
        private void AutoCropImage()
        {
            if (AlignedImage == null)
                return;

            using var mat = ImageSharpToMat(AlignedImage);
            using var gray = new Mat();
            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, gray, 10, 255, ThresholdTypes.Binary);

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(gray, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            if (contours.Length == 0)
                return;

            Rect boundingBox = Cv2.BoundingRect(contours[0]);
            for (int i = 1; i < contours.Length; i++)
            {
                boundingBox = Rect.Union(boundingBox, Cv2.BoundingRect(contours[i]));
            }

            using var croppedMat = new Mat(mat, boundingBox);
            AlignedImage?.Dispose();
            AlignedImage = MatToImageSharp(croppedMat);
        }

        /// <summary>
        /// Преобразует Mat в ImageSharp Image
        /// </summary>
        private Image<Rgba32> MatToImageSharp(Mat mat)
        {
            Cv2.ImEncode(".bmp", mat, out byte[] imageBytes);
            using var ms = new MemoryStream(imageBytes);
            return SixLabors.ImageSharp.Image.Load<Rgba32>(ms);
        }

        /// <summary>
        /// Применяет автообрезание к текущему выровненному изображению
        /// </summary>
        public void ApplyAutoCrop()
        {
            if (AlignedImage == null)
                throw new InvalidOperationException("Выровнённое изображение отсутствует.");

            AutoCropImage();
        }

        public void Dispose()
        {
            OriginalImage?.Dispose();
            AlignedImage?.Dispose();
        }
    }
}