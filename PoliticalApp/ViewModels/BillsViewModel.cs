using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        private bool _hasMore = true;
        private int _currentPage = 1;
        private const int PageSize = 10;

        public ObservableCollection<Bill> Bills { get; } = new();

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                    OnPropertyChanged(nameof(CanLoadMore));
            }
        }

        public bool HasMore
        {
            get => _hasMore;
            set
            {
                if (SetProperty(ref _hasMore, value))
                    OnPropertyChanged(nameof(CanLoadMore));
            }
        }

        public bool CanLoadMore => !IsLoading && HasMore;

        public ICommand LoadCommand { get; }
        public ICommand LoadMoreCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public BillsViewModel(IBillService billService)
        {
            _billService = billService;

            // First page
            LoadCommand = new Command(async () => await LoadFirstPageAsync());

            // Subsequent pages
            LoadMoreCommand = new Command(
                async () => await LoadMoreAsync(),
                () => CanLoadMore);

        }

        private async Task LoadFirstPageAsync()
        {
            if (IsLoading) return;
            IsLoading = true;

            try
            {
                Bills.Clear();
                _currentPage = 1;
                HasMore = true;

                var items = await _billService.GetBillsAsync(_currentPage, PageSize);

                foreach (var bill in items)
                    Bills.Add(bill);

                // If we got a full page, assume there might be more
                HasMore = items.Count == PageSize;
                _currentPage++;
            }
            finally
            {
                IsLoading = false;
                UpdateCommands();
            }
        }

        // -----------------------
        // Load more pages
        // -----------------------
        private async Task LoadMoreAsync()
        {
            if (IsLoading || !HasMore) return;
            IsLoading = true;

            try
            {
                var items = await _billService.GetBillsAsync(_currentPage, PageSize);

                if (items.Count == 0)
                {
                    HasMore = false;
                    return;
                }

                foreach (var bill in items)
                {
                    if (!Bills.Any(b => b.Id == bill.Id))
                        Bills.Add(bill);
                }

                HasMore = items.Count == PageSize;
                _currentPage++;
            }
            finally
            {
                IsLoading = false;
                UpdateCommands();
            }
        }

        private void UpdateCommands()
        {
            if (LoadMoreCommand is Command c)
                c.ChangeCanExecute();
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
