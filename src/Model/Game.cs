namespace MathQuizWPF
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Threading;

    enum GameState
    {
        Ready,
        Running
    }

    public class RoundEventArgs : EventArgs
    {
        public string Request { get; }
        public string Response { get; set; }
        public bool Result { get; }

        public RoundEventArgs(string question)
        {
            this.Request = question;
        }
        public RoundEventArgs(bool result, string response)
        {
            this.Result = result;
            this.Response = response;
        }
    }

    public sealed class Game<T> : IDisposable where T : GameRound, new()
    {
        public static Game<T> Instance { get; } = new Game<T>(); // 1 game per round type - bad

        private GameRound round;
        private DifficultyLevel mode;
        private GameState state;
        private CancellationTokenSource cancellationTokenSource;
        public event EventHandler<RoundEventArgs> OnQuestionAppeared;
        public event EventHandler<RoundEventArgs> OnRoundResult;
        private Random r;

        static Game(){}

        private Game()
        {
            //state = GameState.WaitingForAnswer;
            r = new Random();
        }

        public void Setup(DifficultyLevel mode)
        {
            this.mode = mode;
        }

        public void Start()
        {
            if (this.state == GameState.Ready)
            {
                this.state = GameState.Running;
                //Console.WriteLine($"{GameState.Ready} => {GameState.Running}");
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(GameProcess, cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            }
        }

        private void GameProcess(object ct)
        {
            var cancellationToken = (CancellationToken)ct;
            cancellationToken.Register(() => { state = GameState.Ready; /*Console.WriteLine($"{GameState.Running} => {GameState.Ready}");*/ });

            while (!cancellationToken.IsCancellationRequested)
            {
                round = new T();
                round.Setup(this.mode);
                string answer = FireOnQuestionEvent(round.GetQuestion());
                bool? isCorrect = round.CheckAnswer(answer);
                while (isCorrect == null)
                {
                    answer = FireOnQuestionEvent(round.GetQuestion());
                    isCorrect = round.CheckAnswer(answer);
                }
                FireOnRoundResult(isCorrect.Value, round.GetAnswer());
                Thread.Sleep(r.Next(30000, 60000)); // TODO: timeout options
            }
        }

        public void Stop()
        {
            if (state != GameState.Ready)
                cancellationTokenSource?.Cancel();
        }

        private string FireOnQuestionEvent(string question)
        {
            RoundEventArgs e = new RoundEventArgs(question);
            OnQuestionAppeared(this, e);

            return e.Response;
        }

        private void FireOnRoundResult(bool isCorrect, string correctAnswer)
        {
            RoundEventArgs e = new RoundEventArgs(isCorrect, correctAnswer);
            OnRoundResult(this, e);
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
        }
    }
}
