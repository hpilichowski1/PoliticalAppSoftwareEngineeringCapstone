using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PoliticalApp.Services;

namespace PoliticalApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiClient _apiClient;

        private string email = "";
        private string password = "";
        private string errorMessage = "";
        private bool isBusy;

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

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiClient apiClient)
        {
            _apiClient = apiClient;
            LoginCommand = new Command(async () => await LoginAsync());
        }

        public async Task LoginAsync()
        {
            IsBusy = true;

            var result = await _apiClient.LoginAsync(Email, Password);

            if (result == null || !result.Success)
            {
                ErrorMessage = result?.Message ?? "Login failed";
            }
            else
            {
                // navigate after login
                await Shell.Current.GoToAsync("//HomePage");
            }

            IsBusy = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}