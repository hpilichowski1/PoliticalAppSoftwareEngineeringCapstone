using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliticalAppAPI.Models
{
    [Table("bills")]
    public class Bill
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required, Column("congress")]
        public int Congress { get; set; }   // e.g. 118

        [MaxLength(20)]
        [Column("bill_type")]
        public string BillType { get; set; } = string.Empty;

        [Required, Column("bill_number")]
        public int BillNumber { get; set; } // 1, 23, 1029, ...

        [Required, Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("latest_action_date")]
        public DateTime? LatestActionDate { get; set; }

        [Column("latest_action_text")]
        public string? LatestActionText { get; set; }

        [Column("sponsor_name")]
        public string? SponsorName { get; set; }

        [Column("policy_area")]
        public string? PolicyArea { get; set; }

        // Short text summary we'll fetch on demand
        [Column("summary_text")]
        public string? SummaryText { get; set; }

        [Required, Column("last_updated_utc")]
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;

        // NEW: votes for this bill
        [InverseProperty(nameof(BillVote.Bill))]
        public ICollection<BillVote> Votes { get; set; } = new List<BillVote>();
    }
}
