using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PoliticalApp.Models;
using PoliticalApp.Services;

namespace PoliticalApp.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IProfileService _profileService;
    private readonly IRepresentativeService _repService;

    public ProfileViewModel(
        IProfileService profileService,
        IRepresentativeService repService)
    {
        _profileService = profileService;
        _repService = repService;
    }

    [ObservableProperty]
    string email = string.Empty;

    [ObservableProperty]
    string? name;

    [ObservableProperty]
    string? state;

    [ObservableProperty]
    string? region;

    [ObservableProperty]
    ObservableCollection<UserVote> votes = new();

    [ObservableProperty]
    ObservableCollection<Representative> representatives = new();

    [ObservableProperty]
    bool isBusy;
    [ObservableProperty]
    int? alignmentScore;

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var profile = await _profileService.GetProfileAsync();
            if (profile == null) return;

            Email = profile.Email;
            Name = profile.Name;
            State = profile.State;
            Region = profile.Region;
            AlignmentScore = profile.AlignmentScore;

            Votes.Clear();
            foreach (var v in profile.Votes)
                Votes.Add(v);

            Representatives.Clear(); // will be filled when user presses button
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task SaveLocationAsync()
    {
        if (string.IsNullOrWhiteSpace(State) || string.IsNullOrWhiteSpace(Region))
            return;

        var ok = await _profileService.UpdateLocationAsync(State, Region);
        if (!ok) return;

        await LoadRepresentativesAsync();
    }

    [RelayCommand]
    public async Task LoadRepresentativesAsync()
    {
        if (string.IsNullOrWhiteSpace(State) || string.IsNullOrWhiteSpace(Region))
            return;

        if (IsBusy) return;
        try
        {
            IsBusy = true;

            var reps = await _repService.GetRepresentativesAsync(State, Region);
            Representatives.Clear();

            foreach (var r in reps)
                Representatives.Add(r);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
