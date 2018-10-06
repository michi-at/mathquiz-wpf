namespace MathQuizWPF
{
    using System;
    using System.Windows;
    using MathQuizWPF.View;
    using System.Windows.Input;
    using System.Windows.Navigation;

    public partial class MainWindow : NavigationWindow, IView
    {
        Random r = new Random();
        bool isShown = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            base.DragMove();
        }

        public new void Show()
        {
            double width = SystemParameters.WorkArea.Width;
            double height = SystemParameters.WorkArea.Height;

            if (isShown)
            {
                if (this.Left + ActualWidth >= width)
                    this.Left = width - ActualWidth;
                if (this.Top + ActualHeight >= height)
                    this.Top = height - ActualWidth;
            }
            else
            {
                if (this.ActualHeight == 0 || this.ActualWidth == 0)
                {
                    this.Left = r.Next((int)(width / 2));
                    this.Top = r.Next((int)(height - Height));
                }
                else
                {
                    this.Left = r.Next((int)(width - this.ActualWidth));
                    this.Top = r.Next((int)(height - this.ActualHeight));
                }
            }
            base.Show();
            isShown = true;
        }

        public new void Hide()
        {
            isShown = false;
            base.Hide();
        }
    }
}
