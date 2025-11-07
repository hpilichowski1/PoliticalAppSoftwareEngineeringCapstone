namespace PoliticalApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
        var window = new Window(new AppShell())
        {
            Page = new AuthPage()
        };
        return window;
	}
}