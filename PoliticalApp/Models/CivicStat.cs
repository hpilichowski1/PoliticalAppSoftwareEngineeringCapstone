namespace PoliticalApp.Models;

/// <summary>
/// Represents a civic engagement statistic displayed on the home dashboard.
/// </summary>
public class CivicStat
{
    /// <summary>
    /// Gets or sets the display label for the statistic (e.g., "Bills Voted On").
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value associated with the statistic (e.g., "12").
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the icon name or glyph used to represent the statistic visually.
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional trend indicator for the statistic ("up", "down", "neutral").
    /// </summary>
    public string? TrendDirection { get; set; }
}
