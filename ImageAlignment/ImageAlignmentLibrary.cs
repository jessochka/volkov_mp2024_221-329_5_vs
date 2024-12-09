﻿using System.Drawing;

namespace ImageAlignment.Library
{
    public class ImageProcessor
    {
        public Bitmap OriginalImage { get; private set; }
        public Bitmap AlignedImage { get; private set; }

        public ImageProcessor(Bitmap image)
        {
            OriginalImage = image;
            AlignedImage = image;
        }

        // Метод для выравнивания по внешнему контуру
        public void AlignByContour()
        {
            // Логика нахождения угла и выравнивания
            AlignedImage = RotateImage(OriginalImage, FindAngleByContour());
        }

        // Метод для выравнивания по содержимому
        public void AlignByContent()
        {
            // Логика нахождения угла и выравнивания
            AlignedImage = RotateImage(OriginalImage, FindAngleByContent());
        }

        // Пример метода нахождения угла по внешнему контуру
        private float FindAngleByContour()
        {
            // Логика анализа контура, возвращающая угол
            return 10.5f; // Пример: угол в градусах
        }

        // Пример метода нахождения угла по содержимому
        private float FindAngleByContent()
        {
            // Логика анализа содержимого, возвращающая угол
            return -5.0f; // Пример: угол в градусах
        }

        // Метод вращения изображения
        private Bitmap RotateImage(Bitmap bitmap, float angle)
        {
            // Логика вращения изображения на указанный угол
            // Реализация будет использована в будущем
            return bitmap; // Заглушка
        }
    }
}
