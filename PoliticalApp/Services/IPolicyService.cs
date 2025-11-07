using System.Collections.Generic;
using System.Threading.Tasks;
using PoliticalApp.Models;

namespace PoliticalApp.Services;

public interface IPolicyService
{
    Task<List<Policy>> GetPoliciesAsync();
    Task<Policy?> GetPolicyByIdAsync(int id);
    Task<bool> SubmitVoteAsync(int policyId, VoteChoice vote);
    Task<List<Policy>> GetUserVotedPoliciesAsync();
}
