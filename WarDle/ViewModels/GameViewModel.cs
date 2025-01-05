using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using WarDle.Models;

namespace WarDle.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly ResultsViewModel resultsViewModel;

        //Variables
        private readonly HttpClient httpClient;
        private const string WordsUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
        private const string LocalFileName = "words.txt";
        private const string GameStateFileName = "gamestate.json";
        private List<string> words;
        private int currentRowIndex;
        public ObservableCollection<Game> Rows { get; }

        // Static property to temporarily hold the player's name
        public static string TempPlayerName { get; set; }

        private string playerName;
        public string PlayerName
        {
            get => playerName;
            set
            {
                if (playerName != value)
                {
                    playerName = value;                    
                }
            }
        }

        private string randomWord;
        public string RandomWord
        {
            get => randomWord;
            set
            {
                if (randomWord != value)
                {
                    randomWord = value;
                    SplitRandomWord();
                }
            }   
        }

        private char[] randomWordChars;
        public char[] RandomWordChars
        {
            get => randomWordChars;
            private set
            {
                if (randomWordChars != value)
                {
                    randomWordChars = value;
                }
            }
        }

        private int score;
        public int Score
        {
            get => score;
            set
            {
                if (score != value)
                {
                    score = value;
                    OnPropertyChanged(nameof(Score));
                }
            }
        }

        private int wordsGuessed;
        public int WordsGuessed
        {
            get => wordsGuessed;
            set
            {
                if (wordsGuessed != value)
                {
                    wordsGuessed = value;
                    OnPropertyChanged(nameof(WordsGuessed));
                }
            }
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        public Command SubmitCommand { get; }
        public Command NewGameCommand { get; }

        //Constructor
        public GameViewModel()
        {
            resultsViewModel = new ResultsViewModel();
            PlayerName = TempPlayerName;
            httpClient = new HttpClient();
            Rows = new ObservableCollection<Game>
            {
                new Game(),
                new Game(),
                new Game(),
                new Game(),
                new Game(),
                new Game()

            };
            SubmitCommand = new Command(CheckEnteredWord);
            NewGameCommand = new Command(NewGame);
            Rows[0].EntriesReadOnly[0] = false;
            OnPropertyChanged("Rows");
        }

        //Methods

        public void OnEntryTextChanged(int rowIndex, int colIndex, string newText)
        {
           
            var row = Rows[rowIndex];

            if (!string.IsNullOrEmpty(newText) && colIndex < row.EntriesReadOnly.Count - 1)
            {
                row.EntriesReadOnly[colIndex + 1] = false;
                row.OnPropertyChanged("EntriesReadOnly");

                // Set focus to the next entry
                FocusNextEntry(rowIndex, colIndex + 1);
            }
        }

        public event Action<int, int> FocusNextEntry = delegate { };

        public async Task InitializeWordsAsync()
        {
            try
            {   
                if (!File.Exists(GetLocalFilePath()))
                {
                    await DownloadWordsAsync();
                }

                await LoadWordsFromFileAsync();
                SelectRandomWord();
                
                //DEBUG ONLY-----------------------------
                System.Diagnostics.Debug.WriteLine("Random Word: "+RandomWord);
                //---------------------------------------

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to initialize words: {ex.Message}", "OK");
            }
        }

        private async Task DownloadWordsAsync()
        {
            try
            {
                var response = await httpClient.GetStringAsync(WordsUrl);
                var filePath = GetLocalFilePath();

                File.WriteAllText(filePath, response);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to download words file", ex);
            }
        }

        
        private string GetLocalFilePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, LocalFileName);
        }

        
        private string GetGameStateFilePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, GameStateFileName);
        }

        private async Task LoadWordsFromFileAsync()
        {
            try
            {
                var filePath = GetLocalFilePath();
                var wordsContent = await File.ReadAllTextAsync(filePath);
                
                words = wordsContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load words from file", ex);
            }
        }

        private void SelectRandomWord()
        {
            if (words == null || !words.Any())
            {
                throw new Exception("Word list is empty. Cannot select a random word.");
            }
            RandomWord = words[new Random().Next(words.Count)].ToUpper();
        }

        public void SplitRandomWord()
        {
            if (!string.IsNullOrEmpty(RandomWord))
            {
                RandomWordChars = RandomWord.ToCharArray();
            }
        }

        public async void CheckEnteredWord()
        {
            if (currentRowIndex < Rows.Count)
            {
                var row = Rows[currentRowIndex];
                string userWord = new string(row.Letters.ToArray());

                if (userWord.Length < 5 || userWord.Contains('\0'))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter 5 letters in the current row.", "OK");
                    return;
                }

                // Reset the background colors for the current row
                for (int i = 0; i < row.EntryBackgroundColors.Count; i++)
                {
                    row.EntryBackgroundColors[i] = "Gray";
                }

                bool isCorrectWord = true;
                int roundScore = 0;

                // Compare the entered word with the random word
                for (int i = 0; i < userWord.Length; i++)
                {
                    if (RandomWordChars[i] == userWord[i])
                    {
                        row.EntryBackgroundColors[i] = "Green"; // Correct letter and position
                        roundScore += 2;
                    }
                    else if (RandomWord.Contains(userWord[i]))
                    {
                        row.EntryBackgroundColors[i] = "Orange"; // Correct letter, wrong position
                        roundScore += 1;
                        isCorrectWord = false;
                    }
                    else
                    {
                        row.EntryBackgroundColors[i] = "Red"; // Incorrect letter
                        isCorrectWord = false;
                    }
                }

                Score += roundScore;

                // Notify the UI to update the background colors for the current row
                row.OnPropertyChanged(nameof(row.EntryBackgroundColors));

                if (isCorrectWord)
                {
                    await Shell.Current.DisplayAlert("Well done!", "You've guessed the word correctly!", "OK");
                    WordsGuessed++;
                    await SaveResultAsync();
                    StartNewGame(true); // Start a new game, carrying over the score
                    return;
                }

                // Make the current row read-only
                for (int i = 0; i < row.EntriesReadOnly.Count; i++)
                {
                    row.EntriesReadOnly[i] = true;
                }
                row.OnPropertyChanged(nameof(row.EntriesReadOnly));

                // Move to the next row if available, otherwise end the game
                if (currentRowIndex < Rows.Count - 1)
                {
                    currentRowIndex++;
                    Rows[currentRowIndex].EntriesReadOnly[0] = false;// Enable the first entry in the new row
                    FocusNextEntry(currentRowIndex,0);// Set focus to the first entry in the new row
                    Rows[currentRowIndex].OnPropertyChanged(nameof(row.EntriesReadOnly));
                }
                else
                {
                    await Shell.Current.DisplayAlert("Bad luck!", $"You've run out of tries. The word was: {RandomWord}", "OK");
                    await SaveResultAsync();
                    StartNewGame(false); // Start a new game, resetting the score
                }
            }

            await SaveGameStateAsync();// Save the game state
        }

        public async void StartNewGame(bool carryOverScore)
        {
            if (!carryOverScore)
            {
                Score = 0;
                WordsGuessed = 0;
            }

            // Reset the rows
            Rows.Clear();
            for (int i = 0; i < 6; i++)
            {
                Rows.Add(new Game());
            }

            currentRowIndex = 0;
            Rows[0].EntriesReadOnly[0] = false; // Enable the first entry of the first row

            // Select a new random word
            SelectRandomWord();
            await InitializeWordsAsync();

            OnPropertyChanged(nameof(Rows));

            await SaveGameStateAsync();// Save the game state
        }

        public async void NewGame()
        {
            bool answer = await Shell.Current.DisplayAlert("Start New Game?", "Would you like to start new game?", "Yes", "Cancel");

            if (answer == true)
            {
                // Ask the player for their name
                string result = await Shell.Current.DisplayPromptAsync("Enter Name", "Please enter your name:");
                if (!string.IsNullOrEmpty(result))
                {
                    PlayerName = result;                    
                }
                OnPropertyChanged(nameof(PlayerName));
                StartNewGame(false);// Start a new game, resetting the score
            }
            else { }
        }

        private async Task SaveResultAsync()
        {
            var result = new PlayerResult
            {
                PlayerName = PlayerName,
                Score = Score,
                WordsGuessed = WordsGuessed
            };
            await resultsViewModel.AddResultAsync(result);
        }

        public async Task SaveGameStateAsync()
        {
            var gameState = new GameState
            {
                PlayerName = PlayerName,
                Score = Score,
                WordsGuessed = WordsGuessed,
                RandomWord = RandomWord,
                Grid = Rows.Select(row => row.Letters.Select((letter, index) => new GameEntry
                {
                    Letter = letter.ToString(),
                    IsReadOnly = row.EntriesReadOnly[index],
                    BackgroundColor = row.EntryBackgroundColors[index]
                }).ToList()).ToList()
            };

            var json = JsonSerializer.Serialize(gameState);
            var filePath = GetGameStateFilePath();
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task LoadGameStateAsync()
        {
            var filePath = GetGameStateFilePath();
            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath);
            var gameState = JsonSerializer.Deserialize<GameState>(json);

            PlayerName = gameState.PlayerName;
            Score = gameState.Score;
            WordsGuessed = gameState.WordsGuessed;
            RandomWord = gameState.RandomWord;
            Rows.Clear();
            foreach (var row in gameState.Grid)
            {
                var gameRow = new Game();
                for (int i = 0; i < row.Count; i++)
                {
                    gameRow.Letters[i] = row[i].Letter[0];
                    gameRow.EntriesReadOnly[i] = row[i].IsReadOnly;
                    gameRow.EntryBackgroundColors[i] = row[i].BackgroundColor;
                }
                Rows.Add(gameRow);
            }
            OnPropertyChanged(nameof(Rows));
            OnPropertyChanged(nameof(PlayerName));
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
