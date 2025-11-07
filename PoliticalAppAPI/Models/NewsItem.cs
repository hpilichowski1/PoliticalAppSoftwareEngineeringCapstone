namespace PoliticalAppAPI.Models
{
    public class NewsItem
    {
        public string NewsId { get; set; } = Guid.NewGuid().ToString();
        public string Source { get; set; } = "";
        public string Url { get; set; } = "";
        public string Headline { get; set; } = "";
        public DateTime? PublishedAt { get; set; }
        public string? RawJson { get; set; }
    }
}