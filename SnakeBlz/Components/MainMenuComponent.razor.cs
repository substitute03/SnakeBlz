using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SnakeBlz.Domain;
using SnakeBlz.DTOs;

namespace SnakeBlz.Components;

public partial class MainMenuComponent
{
    private List<string> GameModeOptions { get; set; } = new();
    private MainMenuSelectedOptionsDto SelectedOptions { get; set; } = new();
    private EditContext Model { get; set; }
    private bool IsLoading { get; set; }
    private bool HideHelpModal { get; set; } = true;
    [Inject] private NavigationManager NavigationManager { get; set; }

    protected override void OnInitialized() 
    {
        IsLoading = true;
        LoadData();
        IsLoading = false;
    }

    private void LoadData()
    {
        foreach (var gameMode in Enum.GetValues(typeof(Enums.GameMode)))
        {
            GameModeOptions.Add(gameMode.ToString());
        }

        Model = new EditContext(SelectedOptions);
    }

    private async Task HandleValidSubmit()
    {
        // Force load is set to true as otherwise, when the user navigates using the back and forward browser options when the game is paused, it can interfere with the Snake's movement.
        // Force load makes Blazor bypass it's own routing system and it will instead go to the server to get the content to display.
        NavigationManager.NavigateTo($"/play/{SelectedOptions.SelectedGameMode.ToString().ToLower()}", forceLoad: true);
    }
}
