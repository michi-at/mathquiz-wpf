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
        private SettingsWindow settingsWindow;
        private MathQuizWPF.View.QuestionPage questionPage;
        private MathQuizWPF.View.TimeoutSettingsPage timeoutSettingsPage;
        private MainViewModel viewModel;
        const string appName = "MathQuiz";


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            mainWindow = new MainWindow();
            settingsWindow = new SettingsWindow();
            viewModel = new MainViewModel();
            questionPage = new View.QuestionPage()
            {
                DataContext = viewModel
            };
            timeoutSettingsPage = new View.TimeoutSettingsPage()
            {
                DataContext = viewModel
            };

            // Setting culture to en-GB
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-GB");

            viewModel.OnQuestionAppeared += ViewModel_OnQuestionAppeared;
            viewModel.OnRoundResult += ViewModel_OnRoundResult;

            InitializeIcon();
            mainWindow.Navigate(questionPage);
            settingsWindow.Navigate(timeoutSettingsPage);
        }

        private void ViewModel_OnRoundResult(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => mainWindow.Hide());
        }

        private void ViewModel_OnQuestionAppeared(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => 
            {
                mainWindow.Show();
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            icon.Visible = false;
            icon.Dispose();
            mainWindow.Close();
        }
    }
}
