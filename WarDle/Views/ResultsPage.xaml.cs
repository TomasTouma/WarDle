using WarDle.ViewModels;

namespace WarDle;

public partial class ResultsPage : ContentPage
{
	public ResultsPage()
	{
		InitializeComponent();
        BindingContext = new ResultsViewModel();
    }
}