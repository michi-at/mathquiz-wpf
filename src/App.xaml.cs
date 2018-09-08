namespace MathQuizWPF
{
    using System;
    using System.Windows;
    using System.Globalization;
    using WinForms = System.Windows.Forms;
    using System.Threading;
    using MathQuizWPF.ViewModel;

    public partial class App : Application
    {
        private WinForms.NotifyIcon icon;
        private MainWindow mainWindow;
        private MathQuizWPF.View.QuestionPage questionPage;
        private MainViewModel<ArithmeticRound> viewModel;
        const string appName = "MathQuiz";


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            mainWindow = new MainWindow();
            viewModel = new MainViewModel<ArithmeticRound>();
            questionPage = new View.QuestionPage
            {
                DataContext = viewModel
            };

            // Setting culture to en-GB
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-GB");

            Game<ArithmeticRound>.Instance.OnQuestionAppeared += (sender, args) =>
                                                                 {
                                                                     viewModel.Question = args.Request;
                                                                     Dispatcher.Invoke(() => mainWindow.Show());
                                                                     while(viewModel.Answer == null || viewModel.Answer == ""){}
                                                                     args.Response = viewModel.Answer;
                                                                 };
            Game<ArithmeticRound>.Instance.OnRoundResult += (sender, args) => 
                                                            {
                                                                Dispatcher.Invoke(() =>
                                                                {
                                                                    questionPage.questionLabel.Visibility = Visibility.Hidden;
                                                                    questionPage.answerTextBox.Visibility = Visibility.Hidden;
                                                                });
                                                                if (args.Result)
                                                                    Dispatcher.Invoke(() => questionPage.vFigure.Visibility = Visibility.Visible);
                                                                else
                                                                {
                                                                    viewModel.CorrectAnswer = args.Response;
                                                                    Dispatcher.Invoke(() =>
                                                                    {
                                                                        questionPage.xFigure.Visibility = Visibility.Visible;
                                                                        questionPage.correctAnswerLabel.Visibility = Visibility.Visible;
                                                                    });
                                                                }
                                                                Thread.Sleep(2500);
                                                                viewModel.CorrectAnswer = "";
                                                                viewModel.Answer = "";
                                                                Dispatcher.Invoke(() =>
                                                                {
                                                                    mainWindow.Hide();
                                                                    questionPage.vFigure.Visibility = Visibility.Hidden;
                                                                    questionPage.xFigure.Visibility = Visibility.Hidden;
                                                                    questionPage.correctAnswerLabel.Visibility = Visibility.Hidden;
                                                                    questionPage.questionLabel.Visibility = Visibility.Visible;
                                                                    questionPage.answerTextBox.Visibility = Visibility.Visible;
                                                                });
                                                            };
            InitializeIcon();
            mainWindow.Navigate(questionPage);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            icon.Visible = false;
            icon.Dispose();
            mainWindow.Close();
        }
    }
}
