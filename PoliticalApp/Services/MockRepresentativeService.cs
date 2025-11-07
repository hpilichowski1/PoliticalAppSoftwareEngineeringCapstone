using PoliticalApp.Models;

namespace PoliticalApp.Services;

/// <summary>
/// Provides a mock implementation of <see cref="IRepresentativeService"/> using in-memory data.
/// </summary>
public sealed class MockRepresentativeService : IRepresentativeService
{
    private static readonly List<Representative> Representatives =
    [
        new Representative
        {
            Id = 1,
            Name = "Sarah Johnson",
            Title = "State Senator",
            Party = "Democrat",
            District = "District 12",
            Level = "State",
            PhotoUrl = "https://example.com/images/sarah_johnson.png",
            Email = "sarah.johnson@state.gov",
            Phone = "(555) 201-4821",
            Bio = "Focused on education reform and healthcare access.",
            ConsistencyScore = 87.5,
            YearsInOffice = 8
        },
        new Representative
        {
            Id = 2,
            Name = "Michael Chen",
            Title = "City Council Member",
            Party = "Independent",
            District = "Ward 3",
            Level = "City",
            PhotoUrl = "https://example.com/images/michael_chen.png",
            Email = "mchen@citycouncil.gov",
            Phone = "(555) 889-1120",
            Bio = "Advocate for sustainable urban development.",
            ConsistencyScore = 92.0,
            YearsInOffice = 5
        },
        new Representative
        {
            Id = 3,
            Name = "James Patel",
            Title = "State Representative",
            Party = "Republican",
            District = "District 27",
            Level = "State",
            PhotoUrl = "https://example.com/images/james_patel.png",
            Email = "james.patel@statehouse.gov",
            Phone = "(555) 403-7781",
            Bio = "Championing economic development and infrastructure.",
            ConsistencyScore = 78.6,
            YearsInOffice = 4
        }
    ];

    /// <inheritdoc />
    public async Task<List<Representative>> GetRepresentativesAsync()
    {
        await Task.Delay(500);
        return Representatives.Select(rep => rep).ToList();
    }

    /// <inheritdoc />
    public Task<Representative?> GetRepresentativeByIdAsync(int id)
    {
        var representative = Representatives.FirstOrDefault(rep => rep.Id == id);
        return Task.FromResult(representative);
    }

    /// <inheritdoc />
    public Task<List<Representative>> SearchRepresentativesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.FromResult(Representatives.Select(rep => rep).ToList());
        }

        var trimmedQuery = query.Trim();

        var results = Representatives
            .Where(rep =>
                rep.Name.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase) ||
                rep.Title.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase) ||
                rep.District.Contains(trimmedQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(results);
    }
}
