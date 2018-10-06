namespace MathQuizWPF.View
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for TimeoutSettingsPage.xaml
    /// </summary>
    public partial class TimeoutSettingsPage : BasePage
    {
        public TimeoutSettingsPage()
        {
            InitializeComponent();
            timeoutStart.SmallChange = 1;
            timeoutStart.LargeChange = 1;
            timeoutEnd.SmallChange = 1;
            timeoutEnd.LargeChange = 1;
            timeoutStart.Maximum = timeoutEnd.Maximum = 60;
        }

        private void Timeout_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            int newVal = (int)e.NewValue;
            slider.Value = newVal;
            if (timeoutEnd.Value < timeoutStart.Value)
                timeoutEnd.Value = timeoutStart.Value;
        }
    }
}
