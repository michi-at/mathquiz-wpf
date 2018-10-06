namespace MathQuizWPF
{
    using System;
    using System.Windows;
    using WinForms = System.Windows.Forms;
    using MathQuizWPF.ViewModel;

    public partial class App : Application
    {
        private WinForms.NotifyIcon icon;
        private MainWindow mainWindow;
        private SettingsWindow settingsWindow;
        private View.QuestionPage questionPage;
        private View.TimeoutSettingsPage timeoutSettingsPage;
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
            
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-GB");
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en-GB");

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
