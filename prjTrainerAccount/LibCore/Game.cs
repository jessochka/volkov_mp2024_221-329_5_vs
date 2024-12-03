

namespace LibCore
{
    public class Game
    {
        private Random rnd = new();
        public int CountCorrect { get; private set; }
        public int CountInvalid { get; private set; }
        public string QuestionText { get; private set; }

        private bool answer;

        public event Action? ChangeQuestion;
        public event Action? ChangeStats;

        public void gameStart()
        {
            CountCorrect = 0;
            CountInvalid = 0;

            randomizeQuestion();
        }

        /**
         * Randomizes the question
         */
        private void randomizeQuestion()
        {
            var value1 = rnd.Next(20);
            var value2 = rnd.Next(20);
            var result = value1 + value2;
            var finalResult = result;

            if (rnd.Next(2) == 1)
                finalResult += rnd.Next(1, 7) * (rnd.Next(2) == 1 ? 1 : -1);

            QuestionText = $"{value1} + {value2} = {finalResult}";
            answer = result == finalResult;
            ChangeQuestion.Invoke();
        }

        /**
         * Checks, whether the inputted answer matches the correct one
         */
        public void checkAnswer(bool userAnswer)
        {
            if (userAnswer == answer)
                CountCorrect++;
            else
                CountInvalid++;
            ChangeStats.Invoke();

            randomizeQuestion();
        }
    }
}
