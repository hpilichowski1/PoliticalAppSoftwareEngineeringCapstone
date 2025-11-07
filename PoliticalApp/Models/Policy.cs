using System;

namespace PoliticalApp.Models;

public class Policy
{
    /// <summary>
    /// Gets or sets the unique identifier for the policy.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the official bill number (e.g., "HB 1234", "SB 567").
    /// </summary>
    public string BillNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the short title of the bill.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AI-generated plain-language summary of the bill.
    /// </summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current status of the bill (e.g., "In Committee", "Passed House").
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the bill was introduced.
    /// </summary>
    public DateTime IntroducedDate { get; set; }

    /// <summary>
    /// Gets or sets the government level of the bill (e.g., "City", "State", "Federal").
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the policy category of the bill (e.g., "Education", "Healthcare").
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the brief description of the potential impacts of the bill.
    /// </summary>
    public string ImpactSummary { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the primary sponsor of the bill.
    /// </summary>
    public string SponsorName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's simulated vote on the policy, or null if not voted yet.
    /// </summary>
    public VoteChoice? UserVote { get; set; }

    /// <summary>
    /// Gets or sets the deadline for simulated voting, or null if not applicable.
    /// </summary>
    public DateTime? VoteDeadline { get; set; }
}
