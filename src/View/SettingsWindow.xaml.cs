namespace MathQuizWPF
{
    using System.Windows.Input;
    using System.Windows.Navigation;

    /// <summary>
    /// Interaction logic for TimeoutOptionsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : NavigationWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            base.DragMove();
        }
    }
}
