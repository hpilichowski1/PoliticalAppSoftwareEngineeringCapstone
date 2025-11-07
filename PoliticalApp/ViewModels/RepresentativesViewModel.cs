using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoliticalApp.Models;
using PoliticalApp.Services;

namespace PoliticalApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Representatives page that handles representative data and filtering.
    /// </summary>
    public partial class RepresentativesViewModel : ObservableObject
    {
        private readonly IRepresentativeService _representativeService;
        private List<Representative> _allRepresentatives = new();

        [ObservableProperty]
        private ObservableCollection<Representative> filteredRepresentatives = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string selectedLevel = "All";

        [ObservableProperty]
        private bool isLoading;

        public RepresentativesViewModel(IRepresentativeService representativeService)
        {
            _representativeService = representativeService ?? throw new ArgumentNullException(nameof(representativeService));
            _ = LoadRepresentativesAsync();
        }

        /// <summary>
        /// Loads all representatives from the service.
        /// </summary>
        [RelayCommand]
        private async Task LoadRepresentativesAsync()
        {
            if (IsLoading)
            {
                return;
            }

            try
            {
                IsLoading = true;
                _allRepresentatives = await _representativeService.GetRepresentativesAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading representatives: {ex}");
                FilteredRepresentatives = new ObservableCollection<Representative>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Handles search functionality.
        /// </summary>
        [RelayCommand]
        private async Task SearchAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    ApplyFilters();
                }
                else
                {
                    var searchResults = await _representativeService.SearchRepresentativesAsync(SearchText);
                    _allRepresentatives = searchResults;
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching representatives: {ex}");
            }
        }

        /// <summary>
        /// Handles filtering by government level.
        /// </summary>
        /// <param name="level">The government level to filter by.</param>
        [RelayCommand]
        private void Filter(string level)
        {
            SelectedLevel = level;
            ApplyFilters();
        }

        /// <summary>
        /// Applies all active filters to the representative list.
        /// </summary>
        private void ApplyFilters()
        {
            IEnumerable<Representative> filtered = _allRepresentatives;

            // Apply level filter
            if (SelectedLevel != "All")
            {
                filtered = filtered.Where(r => r.Level.Equals(SelectedLevel, StringComparison.OrdinalIgnoreCase));
            }

            // Sort by consistency score (highest first) and then by name
            filtered = filtered
                .OrderByDescending(r => r.ConsistencyScore)
                .ThenBy(r => r.Name);

            FilteredRepresentatives = new ObservableCollection<Representative>(filtered);
        }

        /// <summary>
        /// Called when SearchText property changes.
        /// </summary>
        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                _ = LoadRepresentativesAsync();
            }
        }
    }
}
