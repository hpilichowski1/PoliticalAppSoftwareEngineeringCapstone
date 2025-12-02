using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliticalAppAPI.Models
{
    [Table("representatives")]
    public class Representative
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // Unique member identifier from Congress.gov
        [Required, Column("bioguide_id")]
        public string BioguideId { get; set; } = string.Empty;

        [Required, Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required, Column("party")]
        public string Party { get; set; } = string.Empty;

        // 2-letter postal code, e.g., "FL"
        [Required, Column("state_code")]
        [MaxLength(2)]
        public string StateCode { get; set; } = string.Empty;

        // Full state/territory name, e.g., "Florida"
        [Required, Column("state_name")]
        [MaxLength(64)]
        public string StateName { get; set; } = string.Empty;

        // Nullable because Senators do not have districts
        [Column("district_number")]
        public int? DistrictNumber { get; set; }

        [Required, Column("chamber")]
        public string Chamber { get; set; } = string.Empty; // "Senate" or "House"

        [Column("start_year")]
        public int StartYear { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; } = null;

        // Used for caching — refresh data only every X hours/days
        [Required, Column("last_updated_utc")]
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
    }
}
