using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SnakeBlz.Domain;
using System.Diagnostics;
using System.Drawing;
using static SnakeBlz.Domain.Enums;

namespace SnakeBlz.Components;

public partial class GameComponent : IDisposable
{
    #region Properties
    private GameboardComponent Gameboard { get; set; }
    private DotNetObjectReference<GameComponent> gameboardObjectReference;

    private int SnakeSpeedInMilliseconds { get; set; } = 80;
    private bool AllowInput => GameState == GameState.InProgress;
    private int Score { get; set; } = 0;
    private string Message { get; set; }

    // Blitz game mode properties
    private CancellationToken BlitzTimerCancellationToken { get; set; }
    private CancellationTokenSource BlitzTimerCancellationSource { get; set; }
    private Stopwatch BlitzStopwatch { get; set; } = new();

    // Blazing properties
    private CancellationToken BazingCancellationToken { get; set; }
    private CancellationTokenSource BlazingCancellationSource { get; set; }
    private int BlazingStatusCounter { get; set; } = 100;
    private bool IsHandleBlazingStatusLoopRunning { get; set; }
    private int ProgressBarPercentageNumber { get; set; } = 0;
    private string ProgressBarPercentageString => $"{ProgressBarPercentageNumber}%";

    [Parameter] public string _gameMode 
    { 
        set
        {
            GameMode = (Enums.GameMode)Enum.Parse(typeof(Enums.GameMode), value, ignoreCase: true);
        }
    }
    public GameMode GameMode { get; set; } = GameMode.Blazor;

    private GameState GameState { get; set; }
    private TimeOnly StartTime { get; set; }
    private TimeOnly EndTime { get; set; }
    private TimeSpan Duration => EndTime - StartTime;
    private LinkedList<string> StoredKeyPresses { get; set; } = new();
    #endregion

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Create a reference to this component which will allow JSRuntime to access it.
            gameboardObjectReference = DotNetObjectReference.Create(this);

