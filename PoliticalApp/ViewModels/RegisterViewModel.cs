using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PoliticalApp.Services;

namespace PoliticalApp.ViewModels;

public class RegisterViewModel : INotifyPropertyChanged
{
    private readonly ApiClient _apiClient;

    private string name = "";
    private string email = "";
    private string password = "";
    private string confirmPassword = "";
    private string errorMessage = "";
    private bool isBusy;

    public string Name
    {
        get => name;
        set { name = value; OnPropertyChanged(); }
    }

    public string Email
    {
        get => email;
        set { email = value; OnPropertyChanged(); }
    }

    public string Password
    {
        get => password;
        set { password = value; OnPropertyChanged(); }
    }

    public string ConfirmPassword
    {
        get => confirmPassword;
        set { confirmPassword = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => errorMessage;
        set { errorMessage = value; OnPropertyChanged(); }
    }

    public bool IsBusy
    {
        get => isBusy;
        set { isBusy = value; OnPropertyChanged(); }
    }

    public ICommand RegisterCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public RegisterViewModel(ApiClient apiClient)
    {
        _apiClient = apiClient;
        RegisterCommand = new Command(async () => await OnRegisterAsync());
    }

    private async Task OnRegisterAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            var result = await _apiClient.RegisterAsync(Name, Email, Password);

            if (result == null || !result.Success)
            {
                ErrorMessage = result?.Message ?? "Registration failed.";
                return;
            }

            // After successful registration, go back to login
            await Shell.Current.GoToAsync("///LoginPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error contacting server: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
