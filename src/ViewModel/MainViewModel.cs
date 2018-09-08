

namespace MathQuizWPF.ViewModel
{
    using System.ComponentModel;
    using System.Windows;

    public class MainViewModel<T> : INotifyPropertyChanged where T : GameRound, new()
    {
        private DifficultyLevel difficultyLevel;
        private bool isPlaying;
        private string question;
        private string answer;
        private string correctAnswer;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DifficultyLevel GameMode
        {
            set { difficultyLevel = value; Game<T>.Instance.Setup(value); }
            get { return difficultyLevel; }
        }
        public bool IsPlaying
        {
            set { isPlaying = value; if (value) Game<T>.Instance.Start(); else Game<T>.Instance.Stop(); }
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
            set { answer = value;  }
            get { return answer; }
        }
    }
}
