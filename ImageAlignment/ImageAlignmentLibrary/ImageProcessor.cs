using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using PointF = System.Drawing.PointF;

namespace ImageAlignmentLibrary
{
    public class ImageProcessor : IDisposable
    {
        /// <summary>
        /// Исходное изображение
        /// </summary>
        public Image<Rgba32>? OriginalImage { get; private set; }

        /// <summary>
        /// Текущее выравненное (или повернутое) изображение
        /// </summary>
        public Image<Rgba32>? AlignedImage { get; private set; }

        /// <summary>
        /// 4 угловые точки последнего детектированного прямоугольника
        /// </summary>
        public List<PointF>? DetectedRectanglePoints { get; private set; }

        /// <summary>
        /// Текущий угол поворота изображения
        /// </summary>
        public double CurrentAngle { get; private set; } = 0.0;

        /// <summary>
        /// Загрузить изображение с диска
        /// </summary>
        public void LoadImage(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            OriginalImage = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
            AlignedImage = OriginalImage.Clone();
            DetectedRectanglePoints = null;
            CurrentAngle = 0.0;
        }

        /// <summary>
        /// Сбросить все изменения и вернуть исходное изображение
        /// </summary>
        public void ResetImage()
        {
            if (OriginalImage == null) return;

            AlignedImage?.Dispose();
            AlignedImage = OriginalImage.Clone();
            DetectedRectanglePoints = null;
            CurrentAngle = 0.0;
        }

        /// <summary>
        /// Метод "автовыравнивания": ищем угол на текущем AlignedImage, 
        /// поворачиваем на -angle, и заново детектируем контур
        /// </summary>
        public void AutoAlignImage()
        {
            if (AlignedImage == null)
                throw new InvalidOperationException("Изображение не загружено.");

            // 1. Ищем угол (по внешнему контуру) - основной способ
            double angle = DetectAngleOnAligned();

            // 2. Поворачиваем на -angle
            double targetAngle = CurrentAngle - angle;
            RotateImage(targetAngle);

            // 3. Повторяем поиск несколько раз, если не получилось
            //    с первого раза найти контур
            const int maxTries = 3;
            for (int i = 0; i < maxTries; i++)
            {
                ReDetectDominantRectangleOnAligned();
                if (DetectedRectanglePoints != null && DetectedRectanglePoints.Count == 4)
                {
                    // если нашёлся контур — выходим
                    break;
                }
                // Иначе можно чуть усилить резкость или контраст
                EnhanceEdges();
            }
        }

        /// <summary>
        /// Заново детектируем прямоугольник на текущем AlignedImage
        /// (используя поиск перепада пикселей "белый фон -> картинка")
        /// Если не находим — повторить процедуру несколько раз в AutoAlignImage()
        /// </summary>
        public void ReDetectDominantRectangleOnAligned()
        {
            DetectedRectanglePoints = null;
            if (AlignedImage == null) return;

            using var mat = ImageSharpToMat(AlignedImage);
            var corners = DetectDominantRectangle(mat, out double _);
            if (corners != null)
            {
                var list = new List<PointF>();
                foreach (var cvp in corners)
                {
                    list.Add(new PointF(cvp.X, cvp.Y));
                }
                DetectedRectanglePoints = list;
            }
        }

        /// <summary>
        /// Повернуть текущее "AlignedImage" на targetAngle (относительно OriginalImage)
        /// </summary>
        public void RotateImage(double targetAngle)
        {
            if (OriginalImage == null) return;

            CurrentAngle = targetAngle;
            AlignedImage?.Dispose();

            AlignedImage = OriginalImage.Clone(ctx =>
            {
                if (IsMultipleOf90((float)CurrentAngle))
                {
                    RotateMode mode = GetRotateMode((float)CurrentAngle);
                    if (mode != RotateMode.None) ctx.Rotate(mode);
                }
                else
                {
                    ctx.Rotate((float)CurrentAngle, KnownResamplers.Bicubic);
                }
            });
        }

        /// <summary>
        /// Сохранить текущее выравненное изображение на диск
        /// </summary>
        public void SaveAlignedImage(string filePath)
        {
            AlignedImage?.Save(filePath);
        }

        #region Внутренние методы

        /// <summary>
        /// Общая логика: ищем угол на текущем "AlignedImage"
        /// </summary>
        private double DetectAngleOnAligned()
        {
            using var mat = ImageSharpToMat(AlignedImage!);
            var corners = DetectDominantRectangle(mat, out double angle);
            return angle;
        }

        /// <summary>
        /// Попытка "причесать" изображение — поднять контраст,
        /// чтобы перепад между белым фоном и объектом был отчётливее
        /// </summary>
        private void EnhanceEdges()
        {
            AlignedImage?.Mutate(ctx =>
            {
                ctx.Contrast(1.2f);
                ctx.Brightness(1.05f);
            });
        }

        /// <summary>
        /// Проверяем, является ли угол кратным 90
        /// </summary>
        private bool IsMultipleOf90(float angle)
        {
            float mod = angle % 90;
            return Math.Abs(mod) < 1e-3;
        }

        /// <summary>
        /// Определяем RotateMode (90/180/270)
        /// </summary>
        private RotateMode GetRotateMode(float angle)
        {
            angle = angle % 360;
            if (angle < 0) angle += 360;

            return angle switch
            {
                90 => RotateMode.Rotate90,
                180 => RotateMode.Rotate180,
                270 => RotateMode.Rotate270,
                _ => RotateMode.None,
            };
        }

        /// <summary>
        /// Детектируем прямоугольник: ищем самый большой контур,
        /// строим minAreaRect, возвращаем 4 точки + угол (outAngle)
        /// Считаем, что фон — белый, а объект — более тёмный
        /// </summary>
        private OpenCvSharp.Point2f[]? DetectDominantRectangle(Mat src, out double outAngle)
        {
            outAngle = 0.0;
            int width = src.Width, height = src.Height;

            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Мягкий blur
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(5, 5), 0);

            // Допустим, фон очень светлый (белый), значит, можно найти переход по threshold
            using var edges = new Mat();
            Cv2.Canny(gray, edges, 80, 200);

            // Находим контуры
            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(edges, out contours, out hierarchy,
                RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            if (contours.Length == 0) return null;

            double imgArea = width * height;
            double minContourArea = imgArea * 0.005; // 0.5% от площади

            double maxArea = 0.0;
            RotatedRect maxRect = new RotatedRect();
            foreach (var c in contours)
            {
                double area = Cv2.ContourArea(c);
                if (area > minContourArea && area > maxArea)
                {
                    maxArea = area;
                    maxRect = Cv2.MinAreaRect(c);
                }
            }

            if (maxArea <= 0.0) return null;

            double angleDetected = maxRect.Angle;
            if (angleDetected < -45) angleDetected += 90;

            outAngle = angleDetected;
            var rectCorners = maxRect.Points();

            return rectCorners;
        }

        /// <summary>
        /// Преобразуем ImageSharp -> OpenCV (Mat)
        /// </summary>
        private Mat ImageSharpToMat(Image<Rgba32> img)
        {
            using var ms = new MemoryStream();
            img.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
        }

        public void Dispose()
        {
            OriginalImage?.Dispose();
            AlignedImage?.Dispose();
        }

        #endregion
    }
}
