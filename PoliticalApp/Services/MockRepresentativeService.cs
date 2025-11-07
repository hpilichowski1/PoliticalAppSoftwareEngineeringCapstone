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
            Bio = "Focused on education reform and healthcare access, Senator Johnson chairs the State Education Committee.",
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
            Bio = "Advocate for sustainable urban development and community-driven public transportation initiatives.",
            ConsistencyScore = 92.0,
            YearsInOffice = 5
        },
        new Representative
        {
            Id = 3,
            Name = "Alicia Rodriguez",
            Title = "County Commissioner",
            Party = "Democrat",
            District = "County District 4",
            Level = "County",
            PhotoUrl = "https://example.com/images/alicia_rodriguez.png",
            Email = "arodriguez@county.gov",
            Phone = "(555) 765-4412",
            Bio = "Leads initiatives on affordable housing and equitable access to county services.",
            ConsistencyScore = 81.3,
            YearsInOffice = 6
        },
        new Representative
        {
            Id = 4,
            Name = "James Patel",
            Title = "State Representative",
            Party = "Republican",
            District = "District 27",
            Level = "State",
            PhotoUrl = "https://example.com/images/james_patel.png",
            Email = "james.patel@statehouse.gov",
            Phone = "(555) 403-7781",
            Bio = "Former small business owner championing economic development and infrastructure modernization.",
            ConsistencyScore = 78.6,
            YearsInOffice = 4
        },
        new Representative
        {
            Id = 5,
            Name = "Lauren Smith",
            Title = "City Council President",
            Party = "Republican",
            District = "At-Large",
            Level = "City",
            PhotoUrl = "https://example.com/images/lauren_smith.png",
            Email = "lauren.smith@citycouncil.gov",
            Phone = "(555) 662-9034",
            Bio = "Leads city council initiatives on public safety, budgeting transparency, and youth engagement.",
            ConsistencyScore = 84.9,
            YearsInOffice = 10
        },
        new Representative
        {
            Id = 6,
            Name = "Robert Williams",
            Title = "County Sheriff",
            Party = "Independent",
            District = "Harbor County",
            Level = "County",
            PhotoUrl = "https://example.com/images/robert_williams.png",
            Email = "rwilliams@sheriff.gov",
            Phone = "(555) 330-9901",
            Bio = "Emphasizes community policing strategies and mental health training for deputies.",
            ConsistencyScore = 73.2,
            YearsInOffice = 12
        },
        new Representative
        {
            Id = 7,
            Name = "Priya Desai",
            Title = "State Assembly Member",
            Party = "Democrat",
            District = "District 8",
            Level = "State",
            PhotoUrl = "https://example.com/images/priya_desai.png",
            Email = "priya.desai@assembly.gov",
            Phone = "(555) 284-1198",
            Bio = "Technology policy expert advocating for broadband expansion and data privacy protections.",
            ConsistencyScore = 94.1,
            YearsInOffice = 3
        },
        new Representative
        {
            Id = 8,
            Name = "Ethan Wright",
            Title = "City Council Member",
            Party = "Republican",
            District = "Ward 6",
            Level = "City",
            PhotoUrl = "https://example.com/images/ethan_wright.png",
            Email = "ewright@citycouncil.gov",
            Phone = "(555) 495-6670",
            Bio = "Former firefighter focusing on emergency response readiness and infrastructure resilience.",
            ConsistencyScore = 69.4,
            YearsInOffice = 7
        },
        new Representative
        {
            Id = 9,
            Name = "Maria Gomez",
            Title = "State Senator",
            Party = "Democrat",
            District = "District 3",
            Level = "State",
            PhotoUrl = "https://example.com/images/maria_gomez.png",
            Email = "maria.gomez@state.gov",
            Phone = "(555) 918-2245",
            Bio = "Public health advocate leading efforts to expand community clinics and mental health services.",
            ConsistencyScore = 90.7,
            YearsInOffice = 9
        },
        new Representative
        {
            Id = 10,
            Name = "Daniel Brooks",
            Title = "City Controller",
            Party = "Independent",
            District = "River City",
            Level = "City",
            PhotoUrl = "https://example.com/images/daniel_brooks.png",
            Email = "dbrooks@rivercity.gov",
            Phone = "(555) 731-5823",
            Bio = "Champion of fiscal transparency, spearheading open data initiatives and citizen budget reporting.",
            ConsistencyScore = 65.8,
            YearsInOffice = 14
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
