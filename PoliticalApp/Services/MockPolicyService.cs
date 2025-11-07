using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PoliticalApp.Models;

namespace PoliticalApp.Services;

public class MockPolicyService : IPolicyService
{
    private static readonly List<Policy> _policies =
    [
        new Policy
        {
            Id = 1,
            BillNumber = "HB 2847",
            Title = "Public School Funding Increase",
            Summary = "This bill proposes a 15% increase in per-student funding for public schools over the next two years, prioritizing rural districts with aging facilities.",
            Status = "In Committee",
            IntroducedDate = DateTime.UtcNow.AddDays(-21),
            Level = "State",
            Category = "Education",
            ImpactSummary = "Would improve teacher salaries, expand after-school programs, and reduce class sizes in under-resourced districts.",
            SponsorName = "Rep. Maria Garcia",
            VoteDeadline = DateTime.UtcNow.AddDays(15)
        },
        new Policy
        {
            Id = 2,
            BillNumber = "SB 1095",
            Title = "Community Health Navigator Program",
            Summary = "Establishes a statewide health navigator program to connect residents with preventive care, mental health services, and insurance resources.",
            Status = "Passed Senate",
            IntroducedDate = DateTime.UtcNow.AddDays(-34),
            Level = "State",
            Category = "Healthcare",
            ImpactSummary = "Expected to reduce emergency room visits and improve chronic disease management for low-income residents.",
            SponsorName = "Sen. Elijah Monroe",
            UserVote = VoteChoice.Yea,
            VoteDeadline = DateTime.UtcNow.AddDays(9)
        },
        new Policy
        {
            Id = 3,
            BillNumber = "Ordinance 22-17",
            Title = "Smart Transit Lane Expansion",
            Summary = "Adds dedicated smart lanes for buses and micromobility along major downtown corridors with adaptive traffic signals.",
            Status = "Under Review",
            IntroducedDate = DateTime.UtcNow.AddDays(-12),
            Level = "City",
            Category = "Transportation",
            ImpactSummary = "Improves commute times by up to 25% and reduces carbon emissions through synchronized traffic flow.",
            SponsorName = "Councilmember Priya Patel",
            UserVote = VoteChoice.Nay,
            VoteDeadline = DateTime.UtcNow.AddDays(20)
        },
        new Policy
        {
            Id = 4,
            BillNumber = "HB 3120",
            Title = "Urban Tree Canopy Restoration Act",
            Summary = "Funds the planting of 100,000 native trees in neighborhoods with low canopy coverage and high heat vulnerability.",
            Status = "In Committee",
            IntroducedDate = DateTime.UtcNow.AddDays(-28),
            Level = "State",
            Category = "Environment",
            ImpactSummary = "Reduces urban heat islands, improves air quality, and creates green jobs for local youth.",
            SponsorName = "Rep. Amina Hassan",
            VoteDeadline = DateTime.UtcNow.AddDays(24)
        },
        new Policy
        {
            Id = 5,
            BillNumber = "Resolution 2024-08",
            Title = "Community Safety Partnership Pilot",
            Summary = "Launches a collaborative response model pairing mental health professionals with public safety officers for non-violent crisis calls.",
            Status = "Adopted",
            IntroducedDate = DateTime.UtcNow.AddDays(-40),
            Level = "City",
            Category = "Public Safety",
            ImpactSummary = "Aims to reduce use-of-force incidents and connect residents with supportive services.",
            SponsorName = "Council President Dana Ruiz",
            UserVote = VoteChoice.Abstain,
            VoteDeadline = DateTime.UtcNow.AddDays(11)
        },
        new Policy
        {
            Id = 6,
            BillNumber = "SB 1450",
            Title = "Clean Energy Workforce Scholarship Fund",
            Summary = "Creates scholarships and apprenticeships for residents entering clean energy trades, with emphasis on displaced fossil fuel workers.",
            Status = "Passed House",
            IntroducedDate = DateTime.UtcNow.AddDays(-18),
            Level = "State",
            Category = "Environment",
            ImpactSummary = "Supports equitable transition to clean energy jobs and strengthens the regional green economy.",
            SponsorName = "Sen. Lucas Freeman",
            VoteDeadline = DateTime.UtcNow.AddDays(17)
        },
        new Policy
        {
            Id = 7,
            BillNumber = "Ordinance 24-05",
            Title = "Affordable Housing Preservation Trust",
            Summary = "Establishes a revolving trust fund to preserve at-risk affordable housing units and provide emergency repairs for senior residents.",
            Status = "Under Review",
            IntroducedDate = DateTime.UtcNow.AddDays(-9),
            Level = "City",
            Category = "Housing",
            ImpactSummary = "Prevents displacement of long-term residents and keeps housing costs stable for low-income families.",
            SponsorName = "Councilmember Jordan Lee",
            VoteDeadline = DateTime.UtcNow.AddDays(27)
        }
    ];

    public async Task<List<Policy>> GetPoliciesAsync()
    {
        await Task.Delay(500);
        return _policies.Select(policy => policy).ToList();
    }

    public Task<Policy?> GetPolicyByIdAsync(int id)
    {
        var policy = _policies.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(policy);
    }

    public Task<bool> SubmitVoteAsync(int policyId, VoteChoice vote)
    {
        var policy = _policies.FirstOrDefault(p => p.Id == policyId);
        if (policy is null)
        {
            return Task.FromResult(false);
        }

        policy.UserVote = vote;
        return Task.FromResult(true);
    }

    public Task<List<Policy>> GetUserVotedPoliciesAsync()
    {
        var votedPolicies = _policies
            .Where(p => p.UserVote.HasValue)
            .Select(policy => policy)
            .ToList();

        return Task.FromResult(votedPolicies);
    }
}
