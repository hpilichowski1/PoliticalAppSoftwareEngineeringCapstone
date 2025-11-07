namespace PoliticalAppAPI.Models
{
    public class Candidate
    {
        public string CandidateId { get; set; } = Guid.NewGuid().ToString();
        public string? UserId { get; set; }
        public User? User { get; set; }

        public string Name { get; set; } = "";
        public string? CampaignInfoJson { get; set; }
    }
}