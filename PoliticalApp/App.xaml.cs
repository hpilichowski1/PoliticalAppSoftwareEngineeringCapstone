using PoliticalApp.Views;

namespace PoliticalApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Use Shell; it will create LoginPage via DI
        MainPage = new AppShell();
    }
}
