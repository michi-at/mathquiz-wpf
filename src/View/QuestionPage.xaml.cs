namespace MathQuizWPF.View
{
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using MathQuizWPF.ViewModel;

    /// <summary>
    /// Interaction logic for QuestionPage.xaml
    /// </summary>
    public partial class QuestionPage : BasePage
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        public QuestionPage()
        {
            InitializeComponent();
        }

        private void PreviewAnswerInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void PastingAnswer(object sender, System.Windows.DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void EnteringAnswer(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainViewModel context = (MainViewModel)DataContext;
                context.Answer = this.answerTextBox.Text;
                this.answerTextBox.Text = "";
            }
        }
    }
}