            await JSRuntime.InvokeVoidAsync("addKeyDownEventListener", gameboardObjectReference);
        }
    }

    private async Task StartNewGame()
    {
        GameState = GameState.InProgress;

        ResetGameStatus();
        await PlayCountdown();
        SpawnSnake();
        SpawnPellet();

        if (GameMode == GameMode.Blitz)
        {
            BlitzTimerCancellationSource = new CancellationTokenSource();
            BlitzTimerCancellationToken = BlitzTimerCancellationSource.Token;

            StartTimer(30000, BlitzTimerCancellationToken);
        }

        await StartGameLoop();

        GameState = GameState.GameOver;
    }

    private void ResetGameStatus()
    {
        IsHandleBlazingStatusLoopRunning = false;
        BlazingCancellationSource = new();
        BlazingStatusCounter = 100;

        Gameboard.ClearCells();
        Score = 0;
        ProgressBarPercentageNumber = 0;
        StoredKeyPresses.Clear();
    }

    public void SpawnSnake()
    {
        Gameboard.Snake = new Snake();

        if (GameMode == GameMode.Blazor || GameMode == GameMode.Blitz)
        {
            Gameboard.Snake.CanBlaze = true;
        }

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
        if (Gameboard.Snake.CountPelletsConsumed > 0)
        {
            PlaySound("consumePellet");

            if ((GameMode == GameMode.Blazor || GameMode == GameMode.Blitz) && Gameboard.Snake.IsBlazing)
            {
                if (IsHandleBlazingStatusLoopRunning)
                {
                    BlazingStatusCounter += 20;
                    StateHasChanged();
                }
                else
                {
                    BlazingCancellationSource = new CancellationTokenSource();
                    BazingCancellationToken = BlazingCancellationSource.Token;
                    HandleBlazingStatus(BazingCancellationToken);
                }
            }
            else if ((GameMode == GameMode.Blazor || GameMode == GameMode.Blitz) && !Gameboard.Snake.IsBlazing)
            {
                ProgressBarPercentageNumber = Gameboard.Snake.BlazingStacks * 20;
                StateHasChanged();
            }
        }

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
        PlaySound("countdownInProgress");
        Message = "Starting in...3";
        StateHasChanged();
        await Task.Delay(1000);

        PlaySound("countdownInProgress");
        Message = "Starting in...2";
        StateHasChanged();
        await Task.Delay(1000);

        PlaySound("countdownInProgress");
        Message = "Starting in...1";
        StateHasChanged();
        await Task.Delay(1000);

        PlaySound("countdownEnd");
        Message = "Go!";
        StateHasChanged();
        await Task.Delay(1000);

        Message = String.Empty;
        StateHasChanged();
    }

    private async Task HandleBlazingStatus(CancellationToken cancellationToken)
    {
        Task handle = Task.Run(async() =>
        {
            IsHandleBlazingStatusLoopRunning = true;

            foreach (var cell in Gameboard.Cells.Where(c => c.CellType == CellType.Snake))
            {
                cell.CellType = CellType.BlazingSnake;
            }

            PlaySound("countdownInProgress");

            // This will loop for a default of 5 seconds and update the progress bar every 0.05 seconds.
            // If pellets are consumed while Blazing status is active, the BlazinStatusCeilingCounter is increased, prolonging the Blazing status. 
            for (int i = BlazingStatusCounter; i >= 0;)
            {
                cancellationToken.ThrowIfCancellationRequested();
                i = BlazingStatusCounter;
                i--;
                BlazingStatusCounter--;
                ProgressBarPercentageNumber = i;
                StateHasChanged();
                await Task.Delay(50);
            }

            foreach (var cell in Gameboard.Cells.Where(c => c.CellType == CellType.BlazingSnake))
            {
                cell.CellType = CellType.Snake;
            }

            BlazingStatusCounter = 100;
            Gameboard.Snake.ResetBlazingStacks();
            PlaySound("countdownEnd");

            IsHandleBlazingStatusLoopRunning = false;
        }, cancellationToken);
    }

    private async Task StartTimer(double durationInMilliseconds, CancellationToken cancellationToken)
    {
        Task timerTask = Task.Run(async () =>
        {
            BlitzStopwatch = new();
            BlitzStopwatch.Start();

            while (BlitzStopwatch.ElapsedMilliseconds <= durationInMilliseconds && !BlitzTimerCancellationSource.IsCancellationRequested)
            {
                TimeSpan timeRemaining =
                    TimeSpan.FromMilliseconds(durationInMilliseconds - BlitzStopwatch.ElapsedMilliseconds);

                string timeRemainingInMinutesAndSeconds =
                    $"{(int)timeRemaining.TotalMinutes}:{timeRemaining.Seconds:00}";

                Message = $"{timeRemainingInMinutesAndSeconds}";
                StateHasChanged();

                await Task.Delay(1000);
            }

            BlitzStopwatch.Stop();
            StateHasChanged();
        }, cancellationToken);
    }

    private async Task PlaySound(string soundName)
    {
        await JSRuntime.InvokeVoidAsync("playAudio", soundName);
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
            Score = Gameboard.Snake.CountPelletsConsumed;

            StateHasChanged();
            await Task.Delay(SnakeSpeedInMilliseconds);

        } while (
            (GameMode == GameMode.Blitz && BlitzStopwatch.IsRunning && !Gameboard.IsInIllegalState) ||
            (GameMode != GameMode.Blitz && !Gameboard.IsInIllegalState));


        EndTime = new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        await HandleGameOver();
   
        //return results;
    }

    private async Task HandleGameOver()
    {
        PlaySound("gameOver");

        if (BlazingCancellationSource != null)
        {
            BlazingCancellationSource.Cancel();
        }

        if (GameMode == GameMode.Blitz && BlitzTimerCancellationSource != null)
        {
            BlitzTimerCancellationSource.Cancel();
        }
        
        GameResults results = new GameResults(Score, Duration);
        GameState = GameState.GameOver;

        if (GameMode == GameMode.Blitz && !BlitzStopwatch.IsRunning)
        {
            Message = "Time's up!";
        }
        else
        {
            Message = $"Game over! Duration {Duration}.";
        }

        StateHasChanged();
    }

    // This annotation allows it to be called from JavaScript.
    [JSInvokable ("HandleKeyPress")]
    public async Task HandleKeyPress (string key)
    {
        if (!AllowInput)
        {
            return;
        }

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
