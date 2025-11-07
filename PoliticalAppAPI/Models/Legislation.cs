namespace PoliticalAppAPI.Models
{
    public class Legislation
    {
        public string BillId { get; set; } = "";
        public string Title { get; set; } = "";
        public string? Summary { get; set; }
        public string Status { get; set; } = "introduced";

        public ICollection<VoteRecord> VoteRecords { get; set; } = [];
        public ICollection<VoteSimulation> Simulations { get; set; } = [];
    }
}