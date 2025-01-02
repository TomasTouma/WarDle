

namespace WarDle;
using WarDle.ViewModels;

public partial class MainPage : ContentPage
{
    

    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel();
    }

   
}
