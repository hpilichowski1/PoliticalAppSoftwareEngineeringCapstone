using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliticalAppAPI.Models
{
    public enum VoteType
    {
        None = 0,
        Up   = 1,
        Down = -1
    }

    [Table("bill_votes")]
    public class BillVote
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, Column("bill_id")]
        public int BillId { get; set; }

        [ForeignKey(nameof(BillId))]
        public Bill Bill { get; set; } = null!;

        // You can swap this to your Identity user id later if you want
        [Required, MaxLength(100)]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required, Column("vote")]
        public VoteType Vote { get; set; }

        [Required, Column("created_utc")]
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
