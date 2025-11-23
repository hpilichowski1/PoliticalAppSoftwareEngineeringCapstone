using PoliticalApp.Models;
using PoliticalApp.ViewModels;
using System.Net.Http;
using System.Net.Http.Json;

namespace PoliticalApp.Views;

public partial class LoginPage : ContentPage
{
    private const string ApiBaseUrl = "http://localhost:5154";

    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        ErrorLabel.Text = string.Empty;

        var email = EmailEntry.Text?.Trim() ?? string.Empty;
        var password = PasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Email and password are required.";
            ErrorLabel.IsVisible = true;
            return;
        }

        try
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            using var http = new HttpClient();
            var response = await http.PostAsJsonAsync(
                $"{ApiBaseUrl}/api/Auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                ErrorLabel.Text = $"Login failed: {msg}";
                ErrorLabel.IsVisible = true;
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result is null || !result.Success)
            {
                ErrorLabel.Text = result?.Message ?? "Login failed.";
                ErrorLabel.IsVisible = true;
                return;
            }

            // ✅ Navigate to home using ABSOLUTE shell route
            await Shell.Current.GoToAsync("///HomePage");
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Error contacting server: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RegisterPage");
    }
}
