using System;
using System.IO;
using OpenCvSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageAlignmentLibrary
{
    /// Способы определения угла
    public enum AutoAlignMethod
    {
        ByContour,
        ByContent
    }

    /// Класс: загружает изображение, умеет вращать, сбрасывать,
    /// находить угол по разным способам, сохранять результат
    public class ImageProcessor : IDisposable
    {
        public Image<Rgba32>? OriginalImage { get; private set; }
        public Image<Rgba32>? AlignedImage { get; private set; }

        public bool AutoCrop { get; set; } = false; // При повороте делаем обрезку

        public void LoadImage(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            OriginalImage = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
            AlignedImage = OriginalImage.Clone();
        }

        /// Сбрасывает AlignedImage к исходному
        public void ResetImage()
        {
            if (OriginalImage == null) return;
            AlignedImage?.Dispose();
            AlignedImage = OriginalImage.Clone();
        }

        /// Определяет угол по контуру или содержимому, поворачивает
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
            }

            RotateImage((float)angle);
        }

        /// Поворот
        /// Если AutoCrop=true, делаем AutoCropImage()
        public void RotateImage(float angleDeg)
        {
            if (OriginalImage == null) return;

            AlignedImage?.Dispose();

            float mod = angleDeg % 360;
            bool is90 = (mod == 90 || mod == -270 ||
                         mod == 180 || mod == -180 ||
                         mod == 270 || mod == -90);

            if (is90)
            {
                RotateMode mode = RotateMode.None;
                if (mod == 90 || mod == -270) mode = RotateMode.Rotate90;
                else if (mod == 180 || mod == -180) mode = RotateMode.Rotate180;
                else if (mod == 270 || mod == -90) mode = RotateMode.Rotate270;

                AlignedImage = OriginalImage.Clone(ctx =>
                {
                    ctx.Rotate(mode);
                });
            }
            else
            {
                AlignedImage = OriginalImage.Clone(ctx =>
                {
                    ctx.Rotate(angleDeg, KnownResamplers.Bicubic);
                });
            }

            if (AutoCrop)
                AutoCropImage();
        }

        /// Автообрезка по белому фону (Threshold=250)
        public void AutoCropImage()
        {
            if (AlignedImage == null) return;

            using var mat = ImageSharpToMat(AlignedImage);
            using var gray = new Mat();
            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Threshold(gray, gray, 250, 255, ThresholdTypes.BinaryInv);

            OpenCvSharp.Point[][] cts;
            HierarchyIndex[] hier;
            Cv2.FindContours(gray, out cts, out hier,
                RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            if (cts.Length == 0) return;

            var r = Cv2.BoundingRect(cts[0]);
            for (int i = 1; i < cts.Length; i++)
            {
                r = Rect.Union(r, Cv2.BoundingRect(cts[i]));
            }
            if (r.Width > 0 && r.Height > 0)
            {
                using var cropped = new Mat(mat, r);
                AlignedImage.Dispose();
                AlignedImage = MatToImageSharp(cropped);
            }
        }

        public void SaveAlignedImage(string filePath)
        {
            AlignedImage?.Save(filePath);
        }

        // --- Угол по внешнему контуру
        private double GetSkewAngleByContour(Mat src)
        {
            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(5, 5), 0);
            Cv2.Canny(gray, gray, 50, 150);
            Cv2.Dilate(gray, gray, null);
            Cv2.Erode(gray, gray, null);

            OpenCvSharp.Point[][] cts;
            HierarchyIndex[] hier;
            Cv2.FindContours(gray, out cts, out hier,
                RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            double maxArea = 0;
            RotatedRect bestRect = new RotatedRect();
            foreach (var contour in cts)
            {
                double area = Cv2.ContourArea(contour);
                if (area > maxArea)
                {
                    maxArea = area;
                    bestRect = Cv2.MinAreaRect(contour);
                }
            }
            double angle = bestRect.Angle;
            if (angle < -45) angle += 90;
            return angle;
        }

        // --- Угол по содержимому (линии)
        private double GetSkewAngleByContent(Mat src)
        {
            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(5, 5), 0);
            Cv2.Threshold(gray, gray, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);

            var lines = Cv2.HoughLinesP(
                gray, 1, Math.PI / 180, 100,
                minLineLength: gray.Cols / 2,
                maxLineGap: 20);
            if (lines.Length == 0) return 0.0;

            double sum = 0;
            foreach (var line in lines)
            {
                double a = Math.Atan2(line.P2.Y - line.P1.Y, line.P2.X - line.P1.X) * 180.0 / Math.PI;
                sum += a;
            }
            return sum / lines.Length;
        }

        private Mat ImageSharpToMat(Image<Rgba32> img)
        {
            using var ms = new MemoryStream();
            img.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
        }
        private Image<Rgba32> MatToImageSharp(Mat mat)
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
