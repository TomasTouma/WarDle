namespace WarDle
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Apply the saved theme preference on startup
            var isDarkMode = Preferences.Get("IsDarkMode", false);
            Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;

            MainPage = new AppShell();
        }
    }
}
