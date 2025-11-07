using PoliticalApp.Models;

namespace PoliticalApp.Services;

/// <summary>
/// Provides methods for retrieving and searching representative data.
/// </summary>
public interface IRepresentativeService
{
    /// <summary>
    /// Retrieves the full collection of representatives available to the application.
    /// </summary>
    /// <returns>A task containing the list of representatives.</returns>
    Task<List<Representative>> GetRepresentativesAsync();

    /// <summary>
    /// Retrieves a single representative by their unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the representative.</param>
    /// <returns>A task containing the matching representative, or <c>null</c> if no match is found.</returns>
    Task<Representative?> GetRepresentativeByIdAsync(int id);

    /// <summary>
    /// Searches for representatives whose name, title, or district matches the provided query text.
    /// </summary>
    /// <param name="query">The search text to apply.</param>
    /// <returns>A task containing the list of representatives that match the query.</returns>
    Task<List<Representative>> SearchRepresentativesAsync(string query);
}
