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
        var a = SelectedOptions.SelectedGameMode;

        NavigationManager.NavigateTo($"/play/{SelectedOptions.SelectedGameMode}", forceLoad: true);
    }
}
