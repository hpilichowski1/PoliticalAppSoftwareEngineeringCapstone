using PoliticalApp.Models;

namespace PoliticalApp.Services
{
    public interface IProfileService
    {
        Task<ProfileInfo?> GetProfileAsync();
        Task<bool> UpdateLocationAsync(string state, string region);
    }
}
