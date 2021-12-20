using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace SnakeMobile.Domain.Model
{
    public class Snake
    {
        public static readonly Color UnitColor = Color.ForestGreen;
        public LinkedList<Cell> Cells { get; set; }
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

        public Cell Head => Cells.First.Value;
        public Cell Tail => Cells.Last.Value;

        public Snake() { }

        public Task Render()
        {
            foreach (var cell in Cells)
            {
                cell.Color = UnitColor;
            }

            return Task.CompletedTask;
        }

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
