using System.Collections.ObjectModel;
using System.ComponentModel;
using WarDle.Models;

namespace WarDle.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
       
        //Variables
        private readonly HttpClient httpClient;
        private const string WordsUrl = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
        private const string LocalFileName = "words.txt";
        private List<string> words;
        private int currentRowIndex;
        public ObservableCollection<Game> Rows { get; }


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

        public event PropertyChangedEventHandler? PropertyChanged;
        public Command SubmitCommand { get; }

        //Constructor
        public GameViewModel()
        {
            httpClient = new HttpClient();
            Rows = new ObservableCollection<Game>
            {
                new Game()
                
            };
            SubmitCommand = new Command(CheckEnteredWord);
            Rows[0].EntriesReadOnly[0] = false;
            OnPropertyChanged("Rows");
        }

        //Methods

        public void OnEntryTextChanged(int rowIndex, int colIndex, string newText)
        {
            if (rowIndex != currentRowIndex)
                return; // Ignore changes in non-current rows

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

        public void CheckEnteredWord()
        {
            if (currentRowIndex < Rows.Count)
            {
                var row = Rows[currentRowIndex];
                string userWord = new string(row.Letters.ToArray());

                // Reset the background colors for the current row
                for (int i = 0; i < row.EntryBackgroundColors.Count; i++)
                {
                    row.EntryBackgroundColors[i] = "White";
                }

                // Compare the entered word with the random word
                bool isCorrectWord = true;
                for (int i = 0; i < userWord.Length; i++)
                {
                    if (RandomWordChars[i] == userWord[i])
                    {
                        row.EntryBackgroundColors[i] = "Green"; // Correct letter and position
                    }
                    else if (RandomWord.Contains(userWord[i]))
                    {
                        row.EntryBackgroundColors[i] = "Gray"; // Correct letter, wrong position
                        isCorrectWord = false;
                    }
                    else
                    {
                        row.EntryBackgroundColors[i] = "Red"; // Incorrect letter
                        isCorrectWord = false;
                    }
                }

                // Notify the UI to update the background colors for the current row
                row.OnPropertyChanged("EntryBackgroundColors");

                if (isCorrectWord)
                {
                    // Start the next round if the word is correct
                    if (currentRowIndex < Rows.Count - 1)
                    {
                        currentRowIndex++;
                        Rows[currentRowIndex].EntriesReadOnly[0] = false; // Enable the first entry in the new row
                        Rows[currentRowIndex].OnPropertyChanged("EntriesEnabled");
                    }
                }
                else if (currentRowIndex < Rows.Count - 1)
                {
                    currentRowIndex++;
                    Rows[currentRowIndex].EntriesReadOnly[0] = false; // Enable the first entry in the new row
                    Rows[currentRowIndex].OnPropertyChanged("EntriesEnabled");
                }
                else
                {
                    // Game over logic
                    // You can add your game over handling code here
                }

                // Disable all entries in the previous row
                for (int i = 0; i < row.EntriesReadOnly.Count; i++)
                {
                    row.EntriesReadOnly[i] = true;
                }
                row.OnPropertyChanged("EntriesEnabled");
            }
        }


        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
