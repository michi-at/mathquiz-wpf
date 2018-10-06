namespace MathQuizWPF
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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

    public sealed class Game : IDisposable
    {

        private GameRound round;
        private DifficultyLevel mode;
        private GameState state;
        private CancellationTokenSource cancellationTokenSource;
        public event EventHandler<RoundEventArgs> OnQuestionAppeared;
        public event EventHandler<RoundEventArgs> OnRoundResult;
        private Random r;

        private int timeoutStart;
        public int TimeoutStart
        {
            set
            {
                timeoutStart = (value < 0) ? 0 : value;
            }
            get
            {
                return timeoutStart;
            }
        }
        private int timeoutEnd;
        public int TimeoutEnd
        {
            set
            {
                timeoutEnd = (value < 0) ? 0 : (value == TimeoutStart) ? ++value : value;
            }
            get
            {
                return timeoutEnd;
            }
        }

        public Game(GameRound round)
        {
            r = new Random();
            this.round = round;
            TimeoutStart = 0;
            TimeoutEnd = 10000;
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
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
                Task.Factory.StartNew(GameProcess, cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            }
        }

        private void GameProcess(object ct)
        {
            var cancellationToken = (CancellationToken)ct;
            cancellationToken.Register(() => { state = GameState.Ready; });

            while (!cancellationToken.IsCancellationRequested)
            {
                round.Setup(this.mode);
                string answer = FireOnQuestionEvent(round.GetQuestion());
                bool? isCorrect = round.CheckAnswer(answer);
                while (isCorrect == null)
                {
                    answer = FireOnQuestionEvent(round.GetQuestion());
                    isCorrect = round.CheckAnswer(answer);
                }
                FireOnRoundResult(isCorrect.Value, round.GetAnswer());
                Thread.Sleep(r.Next(TimeoutStart, TimeoutEnd));
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
