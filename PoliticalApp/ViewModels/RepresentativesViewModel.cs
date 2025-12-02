using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using PoliticalApp.Models;
using PoliticalApp.Services;


namespace PoliticalApp.ViewModels
{
    public class RepresentativesViewModel : INotifyPropertyChanged
    {
        private readonly IRepresentativeService _representativeService;

        private bool _isLoading;
        private bool _hasLoaded;
        private string _searchText = string.Empty;
        private string _selectedLevel = "All";
        private string _searchSummary = string.Empty;
        private string _selectedState = "All";
        private string _selectedDistrict = "All";

        public ObservableCollection<string> AvailableStates { get; } =
            new ObservableCollection<string>();

        public ObservableCollection<string> AvailableDistricts { get; } =
            new ObservableCollection<string>();

        public ObservableCollection<Representative> Representatives { get; } =
            new ObservableCollection<Representative>();

        public ObservableCollection<Representative> FilteredRepresentatives { get; } =
            new ObservableCollection<Representative>();

        public string SelectedState
        {
            get => _selectedState;
            set
            {
                // Normalize null/empty to "All"
                var newValue = string.IsNullOrWhiteSpace(value) ? "All" : value;

                if (_selectedState != newValue)
                {
                    _selectedState = newValue;
                    OnPropertyChanged();
                    UpdateAvailableDistricts();
                    ApplyFilters();
                }
            }
        }

        public string SelectedDistrict
        {
            get => _selectedDistrict;
            set
            {
                // Normalize null/empty to "All"
                var newValue = string.IsNullOrWhiteSpace(value) ? "All" : value;

                if (_selectedDistrict != newValue)
                {
                    _selectedDistrict = newValue;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }


        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasLoaded
        {
            get => _hasLoaded;
            set
            {
                if (_hasLoaded != value)
                {
                    _hasLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                if (_selectedLevel != value)
                {
                    _selectedLevel = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SearchSummary
        {
            get => _searchSummary;
            set
            {
                if (_searchSummary != value)
                {
                    _searchSummary = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand FilterCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RepresentativesViewModel(IRepresentativeService representativeService)
        {
            _representativeService = representativeService;

            SearchCommand = new Command(ExecuteSearch);
            FilterCommand = new Command<string>(ExecuteFilter);
        }

        public async Task LoadRepresentativesAsync(string stateCode)
        {
            if (HasLoaded)
                return;

            try
            {
                IsLoading = true;

                Representatives.Clear();
                FilteredRepresentatives.Clear();
                SearchSummary = string.Empty;

                // ✅ Get ALL reps from your own API/DB
                var reps = await _representativeService.GetRepresentativesAsync(
                    stateCode: null,
                    level: null,
                    search: null);

                foreach (var r in reps)
                {
                    Representatives.Add(r);
                }

                BuildAvailableStates();
                UpdateAvailableDistricts();

                // If caller passed "FL", set that as default selection
                if (!string.IsNullOrWhiteSpace(stateCode) &&
                    AvailableStates.Contains(stateCode.ToUpperInvariant()))
                {
                    SelectedState = stateCode.ToUpperInvariant();
                    UpdateAvailableDistricts();
                }

                ApplyFilters();
                HasLoaded = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading reps: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }


        private void ExecuteSearch()
        {
            ApplyFilters();
        }

        private void ExecuteFilter(string? level)
        {
            if (!string.IsNullOrWhiteSpace(level))
            {
                SelectedLevel = level;
                ApplyFilters();
            }
        }

        private void ApplyFilters()
        {
            // If Representatives is somehow null, just treat it as empty
            var query = (Representatives ?? new ObservableCollection<Representative>())
                .AsEnumerable();

            // 🔍 Search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim().ToLowerInvariant();

                query = query.Where(r =>
                    (!string.IsNullOrEmpty(r.Name) && r.Name.ToLowerInvariant().Contains(term)) ||
                    (!string.IsNullOrEmpty(r.Title) && r.Title.ToLowerInvariant().Contains(term)) ||
                    (!string.IsNullOrEmpty(r.District) && r.District.ToLowerInvariant().Contains(term)));
            }

            // 🌎 State filter
            var stateFilter = string.IsNullOrWhiteSpace(SelectedState) ? "All" : SelectedState;

            if (!stateFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var state = stateFilter.Trim();

                query = query.Where(r =>
                    !string.IsNullOrEmpty(r.District) &&
                    (
                        r.District.StartsWith(state + "-", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(r.District, state, StringComparison.OrdinalIgnoreCase)
                    ));
            }

            // 🗺 District filter
            var districtFilter = string.IsNullOrWhiteSpace(SelectedDistrict) ? "All" : SelectedDistrict;

            if (!districtFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                var dist = districtFilter.Trim();

                query = query.Where(r =>
                    !string.IsNullOrEmpty(r.District) &&
                    (
                        r.District.EndsWith("-" + dist, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(r.District, dist, StringComparison.OrdinalIgnoreCase)
                    ));
            }

            var list = query.ToList();

            FilteredRepresentatives.Clear();
            foreach (var r in list)
            {
                FilteredRepresentatives.Add(r);
            }

            SearchSummary = list.Count == 1
                ? "1 representative"
                : $"{list.Count} representatives";
        }


        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BuildAvailableStates()
        {
            AvailableStates.Clear();
            AvailableStates.Add("All");

            var stateCodes = Representatives
                .Select(r =>
                {
                    // Districts look like "FL-2" or just "FL"
                    if (string.IsNullOrWhiteSpace(r.District))
                        return null;

                    var parts = r.District.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    return parts[0].Trim(); // "FL"
                })
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase);

            foreach (var s in stateCodes)
            {
                if (s != null)
                    AvailableStates.Add(s);
            }

            // default
            SelectedState = "All";
        }

        private void UpdateAvailableDistricts()
        {
            AvailableDistricts.Clear();
            AvailableDistricts.Add("All");

            IEnumerable<Representative> source = Representatives;

            if (!string.Equals(SelectedState, "All", StringComparison.OrdinalIgnoreCase))
            {
                source = source.Where(r =>
                    !string.IsNullOrEmpty(r.District) &&
                    (r.District.StartsWith(SelectedState + "-", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(r.District, SelectedState, StringComparison.OrdinalIgnoreCase)));
            }

            var districts = source
                .Select(r =>
                {
                    if (string.IsNullOrWhiteSpace(r.District))
                        return null;

                    var parts = r.District.Split('-', StringSplitOptions.RemoveEmptyEntries);
                    // if "FL-2" -> "2", if "FL" -> null
                    return parts.Length > 1 ? parts[1].Trim() : null;
                })
                .Where(d => !string.IsNullOrWhiteSpace(d))
                .Distinct()
                .OrderBy(d => d, StringComparer.OrdinalIgnoreCase);

            foreach (var d in districts)
            {
                if (d != null)
                    AvailableDistricts.Add(d);
            }

            SelectedDistrict = "All";
        }

    }
}
