namespace PoliticalApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

     void OnLogoutClicked(object sender, EventArgs e)
    {
        // Go back to AuthPage as the root
        var app = Application.Current;
        if (app is null)
            return;

        // If the application has at least one window, replace the root page of the first window.
        if (app.Windows?.Count > 0)
        {
            app.Windows[0].Page = new AuthPage();
            return;
        }

        // Otherwise open a new window that hosts the AuthPage.
        app.OpenWindow(new Window(new AuthPage()));
    }
}
