
using System.ComponentModel;

namespace WarDle.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        public Command ContinueCommand { get; }
        public Command StartNewCommand { get; }
        public Command SettingsCommand { get; }
        public Command ExitCommand { get; }

        public MainPageViewModel()
        {
            ContinueCommand = new Command(ContinueGame);
            StartNewCommand = new Command(StartNewGame);
            SettingsCommand = new Command(OpenSettings);
            ExitCommand = new Command(ExitGame);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async void ContinueGame()
        {
            await Shell.Current.GoToAsync("///GamePage");
        }

        private async void StartNewGame()
        {
            await Shell.Current.GoToAsync("///GamePage");

        }

        private async void OpenSettings()
        {
            await App.Current.MainPage.DisplayAlert("Settings", "Settings page coming soon!", "OK");
        }

        private void ExitGame()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
