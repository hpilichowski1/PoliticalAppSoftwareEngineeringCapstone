using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PoliticalApp.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    // ===== Shared =====
    bool isBusy;
    public bool IsBusy { get => isBusy; set => SetProperty(ref isBusy, value); }

    // ===== Login state =====
    string loginEmail = string.Empty;
    public string LoginEmail { get => loginEmail; set { SetProperty(ref loginEmail, value); RaiseCanExecutes(); } }

    string loginPassword = string.Empty;
    public string LoginPassword { get => loginPassword; set { SetProperty(ref loginPassword, value); RaiseCanExecutes(); } }

    bool isLoginPasswordHidden = true;
    public bool IsLoginPasswordHidden { get => isLoginPasswordHidden; set => SetProperty(ref isLoginPasswordHidden, value); }

    public string LoginPasswordToggleText => IsLoginPasswordHidden ? "Show" : "Hide";

    bool rememberMe;
    public bool RememberMe { get => rememberMe; set => SetProperty(ref rememberMe, value); }

    string loginStatusMessage = string.Empty;
    public string LoginStatusMessage { get => loginStatusMessage; set => SetProperty(ref loginStatusMessage, value); }

    bool hasLoginError;
    public bool HasLoginError { get => hasLoginError; set => SetProperty(ref hasLoginError, value); }

    // ===== Register state =====
    string registerName = string.Empty;
    public string RegisterName { get => registerName; set { SetProperty(ref registerName, value); RaiseCanExecutes(); } }

    string registerEmail = string.Empty;
    public string RegisterEmail { get => registerEmail; set { SetProperty(ref registerEmail, value); RaiseCanExecutes(); } }

    string registerPassword = string.Empty;
    public string RegisterPassword { get => registerPassword; set { SetProperty(ref registerPassword, value); RaiseCanExecutes(); } }

    string registerConfirmPassword = string.Empty;
    public string RegisterConfirmPassword { get => registerConfirmPassword; set { SetProperty(ref registerConfirmPassword, value); RaiseCanExecutes(); } }

    bool isRegisterPasswordHidden = true;
    public bool IsRegisterPasswordHidden { get => isRegisterPasswordHidden; set => SetProperty(ref isRegisterPasswordHidden, value); }

    public string RegisterPasswordToggleText => IsRegisterPasswordHidden ? "Show" : "Hide";

    bool acceptTerms;
    public bool AcceptTerms { get => acceptTerms; set { SetProperty(ref acceptTerms, value); RaiseCanExecutes(); } }

    string registerStatusMessage = string.Empty;
    public string RegisterStatusMessage { get => registerStatusMessage; set => SetProperty(ref registerStatusMessage, value); }

    bool hasRegisterError;
    public bool HasRegisterError { get => hasRegisterError; set => SetProperty(ref hasRegisterError, value); }

    // ===== Commands =====
    public ICommand ToggleLoginPasswordCommand { get; }
    public ICommand ToggleRegisterPasswordCommand { get; }
    public ICommand ForgotPasswordCommand { get; }
    public ICommand GoogleSignInCommand { get; }
    public ICommand AppleSignInCommand { get; }
    public ICommand GoToRegisterTabCommand { get; }
    public ICommand GoToLoginTabCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }

    // ===== CanExecutes =====
    public bool CanLogin =>
        !string.IsNullOrWhiteSpace(LoginEmail) &&
        !string.IsNullOrWhiteSpace(LoginPassword) &&
        !IsBusy;

    public bool CanRegister =>
        !string.IsNullOrWhiteSpace(RegisterName) &&
        !string.IsNullOrWhiteSpace(RegisterEmail) &&
        !string.IsNullOrWhiteSpace(RegisterPassword) &&
        RegisterPassword == RegisterConfirmPassword &&
        AcceptTerms &&
        !IsBusy;

    // ===== Navigation & Services =====
    readonly Action goToRegister;
    readonly Action goToLogin;
    readonly Action onLoginSuccess;
    readonly Func<string, string, string, Task> showAlert;
    readonly HttpClient http;

    // ===== API DTOs =====
    record RegisterRequest(string Name, string Email, string Password);
    record LoginRequest(string Email, string Password);
    record AuthResponse(string UserId, string Name, string Email, string Role);

    public AuthViewModel(Action goToRegister,
                         Action goToLogin,
                         Action onLoginSuccess,
                         Func<string, string, string, Task> showAlert,
                         HttpClient? httpClient)
    {
        this.goToRegister = goToRegister;
        this.goToLogin = goToLogin;
        this.onLoginSuccess = onLoginSuccess;
        this.showAlert = showAlert;
        this.http = httpClient ?? new HttpClient();

        // UI toggles
        ToggleLoginPasswordCommand = new Command(() =>
        {
            IsLoginPasswordHidden = !IsLoginPasswordHidden;
            OnPropertyChanged(nameof(LoginPasswordToggleText));
        });

        ToggleRegisterPasswordCommand = new Command(() =>
        {
            IsRegisterPasswordHidden = !IsRegisterPasswordHidden;
            OnPropertyChanged(nameof(RegisterPasswordToggleText));
        });

        // Placeholder actions
        ForgotPasswordCommand = new Command(async () =>
            await showAlert("Forgot Password", "This is a placeholder.", "OK"));
        GoogleSignInCommand = new Command(async () =>
            await showAlert("Google Sign-In", "Placeholder for Google Sign-In.", "OK"));
        AppleSignInCommand = new Command(async () =>
            await showAlert("Apple Sign-In", "Placeholder for Apple Sign-In.", "OK"));

        // Navigation between tabs
        GoToRegisterTabCommand = new Command(() => goToRegister?.Invoke());
        GoToLoginTabCommand = new Command(() => goToLogin?.Invoke());

        // Auth actions
        LoginCommand = new Command(async () => await DoLoginAsync(), () => CanLogin);
        RegisterCommand = new Command(async () => await DoRegisterAsync(), () => CanRegister);
    }

    async Task DoLoginAsync()
    {
        IsBusy = true; RaiseCanExecutes();
        HasLoginError = false; LoginStatusMessage = string.Empty;

        try
        {
            if (!IsValidEmail(LoginEmail))
            {
                HasLoginError = true;
                LoginStatusMessage = "Please enter a valid email.";
                return;
            }

            var payload = new LoginRequest(LoginEmail.Trim(), LoginPassword);
            var resp    = await http.PostAsJsonAsync("api/auth/login", payload);

            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                HasLoginError = true;
                LoginStatusMessage = string.IsNullOrWhiteSpace(msg) ? "Invalid email or password." : msg;
                return;
            }

            var auth = await resp.Content.ReadFromJsonAsync<AuthResponse>();
            // TODO: persist auth if desired (Preferences.Set("user_id", auth!.UserId), etc.)

            onLoginSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            HasLoginError = true;
            LoginStatusMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false; RaiseCanExecutes();
        }
    }

    async Task DoRegisterAsync()
    {
        IsBusy = true; RaiseCanExecutes();
        HasRegisterError = false; RegisterStatusMessage = string.Empty;

        try
        {
            if (!IsValidEmail(RegisterEmail))
            {
                HasRegisterError = true;
                RegisterStatusMessage = "Please enter a valid email.";
                return;
            }

            var payload = new RegisterRequest(RegisterName.Trim(), RegisterEmail.Trim(), RegisterPassword);
            var resp    = await http.PostAsJsonAsync("api/auth/register", payload);

            if (!resp.IsSuccessStatusCode)
            {
                var msg = await resp.Content.ReadAsStringAsync();
                HasRegisterError = true;
                RegisterStatusMessage = string.IsNullOrWhiteSpace(msg) ? "Registration failed." : msg;
                return;
            }

            // Prefill login email and switch to Login tab
            LoginEmail = RegisterEmail;
            OnPropertyChanged(nameof(LoginEmail));
            goToLogin?.Invoke();
            await showAlert("Account Created", "You can now sign in.", "OK");
        }
        catch (Exception ex)
        {
            HasRegisterError = true;
            RegisterStatusMessage = $"Registration failed: {ex.Message}";
        }
        finally
        {
            IsBusy = false; RaiseCanExecutes();
        }
    }

    bool IsValidEmail(string email)
    {
        try { return !string.IsNullOrWhiteSpace(email) && new System.Net.Mail.MailAddress(email) is not null; }
        catch { return false; }
    }

    void RaiseCanExecutes()
    {
        (LoginCommand as Command)?.ChangeCanExecute();
        (RegisterCommand as Command)?.ChangeCanExecute();
        OnPropertyChanged(nameof(CanLogin));
        OnPropertyChanged(nameof(CanRegister));
    }

    [RelayCommand]
    static async Task ClickMe()
    {
        // Ensure UI call on main thread & avoid null ref if no page yet
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var windows = Application.Current?.Windows;
            var page = (windows != null && windows.Count > 0) ? windows[0].Page : null;
            if (page is not null)
                await page.DisplayAlert("Button Test", "You clicked me!", "OK");
        });
    }
}
