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
            Summary = "Proposes a 15% increase in per-student funding for public schools over the next two years.",
            Status = "In Committee",
            IntroducedDate = DateTime.UtcNow.AddDays(-21),
            Level = "State",
            Category = "Education",
            ImpactSummary = "Would improve teacher salaries and reduce class sizes.",
            SponsorName = "Rep. Maria Garcia",
            VoteDeadline = DateTime.UtcNow.AddDays(15)
        },
        new Policy
        {
            Id = 2,
            BillNumber = "SB 1095",
            Title = "Community Health Navigator Program",
            Summary = "Establishes a health navigator program to connect residents with preventive care and mental health services.",
            Status = "Passed Senate",
            IntroducedDate = DateTime.UtcNow.AddDays(-34),
            Level = "State",
            Category = "Healthcare",
            ImpactSummary = "Expected to reduce emergency room visits.",
            SponsorName = "Sen. Elijah Monroe",
            UserVote = VoteChoice.Yea,
            VoteDeadline = DateTime.UtcNow.AddDays(9)
        },
        new Policy
        {
            Id = 3,
            BillNumber = "Ordinance 22-17",
            Title = "Smart Transit Lane Expansion",
            Summary = "Adds dedicated lanes for buses along major downtown corridors.",
            Status = "Under Review",
            IntroducedDate = DateTime.UtcNow.AddDays(-12),
            Level = "City",
            Category = "Transportation",
            ImpactSummary = "Improves commute times and reduces emissions.",
            SponsorName = "Councilmember Priya Patel",
            VoteDeadline = DateTime.UtcNow.AddDays(20)
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
