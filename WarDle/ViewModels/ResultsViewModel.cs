

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

        public ObservableCollection<PlayerResult> Results { get; }

        public ResultsViewModel()
        {
            resultsFilePath = Path.Combine(FileSystem.AppDataDirectory, ResultsFileName);
            Results = new ObservableCollection<PlayerResult>();
            LoadResultsAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void LoadResultsAsync()
        {
            var results = await LoadResultsFromFileAsync();
            foreach (var result in results)
            {
                Results.Add(result);
            }
        }

        private async Task<List<PlayerResult>> LoadResultsFromFileAsync()
        {
            if (!File.Exists(resultsFilePath))
                return new List<PlayerResult>();

            var json = await File.ReadAllTextAsync(resultsFilePath);
            return JsonSerializer.Deserialize<List<PlayerResult>>(json);
        }

        private async Task SaveResultsToFileAsync(List<PlayerResult> results)
        {
            var json = JsonSerializer.Serialize(results);
            await File.WriteAllTextAsync(resultsFilePath, json);
        }

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
