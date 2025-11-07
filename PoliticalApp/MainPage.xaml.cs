using PoliticalApp.ViewModels;

namespace PoliticalApp;

public partial class MainPage : ContentPage
{
    public MainPage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    
}
