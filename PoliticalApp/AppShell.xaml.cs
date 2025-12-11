namespace PoliticalApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Register placeholder routes for future pages
        Routing.RegisterRoute("HomePlaceholder", typeof(ContentPage));
        Routing.RegisterRoute("RepresentativesPlaceholder", typeof(ContentPage));
        Routing.RegisterRoute("PoliciesPlaceholder", typeof(ContentPage));
        Routing.RegisterRoute("ProfilePlaceholder", typeof(ContentPage));
        Routing.RegisterRoute("QuizPlaceholder", typeof(ContentPage));
    }
}
