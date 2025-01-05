﻿
using System.ComponentModel;

namespace WarDle.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {

        public Command ContinueCommand { get; }
        public Command StartNewCommand { get; }
        public Command SettingsCommand { get; }
        public Command ResultsCommand { get; }
        public Command ExitCommand { get; }

        public MainPageViewModel()
        {
            ContinueCommand = new Command(ContinueGame);
            StartNewCommand = new Command(StartNewGame);
            SettingsCommand = new Command(OpenSettings);
            ResultsCommand = new Command(OpenResults);
            ExitCommand = new Command(ExitGame);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async void ContinueGame()
        {

            var gameStateFilePath = Path.Combine(FileSystem.AppDataDirectory, "gamestate.json");

            // Check if the saved game file exists
            if (!File.Exists(gameStateFilePath))
            {
                await App.Current.MainPage.DisplayAlert("Error", "No saved game found.", "OK");
                return;
            }

            await Shell.Current.GoToAsync("GamePage");

            // Load the game state after navigating to the GamePage
            var currentPage = Shell.Current.CurrentPage;
            if (currentPage is GamePage gamePage)
            {
                await gamePage.LoadGameStateAsync();
            }
        }

        private async void StartNewGame()
        {
            string playerName = await App.Current.MainPage.DisplayPromptAsync("Enter Name", "Please enter your name:");
            if (!string.IsNullOrEmpty(playerName))
            {
                GameViewModel.TempPlayerName = playerName;               
                await Shell.Current.GoToAsync("GamePage");
            }
        }

        private async void OpenSettings()
        {
            await Shell.Current.GoToAsync("SettingsPage");
        }

        private async void OpenResults()
        {
            await Shell.Current.GoToAsync("ResultsPage");
        }

        private void ExitGame()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
