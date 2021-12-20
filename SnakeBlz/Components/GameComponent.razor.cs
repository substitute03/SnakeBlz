using Microsoft.AspNetCore.Components;
using static SnakeBlz.Domain.Enums;

namespace SnakeBlz.Components;

public partial class GameComponent
{
    private GameboardComponent Gameboard { get; set; }

    protected override async Task OnInitializedAsync()
    {

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        SpawnSnake();
    }

    private void SpawnSnake()
    {
        Gameboard.CellComponents.Where(c => c.X == 7 && c.Y == 7).Single().CellType = CellType.Snake;
        Gameboard.CellComponents.Where(c => c.X == 7 && c.Y == 8).Single().CellType = CellType.Snake;
        Gameboard.CellComponents.Where(c => c.X == 7 && c.Y == 9).Single().CellType = CellType.Snake;
    }
}
