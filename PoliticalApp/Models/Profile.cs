namespace PoliticalApp.Models;

public class UserVote
{
    public int BillId { get; set; }
    public string BillTitle { get; set; } = string.Empty;
    public VoteType Vote { get; set; }
}

public class ProfileInfo
{
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? State { get; set; }
    public string? Region { get; set; }

    public List<UserVote> Votes { get; set; } = new();
    public int? AlignmentScore { get; set; }
}
