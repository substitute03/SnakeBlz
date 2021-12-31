using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBlz.Domain
{
    public class Enums
    {
        public enum CellType
        {
            Empty,
            Snake,
            BlazingSnake,
            Pellet
        }

        public enum GameState
        {
            PreGame,
            Setup,
            InProgress,
            GameOver
        }

        public enum GameMode
        {
            Classic,
            Blazor,
            Blitz
        }
    }
}
