using Microsoft.AspNetCore.Components;
using SnakeBlz.Domain;
using System.Drawing;
using static SnakeBlz.Domain.Enums;

namespace SnakeBlz.Components;

public partial class GameboardComponent
{
    [Parameter] public int GameboardSize { get; set; } = 15;
    [Parameter] public EventCallback PelletConsumed { get; set; }

    public Snake Snake { get; set; }
    public bool IsInIllegalState => Snake.IsOutOfBounds || Snake.HasCollidedWithSelf;
    public List<CellComponent> Cells { get; set; } = new();

    private CellComponent SetComponentRef
    {
        set
        {
            Cells.Add(value);
        }
    }

    protected override void OnInitialized() { }

    public void ClearCells()
    {
        foreach (var cell in Cells)
        {
            cell.CellType = CellType.Empty;
        }
    }

    public async Task MoveSnake(Direction directionToMove)
    {
        if (directionToMove == Direction.None)
        {
            directionToMove = Snake.CurrentDirection;
        }

        if (await IsAdjacentCellOutOfBounds(directionToMove, Snake.Head))
        {
            Snake.IsOutOfBounds = true;
            return;
        }

        CellComponent moveToCell = await GetAdjacentCell(directionToMove, Snake.Head);

        if (moveToCell.CellType == CellType.Empty)
        {
            Snake.CurrentDirection = directionToMove;
            Snake.Tail.CellType = CellType.Empty;
            Snake.Cells.Remove(Snake.Tail);
        }
        else if (moveToCell.CellType == CellType.Snake || moveToCell.CellType == CellType.BlazingSnake)
        {
            Snake.HasCollidedWithSelf = true;
            return;
        }
        else if (moveToCell.CellType == CellType.Pellet)
        {
            Snake.CurrentDirection = directionToMove;
            Snake.ConsumePellet();
            await PelletConsumed.InvokeAsync();
        }

        Snake.Cells.AddFirst(moveToCell);
        Snake.Head.CellType = Snake.IsBlazing ? CellType.BlazingSnake : CellType.Snake;

        StateHasChanged();
    }

    private Task<CellComponent> GetAdjacentCell(Direction direction, CellComponent cell)
    {
        int x = cell.X;
        int y = cell.Y;

        if (direction == Direction.Up)
        {
            return Task.FromResult(Cells.Single(c => c.X == x && c.Y == y - 1));
        }
        else if (direction == Direction.Down)
        {
            return Task.FromResult(Cells.Single(c => c.X == x && c.Y == y + 1));
        }
        else if (direction == Direction.Left)
        {
            return Task.FromResult(Cells.Single(c => c.X == x - 1 && c.Y == y));
        }
        else // direction will be Direction.Right
        {
            return Task.FromResult(Cells.Single(c => c.X == x + 1 && c.Y == y));
        }
    }

    private Task<bool> IsAdjacentCellOutOfBounds(Direction direction, CellComponent cell)
    {
        int x = cell.X;
        int y = cell.Y;

        Point adjacentCoordinates;

        if (direction == Direction.Up)
        {
            adjacentCoordinates = new Point(x, y - 1);
        }
        else if (direction == Direction.Down)
        {
            adjacentCoordinates = new Point(x, y + 1);
        }
        else if (direction == Direction.Left)
        {
            adjacentCoordinates = new Point(x - 1, y);
        }
        else
        {
            adjacentCoordinates = new Point(x + 1, y);
        }

        if (adjacentCoordinates.X == 0 || adjacentCoordinates.Y == 0 ||
            adjacentCoordinates.X > GameboardSize || adjacentCoordinates.Y > GameboardSize)
        {
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public int GetCellIndex(int positionX, int positionY)
    {
        return Cells
            .IndexOf(Cells
                .Single(gbc => gbc.X == positionX && gbc.Y == positionY));
    }
}
