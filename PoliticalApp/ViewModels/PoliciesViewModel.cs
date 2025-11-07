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
    /// ViewModel for the Policies page that handles policy data, filtering, and voting.
    /// </summary>
    public partial class PoliciesViewModel : ObservableObject
    {
        private readonly IPolicyService _policyService;
        private List<Policy> _allPolicies = new();

        [ObservableProperty]
        private ObservableCollection<Policy> filteredPolicies = new();

        [ObservableProperty]
        private string selectedCategory = "All";

        [ObservableProperty]
        private string selectedLevel = "All";

        [ObservableProperty]
        private bool isLoading;

        public PoliciesViewModel(IPolicyService policyService)
        {
            _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
            _ = LoadPoliciesAsync();
        }

        /// <summary>
        /// Loads all policies from the service.
        /// </summary>
        [RelayCommand]
        private async Task LoadPoliciesAsync()
        {
            if (IsLoading)
            {
                return;
            }

            try
            {
                IsLoading = true;
                _allPolicies = await _policyService.GetPoliciesAsync();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading policies: {ex}");
                FilteredPolicies = new ObservableCollection<Policy>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Filters policies by category.
        /// </summary>
        /// <param name="category">The category to filter by.</param>
        [RelayCommand]
        private void FilterByCategory(string category)
        {
            SelectedCategory = category;
            ApplyFilters();
        }

        /// <summary>
        /// Filters policies by government level.
        /// </summary>
        /// <param name="level">The government level to filter by.</param>
        [RelayCommand]
        private void FilterByLevel(string level)
        {
            SelectedLevel = level;
            ApplyFilters();
        }

        /// <summary>
        /// Handles voting Yea on a policy.
        /// </summary>
        /// <param name="policy">The policy to vote on.</param>
        [RelayCommand]
        private async Task VoteAsync(Policy policy)
        {
            if (policy == null)
                return;

            try
            {
                var success = await _policyService.SubmitVoteAsync(policy.Id, VoteChoice.Yea);
                if (success)
                {
                    policy.UserVote = VoteChoice.Yea;
                    // Refresh the display to show the updated vote
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting Yea vote: {ex}");
            }
        }

        /// <summary>
        /// Handles voting Nay on a policy.
        /// </summary>
        /// <param name="policy">The policy to vote on.</param>
        [RelayCommand]
        private async Task VoteNayAsync(Policy policy)
        {
            if (policy == null)
                return;

            try
            {
                var success = await _policyService.SubmitVoteAsync(policy.Id, VoteChoice.Nay);
                if (success)
                {
                    policy.UserVote = VoteChoice.Nay;
                    // Refresh the display to show the updated vote
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting Nay vote: {ex}");
            }
        }

        /// <summary>
        /// Handles abstaining from voting on a policy.
        /// </summary>
        /// <param name="policy">The policy to abstain from.</param>
        [RelayCommand]
        private async Task VoteAbstainAsync(Policy policy)
        {
            if (policy == null)
                return;

            try
            {
                var success = await _policyService.SubmitVoteAsync(policy.Id, VoteChoice.Abstain);
                if (success)
                {
                    policy.UserVote = VoteChoice.Abstain;
                    // Refresh the display to show the updated vote
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting Abstain vote: {ex}");
            }
        }

        /// <summary>
        /// Applies all active filters to the policy list.
        /// </summary>
        private void ApplyFilters()
        {
            IEnumerable<Policy> filtered = _allPolicies;

            // Apply category filter
            if (SelectedCategory != "All")
            {
                filtered = filtered.Where(p => p.Category.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            // Apply level filter
            if (SelectedLevel != "All")
            {
                filtered = filtered.Where(p => p.Level.Equals(SelectedLevel, StringComparison.OrdinalIgnoreCase));
            }

            // Sort by vote deadline (upcoming first), then by introduced date (newest first)
            filtered = filtered
                .OrderBy(p => p.VoteDeadline ?? DateTime.MaxValue)
                .ThenByDescending(p => p.IntroducedDate);

            FilteredPolicies = new ObservableCollection<Policy>(filtered);
        }
    }
}
