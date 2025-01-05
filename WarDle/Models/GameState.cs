

namespace WarDle.Models
{
    public class GameState
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public int WordsGuessed { get; set; }
        public string RandomWord { get; set; }
        public List<List<GameEntry>> Grid { get; set; }
    }

    public class GameEntry
    {
        public string Letter { get; set; }
        public bool IsReadOnly { get; set; }
        public string BackgroundColor { get; set; }
    }
}
