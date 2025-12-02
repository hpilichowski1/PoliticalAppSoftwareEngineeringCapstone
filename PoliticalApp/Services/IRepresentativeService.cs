using System.Collections.Generic;
using System.Threading.Tasks;
using PoliticalApp.Models;

namespace PoliticalApp.Services
{
    public interface IRepresentativeService
    {
        Task<List<Representative>> GetRepresentativesAsync(
            string? stateCode = null,
            string? level = null,
            string? search = null);
    }
}
