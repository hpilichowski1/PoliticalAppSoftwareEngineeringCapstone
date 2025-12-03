using PoliticalAppAPI.Models;

namespace PoliticalAppAPI.Services
{
    public interface IBillSyncService
    {
        Task<(IReadOnlyList<Bill> Bills, int Total)> GetPagedAsync(int page, int pageSize);
        Task<string?> GetOrFetchSummaryAsync(int billId);
    }
}
