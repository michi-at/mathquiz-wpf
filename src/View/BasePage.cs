namespace MathQuizWPF.View
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;

    public class BasePage : Page
    {
        protected virtual void OnXClick(object sender, MouseButtonEventArgs e)
        {
            (this.Parent as NavigationWindow).Hide();
        }
    }
}
