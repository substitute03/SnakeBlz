using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SnakeBlz.Domain;
using System.Drawing;
using static SnakeBlz.Domain.Enums;

namespace SnakeBlz.Components;

public partial class GameComponent : IDisposable
{
    private GameboardComponent Gameboard { get; set; }
    private DotNetObjectReference<GameComponent> gameboardObjectReference;

    private int InputDelayInMilliseconds { get; set; } = 100;
    private int Score { get; set; } = 0;
    private string Message { get; set; }
    private GameState GameState { get; set; }
    private TimeOnly StartTime { get; set; }
    private TimeOnly EndTime { get; set; }
    private TimeSpan Duration => EndTime - StartTime;
    private LinkedList<string> StoredKeyPresses { get; set; } = new();

    protected override async Task OnInitializedAsync(){}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //base.OnAfterRender(firstRender);
        if (firstRender)
        {
            //See warning about memory above in the article
            // Create a reference to this component which will allow JSRuntime to access it.
            gameboardObjectReference = DotNetObjectReference.Create(this);

            await JSRuntime.InvokeVoidAsync("addKeyDownEventListener", gameboardObjectReference);

            StartNewGame();
        }
    }

    private async Task StartNewGame()
    {
        GameState = GameState.InProgress;

        ClearGameboard();
        await PlayCountdown();
        SpawnSnake();
        SpawnPellet();
        await StartGameLoop();

        GameState = GameState.GameOver;
    }

    private void ClearGameboard()
    {
        Gameboard.ClearCells();
    }

    public void SpawnSnake()
    {
        Gameboard.Snake = new Snake();

        CellComponent cell1 = Gameboard.Cells.Where(c => c.X == 12 && c.Y == 7).Single();
        CellComponent cell2 = Gameboard.Cells.Where(c => c.X == 13 && c.Y == 7).Single();
        CellComponent cell3 = Gameboard.Cells.Where(c => c.X == 14 && c.Y == 7).Single();

        // Set Gameboard cells to CellType.Snake.
        cell1.CellType = CellType.Snake;
        cell2.CellType = CellType.Snake;
        cell3.CellType = CellType.Snake;

        // Add references to the Cell Components that are now snake parts to the list of Cells in the Snake.
        Gameboard.Snake.Cells.AddFirst(cell1);
        Gameboard.Snake.Cells.AddLast(cell2);
        Gameboard.Snake.Cells.AddLast(cell3);
    }

    public void SpawnPellet()
    {
        Point emptyCellPoint =
            GetEmptyCellCoordinates();

        var emptyCell = Gameboard.Cells
            .Where(c => c.X == emptyCellPoint.X && c.Y == emptyCellPoint.Y)
            .Single();

        emptyCell.CellType = CellType.Pellet;
    }

    private Point GetEmptyCellCoordinates()
    {
        List<CellComponent>? emptyCells =
            Gameboard.Cells.Where(c => c.CellType == CellType.Empty).ToList();

        Random random = new Random();
        int emptyCellIndex = random.Next(random.Next(0, emptyCells.Count));

        return new Point
        {
            X = emptyCells[emptyCellIndex].X,
            Y = emptyCells[emptyCellIndex].Y
        };
    }

    private async Task PlayCountdown()
    {
        Message = "Starting in...3";
        StateHasChanged();
        await Task.Delay(1000);
        Message = "Starting in...2";
        StateHasChanged();
        await Task.Delay(1000);
        Message = "Starting in...1";
        StateHasChanged();
        await Task.Delay(1000);
        Message = "Go!";
        StateHasChanged();
        await Task.Delay(1000);
        Message = "";
        StateHasChanged();
    }

    public async Task StartGameLoop()
    {
        StartTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        do
        {
            Direction nextDirection = Direction.FromKey(StoredKeyPresses.FirstOrDefault());

            if (StoredKeyPresses.Count > 0)
            {
                StoredKeyPresses.RemoveFirst();
            }

            await Gameboard.MoveSnake(nextDirection);
            StateHasChanged();

            Score = Gameboard.Snake.CountPelletsConsumed;

            await Task.Delay(InputDelayInMilliseconds);

        } while (!Gameboard.IsInIllegalState);

        EndTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        GameResults results = new GameResults(Score, Duration);
        GameState = GameState.GameOver;
        Message = $"Game over! Duration {Duration}.";

        StateHasChanged();

        //return results;
    }

    // This annotation allows it to be called from JavaScript.
    [JSInvokable ("HandleKeyPress")]
    public async Task HandleKeyPress (string key)
    {
        if (!"wasd".Contains(key))
        {
            return;
        }

        Direction directionToMove = Direction.FromKey(key);
        Direction nextDirection = Direction.FromKey(StoredKeyPresses.FirstOrDefault());

        if (StoredKeyPresses.Count == 0)
        {
            if (Gameboard.Snake.CurrentDirection.IsEqualTo(directionToMove) ||
                Gameboard.Snake.CurrentDirection.IsOppositeTo(directionToMove))
            {
                return;
            }
        }
        else if (StoredKeyPresses.Count > 0)
        {
            if (nextDirection.IsEqualTo(directionToMove) 
                || nextDirection.IsOppositeTo(directionToMove))
            {
                return;
            }
        }

        if (StoredKeyPresses.Count == 2)
        {
            StoredKeyPresses.RemoveFirst();
        }

        StoredKeyPresses.AddLast(key);
        StateHasChanged();
    }

    public void Dispose()
    {
        gameboardObjectReference.Dispose();
    }
}
