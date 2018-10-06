namespace MathQuizWPF.ViewModel
{
    using System;
    using System.Windows;
    using System.ComponentModel;
    using System.Threading;

    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Game game;

        private DifficultyLevel difficultyLevel;
        private bool isPlaying;
        private string question;
        private string correctAnswer;

        private Visibility questionVisibility = Visibility.Visible;
        private Visibility answerVisibility = Visibility.Visible;
        private Visibility xFigureVisibility = Visibility.Hidden;
        private Visibility vFigureVisibility = Visibility.Hidden;
        private Visibility correctAnswerVisibility = Visibility.Hidden;

        private int timeoutStart, timeoutEnd;

        public event EventHandler OnQuestionAppeared;
        public event EventHandler OnRoundResult;

        public MainViewModel()
        {
            game = new Game(new ArithmeticRound());
            game.OnQuestionAppeared += Game_OnQuestionAppeared;
            game.OnRoundResult += Game_OnRoundResult;
        }

        private void Game_OnQuestionAppeared(object sender, RoundEventArgs e)
        {
            this.Question = e.Request;
            OnQuestionAppeared(this, null);
            while (this.Answer == null || this.Answer == "") { }
            e.Response = this.Answer;
        }

        private void Game_OnRoundResult(object sender, RoundEventArgs e)
        {
            
            this.QuestionVisibility = Visibility.Hidden;
            this.AnswerVisibility = Visibility.Hidden;

            if (e.Result)
                this.VFigureVisibility = Visibility.Visible;
            else
            {
                this.CorrectAnswer = e.Response;
                this.XFigureVisibility = Visibility.Visible;
                this.CorrectAnswerVisibility = Visibility.Visible;
            }
            Thread.Sleep(2500);
            this.CorrectAnswer = "";
            this.Answer = "";
            this.OnRoundResult(this, null);
            this.VFigureVisibility = Visibility.Hidden;
            this.XFigureVisibility = Visibility.Hidden;
            this.CorrectAnswerVisibility = Visibility.Hidden;
            this.QuestionVisibility = Visibility.Visible;
            this.AnswerVisibility = Visibility.Visible;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DifficultyLevel GameMode
        {
            set { difficultyLevel = value; game.Setup(value); }
            get { return difficultyLevel; }
        }
        public bool IsPlaying
        {
            set { isPlaying = value; if (value) game.Start(); else game.Stop(); }
            get { return isPlaying; }
        }

        public string Question
        {
            set { question = value; OnPropertyChanged("Question");}
            get { return question; }
        }

        public string CorrectAnswer
        {
            set { correctAnswer = value; OnPropertyChanged("CorrectAnswer"); }
            get { return correctAnswer; }
        }

        public string Answer
        {
            set;
            get;
        }

        public Visibility QuestionVisibility
        {
            set { questionVisibility = value; OnPropertyChanged("QuestionVisibility"); }
            get { return questionVisibility; }
        }

        public Visibility AnswerVisibility
        {
            set { answerVisibility = value; OnPropertyChanged("AnswerVisibility"); }
            get { return answerVisibility; }
        }

        public Visibility XFigureVisibility
        {
            set { xFigureVisibility = value; OnPropertyChanged("XFigureVisibility"); }
            get { return xFigureVisibility; }
        }

        public Visibility VFigureVisibility
        {
            set { vFigureVisibility = value; OnPropertyChanged("VFigureVisibility"); }
            get { return vFigureVisibility; }
        }

        public Visibility CorrectAnswerVisibility
        {
            set { correctAnswerVisibility = value; OnPropertyChanged("CorrectAnswerVisibility"); }
            get { return correctAnswerVisibility; }
        }

        public int TimeoutStart
        {
            set { timeoutStart = value; game.TimeoutStart = timeoutStart * 60000; OnPropertyChanged("TimeoutStart"); }
            get { return timeoutStart; }
        }

        public int TimeoutEnd
        {
            set { timeoutEnd = value; game.TimeoutEnd = timeoutEnd * 60000; OnPropertyChanged("TimeoutEnd"); }
            get { return timeoutEnd; }
        }
    }
}
