using PoliticalApp.ViewModels;
using Microsoft.Maui.Controls;

namespace PoliticalApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
