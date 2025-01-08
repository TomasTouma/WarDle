

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using WarDle.Models;

namespace WarDle.ViewModels
{
    public class ResultsViewModel : INotifyPropertyChanged
    {
        private const string ResultsFileName = "results.json";
        private readonly string resultsFilePath;

        //Collection to hold the results
        public ObservableCollection<PlayerResult> Results { get; }

        //Constructor
        public ResultsViewModel()
        {
            resultsFilePath = Path.Combine(FileSystem.AppDataDirectory, ResultsFileName);
            Results = new ObservableCollection<PlayerResult>();
            LoadResultsAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //Method to load the results 
        private async void LoadResultsAsync()
        {
            var results = await LoadResultsFromFileAsync();
            foreach (var result in results)
            {
                Results.Add(result);
            }
        }

        //Method to load the results from the file
        private async Task<List<PlayerResult>> LoadResultsFromFileAsync()
        {
            if (!File.Exists(resultsFilePath))
                return new List<PlayerResult>();

            var json = await File.ReadAllTextAsync(resultsFilePath);
            return JsonSerializer.Deserialize<List<PlayerResult>>(json);
        }

        //Method to save the results to the file
        private async Task SaveResultsToFileAsync(List<PlayerResult> results)
        {
            var json = JsonSerializer.Serialize(results);
            await File.WriteAllTextAsync(resultsFilePath, json);
        }

        //Method to add a result and save it to the file
        public async Task AddResultAsync(PlayerResult result)
        {
            var results = await LoadResultsFromFileAsync();
            results.Add(result);
            await SaveResultsToFileAsync(results);
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
