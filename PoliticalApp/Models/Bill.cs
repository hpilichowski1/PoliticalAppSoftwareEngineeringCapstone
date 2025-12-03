namespace PoliticalApp.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public int Congress { get; set; }
        public string BillType { get; set; } = string.Empty;
        public int BillNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PolicyArea { get; set; }
        public string? SponsorName { get; set; }
        public DateTime? LatestActionDate { get; set; }
        public string? LatestActionText { get; set; }
        public string? SummaryText { get; set; }
    }
}
