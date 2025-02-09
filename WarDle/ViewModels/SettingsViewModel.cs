﻿

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WarDle.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool isDarkMode;


        //Property to store the current theme preference
        public bool IsDarkMode
        {
            get => isDarkMode;
            set
            {
                if (isDarkMode != value)
                {
                    isDarkMode = value;
                    OnPropertyChanged();
                    UpdateTheme();
                    SaveThemePreference();
                }
            }
        }

        public Command ShowRulesCommand { get; }

        //Constructor
        public SettingsViewModel()
        {
            
            ShowRulesCommand = new Command(ShowRules);
            
            LoadThemePreference();
        }

        //Method to update the theme based on the current theme preference
        private void UpdateTheme()
        {
            Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        }

        //Method to save the theme preference
        private void SaveThemePreference()
        {
            Preferences.Set("IsDarkMode", isDarkMode);
        }

        //Method to load the theme preference
        private void LoadThemePreference()
        {
            IsDarkMode = Preferences.Get("IsDarkMode", false);
        }

        //Method to show the rules of the game
        private async void ShowRules()
        {
            string message = "Rules and Scoring:\n\n" +
                             "1. Objective:\n" +
                             "   - The objective of the game is to guess the random 5-letter word within 6 attempts.\n\n" +
                             "2. Scoring:\n" +
                             "   - Each correct letter in the correct position scores 2 points.\n" +
                             "   - Each correct letter in the wrong position scores 1 point.\n" +
                             "   - Incorrect letters score 0 points.\n\n" +
                             "3. How to Play:\n" +
                             "   - Enter your guess in the grid. Each row represents one attempt.\n" +
                             "   - Press the \"Submit\" button to check your guess.\n" +
                             "   - The colors of the entries will change based on the accuracy of your guess:\n" +
                             "     - Green: Correct letter in the correct position.\n" +
                             "     - Gray: Correct letter in the wrong position.\n" +
                             "     - Red: Incorrect letter.\n\n" +
                             "Good luck and have fun!";

            await Application.Current.MainPage.DisplayAlert("Rules and Scoring", message, "OK");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
