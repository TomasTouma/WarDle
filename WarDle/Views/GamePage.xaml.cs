namespace WarDle;
using WarDle.ViewModels;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System;

public partial class GamePage : ContentPage
{
    private GameViewModel gameViewModel;

    // Constructor
    public GamePage()
	{
        InitializeComponent();
        gameViewModel = new GameViewModel();
        BindingContext = gameViewModel;
        gameViewModel.FocusNextEntry += FocusEntry;
    }

    // Initialize the words
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await gameViewModel.InitializeWordsAsync();
    }

    // Load the game state
    public async Task LoadGameStateAsync()
    {
        await gameViewModel.LoadGameStateAsync();
    }

    // Save the game state
    public async Task SaveGameStateAsync()
    {
        await gameViewModel.SaveGameStateAsync();
    }

    // Focus the entry at the specified row and column index
    private void FocusEntry(int rowIndex, int colIndex)
    {
        // Find the entry by index and set focus
        var entry = FindEntryByIndex(rowIndex, colIndex);
        entry?.Focus();
    }


    // Find the entry by row and column index
    private Entry FindEntryByIndex(int rowIndex, int colIndex)
    {
        return (Entry)EntryGrid.Children.FirstOrDefault(e => EntryGrid.GetRow(e) == rowIndex && EntryGrid.GetColumn(e) == colIndex);
    }

    private void EntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null)
            return;

        var rowIndex = EntryGrid.GetRow(entry);
        var colIndex = EntryGrid.GetColumn(entry);
        gameViewModel.OnEntryTextChanged(rowIndex, colIndex, e.NewTextValue);
        if (entry != null)
        {
            entry.Text = e.NewTextValue?.ToUpper();
        }
    }

   
}