using LibCore;

namespace wfaTrainerAccount
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Game game = new Game();
            game.ChangeQuestion += () => labelQuestion.Text = game.QuestionText;
            game.ChangeStats += delegate
            {
                labelCorrectStat.Text = $"Верно = {game.CountCorrect}";
                labelInvalidStat.Text = $"Неверно = {game.CountInvalid}";
            };
            game.gameStart();

            buttonYes.Click += (s, e) => game.checkAnswer(true);
            buttonNo.Click += (s, e) => game.checkAnswer(false);
        }
    }
}
