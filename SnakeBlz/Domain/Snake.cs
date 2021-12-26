using SnakeBlz.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBlz.Domain
{
    public class Snake
    {
        public static readonly Color UnitColor = Color.ForestGreen;
        public LinkedList<CellComponent> Cells { get; set; } = new();
        public Direction CurrentDirection { get; set; } = Direction.Left;
        public bool IsOutOfBounds { get; set; } = false;
        public bool HasCollidedWithSelf { get; set; } = false;

        private int _countPelletsConsumed;
        public int CountPelletsConsumed
        {
            get => _countPelletsConsumed;
            set
            {
                _countPelletsConsumed = value;
            }
        }

        public CellComponent Head => Cells.First.Value;
        public CellComponent Tail => Cells.Last.Value;

        public Snake() { }

        public void ChangeDirection(Direction direction)
        {
            if (!direction.IsOppositeTo(CurrentDirection))
                CurrentDirection = direction;
        }

        public void ConsumePellet()
        {
            CountPelletsConsumed++;
        }
    }
}
