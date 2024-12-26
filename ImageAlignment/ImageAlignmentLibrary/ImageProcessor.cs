using System;
using System.IO;
using System.Collections.Generic;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageAlignmentLibrary
{
    public class ImageProcessor : IDisposable
    {
        public Image<Rgba32>? OriginalImage { get; private set; }
        public Image<Rgba32>? AlignedImage { get; private set; }

        /// <summary>
        /// Храним здесь 4 точки (углы), найденные при последнем вызове "ReDetectDominantRectangleOnAligned"
        /// </summary>
        public List<System.Drawing.PointF>? DetectedRectanglePoints { get; private set; }

        /// <summary>
        /// Текущий угол поворота в градусах
        /// </summary>
        public double CurrentAngle { get; private set; } = 0.0;

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
        /// Сбрасывает выровненное изображение и обнуляет точки
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
        /// Автоматически вычисляет угол (по текущему AlignedImage), 
        /// поворачивает AlignedImage, а затем заново детектирует прямоугольник
        /// уже на повернутом AlignedImage.
        /// </summary>
        public void AutoAlignImage()
        {
            if (AlignedImage == null)
                throw new InvalidOperationException("Изображение не загружено.");

            // 1. Ищем угол на AlignedImage
            using var matAligned = ImageSharpToMat(AlignedImage);
            double angle = DetectAngleByDominantRectangle(matAligned);

            // 2. Поворачиваем AlignedImage на (-angle) градусов для выравнивания
            double targetAngle = CurrentAngle - angle;
            RotateImage(targetAngle);

            // 3. После поворота заново детектируем 
            //    уже на "AlignedImage", чтобы зелёный прямоугольник 
            //    соответствовал повернутой картинке
            ReDetectDominantRectangleOnAligned();
        }

        /// <summary>
        /// Запускает детектор на текущем "AlignedImage" 
        /// и сохраняет углы в DetectedRectanglePoints
        /// </summary>
        public void ReDetectDominantRectangleOnAligned()
        {
            DetectedRectanglePoints = null;
            if (AlignedImage == null) return;

            using var mat = ImageSharpToMat(AlignedImage);
            var corners = DetectDominantRectangle(mat, out double dummyAngle);
            if (corners != null)
            {
                var list = new List<System.Drawing.PointF>();
                foreach (var cvp in corners)
                {
                    list.Add(new System.Drawing.PointF((float)cvp.X, (float)cvp.Y));
                }
                DetectedRectanglePoints = list;
            }
        }

        /// <summary>
        /// Повернуть AlignedImage на targetAngle градусов относительно OriginalImage.
        /// </summary>
        public void RotateImage(double targetAngle)
        {
            if (OriginalImage == null) return;

            // Обновляем текущий угол
            CurrentAngle = targetAngle;

            // Удаляем предыдущую выровненную версию
            AlignedImage?.Dispose();

            // Создаём новую выровненную версию, повернутую на CurrentAngle относительно OriginalImage
            AlignedImage = OriginalImage.Clone(ctx =>
            {
                if (IsMultipleOf90((float)CurrentAngle))
                {
                    RotateMode mode = GetRotateMode((float)CurrentAngle);
                    if (mode != RotateMode.None)
                    {
                        ctx.Rotate(mode);
                    }
                }
                else
                {
                    ctx.Rotate((float)CurrentAngle, KnownResamplers.Bicubic);
                }
            });
        }

        /// <summary>
        /// Проверяет, является ли угол кратным 90 градусам
        /// </summary>
        private bool IsMultipleOf90(float angle)
        {
            float mod = angle % 90;
            return Math.Abs(mod) < 1e-3; // допускаем небольшую погрешность
        }

        /// <summary>
        /// Возвращает RotateMode для поворота кратного 90 градусам
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
        /// Детектируем угол (domRect) на переданном Mat (например, AlignedImage)
        /// Возвращает угол в градусах
        /// </summary>
        private double DetectAngleByDominantRectangle(Mat src)
        {
            // Получаем углы
            var corners = DetectDominantRectangle(src, out double angle);
            // Возвращаем угол
            return angle;
        }

        /// <summary>
        /// Находит самый большой контур, строит minAreaRect, 
        /// возвращает 4 угла (Point2f[4]) и угол (out)
        /// </summary>
        private OpenCvSharp.Point2f[]? DetectDominantRectangle(Mat src, out double outAngle)
        {
            outAngle = 0.0;

            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(5, 5), 0);

            // Используем Canny Edge Detection вместо простой пороговой обработки
            double threshold1 = 100;
            double threshold2 = 200;
            using var edges = new Mat();
            Cv2.Canny(gray, edges, threshold1, threshold2);

            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(edges, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            if (contours.Length == 0) return null;

            // Найти самый большой контур с минимальной площадью
            double imgArea = src.Width * src.Height;
            double minContourArea = imgArea * 0.01; // например, 1% от площади

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

            if (maxArea == 0.0) return null;

            double angleDetected = maxRect.Angle;
            if (angleDetected < -45) angleDetected += 90;

            outAngle = angleDetected;

            var rectCorners = maxRect.Points(); // 4 точки
            return rectCorners;
        }

        public void SaveAlignedImage(string filePath)
        {
            AlignedImage?.Save(filePath);
        }

        private Mat ImageSharpToMat(Image<Rgba32> img)
        {
            using var ms = new MemoryStream();
            img.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
        }

        private SixLabors.ImageSharp.Image<Rgba32> MatToImageSharp(Mat mat)
        {
            Cv2.ImEncode(".bmp", mat, out byte[] bytes);
            using var ms = new MemoryStream(bytes);
            return SixLabors.ImageSharp.Image.Load<Rgba32>(ms);
        }

        public void Dispose()
        {
            OriginalImage?.Dispose();
            AlignedImage?.Dispose();
        }
    }
}
