using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace SnakeMobile.Domain.Model
{
    public class Game : INotifyPropertyChanged
    {
        public GameBoard GameBoard { get; set; }

        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                RaisePropertyChanged(nameof(Score));
            }
        }

        private DateTime startTime;
        private DateTime endTime;
        public TimeSpan Duration
        {
            get => endTime - startTime;
        }

        public Game()
        {
            GameBoard = new GameBoard();
        }

        public async Task<GameResults> StartGameLoop()
        {
            startTime = DateTime.UtcNow;

            do
            {
                await GameBoard.MoveSnake();

                Score = GameBoard.Snake.CountPelletsConsumed;

                System.Threading.Thread.Sleep(200);
            } while (!GameBoard.IsInIllegalState);

            endTime = DateTime.UtcNow;

            GameResults results = new GameResults(Score, Duration);

            return results;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
