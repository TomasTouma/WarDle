

namespace WarDle
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("GamePage", typeof(GamePage));
            Routing.RegisterRoute("ResultsPage", typeof(ResultsPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));


        }
    }
}
