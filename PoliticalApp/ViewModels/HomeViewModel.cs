using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoliticalApp.Models;
using PoliticalApp.Services;
using Microsoft.Maui.Controls;

namespace PoliticalApp.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly IPolicyService _policyService;
    private readonly IRepresentativeService _representativeService;

    [ObservableProperty]
    private ObservableCollection<CivicStat> stats = new();

    [ObservableProperty]
    private ObservableCollection<Policy> recentVotes = new();

    [ObservableProperty]
    private bool isLoading;

    public HomeViewModel(IPolicyService policyService, IRepresentativeService representativeService)
    {
        _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
        _representativeService = representativeService ?? throw new ArgumentNullException(nameof(representativeService));

        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;

            var votedPolicies = await _policyService.GetUserVotedPoliciesAsync() ?? new List<Policy>();

            var recent = votedPolicies
                .OrderByDescending(p => p.VoteDeadline ?? p.IntroducedDate)
                .Take(3)
                .ToList();

            RecentVotes = new ObservableCollection<Policy>(recent);

            var representatives = await _representativeService.GetRepresentativesAsync();
            var representativeTrend = representatives.Count >= 5 ? "up" : "neutral";

            Stats = new ObservableCollection<CivicStat>(new[]
            {
                new CivicStat
                {
                    Label = "Bills Voted On",
                    Value = votedPolicies.Count.ToString(),
                    Icon = "🗳️",
                    TrendDirection = votedPolicies.Count > 0 ? "up" : "neutral"
                },
                new CivicStat
                {
                    Label = "Representatives Followed",
                    Value = "5",
                    Icon = "👥",
                    TrendDirection = representativeTrend
                },
                new CivicStat
                {
                    Label = "Alignment Score",
                    Value = "78%",
                    Icon = "📊",
                    TrendDirection = "up"
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading home dashboard data: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private Task NavigateToPoliciesAsync()
    {
        return Shell.Current?.GoToAsync("//policies") ?? Task.CompletedTask;
    }

    [RelayCommand]
    private Task NavigateToRepresentativesAsync()
    {
        return Shell.Current?.GoToAsync("//representatives") ?? Task.CompletedTask;
    }
}
