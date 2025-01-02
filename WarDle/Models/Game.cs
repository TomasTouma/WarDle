

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WarDle.Models
{
    public class Game : INotifyPropertyChanged
    {
        public ObservableCollection<char> Letters { get; set; }
        public ObservableCollection<bool> EntriesReadOnly { get; set; }
        public ObservableCollection<string> EntryBackgroundColors { get; set; }

        public Game()
        {
            Letters = new ObservableCollection<char> { '\0', '\0', '\0', '\0', '\0' };
            EntriesReadOnly = new ObservableCollection<bool> { true, true, true, true, true };
            EntryBackgroundColors = new ObservableCollection<string> { "White", "White", "White", "White", "White" };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
