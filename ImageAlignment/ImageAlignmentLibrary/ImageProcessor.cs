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
        public Image<Rgba32>? OriginalImage { get; private set; }
        public Image<Rgba32>? AlignedImage { get; set; }
        public List<PointF>? DetectedRectanglePoints { get; private set; }
        public double CurrentAngle { get; private set; } = 0.0;

        // Загрузка изображения
        public void LoadImage(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            OriginalImage = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
            AlignedImage = OriginalImage.Clone();
            DetectedRectanglePoints = null;
            CurrentAngle = 0.0;
        }

        // Сброс к исходному
        public void ResetImage()
        {
            if (OriginalImage == null) return;

            AlignedImage?.Dispose();
            AlignedImage = OriginalImage.Clone();
            DetectedRectanglePoints = null;
            CurrentAngle = 0.0;
        }

        // Главное автовыравнивание
        public void AutoAlignImage()
        {
            if (AlignedImage == null)
                throw new InvalidOperationException("Изображение не загружено.");

            // 1) вычисляем угол
            double angle = DetectAngle(AlignedImage);

            // 2) Поворот = (текущий угол - обнаруженный)
            double targetAngle = CurrentAngle - angle;
            RotateImage(targetAngle);

            // 3) После поворота — ещё раз ищем контур
            for (int attempt = 0; attempt < 3; attempt++)
            {
                ReDetectDominantRectangleOnAligned();
                // если нашли 4 угла — ок
                if (DetectedRectanglePoints != null && DetectedRectanglePoints.Count == 4)
                    break;
                // иначе усилим резкость/контраст
                EnhanceImageForContours();
            }
        }

        // Пере-детектирование контура на текущем AlignedImage
        public void ReDetectDominantRectangleOnAligned()
        {
            DetectedRectanglePoints = null;
            if (AlignedImage == null) return;

            using var mat = ImageSharpToMat(AlignedImage);
            var corners = DetectDominantRectangle(mat, out double dummyAngle);
            if (corners != null && corners.Length == 4)
            {
                var list = new List<PointF>();
                foreach (var cvp in corners)
                    list.Add(new PointF(cvp.X, cvp.Y));
                DetectedRectanglePoints = list;
            }
        }

        // Поворот на нужный угол
        public void RotateImage(double targetAngle)
        {
            if (OriginalImage == null) return;
            CurrentAngle = targetAngle;

            AlignedImage?.Dispose();
            AlignedImage = OriginalImage.Clone(ctx =>
            {
                float fAngle = (float)CurrentAngle;
                if (IsMultipleOf90(fAngle))
                {
                    var mode = GetRotateMode(fAngle);
                    if (mode != RotateMode.None)
                        ctx.Rotate(mode);
                }
                else
                {
                    ctx.Rotate(fAngle, KnownResamplers.Bicubic);
                }
            });
        }

        // Сохранение результата
        public void SaveAlignedImage(string filePath)
        {
            AlignedImage?.Save(filePath);
        }

        // Освобождение ресурсов
        public void Dispose()
        {
            OriginalImage?.Dispose();
            AlignedImage?.Dispose();
        }


        public void AutoCrop()
        {
            // Выровненное изображение есть и определены углы прямоугольника
            if (AlignedImage == null || DetectedRectanglePoints == null || DetectedRectanglePoints.Count == 0)
                return;

            // Определение минимальных и максимальных координат по X и Y среди точек контура
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;
            foreach (var pt in DetectedRectanglePoints)
            {
                if (pt.X < minX) minX = pt.X;
                if (pt.Y < minY) minY = pt.Y;
                if (pt.X > maxX) maxX = pt.X;
                if (pt.Y > maxY) maxY = pt.Y;
            }

            // Добавление margin для отображения содержимого
            int margin = 5;
            int left = Math.Max((int)Math.Floor(minX) - margin, 0);
            int top = Math.Max((int)Math.Floor(minY) - margin, 0);
            int right = Math.Min((int)Math.Ceiling(maxX) + margin, AlignedImage.Width - 1);
            int bottom = Math.Min((int)Math.Ceiling(maxY) + margin, AlignedImage.Height - 1);

            int cropWidth = right - left;
            int cropHeight = bottom - top;

            if (cropWidth > 0 && cropHeight > 0)
            {
                // Корректировка координат точек контура относительно нового изображения
                for (int i = 0; i < DetectedRectanglePoints.Count; i++)
                {
                    var pt = DetectedRectanglePoints[i];
                    DetectedRectanglePoints[i] = new PointF(pt.X - left, pt.Y - top);
                }

                // Обрезка
                AlignedImage.Mutate(ctx => ctx.Crop(new SixLabors.ImageSharp.Rectangle(left, top, cropWidth, cropHeight)));
            }
        }


        #region Внутренние методы

        // Вычисляет угол на переданном Image (через OpenCV)
        private double DetectAngle(Image<Rgba32> srcImg)
        {
            using var mat = ImageSharpToMat(srcImg);
            var corners = DetectDominantRectangle(mat, out double angle);
            return angle;
        }

        // Усиливаем картинку, чтобы OpenCV лучше находил контуры
        private void EnhanceImageForContours()
        {
            AlignedImage?.Mutate(ctx =>
            {
                ctx.Contrast(1.2f);
                ctx.Brightness(1.05f);
            });
        }

        private bool IsMultipleOf90(float angle)
        {
            float mod = angle % 90;
            return Math.Abs(mod) < 1e-3;
        }

        private RotateMode GetRotateMode(float angle)
        {
            angle = angle % 360;
            if (angle < 0) angle += 360;

            return angle switch
            {
                90 => RotateMode.Rotate90,
                180 => RotateMode.Rotate180,
                270 => RotateMode.Rotate270,
                _ => RotateMode.None
            };
        }

        // Метод поиска внешнего прямоугольника (для контура)
        private OpenCvSharp.Point2f[]? DetectDominantRectangle(Mat src, out double outAngle)
        {
            outAngle = 0.0;
            int w = src.Width, h = src.Height;
            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Адаптивный порог
            using var bin = new Mat();
            Cv2.AdaptiveThreshold(gray, bin, 255,
                AdaptiveThresholdTypes.GaussianC,
                ThresholdTypes.BinaryInv, 11, 2);

            // Морфология
            using var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(3, 3));
            Cv2.MorphologyEx(bin, bin, MorphTypes.Close, kernel);

            // Находим контуры
            Cv2.FindContours(bin, out OpenCvSharp.Point[][] contours, out HierarchyIndex[] hierarchy,
                RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            if (contours.Length == 0) return null;

            double imgArea = w * h;
            double minAreaThresh = imgArea * 0.005; // Игнорируем слишком маленькие контуры

            var allPoints = new List<OpenCvSharp.Point>();

            foreach (var contour in contours)
            {
                double area = Cv2.ContourArea(contour);
                if (area >= minAreaThresh && !ContourTouchesBorder(contour, w, h))
                {
                    allPoints.AddRange(contour);
                }
            }

            if (allPoints.Count == 0) return null;

            // Вычисляем выпуклую оболочку по всем подходящим точкам
            OpenCvSharp.Point[] hullPoints = Cv2.ConvexHull(allPoints);

            // Вычисляем минимально охватывающий прямоугольник по выпуклой оболочке
            var hullPoint2f = Array.ConvertAll(hullPoints, p => new OpenCvSharp.Point2f(p.X, p.Y));
            RotatedRect maxRect = Cv2.MinAreaRect(hullPoint2f);

            double angleFound = maxRect.Angle;
            if (angleFound < -45) angleFound += 90;
            outAngle = angleFound;

            var rectCorners = maxRect.Points();
            return rectCorners;
        }


        // Конвертер ImageSharp в Mat для OpenCV
        private Mat ImageSharpToMat(Image<Rgba32> img)
        {
            using var ms = new MemoryStream();
            img.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
        }
        private bool ContourTouchesBorder(OpenCvSharp.Point[] contour, int width, int height)
        {
            foreach (var p in contour)
            {
                if (p.X <= 1 || p.Y <= 1 || p.X >= width - 2 || p.Y >= height - 2)
                    return true;
            }
            return false;
        }
        #endregion
    }
}
