using PoliticalApp.ViewModels;

namespace PoliticalApp.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnBackToLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///LoginPage");
    }
}
