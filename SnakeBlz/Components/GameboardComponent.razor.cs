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
    }
}
