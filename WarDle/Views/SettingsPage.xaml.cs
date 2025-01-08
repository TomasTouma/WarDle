using WarDle.ViewModels;

namespace WarDle;

public partial class SettingsPage : ContentPage
{
    // Constructor
    public SettingsPage()
	{
		InitializeComponent();
        BindingContext = new SettingsViewModel();
    }
}