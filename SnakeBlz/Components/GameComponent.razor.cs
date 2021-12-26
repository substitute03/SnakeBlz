using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SnakeBlz.Domain;
using System.Drawing;
using static SnakeBlz.Domain.Enums;

namespace SnakeBlz.Components;

public partial class GameComponent
{
    private GameboardComponent Gameboard { get; set; }
    private int InputDelayInMilliseconds { get; set; } = 100;
    private int Score { get; set; } = 0;
    private TimeOnly StartTime { get; set; }
    private TimeOnly EndTime { get; set; }
    private LinkedList<string> StoredKeyPresses { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        //base.OnAfterRender(firstRender);
        if (firstRender)
        {
            //See warning about memory above in the article
            // Create a reference to this component which will allow JSRuntime to access it.
            var gameBoardObjectReference = DotNetObjectReference.Create(this);

            await JSRuntime.InvokeVoidAsync("addKeyDownEventListener", gameBoardObjectReference);

            SpawnSnake();
            SpawnPellet();
            await StartGameLoop();
        }
    }
    public void SpawnSnake()
    {
        Gameboard.Snake = new Snake();

        CellComponent cell1 = Gameboard.Cells.Where(c => c.X == 7 && c.Y == 7).Single();
        CellComponent cell2 = Gameboard.Cells.Where(c => c.X == 8 && c.Y == 7).Single();
        CellComponent cell3 = Gameboard.Cells.Where(c => c.X == 9 && c.Y == 7).Single();

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

        //GameResults results = new GameResults(Score, Duration);

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

        //foreach (var keyPress in StoredKeyPresses)
        //{
        //    if (keyPress == "w")
        //    {
        //        Gameboard.Snake.ChangeDirection(Direction.Up);
        //    }
        //    else if (keyPress == "a")
        //    {
        //        Gameboard.Snake.ChangeDirection(Direction.Left);
        //    }
        //    else if (keyPress == "s")
        //    {
        //        Gameboard.Snake.ChangeDirection(Direction.Down);
        //    }
        //    else if (keyPress == "d")
        //    {
        //        Gameboard.Snake.ChangeDirection(Direction.Right);
        //    }

        //    //await Task.Delay(InputDelayInMilliseconds);
        //}

        //if (StoredKeyPresses.First() == "w")
        //{
        //    Gameboard.Snake.ChangeDirection(Direction.Up);
        //    StoredKeyPresses.RemoveFirst();

        //    if (StoredKeyPresses.Count() == 0)
        //    {
        //        return;
        //    }
        //}
        //else if (StoredKeyPresses.First() == "a")
        //{
        //    Gameboard.Snake.ChangeDirection(Direction.Left);
        //    StoredKeyPresses.RemoveFirst();

        //    if (StoredKeyPresses.Count() == 0)
        //    {
        //        return;
        //    }
        //}
        //else if (StoredKeyPresses.First() == "s")
        //{
        //    Gameboard.Snake.ChangeDirection(Direction.Down);
        //    StoredKeyPresses.RemoveFirst();

        //    if (StoredKeyPresses.Count() == 0)
        //    {
        //        return;
        //    }
        //}
        //else if (StoredKeyPresses.First() == "d")
        //{
        //    Gameboard.Snake.ChangeDirection(Direction.Right);
        //    StoredKeyPresses.RemoveFirst();

        //    if (StoredKeyPresses.Count() == 0)
        //    {
        //        return;
        //    }
        //}
    }
}
