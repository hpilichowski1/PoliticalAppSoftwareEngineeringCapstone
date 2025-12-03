using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PoliticalApp.Models;
using PoliticalApp.Services;

namespace PoliticalApp.ViewModels
{
    public class BillsViewModel : INotifyPropertyChanged
    {
        private readonly IBillService _billService;
        private bool _isLoading;

        public ObservableCollection<Bill> Bills { get; } = new();

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadCommand { get; }

        public BillsViewModel(IBillService billService)
        {
            _billService = billService;
            LoadCommand = new Command(async () => await LoadAsync());
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private async Task LoadAsync()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                Bills.Clear();
                var items = await _billService.GetBillsAsync(page: 1, pageSize: 10);
                foreach (var bill in items)
                    Bills.Add(bill);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
