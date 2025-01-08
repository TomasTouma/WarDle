

namespace WarDle;
using WarDle.ViewModels;

public partial class MainPage : ContentPage
{

    // Constructor
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
    }

   
}
