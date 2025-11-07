using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace YourApp.ViewModels;

public class AuthViewModel : INotifyPropertyChanged
{
    // ===== Shared state =====
    bool isBusy;
    public bool IsBusy { get => isBusy; set => Set(ref isBusy, value); }

    // ===== Login state =====
    string loginEmail = string.Empty;
    public string LoginEmail { get => loginEmail; set { Set(ref loginEmail, value); RaiseCanExecutes(); } }

    string loginPassword = string.Empty;
    public string LoginPassword { get => loginPassword; set { Set(ref loginPassword, value); RaiseCanExecutes(); } }

    bool isLoginPasswordHidden = true;
    public bool IsLoginPasswordHidden { get => isLoginPasswordHidden; set => Set(ref isLoginPasswordHidden, value); }

    public string LoginPasswordToggleText => IsLoginPasswordHidden ? "Show" : "Hide";

    bool rememberMe;
    public bool RememberMe { get => rememberMe; set => Set(ref rememberMe, value); }

    string loginStatusMessage = string.Empty;
    public string LoginStatusMessage { get => loginStatusMessage; set => Set(ref loginStatusMessage, value); }

    bool hasLoginError;
    public bool HasLoginError { get => hasLoginError; set => Set(ref hasLoginError, value); }

    // ===== Register state =====
    string registerName = string.Empty;
    public string RegisterName { get => registerName; set { Set(ref registerName, value); RaiseCanExecutes(); } }

    string registerEmail = string.Empty;
    public string RegisterEmail { get => registerEmail; set { Set(ref registerEmail, value); RaiseCanExecutes(); } }

    string registerPassword = string.Empty;
    public string RegisterPassword { get => registerPassword; set { Set(ref registerPassword, value); RaiseCanExecutes(); } }

    string registerConfirmPassword = string.Empty;
    public string RegisterConfirmPassword { get => registerConfirmPassword; set { Set(ref registerConfirmPassword, value); RaiseCanExecutes(); } }

    bool isRegisterPasswordHidden = true;
    public bool IsRegisterPasswordHidden { get => isRegisterPasswordHidden; set => Set(ref isRegisterPasswordHidden, value); }

    public string RegisterPasswordToggleText => IsRegisterPasswordHidden ? "Show" : "Hide";

    bool acceptTerms;
    public bool AcceptTerms { get => acceptTerms; set { Set(ref acceptTerms, value); RaiseCanExecutes(); } }

    string registerStatusMessage = string.Empty;
    public string RegisterStatusMessage { get => registerStatusMessage; set => Set(ref registerStatusMessage, value); }

    bool hasRegisterError;
    public bool HasRegisterError { get => hasRegisterError; set => Set(ref hasRegisterError, value); }

    // ===== Commands (wired to your XAML) =====
    public ICommand ToggleLoginPasswordCommand { get; }
    public ICommand ToggleRegisterPasswordCommand { get; }
    public ICommand ForgotPasswordCommand { get; }
    public ICommand GoogleSignInCommand { get; }
    public ICommand AppleSignInCommand { get; }

    public ICommand GoToRegisterTabCommand { get; }
    public ICommand GoToLoginTabCommand { get; }

    public ICommand LoginCommand { get; }
    public ICommand RegisterCommand { get; }

    // CanExecutes exposed for XAML bindings
    public bool CanLogin => !string.IsNullOrWhiteSpace(LoginEmail) && !string.IsNullOrWhiteSpace(LoginPassword) && !IsBusy;
    public bool CanRegister =>
        !string.IsNullOrWhiteSpace(RegisterName) &&
        !string.IsNullOrWhiteSpace(RegisterEmail) &&
        !string.IsNullOrWhiteSpace(RegisterPassword) &&
        RegisterPassword == RegisterConfirmPassword &&
        AcceptTerms &&
        !IsBusy;

    readonly Action goToRegister;
    readonly Action goToLogin;
    readonly Action onLoginSuccess;

    public event PropertyChangedEventHandler? PropertyChanged;

    public AuthViewModel(Action goToRegister, Action goToLogin, Action onLoginSuccess)
    {
        this.goToRegister = goToRegister;
        this.goToLogin = goToLogin;
        this.onLoginSuccess = onLoginSuccess;

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

        ForgotPasswordCommand = new Command(async () =>
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
            {
                await page.DisplayAlert("Forgot Password", "This is a placeholder.", "OK");
            }
        });

        GoogleSignInCommand = new Command(async () =>
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
            {
                await page.DisplayAlert("Google Sign-In", "Placeholder for Google Sign-In.", "OK");
            }
        });

        AppleSignInCommand = new Command(async () =>
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
            {
                await page.DisplayAlert("Apple Sign-In", "Placeholder for Apple Sign-In.", "OK");
            }
        });

        GoToRegisterTabCommand = new Command(() => goToRegister?.Invoke());
        GoToLoginTabCommand = new Command(() => goToLogin?.Invoke());

        LoginCommand = new Command(async () => await DoLoginAsync(), () => CanLogin);
        RegisterCommand = new Command(async () => await DoRegisterAsync(), () => CanRegister);
    }

    async Task DoLoginAsync()
    {
        IsBusy = true;
        RaiseCanExecutes();
        HasLoginError = false;
        LoginStatusMessage = string.Empty;

        try
        {
            await Task.Delay(500); // pretend to call API

            // super basic "auth": accept any non-empty email/pass
            if (!IsValidEmail(LoginEmail))
            {
                HasLoginError = true;
                LoginStatusMessage = "Please enter a valid email.";
                return;
            }

            // Success → swap to AppShell/Home
            onLoginSuccess?.Invoke();
        }
        finally
        {
            IsBusy = false;
            RaiseCanExecutes();
        }
    }

    async Task DoRegisterAsync()
    {
        IsBusy = true;
        RaiseCanExecutes();
        HasRegisterError = false;
        RegisterStatusMessage = string.Empty;

        try
        {
            await Task.Delay(500); // pretend to call API

            if (!IsValidEmail(RegisterEmail))
            {
                HasRegisterError = true;
                RegisterStatusMessage = "Please enter a valid email.";
                return;
            }

            // After registration, move them to the Login tab and prefill email
            LoginEmail = RegisterEmail;
            OnPropertyChanged(nameof(LoginEmail));
            goToLogin?.Invoke();
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null)
            {
                await page.DisplayAlert("Account Created", "You can now sign in.", "OK");
            }
        }
        finally
        {
            IsBusy = false;
            RaiseCanExecutes();
        }
    }

    bool IsValidEmail(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var _ = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch { return false; }
    }

    void RaiseCanExecutes()
    {
        (LoginCommand as Command)?.ChangeCanExecute();
        (RegisterCommand as Command)?.ChangeCanExecute();
        OnPropertyChanged(nameof(CanLogin));
        OnPropertyChanged(nameof(CanRegister));
    }

    void Set<T>(ref T backing, T value, [CallerMemberName] string? prop = null)
    {
        if (!Equals(backing, value))
        {
            backing = value;
            OnPropertyChanged(prop);
        }
    }

    void OnPropertyChanged([CallerMemberName] string? prop = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
}
