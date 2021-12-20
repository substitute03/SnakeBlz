using Microsoft.AspNetCore.Components;
using SnakeBlz.Components;
using SnakeMobile.Domain.Model;
using System.Collections.ObjectModel;
using static SnakeBlz.Domain.Enums;

namespace SnakeBlz.Pages
{
    public partial class GameScreen
    {
        [Parameter] public int GameboardSize { get; set; } = 15;

        private List<CellComponent> CellComponents { get; set; } = new();

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

        protected override void OnAfterRender(bool firstRender)
        {
            SpawnSnake();
        }

        private void SpawnSnake()
        {
            CellComponents.Where(c => c.X == 7 && c.Y == 7).Single().CellType = CellType.Snake;
            CellComponents.Where(c => c.X == 7 && c.Y == 8).Single().CellType = CellType.Snake;
            CellComponents.Where(c => c.X == 7 && c.Y == 9).Single().CellType = CellType.Snake;
        }
    }
}
