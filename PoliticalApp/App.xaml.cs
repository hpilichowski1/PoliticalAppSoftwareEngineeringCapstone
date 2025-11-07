using PoliticalApp.ViewModels;

namespace PoliticalApp;

public partial class App : Application
{
	protected override Window CreateWindow(IActivationState? activationState)
    {
        var http = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5154/") // adjust to your API
        };

        var authViewModel = new AuthViewModel(
            () => { Shell.Current.GoToAsync("//Register"); },
            () => { Shell.Current.GoToAsync("//Login"); }, 
            () => { Shell.Current.GoToAsync("//Main"); },  
            (title, message, ok) => Application.Current!.Windows[0].Page!.DisplayAlert(title, message, ok),
            http
        );

        var appShell = new AppShell();
        var authPage = new AuthPage(authViewModel); // your ctor that requires VM

        var window = new Window(appShell)
        {
            Page = authPage
        };
        return window;
    }

}
