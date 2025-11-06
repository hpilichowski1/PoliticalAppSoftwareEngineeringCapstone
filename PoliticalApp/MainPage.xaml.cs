namespace PoliticalApp;

<<<<<<< HEAD
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
}
=======
public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

	private void OnClickedTwo(object? sender, EventArgs e)
	{
		DisplayAlert("Hello", "You clicked the second button!", "OK");
	}
}
>>>>>>> ea6b687 (created Web API folder and descriptions for db)
