using System.Windows.Forms;

namespace wfaTrainerAccount
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Game game = new Game();
            game.ChangeQuestion += () => labelQuestion.Text = game.QuestionText;
            game.ChangeStatistic += delegate
            {
                label1 = $"Верно = {game.CountCorrect}";
                label2 = $"Неверно = {game.CountInvalid}";
            };
            game.gameStart();

            buttonYes.Click += (s, e) => game.attemptAnswer(true);
            buttonNo.Click += (s, e) => game.attemptAnswer(false);
        }
    }
}
