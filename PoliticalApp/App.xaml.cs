using PoliticalApp.Views;

namespace PoliticalApp;

public partial class App : Application
{
    public static string CurrentUsername { get; set; } = "h@gmail.com";
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
