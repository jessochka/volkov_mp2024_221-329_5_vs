namespace cnsColor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Красный текст на белом фоне");
            Console.ResetColor();
            Console.WriteLine("Текст здорового человека");

            foreach (var color in Enum.GetValues<ConsoleColor>())
            {
                Console.BackgroundColor = color;
                Console.Write(new string(' ', 3));
                Console.ResetColor();
                Console.WriteLine(" - " + color.ToString());
            }

        }
    }
}
