using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliticalAppAPI.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        [Required, Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required, Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required, Column("role")]
        public string Role { get; set; } = "citizen";

        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("state")]
        public string? State { get; set; }      // e.g. "FL"
        [Column("region")]
        public string? Region { get; set; }
        [Column("alignment_score")]
        public int? AlignmentScore { get; set; }

    }
}