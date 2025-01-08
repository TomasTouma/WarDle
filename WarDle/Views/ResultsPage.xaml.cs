using WarDle.ViewModels;

namespace WarDle;

public partial class ResultsPage : ContentPage
{
    // Constructor
    public ResultsPage()
	{
		InitializeComponent();
        BindingContext = new ResultsViewModel();
    }
}