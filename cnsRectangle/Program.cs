using System.Runtime.CompilerServices;

namespace cnsRectangle
{
    internal class Program
    {
        /// <summary>
        /// Функция для генерации прямоугольника
        /// </summary>
        static string[] GenerateRectangle(
            int width,
            int height,
            bool fill = true,
            char drawChar = '*',
            char spaceChar = ' '
        )
        {
            string[] rectangle = new string[height];
            for (int r = 0; r < height; r++)
                rectangle[r] = (r == 0 || r == height - 1 || fill) ? new string(drawChar, width) : $"{drawChar}{new string(spaceChar, width - 2)}{drawChar}";
            return rectangle;
        }

        static void Main(string[] args)
        {
            Console.Write("Width: ");
            int width = Convert.ToInt16(Console.ReadLine());
            Console.Write("Height: ");
            int height = Convert.ToInt16(Console.ReadLine());
            Console.Write("Draw char: ");
            char drawChar = Convert.ToChar(Console.ReadLine());
            Console.Write("Fill [y/n]: ");
            bool fill = Console.ReadLine().ToLower() == "y";

            Console.WriteLine("Your rectangle generated:");
            foreach (string line in GenerateRectangle(width, height, fill, drawChar).ToArray())
                Console.WriteLine(line);
        }
    }
}
