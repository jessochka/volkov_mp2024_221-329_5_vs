using LibCore;

namespace cnsTrainerAccount
{
    internal class Progam
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Игра 'Устный счёт'");
            Console.WriteLine();

            Game game = new();

            game.ChangeQuestion += () => Console.WriteLine($"Вопрос: {game.QuestionText}");
            game.ChangeStats += () => Console.WriteLine($"Статистика: Верно = {game.CountCorrect}, Неверно = {game.CountInvalid}");

            game.gameStart();

            var dt_start = DateTime.Now;
            while (true)
            {
                Console.Write("Ответ Y/N ?");
                var line = Console.ReadLine()?.ToUpper();

                if (line == "Y")
                    game.checkAnswer(true);
                else if (line == "N")
                    game.checkAnswer(false);
                else
                    break;
            }

            Console.WriteLine();
            Console.WriteLine($"Ты играл {(DateTime.Now - dt_start).TotalSeconds:0} секунд");
            Console.WriteLine("Пока!");
        }
    }
}