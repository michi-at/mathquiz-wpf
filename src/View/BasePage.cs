namespace MathQuizWPF.View
{
    using System.Windows.Controls;
    using System.Windows.Input;

    public class BasePage : Page
    {
        protected virtual void OnXClick(object sender, MouseButtonEventArgs e)
        {
            (this.Parent as MainWindow).Hide();
        }
    }
}
