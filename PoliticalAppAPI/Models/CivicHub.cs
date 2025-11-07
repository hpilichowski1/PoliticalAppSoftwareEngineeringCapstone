using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliticalAppAPI.Models
{
    // One-to-one: User (PK) ↔ CivicHub (PK=UserId)
    public class CivicHub
    {
        [Key]
        public string UserId { get; set; } = "";

        // FK = PK (one-to-one)
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CivicHubAlignedRep> AlignedReps { get; set; } = new List<CivicHubAlignedRep>();
        public ICollection<CivicHubCuratedNews> CuratedNews { get; set; } = new List<CivicHubCuratedNews>();
    }

    // Join: CivicHub(UserId) ↔ Representative(RepId) with payload (Rank, Score)
    public class CivicHubAlignedRep
    {
        // Composite key (UserId, RepId)
        public string UserId { get; set; } = "";
        public CivicHub CivicHub { get; set; } = null!;

        public string RepId { get; set; } = "";
        public Representative Rep { get; set; } = null!;

        public int? Rank { get; set; }
        public decimal? Score { get; set; }
    }

    // Join: CivicHub(UserId) ↔ NewsItem(NewsId) with payload (Score)
    public class CivicHubCuratedNews
    {
        // Composite key (UserId, NewsId)
        public string UserId { get; set; } = "";
        public CivicHub CivicHub { get; set; } = null!;

        public string NewsId { get; set; } = "";
        public NewsItem News { get; set; } = null!;

        public decimal? Score { get; set; }
    }
}
