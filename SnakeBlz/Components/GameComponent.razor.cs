using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
        //base.OnAfterRender(firstRender);
        if (firstRender)
        {
            //See warning about memory above in the article
            // Create a reference to this component which will allow JSRuntime to access it.
            var gameBoardObjectReference = DotNetObjectReference.Create(this);

            await JSRuntime.InvokeVoidAsync("addKeyDownEventListener", gameBoardObjectReference);

            Gameboard.SpawnSnake();
        }
    }

    // This annotation allows it to be called from JavaScript.
    [JSInvokable ("HandleKeyPress")]
    public async Task HandleKeyPress (string key)
    {

    }
}
