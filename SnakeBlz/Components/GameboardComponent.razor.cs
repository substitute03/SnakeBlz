using Microsoft.AspNetCore.Components;
using static SnakeBlz.Domain.Enums;

namespace SnakeBlz.Components
{
    public partial class GameboardComponent
    {
        [Parameter] public int GameboardSize { get; set; } = 15;

        public List<CellComponent> CellComponents { get; set; } = new();

        private CellComponent SetComponentRef
        {
            set
            {
                CellComponents.Add(value);
            }
        }

        protected override void OnInitialized()
        {
        }

        public void SpawnSnake()
        {
            CellComponents.Where(c => c.X == 7 && c.Y == 7).Single().CellType = CellType.Snake;
            CellComponents.Where(c => c.X == 7 && c.Y == 8).Single().CellType = CellType.Snake;
            CellComponents.Where(c => c.X == 7 && c.Y == 9).Single().CellType = CellType.Snake;
        }
    }
}
