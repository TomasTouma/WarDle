namespace WarDle;
using WarDle.ViewModels;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System;

public partial class GamePage : ContentPage
{
    private GameViewModel gameViewModel;

    public GamePage()
	{
        InitializeComponent();
        gameViewModel = new GameViewModel();
        BindingContext = gameViewModel;
        gameViewModel.FocusNextEntry += FocusEntry;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await gameViewModel.InitializeWordsAsync();
    }

    public async Task LoadGameStateAsync()
    {
        await gameViewModel.LoadGameStateAsync();
    }

    public async Task SaveGameStateAsync()
    {
        await gameViewModel.SaveGameStateAsync();
    }
    private void FocusEntry(int rowIndex, int colIndex)
    {
        // Find the entry by index and set focus
        var entry = FindEntryByIndex(rowIndex, colIndex);
        entry?.Focus();
    }


    // Find the entry by row and column index
    private Entry FindEntryByIndex(int rowIndex, int colIndex)
    {
        // Use the instance of the Grid (EntryGrid) to get row and column information
        return (Entry)EntryGrid.Children.FirstOrDefault(e => EntryGrid.GetRow(e) == rowIndex && EntryGrid.GetColumn(e) == colIndex);
    }

    private void EntryTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry == null)
            return;

        // Use the instance of the Grid (EntryGrid) to get row and column information
        var rowIndex = EntryGrid.GetRow(entry);
        var colIndex = EntryGrid.GetColumn(entry);
        gameViewModel.OnEntryTextChanged(rowIndex, colIndex, e.NewTextValue);
        if (entry != null)
        {
            entry.Text = e.NewTextValue?.ToUpper();
        }
    }

   
}