using System.Collections.Generic;
using System.Threading.Tasks;
using PoliticalAppAPI.Models;

namespace PoliticalAppAPI.Services
{
    public interface IRepresentativeSyncService
    {
        /// <summary>
        /// Get all current members for a state.
        /// Uses DB cache; if missing or stale it refreshes from Congress.gov.
        /// </summary>
        Task<List<Representative>> GetOrRefreshByStateAsync(string stateCode);

        /// <summary>
        /// Get all current members across all states (first page only for now).
        /// Uses cache similarly.
        /// </summary>
        Task<List<Representative>> GetOrRefreshAllAsync();
    }
}
