namespace PoliticalAppAPI.Models
{
    public class VoteRecord
    {
        public long VoteId { get; set; }
        public string RepId { get; set; } = "";
        public Representative Rep { get; set; } = null!;
        public string BillId { get; set; } = "";
        public Legislation Bill { get; set; } = null!;
        public string Vote { get; set; } = "YEA"; // enum in DB
        public DateTime VotedAt { get; set; }
    }

    public class VoteSimulation
    {
        public string SimulationId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = "";
        public User User { get; set; } = null!;
        public string BillId { get; set; } = "";
        public Legislation Bill { get; set; } = null!;
        public string SelectedVote { get; set; } = "YEA";
        public string? ComparedRepId { get; set; }
        public Representative? ComparedRep { get; set; }
        public string? CompareResult { get; set; } // MATCH/DIFFER/N/A
    }
}