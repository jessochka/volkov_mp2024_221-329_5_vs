using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAlignmentLibrary
{
    public class ImageProcessor
    {
        public Image<Rgba32> OriginalImage { get; private set; }
        public Image<Rgba32> AlignedImage { get; private set; }

        public void LoadImage(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            OriginalImage = Image.Load<Rgba32>(filePath);
            AlignedImage = OriginalImage.Clone();
        }

        public void RotateImage(float angle)
        {
            if (AlignedImage == null)
                throw new InvalidOperationException("Загрузите изображение перед вращением.");

            AlignedImage.Mutate(ctx => ctx.Rotate(angle));
        }

        public void SaveAlignedImage(string filePath)
        {
            if (AlignedImage == null)
                throw new InvalidOperationException("Обработанное изображение отсутствует.");

            AlignedImage.Save(filePath);
        }

        public void Dispose()
        {
            OriginalImage?.Dispose();
            AlignedImage?.Dispose();
        }
    }
}
