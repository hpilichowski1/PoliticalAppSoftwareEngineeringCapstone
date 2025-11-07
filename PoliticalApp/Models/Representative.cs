namespace PoliticalApp.Models
{
    /// <summary>
    /// Represents an elected official and their key public profile details for display within the PoliticalApp app.
    /// </summary>
    public class Representative
    {
        /// <summary>
        /// Gets or sets the unique identifier for the representative.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the representative.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the official title or role (e.g., State Senator, City Council Member).
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Gets or sets the political party affiliation of the representative.
        /// </summary>
        public required string Party { get; set; }

        /// <summary>
        /// Gets or sets the district or region the representative serves.
        /// </summary>
        public required string District { get; set; }

        /// <summary>
        /// Gets or sets the level of government at which the representative serves (City, County, State, Federal).
        /// </summary>
        public required string Level { get; set; }

        /// <summary>
        /// Gets or sets the URL to the representative's profile photo, if available.
        /// </summary>
        public string? PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the preferred contact email address for the representative.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the office phone number for the representative.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the short biography or description highlighting the representative's focus areas and achievements.
        /// </summary>
        public required string Bio { get; set; }

        /// <summary>
        /// Gets or sets the consistency score (0-100) indicating how closely stated positions align with voting records.
        /// </summary>
        public double ConsistencyScore { get; set; }

        /// <summary>
        /// Gets or sets the total number of years the representative has served in public office.
        /// </summary>
        public int YearsInOffice { get; set; }
    }
}
