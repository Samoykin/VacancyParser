namespace VacancyParser
{
    using System.Windows;

    using View;
    using ViewModel;

    /// <summary>Interaction logic for App.xaml.</summary>
    public partial class App : Application
    {
        /// <summary>Initializes a new instance of the <see cref="App" /> class.</summary>
        public App()
        {
            var mw = new MainWindowView
            {
                DataContext = new MainViewModel()
            };

            mw.Show();
        }
    }
}