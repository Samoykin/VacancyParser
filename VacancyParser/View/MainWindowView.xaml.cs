namespace VacancyParser.View
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Navigation;
    using NLog;

    /// <summary>Interaction logic for MainWindow.xaml.</summary>
    public partial class MainWindowView : Window
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>Initializes a new instance of the <see cref="MainWindowView" /> class.</summary>
        public MainWindowView()
        {
            this.InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                this.logger.Error(ex.Message);
            }
        }
    }
}