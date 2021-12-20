using Microsoft.AspNetCore.Components;
using System.Drawing;
using static SnakeBlz.Domain.Enums;

namespace SnakeBlz.Components;

public partial class CellComponent
{
    [Parameter] public int X { get; set; }
    [Parameter] public int Y { get; set; }

    private CellType _cellType = CellType.Empty;
    public CellType CellType
    {
        get
        {
            return _cellType;
        }

        set
        {
            if (value != _cellType)
            {
                _cellType = value;
                StateHasChanged();
            }
        }
    }
}
