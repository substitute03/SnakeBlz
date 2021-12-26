using System;

namespace SnakeBlz.Domain
{
    public class GameResults
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public TimeSpan Duration { get; set; }

        private GameResults() { }
        public GameResults(int score, TimeSpan duration)
        {
            Score = score;
            Duration = duration;
        }
    }
}
