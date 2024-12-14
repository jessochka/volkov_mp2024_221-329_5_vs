using System.Drawing;

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

        public Bitmap RotateImage(Bitmap bitmap, float angle)
        {
            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.TranslateTransform(bitmap.Width / 2, bitmap.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-bitmap.Width / 2, -bitmap.Height / 2);
                g.DrawImage(bitmap, new Point(0, 0));
            }
            return result;
        }
    }
}
