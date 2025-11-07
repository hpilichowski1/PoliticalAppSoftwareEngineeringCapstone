using PoliticalApp;
using PoliticalApp.ViewModels;

namespace PoliticalApp
{
    public partial class AuthPage : TabbedPage
    {
        public AuthPage()
        {
            InitializeComponent();

            var http = Application.Current?.Handler?.MauiContext?.Services.GetRequiredService<HttpClient>() ?? throw new InvalidOperationException("Services not available");
            Task ShowAlert(string t, string m, string ok) => this.DisplayAlert(t, m, ok);

            BindingContext = new AuthViewModel(
                goToRegister: () => CurrentPage = this.Children.First(p => p.Title == "Register"),
                goToLogin: () => CurrentPage = this.Children.First(p => p.Title == "Login"),
                onLoginSuccess: () =>
                {
                    var app = Application.Current;
                    if (app is not null && app.Windows.Count > 0)
                    {
                        app.Windows[0].Page = new AppShell();
                    }
                },
                showAlert: ShowAlert,
                httpClient: http
            );
        }

        public AuthPage(AuthViewModel vm) : this() // convenience overload
        {
            BindingContext = vm;
        }
    }
}
