namespace VacancyParser.View
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Navigation;

    /// <summary>Interaction logic for MainWindow.xaml.</summary>
    public partial class MainWindowView : Window
    {
        /// <summary>Initializes a new instance of the <see cref="MainWindowView" /> class.</summary>
        public MainWindowView()
        {
            this.InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}