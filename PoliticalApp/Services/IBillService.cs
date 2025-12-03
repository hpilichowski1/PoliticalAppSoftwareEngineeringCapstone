using PoliticalApp.Models;

namespace PoliticalApp.Services
{
    public interface IBillService
    {
        Task<IReadOnlyList<Bill>> GetBillsAsync(int page, int pageSize);
    }
}
