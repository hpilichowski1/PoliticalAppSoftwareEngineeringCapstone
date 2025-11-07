namespace PoliticalAppAPI.Models
{
    public class Representative
    {
        public string RepId { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public User? User { get; set; }

        public string Name { get; set; } = "";
        public string District { get; set; } = "";
        public ICollection<VoteRecord> VotingHistory { get; set; } = [];
    }
}